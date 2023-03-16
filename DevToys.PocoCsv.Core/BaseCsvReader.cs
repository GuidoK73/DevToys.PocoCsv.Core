using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// base class for CsvReader
    /// </summary>
    public abstract class BaseCsvReader : BaseCsvReaderWriter,  IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        protected CsvStreamReader _StreamReader;

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseCsvReader(string file)
        {
            _File = file;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseCsvReader(Stream stream)
        {
            _Stream = stream;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseCsvReader(Stream stream, Encoding encoding, char separator = ',', bool detectEncodingFromByteOrderMarks = true, int bufferSize = -1) : this(stream)
        {
            Encoding = encoding;
            _Separator = separator;
            DetectEncodingFromByteOrderMarks = detectEncodingFromByteOrderMarks;
            _BufferSize = bufferSize;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseCsvReader(string file, Encoding encoding, char separator = ',', bool detectEncodingFromByteOrderMarks = true, int bufferSize = -1) : this(file)
        {
            Encoding = encoding;
            _Separator = separator;
            DetectEncodingFromByteOrderMarks = detectEncodingFromByteOrderMarks;
            _BufferSize = bufferSize;
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
                if (_StreamReader != null)
                {
                    _Separator = _StreamReader.Separator;
                }
                return _Separator;
            }
            set
            {
                _Separator = value;
                if (_StreamReader != null)
                {
                    _StreamReader.Separator = _Separator;
                }
            }
        }

        /// <summary>
        /// Indicates whether to look for byte order marks at the beginning of the file.
        /// </summary>
        public bool DetectEncodingFromByteOrderMarks { get; set; } = true;

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
            _StreamReader.Close();
        }

        /// <summary>
        /// Indicates End of stream, use with Read funcion.
        /// </summary>
        public bool EndOfStream => _StreamReader.EndOfCsvStream;

        /// <summary>
        /// Do a 10 line sample to detect and set separator, it will try ',', ';', '|', '\t', ':'
        /// </summary>
        public virtual void DetectSeparator()
        {

            bool _succes = CsvUtils.GetCsvSeparator(_StreamReader, out _Separator, 10);
            if (_succes)
            {
                Separator = _Separator;
            }
            _StreamReader.BaseStream.Position = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        protected abstract void Init();
    }
}
