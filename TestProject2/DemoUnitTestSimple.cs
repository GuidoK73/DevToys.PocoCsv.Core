using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace TestProject2
{
    public sealed class CsvSimple
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
    public class DemoUnitTestSimple
    {
        [TestMethod]
        public void TestReaderSimple()
        {
            string _file = System.IO.Path.GetTempFileName();

            using (CsvWriter<CsvSimple> _writer = new(_file) { Separator = ',' })
            {
                _writer.Open();
                _writer.WriteHeader();
                _writer.Write(Data());
            }

            using (CsvReader<CsvSimple> _reader = new(_file))
            {
                _reader.Open();
                _reader.Skip(20);

                var _af = _reader.ReadAsEnumerable().ToList();
            }
        }

        [TestMethod]
        public void TestReaderSimpleLast()
        {
            string _file = System.IO.Path.GetTempFileName();

            using (CsvWriter<CsvSimple> _writer = new(_file) { Separator = ',' })
            {
                _writer.Open();
                _writer.WriteHeader();
                _writer.Write(Data());
            }

            using (CsvReader<CsvSimple> _reader = new(_file))
            {
                _reader.Open();
                var _last10 = _reader.Last(10).ToList();
            }
        }

        private IEnumerable<CsvSimple> Data()
        {
            for (int ii = 0; ii < 1000; ii++)
            {
                CsvSimple _line = new()
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