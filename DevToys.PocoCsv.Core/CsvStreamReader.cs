using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Implements a System.IO.TextReader that reads characters from a byte stream in a particular encoding.
    /// Extended with ReadCsvLine to read a Csv Line conform RFC 4180.
    /// </summary>
    public sealed class CsvStreamReader : StreamReader
    {
        private char _Separator = ',';
        private readonly StringBuilder _buffer = new StringBuilder(1027);
        private const int _CR = '\r';
        private const int _LF = '\n';
        private const int _ESCAPE = '"';
        private const int _TERMINATOR = -1;
        private int _byte = 0;
        private int _nextByte = 0;
        private State _state = State.Normal;
        private readonly List<string> _result = new List<string>();
        private string[] _ColumnNames = null;
        private int _CollIndex = 0;
        private int _IndexesIndex = 0;
        private int[] _Indexes = new int[0];

        /// <summary>
        /// Initializes a new instance of the System.IO.StreamReader class for the specified file name.
        /// </summary>
        /// <param name="path">The complete file path to be read.</param>
        public CsvStreamReader(string path) : base(path)
        { }

        /// <summary>
        /// Initializes a new instance of the System.IO.StreamReader class for the specified file name, with the specified character encoding, byte order mark detection option, and buffer size.
        /// </summary>
        /// <param name="path">The complete file path to be read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
        /// <param name="bufferSize">The minimum buffer size.</param>
        public CsvStreamReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize) : base(path, encoding, detectEncodingFromByteOrderMarks, bufferSize)
        { }

        /// <summary>
        /// Initializes a new instance of the System.IO.StreamReader class for the specified stream.
        /// </summary>
        /// <param name="stream">The stream to be read.</param>
        public CsvStreamReader(Stream stream) : base(stream)
        { }

        /// <summary>
        /// Initializes a new instance of the System.IO.StreamReader class for the specified stream based on the specified character encoding, byte order mark detection option, and buffer size, and optionally leaves the stream open.
        /// </summary>
        /// <param name="stream">The stream to be read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
        /// <param name="bufferSize">The minimum buffer size.</param>
        /// <param name="leaveOpen">true to leave the stream open after the System.IO.StreamReader object is disposed; otherwise, false.</param>
        public CsvStreamReader(Stream stream, Encoding encoding = null, bool detectEncodingFromByteOrderMarks = true, int bufferSize = -1, bool leaveOpen = false) : base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize, leaveOpen)
        { }

        /// <summary>
        /// Limit the result array for ReadCsvLine to only these columns.
        /// </summary>
        public void SetColumnIndexes(params int[] indexes)
        {
            if (indexes == null)
            {
                _Indexes = new int[0];
                return;
            }            
            _Indexes = indexes.OrderBy(p => p).ToArray();
            MoveToStart();
        }

        /// <summary>
        /// Reset the column indexes to default, including all columns in the result array.
        /// </summary>
        public void ResetColumnIndexes()
        {
            _Indexes = new int[0];
            MoveToStart();
        }

        /// <summary>
        /// Indicates the stream has ended.
        /// </summary>
        public new bool EndOfStream => _byte == _TERMINATOR;

        /// <summary>
        /// Returns the current line number
        /// </summary>
        public int CurrentLine { get; private set; }


        /// <summary>
        /// Get / Sets the position.
        /// </summary>
        public long Position
        {
            get => BaseStream.Position;
            internal set
            {
                MoveToPosition(value);
            }
        }

        /// <summary>
        /// Move reader to the start position 0
        /// </summary>
        public void MoveToStart() => MoveToPosition(0);

        /// <summary>
        /// Get / Sets the Separator character to use.
        /// </summary>
        public char Separator
        {
            get => _Separator;
            set => _Separator = value;
        }

        /// <summary>
        /// Returns a schema for the CSV with best fitted types to use.
        /// </summary>
        public IEnumerable<CsvColumnInfo> GetCsvSchema(int sampleRows)
        {
            Position = 0;
            var _schema = CsvUtils.GetCsvSchema(this, sampleRows);
            Position = 0;
            return _schema;
        }

        /// <summary>
        /// Detect the separator by sampling first 10 rows. Position is moved to start after execution.
        /// </summary>
        public void DetectSeparator()
        {
            var _succes = CsvUtils.GetCsvSeparator(this, out char separator, 10);
            if (_succes)
            {
                Separator = separator;
            }
            MoveToStart();
        }

        /// <summary>
        /// Detects and sets CSV Separator.
        /// </summary>
        public char GetCsvSeparator(int sampleRows)
        {
            Position = 0;
            bool _succes = CsvUtils.GetCsvSeparator(this, out char separator, sampleRows);
            Position = 0;

            if (!_succes)
            {
                throw new InvalidDataException("Csv does not have valid column counts.");
            }
            return separator;
        }

        /// <summary>
        /// Use to skip first row without materializing, usefull for skipping header.
        /// </summary>
        public void Skip(int rows = 1)
        {
            int ii = 0;

            while (!EndOfStream)
            {
                if (ii >= rows)
                {
                    break;
                }
                SkipRow();
                ii++;
            }
        }

        private void SkipRow()
        {
            _byte = 0;
            _nextByte = 0;
            _state = State.Normal;

            while (true)
            {
                _byte = Read();
                if (_state == State.Normal)
                {
                    if (_byte == _Separator)
                    {
                        continue;
                    }
                    else if (_byte == _CR)
                    {
                        _nextByte = Peek();
                        if (_nextByte == _LF)
                        {
                            continue;
                        }
                        break;
                    }
                    else if (_byte == _LF)
                    {
                        break;
                    }
                    else if (_byte == _ESCAPE)
                    {
                        _state = State.Escaped;
                        continue;
                    }
                    else if (_byte == _TERMINATOR)
                    {
                        break;
                    }
                    continue;
                }
                else if (_state == State.Escaped)
                {
                    if (_byte == _TERMINATOR)
                    {
                        break;
                    }
                    else if (_byte == _ESCAPE)
                    {
                        _nextByte = Peek();
                        if (_nextByte == _Separator || _nextByte == _CR || _nextByte == _LF)
                        {
                            _state = State.Normal;
                            continue;
                        }
                        else if (_nextByte == _TERMINATOR)
                        {
                            break;
                        }
                        else if (_nextByte == _ESCAPE)
                        {
                            _state = State.EscapedEscape;
                            continue;
                        }
                    }
                    continue;
                }
                else if (_state == State.EscapedEscape)
                {
                    _state = State.Escaped;
                    continue;
                }
            }
        }


        private void MoveToPosition(long position)
        {
            BaseStream.Position = position;
            _byte = 0;
        }

        /// <summary>
        /// Reads the CSV line into string array, and advances to the next.
        /// </summary>
        public string[] ReadCsvLine()
        {
            _result.Clear();
            _state = State.Normal;
            _buffer.Length = 0; // Clear the string buffer.
            _byte = 0;
            _nextByte = 0;
            _CollIndex = 0;
            _IndexesIndex = 0;

            while (true)
            {
                _byte = Read();
                if (_state == State.Normal)
                {
                    if (_byte == _Separator)
                    {
                        // End of field'
                        if (_Indexes.Length == 0 || _IndexesIndex < _Indexes.Length && _Indexes[_IndexesIndex] == _CollIndex)
                        {
                            _result.Add(_buffer.ToString());
                            _IndexesIndex++;
                        }
                        _buffer.Length = 0;
                        _CollIndex++;
                        continue;
                    }
                    else if (_byte == _CR)
                    {
                        _nextByte = Peek();
                        if (_nextByte == _LF)
                        {
                            continue; // goes to else if (_byte == '\n')
                        }
                        // end of line.
                        if (_Indexes.Length == 0  || _IndexesIndex < _Indexes.Length && _Indexes[_IndexesIndex] == _CollIndex)
                        {
                            _result.Add(_buffer.ToString());
                        }
                        _buffer.Length = 0;
                        CurrentLine++;
                        break;
                    }
                    else if (_byte == _LF)
                    {
                        // end of line.
                        if (_Indexes.Length == 0 || _IndexesIndex < _Indexes.Length && _Indexes[_IndexesIndex] == _CollIndex)
                        {
                            _result.Add(_buffer.ToString());
                        }
                        _buffer.Length = 0;
                        CurrentLine++;
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
                        // End of doc
                        if (_Indexes.Length == 0 || _IndexesIndex < _Indexes.Length && _Indexes[_IndexesIndex] == _CollIndex)
                        {
                            _result.Add(_buffer.ToString());
                        }
                        _buffer.Length = 0;
                        return _result.ToArray();
                    }
                    _buffer.Append((char)_byte);
                    continue;
                }
                else if (_state == State.Escaped)
                {
                    // ',' and '\r' and "" are part of the value.
                    if (_byte == _TERMINATOR)
                    {
                        // End of field
                        // Set the value
                        _buffer.Clear();
                        break; // end the while loop.
                    }
                    else if (_byte == _ESCAPE)
                    {
                        // " aaa "" ","bbb", "ccc""","ddd """" "
                        _nextByte = Peek();
                        if (_nextByte == _Separator || _nextByte == _CR || _nextByte == _LF)
                        {
                            // this quote is followed by a , so it ends the escape. we continue to next itteration where we read a ',' in nomral mode.
                            _state = State.Normal;
                            continue;
                        }
                        if (_nextByte == _TERMINATOR)
                        {
                            // this quote is followed by a , so it ends the escape. we continue to next itteration where we read a ',' in nomral mode.
                            if (_Indexes.Length == 0 || _IndexesIndex < _Indexes.Length && _Indexes[_IndexesIndex] == _CollIndex)
                            {
                                _result.Add(_buffer.ToString());
                            }
                            break;
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
            }
            return _result.ToArray();
        }

        /// <summary>
        /// Perform ReadCsvLine.
        /// </summary>
        public new string[] ReadLine() => ReadCsvLine();

        /// <summary>
        /// Each iteration will read the next row from stream or file
        /// </summary>
        public IEnumerable<string[]> ReadAsEnumerable()
        {
            while (_byte > _TERMINATOR)
            {
                yield return ReadCsvLine();
            }
        }

        /// <summary>
        /// Assumes first line is the Header with column names. 
        /// </summary>
        public Dictionary<string, string> ReadCsvLineAsDictionary()
        {
            if (_ColumnNames == null)
            {
                MoveToStart();
                _ColumnNames = ReadCsvLine();
            }

            _result.Clear();
            Dictionary<string, string> _Dict = new Dictionary<string, string>();
            _state = State.Normal;
            _buffer.Length = 0; // Clear the string buffer.
            _byte = 0;
            _nextByte = 0;
            _CollIndex = 0;
            _IndexesIndex = 0;

            while (true)
            {
                _byte = Read();
                if (_state == State.Normal)
                {
                    if (_byte == _Separator)
                    {
                        // End of field'
                        if (_Indexes.Length == 0 || _IndexesIndex < _Indexes.Length && _Indexes[_IndexesIndex] == _CollIndex)
                        {
                            _Dict.Add(_ColumnNames[_IndexesIndex], _buffer.ToString());
                            _IndexesIndex++;
                        }
                        _buffer.Length = 0;
                        _CollIndex++;
                        continue;
                    }
                    else if (_byte == _CR)
                    {
                        _nextByte = Peek();
                        if (_nextByte == _LF)
                        {
                            continue; // goes to else if (_byte == '\n')
                        }
                        // end of line.
                        if (_Indexes.Length == 0 || _IndexesIndex < _Indexes.Length && _Indexes[_IndexesIndex] == _CollIndex)
                        {
                            _Dict.Add(_ColumnNames[_IndexesIndex], _buffer.ToString());
                        }
                        _buffer.Length = 0;
                        CurrentLine++;
                        break;
                    }
                    else if (_byte == _LF)
                    {
                        // end of line.
                        if (_Indexes.Length == 0 || _IndexesIndex < _Indexes.Length && _Indexes[_IndexesIndex] == _CollIndex)
                        {
                            _Dict.Add(_ColumnNames[_IndexesIndex], _buffer.ToString());
                        }
                        _buffer.Length = 0;
                        CurrentLine++;
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
                        // End of doc
                        if (_Indexes.Length == 0 || _IndexesIndex < _Indexes.Length && _Indexes[_IndexesIndex] == _CollIndex)
                        {
                            _Dict.Add(_ColumnNames[_IndexesIndex], _buffer.ToString());
                        }
                        _buffer.Length = 0;
                        return _Dict;
                    }
                    _buffer.Append((char)_byte);
                    continue;
                }
                else if (_state == State.Escaped)
                {
                    // ',' and '\r' and "" are part of the value.
                    if (_byte == _TERMINATOR)
                    {
                        // End of field
                        // Set the value
                        _buffer.Clear();
                        break; // end the while loop.
                    }
                    else if (_byte == _ESCAPE)
                    {
                        // " aaa "" ","bbb", "ccc""","ddd """" "
                        _nextByte = Peek();
                        if (_nextByte == _Separator || _nextByte == _CR || _nextByte == _LF)
                        {
                            // this quote is followed by a , so it ends the escape. we continue to next itteration where we read a ',' in nomral mode.
                            _state = State.Normal;
                            continue;
                        }
                        if (_nextByte == _TERMINATOR)
                        {
                            // this quote is followed by a , so it ends the escape. we continue to next itteration where we read a ',' in nomral mode.
                            if (_Indexes.Length == 0 || _IndexesIndex < _Indexes.Length && _Indexes[_IndexesIndex] == _CollIndex)
                            {
                                _Dict.Add(_ColumnNames[_IndexesIndex], _buffer.ToString());
                            }
                            break;
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
            }
            return _Dict;
        }

        /// <summary>
        /// Assumes first line is the Header with column names. 
        /// </summary>
        public IEnumerable<Dictionary<string, string>> ReadAsEnumerableDictionary()
        {
            while (_byte != _TERMINATOR)
            {
                yield return ReadCsvLineAsDictionary();
            }
        }
    }
}