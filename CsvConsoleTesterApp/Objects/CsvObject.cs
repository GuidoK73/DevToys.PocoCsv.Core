using DevToys.PocoCsv.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Objects
{
    public sealed class CsvObject
    {
        [Column(Index = 1, Header = "Datum")]
        public string Datum { get; set; }

        [Column(Index = 1, Header = "NaamOmschrijving")]
        public string NaamOmschrijving { get; set; }

        [Column(Index = 2, Header = "Rekening")]
        public string Rekening { get; set; }

        [Column(Index = 3, Header = "Tegenrekening")]
        public string Tegenrekening { get; set; }
       
        [Column(Index = 4, Header = "Code")]
        public string Code { get; set; }
        [Column(Index = 5, Header = "AfBij")]
        public string AfBij { get; set; }

        [Column(Index = 6, Header = "Bedrag")]
        public string Bedrag { get; set; }

        [Column(Index = 7, Header = "MutatieSoort")]
        public string MutatieSoort { get; set; }

        [Column(Index = 8, Header = "Mededelingen")]
        public string Mededelingen { get; set; }

    }


    public sealed class CsvObjectSmall
    {
        [Column(Index = 5)]
        public string AfBij { get; set; }


        [Column(Index = 2)]
        public string Rekening { get; set; }

        [Column(Index = 3)]
        public string Tegenrekening { get; set; }
    }
}
