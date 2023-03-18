using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// base class for CsvWriter
    /// </summary>
    public abstract class BaseCsvWriter : BaseCsvReaderWriter, IDisposable
    {



        /// <summary>
        /// 
        /// </summary>
        protected StreamWriter _StreamWriter;


        /// <summary>
        /// Constructor
        /// </summary>
        public BaseCsvWriter(string file)
        {
            _File = file;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseCsvWriter(Stream stream)
        {
            _Stream = stream;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseCsvWriter(string file, Encoding encoding, CultureInfo culture, char separator = ',', int bufferSize = 1024) : this(file)
        {
            Culture = culture;
            Separator = separator;
            Encoding = encoding;
            _BufferSize = bufferSize;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseCsvWriter(Stream stream, Encoding encoding, CultureInfo culture, char separator = ',', int bufferSize = 1024) : this(stream)
        {
            Culture = culture;
            Separator = separator;
            Encoding = encoding;
            _BufferSize = bufferSize;
        }

        /// <summary>
        /// Culture info to use for serialization.
        /// </summary>
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// Csv Seperator to use default ','
        /// </summary>
        public char Separator { get; set; } = ',';


        /// <summary>
        /// Releases all resources used by the System.IO.TextReader object.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Close();
        }

        /// <summary>
        /// Close the CSV stream reader
        /// </summary>
        public virtual void Close()
        {
            _StreamWriter.Close();
        }

        /// <summary>
        /// Initialize and open the CSV Stream Writer.
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// 
        /// </summary>
        protected abstract void Init();



    }
}
