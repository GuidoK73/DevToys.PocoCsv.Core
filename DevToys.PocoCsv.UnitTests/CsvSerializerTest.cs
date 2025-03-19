using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevToys.PocoCsv.UnitTests
{

    public class TestSerializerObject
    {
        [Column(Index = 0)]
        public int Id { get; set; }
        [Column(Index = 1)]
        public string Field1 { get; set; }
        [Column(Index = 2)]
        public string Field2 { get; set; }
    }

    [TestClass]
    public class CsvSerializerTest
    {

        private IEnumerable<TestSerializerObject> SimpleData(int count = 10)
        {
            for (int ii = 0; ii < count; ii++)
            {
                yield return new TestSerializerObject() { Id = ii, Field1 = $"A{ii}", Field2 = $"b{ii}" };
            }
        }

        [TestMethod]
        public void TestSerialize()
        {
            CsvSerializer serializer = new CsvSerializer(new CsvSerializerSettings() { Header = true } );

            string _data = serializer.SerializeObject(SimpleData());

            var _resultData = serializer.DeserializeObject<TestSerializerObject>(_data).ToList();
        } 

        [TestMethod]
        public void TestSerialize2()
        {
            CsvSerializer _serializer = new CsvSerializer();
            string _data = @"1,2,3
a,b,c
d,e,f";
            List<string[]> _result = _serializer.Deserialize(_data).ToList();
            string _data2 = _serializer.Serialize(_result);

        }

        [TestMethod]
        public void TestSerialize3()
        {
            CsvSerializer _serializer = new CsvSerializer();

            StringBuilder _sb = new StringBuilder();
            _sb.AppendCsvLine(',', "a", "b", "c");
            _sb.AppendCsvLine(',', "1", "2", "3");
            _sb.AppendCsvLine(',', "d", "e", "f");

            List<string[]> _result = _serializer.Deserialize(_sb).ToList();

            StringBuilder _sbResult = new StringBuilder();
            _serializer.Serialize(_result, ref _sbResult);
            string _stringResult = _sbResult.ToString();

        }
    }
}
