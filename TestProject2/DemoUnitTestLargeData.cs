using DevToys;
using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using TestProject2.Models;

namespace TestProject2
{
    [TestClass]
    public class DemoUnitTestLargeData
    {
        [TestMethod]
        public void TestLargeData()
        {
            string _file = System.IO.Path.GetTempFileName();

            using (CsvWriter<CsvSimple> _writer = new(_file) { Separator = ',' })
            {
                _writer.Open();
                _writer.WriteHeader();
                _writer.Write(LargeData());
            }

            var _w = new StopWatch();

            _w.Start();

            using (var _reader = new CsvReader<CsvSimple>(_file))
            {
                _reader.Open();

                var _rows = _reader.ReadAsEnumerable().ToList(); // Materialize.
            }
            // 00:00:22.3810056
            // 00:00:23.2109005
            // 00:00:21.2599918
            // 00:00:20.9277719
            // 00:00:20.4437562
            // 00:00:20.4812967
            // {00:00:21.3921716}
            // {00:00:20.5838151}


            _w.Stop();
            var _Duration1 = _w.Duration;
            Console.WriteLine(_w.Duration);


            _w.Start();

            using (var _reader = new CsvReader<CsvSimpleSmall>(_file))
            {
                _reader.Open();

                var _rows = _reader.ReadAsEnumerable().ToList(); // Materialize.
            }

            _w.Stop();
            var _Duration2 = _w.Duration;
            Console.WriteLine(_w.Duration);

        }

        private IEnumerable<CsvSimple> LargeData()
        {
            for (int ii = 0; ii < 10000000; ii++)
            {
                CsvSimple _line = new()
                {
                    AfBij = "bij",
                    Bedrag = "100",
                    Code = "test",
                    Datum = "20200203",
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