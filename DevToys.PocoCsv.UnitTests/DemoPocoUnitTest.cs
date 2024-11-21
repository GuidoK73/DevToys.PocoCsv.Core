using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using DevToys.PocoCsv.UnitTests.Models;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace DevToys.PocoCsv.UnitTests
{
    public class SimpleObject
    {
        public int Id { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
    }

    [TestClass]
    public class DemoPocoUnitTest
    {
        [TestMethod]
        public void TestReaderSimple()
        {
            string _file = System.IO.Path.GetTempFileName();

            using (CsvWriter<SimpleObject> _writer = new(_file) { Separator = ',' })
            {
                _writer.WriteHeader();
                _writer.Write(SimpleData());
            }

            string _text = File.ReadAllText(_file);

            using (CsvReader<SimpleObject> _reader = new(_file))
            {
                List<SimpleObject> _materialized = _reader.ReadAsEnumerable().ToList();
            }
        }

        private IEnumerable<SimpleObject> SimpleData(int count = 50)
        {
            for (int ii = 0; ii < count; ii++)
            {
                yield return  new SimpleObject() { Id = ii, Field1 = $"A{ii}", Field2 = $"b{ii}" };                
            }
        }
    }
}
