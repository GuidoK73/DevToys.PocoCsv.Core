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
    public class CsvWriter<T> : IDisposable where T : new()
    {
        private readonly string _File = null;
        private Dictionary<int, PropertyInfo> _Properties = new();
        private Dictionary<int, Func<object, object>> _PropertyGetters = new();
        private CsvStreamWriter _Writer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file"></param>
        public CsvWriter(string file)
        {
            _File = file;
        }

        /// <summary>
        /// Culture info to use for serialization.
        /// </summary>
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// Csv Seperator to use default ','
        /// </summary>
        public char Separator { get; set; } = ',';

        /// <summary>
        /// Write command can be used to append multiple collections to the open Csv Stream.
        /// </summary>
        public bool Append { get; set; } = true;

        /// <summary>
        /// The character encoding to use.
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.Default;

        /// <summary>
        /// Releases all resources used by the System.IO.TextReader object.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _Writer.Close();
        }

        /// <summary>
        /// Initialize and open the CSV Stream Writer.
        /// </summary>
        public void Open()
        {
            Init();
            _Writer = new CsvStreamWriter(_File, Append, Encoding) { Separator = Separator };
        }

        /// <summary>
        /// Write IEnumerable T to Csv Stream
        /// </summary>
        public void Write(IEnumerable<T> rows)
        {
            if (Append)
            {
                FileInfo _info = new(_File);
                _Writer.BaseStream.Position = _info.Length;
            }
            else
            {
                _Writer.BaseStream.Position = 0;
            }

            var columnIndexes = _Properties.Keys.ToList();
            var _max = columnIndexes.Max();
            var _data = new string[_max + 1];

            foreach (T item in rows)
            {
                for (int ii = 0; ii <= _max; ii++)
                {
                    if (_Properties.ContainsKey(ii))
                    {
                        var _propertyGetter = _PropertyGetters[ii];
                        var _value = _propertyGetter(item);
                        _data[ii] = (string)TypeUtils.Convert(_value, typeof(string), Culture);
                    }
                    else
                    {
                        _data[ii] = string.Empty;
                    }
                }
                _Writer.WriteCsvLine(_data);
            }
        }


        private void Init()
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
        }
    }
}