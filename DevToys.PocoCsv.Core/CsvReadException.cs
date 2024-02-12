using System;

namespace DevToys.PocoCsv.Core
{

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class CsvReadException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public CsvReadException() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="values"></param>
        public CsvReadException(string message, params object[] values) : base(string.Format(message, values)) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public CsvReadException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class CsvException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public CsvException() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="values"></param>
        public CsvException(string message, params object[] values) : base(string.Format(message, values)) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public CsvException(string message, Exception innerException) : base(message, innerException) { }
    }
}
