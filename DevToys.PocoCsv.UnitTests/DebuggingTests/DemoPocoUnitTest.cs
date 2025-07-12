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
        public void TestReaderSimple2()
        {
            var _data = SimpleData(30);
            while (true)
            {
                var _batch = _data.Take(10);
                if (!_batch.Any())
                {
                    break;
                }
                var _materialized = _batch.ToList();
                foreach (var item in _batch)
                { }

            }
        }

        private IEnumerable<SimpleObject> SimpleData(int count = 50)
        {
            for (int ii = 0; ii < count; ii++)
            {
                yield return new SimpleObject() { Id = ii, Field1 = $"A{ii}", Field2 = $"b{ii}" };
            }
        }


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


        [TestMethod]
        public void TestWriteString()
        {

            // Writing CSV directly into string.
            string _stringResult = string.Empty;

            using (var stream = new MemoryStream())
            using (var writer = new CsvWriter<SimpleObject>(stream))
            {
                writer.WriteHeader();
                writer.Write(SimpleData());
                writer.Flush();

                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    _stringResult = reader.ReadToEnd();
                }
            }

            // Reading from string into collection.
            byte[] byteArray = Encoding.UTF8.GetBytes(_stringResult);
            using (var memStream = new MemoryStream(byteArray))
            using (var streamReader = new StreamReader(memStream))
            using (var reader = new CsvReader<SimpleObject>(streamReader))
            {
                List<SimpleObject> _materialized = reader.ReadAsEnumerable().ToList();
            }

        }

    }
}
