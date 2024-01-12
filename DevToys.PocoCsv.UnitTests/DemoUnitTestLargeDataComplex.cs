using DevToys;
using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using DevToys.PocoCsv.UnitTests.Models;

namespace DevToys.PocoCsv.UnitTests
{
    [TestClass]
    public class DemoUnitTestLargeDataComplex
    {
        [TestMethod]
        public void TestLargeData()
        {
            string _file = System.IO.Path.GetTempFileName();



            using (CsvWriter<CsvComplex> _writer = new(_file) { Separator = ',' })
            {
                _writer.Open();
                _writer.WriteHeader();
                _writer.Write(LargeData());
            }

            var _w = new StopWatch();

            _w.Start();

            using (var _reader = new CsvReader<CsvComplex>(_file))
            {
                _reader.Open();


                while (!_reader.EndOfStream)
                {
                    CsvComplex _data = _reader.Read();
                    Console.WriteLine("X");
                }


                //var _rows = _reader.ReadAsEnumerable().ToList(); // Materialize.
            }

            _w.Stop();
            Console.WriteLine(_w.Duration);
        }

        private IEnumerable<CsvComplex> LargeData()
        {
            for (int ii = 0; ii < 100000; ii++)
            {
                CsvComplex _line = new()
                {
                    AfBij = "bij",
                    Bedrag = 100.59M,
                    Code = "test",
                    Datum = new DateTime(2023,7,14),
                    Mededelingen = $"test {ii}",
                    Rekening = "3434",
                    Tegenrekening = "3423424",
                    NaamOmschrijving = $"bla,bla {ii}",
                    MutatieSoort = "Bij"
                };
                yield return _line;
            }
        }
    }
}