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
            {
                using CsvStreamReader _reader = new CsvStreamReader(_stream);
                {
                    return CsvHeader(_reader, separator);
                }
            }
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
        /// Retrieves CSV separator from text.
        /// </summary>
        public static bool GetCsvSeparator(CsvStreamReader reader, out char separator, int sampleRows)
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
        /// Retrieves CSV separator from text.
        /// </summary>
        public static bool GetCsvSeparator(string text, out char separator)
        {
            char[] _tests = new char[] { ',', ';', ':', '\t', '|', (char)1643, (char)1644, (char)1616, (char)1562, (char)890, (char)885, (char)825, (char)806, (char)716 };
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
            using MemoryStream _stream = new MemoryStream(byteArray);
            using CsvStreamReader _reader = new CsvStreamReader(_stream);
            return IsCsv(_reader, separator, sampleRows);
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



        private static char[] _EscapeChars = new char[] { '\r', '\n', '"', ',' };

        internal static string Escape(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            if (s.IndexOfAny(_EscapeChars) == -1)
            {
                return s;
            }

            if (s.IndexOf('"') == -1)
            {
                return $"\"{s}\""; // No need for replace, just surround with quotes.
            }
            return $"\"{s.Replace("\"", "\"\"")}\""; // Unusual replace when string contains '"' 
        }

        internal static string ToCsvString(params string[] values)
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

        private const int _CR = '\r';
        private const int _LF = '\n';
        private const int _ESCAPE = '"';
        private const int _TERMINATOR = -1;

        internal static string[] SplitCsvString(string s, char separator)
        {
            List<string> _result = new List<string>();
            StringBuilder _buffer = new StringBuilder();
            State _state = State.Normal;
            int _byte = 0;
            int _nextByte = 0;


            for (int ii = 0; ii < s.Length ; ii++)
            {
                _byte = s[ii];
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
                        if (ii + 1 < s.Length)
                        {
                            _nextByte = s[ii + 1];
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
                        if (ii + 1 < s.Length)
                        {
                            _nextByte = s[ii + 1];
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

        internal static int Lowest(int value1, int value2)
        {
            if (value1 < value2)
            {
                return value1;
            }
            return value2;
        }

    }
}