using DevToys;
using DevToys.Poco.Csv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestProject2
{
    [TestClass]
    public class UnitTestLargeData
    {
        [TestMethod]
        public void TestWriterXL()
        {
            string file = @"D:\largedata.csv";

            if (File.Exists(file))
            {
                File.Delete(file);
            }

            using (CsvWriter<CsvSimple> _writer = new(file) { Separator = ','})
            {
                _writer.Open();
                _writer.WriteHeader();
                _writer.Write(LargeData());
            }
        }

        [TestMethod]
        public void TestReaderXL()
        {
            var _w = new StopWatch();

            string file = @"D:\largedata.csv";
            _w.Start();

            using (var _reader = new CsvReader<CsvSimple>(file))
            {
                _reader.Open();

                var x = _reader.ReadAsEnumerable().ToList();
                // var _af = _reader.Rows().Where(p => p.AfBij == "Af").ToList();
            }

            _w.Stop();
            Console.WriteLine(_w.Duration);

        }



        [TestMethod]
        public void TestReaderXLSkipAndTake()
        {
            var _w = new StopWatch();

            List<CsvSimple> _result1;
            List<CsvSimple> _result2;

            string file = @"D:\largedata.csv";
            _w.Start();

            using (var _reader = new CsvReader<CsvSimple>(file))
            {
                _reader.Open();

                _reader.Skip(); // skip the Header row.
                _reader.Skip(10); // skip another 10 rows, this skip does not materialize. skip on Enumerable requires T to be materialized.
                _result1 = _reader.ReadAsEnumerable().Skip(10).Take(10).ToList(); // Materializes 20 records but returns 10.
                _result1 = _reader.ReadAsEnumerable().Take(10).ToList(); // Only Read 10 sample rows without materializing the entire document.
                _result2 = _reader.ReadAsEnumerable().Take(10).ToList(); // Read the next 10 sample rows without materializing the entire document.
            }

            _w.Stop();
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