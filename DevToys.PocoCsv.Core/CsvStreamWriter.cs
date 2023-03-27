using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Implements a System.IO.TextWriter for writing characters to a stream in a particular encoding.
    /// Extended with WriteCsvLine to write a Csv Line conform RFC 4180.
    /// </summary>
    public sealed class CsvStreamWriter : StreamWriter
    {
        private StringBuilder _sb = new StringBuilder();

        /// <summary>
        /// Initializes a new instance of the System.IO.StreamWriter class for the specified file by using the default encoding and buffer size.
        /// </summary>
        /// <param name="path">The complete file path to write to. path can be a file name.</param>
        public CsvStreamWriter(string path) : base(path)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.IO.StreamWriter class for the specified file on the specified path, using the specified encoding and buffer size. If the file exists, it can be either overwritten or appended to. If the file does not exist, this constructor creates a new file.
        /// </summary>
        /// <param name="path">The complete file path to write to. path can be a file name.</param>
        /// <param name="append">true to append data to the file; false to overwrite the file. If the specified file does not exist, this parameter has no effect, and the constructor creates a new file.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="bufferSize">The buffer size, in bytes.</param>
        public CsvStreamWriter(string path, bool append = false, Encoding encoding = null, int bufferSize = -1) : base(path, append, encoding, bufferSize)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <param name="options"></param>
        public CsvStreamWriter(string path, Encoding encoding, FileStreamOptions options) : base(path, encoding, options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.IO.StreamWriter class for the specified stream by using UTF-8 encoding and the default buffer size.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        public CsvStreamWriter(Stream stream) : base(stream)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.IO.StreamWriter class for the specified file by using the default encoding and buffer size.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="bufferSize">The buffer size, in bytes.</param>
        /// <param name="leaveOpen">true to leave the stream open after the System.IO.StreamWriter object is disposed; otherwise, false.</param>
        public CsvStreamWriter(Stream stream, Encoding encoding = null, int bufferSize = -1, bool leaveOpen = false) : base(stream, encoding, bufferSize, leaveOpen)
        {
        }

        /// <summary>
        /// Csv Seperator to use default ','
        /// </summary>
        public char Separator { get; set; } = ',';

        /// <summary>
        /// \r\n = CR + LF → Used as a new line character in Windows.
        /// \r = CR(Carriage Return) → Used as a new line character in Mac OS before X.
        /// \n = LF(Line Feed) → Used as a new line character in Unix/Mac OS X
        /// </summary>
        public CRLFMode CRLFMode { get; set; } = CRLFMode.CRLF;

        /// <summary>
        /// Write an array of strings to the Csv Stream and escapes when nececary.
        /// </summary>
        /// <param name="values">Array of strings</param>
        public void WriteCsvLine(params string[] values)
        {
            _sb.Length = 0;

            if (BaseStream.Position > 0)
            {
                if (CRLFMode == CRLFMode.CRLF)
                {
                    _sb.Append("\r\n");
                }
                else if (CRLFMode == CRLFMode.CR)
                {
                    _sb.Append('\r');
                }
                else if (CRLFMode == CRLFMode.LF)
                {
                    _sb.Append('\n');
                }
            }

            for (int ii = 0; ii < values.Length; ii++)
            {
                _sb.Append(CsvUtils.Esc(Separator, values[ii] ?? "")).Append(Separator);
            }
            _sb.Length--;
            BaseStream.Write(Encoding.Default.GetBytes(_sb.ToString()), 0, _sb.Length);
        }

        /// <summary>
        /// Write an array of strings to the Csv Stream and escapes when nececary.
        /// </summary>
        /// <param name="values">Array of strings</param>
        public void WriteCsvLine(IEnumerable<string> values)
        {
            _sb.Length = 0;

            if (BaseStream.Position > 0)
            {
                if (CRLFMode == CRLFMode.CRLF)
                {
                    _sb.Append("\r\n");
                }
                else if (CRLFMode == CRLFMode.CR)
                {
                    _sb.Append('\r');
                }
                else if (CRLFMode == CRLFMode.LF)
                {
                    _sb.Append('\n');
                }
            }

            foreach (string value in values)
            {
                _sb.Append(CsvUtils.Esc(Separator, value ?? "")).Append(Separator);
            }
            _sb.Length--;
            BaseStream.Write(Encoding.Default.GetBytes(_sb.ToString()), 0, _sb.Length);
        }
    }
}