using System;
using System.Runtime.Serialization;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected CsvReadException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context) => base.GetObjectData(info, context);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected CsvException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context) => base.GetObjectData(info, context);
    }
}
