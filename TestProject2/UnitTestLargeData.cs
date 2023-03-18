using DevToys;
using DevToys.PocoCsv.Core;
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
            StopWatch _w = new StopWatch();

            string file = @"D:\largedata.csv";
            _w.Start();

            using (CsvReader<CsvSimple> _reader = new CsvReader<CsvSimple>(file))
            {
                _reader.Open();

                var x = _reader.ReadAsEnumerable().ToList();
                // var _af = _reader.Rows().Where(p => p.AfBij == "Af").ToList();
            }

            _w.Stop();
            Console.WriteLine(_w.Duration);

            // 00:00:21.9864542 No Trimming

            // 00:00:31.2696500
            // 00:00:30.6005957

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