using System.Globalization;
using System.IO;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Base class for BaseCsvReader and BaseCsvWriter
    /// </summary>
    public abstract class BaseCsv
    {
        /// <summary>
        /// Property Set by contructor, either File or Stream is used.
        /// </summary>
        protected string _File = null;

        /// <summary>
        /// Property Set by contructor, either File or Stream is used.
        /// </summary>
        protected Stream _Stream = null;

        /// <summary>
        /// Separator to use. Default: ','
        /// </summary>
        protected char _Separator = ',';

        /// <summary>
        /// Stream buffer size, Default: 1024
        /// </summary>
        protected int _BufferSize = 1024;

        /// <summary>
        /// Culture info to use for serialization.
        /// </summary>
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// The character encoding to use.
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.Default;
    }
}