using DevToys;
using DevToys.PocoCsv.Core;
using DevToys.PocoCsv.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using DevToys.PocoCsv.UnitTests.Models;

namespace DevToys.PocoCsv.UnitTests
{
    [TestClass]
    public class DemoUnitTestIgnoreColumnAttributes
    {
        private IEnumerable<CsvSimpleNoAttribute> LargeData()
        {
            for (int ii = 0; ii < 10; ii++)
            {
                CsvSimpleNoAttribute _line = new()
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


        [TestMethod]
        public void TestTestIgnoreColumnAttribute()
        {
            string _file = System.IO.Path.GetTempFileName();

            using (CsvWriter<CsvSimpleNoAttribute> _writer = new(_file) { Separator = ',' })
            {
                _writer.IgnoreColumnAttributes = true;
                _writer.WriteHeader();
                _writer.Write(LargeData());
            }

            string _data = File.ReadAllText(_file);

            using (CsvReader<CsvSimpleNoAttribute> _reader = new(_file))
            {
                _reader.IgnoreColumnAttributes = true;
                var _result = _reader.ReadAsEnumerable().ToList();
            }
        }


    }
}