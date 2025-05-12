using System.Globalization;

namespace DevToys.PocoCsv.Core
{ 
    /// <summary>
    /// 
    /// </summary>
    public sealed class CsvSerializerSettings
    {
        /// <summary>
        /// Write header or skip header
        /// </summary>
        public bool Header { get; set; } = false;

        /// <summary>
        /// Separator to use.
        /// </summary>
        public char Separator { get; set; } = ',';

        /// <summary>
        /// Automatic separator detection.
        /// </summary>
        public bool DetectSeparator { get; set; } = false;

        /// <summary>
        /// Culture to use.
        /// </summary>
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// CRLF mode to use for the Serializer.
        /// </summary>
        public CRLFMode CRLFMode { get; set; } = CRLFMode.CRLF;


        /// <summary>
        /// Size of StringBuilder buffer.
        /// </summary>
        public int BufferSize { get; set; } = 1024;
    }
}
