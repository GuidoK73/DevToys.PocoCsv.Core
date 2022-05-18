using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Csv Utilities.
    /// </summary>
    public static class CsvUtils
    {
        /// <summary>
        /// Returns first row of CSV
        /// </summary>
        public static string[] CsvHeader(string text, char separator)
        {
            byte[] byteArray = Encoding.Default.GetBytes(text);
            using MemoryStream _stream = new MemoryStream(byteArray);
            using (CsvStreamReader _reader = new CsvStreamReader(_stream))
            {
                return CsvHeader(_reader, separator);
            }
        }

        /// <summary>
        /// Returns first row of CSV
        /// </summary>
        public static string[] CsvHeader(CsvStreamReader reader, char separator)
        {
            reader.Position = 0;
            reader.Separator = separator;
            return reader.ReadCsvLine().ToArray();
        }

        /// <summary>
        /// Returns a schema for the CSV with best fitted types to use.
        /// </summary>
        public static IEnumerable<CsvColumnInfo> GetCsvSchema(CsvStreamReader reader, int sampleRows)
        {
            bool _succes = CsvUtils.GetCsvSeparator(reader, out char _separator, sampleRows);
            if (!_succes)
                throw new ArgumentException("text is not valid csv!");

            string[] _header = CsvHeader(reader, _separator);
            NetType[] _columnTypes = new NetType[_header.Length];
            bool[] _columnNullable = new bool[_header.Length];
            reader.Position = 0;

            bool _first = true;
            int _row = 0;

            while (!reader.EndOfCsvStream && (_row < sampleRows))
            {
                string[] _items = reader.ReadCsvLine().ToArray();
                if (!_first)
                {
                    for (int ii = 0; ii < _items.Length; ii++)
                    {
                        if (_items.Length == _header.Length)
                        {
                            NetType _netType = TypeUtils.BestNetType(_columnTypes[ii], _items[ii]);
                            if (_netType != NetType.Null)
                            {
                                _columnTypes[ii] = _netType;
                            }
                            else
                            {
                                if (_columnTypes[ii] != NetType.String && _columnTypes[ii] != NetType.Unknown)
                                {
                                    _columnNullable[ii] = true;
                                }
                            }
                        }
                    }
                }
                _row++;
                _first = false;
            }

            for (int ii = 0; ii < _header.Length; ii++)
            {
                yield return new CsvColumnInfo()
                {
                    Name = _header[ii],
                    DotNetType = _columnTypes[ii],
                    Nullable = _columnNullable[ii],
                    Index = ii,
                    DatabaseType = TypeUtils.GetDbType(_columnTypes[ii]),
                    SqlDatabaseType = TypeUtils.GetSqlDbType(_columnTypes[ii]),
                    IsLast = (ii == _header.Length - 1)
                };
            }
        }

        /// <summary>
        /// Retrieves CSV separator from text.
        /// </summary>
        public static bool GetCsvSeparator(CsvStreamReader reader, out char separator, int sampleRows)
        {
            char[] _tests = new char[] { ',', ';', '\t', '|' };
            foreach (char c in _tests)
            {
                if (IsCsv(reader, c, sampleRows))
                {
                    separator = c;
                    return true;
                }
            }
            separator = ' ';
            return false;
        }

        /// <summary>
        /// Retrieves CSV separator from text.
        /// </summary>
        public static bool GetCsvSeparator(string text, out char separator)
        {
            char[] _tests = new char[] { ',', ';', '\t', '|' };
            foreach (char c in _tests)
            {
                if (IsCsv(text, c, 20))
                {
                    separator = c;
                    return true;
                }
            }
            separator = ' ';
            return false;
        }

        /// <summary>
        /// Test whether piece of text is CSV
        /// </summary>
        public static bool IsCsv(string text, char separator, int sampleRows)
        {
            byte[] byteArray = Encoding.Default.GetBytes(text);
            var _columnCount = new List<int>();

            using (MemoryStream _stream = new MemoryStream(byteArray))
            {
                using (CsvStreamReader _reader = new CsvStreamReader(_stream))
                {
                    return IsCsv(_reader, separator, sampleRows);
                }
            }
        }

        /// <summary>
        /// Test whether piece of text is CSV
        /// </summary>
        public static bool IsCsv(CsvStreamReader reader, char separator, int sampleRows)
        {
            List<int> _columnCount = new List<int>();

            reader.Position = 0;
            reader.Separator = separator;
            while (!reader.EndOfCsvStream)
            {
                string[] _items = reader.ReadCsvLine().ToArray();
                int _length = _items.Length == 1 && string.IsNullOrWhiteSpace(_items[0]) ? 0 : _items.Length;
                if (_length > 0)
                {
                    _columnCount.Add(_length);
                }
            }
            int prevcount = _columnCount.First();
            foreach (int count in _columnCount)
            {
                if (count == 1)
                    return false;
                if (count != prevcount)
                    return false;
            }

            return true;
        }
    }
}