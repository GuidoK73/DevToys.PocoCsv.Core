using System;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Csv Column Attribute to assign to POCO Properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// Property corresponds to Csv Column Index.
        /// </summary>
        public int Index { get; set; }
    }
}