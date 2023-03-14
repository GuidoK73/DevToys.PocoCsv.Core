using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    public abstract class BaseCsvReader : IDisposable 
    {
        protected readonly string _File = null;
        protected Stream _Stream = null;
        protected CsvStreamReader _Reader;
        protected char _Separator = ',';

        public BaseCsvReader(string file)
        {
            _File = file;
        }

        public BaseCsvReader(Stream stream)
        {
            _Stream = stream;
        }

        public BaseCsvReader(Stream stream, Encoding encoding, char separator = ',', bool detectEncodingFromByteOrderMarks = true) : this(stream)
        {
            Encoding = encoding;
            _Separator = separator;
            DetectEncodingFromByteOrderMarks = detectEncodingFromByteOrderMarks;
        }

        public BaseCsvReader(string file, Encoding encoding, char separator = ',', bool detectEncodingFromByteOrderMarks = true) : this(file)
        {
            Encoding = encoding;
            _Separator = separator;
            DetectEncodingFromByteOrderMarks = detectEncodingFromByteOrderMarks;
        }

        /// <summary>
        /// Culture info to use for serialization.
        /// </summary>
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// Csv Seperator to use default ','
        /// </summary>
        public char Separator
        {
            get
            {
                if (_Reader != null)
                {
                    _Separator = _Reader.Separator;
                }
                return _Separator;
            }
            set
            {
                _Separator = value;
                if (_Reader != null)
                {
                    _Reader.Separator = _Separator;
                }
            }
        }

        /// <summary>
        /// Indicates whether to look for byte order marks at the beginning of the file.
        /// </summary>
        public bool DetectEncodingFromByteOrderMarks { get; set; } = true;

        /// <summary>
        /// The character encoding to use.
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.Default;

        /// <summary>
        /// Releases all resources used by the System.IO.TextReader object.
        /// </summary>
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
            Close();
        }

        /// <summary>
        /// Initialize and open the CSV Stream Reader.
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// Close the CSV stream reader
        /// </summary>
        public virtual void Close()
        {
            _Reader.Close();
        }

        /// <summary>
        /// Indicates End of stream, use with Read funcion.
        /// </summary>
        public bool EndOfStream => _Reader.EndOfCsvStream;

        /// <summary>
        /// Do a 10 line sample to detect and set separator.
        /// </summary>
        public virtual void DetectSeparator()
        {

            bool _succes = CsvUtils.GetCsvSeparator(_Reader, out _Separator, 10);
            if (_succes)
            {
                Separator = _Separator;
            }
            _Reader.BaseStream.Position = 0;
        }

        protected abstract void Init();
    }
}
