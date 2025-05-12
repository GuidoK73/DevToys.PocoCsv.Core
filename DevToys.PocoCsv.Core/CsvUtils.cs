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
        private const int _CR = '\r';
        private const int _ESCAPE = '"';
        private const int _LF = '\n';
        private static char[] _EscapeChars = new char[] { '\r', '\n', '"', ',' };

        /// <summary>
        /// Returns first row of CSV
        /// </summary>
        public static string[] CsvHeader(string text, char separator)
        {
            CsvSerializer serializer = new CsvSerializer(new CsvSerializerSettings() { Separator = separator });
            return serializer.Deserialize(text).FirstOrDefault();
        }

        /// <summary>
        /// Returns first row of CSV
        /// </summary>
        public static string[] CsvHeader(StringBuilder text, char separator)
        {
            CsvSerializer serializer = new CsvSerializer(new CsvSerializerSettings() { Separator = separator });
            return serializer.Deserialize(text).FirstOrDefault();
        }

        /// <summary>
        /// Returns first row of CSV
        /// </summary>
        public static string[] CsvHeader(CsvStreamReader reader, char separator)
        {
            reader.Position = 0;
            reader.Separator = separator;
            return reader.ReadCsvLine();
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

            while (!reader.EndOfStream && (_row < sampleRows))
            {
                string[] _items = reader.ReadCsvLine();
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
        /// Returns a schema for the CSV with best fitted types to use.
        /// </summary>
        public static IEnumerable<CsvColumnInfo> GetCsvSchema(string text, int sampleRows)
        {
            bool _succes = CsvUtils.GetCsvSeparator(text, out char _separator);
            if (!_succes)
                throw new ArgumentException("text is not valid csv!");

            string[] _header = CsvHeader(text, _separator);
            NetType[] _columnTypes = new NetType[_header.Length];
            bool[] _columnNullable = new bool[_header.Length];

            bool _first = true;

            CsvSerializer csvSerializer = new CsvSerializer(new CsvSerializerSettings() { Separator = _separator });

            foreach (string[] _items in csvSerializer.Deserialize(text).Take(sampleRows))
            {
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
        /// Returns a schema for the CSV with best fitted types to use.
        /// </summary>
        public static IEnumerable<CsvColumnInfo> GetCsvSchema(StringBuilder text, int sampleRows)
        {
            bool _succes = CsvUtils.GetCsvSeparator(text, out char _separator);
            if (!_succes)
                throw new ArgumentException("text is not valid csv!");

            string[] _header = CsvHeader(text, _separator);
            NetType[] _columnTypes = new NetType[_header.Length];
            bool[] _columnNullable = new bool[_header.Length];

            bool _first = true;

            CsvSerializer csvSerializer = new CsvSerializer(new CsvSerializerSettings() { Separator = _separator });

            foreach (string[] _items in csvSerializer.Deserialize(text).Take(sampleRows))
            {
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
        /// Guesses CSV separator from text.
        /// </summary>
        public static bool GetCsvSeparator(CsvStreamReader reader, out char separator, int sampleRows = 20)
        {
            char[] _tests = new char[] { ',', ';', ':', '\t', '|', (char)1643, (char)1644, (char)1616, (char)1562, (char)890, (char)885, (char)825, (char)806, (char)716 };
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
        /// Guesses CSV separator from text.
        /// </summary>
        public static bool GetCsvSeparator(string text, out char separator, int sampleRows = 20)
        {
            char[] _tests = new char[] { ',', ';', ':', '\t', '|', (char)1643, (char)1644, (char)1616, (char)1562, (char)890, (char)885, (char)825, (char)806, (char)716 };
            foreach (char c in _tests)
            {
                if (IsCsv(text, c, sampleRows))
                {
                    separator = c;
                    return true;
                }
            }
            separator = ' ';
            return false;
        }

        /// <summary>
        /// Guesses CSV separator from text.
        /// </summary>
        public static bool GetCsvSeparator(StringBuilder text, out char separator, int sampleRows = 20)
        {
            char[] _tests = new char[] { ',', ';', ':', '\t', '|', (char)1643, (char)1644, (char)1616, (char)1562, (char)890, (char)885, (char)825, (char)806, (char)716 };
            foreach (char c in _tests)
            {
                if (IsCsv(text, c, sampleRows))
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
        public static bool IsCsv(string text, char separator, int sampleRows = 20)
        {
            var _columnCount = new List<int>();

            CsvSerializer serializer = new CsvSerializer(new CsvSerializerSettings() { Separator = separator });

            int _row = 0;

            foreach (string[] _items in serializer.Deserialize(text).Take(sampleRows))
            {
                int _length = _items.Length == 1 && string.IsNullOrWhiteSpace(_items[0]) ? 0 : _items.Length;
                if (_length > 0)
                {
                    _columnCount.Add(_length);
                }
                _row++;
                if (_row > sampleRows)
                {
                    break;
                }
            }
            if (_columnCount.Count == 0)
            {
                return false;
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

        /// <summary>
        /// Test whether piece of text is CSV
        /// </summary>
        public static bool IsCsv(StringBuilder text, char separator, int sampleRows = 20)
        {
            var _columnCount = new List<int>();

            CsvSerializer serializer = new CsvSerializer(new CsvSerializerSettings() { Separator = separator });

            int _row = 0;

            foreach (string[] _items in serializer.Deserialize(text).Take(sampleRows))
            {
                int _length = _items.Length == 1 && string.IsNullOrWhiteSpace(_items[0]) ? 0 : _items.Length;
                if (_length > 0)
                {
                    _columnCount.Add(_length);
                }
                _row++;
                if (_row > sampleRows)
                {
                    break;
                }
            }
            if (_columnCount.Count == 0)
            {
                return false;
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

        /// <summary>
        /// Test whether piece of text is CSV
        /// </summary>
        public static bool IsCsv(CsvStreamReader reader, char separator, int sampleRows = 20)
        {
            var _columnCount = new List<int>();

            reader.Position = 0;
            reader.Separator = separator;
            int _row = 0;

            while (!reader.EndOfStream)
            {
                string[] _items = reader.ReadCsvLine();
                int _length = _items.Length == 1 && string.IsNullOrWhiteSpace(_items[0]) ? 0 : _items.Length;
                if (_length > 0)
                {
                    _columnCount.Add(_length);
                }
                _row++;
                if (_row > sampleRows)
                {
                    break;
                }
            }
            if (_columnCount.Count == 0)
            {
                return false;
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

        /// <summary>
        /// Join a string as a CSV line.
        /// </summary>
        public static string JoinCsvLine(params string[] values)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string value in values)
            {
                sb.Append(Escape(value));
                sb.Append(',');
            }
            sb.Length--;
            return sb.ToString();
        }

        /// <summary>
        /// Join a string as a CSV line.
        /// </summary>
        public static string JoinCsvLine(char separator, params string[] values)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string value in values)
            {
                sb.Append(Escape(value));
                sb.Append(separator);
            }
            sb.Length--;
            return sb.ToString();
        }

        /// <summary>
        /// Split a CSV based line.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string[] SplitCsvLine(string text, char separator)
        {
            List<string> _result = new List<string>();
            StringBuilder _buffer = new StringBuilder();
            State _state = State.Normal;
            int _byte = 0;
            int _nextByte = 0;

            for (int ii = 0; ii < text.Length; ii++)
            {
                _byte = text[ii];
                if (_state == State.Normal)
                {
                    if (_byte == separator)
                    {
                        // End of field
                        _result.Add(_buffer.ToString());
                        _buffer.Length = 0;
                        continue;
                    }
                    else if (_byte == _CR)
                    {
                        if (ii + 1 < text.Length)
                        {
                            _nextByte = text[ii + 1];
                            if (_nextByte == _LF)
                            {
                                continue; // goes to else if (_byte == '\n')
                            }
                        }
                        // end of line.
                        _result.Add(_buffer.ToString());
                        _buffer.Length = 0;
                        break;
                    }
                    else if (_byte == _LF)
                    {
                        // end of line.
                        _result.Add(_buffer.ToString());
                        _buffer.Length = 0;
                        break;
                    }
                    else if (_byte == _ESCAPE)
                    {
                        // switch mode
                        _state = State.Escaped;
                        continue; // do not add this char. (TRIM)
                    }
                    _buffer.Append((char)_byte);
                    continue;
                }
                else if (_state == State.Escaped)
                {
                    // ',' and '\r' and "" are part of the value.
                    if (_byte == _ESCAPE)
                    {
                        // " aaa "" ","bbb", "ccc""","ddd """" "
                        if (ii + 1 < text.Length)
                        {
                            _nextByte = text[ii + 1];
                            if (_nextByte == separator || _nextByte == _CR || _nextByte == _LF)
                            {
                                // this quote is followed by a , so it ends the escape. we continue to next itteration where we read a ',' in nomral mode.
                                _state = State.Normal;
                                continue;
                            }
                            else if (_nextByte == _ESCAPE)
                            {
                                _state = State.EscapedEscape;
                                continue; // Also do not add this char, we add it when we are in EscapedEscape mode and from their we turn back to normal Escape.  (basically adding one of two)
                            }
                        }
                    }
                    _buffer.Append((char)_byte);
                    continue;
                }
                else if (_state == State.EscapedEscape)
                {
                    _buffer.Append((char)_byte);
                    _state = State.Escaped;
                    continue;
                }
            }
            _result.Add(_buffer.ToString());

            return _result.ToArray();
        }

        internal static string Escape(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            if (text.IndexOfAny(_EscapeChars) == -1)
            {
                return text;
            }

            if (text.IndexOf('"') == -1)
            {
                return $"\"{text}\""; // No need for replace, just surround with quotes.
            }
            return $"\"{text.Replace("\"", "\"\"")}\""; // Unusual replace when string contains '"'
        }

        internal static string JoinCsvLine(int max, params string[] values)
        {
            StringBuilder sb = new StringBuilder();
            if (values.Length < max)
            {
                max = values.Length;
            }
            for (int ii = 0; ii < max; ii++)
            {
                sb.Append(Escape(values[ii]));
                sb.Append(',');
            }
            sb.Length--;
            return sb.ToString();
        }

        internal static int Lowest(int value1, int value2) => (value1 < value2) ? value1 : value2;
    }
}