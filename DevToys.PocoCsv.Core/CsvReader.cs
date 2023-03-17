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
    public sealed class CsvReader<T> : BaseCsvReader where T : new()
    {
        private PropertyInfo[] _Properties = null;
        private Action<object, object>[] _PropertySetters = null;
        private Action<T, string>[] _PropertyStringSetters = null;

        /// <summary>
        /// Increase performance by only allowing string properties (No implicit casting)
        /// </summary>
        private bool _AllPropertiesAreStrings { get; set; } = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvReader(string file) : base(file)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvReader(Stream stream) : base(stream)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvReader(Stream stream, Encoding encoding, char separator = ',', bool detectEncodingFromByteOrderMarks = true, int buffersize = 1024) : base(stream, encoding, separator, detectEncodingFromByteOrderMarks, buffersize)
        {
            _Streamer.Separator = separator;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvReader(string file, Encoding encoding, char separator = ',', bool detectEncodingFromByteOrderMarks = true, int buffersize = 1024) : base(file, encoding, separator, detectEncodingFromByteOrderMarks, buffersize)
        {
            _Streamer.Separator = separator;
        }

        /// <summary>
        ///
        /// </summary>
        [Obsolete("Use ReadAsEnumerable() or Read() instead.")]
        public IEnumerable<T> Rows() => ReadAsEnumerable();

        /// <summary>
        /// Detect the separator by sampling first 10 rows.
        /// </summary>
        public void DetectSeparator()
        {
            CsvStreamReader _reader = new CsvStreamReader(_StreamReader.BaseStream);
            bool _succes = CsvUtils.GetCsvSeparator(_reader, out _Separator, 10);
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

            while (!_EndOfStream)
            {
                yield return Read();
            }
        }

        private bool _EndOfStream => (_StreamReader.BaseStream.Position >= _StreamReader.BaseStream.Length);

        /// <summary>
        /// Use to skip to skip first row without serializing, usefull for skipping header.
        /// </summary>
        public void Skip(int rows = 1)
        {
            for (int ii = 0; ii < rows; ii++)
            {
                _Streamer.ReadRow(_StreamReader.BaseStream, (columnIndex, value) => { }); // do nothing with read.
            }
        }

        // SLOWER
        //public T Read()
        //{
        //    T _result = new();
        //    _Streamer.ReadRow(_StreamReader.BaseStream, (columnIndex, value) => 
        //        {
        //            _property = _Properties[columnIndex];
        //            _propertySetter = _PropertySetters[columnIndex];
        //            _propertySetterString = _PropertyStringSetters[columnIndex];

        //            if (_property != null)
        //            {
        //                if (_AllPropertiesAreStrings)
        //                {
        //                    _propertySetterString(_result, value);
        //                }
        //                else
        //                {
        //                    if (_property.PropertyType == typeof(string))
        //                    {
        //                        _propertySetter(_result, value);
        //                    }
        //                    else
        //                    {
        //                        SetValue(value, _property.PropertyType, Culture, _result, _propertySetter);
        //                    }
        //                }
        //            }
        //        }
        //    ); // do nothing with read.
        //    return _result;
        //}


        private PropertyInfo _property;
        private Action<object, object> _propertySetter;
        private Action<T, string> _propertySetterString;

        #region Performance Read

        private enum State
        {
            First = 0,
            Normal = 1,
            Escaped = 2
        }

        private readonly StringBuilder _sb = new(63);
        private char _char;
        private int _byte;
        private string _value;

        /// <summary>
        /// reads the CsvLine
        /// </summary>
        public T Read()
        {
            T _result = new();
            int _columnIndex = 0;

            var _state = State.First;
            _sb.Length = 0;
            _byte = 0;
            while (_byte > -1)
            {
                _byte = _StreamReader.BaseStream.ReadByte();
                _char = (char)_byte;
                if (_byte == -1 || (_state == State.Normal && (_char == '\r' || _char == '\n')))
                {
                    _property = _Properties[_columnIndex];
                    _propertySetter = _PropertySetters[_columnIndex];
                    _propertySetterString = _PropertyStringSetters[_columnIndex];
                    _value = _sb.ToString().Trim('"');

                    if (_property != null)
                    {
                        if (_AllPropertiesAreStrings)
                        {
                            _propertySetterString(_result, _value);
                        }
                        else
                        {
                            if (_property.PropertyType == typeof(string))
                            {
                                _propertySetterString(_result, _value);
                            }
                            else
                            {
                                SetValue(_value, _property.PropertyType, Culture, _result, _propertySetter);
                            }
                        }
                    }

                    _columnIndex++;
                    break;
                }
                if (_state == State.First)
                {
                    _state = State.Normal;
                    if (_char == '\n')
                    {
                        continue;
                    }
                }
                if (_state == State.Normal && _char == Separator)
                {
                    _property = _Properties[_columnIndex];
                    _propertySetter = _PropertySetters[_columnIndex];
                    _propertySetterString = _PropertyStringSetters[_columnIndex];

                    _value = _sb.ToString().Trim('"');

                    if (_property != null)
                    {
                        if (_AllPropertiesAreStrings)
                        {
                            _propertySetterString(_result, _value);
                        }
                        else
                        {
                            if (_property.PropertyType == typeof(string))
                            {
                                _propertySetter(_result, _value);
                            }
                            else
                            {
                                SetValue(_value, _property.PropertyType, Culture, _result, _propertySetter);
                            }
                        }
                    }

                    _sb.Length = 0;
                    _columnIndex++;
                    continue;
                }
                if (_char == '"')
                {
                    _state = (_state == State.Normal) ? State.Escaped : State.Normal;
                }

                _sb.Append(_char);
            }
            return _result;
        }

        #endregion Performance Read

        private void SetValue(string value, Type targetType, CultureInfo culture, T targetObject, Action<object, object> _propertySetter)
        {
            targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;
            bool succes = false;

            if (targetType == typeof(string))
            {
                _propertySetter(targetObject, value);
                return;
            }

            if (targetType == typeof(Int32))
            {
                Int32 _value;
                succes = Int32.TryParse(value, out _value);
                if (succes)
                {
                    _propertySetter(targetObject, _value);
                }
                return;
            }
            if (targetType == typeof(Int64))
            {
                Int64 _value;
                succes = Int64.TryParse(value, out _value);
                if (succes)
                {
                    _propertySetter(targetObject, _value);
                }
                return;
            }
            if (targetType == typeof(Single))
            {
                Single _value;
                succes = Single.TryParse(value, out _value);
                if (succes)
                {
                    _propertySetter(targetObject, _value);
                }
                return;
            }
            if (targetType == typeof(Decimal))
            {
                Decimal _value;
                succes = Decimal.TryParse(value, out _value);
                if (succes)
                {
                    _propertySetter(targetObject, _value);
                }
                return;
            }
            if (targetType == typeof(Double))
            {
                Double _value;
                succes = Double.TryParse(value, out _value);
                if (succes)
                {
                    _propertySetter(targetObject, _value);
                }
                return;
            }
            if (targetType == typeof(Guid))
            {
                Guid _value;
                succes = Guid.TryParse(value, out _value);
                if (succes)
                {
                    _propertySetter(targetObject, _value);
                }
                return;
            }
            if (targetType == typeof(Boolean))
            {
                Boolean _value;
                succes = Boolean.TryParse(value, out _value);
                if (succes)
                {
                    _propertySetter(targetObject, _value);
                }
                return;
            }
            if (targetType == typeof(DateTime))
            {
                DateTime _value;
                succes = DateTime.TryParse(value, out _value);
                if (succes)
                {
                    _propertySetter(targetObject, _value);
                }
                return;
            }
            if (targetType == typeof(DateTimeOffset))
            {
                DateTimeOffset _value;
                succes = DateTimeOffset.TryParse(value, out _value);
                if (succes)
                {
                    _propertySetter(targetObject, _value);
                }
                return;
            }
            if (targetType == typeof(TimeSpan))
            {
                TimeSpan _value;
                succes = TimeSpan.TryParse(value, out _value);
                if (succes)
                {
                    _propertySetter(targetObject, _value);
                }
                return;
            }
            if (targetType.IsEnum)
            {
                _propertySetter(targetObject, Enum.Parse(targetType, value));
                return;
            }
            if (targetType == typeof(byte[]))
            {
                byte[] _byteValue = Convert.FromBase64String(value);
                _propertySetter(targetObject, _byteValue);
                return;
            }
            if (targetType == typeof(Byte))
            {
                Byte _value;
                succes = Byte.TryParse(value, out _value);
                if (succes)
                {
                    _propertySetter(targetObject, _value);
                }
                return;
            }
            if (targetType == typeof(SByte))
            {
                SByte _value;
                succes = SByte.TryParse(value, out _value);
                if (succes)
                {
                    _propertySetter(targetObject, _value);
                }
                return;
            }
            if (targetType == typeof(Int16))
            {
                Int16 _value;
                succes = Int16.TryParse(value, out _value);
                if (succes)
                {
                    _propertySetter(targetObject, _value);
                }
                return;
            }
            if (targetType == typeof(UInt16))
            {
                UInt16 _value;
                succes = UInt16.TryParse(value, out _value);
                if (succes)
                {
                    _propertySetter(targetObject, _value);
                }
                return;
            }
            if (targetType == typeof(UInt32))
            {
                UInt32 _value;
                succes = UInt32.TryParse(value, out _value);
                if (succes)
                {
                    _propertySetter(targetObject, _value);
                }
                return;
            }
            if (targetType == typeof(UInt64))
            {
                UInt64 _value;
                succes = UInt64.TryParse(value, out _value);
                if (succes)
                {
                    _propertySetter(targetObject, _value);
                }
                return;
            }
        }

        /// <summary>
        /// Open the reader
        /// </summary>
        public override void Open()
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
                _StreamReader = new StreamReader(path: _File, encoding: Encoding, detectEncodingFromByteOrderMarks: DetectEncodingFromByteOrderMarks, bufferSize: _BufferSize);
            }
        }

        /// <summary>
        /// Initialize the CsvReader
        /// </summary>
        protected override void Init()
        {
            if (_Properties != null)
                return;

            var _type = typeof(T);

            int _max = _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index).Max();

            _Properties = new PropertyInfo[_max + 1];
            _PropertySetters = new Action<object, object>[_max + 1];
            _PropertyStringSetters = new Action<T, string>[_max + 1];

            _AllPropertiesAreStrings = true;

            foreach (var _property in _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => new { Property = p, Index = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index })
                )
            {
                if (_property.Property.PropertyType != typeof(string))
                {
                    _AllPropertiesAreStrings = false;
                }
                else
                {
                    _PropertyStringSetters[_property.Index] = DelegateFactory.PropertySet<T, string>(_property.Property.Name);
                }
                _Properties[_property.Index] = _property.Property;

                _PropertySetters[_property.Index] = _type.PropertySet(_property.Property.Name);
            }
        }
    }
}