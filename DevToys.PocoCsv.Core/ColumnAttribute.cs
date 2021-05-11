using System;

namespace DevToys.PocoCsv.Core
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {
        public int Index { get; set; }
    }
}