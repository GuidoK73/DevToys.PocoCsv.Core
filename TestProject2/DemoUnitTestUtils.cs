using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TestProject2
{
    [TestClass]
    public class DemoUnitTestUtils
    {
        [TestMethod]
        public void TestUtils()
        {
            StringBuilder sb = new StringBuilder();
            for (int ii = 1; ii < 2000; ii++)
            {
                char c = (char)ii;

                sb.Append($"{ii} - {c}\r\n");
            }
            string _text = sb.ToString();

            string _file = Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "data.csv");
            int _sampleRows = 20;

            using (CsvStreamReader _reader = new CsvStreamReader(_file))
            {
                bool _succes = CsvUtils.GetCsvSeparator(_reader, out char _separator, _sampleRows);

                _reader.MoveToStart();

                string[] _header = CsvUtils.CsvHeader(_reader, _separator);

                _reader.MoveToStart();

                List<CsvColumnInfo> _schema = CsvUtils.GetCsvSchema(_reader, _sampleRows).ToList();

                _reader.MoveToStart();

                while (!_reader.EndOfStream)
                {
                    string[] _values = _reader.ReadCsvLine();
                }
            }
        }

        [TestMethod]
        public void TestBuilder()
        {
        }

    }


}