using DevToys;
using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject2
{
    [TestClass]
    public class UnitTestStreamReader
    {
        [TestMethod]
        public void TestReaderComplex()
        {
            StopWatch _w = new StopWatch();

            _w.Start();
            string file = Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "data.csv");
            using (CsvStreamReader _reader = new CsvStreamReader(file))
            {
                while (!_reader.EndOfCsvStream)
                {
                    List<string> _values = _reader.ReadCsvLine().ToList();
                }
            }

            _w.Stop();
            Console.WriteLine(_w.Duration);



        }
    }
}
