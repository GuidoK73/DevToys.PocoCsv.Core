using System;

namespace DevToys.PocoCsv.Core
{
    public class RowData
    {
        public string[] Row { get; internal set; }

        /// <summary>
        /// If true, the row will not be serialized, in this case a null element will occur in the enumerator.
        /// </summary>
        public bool Skip { get; set; } = false;

        public int RowNumber { get; internal set; } = 0;
    }
}