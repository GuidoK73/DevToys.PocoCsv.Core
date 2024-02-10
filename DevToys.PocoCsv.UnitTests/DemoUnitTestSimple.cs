using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevToys.PocoCsv.UnitTests.Models;

namespace DevToys.PocoCsv.UnitTests
{

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
               // _reader.Skip(20);

                var _af = _reader.ReadAsEnumerable().Skip(20).ToList();
            }
        }

        [TestMethod]
        public void TestQuotes()
        {
            string _file = System.IO.Path.GetTempFileName();

            using (CsvWriter<CsvSimple> _writer = new(_file) { Separator = ',' })
            {
                _writer.Open();
                //_writer.WriteHeader();
                _writer.Write(Data());
            }

            string _text = File.ReadAllText(_file);

            using (CsvReader<CsvSimple> _reader = new(_file))
            {
                _reader.Open();
                var _data = _reader.ReadAsEnumerable().ToList();
            }
        }

        private IEnumerable<CsvSimple> Data()
        {
            // _writer.WriteCsvLine("Row 7", "Row F\",\"F\r\nF,F\"", "\"", "F6\" ");
            for (int ii = 0; ii < 100; ii++)
            {
                CsvSimple _line = new()
                {
                    AfBij = "bij,af",
                    Bedrag = "AAAA\"\"",
                    Code = "TEST\" ",
                    Datum = "\"",
                    Mededelingen = $"test {ii}",
                    Rekening = "\"\"",
                    Tegenrekening = "3423424"
                };
                yield return _line;
            }
        }
    }
}