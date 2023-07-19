using DevToys;
using DevToys.PocoCsv.Core;
using DevToys.PocoCsv.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TestProject2.Models;

namespace TestProject2
{
    [TestClass]
    public class DemoUnitTestDataTable
    {
        [TestMethod]
        public void TestDataTable()
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

            DataTable _table = new DataTable();
            _table.ImportCsv(_file, ',', true);


            _w.Stop();
            Console.WriteLine(_w.Duration);

            _file = System.IO.Path.GetTempFileName();
            _table.ExportCsv(_file, ',');

        }

        private IEnumerable<CsvSimple> LargeData()
        {
            for (int ii = 0; ii < 1000000; ii++)
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