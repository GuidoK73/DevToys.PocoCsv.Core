using Delegates;
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
        private string[] _CurrentRow = null;
        private readonly string _File = null;
        private PropertyInfo[] _Properties = null;
        private Action<object, object>[] _ActionSetters = null;
        private CsvStreamReader _Reader;
        

        public CsvReader(string file)
        {
            _File = file;
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

        /// <summary>
        /// Each iteration will read the next row.
        /// </summary>
        public IEnumerable<T> Rows()
        {
            _Reader.BaseStream.Position = 0;

            int _rowNumber = -1;

            while (!_Reader.EndOfCsvStream)
            {
                T _result = new();
                _rowNumber++;

                int _columnIndex = 0;
                foreach (string cell in _Reader.ReadCsvLine())
                {
                    PropertyInfo _prop = _Properties[_columnIndex];
                    var _setterAction = _ActionSetters[_columnIndex];
                    try
                    {
                        //_prop.SetValue(_result, Convert(cell, _prop.PropertyType, Culture));
                        object _value = Convert(cell, _prop.PropertyType, Culture);
                        _setterAction(_result, _value);
                    }
                    catch
                    {
                    }
                    _columnIndex++;
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
            if (_Properties != null)
                return;

            var _type = typeof(T);

            int _max = _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index).Max();

            _Properties = new PropertyInfo[_max + 1];
            _ActionSetters = new Action<object, object>[_max + 1];

            foreach (var _property in _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => new { Property = p, Index = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index })
                )
            {
                _Properties[_property.Index] = _property.Property;
                _ActionSetters[_property.Index] = _type.PropertySet(_property.Property.Name);
            }
        }
    }
}