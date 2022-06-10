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
    public class CsvWriterDynamic : IDisposable
    {
        private readonly string _File = null;
        private readonly Dictionary<int, PropertyInfo> _Properties = new();
        private readonly Dictionary<int, Func<object, object>> _PropertyGetters = new();
        private CsvStreamWriter _Writer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file"></param>
        public CsvWriterDynamic(string file)
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
            _Writer = new CsvStreamWriter(_File, Append, Encoding) { Separator = Separator };
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