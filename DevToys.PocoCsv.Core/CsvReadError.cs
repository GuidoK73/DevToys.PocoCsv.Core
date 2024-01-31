using System;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Read error info, only when type conversions take place.
    /// </summary>
    public sealed class CsvReadError
    {
        /// <summary>
        ///
        /// </summary>
        public int ColumnIndex { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public Type PropertyType { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Exception Message
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        ///
        /// </summary>
        public override string ToString()
        {
            return $"Line: {LineNumber}, Column: {ColumnIndex}, Property: {PropertyName}, Value: {Value}";
        }

        /// <summary>
        /// Deconstructor
        /// </summary>
        public void Decontruct(out int columnIndex, out string propertyName, out Type propertyType, out string value, out int lineNumber, out Exception exception)
        {
            columnIndex = ColumnIndex;
            propertyName = PropertyName;
            propertyType = PropertyType;
            value = Value;
            lineNumber = LineNumber;
            exception = Exception;
        }
    }
}