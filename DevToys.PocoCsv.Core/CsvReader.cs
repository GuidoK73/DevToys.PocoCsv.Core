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
        /// <summary>
        /// Property Set by contructor, either File or Stream is used.
        /// </summary>
        protected StreamReader _Stream = null;

        private ImmutableArray<Action<T, Int32>> _PropertySettersEnum;
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

        private const char _CR = '\r';
        private const char _LF = '\n';
        private const char _ESCAPE = '"';

        private readonly List<CsvReadError> _Errors = new List<CsvReadError>();
        private readonly StringBuilder _buffer = new StringBuilder(1027);
        private InfiniteLoopQueue<long> _takeLastXQueue;
        private State _state = State.Normal;
        private int _propertyCount = 0;
        private int _CurrentLine = 0;
        private int _colIndex = 0; // column index.
        private int _colPosition = -1; // char position within column
        private int _linePosition = 0;
        private int _byte = 0;
        private int _nextByte = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvReader(StreamReader stream, char separator = ',', int buffersize = 1024)
        {
            _Stream = stream;
            _Separator = separator;
            BufferSize = buffersize;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvReader(string file, char separator = ',', int buffersize = 1024)
        {
            _File = file;
            _Separator = separator;
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
        /// All properties are handled in order of property occurrence and mapped directly to their respective index. Only use when CsvWriter has this set to true as well. (ColumnAttribute is ignored.)
        /// </summary>
        public bool IgnoreColumnAttributes { get; set; } = false;

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
            if (_Stream == null)
            {
                return;
            }
            _Stream.BaseStream.Flush();
            _Stream.Close();
        }

        /// <summary>
        /// Flush all buffers.
        /// </summary>
        public void Flush() => _Stream.BaseStream.Flush();

        private void MoveToPosition(long position)
        {
            _Stream.BaseStream.Position = position;
            _Stream.BaseStream.Flush();
            _byte = 0;
        }

        /// <summary>
        /// Detect the separator by sampling first 10 rows. Position is moved to start after execution.
        /// </summary>
        public void DetectSeparator()
        {
            var _reader = new CsvStreamReader(_Stream.BaseStream);
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
            _Stream.BaseStream.Position = 0;
        }

        /// <summary>
        /// Skip 1 or more rows.
        /// </summary>
        public void Skip(int rows)
        {
            if (_Stream == null)
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
                Skip();
                ii++;
            }
        }

        /// <summary>
        /// Skip a single row.
        /// </summary>
        public void Skip()
        {
            _byte = 0;
            _colIndex = 0;
            _state = State.Normal;
            _nextByte = 0;

            for (; ; )
            {
                _byte = _Stream.Read();
                if (_state == State.Normal)
                {
                    if (_byte == _Separator)
                    {
                        _colIndex++;
                        continue;
                    }
                    else if (_byte == _CR)
                    {
                        _nextByte = _Stream.Peek();
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
        /// MoveToStart then skip first row.
        /// </summary>
        public void SkipHeader()
        {
            MoveToStart();
            Skip();
        }


        /// <summary>
        /// Each iteration will read the next row from stream or file
        /// </summary>
        public IEnumerable<T> ReadAsEnumerable()
        {
            if (_Stream == null)
            {
                throw new IOException("Reader is closed!");
            }

            while (!EndOfStream)
            {
                yield return Read();
            }
        }

        /// <summary>
        /// reads the CsvLine
        /// </summary>
        //  Called each line.
        public T Read()
        {
            T _result = new T();

        SkipEmptyLineAndReadNext:

            _state = State.Normal;
            _buffer.Length = 0; // Clear the string buffer.
            _linePosition = 0;
            _byte = 0;
            _nextByte = 0;
            _colIndex = 0;
            _colPosition = -1;


            for (; ; )
            {
                _byte = _Stream.Read();
                if (_state == State.Normal)
                {
                    if (_byte == _Separator)
                    {
                        // End of field
                        if (_colIndex < _propertyCount && _IsAssigned[_colIndex])
                        {
                            SetValue(_result);
                        }
                        _colIndex++;
                        _buffer.Length = 0;
                        continue;
                    }
                    else if (_byte == _CR)
                    {
                        // in case of CR, peek next char, when LF, then skip the CR and let newline happen on LF, otherwise newline happens on CR.
                        _nextByte = _Stream.Peek();
                        if (_nextByte == _LF)
                        {
                            continue;
                        }
                        // end of line.
                        if (_colIndex < _propertyCount && _IsAssigned[_colIndex])
                        {
                            SetValue(_result);
                        }
                        _colIndex = 0;
                        _buffer.Length = 0;
                        _CurrentLine++;
                        break;
                    }
                    else if (_byte == _LF)
                    {
                        // end of line.
                        if (_colIndex < _propertyCount && _IsAssigned[_colIndex])
                        {
                            SetValue(_result);
                        }
                        _colIndex = 0;
                        _buffer.Length = 0;
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
                        if (_colIndex < _propertyCount && _IsAssigned[_colIndex])
                        {
                            SetValue(_result);
                        }
                        _buffer.Length = 0;
                        break; // end the while loop.
                    }
                    _linePosition++;
                    _colPosition++;
                    if (_colIndex < _propertyCount)
                    {
                        AppendChar();
                    }
                    continue;
                }
                else if (_state == State.Escaped)
                {
                    // ',' and '\r' and "" are part of the value.
                    if (_byte == -1)
                    {
                        // In a proper CSV this would not occur.
                        if (_colIndex < _propertyCount && _IsAssigned[_colIndex])
                        {
                            SetValue(_result);
                        }
                        _buffer.Clear();
                        break; // end the while loop.
                    }
                    else if (_byte == _ESCAPE)
                    {
                        // " aaa "" ","bbb", "ccc""","ddd """" "
                        _nextByte = _Stream.Peek();
                        if (_nextByte == Separator || _nextByte == _CR || _nextByte == _LF)
                        {
                            // this quote is followed by a , so it ends the escape. we continue to next itteration where we read a ',' in nomral mode.
                            _state = State.Normal;
                            continue;
                        }
                        else if (_nextByte == -1)
                        {
                            if (_colIndex < _propertyCount && _IsAssigned[_colIndex])
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
                    _linePosition++;
                    _colPosition++;
                    if (_colIndex < _propertyCount)
                    {
                        AppendChar();
                    }
                    continue;
                }
                else if (_state == State.EscapedEscape)
                {
                    _linePosition++;
                    _colPosition++;
                    if (_colIndex < _propertyCount)
                    {
                        AppendChar();
                    }
                    _state = State.Escaped;
                    continue;
                }
            }

            if (_linePosition == 0)
            {
                switch (EmptyLineBehaviour)
                {
                    case EmptyLineBehaviour.NullValue:
                        return default;
                    case EmptyLineBehaviour.SkipAndReadNext:
                        if (_byte != -1)
                        {
                            goto SkipEmptyLineAndReadNext;
                        }
                        return default;
                    case EmptyLineBehaviour.LogError:
                        _Errors.Add(new CsvReadError() { ColumnIndex = 0, LineNumber = _CurrentLine });
                        return default;
                    case EmptyLineBehaviour.ThrowException:
                        throw new CsvReadException($"Could not read empty line. Linenumber: {_CurrentLine}, Reader Position: {_Stream.BaseStream.Position}.");
                }
            }
            return _result;
        }

        // Called each character.
        private void AppendChar()
        {                        
            if (_ICustomCsvParseBase[_colIndex] == null)
            {
                _buffer.Append((char)_byte);
                return;
            }
            _ICustomCsvParseBase[_colIndex].Reading(_buffer, _CurrentLine, _colIndex, _Stream.BaseStream.Position, _linePosition, _colPosition, (char)_byte);
        }

        // Called each field value. (if assigned)
        private void SetValue(T targetObject)
        {
            switch (_IsNullable[_colIndex])
            {
                case false:
                    switch (_PropertyTypes[_colIndex])
                    {
                        case NetTypeComplete.String: SetValueString(targetObject); break;
                        case NetTypeComplete.Decimal: SetValueDecimal(targetObject); break;
                        case NetTypeComplete.Int32: SetValueInt32(targetObject); break;
                        case NetTypeComplete.Double: SetValueDouble(targetObject); break;
                        case NetTypeComplete.DateTime: SetValueDateTime(targetObject); break;
                        case NetTypeComplete.Int64: SetValueInt64(targetObject); break;
                        case NetTypeComplete.Guid: SetValueGuid(targetObject); break;
                        case NetTypeComplete.Single: SetValueSingle(targetObject); break;
                        case NetTypeComplete.Boolean: SetValueBoolean(targetObject); break;
                        case NetTypeComplete.TimeSpan: SetValueTimeSpan(targetObject); break;
                        case NetTypeComplete.Int16: SetValueInt16(targetObject); break;
                        case NetTypeComplete.Byte: SetValueByte(targetObject); break;
                        case NetTypeComplete.DateTimeOffset: SetValueDateTimeOffset(targetObject); break;
                        case NetTypeComplete.SByte: SetValueSByte(targetObject); break;
                        case NetTypeComplete.UInt16: SetValueUInt16(targetObject); break;
                        case NetTypeComplete.UInt32: SetValueUInt32(targetObject); break;
                        case NetTypeComplete.UInt64: SetValueUInt64(targetObject); break;
                        case NetTypeComplete.BigInteger: SetValueBigInteger(targetObject); break;
                        case NetTypeComplete.ByteArray: SetValueByteArray(targetObject); break;
                        case NetTypeComplete.Enum: SetValueEnum(targetObject); break;
                    }
                    return;
                case true:
                    switch (_PropertyTypes[_colIndex])
                    {
                        case NetTypeComplete.DecimalNullable: SetValueDecimalNull(targetObject); break;
                        case NetTypeComplete.Int32Nullable: SetValueInt32Null(targetObject); break;
                        case NetTypeComplete.DoubleNullable: SetValueDoubleNull(targetObject); break;
                        case NetTypeComplete.DateTimeNullable: SetValueDateTimeNull(targetObject); break;
                        case NetTypeComplete.Int64Nullable: SetValueInt64Null(targetObject); break;
                        case NetTypeComplete.GuidNullable: SetValueGuidNull(targetObject); break;
                        case NetTypeComplete.SingleNullable: SetValueSingleNull(targetObject); break;
                        case NetTypeComplete.BooleanNullable: SetValueBooleanNull(targetObject); break;
                        case NetTypeComplete.TimeSpanNullable: SetValueTimeSpanNull(targetObject); break;
                        case NetTypeComplete.Int16Nullable: SetValueInt16Null(targetObject); break;
                        case NetTypeComplete.ByteNullable: SetValueByteNull(targetObject); break;
                        case NetTypeComplete.DateTimeOffsetNullable: SetValueDateTimeOffsetNull(targetObject); break;
                        case NetTypeComplete.SByteNullable: SetValueSByteNull(targetObject); break;
                        case NetTypeComplete.UInt16Nullable: SetValueUInt16Null(targetObject); break;
                        case NetTypeComplete.UInt32Nullable: SetValueUInt32Null(targetObject); break;
                        case NetTypeComplete.UInt64Nullable: SetValueUInt64Null(targetObject); break;
                        case NetTypeComplete.BigIntegerNullable: SetValueBigIntegerNull(targetObject); break;
                    }
                    return;
            }
        }

        #region Value Setters

        private void SetValueString(T targetObject)
        {
            if (_CustomParserString[_colIndex] == null)
            {
                _PropertySettersString[_colIndex](targetObject, _buffer.ToString());                
            }
            else
            {
                try
                {
                    String _customParserValue = (String)_CustomParserCall[_colIndex](_CustomParserString[_colIndex], new object[] { _buffer });
                    _PropertySettersString[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex,  ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueEnum(T targetObject)
        {
            bool succes = int.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out int _value);
            if (succes)
            {
                _PropertySettersEnum[_colIndex](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueDecimal(T targetObject)
        {
            if (_CustomParserDecimal[_colIndex] == null)
            {
                bool succes = Decimal.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out decimal _value);
                if (succes)
                {
                    _PropertySettersDecimal[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    Decimal _customParserValue = (Decimal)_CustomParserCall[_colIndex](_CustomParserDecimal[_colIndex], new object[] { _buffer });
                    _PropertySettersDecimal[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueInt32(T targetObject)
        {
            if (_CustomParserInt32[_colIndex] == null)
            {
                bool succes = Int32.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out int _value);
                if (succes)
                {
                    _PropertySettersInt32[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    Int32 _customParserValue = (Int32)_CustomParserCall[_colIndex](_CustomParserInt32[_colIndex], new object[] { _buffer });
                    _PropertySettersInt32[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueInt64(T targetObject)
        {
            if (_CustomParserInt64[_colIndex] == null)
            {
                bool succes = Int64.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out long _value);
                if (succes)
                {
                    _PropertySettersInt64[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    Int64 _customParserValue = (Int64)_CustomParserCall[_colIndex](_CustomParserInt64[_colIndex], new object[] { _buffer });
                    _PropertySettersInt64[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
                return;
            }
        }

        private void SetValueDouble(T targetObject)
        {
            if (_CustomParserDouble[_colIndex] == null)
            {
                bool succes = Double.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out double _value);
                if (succes)
                {
                    _PropertySettersDouble[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    Double _customParserValue = (Double)_CustomParserCall[_colIndex](_CustomParserDouble[_colIndex], new object[] { _buffer });
                    _PropertySettersDouble[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueDateTime(T targetObject)
        {
            if (_CustomParserDateTime[_colIndex] == null)
            {
                bool succes = DateTime.TryParse(_buffer.ToString(), Culture, DateTimeStyles.None, out DateTime _value);
                if (succes)
                {
                    _PropertySettersDateTime[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    DateTime _customParserValue = (DateTime)_CustomParserCall[_colIndex](_CustomParserDateTime[_colIndex], new object[] { _buffer });
                    _PropertySettersDateTime[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueGuid(T targetObject)
        {
            if (_CustomParserGuid[_colIndex] == null)
            {
                bool succes = Guid.TryParse(_buffer.ToString(), out Guid _value);
                if (succes)
                {
                    _PropertySettersGuid[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    Guid _customParserValue = (Guid)_CustomParserCall[_colIndex](_CustomParserGuid[_colIndex], new object[] { _buffer });
                    _PropertySettersGuid[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueSingle(T targetObject)
        {
            if (_CustomParserSingle[_colIndex] == null)
            {
                bool succes = Single.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out float _value);
                if (succes)
                {
                    _PropertySettersSingle[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    Single _customParserValue = (Single)_CustomParserCall[_colIndex](_CustomParserSingle[_colIndex], new object[] { _buffer });
                    _PropertySettersSingle[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueBoolean(T targetObject)
        {
            if (_CustomParserBoolean[_colIndex] == null)
            {
                bool succes = Boolean.TryParse(_buffer.ToString(), out Boolean _value);
                if (succes)
                {
                    _PropertySettersBoolean[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    bool _customParserValue = (bool)_CustomParserCall[_colIndex](_CustomParserBoolean[_colIndex], new object[] { _buffer });
                    _PropertySettersBoolean[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueTimeSpan(T targetObject)
        {
            if (_CustomParserTimeSpan[_colIndex] == null)
            {
                bool succes = TimeSpan.TryParse(_buffer.ToString(), Culture, out TimeSpan _value);
                if (succes)
                {
                    _PropertySettersTimeSpan[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    TimeSpan _customParserValue = (TimeSpan)_CustomParserCall[_colIndex](_CustomParserTimeSpan[_colIndex], new object[] { _buffer });
                    _PropertySettersTimeSpan[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueInt16(T targetObject)
        {
            if (_CustomParserInt16[_colIndex] == null)
            {
                bool succes = Int16.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out Int16 _value);
                if (succes)
                {
                    _PropertySettersInt16[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    Int16 _customParserValue = (Int16)_CustomParserCall[_colIndex](_CustomParserInt16[_colIndex], new object[] { _buffer });
                    _PropertySettersInt16[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }

        }

        private void SetValueByte(T targetObject)
        {
            if (_CustomParserByte[_colIndex] == null)
            {
                bool succes = Byte.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out Byte _value);
                if (succes)
                {
                    _PropertySettersByte[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    Byte _customParserValue = (Byte)_CustomParserCall[_colIndex](_CustomParserByte[_colIndex], new object[] { _buffer });
                    _PropertySettersByte[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueDateTimeOffset(T targetObject)
        {
            if (_CustomParserDateTimeOffset[_colIndex] == null)
            {
                bool succes = DateTimeOffset.TryParse(_buffer.ToString(), Culture, DateTimeStyles.None, out DateTimeOffset _value);
                if (succes)
                {
                    _PropertySettersDateTimeOffset[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    DateTimeOffset _customParserValue = (DateTimeOffset)_CustomParserCall[_colIndex](_CustomParserDateTimeOffset[_colIndex], new object[] { _buffer });
                    _PropertySettersDateTimeOffset[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueByteArray(T targetObject)
        {
            byte[] _byteValue = null;
            try
            {
                _byteValue = Convert.FromBase64String(_buffer.ToString());
            }
            catch (Exception ex)
            {
                _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
            }
            _PropertySetters[_colIndex](targetObject, _byteValue);
        }

        private void SetValueSByte(T targetObject)
        {
            if (_CustomParserSByte[_colIndex] == null)
            {
                bool succes = SByte.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out SByte _value);
                if (succes)
                {
                    _PropertySettersSByte[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    SByte _customParserValue = (SByte)_CustomParserCall[_colIndex](_CustomParserSByte[_colIndex], new object[] { _buffer });
                    _PropertySettersSByte[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueUInt16(T targetObject)
        {
            if (_CustomParserUInt16[_colIndex] == null)
            {
                bool succes = UInt16.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out UInt16 _value);
                if (succes)
                {
                    _PropertySettersUInt16[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    UInt16 _customParserValue = (UInt16)_CustomParserCall[_colIndex](_CustomParserUInt16[_colIndex], new object[] { _buffer });
                    _PropertySettersUInt16[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueUInt32(T targetObject)
        {
            if (_CustomParserUInt32[_colIndex] == null)
            {
                bool succes = UInt32.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out UInt32 _value);
                if (succes)
                {
                    _PropertySettersUInt32[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    UInt32 _customParserValue = (UInt32)_CustomParserCall[_colIndex](_CustomParserUInt32[_colIndex], new object[] { _buffer });
                    _PropertySettersUInt32[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueUInt64(T targetObject)
        {
            if (_CustomParserUInt64[_colIndex] == null)
            {
                bool succes = UInt64.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out UInt64 _value);
                if (succes)
                {
                    _PropertySettersUInt64[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    UInt64 _customParserValue = (UInt64)_CustomParserCall[_colIndex](_CustomParserUInt64[_colIndex], new object[] { _buffer });
                    _PropertySettersUInt64[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueBigInteger(T targetObject)
        {
            if (_CustomParserBigInteger[_colIndex] == null)
            {
                bool succes = BigInteger.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out BigInteger _value);
                if (succes)
                {
                    _PropertySettersBigInteger[_colIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    BigInteger _customParserValue = (BigInteger)_CustomParserCall[_colIndex](_CustomParserBigInteger[_colIndex], new object[] { _buffer });
                    _PropertySettersBigInteger[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        #endregion

        #region Value Setters Nullable

        private void SetValueGuidNull(T targetObject)
        {
            if (_CustomParserGuidNullable[_colIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = Guid.TryParse(_buffer.ToString(), out Guid _value);
                    if (succes)
                    {
                        _PropertySettersGuidNull[_colIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersGuidNull[_colIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    Guid? _customParserValue = (Guid?)_CustomParserCall[_colIndex](_CustomParserGuidNullable[_colIndex], new object[] { _buffer });
                    _PropertySettersGuidNull[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueBooleanNull(T targetObject)
        {
            if (_CustomParserBooleanNullable[_colIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = Boolean.TryParse(_buffer.ToString(), out Boolean _value);
                    if (succes)
                    {
                        _PropertySettersBooleanNull[_colIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersBooleanNull[_colIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    bool? _customParserValue = (bool?)_CustomParserCall[_colIndex](_CustomParserBooleanNullable[_colIndex], new object[] { _buffer });
                    _PropertySettersBooleanNull[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueDateTimeNull(T targetObject)
        {
            if (_CustomParserDateTimeNullable[_colIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = DateTime.TryParse(_buffer.ToString(), out DateTime _value);
                    if (succes)
                    {
                        _PropertySettersDateTimeNull[_colIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersDateTimeNull[_colIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    DateTime? _customParserValue = (DateTime?)_CustomParserCall[_colIndex](_CustomParserDateTimeNullable[_colIndex], new object[] { _buffer });
                    _PropertySettersDateTimeNull[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueDateTimeOffsetNull(T targetObject)
        {
            if (_CustomParserDateTimeOffsetNullable[_colIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = DateTimeOffset.TryParse(_buffer.ToString(), out DateTimeOffset _value);
                    if (succes)
                    {
                        _PropertySettersDateTimeOffsetNull[_colIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersDateTimeOffsetNull[_colIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    DateTimeOffset? _customParserValue = (DateTimeOffset?)_CustomParserCall[_colIndex](_CustomParserDateTimeOffsetNullable[_colIndex], new object[] { _buffer });
                    _PropertySettersDateTimeOffsetNull[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueTimeSpanNull(T targetObject)
        {
            if (_CustomParserTimeSpanNullable[_colIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = TimeSpan.TryParse(_buffer.ToString(), out TimeSpan _value);
                    if (succes)
                    {
                        _PropertySettersTimeSpanNull[_colIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersTimeSpanNull[_colIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    TimeSpan? _customParserValue = (TimeSpan?)_CustomParserCall[_colIndex](_CustomParserTimeSpanNullable[_colIndex], new object[] { _buffer });
                    _PropertySettersTimeSpanNull[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueByteNull(T targetObject)
        {
            if (_CustomParserByteNullable[_colIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = Byte.TryParse(_buffer.ToString(), out Byte _value);
                    if (succes)
                    {
                        _PropertySettersByteNull[_colIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersByteNull[_colIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    byte? _customParserValue = (byte?)_CustomParserCall[_colIndex](_CustomParserByteNullable[_colIndex], new object[] { _buffer });
                    _PropertySettersByteNull[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueSByteNull(T targetObject)
        {
            if (_CustomParserSByteNullable[_colIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = SByte.TryParse(_buffer.ToString(), out SByte _value);
                    if (succes)
                    {
                        _PropertySettersSByteNull[_colIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersSByteNull[_colIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    SByte? _customParserValue = (SByte?)_CustomParserCall[_colIndex](_CustomParserSByteNullable[_colIndex], new object[] { _buffer });
                    _PropertySettersSByteNull[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueInt16Null(T targetObject)
        {
            if (_CustomParserInt16Nullable[_colIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = Int16.TryParse(_buffer.ToString(), out Int16 _value);
                    if (succes)
                    {
                        _PropertySettersInt16Null[_colIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersInt16Null[_colIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    Int16? _customParserValue = (Int16?)_CustomParserCall[_colIndex](_CustomParserInt16Nullable[_colIndex], new object[] { _buffer });
                    _PropertySettersInt16Null[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueInt32Null(T targetObject)
        {
            if (_CustomParserInt32Nullable[_colIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = Int32.TryParse(_buffer.ToString(), out Int32 _value);
                    if (succes)
                    {
                        _PropertySettersInt32Null[_colIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersInt32Null[_colIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    Int32? _customParserValue = (Int32?)_CustomParserCall[_colIndex](_CustomParserInt32Nullable[_colIndex], new object[] { _buffer });
                    _PropertySettersInt32Null[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueInt64Null(T targetObject)
        {
            if (_CustomParserInt64Nullable[_colIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = Int64.TryParse(_buffer.ToString(), out Int64 _value);
                    if (succes)
                    {
                        _PropertySettersInt64Null[_colIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersInt64Null[_colIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    Int64? _customParserValue = (Int64?)_CustomParserCall[_colIndex](_CustomParserInt64Nullable[_colIndex], new object[] { _buffer });
                    _PropertySettersInt64Null[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueSingleNull(T targetObject)
        {
            if (_CustomParserSingleNullable[_colIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = Single.TryParse(_buffer.ToString(), out Single _value);
                    if (succes)
                    {
                        _PropertySettersSingleNull[_colIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersSingleNull[_colIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    Single? _customParserValue = (Single?)_CustomParserCall[_colIndex](_CustomParserSingleNullable[_colIndex], new object[] { _buffer });
                    _PropertySettersSingleNull[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueDecimalNull(T targetObject)
        {
            if (_CustomParserDecimalNullable[_colIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = Decimal.TryParse(_buffer.ToString(), out Decimal _value);
                    if (succes)
                    {
                        _PropertySettersDecimalNull[_colIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersDecimalNull[_colIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    Decimal? _customParserValue = (Decimal?)_CustomParserCall[_colIndex](_CustomParserDecimalNullable[_colIndex], new object[] { _buffer });
                    _PropertySettersDecimalNull[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueDoubleNull(T targetObject)
        {
            if (_CustomParserDoubleNullable[_colIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = Double.TryParse(_buffer.ToString(), out Double _value);
                    if (succes)
                    {
                        _PropertySettersDoubleNull[_colIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersDoubleNull[_colIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    Double? _customParserValue = (Double?)_CustomParserCall[_colIndex](_CustomParserDoubleNullable[_colIndex], new object[] { _buffer });
                    _PropertySettersDoubleNull[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueUInt16Null(T targetObject)
        {
            if (_CustomParserUInt16Nullable[_colIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = UInt16.TryParse(_buffer.ToString(), out UInt16 _value);
                    if (succes)
                    {
                        _PropertySettersUInt16Null[_colIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersUInt16Null[_colIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    UInt16? _customParserValue = (UInt16?)_CustomParserCall[_colIndex](_CustomParserUInt16Nullable[_colIndex], new object[] { _buffer });
                    _PropertySettersUInt16Null[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueUInt32Null(T targetObject)
        {
            if (_CustomParserUInt32Nullable[_colIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = UInt32.TryParse(_buffer.ToString(), out UInt32 _value);
                    if (succes)
                    {
                        _PropertySettersUInt32Null[_colIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersUInt32Null[_colIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    UInt32? _customParserValue = (UInt32?)_CustomParserCall[_colIndex](_CustomParserUInt32Nullable[_colIndex], new object[] { _buffer });
                    _PropertySettersUInt32Null[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueUInt64Null(T targetObject)
        {
            if (_CustomParserUInt64Nullable[_colIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = UInt64.TryParse(_buffer.ToString(), out UInt64 _value);
                    if (succes)
                    {
                        _PropertySettersUInt64Null[_colIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersUInt64Null[_colIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    UInt64? _customParserValue = (UInt64?)_CustomParserCall[_colIndex](_CustomParserUInt64Nullable[_colIndex], new object[] { _buffer });
                    _PropertySettersUInt64Null[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueBigIntegerNull(T targetObject)
        {
            if (_CustomParserBigIntegerNullable[_colIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = BigInteger.TryParse(_buffer.ToString(), out BigInteger _value);
                    if (succes)
                    {
                        _PropertySettersBigIntegerNull[_colIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersBigIntegerNull[_colIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    BigInteger? _customParserValue = (BigInteger?)_CustomParserCall[_colIndex](_CustomParserBigIntegerNullable[_colIndex], new object[] { _buffer });
                    _PropertySettersBigIntegerNull[_colIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _colIndex, PropertyName = _Properties[_colIndex].Name, PropertyType = _Properties[_colIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
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
            if (!string.IsNullOrEmpty(_File))
            {
                if (!File.Exists(_File))
                {
                    throw new FileNotFoundException($"File '{_File}' not found.");
                }
                _Stream = new StreamReader(path: _File, detectEncodingFromByteOrderMarks: true);
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

            var _propertyAttributeCollection = _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => new { 
                    Property = p, 
                    Index = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index, 
                    Attrib = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute) 
                });

            if (IgnoreColumnAttributes == true)
            {
                _propertyAttributeCollection = _type.GetProperties()
                    .Select((value, index) => new { 
                        Property = value, 
                        Index = index, 
                        Attrib = new ColumnAttribute() { Index = index } 
                    });
            }

            int _max = _propertyAttributeCollection.Select(p => p.Index).Max();

            InitCustomCsvParseArrays(_max + 1);
            InitCsvAttributeRead(_type, _max + 1);

            var _properties = new PropertyInfo[_max + 1];
            var _propertyTypes = new NetTypeComplete[_max + 1];
            var _propertySetters = new Action<object, object>[_max + 1];
            var _isNullable = new Boolean[_max + 1];
            var _isAssigned = new Boolean[_max + 1];
            var _propertySettersEnum = new Action<T, Int32>[_max + 1];
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

            foreach (var property in _propertyAttributeCollection)
            {
                Type propertyType = property.Property.PropertyType;

                _isNullable[property.Index] = Nullable.GetUnderlyingType(propertyType) != null;
                _isAssigned[property.Index] = true;

                if (property.Attrib.CustomParserType != null)
                {
                    SetCustomParserType(property.Index, property.Attrib.CustomParserType, property.Property.Name);
                    __CustomParserCall[property.Index] = DelegateFactory.InstanceMethod(property.Attrib.CustomParserType, "Read", typeof(StringBuilder));
                    __ICustomCsvParseBase[property.Index] = __CustomParserString[property.Index];
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
                else if (propertyType.IsEnum)
                {
                    _propertySettersEnum[property.Index] = DelegateFactory.PropertySet<T, int>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Enum;
                }
                else
                {
                    throw new CsvException($"Property: {property.Property} Type: {propertyType.Name} not supported.");
                }
                
                _properties[property.Index] = property.Property;
                _propertySetters[property.Index] = _type.PropertySet(property.Property.Name);
            }


            _propertyCount = _properties.Length;

            _IsNullable = _isNullable.ToImmutableArray();
            _IsAssigned = _isAssigned.ToImmutableArray();
            _Properties = _properties.ToImmutableArray();
            _PropertySettersEnum = _propertySettersEnum.ToImmutableArray();
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

        private void InitCsvAttributeRead(Type type, int size)
        {
            _CsvAttribute = type.GetCustomAttribute<CsvAttribute>();
            if (_CsvAttribute == null)
            {
                return;
            }

            for (int index = 0; index < size; index++)
            {
                if (_CsvAttribute.DefaultCustomParserTypeString != null)
                {
                    __CustomParserString[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeString) as ICustomCsvParse<string>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeString, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeGuid != null)
                {
                    __CustomParserGuid[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeGuid) as ICustomCsvParse<Guid>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeGuid, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeBoolean != null)
                {
                    __CustomParserBoolean[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeBoolean) as ICustomCsvParse<Boolean>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBoolean, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeDateTime != null)
                {
                    __CustomParserDateTime[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDateTime) as ICustomCsvParse<DateTime>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTime, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeDateTimeOffset != null)
                {
                    __CustomParserDateTimeOffset[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDateTimeOffset) as ICustomCsvParse<DateTimeOffset>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTimeOffset, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeTimeSpan != null)
                {
                    __CustomParserTimeSpan[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeTimeSpan) as ICustomCsvParse<TimeSpan>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeTimeSpan, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeByte != null)
                {
                    __CustomParserByte[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeByte) as ICustomCsvParse<Byte>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeByte, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeSByte != null)
                {
                    __CustomParserSByte[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeSByte) as ICustomCsvParse<SByte>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSByte, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt16 != null)
                {
                    __CustomParserInt16[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt16) as ICustomCsvParse<Int16>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt16, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt32 != null)
                {
                    __CustomParserInt32[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt32) as ICustomCsvParse<Int32>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt32, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt64 != null)
                {
                    __CustomParserInt64[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt64) as ICustomCsvParse<Int64>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt64, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeSingle != null)
                {
                    __CustomParserSingle[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeSingle) as ICustomCsvParse<Single>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSingle, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeDecimal != null)
                {
                    __CustomParserDecimal[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDecimal) as ICustomCsvParse<Decimal>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDecimal, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeDouble != null)
                {
                    __CustomParserDouble[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDouble) as ICustomCsvParse<Double>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDouble, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt16 != null)
                {
                    __CustomParserUInt16[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt16) as ICustomCsvParse<UInt16>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt16, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt32 != null)
                {
                    __CustomParserUInt32[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt32) as ICustomCsvParse<UInt32>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt32, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt64 != null)
                {
                    __CustomParserUInt64[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt64) as ICustomCsvParse<UInt64>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt64, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeBigInteger != null)
                {
                    __CustomParserBigInteger[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeBigInteger) as ICustomCsvParse<BigInteger>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBigInteger, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeGuidNullable != null)
                {
                    __CustomParserGuidNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeGuidNullable) as ICustomCsvParse<Guid?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeGuidNullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeBooleanNullable != null)
                {
                    __CustomParserBooleanNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeBooleanNullable) as ICustomCsvParse<Boolean?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBooleanNullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeDateTimeNullable != null)
                {
                    __CustomParserDateTimeNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDateTimeNullable) as ICustomCsvParse<DateTime?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTimeNullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeDateTimeOffsetNullable != null)
                {
                    __CustomParserDateTimeOffsetNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDateTimeOffsetNullable) as ICustomCsvParse<DateTimeOffset?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTimeOffsetNullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeTimeSpanNullable != null)
                {
                    __CustomParserTimeSpanNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeTimeSpanNullable) as ICustomCsvParse<TimeSpan?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeTimeSpanNullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeByteNullable != null)
                {
                    __CustomParserByteNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeByteNullable) as ICustomCsvParse<Byte?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeByteNullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeSByteNullable != null)
                {
                    __CustomParserSByteNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeSByteNullable) as ICustomCsvParse<SByte?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSByteNullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt16Nullable != null)
                {
                    __CustomParserInt16Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt16Nullable) as ICustomCsvParse<Int16?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt16Nullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt32Nullable != null)
                {
                    __CustomParserInt32Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt32Nullable) as ICustomCsvParse<Int32?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt32Nullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt64Nullable != null)
                {
                    __CustomParserInt64Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt64Nullable) as ICustomCsvParse<Int64?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt64Nullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeSingleNullable != null)
                {
                    __CustomParserSingleNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeSingleNullable) as ICustomCsvParse<Single?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSingleNullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeDecimalNullable != null)
                {
                    __CustomParserDecimalNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDecimalNullable) as ICustomCsvParse<Decimal?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDecimalNullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeDoubleNullable != null)
                {
                    __CustomParserDoubleNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDoubleNullable) as ICustomCsvParse<Double?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDoubleNullable, "Read", typeof(StringBuilder));

                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt16Nullable != null)
                {
                    __CustomParserUInt16Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt16Nullable) as ICustomCsvParse<UInt16?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt16Nullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt32Nullable != null)
                {
                    __CustomParserUInt32Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt32Nullable) as ICustomCsvParse<UInt32?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt32Nullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt64Nullable != null)
                {
                    __CustomParserUInt64Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt64Nullable) as ICustomCsvParse<UInt64?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt64Nullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeBigIntegerNullable != null)
                {
                    __CustomParserBigIntegerNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeBigIntegerNullable) as ICustomCsvParse<BigInteger?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBigIntegerNullable, "Read", typeof(StringBuilder));
                }
            }
        }
    }
}