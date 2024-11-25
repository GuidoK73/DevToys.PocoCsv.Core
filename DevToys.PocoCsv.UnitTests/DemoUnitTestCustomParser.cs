using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text;

namespace DevToys.PocoCsv.UnitTests
{
    public class ParseBooleanNullable : ICustomCsvParse<bool?>
    {
        public bool? Read(StringBuilder value)
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

        public string Write(bool? value)
        {
            if (value.HasValue)
            {
                if (value == true)
                {
                    return "yes";
                }
                return "no";
            }
            return string.Empty;
        }
    }

    public class ParseText : ICustomCsvParse<string>
    {
        public string Read(StringBuilder value)
        {
            if (value.ToString() == "Test")
            {
                return "XXX";
            }
            return value.ToString();
        }

        public string Write(string value)
        {
            return value;
        }
    }

    public sealed class CsvPreParseTestObject
    {
        [Column(Index = 0, CustomParserType = typeof(ParseBooleanNullable))]
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
                //_reader.Skip(); // Slip header.
                var _rows = _reader.ReadAsEnumerable().ToArray(); // Materialize.

                Assert.AreEqual(true, _rows[0].IsOk);
                Assert.AreEqual(false, _rows[1].IsOk);
                Assert.AreEqual(null, _rows[2].IsOk);
                Assert.AreEqual("XXX", _rows[0].Text);
            }
        }
    }
}