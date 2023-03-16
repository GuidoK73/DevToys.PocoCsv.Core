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
    /// Write T to Csv Stream from an IEnumerable source.
    /// </summary>
    public class CsvWriter<T> : BaseCsvWriter where T : new()
    {
        private List<int> _ColumnIndexes;
        private int _MaxColumnIndex = 0;
        private string[] _Data;
        private bool _wroteHeader = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvWriter(string file) : base(file)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvWriter(Stream stream) : base(stream)
        {  }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvWriter(string file, Encoding encoding, CultureInfo culture, char separator = ',', bool append = true, int buffersize = -1) : base (file, encoding, culture, separator, append, buffersize)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvWriter(Stream stream, Encoding encoding, CultureInfo culture, char separator = ',', bool append = true, int buffersize = -1) : base (stream, encoding, culture, separator, append, buffersize)
        {  }

        /// <summary>
        /// Write IEnumerable T to Csv Stream
        /// </summary>
        public void Write(IEnumerable<T> rows, bool writeHeader = false)
        {
            if (Append || _wroteHeader)
            {                
                if (_File != null)
                {
                    FileInfo _info = new(_File);
                    _StreamWrtier.BaseStream.Position = _info.Length;
                }                
            }
            else
            {
                _StreamWrtier.BaseStream.Position = 0;
            }

            if (_StreamWrtier.BaseStream.Position == 0 && writeHeader)
            {
                if (_wroteHeader == false)
                {
                    WriteHeader();
                }
                _wroteHeader = false;
            }

            foreach (T row in rows)
            {
                Write(row);
            }
        }

        /// <summary>
        /// Write header with property names of T.
        /// </summary>
        public void WriteHeader()
        {
            for (int ii = 0; ii <= _MaxColumnIndex; ii++)
            {
                if (_Properties.ContainsKey(ii))
                {
                    var _property = _Properties[ii];
                    var _name = _property.Name;
                    var _attribute = _property.GetCustomAttribute<ColumnAttribute>();
                    if (_attribute != null && string.IsNullOrEmpty(_attribute.Header))
                    {
                        _name = _attribute.Header;
                    }
                    _Data[ii] = _name;
                }
                else
                {
                    _Data[ii] = string.Empty;
                }
            }
            _StreamWrtier.WriteCsvLine(_Data);
            _wroteHeader = true;
        }

        /// <summary>
        /// Write single row to CSV
        /// </summary>
        public void Write(T row)
        {

            for (int ii = 0; ii <= _MaxColumnIndex; ii++)
            {
                if (_Properties.ContainsKey(ii))
                {
                    var _property = _Properties[ii];
                    var _propertyGetter = _PropertyGetters[ii];
                    var _value = _propertyGetter(row);

                    if (_property.PropertyType == typeof(byte[]))
                    {
                        if (_value != null)
                        {
                            var _byteVakye = (byte[])_value;
                            _Data[ii] = Convert.ToBase64String(_byteVakye);
                        }
                        else
                        {
                            _Data[ii] = string.Empty;
                        }
                    }
                    else
                    {
                        _Data[ii] = (string)TypeUtils.Convert(_value, typeof(string), Culture);
                    }
                }
                else
                {
                    _Data[ii] = string.Empty;
                }
            }
            _StreamWrtier.WriteCsvLine(_Data);
        }

        /// <summary>
        /// Initialize and open the CSV Stream Writer.
        /// </summary>
        public override void Open()
        {
            Init();
            if (_Stream != null)
            {
                _StreamWrtier = new CsvStreamWriter(stream: _Stream, encoding: Encoding, bufferSize: _BufferSize) { Separator = Separator };
            }
            if (!string.IsNullOrEmpty(_File))
            {
                _StreamWrtier = new CsvStreamWriter(path: _File, append: Append, encoding: Encoding, bufferSize : _BufferSize) { Separator = Separator };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Init()
        {
            if (_Properties.Count > 0)
                return;

            var _type = typeof(T);

            _Properties = _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => new { Value = p, Key = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index })
                .ToDictionary(p => p.Key, p => p.Value);

            _PropertyGetters = _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => new { Value = p, Key = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index })
                .ToDictionary(p => p.Key, p => _type.PropertyGet(p.Value.Name));

            _ColumnIndexes = _Properties.Keys.ToList();
            _MaxColumnIndex = _ColumnIndexes.Max();
            _Data = new string[_MaxColumnIndex + 1];

        }
    }
}