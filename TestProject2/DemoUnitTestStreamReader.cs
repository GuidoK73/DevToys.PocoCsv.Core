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
    public class DemoUnitTestStreamReader
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
    }
}