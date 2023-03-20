using System.Globalization;
using System.IO;
using System.Text;

namespace DevToys.Poco.Csv.Core
{
    /// <summary>
    /// Base class for BaseCsvReader and BaseCsvWriter
    /// </summary>
    public abstract class BaseCsv
    {
        /// <summary>
        ///
        /// </summary>
        protected string _File = null;

        /// <summary>
        ///
        /// </summary>
        protected Stream _Stream = null;

        /// <summary>
        ///
        /// </summary>
        protected char _Separator = ',';

        /// <summary>
        ///
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