using System;

namespace DevToys.PocoCsv.Core
{
    public class ColumnAttribute : Attribute
    {
        public int Index { get; set; }
    }
}