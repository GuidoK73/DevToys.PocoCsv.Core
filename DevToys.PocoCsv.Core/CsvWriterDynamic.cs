using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Write T to Csv Stream from an IEnumerable source.
    /// </summary>
    public sealed class CsvWriterDynamic : BaseCsvWriter
    {
        /// <summary>
        ///
        /// </summary>
        public CsvWriterDynamic(string file) : base(file)
        { }

        /// <summary>
        ///
        /// </summary>
        public CsvWriterDynamic(Stream stream) : base(stream)
        { }

        /// <summary>
        ///
        /// </summary>
        public CsvWriterDynamic(string file, Encoding encoding, CultureInfo culture, char separator = ',', int buffersize = -1) : base(file, encoding, culture, separator, buffersize)
        { }

        /// <summary>
        ///
        /// </summary>
        public CsvWriterDynamic(Stream stream, Encoding encoding, CultureInfo culture, char separator = ',', int buffersize = -1) : base(stream, encoding, culture, separator, buffersize)
        { }

        /// <summary>
        /// Initialize and open the CSV Stream Writer.
        /// </summary>
        public override void Open()
        {
            _StreamWriter = new CsvStreamWriter(path: _File, encoding: Encoding, bufferSize: _BufferSize) { Separator = Separator };
        }

        /// <summary>
        ///
        /// </summary>
        protected override void Init()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write IEnumerable T to Csv Stream
        /// </summary>
        public void Write(IEnumerable<dynamic> rows)
        {
            var _first = true;
            foreach (dynamic item in rows)
            {
                if (_first)
                {
                    WriteHeader(item);
                    _first = false;
                }
                WriteObject(item);
            }
        }

        private void WriteObject(dynamic dataobject)
        {
            var dataobject2 = dataobject as IDictionary<string, Object>;
            var _keys = dataobject2.Keys.ToArray();
            _StreamWriter.Write("\r\n");
            for (int ii = 0; ii < _keys.Length; ii++)
            {
                string _value = (string)Convert(dataobject2[_keys[ii]], typeof(string), Culture);
                _StreamWriter.Write(Esc(_value));
                if (ii < _keys.Length-1)
                {
                    _StreamWriter.Write(Separator);
                }
            }
        }

        private string Esc(string s)
        {
            if (s.IndexOfAny(new char[] { '\r', '\n', '"', Separator }) == -1)
            {
                return s;
            }
            return $"\"{s.Replace("\"", "\"\"")}\"";
        }

        private void WriteHeader(dynamic dataobject)
        {
            var dataobject2 = dataobject as IDictionary<string, Object>;
            var _keys = dataobject2.Keys.ToArray();
            for (int ii = 0; ii < _keys.Length; ii++)
            {
                _StreamWriter.Write(Esc(_keys[ii]));
                if (ii < _keys.Length-1)
                {
                    _StreamWriter.Write(Separator);
                }
            }
        }

        private static object Convert(object value, Type target, CultureInfo culture)
        {
            target = Nullable.GetUnderlyingType(target) ?? target;
            return (target.IsEnum) ? Enum.Parse(target, value.ToString()) : System.Convert.ChangeType(value, target, culture);
        }
    }
}