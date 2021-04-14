using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevToys.PocoCsv.Core
{
    public sealed class CsvStreamWriter : StreamWriter
    {
        public CsvStreamWriter(Stream stream) : base(stream) { }

        public CsvStreamWriter(string path) : base(path) { }

        public CsvStreamWriter(Stream stream, Encoding encoding) : base(stream, encoding) { }

        public CsvStreamWriter(string path, bool append) : base(path, append) { }

        public CsvStreamWriter(Stream stream, Encoding encoding, int bufferSize) : base(stream,  encoding,  bufferSize) { }

        public CsvStreamWriter(string path, bool append, Encoding encoding) : base(path, append, encoding) { }

        public CsvStreamWriter(Stream stream, Encoding? encoding = null, int bufferSize = -1, bool leaveOpen = false) : base(stream, encoding = null, bufferSize = -1, leaveOpen = false) { }

        public CsvStreamWriter(string path, bool append, Encoding encoding, int bufferSize) : base(path, append, encoding, bufferSize) { }

        public char Separator { get; set; } = ',';

        public void WriteCsvLine(params string[] values)
        {
            var _sb = new StringBuilder().Append((BaseStream.Position > 0) ? "\r\n" : "");
            for (int ii = 0; ii < values.Length; ii++)
                _sb.Append(Esc(values[ii] ?? "")).Append(Separator);
            _sb.Length--;
            BaseStream.Write(Encoding.Default.GetBytes(_sb.ToString()), 0, _sb.Length);
        }

        private string Esc(string s) => (s.IndexOfAny(new char[] { '\r', '\n', '"', Separator }) == -1) ? s : $"\"{s.Replace("\"", "\"\"")}\"";
    }
}
