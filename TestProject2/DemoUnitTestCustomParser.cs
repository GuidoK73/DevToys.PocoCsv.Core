using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text;

namespace TestProject2
{
    public class ParseBoolean : ICustomCsvParse<bool?>
    {
        public bool? Parse(StringBuilder value)
        {
            switch (value.ToString().ToLower())
            {
                case "on":
                case "true":
                case "yes":
                case "1":
                    return true;
                case "off":
                case "false":
                case "no":
                case "0":
                    return false;
                default:
                    return null;
            }
        }
    }

    public class ParseText : ICustomCsvParse<string>
    {
        public string Parse(StringBuilder value)
        {
            if (value.ToString() == "Test")
            {
                return "XXX";
            }
            return value.ToString();
        }
    }

    public sealed class CsvPreParseTestObject
    {
        [Column(Index = 0, CustomParserType = typeof(ParseBoolean))]
        public Boolean? IsOk { get; set; }

        [Column(Index = 1)]
        public string Name { get; set; }

        [Column(Index = 2, CustomParserType = typeof(ParseText))]
        public string Text { get; set; }
    }

    [TestClass]
    public class DemoUnitTestCustomParser
    {
        [TestMethod]
        public void CustomParseTest()
        {
            string _file = System.IO.Path.GetTempFileName();

            using (CsvStreamWriter _writer = new CsvStreamWriter(_file))
            {
                _writer.WriteCsvLine("IsOk", "Name", "Text");
                _writer.WriteCsvLine("Yes", "name 1", "Test");
                _writer.WriteCsvLine("no", "name 2", "Ok");
                _writer.WriteCsvLine("", "name 3", "AAA");
                _writer.Flush();
            }

            using (var _reader = new CsvReader<CsvPreParseTestObject>(_file))
            {
                _reader.Open();

                _reader.Skip(); // Slip header.
                var _rows = _reader.ReadAsEnumerable().ToArray(); // Materialize.

                Assert.AreEqual(true, _rows[0].IsOk);
                Assert.AreEqual(false, _rows[1].IsOk);
                Assert.AreEqual(null, _rows[2].IsOk);
                Assert.AreEqual("XXX", _rows[0].Text);
            }
        }
    }
}