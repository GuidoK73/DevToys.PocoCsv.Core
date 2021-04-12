using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestProject2
{
    public class CsvSimple
    {
        [Column(Index = 5)]
        public string AfBij { get; set; }

        [Column(Index = 6)]
        public string Bedrag { get; set; }

        [Column(Index = 4)]
        public string Code { get; set; }

        [Column(Index = 0)]
        public string Datum { get; set; }

        [Column(Index = 8)]
        public string Mededelingen { get; set; }

        [Column(Index = 7)]
        public string MutatieSoort { get; set; }

        [Column(Index = 1)]
        public string NaamOmschrijving { get; set; }

        [Column(Index = 2)]
        public string Rekening { get; set; }

        [Column(Index = 3)]
        public string Tegenrekening { get; set; }
    }

    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod2()
        {
            string file = Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "data.csv");

            using (CsvReader<CsvSimple> _reader = new CsvReader<CsvSimple>(file))
            {
                _reader.Open();

                var _af = _reader.Rows().Where(p => p.AfBij == "Af").ToList();
            }
        }

        [TestMethod]
        public void TestWriter()
        {
            string file = @"D:\data2.csv";

            using (CsvWriter<CsvSimple> _writer = new CsvWriter<CsvSimple>(file) { Separator = ',', Append = true })
            {
                _writer.Open();
                _writer.Write(Data());
            }
        }

        private IEnumerable<CsvSimple> Data()
        {
            for (int ii = 0; ii < 1000; ii++)
            {
                CsvSimple _line = new CsvSimple()
                {
                    AfBij = "bij",
                    Bedrag = "100",
                    Code = "test",
                    Datum = "20200203",
                    Mededelingen = $"test {ii}",
                    Rekening = "3434",
                    Tegenrekening = "3423424"
                };
                yield return _line;
            }
        }
    }
}