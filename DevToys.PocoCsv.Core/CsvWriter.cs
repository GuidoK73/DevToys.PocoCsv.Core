using Delegates;
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

        public CsvWriter(string file) : base(file)
        { }

        public CsvWriter(Stream stream) : base(stream)
        {  }

        public CsvWriter(string file, Encoding encoding, CultureInfo culture, char separator = ',', bool append = true) : base (file, encoding, culture, separator)
        { }

        public CsvWriter(Stream stream, Encoding encoding, CultureInfo culture, char separator = ',', bool append = true) : base (stream, encoding, culture, separator)
        {  }

        /// <summary>
        /// Write IEnumerable T to Csv Stream
        /// </summary>
        public void Write(IEnumerable<T> rows)
        {
            if (Append)
            {
                if (_File != null)
                {
                    FileInfo _info = new(_File);
                    _Writer.BaseStream.Position = _info.Length;
                }                
            }
            else
            {
                _Writer.BaseStream.Position = 0;
            }

            foreach (T row in rows)
            {
                Write(row);
            }
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
                    var _propertyGetter = _PropertyGetters[ii];
                    var _value = _propertyGetter(row);
                    _Data[ii] = (string)TypeUtils.Convert(_value, typeof(string), Culture);
                }
                else
                {
                    _Data[ii] = string.Empty;
                }
            }
            _Writer.WriteCsvLine(_Data);
        }

        /// <summary>
        /// Initialize and open the CSV Stream Writer.
        /// </summary>
        public override void Open()
        {
            Init();
            if (_Stream != null)
            {
                _Writer = new CsvStreamWriter(_Stream, Encoding) { Separator = Separator };
            }
            if (!string.IsNullOrEmpty(_File))
            {
                _Writer = new CsvStreamWriter(_File, Append, Encoding) { Separator = Separator };
            }
        }

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