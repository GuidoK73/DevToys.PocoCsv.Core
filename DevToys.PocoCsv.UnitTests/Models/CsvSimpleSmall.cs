using DevToys.PocoCsv.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevToys.PocoCsv.UnitTests.Models
{
    public sealed class CsvSimpleSmall
    {
        [Column(Index = 5)]
        public string AfBij { get; set; }


        [Column(Index = 2)]
        public string Rekening { get; set; }

        [Column(Index = 3)]
        public string Tegenrekening { get; set; }
    }

}
