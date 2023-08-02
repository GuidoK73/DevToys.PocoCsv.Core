using Delegates;
using System;
using System.Collections.Generic;
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
    public sealed class CsvReader<T> : BaseCsv, IDisposable where T : new()
    {
        private StreamReader _StreamReader;
        private readonly CsvStreamHelper _StreamHelper = new CsvStreamHelper();

        private Action<object, object>[] _PropertySetters = null;
        private Action<T, string>[] _PropertySettersString = null;
        private Action<T, Guid>[] _PropertySettersGuid = null;
        private Action<T, Boolean>[] _PropertySettersBoolean = null;
        private Action<T, DateTime>[] _PropertySettersDateTime = null;
        private Action<T, DateTimeOffset>[] _PropertySettersDateTimeOffset = null;
        private Action<T, TimeSpan>[] _PropertySettersTimeSpan = null;
        private Action<T, Byte>[] _PropertySettersByte = null;
        private Action<T, SByte>[] _PropertySettersSByte = null;
        private Action<T, Int16>[] _PropertySettersInt16 = null;
        private Action<T, Int32>[] _PropertySettersInt32 = null;
        private Action<T, Int64>[] _PropertySettersInt64 = null;
        private Action<T, Single>[] _PropertySettersSingle = null;
        private Action<T, Decimal>[] _PropertySettersDecimal = null;
        private Action<T, Double>[] _PropertySettersDouble = null;
        private Action<T, UInt16>[] _PropertySettersUInt16 = null;
        private Action<T, UInt32>[] _PropertySettersUInt32 = null;
        private Action<T, UInt64>[] _PropertySettersUInt64 = null;
        private Action<T, BigInteger>[] _PropertySettersBigInteger = null;

        private Action<T, Guid?>[] _PropertySettersGuidNull = null;
        private Action<T, Boolean?>[] _PropertySettersBooleanNull = null;
        private Action<T, DateTime?>[] _PropertySettersDateTimeNull = null;
        private Action<T, DateTimeOffset?>[] _PropertySettersDateTimeOffsetNull = null;
        private Action<T, TimeSpan?>[] _PropertySettersTimeSpanNull = null;
        private Action<T, Byte?>[] _PropertySettersByteNull = null;
        private Action<T, SByte?>[] _PropertySettersSByteNull = null;
        private Action<T, Int16?>[] _PropertySettersInt16Null = null;
        private Action<T, Int32?>[] _PropertySettersInt32Null = null;
        private Action<T, Int64?>[] _PropertySettersInt64Null = null;
        private Action<T, Single?>[] _PropertySettersSingleNull = null;
        private Action<T, Decimal?>[] _PropertySettersDecimalNull = null;
        private Action<T, Double?>[] _PropertySettersDoubleNull = null;
        private Action<T, UInt16?>[] _PropertySettersUInt16Null = null;
        private Action<T, UInt32?>[] _PropertySettersUInt32Null = null;
        private Action<T, UInt64?>[] _PropertySettersUInt64Null = null;
        private Action<T, BigInteger?>[] _PropertySettersBigIntegerNull = null;

        private readonly List<CsvReadError> _Errors = new List<CsvReadError>();
        private readonly StringBuilder _sbValue = new StringBuilder(127);
        private char _char;
        private const char _CR = '\r';
        private const char _LF = '\n';
        private const char _ESCAPE = '"';

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvReader(string file)
        {
            _File = file;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvReader(Stream stream)
        {
            _Stream = stream;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvReader(Stream stream, Encoding encoding, char separator = ',', bool detectEncodingFromByteOrderMarks = true, int buffersize = 1024)
        {
            _Stream = stream;
            _StreamHelper.Separator = separator;
            Encoding = encoding;
            DetectEncodingFromByteOrderMarks = detectEncodingFromByteOrderMarks;
            _BufferSize = buffersize;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvReader(string file, Encoding encoding, char separator = ',', bool detectEncodingFromByteOrderMarks = true, int buffersize = 1024)
        {
            _File = file;
            _StreamHelper.Separator = separator;
            Encoding = encoding;
            _StreamHelper.Separator = separator;
            DetectEncodingFromByteOrderMarks = detectEncodingFromByteOrderMarks;
            _BufferSize = buffersize;
        }

        /// <summary>
        /// Csv Seperator to use default ','
        /// </summary>
        public char Separator
        {
            get => _StreamHelper.Separator;
            set => _StreamHelper.Separator = value;
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
        /// Indicates the End of the stream.
        /// </summary>
        public bool EndOfStream => _StreamHelper.EndOfStream;

        /// <summary>
        /// Returns current Line position.
        /// </summary>
        public int CurrentLine { get => _StreamHelper.CurrentLine; }

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
            _StreamHelper.CurrentLine = 0;
            _StreamHelper._byte = 0;
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
                _StreamHelper.Skip(_StreamReader.BaseStream);
                ii++;
            }
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

            _StreamHelper.MoveToLast(_StreamReader.BaseStream, rows);

            while (!EndOfStream)
            {
                yield return Read();
            }
        }

        //  \r = CR(Carriage Return) → Used as a new line character in Mac OS before X
        //  \n = LF(Line Feed) → Used as a new line character in Unix/Mac OS X
        //  \r\n = CR + LF → Used as a new line character in Windows

        /// <summary>
        /// reads the CsvLine
        /// </summary>
        public T Read()
        {
            // own implementation of _Streamer.ReadRow to futher improve speed.
            T _result = new T();
            int _columnIndex = 0;
            State _state = State.Normal;
            bool _trimLast = false;
            _sbValue.Length = 0; // Clear the string buffer.
            _StreamHelper._byte = 0;
            bool _rowEnd = false;
            int lineLength = 0;
            bool _escapedEscape = false;

            while (_StreamHelper._byte > -1)
            {
                _StreamHelper._byte = _StreamReader.BaseStream.ReadByte();
                _char = (char)_StreamHelper._byte;

                if (_StreamHelper._byte == -1)
                {
                    _rowEnd = true; // End of the document. execute the row end.
                }
                else if ((_state == State.Normal && _char == _CR))
                {
                    _StreamHelper._byte = _StreamReader.BaseStream.ReadByte(); // Read next byte to see if it is LF.
                    _char = (char)_StreamHelper._byte;
                    if (_char != _LF)
                    {
                        _StreamReader.BaseStream.Position--;
                        _rowEnd = true; // Indipendend CR, this means: end the row.
                    } // in case of LF: see next if statement below.
                }
                if ((_state == State.Normal && _char == _LF))
                {
                    _rowEnd = true; // LF always leeds to row end.
                }
                if (_rowEnd)
                {
                    if (_trimLast)
                    {
                        if (_sbValue.Length > 0)
                        {
                            if (_sbValue[_sbValue.Length - 1] == _ESCAPE)
                            {
                                _sbValue.Length--;
                            }
                        }
                    }
                    SetValue(_columnIndex, _result);
                    _StreamHelper.CurrentLine++;
                    break; // END ROW.
                }
                if (_char == Separator)
                {
                    if (_state == State.Normal)
                    {
                        if (_trimLast)
                        {
                            if (_sbValue.Length > 0)
                            {
                                if (_sbValue[_sbValue.Length - 1] == _ESCAPE)
                                {
                                    _sbValue.Length--;
                                }
                            }
                            _trimLast = false;
                        }
                        SetValue(_columnIndex, _result);
                        _sbValue.Length = 0; // Clear the string buffer.
                        _columnIndex++;
                        continue; // NEXT FIELD
                    }
                }
                else if (_char == _ESCAPE)
                {
                    if (_sbValue.Length == 0 && _state == State.Normal)
                    {
                        // first character in field is a double quote so we go to escape state.
                        _state = State.Escaped; // first char in field is escape char.
                        _trimLast = true; // we need to trim the last field as well.
                        continue; // LAST CHAR IN FIELD
                    }
                    else if (_state == State.Normal)
                    {
                        _state = State.Escaped;
                    }
                    else if (_state == State.Escaped)
                    {
                        char _nextChar = PeakNextChar(out int charByte);
                        if (charByte == -1)
                        {
                            _state = State.Normal;
                            continue;
                        }
                        if (_nextChar == _CR || _nextChar == _LF && _escapedEscape == false)
                        {
                            _state = State.Normal;
                            continue;
                        }
                        if (_nextChar == Separator && _escapedEscape == false)
                        {
                            _state = State.Normal;
                        }
                        if (_nextChar == _ESCAPE && _escapedEscape == false)
                        {
                            _state = State.Escaped;
                            _escapedEscape = true;
                            continue;
                        }
                    }
                }
                lineLength++;
                _sbValue.Append(_char);
                _escapedEscape = false;
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

        private char PeakNextChar(out int charByte)
        {
            charByte = _StreamReader.BaseStream.ReadByte(); // Read next byte to see if it is LF.
            char _c = (char)charByte;
            return _c;
        }

        private void SetValue(int index, T targetObject)
        {
            if (index >= _Properties.Length)
            {
                return;
            }
            if (_Properties[index] == null)
            {
                return;
            }
            else
            {
                if (_Properties[index].PropertyType == typeof(string))
                {
                    if (_CustomParserString[index] == null)
                    {
                        _PropertySettersString[index](targetObject, _sbValue.ToString());
                        return;
                    }
                    else
                    {
                        try
                        {
                            String _customParserValue = (String)_CustomParserCall[index](_CustomParserString[index], new object[] { _sbValue });
                            _PropertySettersString[index](targetObject, _customParserValue);
                        }
                        catch
                        {
                            _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                        }
                        return;
                    }
                }
                else
                {
                    SetValueOther(_Properties[index].PropertyType, index, targetObject);
                }
            }
        }

        #region Value Setters

        private void SetValueOther(Type targetType, int index, T targetObject)
        {
            if (!_IsNullable[index])
            {
                SetValueOtherNonNullable(targetType, index, targetObject);
            }
            else
            {
                SetValueOtherNullable(targetType, index, targetObject);
            }
        }

        private void SetValueOtherNonNullable(Type targetType, int index, T targetObject)
        {
            if (targetType == typeof(Decimal))
            {
                SetValueDecimal(index, targetObject);
                return;
            }
            else if (targetType == typeof(Int32))
            {
                SetValueInt32(index, targetObject);
                return;
            }
            else if (targetType == typeof(Double))
            {
                SetValueDouble(index, targetObject);
                return;
            }
            else if (targetType == typeof(DateTime))
            {
                SetValueDateTime(index, targetObject);
                return;
            }
            else if (targetType == typeof(Int64))
            {
                SetValueInt64(index, targetObject);
                return;
            }
            else if (targetType == typeof(Guid))
            {
                SetValueGuid(index, targetObject);
                return;
            }
            else if (targetType == typeof(Single))
            {
                SetValueSingle(index, targetObject);
                return;
            }
            else if (targetType == typeof(Boolean))
            {
                SetValueBoolean(index, targetObject);
                return;
            }
            else if (targetType == typeof(TimeSpan))
            {
                SetValueTimeSpan(index, targetObject);
                return;
            }
            else if (targetType == typeof(Int16))
            {
                SetValueInt16(index, targetObject);
                return;
            }
            else if (targetType == typeof(Byte))
            {
                SetValueByte(index, targetObject);
                return;
            }
            else if (targetType == typeof(DateTimeOffset))
            {
                SetValueDateTimeOffset(index, targetObject);
                return;
            }
            else if (targetType.IsEnum)
            {
                _PropertySetters[index](targetObject, Enum.Parse(targetType, _sbValue.ToString()));
                return;
            }
            else if (targetType == typeof(byte[]))
            {
                SetValueByteArray(index, targetObject);
                return;
            }
            else if (targetType == typeof(SByte))
            {
                SetValueSByte(index, targetObject);
                return;
            }
            else if (targetType == typeof(UInt16))
            {
                SetValueUInt16(index, targetObject);
                return;
            }
            else if (targetType == typeof(UInt32))
            {
                SetValueUInt32(index, targetObject);
                return;
            }
            else if (targetType == typeof(UInt64))
            {
                SetValueUInt64(index, targetObject);
                return;
            }
            else if (targetType == typeof(BigInteger))
            {
                SetValueBigInteger(index, targetObject);
                return;
            }
        }

        private void SetValueOtherNullable(Type targetType, int index, T targetObject)
        {
            if (targetType == typeof(DateTime?))
            {
                SetValueDateTimeNull(index, targetObject);
                return;
            }
            else if (targetType == typeof(Guid?))
            {
                SetValueGuidNull(index, targetObject);
                return;
            }
            else if (targetType == typeof(Boolean?))
            {
                SetValueBooleanNull(index, targetObject);
                return;
            }
            else if (targetType == typeof(DateTime?))
            {
                SetValueDateTimeNull(index, targetObject);
                return;
            }
            else if (targetType == typeof(DateTimeOffset?))
            {
                SetValueDateTimeOffsetNull(index, targetObject);
                return;
            }
            else if (targetType == typeof(TimeSpan?))
            {
                SetValueTimeSpanNull(index, targetObject);
                return;
            }
            else if (targetType == typeof(Byte?))
            {
                SetValueByteNull(index, targetObject);
                return;
            }
            else if (targetType == typeof(SByte?))
            {
                SetValueSByteNull(index, targetObject);
                return;
            }
            else if (targetType == typeof(Int16?))
            {
                SetValueInt16Null(index, targetObject);
                return;
            }
            else if (targetType == typeof(Int32?))
            {
                SetValueInt32Null(index, targetObject);
                return;
            }
            else if (targetType == typeof(Int64?))
            {
                SetValueInt64Null(index, targetObject);
                return;
            }
            else if (targetType == typeof(Single?))
            {
                SetValueSingleNull(index, targetObject);
                return;
            }
            else if (targetType == typeof(Decimal?))
            {
                SetValueDecimalNull(index, targetObject);
                return;
            }
            else if (targetType == typeof(Double?))
            {
                SetValueDoubleNull(index, targetObject);
                return;
            }
            else if (targetType == typeof(UInt16?))
            {
                SetValueUInt16Null(index, targetObject);
                return;
            }
            else if (targetType == typeof(UInt32?))
            {
                SetValueUInt32Null(index, targetObject);
                return;
            }
            else if (targetType == typeof(UInt64?))
            {
                SetValueUInt64Null(index, targetObject);
                return;
            }
            else if (targetType == typeof(BigInteger?))
            {
                SetValueBigIntegerNull(index, targetObject);
                return;
            }
        }

        private void SetValueDecimal(int index, T targetObject)
        {
            if (_CustomParserDecimal[index] != null)
            {
                try
                {
                    Decimal _customParserValue = (Decimal)_CustomParserCall[index](_CustomParserDecimal[index], new object[] { _sbValue });
                    _PropertySettersDecimal[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = Decimal.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out decimal _value);
            if (succes)
            {
                _PropertySettersDecimal[index](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueInt32(int index, T targetObject)
        {
            if (_CustomParserInt32[index] != null)
            {
                try
                {
                    Int32 _customParserValue = (Int32)_CustomParserCall[index](_CustomParserInt32[index], new object[] { _sbValue });
                    _PropertySettersInt32[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = Int32.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out int _value);
            if (succes)
            {
                _PropertySettersInt32[index](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueInt64(int index, T targetObject)
        {
            if (_CustomParserInt64[index] != null)
            {
                try
                {
                    Int64 _customParserValue = (Int64)_CustomParserCall[index](_CustomParserInt64[index], new object[] { _sbValue });
                    _PropertySettersInt64[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = Int64.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out long _value);
            if (succes)
            {
                _PropertySettersInt64[index](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueDouble(int index, T targetObject)
        {
            if (_CustomParserDouble[index] != null)
            {
                try
                {
                    Double _customParserValue = (Double)_CustomParserCall[index](_CustomParserDouble[index], new object[] { _sbValue });
                    _PropertySettersDouble[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = Double.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out double _value);
            if (succes)
            {
                _PropertySettersDouble[index](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueDateTime(int index, T targetObject)
        {
            if (_CustomParserDateTime[index] != null)
            {
                try
                {
                    DateTime _customParserValue = (DateTime)_CustomParserCall[index](_CustomParserDateTime[index], new object[] { _sbValue });
                    _PropertySettersDateTime[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = DateTime.TryParse(_sbValue.ToString(), Culture, DateTimeStyles.None, out DateTime _value);
            if (succes)
            {
                _PropertySettersDateTime[index](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueGuid(int index, T targetObject)
        {
            if (_CustomParserGuid[index] != null)
            {
                try
                {
                    Guid _customParserValue = (Guid)_CustomParserCall[index](_CustomParserGuid[index], new object[] { _sbValue });
                    _PropertySettersGuid[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = Guid.TryParse(_sbValue.ToString(), out Guid _value);
            if (succes)
            {
                _PropertySettersGuid[index](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueSingle(int index, T targetObject)
        {
            if (_CustomParserSingle[index] != null)
            {
                try
                {
                    Single _customParserValue = (Single)_CustomParserCall[index](_CustomParserSingle[index], new object[] { _sbValue });
                    _PropertySettersSingle[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = Single.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out float _value);
            if (succes)
            {
                _PropertySettersSingle[index](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueBoolean(int index, T targetObject)
        {
            if (_CustomParserBoolean[index] != null)
            {
                try
                {
                    bool _customParserValue = (bool)_CustomParserCall[index](_CustomParserBoolean[index], new object[] { _sbValue });
                    _PropertySettersBoolean[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = Boolean.TryParse(_sbValue.ToString(), out Boolean _value);
            if (succes)
            {
                _PropertySettersBoolean[index](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueTimeSpan(int index, T targetObject)
        {
            if (_CustomParserTimeSpan[index] != null)
            {
                try
                {
                    TimeSpan _customParserValue = (TimeSpan)_CustomParserCall[index](_CustomParserTimeSpan[index], new object[] { _sbValue });
                    _PropertySettersTimeSpan[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = TimeSpan.TryParse(_sbValue.ToString(), Culture, out TimeSpan _value);
            if (succes)
            {
                _PropertySettersTimeSpan[index](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueInt16(int index, T targetObject)
        {
            if (_CustomParserInt16[index] != null)
            {
                try
                {
                    Int16 _customParserValue = (Int16)_CustomParserCall[index](_CustomParserInt16[index], new object[] { _sbValue });
                    _PropertySettersInt16[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = Int16.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out Int16 _value);
            if (succes)
            {
                _PropertySettersInt16[index](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueByte(int index, T targetObject)
        {
            if (_CustomParserByte[index] != null)
            {
                try
                {
                    Byte _customParserValue = (Byte)_CustomParserCall[index](_CustomParserByte[index], new object[] { _sbValue });
                    _PropertySettersByte[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = Byte.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out Byte _value);
            if (succes)
            {
                _PropertySettersByte[index](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueDateTimeOffset(int index, T targetObject)
        {
            if (_CustomParserDateTimeOffset[index] != null)
            {
                try
                {
                    DateTimeOffset _customParserValue = (DateTimeOffset)_CustomParserCall[index](_CustomParserDateTimeOffset[index], new object[] { _sbValue });
                    _PropertySettersDateTimeOffset[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = DateTimeOffset.TryParse(_sbValue.ToString(), Culture, DateTimeStyles.None, out DateTimeOffset _value);
            if (succes)
            {
                _PropertySettersDateTimeOffset[index](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueByteArray(int index, T targetObject)
        {
            byte[] _byteValue = null;
            try
            {
                _byteValue = Convert.FromBase64String(_sbValue.ToString());
            }
            catch
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
            _PropertySetters[index](targetObject, _byteValue);
        }

        private void SetValueSByte(int index, T targetObject)
        {
            if (_CustomParserSByte[index] != null)
            {
                try
                {
                    SByte _customParserValue = (SByte)_CustomParserCall[index](_CustomParserSByte[index], new object[] { _sbValue });
                    _PropertySettersSByte[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            bool succes = SByte.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out SByte _value);
            if (succes)
            {
                _PropertySettersSByte[index](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueUInt16(int index, T targetObject)
        {
            if (_CustomParserUInt16[index] != null)
            {
                try
                {
                    UInt16 _customParserValue = (UInt16)_CustomParserCall[index](_CustomParserUInt16[index], new object[] { _sbValue });
                    _PropertySettersUInt16[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }
            bool succes = UInt16.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out UInt16 _value);
            if (succes)
            {
                _PropertySettersUInt16[index](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueUInt32(int index, T targetObject)
        {
            if (_CustomParserUInt32[index] != null)
            {
                try
                {
                    UInt32 _customParserValue = (UInt32)_CustomParserCall[index](_CustomParserUInt32[index], new object[] { _sbValue });
                    _PropertySettersUInt32[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }
            bool succes = UInt32.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out UInt32 _value);
            if (succes)
            {
                _PropertySettersUInt32[index](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueUInt64(int index, T targetObject)
        {
            if (_CustomParserUInt64[index] != null)
            {
                try
                {
                    UInt64 _customParserValue = (UInt64)_CustomParserCall[index](_CustomParserUInt64[index], new object[] { _sbValue });
                    _PropertySettersUInt64[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }
            bool succes = UInt64.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out UInt64 _value);
            if (succes)
            {
                _PropertySettersUInt64[index](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueBigInteger(int index, T targetObject)
        {
            if (_CustomParserBigInteger[index] != null)
            {
                try
                {
                    BigInteger _customParserValue = (BigInteger)_CustomParserCall[index](_CustomParserBigInteger[index], new object[] { _sbValue });
                    _PropertySettersBigInteger[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }
            bool succes = BigInteger.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out BigInteger _value);
            if (succes)
            {
                _PropertySettersBigInteger[index](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueGuidNull(int index, T targetObject)
        {
            if (_CustomParserGuidNullable[index] != null)
            {
                try
                {
                    Guid? _customParserValue = (Guid?)_CustomParserCall[index](_CustomParserGuidNullable[index], new object[] { _sbValue });
                    _PropertySettersGuidNull[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = Guid.TryParse(_valueRead, out Guid _value);
                if (succes)
                {
                    _PropertySettersGuidNull[index](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersGuidNull[index](targetObject, null);
            }
        }

        private void SetValueBooleanNull(int index, T targetObject)
        {
            if (_CustomParserBooleanNullable[index] != null)
            {
                try
                {
                    bool? _customParserValue = (bool?)_CustomParserCall[index](_CustomParserBooleanNullable[index], new object[] { _sbValue });
                    _PropertySettersBooleanNull[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = Boolean.TryParse(_valueRead, out Boolean _value);
                if (succes)
                {
                    _PropertySettersBooleanNull[index](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersBooleanNull[index](targetObject, null);
            }
        }

        private void SetValueDateTimeNull(int index, T targetObject)
        {
            if (_CustomParserDateTimeNullable[index] != null)
            {
                try
                {
                    DateTime? _customParserValue = (DateTime?)_CustomParserCall[index](_CustomParserDateTimeNullable[index], new object[] { _sbValue });
                    _PropertySettersDateTimeNull[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = DateTime.TryParse(_valueRead, out DateTime _value);
                if (succes)
                {
                    _PropertySettersDateTimeNull[index](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersDateTimeNull[index](targetObject, null);
            }
        }

        private void SetValueDateTimeOffsetNull(int index, T targetObject)
        {
            if (_CustomParserDateTimeOffsetNullable[index] != null)
            {
                try
                {
                    DateTimeOffset? _customParserValue = (DateTimeOffset?)_CustomParserCall[index](_CustomParserDateTimeOffsetNullable[index], new object[] { _sbValue });
                    _PropertySettersDateTimeOffsetNull[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = DateTimeOffset.TryParse(_valueRead, out DateTimeOffset _value);
                if (succes)
                {
                    _PropertySettersDateTimeOffsetNull[index](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersDateTimeOffsetNull[index](targetObject, null);
            }
        }

        private void SetValueTimeSpanNull(int index, T targetObject)
        {
            if (_CustomParserTimeSpanNullable[index] != null)
            {
                try
                {
                    TimeSpan? _customParserValue = (TimeSpan?)_CustomParserCall[index](_CustomParserTimeSpanNullable[index], new object[] { _sbValue });
                    _PropertySettersTimeSpanNull[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = TimeSpan.TryParse(_valueRead, out TimeSpan _value);
                if (succes)
                {
                    _PropertySettersTimeSpanNull[index](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersTimeSpanNull[index](targetObject, null);
            }
        }

        private void SetValueByteNull(int index, T targetObject)
        {
            if (_CustomParserByteNullable[index] != null)
            {
                try
                {
                    byte? _customParserValue = (byte?)_CustomParserCall[index](_CustomParserByteNullable[index], new object[] { _sbValue });
                    _PropertySettersByteNull[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = Byte.TryParse(_valueRead, out Byte _value);
                if (succes)
                {
                    _PropertySettersByteNull[index](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersByteNull[index](targetObject, null);
            }
        }

        private void SetValueSByteNull(int index, T targetObject)
        {
            if (_CustomParserSByteNullable[index] != null)
            {
                try
                {
                    SByte? _customParserValue = (SByte?)_CustomParserCall[index](_CustomParserSByteNullable[index], new object[] { _sbValue });
                    _PropertySettersSByteNull[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = SByte.TryParse(_valueRead, out SByte _value);
                if (succes)
                {
                    _PropertySettersSByteNull[index](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersSByteNull[index](targetObject, null);
            }
        }

        private void SetValueInt16Null(int index, T targetObject)
        {
            if (_CustomParserInt16Nullable[index] != null)
            {
                try
                {
                    Int16? _customParserValue = (Int16?)_CustomParserCall[index](_CustomParserInt16Nullable[index], new object[] { _sbValue });
                    _PropertySettersInt16Null[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = Int16.TryParse(_valueRead, out Int16 _value);
                if (succes)
                {
                    _PropertySettersInt16Null[index](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersInt16Null[index](targetObject, null);
            }
        }

        private void SetValueInt32Null(int index, T targetObject)
        {
            if (_CustomParserInt32Nullable[index] != null)
            {
                try
                {
                    Int32? _customParserValue = (Int32?)_CustomParserCall[index](_CustomParserInt32Nullable[index], new object[] { _sbValue });
                    _PropertySettersInt32Null[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = Int32.TryParse(_valueRead, out Int32 _value);
                if (succes)
                {
                    _PropertySettersInt32Null[index](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersInt32Null[index](targetObject, null);
            }
        }

        private void SetValueInt64Null(int index, T targetObject)
        {
            if (_CustomParserInt64Nullable[index] != null)
            {
                try
                {
                    Int64? _customParserValue = (Int64?)_CustomParserCall[index](_CustomParserInt64Nullable[index], new object[] { _sbValue });
                    _PropertySettersInt64Null[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = Int64.TryParse(_valueRead, out Int64 _value);
                if (succes)
                {
                    _PropertySettersInt64Null[index](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersInt64Null[index](targetObject, null);
            }
        }

        private void SetValueSingleNull(int index, T targetObject)
        {
            if (_CustomParserSingleNullable[index] != null)
            {
                try
                {
                    Single? _customParserValue = (Single?)_CustomParserCall[index](_CustomParserSingleNullable[index], new object[] { _sbValue });
                    _PropertySettersSingleNull[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = Single.TryParse(_valueRead, out Single _value);
                if (succes)
                {
                    _PropertySettersSingleNull[index](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersSingleNull[index](targetObject, null);
            }
        }

        private void SetValueDecimalNull(int index, T targetObject)
        {
            if (_CustomParserDecimalNullable[index] != null)
            {
                try
                {
                    Decimal? _customParserValue = (Decimal?)_CustomParserCall[index](_CustomParserDecimalNullable[index], new object[] { _sbValue });
                    _PropertySettersDecimalNull[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = Decimal.TryParse(_valueRead, out Decimal _value);
                if (succes)
                {
                    _PropertySettersDecimalNull[index](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersDecimalNull[index](targetObject, null);
            }
        }

        private void SetValueDoubleNull(int index, T targetObject)
        {
            if (_CustomParserDoubleNullable[index] != null)
            {
                try
                {
                    Double? _customParserValue = (Double?)_CustomParserCall[index](_CustomParserDoubleNullable[index], new object[] { _sbValue });
                    _PropertySettersDoubleNull[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = Double.TryParse(_valueRead, out Double _value);
                if (succes)
                {
                    _PropertySettersDoubleNull[index](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersDoubleNull[index](targetObject, null);
            }
        }

        private void SetValueUInt16Null(int index, T targetObject)
        {
            if (_CustomParserUInt16Nullable[index] != null)
            {
                try
                {
                    UInt16? _customParserValue = (UInt16?)_CustomParserCall[index](_CustomParserUInt16Nullable[index], new object[] { _sbValue });
                    _PropertySettersUInt16Null[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = UInt16.TryParse(_valueRead, out UInt16 _value);
                if (succes)
                {
                    _PropertySettersUInt16Null[index](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersUInt16Null[index](targetObject, null);
            }
        }

        private void SetValueUInt32Null(int index, T targetObject)
        {
            if (_CustomParserUInt32Nullable[index] != null)
            {
                try
                {
                    UInt32? _customParserValue = (UInt32?)_CustomParserCall[index](_CustomParserUInt32Nullable[index], new object[] { _sbValue });
                    _PropertySettersUInt32Null[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = UInt32.TryParse(_valueRead, out UInt32 _value);
                if (succes)
                {
                    _PropertySettersUInt32Null[index](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersUInt32Null[index](targetObject, null);
            }
        }

        private void SetValueUInt64Null(int index, T targetObject)
        {
            if (_CustomParserUInt64Nullable[index] != null)
            {
                try
                {
                    UInt64? _customParserValue = (UInt64?)_CustomParserCall[index](_CustomParserUInt64Nullable[index], new object[] { _sbValue });
                    _PropertySettersUInt64Null[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = UInt64.TryParse(_valueRead, out UInt64 _value);
                if (succes)
                {
                    _PropertySettersUInt64Null[index](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersUInt64Null[index](targetObject, null);
            }
        }

        private void SetValueBigIntegerNull(int index, T targetObject)
        {
            if (_CustomParserBigIntegerNullable[index] != null)
            {
                try
                {
                    BigInteger? _customParserValue = (BigInteger?)_CustomParserCall[index](_CustomParserBigIntegerNullable[index], new object[] { _sbValue });
                    _PropertySettersBigIntegerNull[index](targetObject, _customParserValue);
                }
                catch
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
                return;
            }

            string _valueRead = _sbValue.ToString();

            if (!string.IsNullOrEmpty(_valueRead))
            {
                bool succes = BigInteger.TryParse(_valueRead, out BigInteger _value);
                if (succes)
                {
                    _PropertySettersBigIntegerNull[index](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = index, PropertyName = _Properties[index].Name, PropertyType = _Properties[index].PropertyType, Value = _sbValue.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                _PropertySettersBigIntegerNull[index](targetObject, null);
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
                _StreamReader = new StreamReader(stream: _Stream, encoding: Encoding, detectEncodingFromByteOrderMarks: DetectEncodingFromByteOrderMarks, bufferSize: _BufferSize);
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
                _StreamReader = new StreamReader(path: _File, encoding: Encoding, detectEncodingFromByteOrderMarks: DetectEncodingFromByteOrderMarks, bufferSize: _BufferSize);

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

            _Properties = new PropertyInfo[_max + 1];
            _PropertySetters = new Action<object, object>[_max + 1];
            _IsNullable = new Boolean[_max + 1];


            _PropertySettersString = new Action<T, String>[_max + 1];
            _PropertySettersGuid = new Action<T, Guid>[_max + 1];
            _PropertySettersBoolean = new Action<T, Boolean>[_max + 1];
            _PropertySettersDateTime = new Action<T, DateTime>[_max + 1];
            _PropertySettersDateTimeOffset = new Action<T, DateTimeOffset>[_max + 1];
            _PropertySettersTimeSpan = new Action<T, TimeSpan>[_max + 1];
            _PropertySettersByte = new Action<T, Byte>[_max + 1];
            _PropertySettersSByte = new Action<T, SByte>[_max + 1];
            _PropertySettersInt16 = new Action<T, Int16>[_max + 1];
            _PropertySettersInt32 = new Action<T, Int32>[_max + 1];
            _PropertySettersInt64 = new Action<T, Int64>[_max + 1];
            _PropertySettersSingle = new Action<T, Single>[_max + 1];
            _PropertySettersDecimal = new Action<T, Decimal>[_max + 1];
            _PropertySettersDouble = new Action<T, Double>[_max + 1];
            _PropertySettersUInt16 = new Action<T, UInt16>[_max + 1];
            _PropertySettersUInt32 = new Action<T, UInt32>[_max + 1];
            _PropertySettersUInt64 = new Action<T, UInt64>[_max + 1];
            _PropertySettersBigInteger = new Action<T, BigInteger>[_max + 1];

            _PropertySettersGuidNull = new Action<T, Guid?>[_max + 1];
            _PropertySettersBooleanNull = new Action<T, Boolean?>[_max + 1];
            _PropertySettersDateTimeNull = new Action<T, DateTime?>[_max + 1];
            _PropertySettersDateTimeOffsetNull = new Action<T, DateTimeOffset?>[_max + 1];
            _PropertySettersTimeSpanNull = new Action<T, TimeSpan?>[_max + 1];
            _PropertySettersByteNull = new Action<T, Byte?>[_max + 1];
            _PropertySettersSByteNull = new Action<T, SByte?>[_max + 1];
            _PropertySettersInt16Null = new Action<T, Int16?>[_max + 1];
            _PropertySettersInt32Null = new Action<T, Int32?>[_max + 1];
            _PropertySettersInt64Null = new Action<T, Int64?>[_max + 1];
            _PropertySettersSingleNull = new Action<T, Single?>[_max + 1];
            _PropertySettersDecimalNull = new Action<T, Decimal?>[_max + 1];
            _PropertySettersDoubleNull = new Action<T, Double?>[_max + 1];
            _PropertySettersUInt16Null = new Action<T, UInt16?>[_max + 1];
            _PropertySettersUInt32Null = new Action<T, UInt32?>[_max + 1];
            _PropertySettersUInt64Null = new Action<T, UInt64?>[_max + 1];
            _PropertySettersBigIntegerNull = new Action<T, BigInteger?>[_max + 1];

            foreach (var property in _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => new { Property = p, Index = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index, Attrib = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute) })
                )
            {
                Type propertyType = property.Property.PropertyType;

                _IsNullable[property.Index] = Nullable.GetUnderlyingType(propertyType) != null;
               
                if (property.Attrib.CustomParserType != null)
                {
                    SetCustomParserType(property.Index, property.Attrib.CustomParserType, property.Property.Name);

                    _CustomParserCall[property.Index] = DelegateFactory.InstanceMethod(property.Attrib.CustomParserType, "Read", typeof(StringBuilder));
                }

                if (propertyType == typeof(string))
                {
                    _PropertySettersString[property.Index] = DelegateFactory.PropertySet<T, string>(property.Property.Name);
                }
                else if (propertyType == typeof(Guid))
                {
                    _PropertySettersGuid[property.Index] = DelegateFactory.PropertySet<T, Guid>(property.Property.Name);
                }
                else if (propertyType == typeof(Boolean))
                {
                    _PropertySettersBoolean[property.Index] = DelegateFactory.PropertySet<T, Boolean>(property.Property.Name);
                }
                else if (propertyType == typeof(DateTime))
                {
                    _PropertySettersDateTime[property.Index] = DelegateFactory.PropertySet<T, DateTime>(property.Property.Name);
                }
                else if (propertyType == typeof(DateTimeOffset))
                {
                    _PropertySettersDateTimeOffset[property.Index] = DelegateFactory.PropertySet<T, DateTimeOffset>(property.Property.Name);
                }
                else if (propertyType == typeof(TimeSpan))
                {
                    _PropertySettersTimeSpan[property.Index] = DelegateFactory.PropertySet<T, TimeSpan>(property.Property.Name);
                }
                else if (propertyType == typeof(Byte))
                {
                    _PropertySettersByte[property.Index] = DelegateFactory.PropertySet<T, Byte>(property.Property.Name);
                }
                else if (propertyType == typeof(SByte))
                {
                    _PropertySettersSByte[property.Index] = DelegateFactory.PropertySet<T, SByte>(property.Property.Name);
                }
                else if (propertyType == typeof(Int16))
                {
                    _PropertySettersInt16[property.Index] = DelegateFactory.PropertySet<T, Int16>(property.Property.Name);
                }
                else if (propertyType == typeof(Int32))
                {
                    _PropertySettersInt32[property.Index] = DelegateFactory.PropertySet<T, Int32>(property.Property.Name);
                }
                else if (propertyType == typeof(Int64))
                {
                    _PropertySettersInt64[property.Index] = DelegateFactory.PropertySet<T, Int64>(property.Property.Name);
                }
                else if (propertyType == typeof(Single))
                {
                    _PropertySettersSingle[property.Index] = DelegateFactory.PropertySet<T, Single>(property.Property.Name);
                }
                else if (propertyType == typeof(Decimal))
                {
                    _PropertySettersDecimal[property.Index] = DelegateFactory.PropertySet<T, Decimal>(property.Property.Name);
                }
                else if (propertyType == typeof(Double))
                {
                    _PropertySettersDouble[property.Index] = DelegateFactory.PropertySet<T, Double>(property.Property.Name);
                }
                else if (propertyType == typeof(UInt16))
                {
                    _PropertySettersUInt16[property.Index] = DelegateFactory.PropertySet<T, UInt16>(property.Property.Name);
                }
                else if (propertyType == typeof(UInt32))
                {
                    _PropertySettersUInt32[property.Index] = DelegateFactory.PropertySet<T, UInt32>(property.Property.Name);
                }
                else if (propertyType == typeof(UInt64))
                {
                    _PropertySettersUInt64[property.Index] = DelegateFactory.PropertySet<T, UInt64>(property.Property.Name);
                }
                else if (propertyType == typeof(BigInteger))
                {
                    _PropertySettersBigInteger[property.Index] = DelegateFactory.PropertySet<T, BigInteger>(property.Property.Name);
                }
                else if (propertyType == typeof(Guid?))
                {
                    _PropertySettersGuidNull[property.Index] = DelegateFactory.PropertySet<T, Guid?>(property.Property.Name);
                }
                else if (propertyType == typeof(Boolean?))
                {
                    _PropertySettersBooleanNull[property.Index] = DelegateFactory.PropertySet<T, Boolean?>(property.Property.Name);
                }
                else if (propertyType == typeof(DateTime?))
                {
                    _PropertySettersDateTimeNull[property.Index] = DelegateFactory.PropertySet<T, DateTime?>(property.Property.Name);
                }
                else if (propertyType == typeof(DateTimeOffset?))
                {
                    _PropertySettersDateTimeOffsetNull[property.Index] = DelegateFactory.PropertySet<T, DateTimeOffset?>(property.Property.Name);
                }
                else if (propertyType == typeof(TimeSpan?))
                {
                    _PropertySettersTimeSpanNull[property.Index] = DelegateFactory.PropertySet<T, TimeSpan?>(property.Property.Name);
                }
                else if (propertyType == typeof(Byte?))
                {
                    _PropertySettersByteNull[property.Index] = DelegateFactory.PropertySet<T, Byte?>(property.Property.Name);
                }
                else if (propertyType == typeof(SByte?))
                {
                    _PropertySettersSByteNull[property.Index] = DelegateFactory.PropertySet<T, SByte?>(property.Property.Name);
                }
                else if (propertyType == typeof(Int16?))
                {
                    _PropertySettersInt16Null[property.Index] = DelegateFactory.PropertySet<T, Int16?>(property.Property.Name);
                }
                else if (propertyType == typeof(Int32?))
                {
                    _PropertySettersInt32Null[property.Index] = DelegateFactory.PropertySet<T, Int32?>(property.Property.Name);
                }
                else if (propertyType == typeof(Int64?))
                {
                    _PropertySettersInt64Null[property.Index] = DelegateFactory.PropertySet<T, Int64?>(property.Property.Name);
                }
                else if (propertyType == typeof(Single?))
                {
                    _PropertySettersSingleNull[property.Index] = DelegateFactory.PropertySet<T, Single?>(property.Property.Name);
                }
                else if (propertyType == typeof(Decimal?))
                {
                    _PropertySettersDecimalNull[property.Index] = DelegateFactory.PropertySet<T, Decimal?>(property.Property.Name);
                }
                else if (propertyType == typeof(Double?))
                {
                    _PropertySettersDoubleNull[property.Index] = DelegateFactory.PropertySet<T, Double?>(property.Property.Name);
                }
                else if (propertyType == typeof(UInt16?))
                {
                    _PropertySettersUInt16Null[property.Index] = DelegateFactory.PropertySet<T, UInt16?>(property.Property.Name);
                }
                else if (propertyType == typeof(UInt32?))
                {
                    _PropertySettersUInt32Null[property.Index] = DelegateFactory.PropertySet<T, UInt32?>(property.Property.Name);
                }
                else if (propertyType == typeof(UInt64?))
                {
                    _PropertySettersUInt64Null[property.Index] = DelegateFactory.PropertySet<T, UInt64?>(property.Property.Name);
                }
                else if (propertyType == typeof(BigInteger?))
                {
                    _PropertySettersBigIntegerNull[property.Index] = DelegateFactory.PropertySet<T, BigInteger?>(property.Property.Name);
                }
                _Properties[property.Index] = property.Property;
                _PropertySetters[property.Index] = _type.PropertySet(property.Property.Name);
            }
        }

        private TypeGroup GetTypeGroup(Type type)
        {
            bool _nullable = Nullable.GetUnderlyingType(type) != null;
            Type _underlyingType = Nullable.GetUnderlyingType(type);

            if (_underlyingType == typeof(string))
            {
                return TypeGroup.String;
            }

            if (_underlyingType == typeof(Guid))
            {
                return _nullable ? TypeGroup.Nullable : TypeGroup.NonNullable;
            }
            if (_underlyingType == typeof(Boolean))
            {
                return _nullable ? TypeGroup.Nullable : TypeGroup.NonNullable;
            }
            if (_underlyingType == typeof(DateTime))
            {
                return _nullable ? TypeGroup.Nullable : TypeGroup.NonNullable;
            }
            if (_underlyingType == typeof(DateTimeOffset))
            {
                return _nullable ? TypeGroup.Nullable : TypeGroup.NonNullable;
            }
            if (_underlyingType == typeof(TimeSpan))
            {
                return _nullable ? TypeGroup.Nullable : TypeGroup.NonNullable;
            }
            if (_underlyingType == typeof(Byte))
            {
                return _nullable ? TypeGroup.Nullable : TypeGroup.NonNullable;
            }
            if (_underlyingType == typeof(Int16))
            {
                return _nullable ? TypeGroup.Nullable : TypeGroup.NonNullable;
            }
            if (_underlyingType == typeof(Int32))
            {
                return _nullable ? TypeGroup.Nullable : TypeGroup.NonNullable;
            }
            if (_underlyingType == typeof(Int64))
            {
                return _nullable ? TypeGroup.Nullable : TypeGroup.NonNullable;
            }
            if (_underlyingType == typeof(Single))
            {
                return _nullable ? TypeGroup.Nullable : TypeGroup.NonNullable;
            }
            if (_underlyingType == typeof(Decimal))
            {
                return _nullable ? TypeGroup.Nullable : TypeGroup.NonNullable;
            }
            if (_underlyingType == typeof(Double))
            {
                return _nullable ? TypeGroup.Nullable : TypeGroup.NonNullable;
            }
            if (_underlyingType == typeof(SByte))
            {
                return _nullable ? TypeGroup.NullableSpecial : TypeGroup.NonNullableSpecial;
            }
            if (_underlyingType == typeof(UInt16))
            {
                return _nullable ? TypeGroup.NullableSpecial : TypeGroup.NonNullableSpecial;
            }
            if (_underlyingType == typeof(UInt32))
            {
                return _nullable ? TypeGroup.NullableSpecial : TypeGroup.NonNullableSpecial;
            }
            if (_underlyingType == typeof(UInt64))
            {
                return _nullable ? TypeGroup.NullableSpecial : TypeGroup.NonNullableSpecial;
            }
            if (_underlyingType == typeof(BigInteger))
            {
                return _nullable ? TypeGroup.NullableSpecial : TypeGroup.NonNullableSpecial;
            }

            return TypeGroup.String;
        }
    }
}