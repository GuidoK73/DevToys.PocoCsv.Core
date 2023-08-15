using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DevToys.PocoCsv.Core
{

    [Serializable]
    public sealed class CsvReadException : Exception
    {
        public CsvReadException() { }

        public CsvReadException(string message, params object[] values) : base(string.Format(message, values)) { }

        public CsvReadException(string message, Exception innerException) : base(message, innerException) { }

        protected CsvReadException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) => base.GetObjectData(info, context);
    }


    [Serializable]
    public sealed class CsvException : Exception
    {
        public CsvException() { }

        public CsvException(string message, params object[] values) : base(string.Format(message, values)) { }

        public CsvException(string message, Exception innerException) : base(message, innerException) { }

        protected CsvException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) => base.GetObjectData(info, context);
    }
}
