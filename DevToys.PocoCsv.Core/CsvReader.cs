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
        public CsvReader(Stream stream, Encoding encoding, char separator = ',', bool detectEncodingFromByteOrderMarks = true, int buffersize = -1) : base(stream, encoding, separator, detectEncodingFromByteOrderMarks, buffersize)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvReader(string file, Encoding encoding, char separator = ',', bool detectEncodingFromByteOrderMarks = true, int buffersize = -1) : base(file, encoding, separator, detectEncodingFromByteOrderMarks, buffersize)
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
                var _property = _Properties[_columnIndex];
                var _propertySetter = _PropertySetters[_columnIndex];
                try
                {
                    if (_property != null)
                    {

                        if (_property.PropertyType == typeof(byte[]))
                        {
                            if (!string.IsNullOrEmpty(cell))
                            {
                                byte[] _byteValue = Convert.FromBase64String(cell);
                                _propertySetter(_result, _byteValue);
                            }
                            else
                            {
                                _propertySetter(_result, null);
                            }
                        }
                        else
                        {
                            object _value = TypeUtils.Convert(cell, _property.PropertyType, Culture);
                            _propertySetter(_result, _value);
                        }
                    }
                }
                catch
                {
                }
                _columnIndex++;
            }
            return _result;
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