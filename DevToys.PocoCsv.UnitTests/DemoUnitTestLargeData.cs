using DevToys;
using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DevToys.PocoCsv.UnitTests.Models;
using static System.Net.WebRequestMethods;

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
            //{{00:00:15.7702391}}
            //00:00:08.6757924


            _w.Start();

            using (var _reader = new CsvReader<CsvSimple>(_file, ',') { BufferSize = 2048 })
            {
                var _rows = _reader.ReadAsEnumerable().ToList(); // Materialize.
            }
            //00:00:22.3810056
            // 00:00:23.2109005
            // 00:00:21.2599918
            // 00:00:20.9277719
            // 00:00:20.4437562
            // 00:00:20.4812967
            // { 00:00:21.3921716}
            //{ 00:00:20.5838151}
            //{ 00:00:16.5241491}
            //{ 00:00:16.7875149}
            //{ 00:00:17.0972567}

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