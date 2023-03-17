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
        protected Dictionary<int, PropertyInfo> _Properties = new();

        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<int, Func<object, object>> _PropertyGetters = new();

        /// <summary>
        /// 
        /// </summary>
        protected CsvStreamWriter _StreamWrtier;


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
        public BaseCsvWriter(string file, Encoding encoding, CultureInfo culture, char separator = ',', bool append = true, int bufferSize = 1024) : this(file)
        {
            Culture = culture;
            Separator = separator;
            Encoding = encoding;
            Append = append;
            _BufferSize = bufferSize;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseCsvWriter(Stream stream, Encoding encoding, CultureInfo culture, char separator = ',', bool append = true, int bufferSize = 1024) : this(stream)
        {
            Culture = culture;
            Separator = separator;
            Encoding = encoding;
            Append = append;
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
                if (_StreamWrtier != null)
                {
                    _Separator = _StreamWrtier.Separator;
                }
                return _Separator;
            }
            set
            {
                _Separator = value;
                if (_StreamWrtier != null)
                {
                    _StreamWrtier.Separator = _Separator;
                }
            }
        }

        /// <summary>
        /// Write command can be used to append multiple collections to the open Csv Stream.
        /// </summary>
        public bool Append { get; set; } = true;

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
            _StreamWrtier.Close();
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
