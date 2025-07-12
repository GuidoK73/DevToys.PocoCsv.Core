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
using System.Xml.Serialization;



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

            CsvDataTypeObject _CsvDto3 = "1,2,4";
            CsvDataTypeObject _CsvDto4 = "1,2,3";


            bool x = _CsvDto3 > _CsvDto4;


            CsvDataTypeObject _CsvDto5 = "1,2,3";
            CsvDataTypeObject _CsvDto6 = null;

            bool x2 = _CsvDto5 > _CsvDto6; 


            using (CsvWriter<CsvDataTypeObject> _writer = new(_file) { Separator = ',', ColumnLimit = 5 })
            {
                _writer.WriteHeader();
                _writer.Write(SimpleData());
            }

            string _text = File.ReadAllText(_file);

            using (CsvReader<CsvDataTypeObject> _reader = new(_file))
            {
                foreach (var (id, name) in _reader.ReadAsEnumerable())
                {

                }
            }
        }



        private IEnumerable<CsvDataTypeObject> SimpleData(int count = 50)
        {
            for (int ii = 0; ii < count; ii++)
            {
                yield return new CsvDataTypeObject($"A{ii},B{ii},C{ii},D{ii},E{ii}");
            }
        }

    }
}
