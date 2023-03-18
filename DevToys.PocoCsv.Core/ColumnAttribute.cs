using System;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Csv Column Attribute to assign to POCO Properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ColumnAttribute : Attribute
    {
        /// <summary>
        /// Property corresponds to Csv Column Index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Only used by CsvWriter
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Only used by CsvWriter
        /// </summary>
        public string OutputFormat { get; set; }
    }
}