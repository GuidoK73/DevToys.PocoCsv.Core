using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Implements a System.IO.TextReader that reads characters from a byte stream in a particular encoding.
    /// Extended with ReadCsvLine to read a Csv Line conform RFC 4180.
    /// </summary>
    public sealed class CsvStreamReader : StreamReader
    {
        private readonly CsvStreamHelper _StreamHelper = new CsvStreamHelper();

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


#if NET60 || NET70
        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="options"></param>
        public CsvStreamReader(string path, FileStreamOptions options) : base(path, Encoding.UTF8, true, options)
        { }
#endif

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
        /// Indicates end of the stream.
        /// </summary>
        public new bool EndOfStream
        {
            get
            {
                return _StreamHelper.EndOfStream;
            }
        }

        /// <summary>
        /// Get / Sets the position.
        /// </summary>
        public long Position
        {
            get => BaseStream.Position;
            internal set
            {
                _StreamHelper.MoveToPosition(BaseStream, value);
            }
        }

        /// <summary>
        /// Move reader to the start position 0
        /// </summary>
        public void MoveToStart()
        {
            _StreamHelper.MoveToPosition(BaseStream, 0);
        }

        /// <summary>
        /// Get / Sets the Separator character to use.
        /// </summary>
        public char Separator
        {
            get
            {
                return _StreamHelper.Separator;
            }
            set
            {
                _StreamHelper.Separator = value;
            }
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
        /// Detects and sets CSV Separator. over 10 sample rows
        /// </summary>
        public void DetectSeparator()
        {
            GetCsvSeparator(10);
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
        /// Use to skip first row without materializing, usefull for skipping header.
        /// </summary>
        public void Skip(int rows = 1)
        {
            int ii = 0;

            while (!_StreamHelper.EndOfStream)
            {
                if (ii >= rows)
                {
                    break;
                }
                _StreamHelper.Skip(BaseStream);
                ii++;
            }
        }

        /// <summary>
        /// Moves to the last X rows, use ReadCsvLine after this call.
        /// </summary>
        public void MoveToLast(int rows = 1)
        {
            _StreamHelper.MoveToLast(BaseStream, rows);
        }

        /// <summary>
        /// reads the CsvLine
        /// </summary>
        public IEnumerable<string> ReadCsvLine()
        {
            foreach (string field in _StreamHelper.ReadRow(BaseStream))
            {
                yield return field;
            }
        }
    }
}