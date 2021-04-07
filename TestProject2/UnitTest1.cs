using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace TestProject2
{
    public class Csv
    {
        [Column(Index = 0)]
        public DateTime Datum { get; set; }

        [Column(Index = 1)]
        public string NaamOmschrijving { get; set; }

        [Column(Index = 2)]
        public string Rekening { get; set; }

        [Column(Index = 3)]
        public string Tegenrekening { get; set; }

        [Column(Index = 4)]
        public string Code { get; set; }

        [Column(Index = 5)]
        public string AfBij { get; set; }

        [Column(Index = 6)]
        public decimal Bedrag { get; set; }

        [Column(Index = 7)]
        public string MutatieSoort { get; set; }

        [Column(Index = 8)]
        public string Mededelingen { get; set; }
    }


    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string file = Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "data.csv");

            using (CsvReader<Csv> _reader = new CsvReader<Csv>(file))
            {
                _reader.Open();
                _reader.BeforeSerialize += OnBeforeSerialize;

                _reader.Culture = CultureInfo.GetCultureInfo("nl-NL");

                foreach (Csv csv in _reader.Rows())
                {
                }

                var _bij = _reader.Rows().Where(p => p.AfBij == "Bij").ToList();
            }
        }

        // use this to prepare row values which can not be serialized directly.
        private void OnBeforeSerialize(RowData r)
        {
            if (r.Row.Length != 9)
            {
                r.Skip = true; // Skip invalid lines.
                return;
            }
            
            if (r.RowNumber == 0)
            {
                r.Skip = true; // Skip the header.
                return;
            }
            
            r.Row[0] = $"{r.Row[0].Substring(0, 4)}-{r.Row[0].Substring(4, 2)}-{r.Row[0].Substring(6, 2)}";  // Make the date castable.
        }
    }
}
