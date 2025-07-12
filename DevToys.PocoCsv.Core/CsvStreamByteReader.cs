using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    public enum ByteType
    {
        Value = 0,
        FieldEnd = 1,
        LineEnd = 2,
        DocEnd = 4
    }

    public struct CsvByte
    {
        public int Byte { get; set; }

        public char Char { get; set; }

        public int CollIndex { get; set; }

        public long Position { get; set; }

        public ByteType CharType { get; set; } 
    }

    /// <summary>
    /// Implements a System.IO.TextReader that reads characters from a byte stream in a particular encoding.
    /// Extended with ReadCsvLine to read a Csv Line conform RFC 4180.
    /// </summary>
    public sealed class CsvStreamByteReader : StreamReader
    {
        private char _Separator = ',';
        private const int _CR = '\r';
        private const int _LF = '\n';
        private const int _ESCAPE = '"';
        private const int _TERMINATOR = -1;
        private int _byte = 0;
        private int _nextByte = 0;
        private State _state = State.Normal;
        private int _CollIndex = 0;

        /// <summary>
        /// Initializes a new instance of the System.IO.StreamReader class for the specified file name.
        /// </summary>
        /// <param name="path">The complete file path to be read.</param>
        public CsvStreamByteReader(string path) : base(path)
        { }

        /// <summary>
        /// Initializes a new instance of the System.IO.StreamReader class for the specified file name, with the specified character encoding, byte order mark detection option, and buffer size.
        /// </summary>
        /// <param name="path">The complete file path to be read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
        /// <param name="bufferSize">The minimum buffer size.</param>
        public CsvStreamByteReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize) : base(path, encoding, detectEncodingFromByteOrderMarks, bufferSize)
        {
            if (bufferSize < 256)
            {
                bufferSize = 256;
            }
        }

        /// <summary>
        /// Initializes a new instance of the System.IO.StreamReader class for the specified stream.
        /// </summary>
        /// <param name="stream">The stream to be read.</param>
        public CsvStreamByteReader(Stream stream) : base(stream)
        { }

        /// <summary>
        /// Initializes a new instance of the System.IO.StreamReader class for the specified stream based on the specified character encoding, byte order mark detection option, and buffer size, and optionally leaves the stream open.
        /// </summary>
        /// <param name="stream">The stream to be read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
        /// <param name="bufferSize">The minimum buffer size.</param>
        /// <param name="leaveOpen">true to leave the stream open after the System.IO.StreamReader object is disposed; otherwise, false.</param>
        public CsvStreamByteReader(Stream stream, Encoding encoding = null, bool detectEncodingFromByteOrderMarks = true, int bufferSize = -1, bool leaveOpen = false) : base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize, leaveOpen)
        {
            if (bufferSize < 256)
            {
                bufferSize = 256;
            }
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

        private void MoveToPosition(long position)
        {
            BaseStream.Position = position;
            _byte = 0;
        }

        private CsvByte _resultByte = new CsvByte();

        /// <summary>
        /// Reads the CSV line into string array, and advances to the next.
        /// </summary>
        public CsvByte ReadCsvByte()
        {
            _state = State.Normal;
            _byte = 0;
            _nextByte = 0;
            _CollIndex = 0;


        MoveNextByte:

            _byte = Read();
            if (_state == State.Normal)
            {
                if (_byte == _Separator)
                {
                    // End of field'
                    _resultByte.Position = BaseStream.Position;
                    _resultByte.Byte = _byte;
                    _resultByte.Char = (char)_byte;
                    _resultByte.CharType = ByteType.FieldEnd;
                    _resultByte.CollIndex = _CollIndex;
                    _CollIndex++;
                    return _resultByte;

                }
                else if (_byte == _CR)
                {
                    _nextByte = Peek();
                    if (_nextByte == _LF)
                    {
                        goto MoveNextByte; // goes to else if (_byte == '\n')
                    }
                    // end of line.
                    _resultByte.Position = BaseStream.Position;
                    _resultByte.Byte = _byte;
                    _resultByte.Char = (char)_byte;
                    _resultByte.CharType = ByteType.LineEnd;
                    _resultByte.CollIndex = _CollIndex;
                    _CollIndex++;
                    return _resultByte;
                }
                else if (_byte == _LF)
                {
                    // end of line.
                    _resultByte.Position = BaseStream.Position;
                    _resultByte.Byte = _byte;
                    _resultByte.Char = (char)_byte;
                    _resultByte.CharType = ByteType.LineEnd;
                    _resultByte.CollIndex = _CollIndex;
                    _CollIndex++;
                    return _resultByte;
                }
                else if (_byte == _ESCAPE)
                {
                    // switch mode
                    //continue; // do not add this char. (TRIM)
                    _state = State.Escaped;
                    goto MoveNextByte;
                    
                }
                else if (_byte == _TERMINATOR)
                {
                    // End of doc
                    _resultByte.Position = BaseStream.Position;
                    _resultByte.Byte = _byte;
                    _resultByte.Char = (char)_byte;
                    _resultByte.CharType = ByteType.DocEnd;
                    _resultByte.CollIndex = _CollIndex;
                    _CollIndex++;
                    return _resultByte;
                }
                _resultByte.Position = BaseStream.Position;
                _resultByte.Byte = _byte;
                _resultByte.Char = (char)_byte;
                _resultByte.CharType = ByteType.Value;
                _resultByte.CollIndex = _CollIndex;
                _CollIndex++;
                return _resultByte;
            }
            else if (_state == State.Escaped)
            {
                // ',' and '\r' and "" are part of the value.
                if (_byte == _TERMINATOR)
                {
                    // End of doc
                    _resultByte.Position = BaseStream.Position;
                    _resultByte.Byte = _byte;
                    _resultByte.Char = (char)_byte;
                    _resultByte.CharType = ByteType.DocEnd;
                    _resultByte.CollIndex = _CollIndex;
                    _CollIndex++;
                    return _resultByte;
                }
                else if (_byte == _ESCAPE)
                {
                    // " aaa "" ","bbb", "ccc""","ddd """" "
                    _nextByte = Peek();
                    if (_nextByte == _Separator || _nextByte == _CR || _nextByte == _LF)
                    {
                        // this quote is followed by a , so it ends the escape. we continue to next itteration where we read a ',' in nomral mode.
                        _state = State.Normal;
                        goto MoveNextByte;
                    }
                    if (_nextByte == _TERMINATOR)
                    {
                        // this quote is followed by a , so it ends the escape. we continue to next itteration where we read a ',' in nomral mode.
                        // End of doc
                        _resultByte.Position = BaseStream.Position;
                        _resultByte.Byte = _byte;
                        _resultByte.Char = (char)_byte;
                        _resultByte.CharType = ByteType.DocEnd;
                        _resultByte.CollIndex = _CollIndex;
                        _CollIndex++;
                        return _resultByte;
                    }

                    else if (_nextByte == _ESCAPE)
                    {
                        _state = State.Escaped;
                        goto MoveNextByte; // Also do not add this char, we add it when we are in EscapedEscape mode and from their we turn back to normal Escape.  (basically adding one of two)
                    }
                }
                _resultByte.Position = BaseStream.Position;
                _resultByte.Byte = _byte;
                _resultByte.Char = (char)_byte;
                _resultByte.CharType = ByteType.Value;
                _resultByte.CollIndex = _CollIndex;
                _CollIndex++;
                return _resultByte;
            }
            else if (_state == State.EscapedEscape)
            {
                _state = State.Escaped;

                _resultByte.Position = BaseStream.Position;
                _resultByte.Byte = _byte;
                _resultByte.Char = (char)_byte;
                _resultByte.CharType = ByteType.Value;
                _resultByte.CollIndex = _CollIndex;
                _CollIndex++;
                return _resultByte;
            }


            _resultByte.Position = BaseStream.Position;
            _resultByte.Byte = _byte;
            _resultByte.Char = (char)_byte;
            _resultByte.CharType = ByteType.DocEnd;
            _resultByte.CollIndex = _CollIndex;
            _CollIndex++;
            return _resultByte;
        }
    }
}