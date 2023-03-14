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
    public sealed class CsvWriterDynamic : BaseCsvWriter
    {

        public CsvWriterDynamic(string file) : base(file)
        { }

        public CsvWriterDynamic(Stream stream) : base(stream)
        { }

        public CsvWriterDynamic(string file, Encoding encoding, CultureInfo culture, char separator = ',', bool append = true) : base(file, encoding, culture, separator)
        { }

        public CsvWriterDynamic(Stream stream, Encoding encoding, CultureInfo culture, char separator = ',', bool append = true) : base(stream, encoding, culture, separator)
        { }


        /// <summary>
        /// Initialize and open the CSV Stream Writer.
        /// </summary>
        public override void Open()
        {
            _Writer = new CsvStreamWriter(_File, Append, Encoding) { Separator = Separator };
        }

        protected override void Init()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write IEnumerable T to Csv Stream
        /// </summary>
        public void Write(IEnumerable<dynamic> rows)
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

            var _first = true;
            string[] _data;

            foreach (dynamic item in rows)
            {
                if (_first)
                {
                    _data = Header(item);
                    _Writer.WriteCsvLine(_data);
                    _first = false;
                }
                _data = ToArray(item);
                _Writer.WriteCsvLine(_data);
            }
        }

        private string[] ToArray(dynamic dataobject)
        {
            var dataobject2 = dataobject as IDictionary<string, Object>;
            var _items = new string[dataobject2.Keys.Count()];
            var _keys = dataobject2.Keys.ToArray();

            for (int ii = 0; ii < _items.Length; ii++)
                _items[ii] = (string)Convert(dataobject2[_keys[ii]], typeof(string), Culture);

            return _items;
        }

        private static string[] Header(dynamic dataobject)
        {
            var dataobject2 = dataobject as IDictionary<string, Object>;
            return dataobject2.Keys.ToArray();
        }

        private static object Convert(object value, Type target, CultureInfo culture)
        {
            target = Nullable.GetUnderlyingType(target) ?? target;
            return (target.IsEnum) ? Enum.Parse(target, value.ToString()) : System.Convert.ChangeType(value, target, culture);
        }
    }
}