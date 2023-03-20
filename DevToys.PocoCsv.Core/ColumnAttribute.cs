using System;

namespace DevToys.Poco.Csv.Core
{
    /// <summary>
    /// Csv Column Attribute to assign to POCO Properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ColumnAttribute : Attribute
    {
        /// <summary>
        /// Defines the index position within the CSV document. Numbers can be skipped for the reader to ignore certain columns, for the writer numbers can also be skipped which leads to empty columns.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Defines the header text, this property only applies to the CsvWriter, if not specified, the property name is used.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Apply a string format, depending on the Property type. This property is for CsvWriter only.
        /// </summary>
        public string OutputFormat { get; set; }

        /// <summary>
        /// Defines the value to write as a default for null, This property is for CsvWriter only.
        /// </summary>
        public string OutputNullValue { get; set; } = string.Empty;
    }
}