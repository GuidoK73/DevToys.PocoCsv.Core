using Delegates;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Enumerate Csv Stream Reader over T.
    /// Properties needs to be marked with ColumnAttribute
    /// </summary>
    public class CsvReader<T> : IDisposable where T : new()
    {
        /// <summary>
        /// After Read, before Serialize. use this to prepare row values for serialization.
        /// </summary>
        private string[] _CurrentRow = null;
        private readonly string _File = null;
        private PropertyInfo[] _Properties = null;
        private Action<object, object>[] _PropertySetters = null;
        private CsvStreamReader _Reader;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file"></param>
        public CsvReader(string file)
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
        /// Indicates whether to look for byte order marks at the beginning of the file.
        /// </summary>
        public bool DetectEncodingFromByteOrderMarks { get; set; } = true;

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
            Close();
        }

        /// <summary>
        /// Initialize and open the CSV Stream Reader.
        /// </summary>
        public void Open()
        {
            Init();
            _Reader = new CsvStreamReader(_File, Encoding, DetectEncodingFromByteOrderMarks)  { Separator = Separator };
        }

        /// <summary>
        /// Close the CSV stream reader
        /// </summary>
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
                    var _propertySetter = _PropertySetters[_columnIndex];
                    try
                    {
                        object _value = Convert(cell, _prop.PropertyType, Culture);
                        _propertySetter(_result, _value);
                    }
                    catch
                    {
                    }
                    _columnIndex++;
                }
                yield return _result;
            }
        }

        /// <summary>
        /// Reads first row as a string Array
        /// </summary>
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