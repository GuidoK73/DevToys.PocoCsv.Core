using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    public abstract class BaseCsvWriter : IDisposable
    {
        protected readonly string _File = null;
        protected Dictionary<int, PropertyInfo> _Properties = new();
        protected Dictionary<int, Func<object, object>> _PropertyGetters = new();
        protected CsvStreamWriter _Writer;
        protected Stream _Stream = null;

        public BaseCsvWriter(string file)
        {
            _File = file;
        }

        public BaseCsvWriter(Stream stream)
        {
            _Stream = stream;
        }

        public BaseCsvWriter(string file, Encoding encoding, CultureInfo culture, char separator = ',', bool append = true) : this(file)
        {
            Culture = culture;
            Separator = separator;
            Encoding = encoding;
            Append = append;
        }

        public BaseCsvWriter(Stream stream, Encoding encoding, CultureInfo culture, char separator = ',', bool append = true) : this(stream)
        {
            Culture = culture;
            Separator = separator;
            Encoding = encoding;
            Append = append;
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
        /// Write command can be used to append multiple collections to the open Csv Stream.
        /// </summary>
        public bool Append { get; set; } = true;

        /// <summary>
        /// The character encoding to use.
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.Default;

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
            _Writer.Close();
        }

        /// <summary>
        /// Initialize and open the CSV Stream Writer.
        /// </summary>
        public abstract void Open();


        protected abstract void Init();



    }
}
