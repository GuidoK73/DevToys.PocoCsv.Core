using Delegates;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Enumerate Csv Stream Reader over T.
    /// Properties needs to be marked with ColumnAttribute
    /// </summary>
    public sealed class CsvReader<T> : BaseCsv, IDisposable where T : class, new()
    {
        private StreamReader _StreamReader;

        private ImmutableArray<Action<object, object>> _PropertySetters;
        private ImmutableArray<Action<T, string>> _PropertySettersString;
        private ImmutableArray<Action<T, Guid>> _PropertySettersGuid;
        private ImmutableArray<Action<T, Boolean>> _PropertySettersBoolean;
        private ImmutableArray<Action<T, DateTime>> _PropertySettersDateTime;
        private ImmutableArray<Action<T, DateTimeOffset>> _PropertySettersDateTimeOffset;
        private ImmutableArray<Action<T, TimeSpan>> _PropertySettersTimeSpan;
        private ImmutableArray<Action<T, Byte>> _PropertySettersByte;
        private ImmutableArray<Action<T, SByte>> _PropertySettersSByte;
        private ImmutableArray<Action<T, Int16>> _PropertySettersInt16;
        private ImmutableArray<Action<T, Int32>> _PropertySettersInt32;
        private ImmutableArray<Action<T, Int64>> _PropertySettersInt64;
        private ImmutableArray<Action<T, Single>> _PropertySettersSingle;
        private ImmutableArray<Action<T, Decimal>> _PropertySettersDecimal;
        private ImmutableArray<Action<T, Double>> _PropertySettersDouble;
        private ImmutableArray<Action<T, UInt16>> _PropertySettersUInt16;
        private ImmutableArray<Action<T, UInt32>> _PropertySettersUInt32;
        private ImmutableArray<Action<T, UInt64>> _PropertySettersUInt64;
        private ImmutableArray<Action<T, BigInteger>> _PropertySettersBigInteger;

        private ImmutableArray<Action<T, Guid?>> _PropertySettersGuidNull;
        private ImmutableArray<Action<T, Boolean?>> _PropertySettersBooleanNull;
        private ImmutableArray<Action<T, DateTime?>> _PropertySettersDateTimeNull;
        private ImmutableArray<Action<T, DateTimeOffset?>> _PropertySettersDateTimeOffsetNull;
        private ImmutableArray<Action<T, TimeSpan?>> _PropertySettersTimeSpanNull;
        private ImmutableArray<Action<T, Byte?>> _PropertySettersByteNull;
        private ImmutableArray<Action<T, SByte?>> _PropertySettersSByteNull;
        private ImmutableArray<Action<T, Int16?>> _PropertySettersInt16Null;
        private ImmutableArray<Action<T, Int32?>> _PropertySettersInt32Null;
        private ImmutableArray<Action<T, Int64?>> _PropertySettersInt64Null;
        private ImmutableArray<Action<T, Single?>> _PropertySettersSingleNull;
        private ImmutableArray<Action<T, Decimal?>> _PropertySettersDecimalNull;
        private ImmutableArray<Action<T, Double?>> _PropertySettersDoubleNull;
        private ImmutableArray<Action<T, UInt16?>> _PropertySettersUInt16Null;
        private ImmutableArray<Action<T, UInt32?>> _PropertySettersUInt32Null;
        private ImmutableArray<Action<T, UInt64?>> _PropertySettersUInt64Null;
        private ImmutableArray<Action<T, BigInteger?>> _PropertySettersBigIntegerNull;

        private readonly List<CsvReadError> _Errors = new List<CsvReadError>();
        private readonly StringBuilder _sbValue = new StringBuilder(127);
        private int _colIndex = 0;
        private State _state = State.Normal;
        private int lineLength = 0;
        internal int _byte = 0;
        private const char _CR = '\r';
        private const char _LF = '\n';
        private const char _ESCAPE = '"';
        private int _nextByte = 0;
        private int _CurrentLine = 0;
        private InfiniteLoopQueue<long> _takeLastQueue;

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvReader(string file, char separator = ',')
        {
            _File = file;
            _Separator = separator;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvReader(Stream stream, char separator = ',')
        {
            _Stream = stream;
            _Separator = separator;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvReader(Stream stream, Encoding encoding, char separator = ',', bool detectEncodingFromByteOrderMarks = true, int buffersize = 1024)
        {
            _Stream = stream;
            _Separator = separator;
            Encoding = encoding;
            DetectEncodingFromByteOrderMarks = detectEncodingFromByteOrderMarks;
            BufferSize = buffersize;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvReader(string file, Encoding encoding, char separator = ',', bool detectEncodingFromByteOrderMarks = true, int buffersize = 1024)
        {
            _File = file;
            _Separator = separator;
            Encoding = encoding;
            DetectEncodingFromByteOrderMarks = detectEncodingFromByteOrderMarks;
            BufferSize = buffersize;
        }

        

        /// <summary>
        /// Csv Seperator to use default ','
        /// </summary>
        public char Separator
        {
            get => _Separator;
            set => _Separator = value;
        }

        /// <summary>
        /// How should empty lines be treated.
        /// </summary>
        public EmptyLineBehaviour EmptyLineBehaviour { get; set; } = EmptyLineBehaviour.DefaultInstance;

        /// <summary>
        /// Indicates there are read conversion errors.
        /// </summary>
        public bool HasErrors => _Errors.Count > 0;

        /// <summary>
        /// Returns collection of error messages.
        /// </summary>
        public IEnumerable<CsvReadError> Errors => _Errors;


        /// <summary>
        /// Indicates the stream has ended.
        /// </summary>
        public bool EndOfStream => _byte == -1;

        /// <summary>
        /// Returns current Line position.
        /// </summary>
        public int CurrentLine { get => _CurrentLine; }

        /// <summary>
        /// Indicates whether to look for byte order marks at the beginning of the file.
        /// </summary>
        public bool DetectEncodingFromByteOrderMarks { get; set; } = true;

        /// <summary>
        /// Releases all resources used by the System.IO.TextReader object.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Close();
        }

        /// <summary>
        /// Close the CSV stream reader
        /// </summary>
        public void Close()
        {
            if (_StreamReader == null)
            {
                return;
            }
            _StreamReader.BaseStream.Flush();
            _StreamReader.Close();
        }

        /// <summary>
        /// Flush all buffers.
        /// </summary>
        public void Flush()
        {
            _StreamReader.BaseStream.Flush();
        }

        private void MoveToPosition(long position)
        {
            _StreamReader.BaseStream.Position = position;
            _byte = 0;
        }

        /// <summary>
        /// Detect the separator by sampling first 10 rows. Position is moved to start after execution.
        /// </summary>
        public void DetectSeparator()
        {
            var _reader = new CsvStreamReader(_StreamReader.BaseStream);
            var _succes = CsvUtils.GetCsvSeparator(_reader, out _Separator, 10);
            if (_succes)
            {
                Separator = _Separator;
            }
            MoveToStart();
        }

        /// <summary>
        /// Moves the reader to the start position.
        /// </summary>
        public void MoveToStart()
        {
            _CurrentLine = 0;
            _byte = 0;
            _StreamReader.BaseStream.Position = 0;
        }

        /// <summary>
        /// Each iteration will read the next row from stream or file
        /// </summary>
        public IEnumerable<T> ReadAsEnumerable()
        {
            if (_StreamReader == null)
            {
                throw new IOException("Reader is closed!");
            }

            while (!EndOfStream)
            {
                yield return Read();
            }
        }

        /// <summary>
        /// Use to skip first row without materializing, usefull for skipping header.
        /// </summary>
        public void Skip(int rows = 1)
        {
            if (_StreamReader == null)
            {
                throw new IOException("Reader is closed!");
            }

            int ii = 0;

            while (!EndOfStream)
            {
                if (ii >= rows)
                {
                    break;
                }
                _Skip();
                ii++;
            }
        }

        private void _Skip()
        {
            _byte = 0;
            _nextByte = 0;
            _colIndex = 0;
            _state = State.Normal;
            
            while (true)
            {
                _byte = _StreamReader.BaseStream.ReadByte();
                if (_state == State.Normal)
                {
                    if (_byte == Separator)
                    {
                        _colIndex++;
                        continue;
                    }
                    else if (_byte == _CR)
                    {
                        _nextByte = _StreamReader.BaseStream.ReadByte();
                        _StreamReader.BaseStream.Position--;
                        if (_nextByte == _LF)
                        {
                            continue; // goes to else if (_byte == '\n')
                        }
                        // end of line.
                        _colIndex = 0;
                        _CurrentLine++;
                        break;
                    }
                    else if (_byte == _LF)
                    {
                        // end of line.
                        _colIndex = 0;
                        _CurrentLine++;
                        break;
                    }
                    else if (_byte == _ESCAPE)
                    {
                        // switch mode
                        _state = State.Escaped;
                        continue; // do not add this char. (TRIM)
                    }
                    else if (_byte == -1)
                    {
                        break; // end the while loop.
                    }
                    continue;
                }
                else if (_state == State.Escaped)
                {
                    // ',' and '\r' and "" are part of the value.
                    if (_byte == -1)
                    {
                        break; 
                    }
                    else if (_byte == _ESCAPE)
                    {
                        _state = State.Normal;
                        continue; // Move to next itteration in Normal state, do not add this char (TRIM).
                    }
                    continue;
                }
            }
        }

        /// <summary>
        /// Ensures stream is at start then skips the first row.
        /// </summary>
        public void SkipHeader()
        {
            MoveToStart();
            Skip();
        }

        /// <summary>
        /// Returns Last X records in the Csv Document. This one is much faster then ReadAsEnumerable().Last().
        /// </summary>
        public IEnumerable<T> Last(int rows = 1)
        {
            if (_StreamReader == null)
            {
                throw new IOException("Reader is closed!");
            }

            MoveToLast(rows);

            while (!EndOfStream)
            {
                yield return Read();
            }
        }

        /// <summary>
        /// Move to a last row position before number of rows.
        /// </summary>
        public void MoveToLast(int rows)
        {
            _takeLastQueue = new InfiniteLoopQueue<long>(rows);
            MoveToStart();

            _state = State.Normal;
            _byte = 0;
            _nextByte = 0;
            _colIndex = 0;

            while (true)
            {
                _byte = _StreamReader.BaseStream.ReadByte();
                if (_state == State.Normal)
                {
                    if (_byte == Separator)
                    {
                        continue;
                    }
                    else if (_byte == _CR)
                    {
                        _nextByte = _StreamReader.BaseStream.ReadByte();
                        _StreamReader.BaseStream.Position--;
                        if (_nextByte == _LF)
                        {
                            continue; // goes to else if (_byte == '\n')
                        }
                        _CurrentLine++;
                        _takeLastQueue.Add(_StreamReader.BaseStream.Position);
                        continue;
                    }
                    else if (_byte == _LF)
                    {
                        _CurrentLine++;
                        _takeLastQueue.Add(_StreamReader.BaseStream.Position);
                        continue;
                    }
                    else if (_byte == _ESCAPE)
                    {
                        _state = State.Escaped;
                        continue; // do not add this char. (TRIM)
                    }
                    else if (_byte == -1)
                    {
                        break; // end the while loop.
                    }
                    continue;
                }
                else if (_state == State.Escaped)
                {
                    // ',' and '\r' and "" are part of the value.
                    if (_byte == -1)
                    {
                        break; // end the while loop.
                    }
                    else if (_byte == _ESCAPE)
                    {
                        _state = State.Normal; // not need for special escape checking, just switch.
                        continue; 
                    }
                    continue;
                }
            }

            var _queuePosition = _takeLastQueue.GetQueue();
            _CurrentLine -= _queuePosition.Length;
            MoveToPosition(_StreamReader.BaseStream.Position); // Get first position of Queue to move to the file position of last x rows.
            _CurrentLine -= rows;
        }

        //  \r = CR(Carriage Return) → Used as a new line character in Mac OS before X
        //  \n = LF(Line Feed) → Used as a new line character in Unix/Mac OS X
        //  \r\n = CR + LF → Used as a new line character in Windows

        /// <summary>
        /// reads the CsvLine
        /// </summary>
        public T Read()
        {
            T _result = new T();
            _state = State.Normal;
            _sbValue.Length = 0; // Clear the string buffer.
            lineLength = 0;
            _byte = 0;
            _nextByte = 0;
            _colIndex = 0;
            
            for(;;)
            {
                _byte = _StreamReader.BaseStream.ReadByte();
                if (_state == State.Normal)
                {
                    if (_byte == Separator)
                    {
                        // End of field
                        if (_colIndex < _Properties.Length && _Properties[_colIndex] != null)
                        {
                            SetValue(_result);
                        }
                        _colIndex++;
                        _sbValue.Length = 0;
                        continue;
                    }
                    else if (_byte == _CR)
                    {
                        _nextByte = _StreamReader.BaseStream.ReadByte();
                        _StreamReader.BaseStream.Position--;
                        if (_nextByte == _LF)
                        {
                            continue; // skip /r (CR) let it be handled on /n (LF)
                        }
                        // end of line.
                        if (_colIndex < _Properties.Length && _Properties[_colIndex] != null)
                        {
                            SetValue(_result);
                        }
                        _colIndex = 0;
                        _sbValue.Length = 0;
                        _CurrentLine++;
                        break;
                    }
                    else if (_byte == _LF)
                    {
                        // end of line.
                        if (_colIndex < _Properties.Length && _Properties[_colIndex] != null)
                        {
                            SetValue(_result);
                        }
                        _colIndex = 0;
                        _sbValue.Length = 0;
                        _CurrentLine++;
                        break;
                    }
                    else if (_byte == _ESCAPE)
                    {
                        // switch mode
                        _state = State.Escaped;
                        continue; // do not add this char. (TRIM)
                    }
                    else if (_byte == -1)
                    {
                        // End of field
                        if (_colIndex < _Properties.Length && _Properties[_colIndex] != null)
                        {
                            SetValue(_result);
                        }
                        _sbValue.Length = 0;
                        break; // end the while loop.
                    }
                    lineLength++;
                    _sbValue.Append((char)_byte);
                    continue;
                }
                else if (_state == State.Escaped)
                {
                    // ',' and '\r' and "" are part of the value.
                    if (_byte == -1)
                    {
                        // In a proper CSV this would not occur.
                        if (_colIndex < _Properties.Length && _Properties[_colIndex] != null)
                        {                            
                            SetValue(_result);
                        }
                        _sbValue.Clear();
                        break; // end the while loop.
                    }
                    else if (_byte == _ESCAPE)
                    {
                        // " aaa "" ","bbb", "ccc""","ddd """" "
                        _nextByte = _StreamReader.BaseStream.ReadByte();
                        _StreamReader.BaseStream.Position--; // Next read will read the , and act upon it in normal state.
                        if (_nextByte == Separator || _nextByte == _CR || _nextByte == _LF)
                        {
                            // this quote is followed by a , so it ends the escape. we continue to next itteration where we read a ',' in nomral mode.
                            _state = State.Normal;
                            continue;
                        }
                        else if (_nextByte == -1)
                        {
                            if (_colIndex < _Properties.Length && _Properties[_colIndex] != null)
                            {
                                SetValue(_result);
                            }
                            break;
                        }
                        else if (_nextByte == _ESCAPE)
                        {
                            _state = State.EscapedEscape;
                            continue; // Also do not add this char, we add it when we are in EscapedEscape mode and from their we turn back to normal Escape.  (basically adding one of two)
                        }
                    }
                    lineLength++;
                    _sbValue.Append((char)_byte);
                    continue;
                }
                else if (_state == State.EscapedEscape)
                {
                    lineLength++;
                    _sbValue.Append((char)_byte);
                    _state = State.Escaped;
                    continue;
                }
            }

            if (lineLength == 0)
            {
                if (EmptyLineBehaviour == EmptyLineBehaviour.NullValue)
                {
                    return default;
                }
            }
            return _result;
        }

        private void SetValue(T targetObject)
        {
            if (_PropertyTypes[_colIndex] == NetTypeComplete.String)
            {
                if (_CustomParserString[_colIndex] == null)
                {
                    _PropertySettersString[_colIndex](targetObject, _sbValue.ToString());
                    return;
                }
                else
                {
                    try
                    {
                        String _customParserValue = (String)_CustomParserCall[_colIndex](_CustomParserString[_colIndex], new object[] { _sbValue });
                        _PropertySettersString[_colIndex](targetObject, _customParserValue);
                    }
                    catch
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                    }
                    return;
                }
            }
            else
            {
                SetValueOther(targetObject);
            }
        }

        #region Value Setters

        private void SetValueOther(T targetObject)
        {
            if (!_IsNullable[_colIndex])
            {
                SetValueOtherNonNullable(targetObject);
            }
            else
            {
                SetValueOtherNullable(targetObject);
            }
        }

        private void SetValueOtherNonNullable(T targetObject)
        {
            if (_Properties[_colIndex].PropertyType == typeof(Decimal))
            {
                SetValueDecimal(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(Int32))
            {
                SetValueInt32(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(Double))
            {
                SetValueDouble(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(DateTime))
            {
                SetValueDateTime(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(Int64))
            {
                SetValueInt64(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(Guid))
            {
                SetValueGuid(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(Single))
            {
                SetValueSingle(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(Boolean))
            {
                SetValueBoolean(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(TimeSpan))
            {
                SetValueTimeSpan(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(Int16))
            {
                SetValueInt16(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(Byte))
            {
                SetValueByte(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(DateTimeOffset))
            {
                SetValueDateTimeOffset(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType.IsEnum)
            {
                _PropertySetters[_colIndex](targetObject, Enum.Parse(_Properties[_colIndex].PropertyType, _sbValue.ToString()));
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(byte[]))
            {
                SetValueByteArray(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(SByte))
            {
                SetValueSByte(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(UInt16))
            {
                SetValueUInt16(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(UInt32))
            {
                SetValueUInt32(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(UInt64))
            {
                SetValueUInt64(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(BigInteger))
            {
                SetValueBigInteger(targetObject);
                return;
            }
        }

        private void SetValueOtherNullable(T targetObject)
        {
            if (_Properties[_colIndex].PropertyType == typeof(DateTime?))
            {
                SetValueDateTimeNull(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(Guid?))
            {
                SetValueGuidNull(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(Boolean?))
            {
                SetValueBooleanNull(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(DateTime?))
            {
                SetValueDateTimeNull(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(DateTimeOffset?))
            {
                SetValueDateTimeOffsetNull(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(TimeSpan?))
            {
                SetValueTimeSpanNull(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(Byte?))
            {
                SetValueByteNull(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(SByte?))
            {
                SetValueSByteNull(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(Int16?))
            {
                SetValueInt16Null(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(Int32?))
            {
                SetValueInt32Null(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(Int64?))
            {
                SetValueInt64Null(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(Single?))
            {
                SetValueSingleNull(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(Decimal?))
            {
                SetValueDecimalNull(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(Double?))
            {
                SetValueDoubleNull(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(UInt16?))
            {
                SetValueUInt16Null(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(UInt32?))
            {
                SetValueUInt32Null(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(UInt64?))
            {
                SetValueUInt64Null(targetObject);
                return;
            }
            else if (_Properties[_colIndex].PropertyType == typeof(BigInteger?))
            {
                SetValueBigIntegerNull(targetObject);
                return;
            }
        }

        private void SetValueDecimal(T targetObject)
        {
            if (_CustomParserDecimal[_colIndex] != null)
            {
                try
                {
                    Decimal _customParserValue = (Decimal)_CustomParserCall[_colIndex](_CustomParserDecimal[_colIndex], new object[] { _sbValue });
                    _PropertySettersDecimal[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = Decimal.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out decimal _value);
            if (succes)
            {
                _PropertySettersDecimal[_colIndex](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueInt32(T targetObject)
        {
            if (_CustomParserInt32[_colIndex] != null)
            {
                try
                {
                    Int32 _customParserValue = (Int32)_CustomParserCall[_colIndex](_CustomParserInt32[_colIndex], new object[] { _sbValue });
                    _PropertySettersInt32[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = Int32.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out int _value);
            if (succes)
            {
                _PropertySettersInt32[_colIndex](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueInt64(T targetObject)
        {
            if (_CustomParserInt64[_colIndex] != null)
            {
                try
                {
                    Int64 _customParserValue = (Int64)_CustomParserCall[_colIndex](_CustomParserInt64[_colIndex], new object[] { _sbValue });
                    _PropertySettersInt64[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = Int64.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out long _value);
            if (succes)
            {
                _PropertySettersInt64[_colIndex](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueDouble(T targetObject)
        {
            if (_CustomParserDouble[_colIndex] != null)
            {
                try
                {
                    Double _customParserValue = (Double)_CustomParserCall[_colIndex](_CustomParserDouble[_colIndex], new object[] { _sbValue });
                    _PropertySettersDouble[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = Double.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out double _value);
            if (succes)
            {
                _PropertySettersDouble[_colIndex](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueDateTime(T targetObject)
        {
            if (_CustomParserDateTime[_colIndex] != null)
            {
                try
                {
                    DateTime _customParserValue = (DateTime)_CustomParserCall[_colIndex](_CustomParserDateTime[_colIndex], new object[] { _sbValue });
                    _PropertySettersDateTime[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = DateTime.TryParse(_sbValue.ToString(), Culture, DateTimeStyles.None, out DateTime _value);
            if (succes)
            {
                _PropertySettersDateTime[_colIndex](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueGuid(T targetObject)
        {
            if (_CustomParserGuid[_colIndex] != null)
            {
                try
                {
                    Guid _customParserValue = (Guid)_CustomParserCall[_colIndex](_CustomParserGuid[_colIndex], new object[] { _sbValue });
                    _PropertySettersGuid[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = Guid.TryParse(_sbValue.ToString(), out Guid _value);
            if (succes)
            {
                _PropertySettersGuid[_colIndex](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueSingle(T targetObject)
        {
            if (_CustomParserSingle[_colIndex] != null)
            {
                try
                {
                    Single _customParserValue = (Single)_CustomParserCall[_colIndex](_CustomParserSingle[_colIndex], new object[] { _sbValue });
                    _PropertySettersSingle[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = Single.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out float _value);
            if (succes)
            {
                _PropertySettersSingle[_colIndex](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueBoolean(T targetObject)
        {
            if (_CustomParserBoolean[_colIndex] != null)
            {
                try
                {
                    bool _customParserValue = (bool)_CustomParserCall[_colIndex](_CustomParserBoolean[_colIndex], new object[] { _sbValue });
                    _PropertySettersBoolean[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = Boolean.TryParse(_sbValue.ToString(), out Boolean _value);
            if (succes)
            {
                _PropertySettersBoolean[_colIndex](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueTimeSpan(T targetObject)
        {
            if (_CustomParserTimeSpan[_colIndex] != null)
            {
                try
                {
                    TimeSpan _customParserValue = (TimeSpan)_CustomParserCall[_colIndex](_CustomParserTimeSpan[_colIndex], new object[] { _sbValue });
                    _PropertySettersTimeSpan[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = TimeSpan.TryParse(_sbValue.ToString(), Culture, out TimeSpan _value);
            if (succes)
            {
                _PropertySettersTimeSpan[_colIndex](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueInt16(T targetObject)
        {
            if (_CustomParserInt16[_colIndex] != null)
            {
                try
                {
                    Int16 _customParserValue = (Int16)_CustomParserCall[_colIndex](_CustomParserInt16[_colIndex], new object[] { _sbValue });
                    _PropertySettersInt16[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = Int16.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out Int16 _value);
            if (succes)
            {
                _PropertySettersInt16[_colIndex](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueByte(T targetObject)
        {
            if (_CustomParserByte[_colIndex] != null)
            {
                try
                {
                    Byte _customParserValue = (Byte)_CustomParserCall[_colIndex](_CustomParserByte[_colIndex], new object[] { _sbValue });
                    _PropertySettersByte[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = Byte.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out Byte _value);
            if (succes)
            {
                _PropertySettersByte[_colIndex](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueDateTimeOffset(T targetObject)
        {
            if (_CustomParserDateTimeOffset[_colIndex] != null)
            {
                try
                {
                    DateTimeOffset _customParserValue = (DateTimeOffset)_CustomParserCall[_colIndex](_CustomParserDateTimeOffset[_colIndex], new object[] { _sbValue });
                    _PropertySettersDateTimeOffset[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = DateTimeOffset.TryParse(_sbValue.ToString(), Culture, DateTimeStyles.None, out DateTimeOffset _value);
            if (succes)
            {
                _PropertySettersDateTimeOffset[_colIndex](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueByteArray(T targetObject)
        {
            byte[] _byteValue = null;
            try
            {
                _byteValue = Convert.FromBase64String(_sbValue.ToString());
            }
            catch
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
            _PropertySetters[_colIndex](targetObject, _byteValue);
        }

        private void SetValueSByte(T targetObject)
        {
            if (_CustomParserSByte[_colIndex] != null)
            {
                try
                {
                    SByte _customParserValue = (SByte)_CustomParserCall[_colIndex](_CustomParserSByte[_colIndex], new object[] { _sbValue });
                    _PropertySettersSByte[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = SByte.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out SByte _value);
            if (succes)
            {
                _PropertySettersSByte[_colIndex](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueUInt16(T targetObject)
        {
            if (_CustomParserUInt16[_colIndex] != null)
            {
                try
                {
                    UInt16 _customParserValue = (UInt16)_CustomParserCall[_colIndex](_CustomParserUInt16[_colIndex], new object[] { _sbValue });
                    _PropertySettersUInt16[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }
            bool succes = UInt16.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out UInt16 _value);
            if (succes)
            {
                _PropertySettersUInt16[_colIndex](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueUInt32(T targetObject)
        {
            if (_CustomParserUInt32[_colIndex] != null)
            {
                try
                {
                    UInt32 _customParserValue = (UInt32)_CustomParserCall[_colIndex](_CustomParserUInt32[_colIndex], new object[] { _sbValue });
                    _PropertySettersUInt32[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }
            bool succes = UInt32.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out UInt32 _value);
            if (succes)
            {
                _PropertySettersUInt32[_colIndex](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueUInt64(T targetObject)
        {
            if (_CustomParserUInt64[_colIndex] != null)
            {
                try
                {
                    UInt64 _customParserValue = (UInt64)_CustomParserCall[_colIndex](_CustomParserUInt64[_colIndex], new object[] { _sbValue });
                    _PropertySettersUInt64[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }
            bool succes = UInt64.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out UInt64 _value);
            if (succes)
            {
                _PropertySettersUInt64[_colIndex](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueBigInteger(T targetObject)
        {
            if (_CustomParserBigInteger[_colIndex] != null)
            {
                try
                {
                    BigInteger _customParserValue = (BigInteger)_CustomParserCall[_colIndex](_CustomParserBigInteger[_colIndex], new object[] { _sbValue });
                    _PropertySettersBigInteger[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }
            bool succes = BigInteger.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out BigInteger _value);
            if (succes)
            {
                _PropertySettersBigInteger[_colIndex](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueGuidNull(T targetObject)
        {
            if (_CustomParserGuidNullable[_colIndex] != null)
            {
                try
                {
                    Guid? _customParserValue = (Guid?)_CustomParserCall[_colIndex](_CustomParserGuidNullable[_colIndex], new object[] { _sbValue });
                    _PropertySettersGuidNull[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = Guid.TryParse(_valueRead, out Guid _value);
                if (succes)
                {
                    _PropertySettersGuidNull[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersGuidNull[_colIndex](targetObject, null);
            }
        }

        private void SetValueBooleanNull(T targetObject)
        {
            if (_CustomParserBooleanNullable[_colIndex] != null)
            {
                try
                {
                    bool? _customParserValue = (bool?)_CustomParserCall[_colIndex](_CustomParserBooleanNullable[_colIndex], new object[] { _sbValue });
                    _PropertySettersBooleanNull[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = Boolean.TryParse(_valueRead, out Boolean _value);
                if (succes)
                {
                    _PropertySettersBooleanNull[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersBooleanNull[_colIndex](targetObject, null);
            }
        }

        private void SetValueDateTimeNull(T targetObject)
        {
            if (_CustomParserDateTimeNullable[_colIndex] != null)
            {
                try
                {
                    DateTime? _customParserValue = (DateTime?)_CustomParserCall[_colIndex](_CustomParserDateTimeNullable[_colIndex], new object[] { _sbValue });
                    _PropertySettersDateTimeNull[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = DateTime.TryParse(_valueRead, out DateTime _value);
                if (succes)
                {
                    _PropertySettersDateTimeNull[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersDateTimeNull[_colIndex](targetObject, null);
            }
        }

        private void SetValueDateTimeOffsetNull(T targetObject)
        {
            if (_CustomParserDateTimeOffsetNullable[_colIndex] != null)
            {
                try
                {
                    DateTimeOffset? _customParserValue = (DateTimeOffset?)_CustomParserCall[_colIndex](_CustomParserDateTimeOffsetNullable[_colIndex], new object[] { _sbValue });
                    _PropertySettersDateTimeOffsetNull[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = DateTimeOffset.TryParse(_valueRead, out DateTimeOffset _value);
                if (succes)
                {
                    _PropertySettersDateTimeOffsetNull[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersDateTimeOffsetNull[_colIndex](targetObject, null);
            }
        }

        private void SetValueTimeSpanNull(T targetObject)
        {
            if (_CustomParserTimeSpanNullable[_colIndex] != null)
            {
                try
                {
                    TimeSpan? _customParserValue = (TimeSpan?)_CustomParserCall[_colIndex](_CustomParserTimeSpanNullable[_colIndex], new object[] { _sbValue });
                    _PropertySettersTimeSpanNull[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = TimeSpan.TryParse(_valueRead, out TimeSpan _value);
                if (succes)
                {
                    _PropertySettersTimeSpanNull[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersTimeSpanNull[_colIndex](targetObject, null);
            }
        }

        private void SetValueByteNull(T targetObject)
        {
            if (_CustomParserByteNullable[_colIndex] != null)
            {
                try
                {
                    byte? _customParserValue = (byte?)_CustomParserCall[_colIndex](_CustomParserByteNullable[_colIndex], new object[] { _sbValue });
                    _PropertySettersByteNull[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = Byte.TryParse(_valueRead, out Byte _value);
                if (succes)
                {
                    _PropertySettersByteNull[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersByteNull[_colIndex](targetObject, null);
            }
        }

        private void SetValueSByteNull(T targetObject)
        {
            if (_CustomParserSByteNullable[_colIndex] != null)
            {
                try
                {
                    SByte? _customParserValue = (SByte?)_CustomParserCall[_colIndex](_CustomParserSByteNullable[_colIndex], new object[] { _sbValue });
                    _PropertySettersSByteNull[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = SByte.TryParse(_valueRead, out SByte _value);
                if (succes)
                {
                    _PropertySettersSByteNull[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersSByteNull[_colIndex](targetObject, null);
            }
        }

        private void SetValueInt16Null(T targetObject)
        {
            if (_CustomParserInt16Nullable[_colIndex] != null)
            {
                try
                {
                    Int16? _customParserValue = (Int16?)_CustomParserCall[_colIndex](_CustomParserInt16Nullable[_colIndex], new object[] { _sbValue });
                    _PropertySettersInt16Null[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = Int16.TryParse(_valueRead, out Int16 _value);
                if (succes)
                {
                    _PropertySettersInt16Null[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersInt16Null[_colIndex](targetObject, null);
            }
        }

        private void SetValueInt32Null(T targetObject)
        {
            if (_CustomParserInt32Nullable[_colIndex] != null)
            {
                try
                {
                    Int32? _customParserValue = (Int32?)_CustomParserCall[_colIndex](_CustomParserInt32Nullable[_colIndex], new object[] { _sbValue });
                    _PropertySettersInt32Null[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = Int32.TryParse(_valueRead, out Int32 _value);
                if (succes)
                {
                    _PropertySettersInt32Null[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersInt32Null[_colIndex](targetObject, null);
            }
        }

        private void SetValueInt64Null(T targetObject)
        {
            if (_CustomParserInt64Nullable[_colIndex] != null)
            {
                try
                {
                    Int64? _customParserValue = (Int64?)_CustomParserCall[_colIndex](_CustomParserInt64Nullable[_colIndex], new object[] { _sbValue });
                    _PropertySettersInt64Null[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = Int64.TryParse(_valueRead, out Int64 _value);
                if (succes)
                {
                    _PropertySettersInt64Null[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersInt64Null[_colIndex](targetObject, null);
            }
        }

        private void SetValueSingleNull(T targetObject)
        {
            if (_CustomParserSingleNullable[_colIndex] != null)
            {
                try
                {
                    Single? _customParserValue = (Single?)_CustomParserCall[_colIndex](_CustomParserSingleNullable[_colIndex], new object[] { _sbValue });
                    _PropertySettersSingleNull[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = Single.TryParse(_valueRead, out Single _value);
                if (succes)
                {
                    _PropertySettersSingleNull[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersSingleNull[_colIndex](targetObject, null);
            }
        }

        private void SetValueDecimalNull(T targetObject)
        {
            if (_CustomParserDecimalNullable[_colIndex] != null)
            {
                try
                {
                    Decimal? _customParserValue = (Decimal?)_CustomParserCall[_colIndex](_CustomParserDecimalNullable[_colIndex], new object[] { _sbValue });
                    _PropertySettersDecimalNull[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = Decimal.TryParse(_valueRead, out Decimal _value);
                if (succes)
                {
                    _PropertySettersDecimalNull[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersDecimalNull[_colIndex](targetObject, null);
            }
        }

        private void SetValueDoubleNull(T targetObject)
        {
            if (_CustomParserDoubleNullable[_colIndex] != null)
            {
                try
                {
                    Double? _customParserValue = (Double?)_CustomParserCall[_colIndex](_CustomParserDoubleNullable[_colIndex], new object[] { _sbValue });
                    _PropertySettersDoubleNull[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = Double.TryParse(_valueRead, out Double _value);
                if (succes)
                {
                    _PropertySettersDoubleNull[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersDoubleNull[_colIndex](targetObject, null);
            }
        }

        private void SetValueUInt16Null(T targetObject)
        {
            if (_CustomParserUInt16Nullable[_colIndex] != null)
            {
                try
                {
                    UInt16? _customParserValue = (UInt16?)_CustomParserCall[_colIndex](_CustomParserUInt16Nullable[_colIndex], new object[] { _sbValue });
                    _PropertySettersUInt16Null[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = UInt16.TryParse(_valueRead, out UInt16 _value);
                if (succes)
                {
                    _PropertySettersUInt16Null[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersUInt16Null[_colIndex](targetObject, null);
            }
        }

        private void SetValueUInt32Null(T targetObject)
        {
            if (_CustomParserUInt32Nullable[_colIndex] != null)
            {
                try
                {
                    UInt32? _customParserValue = (UInt32?)_CustomParserCall[_colIndex](_CustomParserUInt32Nullable[_colIndex], new object[] { _sbValue });
                    _PropertySettersUInt32Null[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = UInt32.TryParse(_valueRead, out UInt32 _value);
                if (succes)
                {
                    _PropertySettersUInt32Null[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersUInt32Null[_colIndex](targetObject, null);
            }
        }

        private void SetValueUInt64Null(T targetObject)
        {
            if (_CustomParserUInt64Nullable[_colIndex] != null)
            {
                try
                {
                    UInt64? _customParserValue = (UInt64?)_CustomParserCall[_colIndex](_CustomParserUInt64Nullable[_colIndex], new object[] { _sbValue });
                    _PropertySettersUInt64Null[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = UInt64.TryParse(_valueRead, out UInt64 _value);
                if (succes)
                {
                    _PropertySettersUInt64Null[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersUInt64Null[_colIndex](targetObject, null);
            }
        }

        private void SetValueBigIntegerNull(T targetObject)
        {
            if (_CustomParserBigIntegerNullable[_colIndex] != null)
            {
                try
                {
                    BigInteger? _customParserValue = (BigInteger?)_CustomParserCall[_colIndex](_CustomParserBigIntegerNullable[_colIndex], new object[] { _sbValue });
                    _PropertySettersBigIntegerNull[_colIndex](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = BigInteger.TryParse(_valueRead, out BigInteger _value);
                if (succes)
                {
                    _PropertySettersBigIntegerNull[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersBigIntegerNull[_colIndex](targetObject, null);
            }
        }

        #endregion Value Setters

        /// <summary>
        /// Open the reader
        /// </summary>
        public void Open()
        {
            Init();

            if (_Stream == null && string.IsNullOrEmpty(_File))
            {
                throw new IOException("No file or stream specified in constructor.");
            }

            if (_Stream != null)
            {
                _StreamReader = new StreamReader(stream: _Stream, encoding: Encoding, detectEncodingFromByteOrderMarks: DetectEncodingFromByteOrderMarks, bufferSize: BufferSize);
            }
            if (!string.IsNullOrEmpty(_File))
            {
                if (!File.Exists(_File))
                {
                    throw new FileNotFoundException($"File '{_File}' not found.");
                }
#if NET60 || NET70
                var _options = new FileStreamOptions
                {
                    Access = FileAccess.Read,
                    BufferSize = _BufferSize,
                    Share = FileShare.Read,
                    Mode = FileMode.Open
                };

                _StreamReader = new StreamReader(path: _File, encoding: Encoding, detectEncodingFromByteOrderMarks: DetectEncodingFromByteOrderMarks, options: _options);
#endif
#if NET50 || NETCOREAPP3_1 || NETCOREAPP3_0 || NETCOREAPP2_2 || NETCOREAPP1_0_OR_GREATER
                _StreamReader = new StreamReader(path: _File, encoding: Encoding, detectEncodingFromByteOrderMarks: DetectEncodingFromByteOrderMarks, bufferSize: BufferSize);

#endif
            }
        }

        /// <summary>
        /// Initialize the CsvReader
        /// </summary>
        private void Init()
        {
            if (_Properties != null)
                return;

            var _type = typeof(T);

            int _max = _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index).Max();

            InitCustomCsvParseArrays(_max + 1);

            InitCsvAttribute(_type, _max + 1, ReadOrWrite.Read);

            var _properties = new PropertyInfo[_max + 1];
            var _propertyTypes = new NetTypeComplete[_max + 1];
            var _propertySetters = new Action<object, object>[_max + 1];
            var _isNullable = new Boolean[_max + 1];

            var _propertySettersString = new Action<T, String>[_max + 1];
            var _propertySettersGuid = new Action<T, Guid>[_max + 1];
            var _propertySettersBoolean = new Action<T, Boolean>[_max + 1];
            var _propertySettersDateTime = new Action<T, DateTime>[_max + 1];
            var _propertySettersDateTimeOffset = new Action<T, DateTimeOffset>[_max + 1];
            var _propertySettersTimeSpan = new Action<T, TimeSpan>[_max + 1];
            var _propertySettersByte = new Action<T, Byte>[_max + 1];
            var _propertySettersSByte = new Action<T, SByte>[_max + 1];
            var _propertySettersInt16 = new Action<T, Int16>[_max + 1];
            var _propertySettersInt32 = new Action<T, Int32>[_max + 1];
            var _propertySettersInt64 = new Action<T, Int64>[_max + 1];
            var _propertySettersSingle = new Action<T, Single>[_max + 1];
            var _propertySettersDecimal = new Action<T, Decimal>[_max + 1];
            var _propertySettersDouble = new Action<T, Double>[_max + 1];
            var _propertySettersUInt16 = new Action<T, UInt16>[_max + 1];
            var _propertySettersUInt32 = new Action<T, UInt32>[_max + 1];
            var _propertySettersUInt64 = new Action<T, UInt64>[_max + 1];
            var _propertySettersBigInteger = new Action<T, BigInteger>[_max + 1];

            var _propertySettersGuidNull = new Action<T, Guid?>[_max + 1];
            var _propertySettersBooleanNull = new Action<T, Boolean?>[_max + 1];
            var _propertySettersDateTimeNull = new Action<T, DateTime?>[_max + 1];
            var _propertySettersDateTimeOffsetNull = new Action<T, DateTimeOffset?>[_max + 1];
            var _propertySettersTimeSpanNull = new Action<T, TimeSpan?>[_max + 1];
            var _propertySettersByteNull = new Action<T, Byte?>[_max + 1];
            var _propertySettersSByteNull = new Action<T, SByte?>[_max + 1];
            var _propertySettersInt16Null = new Action<T, Int16?>[_max + 1];
            var _propertySettersInt32Null = new Action<T, Int32?>[_max + 1];
            var _propertySettersInt64Null = new Action<T, Int64?>[_max + 1];
            var _propertySettersSingleNull = new Action<T, Single?>[_max + 1];
            var _propertySettersDecimalNull = new Action<T, Decimal?>[_max + 1];
            var _propertySettersDoubleNull = new Action<T, Double?>[_max + 1];
            var _propertySettersUInt16Null = new Action<T, UInt16?>[_max + 1];
            var _propertySettersUInt32Null = new Action<T, UInt32?>[_max + 1];
            var _propertySettersUInt64Null = new Action<T, UInt64?>[_max + 1];
            var _propertySettersBigIntegerNull = new Action<T, BigInteger?>[_max + 1];

            foreach (var property in _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => new { Property = p, Index = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index, Attrib = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute) })
                )
            {
                Type propertyType = property.Property.PropertyType;

                _isNullable[property.Index] = Nullable.GetUnderlyingType(propertyType) != null;

                if (property.Attrib.CustomParserType != null)
                {
                    SetCustomParserType(property.Index, property.Attrib.CustomParserType, property.Property.Name);

                    __CustomParserCall[property.Index] = DelegateFactory.InstanceMethod(property.Attrib.CustomParserType, "Read", typeof(StringBuilder));
                }

                if (propertyType == typeof(string))
                {
                    _propertySettersString[property.Index] = DelegateFactory.PropertySet<T, string>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.String;
                }
                else if (propertyType == typeof(Guid))
                {
                    _propertySettersGuid[property.Index] = DelegateFactory.PropertySet<T, Guid>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Guid;
                }
                else if (propertyType == typeof(Boolean))
                {
                    _propertySettersBoolean[property.Index] = DelegateFactory.PropertySet<T, Boolean>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Boolean;
                }
                else if (propertyType == typeof(DateTime))
                {
                    _propertySettersDateTime[property.Index] = DelegateFactory.PropertySet<T, DateTime>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.DateTime;
                }
                else if (propertyType == typeof(DateTimeOffset))
                {
                    _propertySettersDateTimeOffset[property.Index] = DelegateFactory.PropertySet<T, DateTimeOffset>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.DateTimeOffset;
                }
                else if (propertyType == typeof(TimeSpan))
                {
                    _propertySettersTimeSpan[property.Index] = DelegateFactory.PropertySet<T, TimeSpan>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.TimeSpan;
                }
                else if (propertyType == typeof(Byte))
                {
                    _propertySettersByte[property.Index] = DelegateFactory.PropertySet<T, Byte>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Byte;
                }
                else if (propertyType == typeof(SByte))
                {
                    _propertySettersSByte[property.Index] = DelegateFactory.PropertySet<T, SByte>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.SByte;
                }
                else if (propertyType == typeof(Int16))
                {
                    _propertySettersInt16[property.Index] = DelegateFactory.PropertySet<T, Int16>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Int16;
                }
                else if (propertyType == typeof(Int32))
                {
                    _propertySettersInt32[property.Index] = DelegateFactory.PropertySet<T, Int32>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Int32;
                }
                else if (propertyType == typeof(Int64))
                {
                    _propertySettersInt64[property.Index] = DelegateFactory.PropertySet<T, Int64>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Int64;
                }
                else if (propertyType == typeof(Single))
                {
                    _propertySettersSingle[property.Index] = DelegateFactory.PropertySet<T, Single>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Single;
                }
                else if (propertyType == typeof(Decimal))
                {
                    _propertySettersDecimal[property.Index] = DelegateFactory.PropertySet<T, Decimal>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Decimal;
                }
                else if (propertyType == typeof(Double))
                {
                    _propertySettersDouble[property.Index] = DelegateFactory.PropertySet<T, Double>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Double;
                }
                else if (propertyType == typeof(UInt16))
                {
                    _propertySettersUInt16[property.Index] = DelegateFactory.PropertySet<T, UInt16>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.UInt16;
                }
                else if (propertyType == typeof(UInt32))
                {
                    _propertySettersUInt32[property.Index] = DelegateFactory.PropertySet<T, UInt32>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.UInt32;
                }
                else if (propertyType == typeof(UInt64))
                {
                    _propertySettersUInt64[property.Index] = DelegateFactory.PropertySet<T, UInt64>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.UInt64;
                }
                else if (propertyType == typeof(BigInteger))
                {
                    _propertySettersBigInteger[property.Index] = DelegateFactory.PropertySet<T, BigInteger>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.BigInteger;
                }
                else if (propertyType == typeof(Guid?))
                {
                    _propertySettersGuidNull[property.Index] = DelegateFactory.PropertySet<T, Guid?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.GuidNullable;
                }
                else if (propertyType == typeof(Boolean?))
                {
                    _propertySettersBooleanNull[property.Index] = DelegateFactory.PropertySet<T, Boolean?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.BooleanNullable;
                }
                else if (propertyType == typeof(DateTime?))
                {
                    _propertySettersDateTimeNull[property.Index] = DelegateFactory.PropertySet<T, DateTime?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.DateTimeNullable;
                }
                else if (propertyType == typeof(DateTimeOffset?))
                {
                    _propertySettersDateTimeOffsetNull[property.Index] = DelegateFactory.PropertySet<T, DateTimeOffset?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.DateTimeOffsetNullable;
                }
                else if (propertyType == typeof(TimeSpan?))
                {
                    _propertySettersTimeSpanNull[property.Index] = DelegateFactory.PropertySet<T, TimeSpan?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.TimeSpanNullable;
                }
                else if (propertyType == typeof(Byte?))
                {
                    _propertySettersByteNull[property.Index] = DelegateFactory.PropertySet<T, Byte?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.ByteNullable;
                }
                else if (propertyType == typeof(SByte?))
                {
                    _propertySettersSByteNull[property.Index] = DelegateFactory.PropertySet<T, SByte?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.SByteNullable;
                }
                else if (propertyType == typeof(Int16?))
                {
                    _propertySettersInt16Null[property.Index] = DelegateFactory.PropertySet<T, Int16?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Int16Nullable;
                }
                else if (propertyType == typeof(Int32?))
                {
                    _propertySettersInt32Null[property.Index] = DelegateFactory.PropertySet<T, Int32?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Int32Nullable;
                }
                else if (propertyType == typeof(Int64?))
                {
                    _propertySettersInt64Null[property.Index] = DelegateFactory.PropertySet<T, Int64?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Int64Nullable;
                }
                else if (propertyType == typeof(Single?))
                {
                    _propertySettersSingleNull[property.Index] = DelegateFactory.PropertySet<T, Single?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.SingleNullable;
                }
                else if (propertyType == typeof(Decimal?))
                {
                    _propertySettersDecimalNull[property.Index] = DelegateFactory.PropertySet<T, Decimal?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.DecimalNullable;
                }
                else if (propertyType == typeof(Double?))
                {
                    _propertySettersDoubleNull[property.Index] = DelegateFactory.PropertySet<T, Double?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.DoubleNullable;
                }
                else if (propertyType == typeof(UInt16?))
                {
                    _propertySettersUInt16Null[property.Index] = DelegateFactory.PropertySet<T, UInt16?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.UInt16Nullable;
                }
                else if (propertyType == typeof(UInt32?))
                {
                    _propertySettersUInt32Null[property.Index] = DelegateFactory.PropertySet<T, UInt32?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.UInt32Nullable;
                }
                else if (propertyType == typeof(UInt64?))
                {
                    _propertySettersUInt64Null[property.Index] = DelegateFactory.PropertySet<T, UInt64?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.UInt64Nullable;
                }
                else if (propertyType == typeof(BigInteger?))
                {
                    _propertySettersBigIntegerNull[property.Index] = DelegateFactory.PropertySet<T, BigInteger?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.BigIntegerNullable;
                }
                else if (propertyType == typeof(byte[]))
                {
                    _propertySetters[property.Index] = _type.PropertySet(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.ByteArray;
                }
                _properties[property.Index] = property.Property;
                _propertySetters[property.Index] = _type.PropertySet(property.Property.Name);

            }

            _IsNullable = _isNullable.ToImmutableArray();
            _Properties = _properties.ToImmutableArray();
            _PropertySetters = _propertySetters.ToImmutableArray();
            _PropertySettersString = _propertySettersString.ToImmutableArray();
            _PropertySettersGuid = _propertySettersGuid.ToImmutableArray();
            _PropertySettersBoolean = _propertySettersBoolean.ToImmutableArray();
            _PropertySettersDateTime = _propertySettersDateTime.ToImmutableArray();
            _PropertySettersDateTimeOffset = _propertySettersDateTimeOffset.ToImmutableArray();
            _PropertySettersTimeSpan = _propertySettersTimeSpan.ToImmutableArray();
            _PropertySettersByte = _propertySettersByte.ToImmutableArray();
            _PropertySettersSByte = _propertySettersSByte.ToImmutableArray();
            _PropertySettersInt16 = _propertySettersInt16.ToImmutableArray();
            _PropertySettersInt32 = _propertySettersInt32.ToImmutableArray();
            _PropertySettersInt64 = _propertySettersInt64.ToImmutableArray();
            _PropertySettersSingle = _propertySettersSingle.ToImmutableArray();
            _PropertySettersDecimal = _propertySettersDecimal.ToImmutableArray();
            _PropertySettersDouble = _propertySettersDouble.ToImmutableArray();
            _PropertySettersUInt16 = _propertySettersUInt16.ToImmutableArray();
            _PropertySettersUInt32 = _propertySettersUInt32.ToImmutableArray();
            _PropertySettersUInt64 = _propertySettersUInt64.ToImmutableArray();
            _PropertySettersBigInteger = _propertySettersBigInteger.ToImmutableArray();
            _PropertySettersGuidNull = _propertySettersGuidNull.ToImmutableArray();
            _PropertySettersBooleanNull = _propertySettersBooleanNull.ToImmutableArray();
            _PropertySettersDateTimeNull = _propertySettersDateTimeNull.ToImmutableArray();
            _PropertySettersDateTimeOffsetNull = _propertySettersDateTimeOffsetNull.ToImmutableArray();
            _PropertySettersTimeSpanNull = _propertySettersTimeSpanNull.ToImmutableArray();
            _PropertySettersByteNull = _propertySettersByteNull.ToImmutableArray();
            _PropertySettersSByteNull = _propertySettersSByteNull.ToImmutableArray();
            _PropertySettersInt16Null = _propertySettersInt16Null.ToImmutableArray();
            _PropertySettersInt32Null = _propertySettersInt32Null.ToImmutableArray();
            _PropertySettersInt64Null = _propertySettersInt64Null.ToImmutableArray();
            _PropertySettersSingleNull = _propertySettersSingleNull.ToImmutableArray();
            _PropertySettersDecimalNull = _propertySettersDecimalNull.ToImmutableArray();
            _PropertySettersDoubleNull = _propertySettersDoubleNull.ToImmutableArray();
            _PropertySettersUInt16Null = _propertySettersUInt16Null.ToImmutableArray();
            _PropertySettersUInt32Null = _propertySettersUInt32Null.ToImmutableArray();
            _PropertySettersUInt64Null = _propertySettersUInt64Null.ToImmutableArray();
            _PropertySettersBigIntegerNull = _propertySettersBigIntegerNull.ToImmutableArray();
            _PropertyTypes = _propertyTypes.ToImmutableArray();

            base.InitImmutableArray();
        }
    }
}