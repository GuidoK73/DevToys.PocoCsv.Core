using System;
using System.Collections.Generic;
using System.Text;

namespace DevToys.PocoCsv.Core.CsvDataTypeObject
{
    public sealed class CsvDataTypeObject5
    {
        [Column(Index = 0)]
        public string Field1 { get; set; }
        [Column(Index = 1)]
        public string Field2 { get; set; }
        [Column(Index = 2)]
        public string Field3 { get; set; }
        [Column(Index = 3)]
        public string Field4 { get; set; }
        [Column(Index = 4)]
        public string Field5 { get; set; }
    }
}
