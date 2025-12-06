using System.Collections.Generic;
using System.IO;
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
        
        /// <summary>
        /// 
        /// </summary>
        public CsvSerializer()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        public CsvSerializer(CsvSerializerSettings settings)
        {
            Settings = settings;
        }

        /// <summary>
        /// 
        /// </summary>
        public CsvSerializerSettings Settings { get; private set; } = new CsvSerializerSettings();
        /// <summary>
        /// Deserializes a CSV string to IEnumerable string[]
        /// </summary>
        /// <param name="data">string to convert</param>
        /// <param name="selectIndexes">columns to select starting at 0, specify none for all columns</param>
        /// <returns></returns>
        public IEnumerable<string[]> Deserialize(string data, params int[] selectIndexes)
        {
            int _lastposition = data.Length;
            int _position = 0;
            char _separator = Settings.Separator;
            int _bufferSize = Settings.BufferSize;
            if (Settings.DetectSeparator)
            {
                CsvUtils.GetCsvSeparator(data, out _separator);
            }
            while (_position < _lastposition)
            {
                string[] _result = Deserialize(data, ref _position, _separator, _bufferSize, selectIndexes);
                yield return _result;
            }
        }

        /// <summary>
        /// Deserializes a CSV string to IEnumerable string[]
        /// </summary>
        /// <param name="data">string to convert</param>
        /// <param name="selectIndexes">columns to select starting at 0, skip for all columns</param>
        /// <returns></returns>
        public IEnumerable<string[]> Deserialize(StringBuilder data, params int[] selectIndexes)
        {
            int _lastposition = data.Length;
            int _position = 0;
            char _separator = Settings.Separator;
            int _bufferSize = Settings.BufferSize;
            if (Settings.DetectSeparator)
            {
                CsvUtils.GetCsvSeparator(data, out _separator);
            }
            while (_position < _lastposition)
            {
                string[] _result = Deserialize(data, ref _position, _separator, _bufferSize, selectIndexes);
                yield return _result;
            }
        }

        /// <summary>
        /// Deserialize a CSV formatted file to a collection.
        /// WARNING: FULLY CONSUME FILE FOR PROPER FILE DISPOSAL.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="selectIndexes"></param>
        /// <returns></returns>
        public IEnumerable<string[]> Deserialize(FileInfo file, params int[] selectIndexes)
        {
            using (CsvStreamReader _reader = new CsvStreamReader(file.FullName) { Separator = Settings.Separator  })
            {
                _reader.SetColumnIndexes(selectIndexes);
                if (Settings.DetectSeparator)
                {
                    _reader.DetectSeparator();
                }


                while (!_reader.EndOfStream)
                {
                    string[] _resultArray = _reader.ReadCsvLine();
                    yield return _resultArray;
                }
            }
        }

        /// <summary>
        /// Deserialize a CSV formatted string to a collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public IEnumerable<T> DeserializeObject<T>(StringBuilder data) where T : class, new()
        {
            CsvStringReader<T> _reader = new CsvStringReader<T>(data, Settings.Separator, Settings.BufferSize);
            _reader.Culture = Settings.Culture;
            _reader.DetectSeparator = Settings.DetectSeparator;
            if (Settings.Header)
            {
                return _reader.ReadAsEnumerable().Skip(1);
            }
            return _reader.ReadAsEnumerable();
        }

        /// <summary>
        /// Deserialize a CSV formatted string to a collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public IEnumerable<T> DeserializeObject<T>(string data) where T : class, new()
        {
            StringBuilder _sb = new StringBuilder(data);
            CsvStringReader<T> _reader = new CsvStringReader<T>(_sb, Settings.Separator);
            _reader.Culture = Settings.Culture;
            _reader.DetectSeparator = Settings.DetectSeparator;
            if (Settings.Header)
            {
                return _reader.ReadAsEnumerable().Skip(1);
            }
            return _reader.ReadAsEnumerable();
        }


        /// <summary>
        /// Deserialize a CSV formatted file to a collection.
        /// WARNING: FULLY CONSUME FILE FOR PROPER FILE DISPOSAL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> DeserializeObject<T>(FileInfo file) where T : class, new() => DeserializeObject<T>((FileSystemInfo)file);

        /// <summary>
        /// Deserialize a CSV formatted file to a collection.
        /// WARNING: FULLY CONSUME FILE FOR PROPER FILE DISPOSAL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> DeserializeObject<T>(DirectoryInfo file) where T : class, new() => DeserializeObject<T>((FileSystemInfo)file);

        private IEnumerable<T> DeserializeObject<T>(FileSystemInfo fileOrDirectory) where T : class, new()
        {
            using (CsvReader<T> _reader = new CsvReader<T>(fileOrDirectory.FullName)
            {
                Separator = Settings.Separator,
                Culture = Settings.Culture,
                EmptyLineBehaviour = EmptyLineBehaviour.SkipAndReadNext
            })
            {
                if (Settings.DetectSeparator)
                {
                    _reader.DetectSeparator();
                }

                foreach (T item in _reader.ReadAsEnumerable())
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Serialize a IEnumerable string[] collection to CSV file.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="items"></param>
        public void Serialize(FileInfo file, IEnumerable<string> items, params int[] selectIndexes)
        {
            using (CsvStreamWriter _writer = new CsvStreamWriter(file.FullName)
            {
                Separator = Settings.Separator,
                CRLFMode = Settings.CRLFMode,
            })
            {
                _writer.SetColumnIndexes(selectIndexes);
                _writer.Write(items);
            }
        }

        /// <summary>
        /// Serialize a IEnumerable string[] collection to CSV.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="selectIndexes">Limit the result array to only these columns.</param>
        /// <returns></returns>
        public string Serialize(IEnumerable<string[]> data, params int[] selectIndexes)
        {
            StringBuilder _sb = new StringBuilder();
            Serialize(data, ref _sb, selectIndexes);
            return _sb.ToString();
        }

        /// <summary>
        /// Serialize a IEnumerable string[] collection to CSV.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="result"></param>
        /// <param name="selectIndexes">Limit the result array to only these columns.</param>
        public void Serialize(IEnumerable<string[]> data, ref StringBuilder result, params int[] selectIndexes)
        {
            InitializeEscapeChars();
            foreach (var item in data)
            {
                WriteCsvLine(ref result, item, selectIndexes);
            }
        }

        /// <summary>
        /// Serialize a collection to string in CSV format.
        /// </summary>
        /// <param name="items">Items collection to serialize.</param>
        /// <returns></returns>
        public string SerializeObject<T>(IEnumerable<T> items) where T : class, new()
        {
            var _writer = new CsvStringWriter<T>(Settings.Culture, Settings.Separator);
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
        /// <typeparam name="T"></typeparam>
        /// <param name="target">Target StringBuilder to write the result to.</param>
        /// <param name="items">Items collection to serialize.</param>
        /// <returns></returns>
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


        /// <summary>
        /// Serialize a IEnumerable T collection to CSV file in folder. filename based on T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="directoryPath"></param>
        /// <param name="items"></param>
        public void SerializeObject<T>(DirectoryInfo directoryPath, IEnumerable<T> items) where T : class, new() => SerializeObject<T>((FileSystemInfo)directoryPath, items);


        /// <summary>
        /// Serialize a IEnumerable T collection to CSV file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <param name="items"></param>
        public void SerializeObject<T>(FileInfo file, IEnumerable<T> items) where T : class, new() => SerializeObject<T>((FileSystemInfo)file, items);


        private void SerializeObject<T>(FileSystemInfo fileOrDirectory, IEnumerable<T> items) where T : class, new()
        {
            using (CsvWriter<T> _writer = new CsvWriter<T>(fileOrDirectory.FullName)
            {
                Separator = Settings.Separator,
                Culture = Settings.Culture,
                CRLFMode = Settings.CRLFMode,
            })
            {
                _writer.Write(items);
            }
        }

        private string[] Deserialize(string data, ref int position, char seperator, int bufferSize, params int[] selectIndexes)
        {
            StringBuilder _buffer = new StringBuilder(bufferSize);

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

        private string[] Deserialize(StringBuilder data, ref int position, char seperator, int bufferSize, params int[] selectIndexes)
        {
            StringBuilder _buffer = new StringBuilder(bufferSize);

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

        private int Peek(ref StringBuilder data, int position)
        {
            if (position + 1 > data.Length - 1)
            {
                return _TERMINATOR;
            }
            return (int)data[position + 1];
        }

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