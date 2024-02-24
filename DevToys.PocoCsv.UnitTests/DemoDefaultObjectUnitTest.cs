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
            var _CsvDto1 = new CsvDataTypeObject() { Field01 = "01", Field02 = "02", Field03 = "03", Field04 = "04", Field05 = "05" };
            CsvDataTypeObject _CsvDto2 = "01,02,03,04,05"; // Implicit csv string to csv data type object.

            bool _equals = _CsvDto1 == _CsvDto2; // Compare by value

            string _csv = _CsvDto1; // implicit convert csv data type object to csv string.

            string _file = System.IO.Path.GetTempFileName();

            using (CsvWriter<CsvDataTypeObject> _writer = new(_file) { Separator = ',', ColumnLimit = 5 })
            {
                _writer.Open();
                _writer.WriteHeader();
                _writer.Write(SimpleData());
            }

            string _text = File.ReadAllText(_file);

            using (CsvReader<CsvDataTypeObject> _reader = new(_file))
            {
                _reader.Open();
                foreach (var (id, name) in _reader.ReadAsEnumerable())
                {

                }
            }
        }



        private IEnumerable<CsvDataTypeObject> SimpleData(int count = 50)
        {
            for (int ii = 0; ii < count; ii++)
            {
                yield return new CsvDataTypeObject() { Field01 = $"A{ii}", Field02 = $"b{ii}", Field03 = $"c{ii}", Field04 = $"d{ii}", Field05 = $"e{ii}" };
            }
        }

    }
}
