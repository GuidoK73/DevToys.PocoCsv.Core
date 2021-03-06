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
        /// <param name="path">The complete file path to write to. path can be a file name.</param>
        public CsvStreamWriter(string path) : base(path)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.IO.StreamWriter class for the specified stream by using the specified encoding and the default buffer size.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public CsvStreamWriter(Stream stream, Encoding encoding) : base(stream, encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.IO.StreamWriter class for the specified file by using the default encoding and buffer size. If the file exists, it can be either overwritten or appended to. If the file does not exist, this constructor creates a new file.
        /// </summary>
        /// <param name="path">The complete file path to write to. path can be a file name.</param>
        /// <param name="append">true to append data to the file; false to overwrite the file. If the specified file does not exist, this parameter has no effect, and the constructor creates a new file.</param>
        public CsvStreamWriter(string path, bool append) : base(path, append)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.IO.StreamWriter class for the specified stream by using the specified encoding and buffer size.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="bufferSize">The buffer size, in bytes.</param>
        public CsvStreamWriter(Stream stream, Encoding encoding, int bufferSize) : base(stream, encoding, bufferSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.IO.StreamWriter class for the specified file by using the specified encoding and default buffer size. If the file exists, it can be either overwritten or appended to. If the file does not exist, this constructor creates a new file.
        /// </summary>
        /// <param name="path">The complete file path to write to. path can be a file name.</param>
        /// <param name="append">true to append data to the file; false to overwrite the file. If the specified file does not exist, this parameter has no effect, and the constructor creates a new file.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public CsvStreamWriter(string path, bool append, Encoding encoding) : base(path, append, encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.IO.StreamWriter class for the specified file by using the default encoding and buffer size.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="bufferSize">The buffer size, in bytes.</param>
        /// <param name="leaveOpen">true to leave the stream open after the System.IO.StreamWriter object is disposed; otherwise, false.</param>
        public CsvStreamWriter(Stream stream, Encoding? encoding = null, int bufferSize = -1, bool leaveOpen = false) : base(stream, encoding, bufferSize, leaveOpen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.IO.StreamWriter class for the specified file on the specified path, using the specified encoding and buffer size. If the file exists, it can be either overwritten or appended to. If the file does not exist, this constructor creates a new file.
        /// </summary>
        /// <param name="path">The complete file path to write to. path can be a file name.</param>
        /// <param name="append">true to append data to the file; false to overwrite the file. If the specified file does not exist, this parameter has no effect, and the constructor creates a new file.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="bufferSize">The buffer size, in bytes.</param>
        public CsvStreamWriter(string path, bool append, Encoding encoding, int bufferSize) : base(path, append, encoding, bufferSize)
        {
        }

        /// <summary>
        /// Csv Seperator to use default ','
        /// </summary>
        public char Separator { get; set; } = ',';

        /// <summary>
        /// Write an array of strings to the Csv Stream and escapes when nececary.
        /// </summary>
        /// <param name="values">Array of strings</param>
        public void WriteCsvLine(params string[] values)
        {
            var _sb = new StringBuilder().Append((BaseStream.Position > 0) ? "\r\n" : "");
            for (int ii = 0; ii < values.Length; ii++)
            {
                _sb.Append(Esc(values[ii] ?? "")).Append(Separator);
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
            var _sb = new StringBuilder().Append((BaseStream.Position > 0) ? "\r\n" : "");
            foreach (string value in values)
            {
                _sb.Append(Esc(value ?? "")).Append(Separator);
            }
            _sb.Length--;
            BaseStream.Write(Encoding.Default.GetBytes(_sb.ToString()), 0, _sb.Length);
        }

        private string Esc(string s) => (s.IndexOfAny(new char[] { '\r', '\n', '"', Separator }) == -1) ? s : $"\"{s.Replace("\"", "\"\"")}\"";
    }
}