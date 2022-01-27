using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;

namespace TestProject2
{
    [TestClass]
    public class UnitTestDynamic
    {
        [TestMethod]
        public void TestReaderDynamic()
        {
            string file = Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "data.csv");

            using (CsvReaderDynamic _reader = new(file))
            {
                _reader.FirstRowIsHeader = true;
                _reader.Open();
                foreach (dynamic row in _reader.Rows())
                {
                    Console.WriteLine("X");
                }
            }
        }

        [TestMethod]
        public void TestWriterDynamic()
        {
            string file = Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "data.csv");

            using (CsvWriterDynamic _writer = new(file))
            {
                _writer.Open();
                dynamic row = new ExpandoObject();
                row.Id = 124;
                row.Name = "Name";

                List<dynamic> _data = new List<dynamic>();
                _data.Add(row);

                _writer.Write(_data);
            }
        }
    }
}