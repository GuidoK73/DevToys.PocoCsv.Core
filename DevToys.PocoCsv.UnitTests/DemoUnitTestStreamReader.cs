﻿using DevToys;
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
                _writer.WriteHeader();
                _writer.Write(LargeData());
            }

            var _w = new StopWatch();

            _w.Start();

            List<string[]> _rows = new List<string[]>();

            using (CsvStreamReader _reader = new CsvStreamReader(_file))
            {
                _reader.Separator = ',';
                _reader.SetColumnIndexes(3,8);
                _reader.Skip();
                

                while (!_reader.EndOfStream)
                {
                    string[] _values = _reader.ReadCsvLine();
                    _rows.Add(_values);
                }
            }
            _w.Stop();


            _w.Start();


            using (CsvStreamReader _reader = new CsvStreamReader(_file))
            {
                foreach(var (first, second, third) in _reader.ReadAsEnumerable())
                {
                    Console.WriteLine(first, second, third);
                }
            }
            _w.Stop();
        }


        [TestMethod]
        public void TestStreamReaderDict()
        {

            string _file = System.IO.Path.GetTempFileName();

            using (CsvWriter<CsvSimple> _writer = new(_file) { Separator = ',' })
            {
                _writer.WriteHeader();
                _writer.Write(LargeData());
            }

            var _w = new StopWatch();

            _w.Start();

            List<Dictionary<string, string>> _rows = new List<Dictionary<string, string>>();

            using (CsvStreamReader _reader = new CsvStreamReader(_file))
            {
                _reader.Separator = ',';
                _reader.SetColumnIndexes(3, 8);

                while (!_reader.EndOfStream)
                {
                    Dictionary<string,string> _values = _reader.ReadCsvLineAsDictionary();
                    string tegenRekening = _values["Tegenrekening"];

                    _rows.Add(_values);
                }
            }
            _w.Stop();


            _w.Start();


            using (CsvStreamReader _reader = new CsvStreamReader(_file))
            {
                List<Dictionary<string, string>> _items = _reader.ReadAsEnumerableDictionary().ToList();

                Console.WriteLine("");
            }
            _w.Stop();
        }

        private IEnumerable<CsvSimple> LargeData()
        {
            for (int ii = 0; ii < 100000; ii++)
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