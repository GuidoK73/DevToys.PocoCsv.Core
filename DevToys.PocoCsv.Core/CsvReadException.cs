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
        public CsvReadException(string message) : base(message) { }
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
        public CsvException(string message) : base(message) { }

    }
}
