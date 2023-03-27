using DevToys;
using DevToys.PocoCsv.Core;
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

                _reader.Skip();

                while (!_reader.EndOfStream)
                {
                    string[] _values = _reader.ReadCsvLine().ToArray();
                }
            }
        }

        [TestMethod]
        public void TestingMixedCRLFModes()
        {
            string file = @"D:\Temp\test.csv";
            using (CsvStreamWriter _writer = new CsvStreamWriter(file))
            {
                _writer.CRLFMode = CRLFMode.CRLF;
                _writer.WriteCsvLine("Row 1", "Row A,A", "A", "A1");
                _writer.WriteCsvLine("Row 2", "Row B,B", "B", "B2");
                _writer.CRLFMode = CRLFMode.LF;
                _writer.WriteCsvLine("Row 3", "Row C,C", "C", "C3");
                _writer.WriteCsvLine("Row 4", "Row D,D", "D", "D4");
                _writer.CRLFMode = CRLFMode.CR;
                _writer.WriteCsvLine("Row 5", "Row E,E", "E", "E5");
                _writer.WriteCsvLine("Row 6", "Row F,F", "F", "F6");
                _writer.Flush();
            }

            using (CsvStreamReader _reader = new CsvStreamReader(file))
            {
                _reader.Separator = ',';

                while (!_reader.EndOfStream)
                {
                    string[] _values = _reader.ReadCsvLine().ToArray();
                }
            }


        }
    }
}