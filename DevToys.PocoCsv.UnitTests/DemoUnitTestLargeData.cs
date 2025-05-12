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
    public class DemoUnitTestLargeData
    {
        [TestMethod]
        public void TestLargeData()
        {
            string _file = System.IO.Path.GetTempFileName();

            var _w = new StopWatch();

            _w.Start();

            using (CsvWriter<CsvSimple> _writer = new(_file) { Separator = ',' })
            {
                _writer.WriteHeader();
                _writer.Write(LargeData());
            }

            _w.Stop();
            var _DurationA = _w.Duration;
            Console.WriteLine(_w.Duration);





            _w.Start();

            using (var _reader = new CsvReader<CsvSimple>(_file, ','))
            {
                var _rows = _reader.ReadAsEnumerable().ToList(); // Materialize.
            }

            _w.Stop();
            var _Duration1 = _w.Duration;
            Console.WriteLine(_w.Duration);




            _w.Start();

            string _text = System.IO.File.ReadAllText(_file);

            CsvSerializer _se = new CsvSerializer();

            var _rows2 = _se.DeserializeObject<CsvSimple>(_text).ToList();


            _w.Stop();
            var _Duration2 = _w.Duration;
            Console.WriteLine(_w.Duration);




            _w.Start();

            using (var _reader = new CsvReader<CsvSimpleSmall>(_file))
            {
                var _rows = _reader.ReadAsEnumerable().ToList(); // Materialize.
            }

            _w.Stop();
            var _Duration3 = _w.Duration;
            Console.WriteLine(_w.Duration);
        }



        [TestMethod]
        public void TestLargeDataCsvStreamWriter()
        {
            string _file = System.IO.Path.GetTempFileName();

            var _w = new StopWatch();

            _w.Start();

            using (CsvStreamWriter _writer = new(_file) { Separator = ',' })
            {
                foreach (CsvSimple item in LargeData())
                {
                    _writer.WriteCsvLine(item.AfBij, item.Bedrag, item.Code, item.Datum, item.Mededelingen, item.Rekening, item.Tegenrekening, item.NaamOmschrijving, item.MutatieSoort);
                }
            }

            _w.Stop();
            var _DurationA = _w.Duration;
            Console.WriteLine(_w.Duration);

            string _text = System.IO.File.ReadAllText(_file);

            // 00:00:07.5195285
            // 00:00:06.8875798

        }



        private IEnumerable<CsvSimple> LargeData()
        {
            // 
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