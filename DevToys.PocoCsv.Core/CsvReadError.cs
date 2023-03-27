using System;

namespace DevToys.PocoCsv.Core
{
    public class CsvReadError
    {
        public int RowNumber { get; set; }

        public int ColumnIndex { get; set; }

        public string PropertyName { get; set; }

        public Type PropertyType { get; set; }

        public string Value { get; set; }
    }
}