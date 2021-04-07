using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace DevToys.PocoCsv.Core
{
    public class CsvReader<T> : IDisposable where T : new()
    {
        /// <summary>
        /// After Read, before Serialize. use this to prepare row values for serialization.
        /// </summary>
        public Action<RowData> BeforeSerialize = delegate { };


        private string[] _CurrentRow = null;
        private string _File = null;
        private Dictionary<int, PropertyInfo> _Properties = new Dictionary<int, PropertyInfo>();
        private CsvStreamReader _Reader;

        public CsvReader(string file)
        {
            _File = file;
        }

        public CsvReader(string file, Action<RowData> beforeSerialize)
        {
            _File = file;
            BeforeSerialize = beforeSerialize;
        }

        /// <summary>
        /// Culture info to use for serialization.
        /// </summary>
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;
        public char Separator { get; set; } = ',';

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _Reader.Close();
        }

        public void Open()
        {
            Init();
            _Reader = new CsvStreamReader(_File);
            _Reader.Separator = Separator;
        }

        public IEnumerable<T> Rows()
        {
            _Reader.BaseStream.Position = 0;

            int _rowNumber = -1;

            while (!_Reader.EndOfCsvStream)
            {
                T _result = new T();
                _CurrentRow = _Reader.ReadCsvLine();
                _rowNumber++;

                if (_CurrentRow.Length != 9)
                {
                }

                RowData _ea = new RowData() { Row = _CurrentRow, RowNumber = _rowNumber };
                BeforeSerialize(_ea);

                if (_ea.Skip)
                {
                    //yield return default;
                    continue;
                }

                for (int ii = 0; ii < _ea.Row.Length; ii++)
                {
                    PropertyInfo _prop = _Properties[ii];
                    string _value = _ea.Row[ii];
                    try
                    {
                        _prop.SetValue(_result, Convert(_value, _prop.PropertyType, Culture));
                    }
                    catch
                    {
                    }
                }

                yield return _result;
            }
        }

        public string[] SampleRow()
        {
            string[] _result = null;
            if (_Reader.BaseStream.Position == 0)
            {
                _result = _Reader.ReadCsvLine();
                _Reader.BaseStream.Position = 0;
            }
            else
            {
                _result = _CurrentRow;
            }
            return _result;
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