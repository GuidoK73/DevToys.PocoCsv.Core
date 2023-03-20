using DevToys;
using DevToys.Poco.Csv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestProject2
{
    [TestClass]
    public class UnitTestStreamReader
    {
        [TestMethod]
        public void TestReader()
        {
            string file = Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "data.csv");
            using (CsvStreamReader _reader = new CsvStreamReader(file))
            {
                _reader.Separator = ',';

                while (!_reader.EndOfCsvStream)
                {
                    List<string> _values = _reader.ReadCsvLine().ToList();
                }
            }
        }

        [TestMethod]
        public void TestWriter()
        {
            string file = @"D:\Temp\test.csv";
            using (CsvStreamWriter _writer = new CsvStreamWriter(file))
            {
                var _line = new string[] { "Row 1", "Row A,A", "Row 3", "Row B" };
                _writer.WriteCsvLine(_line);
            }
        }
    }
}