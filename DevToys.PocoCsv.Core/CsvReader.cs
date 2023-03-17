using Delegates;
using Delegates.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
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
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvReader(string file, Encoding encoding, char separator = ',', bool detectEncodingFromByteOrderMarks = true, int buffersize = 1024) : base(file, encoding, separator, detectEncodingFromByteOrderMarks, buffersize)
        { }

        /// <summary>
        /// 
        /// </summary>
        [Obsolete("Use ReadAsEnumerable() or Read() instead.")]
        public IEnumerable<T> Rows() => ReadAsEnumerable();

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

            while (!_StreamReader.EndOfCsvStream)
            {
                yield return Read();
            }
        }

        /// <summary>
        /// Use to skip first row(s) combined with Read() method. for ReadAsEnumerable() just use the Enumerable Skip method.
        /// </summary>
        public void Skip(int rows = 1)
        {
            for (int ii = 0; ii < rows; ii++) 
            {
                Read();
            }
        }

        private PropertyInfo _property;
        private Action<object, object> _propertySetter;


        /// <summary>
        /// Single row read 
        /// </summary>
        public T Read()
        {
            if (_StreamReader == null)
            {
                throw new IOException("Reader is closed!");
            }

            T _result = new();

            int _columnIndex = 0;

            foreach (string cell in _StreamReader.ReadCsvLine())
            {
                _property = _Properties[_columnIndex];
                _propertySetter = _PropertySetters[_columnIndex];

                if (_property != null)
                {
                    if (_AllPropertiesAreStrings)
                    {
                        _propertySetter(_result, cell);
                    }
                    else
                    {
                        SetValue(cell, _property.PropertyType, Culture, _result, _propertySetter);
                    }
                }
                _columnIndex++;
            }
            return _result;
        }



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
                _StreamReader = new CsvStreamReader(stream: _Stream, encoding : Encoding, detectEncodingFromByteOrderMarks: DetectEncodingFromByteOrderMarks, bufferSize: _BufferSize) { Separator = Separator };
            }
            if (!string.IsNullOrEmpty(_File))
            {
                if (!File.Exists(_File))
                {
                    throw new FileNotFoundException($"File '{_File}' not found.");
                }
                _StreamReader = new CsvStreamReader(path: _File, encoding: Encoding, detectEncodingFromByteOrderMarks : DetectEncodingFromByteOrderMarks, bufferSize : _BufferSize) { Separator = Separator };
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
                _Properties[_property.Index] = _property.Property;
                
                _PropertySetters[_property.Index] = _type.PropertySet(_property.Property.Name);
                
            }
        }
    }
}