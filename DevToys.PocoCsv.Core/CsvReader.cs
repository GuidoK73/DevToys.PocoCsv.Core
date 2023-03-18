using Delegates;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
        /// <summary>
        ///
        /// </summary>
        private StreamReader _StreamReader;

        private readonly CsvStreamer _Streamer = new();

        private PropertyInfo[] _Properties = null;
        private Type[] _PropertyUnderlyingType = null;

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
            _Streamer.Separator = separator;
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
            _Streamer.Separator = separator;
            Encoding = encoding;
            _Streamer.Separator = separator;
            DetectEncodingFromByteOrderMarks = detectEncodingFromByteOrderMarks;
            _BufferSize = buffersize;
        }

        /// <summary>
        /// Csv Seperator to use default ','
        /// </summary>
        public char Separator
        {
            get
            {
                return _Streamer.Separator;
            }
            set
            {
                _Streamer.Separator = value;
            }
        }

        /// <summary>
        /// Indicates the End of the stream.
        /// </summary>
        public bool EndOfStream => (_StreamReader.BaseStream.Position >= _StreamReader.BaseStream.Length);

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
            _StreamReader.Close();
        }

        /// <summary>
        /// Detect the separator by sampling first 10 rows.
        /// </summary>
        public void DetectSeparator()
        {
            var _reader = new CsvStreamReader(_StreamReader.BaseStream);
            var _succes = CsvUtils.GetCsvSeparator(_reader, out _Separator, 10);
            if (_succes)
            {
                Separator = _Separator;
            }
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

            _StreamReader.BaseStream.Position = 0;

            while (!EndOfStream)
            {
                yield return Read();
            }
        }

        /// <summary>
        /// Use to skip first row without serializing, usefull for skipping header.
        /// </summary>
        public void Skip(int rows = 1)
        {
            for (int ii = 0; ii < rows; ii++)
            {
                _Streamer.ReadRow(_StreamReader.BaseStream, (columnIndex, value) => { }); // do nothing with read.
            }
        }

        private enum State
        {
            First = 0,
            Normal = 1,
            Escaped = 2
        }

        private readonly StringBuilder _sbValue = new(127);
        private char _char;
        private int _byte;

        private const char _r = '\r';
        private const char _n = '\n';
        private const char _escape = '"';

        /// <summary>
        /// reads the CsvLine
        /// </summary>
        public T Read()
        {
            T _result = new();
            int _columnIndex = 0;
            var _state = State.First;
            bool _trimLast = false;
            _sbValue.Length = 0; // Clear
            _byte = 0;

            while (_byte > -1)
            {
                _byte = _StreamReader.BaseStream.ReadByte();
                _char = (char)_byte;
                if (_byte == -1 || (_state == State.Normal && (_char == _r || _char == _n)))
                {
                    if (_trimLast)
                    {
                        if (_sbValue.Length > 0 && _sbValue[_sbValue.Length - 1] == _escape)
                        {
                            _sbValue.Length--;
                        }
                    }
                    SetValue(_columnIndex, _result);
                    break;
                }
                if (_state == State.First)
                {
                    _state = State.Normal;
                    if (_char == _n)
                    {
                        continue;
                    }
                }
                if (_state == State.Normal && _char == Separator)
                {
                    if (_trimLast)
                    {
                        if (_sbValue.Length > 0)
                        {
                            _sbValue.Length--;
                        }
                        _trimLast = false;
                    }
                    SetValue(_columnIndex, _result);
                    _sbValue.Length = 0;
                    _columnIndex++;
                    continue;
                }
                if (_char == _escape)
                {
                    _state = (_state == State.Normal) ? State.Escaped : State.Normal;
                    if (_sbValue.Length == 0)
                    {
                        _trimLast = true; // .Trim() is costly on large sets. so we will change the length of the stringbuilder and skip adding first escape char.
                        continue;
                    }
                }

                _sbValue.Append(_char);
            }
            return _result;
        }

        #region Value Setters

        private void SetValue(int index, T targetObject)
        {
            if (_Properties[index] == null)
            {
                return;
            }

            Type targetType = _PropertyUnderlyingType[index];

            if (targetType == typeof(string))
            {
                _PropertySettersString[index](targetObject, _sbValue.ToString());
                return;
            }
            else if (targetType == typeof(Decimal))
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
        }

        private void SetValueDecimal(int index, T targetObject)
        {
            bool succes = Decimal.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out decimal _value);
            if (succes)
            {
                _PropertySettersDecimal[index](targetObject, _value);
            }
        }

        private void SetValueInt32(int index, T targetObject)
        {
            bool succes = Int32.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out int _value);
            if (succes)
            {
                _PropertySettersInt32[index](targetObject, _value);
            }
        }

        private void SetValueInt64(int index, T targetObject)
        {
            bool succes = Int64.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out long _value);
            if (succes)
            {
                _PropertySettersInt64[index](targetObject, _value);
            }
        }

        private void SetValueDouble(int index, T targetObject)
        {
            bool succes = Double.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out double _value);
            if (succes)
            {
                _PropertySettersDouble[index](targetObject, _value);
            }
        }

        private void SetValueDateTime(int index, T targetObject)
        {
            bool succes = DateTime.TryParse(_sbValue.ToString(), Culture, DateTimeStyles.None, out DateTime _value);
            if (succes)
            {
                _PropertySettersDateTime[index](targetObject, _value);
            }
        }

        private void SetValueGuid(int index, T targetObject)
        {
            bool succes = Guid.TryParse(_sbValue.ToString(), out Guid _value);
            if (succes)
            {
                _PropertySettersGuid[index](targetObject, _value);
            }
        }

        private void SetValueSingle(int index, T targetObject)
        {
            bool succes = Single.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out float _value);
            if (succes)
            {
                _PropertySettersSingle[index](targetObject, _value);
            }
        }

        private void SetValueBoolean(int index, T targetObject)
        {
            bool succes = Boolean.TryParse(_sbValue.ToString(), out Boolean _value);
            if (succes)
            {
                _PropertySettersBoolean[index](targetObject, _value);
            }
        }

        private void SetValueTimeSpan(int index, T targetObject)
        {
            bool succes = TimeSpan.TryParse(_sbValue.ToString(), Culture, out TimeSpan _value);
            if (succes)
            {
                _PropertySettersTimeSpan[index](targetObject, _value);
            }
        }

        private void SetValueInt16(int index, T targetObject)
        {
            bool succes = Int16.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out Int16 _value);
            if (succes)
            {
                _PropertySettersInt16[index](targetObject, _value);
            }
        }

        private void SetValueByte(int index, T targetObject)
        {
            bool succes = Byte.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out Byte _value);
            if (succes)
            {
                _PropertySettersByte[index](targetObject, _value);
            }
        }

        private void SetValueDateTimeOffset(int index, T targetObject)
        {
            bool succes = DateTimeOffset.TryParse(_sbValue.ToString(), Culture, DateTimeStyles.None, out DateTimeOffset _value);
            if (succes)
            {
                _PropertySettersDateTimeOffset[index](targetObject, _value);
            }
        }

        private void SetValueByteArray(int index, T targetObject)
        {
            byte[] _byteValue = Convert.FromBase64String(_sbValue.ToString());
            _PropertySetters[index](targetObject, _byteValue);
        }

        private void SetValueSByte(int index, T targetObject)
        {
            bool succes = SByte.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out SByte _value);
            if (succes)
            {
                _PropertySettersSByte[index](targetObject, _value);
            }
        }

        private void SetValueUInt16(int index, T targetObject)
        {
            bool succes = UInt16.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out UInt16 _value);
            if (succes)
            {
                _PropertySettersUInt16[index](targetObject, _value);
            }
        }

        private void SetValueUInt32(int index, T targetObject)
        {
            bool succes = UInt32.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out UInt32 _value);
            if (succes)
            {
                _PropertySettersUInt32[index](targetObject, _value);
            }
        }

        private void SetValueUInt64(int index, T targetObject)
        {
            bool succes = UInt64.TryParse(_sbValue.ToString(), NumberStyles.Any, Culture, out UInt64 _value);
            if (succes)
            {
                _PropertySettersUInt64[index](targetObject, _value);
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

                var _options = new FileStreamOptions
                {
                    Access = FileAccess.Read,
                    BufferSize = _BufferSize,
                    Share = FileShare.Read,
                    Mode = FileMode.Open
                };

                _StreamReader = new StreamReader(path: _File, encoding: Encoding, detectEncodingFromByteOrderMarks: DetectEncodingFromByteOrderMarks, options: _options);
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

            _Properties = new PropertyInfo[_max + 1];
            _PropertyUnderlyingType = new Type[_max + 1];
            _PropertySetters = new Action<object, object>[_max + 1];

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

            foreach (var _property in _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => new { Property = p, Index = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index })
                )
            {
                Type underlyingType = Nullable.GetUnderlyingType(_property.Property.PropertyType) ?? _property.Property.PropertyType;
                _PropertyUnderlyingType[_property.Index] = underlyingType;

                if (underlyingType == typeof(string))
                {
                    _PropertySettersString[_property.Index] = DelegateFactory.PropertySet<T, string>(_property.Property.Name);
                }
                if (underlyingType == typeof(Guid))
                {
                    _PropertySettersGuid[_property.Index] = DelegateFactory.PropertySet<T, Guid>(_property.Property.Name);
                }
                if (underlyingType == typeof(Boolean))
                {
                    _PropertySettersBoolean[_property.Index] = DelegateFactory.PropertySet<T, Boolean>(_property.Property.Name);
                }
                if (underlyingType == typeof(DateTime))
                {
                    _PropertySettersDateTime[_property.Index] = DelegateFactory.PropertySet<T, DateTime>(_property.Property.Name);
                }
                if (underlyingType == typeof(DateTimeOffset))
                {
                    _PropertySettersDateTimeOffset[_property.Index] = DelegateFactory.PropertySet<T, DateTimeOffset>(_property.Property.Name);
                }
                if (underlyingType == typeof(TimeSpan))
                {
                    _PropertySettersTimeSpan[_property.Index] = DelegateFactory.PropertySet<T, TimeSpan>(_property.Property.Name);
                }
                if (underlyingType == typeof(Byte))
                {
                    _PropertySettersByte[_property.Index] = DelegateFactory.PropertySet<T, Byte>(_property.Property.Name);
                }
                if (underlyingType == typeof(SByte))
                {
                    _PropertySettersSByte[_property.Index] = DelegateFactory.PropertySet<T, SByte>(_property.Property.Name);
                }
                if (underlyingType == typeof(Int16))
                {
                    _PropertySettersInt16[_property.Index] = DelegateFactory.PropertySet<T, Int16>(_property.Property.Name);
                }
                if (underlyingType == typeof(Int32))
                {
                    _PropertySettersInt32[_property.Index] = DelegateFactory.PropertySet<T, Int32>(_property.Property.Name);
                }
                if (underlyingType == typeof(Int64))
                {
                    _PropertySettersInt64[_property.Index] = DelegateFactory.PropertySet<T, Int64>(_property.Property.Name);
                }
                if (underlyingType == typeof(Single))
                {
                    _PropertySettersSingle[_property.Index] = DelegateFactory.PropertySet<T, Single>(_property.Property.Name);
                }
                if (underlyingType == typeof(Decimal))
                {
                    _PropertySettersDecimal[_property.Index] = DelegateFactory.PropertySet<T, Decimal>(_property.Property.Name);
                }
                if (underlyingType == typeof(Double))
                {
                    _PropertySettersDouble[_property.Index] = DelegateFactory.PropertySet<T, Double>(_property.Property.Name);
                }
                if (underlyingType == typeof(UInt16))
                {
                    _PropertySettersUInt16[_property.Index] = DelegateFactory.PropertySet<T, UInt16>(_property.Property.Name);
                }
                if (underlyingType == typeof(UInt32))
                {
                    _PropertySettersUInt32[_property.Index] = DelegateFactory.PropertySet<T, UInt32>(_property.Property.Name);
                }
                if (underlyingType == typeof(UInt64))
                {
                    _PropertySettersUInt64[_property.Index] = DelegateFactory.PropertySet<T, UInt64>(_property.Property.Name);
                }

                _Properties[_property.Index] = _property.Property;

                _PropertySetters[_property.Index] = _type.PropertySet(_property.Property.Name);
            }
        }
    }
}