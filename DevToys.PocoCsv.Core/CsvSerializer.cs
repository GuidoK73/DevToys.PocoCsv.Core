using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevToys.PocoCsv.Core
{ 
    /// <summary>
    /// Csv serialization/deserialization from and to strings.
    /// </summary>
    public sealed class CsvSerializer
    {
        private const int _CR = '\r';
        private const string _CRLF = "\r\n";
        private const int _ESCAPE = '"';
        private const int _LF = '\n';
        private const int _TERMINATOR = -1;
        private char[] _EscapeChars = null;
        public CsvSerializer()
        { }

        public CsvSerializer(CsvSerializerSettings settings)
        {
            Settings = settings;
        }

        public CsvSerializerSettings Settings { get; private set; } = new CsvSerializerSettings();
        /// <summary>
        /// Deserializes a CSV string to IEnumerable string[]
        /// </summary>
        /// <param name="data">string to convert</param>
        /// <param name="seperator">separator to use</param>
        /// <param name="selectIndexes">columns to select starting at 0, specify none for all columns</param>
        /// <returns></returns>
        public IEnumerable<string[]> Deserialize(string data, params int[] selectIndexes)
        {
            int _lastposition = data.Length;
            int _position = 0;
            char _separator = Settings.Separator;
            if (Settings.DetectSeparator)
            {
                CsvUtils.GetCsvSeparator(data, out _separator);
            }
            while (_position < _lastposition)
            {
                string[] _result = Deserialize(data, ref _position, _separator, selectIndexes);
                yield return _result;
            }
        }

        /// <summary>
        /// Deserialize a CSV formatted string to a collection.
        /// </summary>
        public IEnumerable<T> DeserializeObject<T>(string data) where T : class, new()
        {
            CsvStringReader<T> _reader = new CsvStringReader<T>(data, Settings.Separator);
            _reader.Culture = Settings.Culture;
            _reader.DetectSeparator = Settings.DetectSeparator;
            if (Settings.Header)
            {
                return _reader.ReadAsEnumerable().Skip(1);
            }
            return _reader.ReadAsEnumerable();
        }

        /// <summary>
        /// Serialize a IEnumerable string[] collection to CSV.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="selectIndexes"></param>
        /// <returns></returns>
        public string Serialize(IEnumerable<string[]> data, params int[] selectIndexes)
        {
            InitializeEscapeChars();
            StringBuilder _sb = new StringBuilder();
            foreach (var item in data)
            {
                WriteCsvLine(ref _sb, item, selectIndexes);
            }
            return _sb.ToString();
        }

        /// <summary>
        /// Serialize a collection to string in CSV format.
        /// </summary>
        public string SerializeObject<T>(IEnumerable<T> items) where T : class, new()
        {
            CsvStringWriter<T> _writer = new CsvStringWriter<T>(Settings.Culture, Settings.Separator);
            _writer.Culture = Settings.Culture;
            _writer.CRLFMode = Settings.CRLFMode;
            if (Settings.Header)
            {
                _writer.WriteHeader();
            }
            return _writer.Write(items).ToString();
        }

        /// <summary>
        /// Serialize a collection to string in CSV format.
        /// </summary>
        public StringBuilder SerializeObject<T>(StringBuilder target, IEnumerable<T> items) where T : class, new()
        {
            CsvStringWriter<T> _writer = new CsvStringWriter<T>(target, Settings.Culture, Settings.Separator);
            _writer.CRLFMode = Settings.CRLFMode;
            if (Settings.Header)
            {
                _writer.WriteHeader();
            }
            return _writer.Write(items);
        }

        private string[] Deserialize(string data, ref int position, char seperator, params int[] selectIndexes)
        {
            StringBuilder _buffer = new StringBuilder(1027);

            int _byte = 0;
            int _nextByte = 0;
            State _state = State.Normal;
            List<string> _result = new List<string>();
            int _collIndex = 0;
            int _selectIndexesIndex = 0;

            int _resultPosition = position;

            for (int ii = position; ii < data.Length; ii++)
            {
                _resultPosition = ii;
                _byte = (int)data[ii];
                if (_state == State.Normal)
                {
                    if (_byte == seperator)
                    {
                        // End of field'
                        if (selectIndexes.Length == 0 || _selectIndexesIndex < selectIndexes.Length && selectIndexes[_selectIndexesIndex] == _collIndex)
                        {
                            _result.Add(_buffer.ToString());
                            _selectIndexesIndex++;
                        }
                        _buffer.Length = 0;
                        _collIndex++;
                        continue;
                    }
                    else if (_byte == _CR)
                    {
                        _nextByte = Peek(ref data, ii);
                        if (_nextByte == _LF)
                        {
                            continue; // goes to else if (_byte == '\n')
                        }
                        // end of line.
                        if (selectIndexes.Length == 0 || _selectIndexesIndex < selectIndexes.Length && selectIndexes[_selectIndexesIndex] == _collIndex)
                        {
                            _result.Add(_buffer.ToString());
                        }
                        _buffer.Length = 0;
                        break;
                    }
                    else if (_byte == _LF)
                    {
                        // end of line.
                        if (selectIndexes.Length == 0 || _selectIndexesIndex < selectIndexes.Length && selectIndexes[_selectIndexesIndex] == _collIndex)
                        {
                            _result.Add(_buffer.ToString());
                        }
                        _buffer.Length = 0;
                        break;
                    }
                    else if (_byte == _ESCAPE)
                    {
                        // switch mode
                        _state = State.Escaped;
                        continue; // do not add this char. (TRIM)
                    }
                    else if (_byte == _TERMINATOR)
                    {
                        break; // end the while loop.
                    }
                    _buffer.Append((char)_byte);
                    continue;
                }
                else if (_state == State.Escaped)
                {
                    // ',' and '\r' and "" are part of the value.
                    if (_byte == _TERMINATOR)
                    {
                        break; // end the while loop.
                    }
                    else if (_byte == _ESCAPE)
                    {
                        // " aaa "" ","bbb", "ccc""","ddd """" "
                        _nextByte = Peek(ref data, ii);
                        if (_nextByte == seperator || _nextByte == _CR || _nextByte == _LF)
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
                    _buffer.Append((char)_byte);
                    continue;
                }
                else if (_state == State.EscapedEscape)
                {
                    _buffer.Append((char)_byte);
                    _state = State.Escaped;
                    continue;
                }
                else if (_byte == _TERMINATOR)
                {
                    break;
                }
            }

            _resultPosition++;
            position = _resultPosition;
            if (_buffer.Length > 0)
            {
                if (selectIndexes.Length == 0 || _selectIndexesIndex < selectIndexes.Length && selectIndexes[_selectIndexesIndex] == _collIndex)
                {
                    _result.Add(_buffer.ToString());
                }
                _buffer.Length = 0;
            }
            return _result.ToArray();
        }

        private string Escape(string s)
        {
            if (s.IndexOfAny(_EscapeChars) == -1)
            {
                return s;
            }
            if (s.IndexOf('"') == -1)
            {
                return $"\"{s}\""; // No need for replace.
            }
            return $"\"{s.Replace("\"", "\"\"")}\"";
        }

        private void InitializeEscapeChars()
        {
            _EscapeChars = new char[] { '\r', '\n', '"', Settings.Separator };
        }

        private int Peek(ref string data, int position)
        {
            if (position + 1 > data.Length - 1)
            {
                return _TERMINATOR;
            }
            return (int)data[position + 1];
        }

        /// <summary>
        /// Write an array of strings to the Csv Stream and escapes when nececary.
        /// </summary>
        /// <param name="values">Array of strings</param>
        private void WriteCsvLine(ref StringBuilder sb, string[] values, params int[] selectIndexes)
        {
            int _selectIndexesIndex = 0;

            if (sb.Length > 0)
            {
                if (Settings.CRLFMode == CRLFMode.CRLF)
                {
                    sb.Append(_CRLF);
                }
                else if (Settings.CRLFMode == CRLFMode.CR)
                {
                    sb.Append(_CR);
                }
                else if (Settings.CRLFMode == CRLFMode.LF)
                {
                    sb.Append(_LF);
                }
            }

            bool _first = true;
            for (int ii = 0; ii < values.Length; ii++)
            {
                if (selectIndexes.Length == 0 || _selectIndexesIndex < selectIndexes.Length && selectIndexes[_selectIndexesIndex] == ii)
                {
                    if (_first == false)
                    {
                        sb.Append(Settings.Separator);
                    }
                    _first = false;
                    sb.Append(Escape(values[ii] ?? ""));
                    _selectIndexesIndex++;
                }
            }
        }
    }
}