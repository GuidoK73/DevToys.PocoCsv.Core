using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Implements a System.IO.TextReader that reads characters from a byte stream in a particular encoding.
    /// Extended with ReadCsvLine to read a Csv Line conform RFC 4180.
    /// </summary>
    public sealed class CsvStreamReader : StreamReader
    {

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
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="options"></param>
        public CsvStreamReader(string path, FileStreamOptions options) : base(path, Encoding.UTF8, true, options)
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

        private enum State
        { First = 0, Normal = 1, Escaped = 2 }

        /// <summary>
        /// Indicates end of Csv Stream.
        /// </summary>
        public bool EndOfCsvStream => (BaseStream.Position >= BaseStream.Length);

        /// <summary>
        /// Get / Sets the position.
        /// </summary>
        public long Position
        {
            get => BaseStream.Position;
            set => BaseStream.Position = value;
        }

        /// <summary>
        /// Get / Sets the Separator character to use.
        /// </summary>
        public char Separator { get; set; } = ',';

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

            Separator = separator;

            return separator;
        }

        /// <summary>
        /// Each iteration steps to the next cell till the end of the CSV line.
        /// </summary>
        public IEnumerable<string> ReadCsvLine()
        {
            var _state = State.First;
            var _sb = new StringBuilder();

            while (true)
            {
                var _char = (char)BaseStream.ReadByte();
                if (_char == char.MaxValue || (_state == State.Normal && (_char == '\r' || _char == '\n')))
                {
                    yield return _sb.ToString().Trim(new char[] { '"' });
                    break;
                }
                if (_state == State.First)
                {
                    _state = State.Normal;
                    if (_char == '\n')
                        continue;
                }
                if (_state == State.Normal && _char == Separator)
                {
                    yield return _sb.ToString().Trim(new char[] { '"' });
                    _sb.Clear();
                    continue;
                }
                if (_char == '"')
                    _state = (_state == State.Normal) ? State.Escaped : State.Normal;

                _sb.Append(_char);
            }
        }
    }
}