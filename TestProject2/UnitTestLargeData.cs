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

            using (CsvWriter<CsvSimple> _writer = new(file) { Separator = ',', Append = true })
            {
                _writer.Open();
                _writer.Write(LargeData());
            }
        }



        [TestMethod]
        public void TestReaderXL()
        {
            string file = @"D:\largedata.csv";

            using (CsvReader<CsvSimple> _reader = new CsvReader<CsvSimple>(file))
            {
                _reader.Open();

                var x = _reader.Rows().ToList();


                var _af = _reader.Rows().Where(p => p.AfBij == "Af").ToList();

                Console.WriteLine('X');
            }
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
                    Tegenrekening = "3423424"
                };
                yield return _line;
            }
        }
    }
}