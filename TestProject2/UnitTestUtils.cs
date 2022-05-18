using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestProject2
{
    [TestClass]
    public class UnitTestUtils
    {
        [TestMethod]
        public void TestUtils()
        {
            string _file = Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "data.csv");
            int _sampleRows = 20;

            using (CsvStreamReader _reader = new CsvStreamReader(_file))
            {
                bool _succes = CsvUtils.GetCsvSeparator(_reader, out char _separator, _sampleRows);

                _reader.Position = 0;

                string[] _header = CsvUtils.CsvHeader(_reader, _separator);

                _reader.Position = 0;

                List<CsvColumnInfo> _schema = CsvUtils.GetCsvSchema(_reader, _sampleRows).ToList();

                _reader.Position = 0;

                while (!_reader.EndOfCsvStream)
                {
                    List<string> _values = _reader.ReadCsvLine().ToList();
                }
            }
        }
    }
}