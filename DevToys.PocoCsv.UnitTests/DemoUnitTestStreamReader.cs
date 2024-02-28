using DevToys;
using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevToys.PocoCsv.UnitTests.Models;
using System.Reflection.PortableExecutable;

namespace DevToys.PocoCsv.UnitTests
{
    [TestClass]
    public class DemoUnitTestStreamReader
    {
        [TestMethod]
        public void TestStreamReader()
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

            List<string[]> _rows = new List<string[]>();

            using (CsvStreamReader _reader = new CsvStreamReader(_file))
            {
                _reader.Separator = ',';

                _reader.Skip();

                while (!_reader.EndOfStream)
                {
                    string[] _values = _reader.ReadCsvLine();
                    _rows.Add(_values);
                }
            }
            _w.Stop();
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