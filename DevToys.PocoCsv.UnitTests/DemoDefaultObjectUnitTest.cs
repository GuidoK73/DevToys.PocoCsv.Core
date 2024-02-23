using DevToys.PocoCsv.Core;
using DevToys.PocoCsv.Core.CsvDataTypeObject;
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



 



     [TestClass]
    public class DemoDefaultObjectUnitTest
    {
        [TestMethod]
        public void TestReaderSimple()
        {
            var _CsvDto1 = new CsvDataTypeObject5() { Field1 = "01", Field2 = "02", Field3 = "03", Field4 = "04", Field5 = "05" };
            CsvDataTypeObject5 _CsvDto2 = "01,02,03,04,05"; // Implicit csv string to csv data type object.

            bool _equals = _CsvDto1 == _CsvDto2; // Compare by value

            string _csv = _CsvDto1; // implicit convert csv data type object to csv string.

            string _file = System.IO.Path.GetTempFileName();

            using (CsvWriter<CsvDataTypeObject5> _writer = new(_file) { Separator = ',' })
            {
                _writer.Open();
                _writer.WriteHeader();
                _writer.Write(SimpleData());
            }

            string _text = File.ReadAllText(_file);

            using (CsvReader<CsvDataTypeObject10> _reader = new(_file))
            {
                _reader.Open();
                foreach (var (id, name) in _reader.ReadAsEnumerable())
                {

                }
            }
        }

        private IEnumerable<CsvDataTypeObject5> SimpleData(int count = 50)
        {
            for (int ii = 0; ii < count; ii++)
            {
                yield return  new CsvDataTypeObject5() { Field1 = $"A{ii}", Field2 = $"b{ii}", Field3 = $"c{ii}", Field4 = $"d{ii}",  Field5= $"e{ii}" };                
            }
        }
    }
}
