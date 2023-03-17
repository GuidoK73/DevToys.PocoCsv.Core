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
        protected StreamReader _StreamReader;

        protected CsvStreamer _Streamer = new CsvStreamer();

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
        public BaseCsvReader(Stream stream, Encoding encoding, char separator = ',', bool detectEncodingFromByteOrderMarks = true, int bufferSize = 1024) : this(stream)
        {
            Encoding = encoding;
            _Streamer.Separator = separator;
            DetectEncodingFromByteOrderMarks = detectEncodingFromByteOrderMarks;
            _BufferSize = bufferSize;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseCsvReader(string file, Encoding encoding, char separator = ',', bool detectEncodingFromByteOrderMarks = true, int bufferSize = 1024) : this(file)
        {
            Encoding = encoding;
            _Streamer.Separator = separator;
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
                return _Streamer.Separator;
            }
            set
            {
                _Streamer.Separator = value;
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
        /// 
        /// </summary>
        protected abstract void Init();
    }
}
