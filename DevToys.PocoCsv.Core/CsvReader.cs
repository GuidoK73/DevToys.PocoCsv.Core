using Delegates;
using System;
using System.Collections.Generic;
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

        public CsvReader(string file) : base(file)
        { }

        public CsvReader(Stream stream) : base(stream)
        { }

        public CsvReader(Stream stream, Encoding encoding, char separator = ',', bool detectEncodingFromByteOrderMarks = true) : base(stream, encoding, separator, detectEncodingFromByteOrderMarks)
        { }

        public CsvReader(string file, Encoding encoding, char separator = ',', bool detectEncodingFromByteOrderMarks = true) : base(file, encoding, separator, detectEncodingFromByteOrderMarks)
        { }

        [Obsolete("Use ReadAsEnumerable() or Read() instead.")]
        public IEnumerable<T> Rows() => ReadAsEnumerable();

        /// <summary>
        /// Each iteration will read the next row from stream or file
        /// </summary>
        public IEnumerable<T> ReadAsEnumerable()
        {
            if (_Reader == null)
            {
                throw new IOException("Reader is closed!");
            }

            _Reader.BaseStream.Position = 0;

            while (!_Reader.EndOfCsvStream)
            {
                yield return Read();
            }
        }


        /// <summary>
        /// Single row read 
        /// </summary>
        public T Read()
        {
            if (_Reader == null)
            {
                throw new IOException("Reader is closed!");
            }

            T _result = new();

            int _columnIndex = 0;
            foreach (string cell in _Reader.ReadCsvLine())
            {
                PropertyInfo _prop = _Properties[_columnIndex];
                var _propertySetter = _PropertySetters[_columnIndex];
                try
                {
                    if (_prop != null)
                    {
                        object _value = TypeUtils.Convert(cell, _prop.PropertyType, Culture);
                        _propertySetter(_result, _value);
                    }
                }
                catch
                {
                }
                _columnIndex++;
            }
            return _result;
        }

        public override void Open()
        {
            Init();

            if (_Stream == null && string.IsNullOrEmpty(_File))
            {
                throw new IOException("No file or stream specified in constructor.");
            }

            if (_Stream != null)
            {
                _Reader = new CsvStreamReader(_Stream, Encoding, DetectEncodingFromByteOrderMarks) { Separator = Separator };
            }
            if (!string.IsNullOrEmpty(_File))
            {
                if (!File.Exists(_File))
                {
                    throw new FileNotFoundException($"File '{_File}' not found.");
                }
                _Reader = new CsvStreamReader(_File, Encoding, DetectEncodingFromByteOrderMarks) { Separator = Separator };
            }
        }

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

            foreach (var _property in _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => new { Property = p, Index = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index })
                )
            {
                _Properties[_property.Index] = _property.Property;
                _PropertySetters[_property.Index] = _type.PropertySet(_property.Property.Name);
            }
        }
    }
}