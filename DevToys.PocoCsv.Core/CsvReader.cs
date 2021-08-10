using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    public class CsvReader<T> : IDisposable where T : new()
    {
        /// <summary>
        /// After Read, before Serialize. use this to prepare row values for serialization.
        /// </summary>
        public Action<RowData> BeforeSerialize = delegate { };


        private string[] _CurrentRow = null;
        private readonly string _File = null;
        private Dictionary<int, PropertyInfo> _Properties = new();
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

        public bool DetectEncodingFromByteOrderMarks { get; set; } = true;

        public Encoding Encoding { get; set; } = Encoding.Default;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Close();
        }

        public void Open()
        {
            Init();
            _Reader = new CsvStreamReader(_File, Encoding, DetectEncodingFromByteOrderMarks)  { Separator = Separator };
        }

        public void Close()
        {
            _Reader.Close();
        }

        public IEnumerable<T> Rows()
        {
            _Reader.BaseStream.Position = 0;

            int _rowNumber = -1;

            while (!_Reader.EndOfCsvStream)
            {
                T _result = new T();
                _CurrentRow = _Reader.ReadCsvLine().ToArray();
                _rowNumber++;

                RowData _ea = new() { Row = _CurrentRow, RowNumber = _rowNumber };
                BeforeSerialize(_ea);

                if (_ea.Skip)
                {
                    continue;
                }

                foreach (int columnIndex in _Properties.Keys)
                {
                    PropertyInfo _prop = _Properties[columnIndex];
                    if (columnIndex > _ea.Row.Length -1)
                    {
                        throw new IndexOutOfRangeException($"Column Index {columnIndex} is out of range");
                    }
                    string _value = _ea.Row[columnIndex];
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
            string[] _result;
            if (_Reader.BaseStream.Position == 0)
            {
                _result = _Reader.ReadCsvLine().ToArray();
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