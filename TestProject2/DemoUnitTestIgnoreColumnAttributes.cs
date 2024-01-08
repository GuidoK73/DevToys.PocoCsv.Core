﻿using DevToys;
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
                _writer.Open();
                _writer.WriteHeader();
                _writer.Write(LargeData());
            }

            using (CsvReader<CsvSimpleNoAttribute> _reader = new(_file))
            {
                _reader.IgnoreColumnAttributes = true;
                _reader.Open();

                var _result = _reader.ReadAsEnumerable().ToList();
            }
        }


    }
}