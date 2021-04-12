using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    public class CsvWriter<T> : IDisposable where T : new()
    {
        /// <summary>
        /// After Read, before Serialize. use this to prepare row values for serialization.
        /// </summary>
        public Action<RowData> BeforeSerialize = delegate { };

        private string[] _CurrentRow = null;
        private string _File = null;
        private Dictionary<int, PropertyInfo> _Properties = new Dictionary<int, PropertyInfo>();
        private CsvStreamWriter _Writer;


        public CsvWriter(string file)
        {
            _File = file;
        }

        public CsvWriter(string file, Action<RowData> beforeSerialize)
        {
            _File = file;
            BeforeSerialize = beforeSerialize;
        }

        /// <summary>
        /// Culture info to use for serialization.
        /// </summary>
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        public char Separator { get; set; } = ',';

        public bool Append { get; set; } = false;

        public Encoding Encoding { get; set; } = Encoding.Default;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _Writer.Close();
        }

        public void Open()
        {
            Init();
            _Writer = new CsvStreamWriter(_File, Append, Encoding);
            _Writer.Separator = Separator;
        }

        public void Write(IEnumerable<T> rows)
        {
            _Writer.BaseStream.Position = 0;

            List<int> columnIndexes = _Properties.Keys.ToList();
            int _max = columnIndexes.Max();

            string[] _data = new string[_max + 1];

            foreach (T item in rows)
            {
                for (int ii = 0; ii <= _max; ii++)
                {
                    if (_Properties.ContainsKey(ii))
                    {
                        _data[ii] = (string)Convert(_Properties[ii].GetValue(item), typeof(string), Culture);
                    }
                    else
                    {
                        _data[ii] = string.Empty;
                    }
                }
                _Writer.WriteCsvLine(_data);
            }
        }

        private static object Convert(object value, Type target, CultureInfo culture)
        {
            target = Nullable.GetUnderlyingType(target) ?? target;
            return (target.IsEnum) ? Enum.Parse(target, value.ToString()) : System.Convert.ChangeType(value, target, culture);
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
        }
    }
}