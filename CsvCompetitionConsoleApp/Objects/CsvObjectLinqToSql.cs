using LINQtoCSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Objects
{
    public sealed class CsvObjectLinqToSql
    {
        [CsvColumn(FieldIndex = 5, Name = "AfBij")]
        public string AfBij { get; set; }

        [CsvColumn(FieldIndex = 6, Name = "Bedrag")]
        public string Bedrag { get; set; }

        [CsvColumn(FieldIndex = 4, Name = "Code")]
        public string Code { get; set; }

        [CsvColumn(FieldIndex = 0, Name = "Datum")]
        public string Datum { get; set; }

        [CsvColumn(FieldIndex = 8, Name = "Mededelingen")]
        public string Mededelingen { get; set; }

        [CsvColumn(FieldIndex = 7, Name = "MutatieSoort")]
        public string MutatieSoort { get; set; }

        [CsvColumn(FieldIndex = 1, Name = "NaamOmschrijving")]
        public string NaamOmschrijving { get; set; }

        [CsvColumn(FieldIndex = 2, Name = "Rekening")]
        public string Rekening { get; set; }

        [CsvColumn(FieldIndex = 3, Name = "Tegenrekening")]
        public string Tegenrekening { get; set; }
    }
}
