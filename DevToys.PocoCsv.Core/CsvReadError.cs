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
        ///
        /// </summary>
        public override string ToString()
        {
            return $"Line: {LineNumber}, Column: {ColumnIndex}, Property: {PropertyName}, Value: {Value}";
        }
    }
}