using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace TestProject2
{
    [TestClass]
    public class UnitTestDynamic
    {
        [TestMethod]
        public void TestReaderDynamic()
        {
            string file = Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "data.csv");

            using (CsvReaderDynamic _reader = new(file))
            {
                _reader.FirstRowIsHeader = true;
                _reader.Open();
                foreach (dynamic row in _reader.Rows())
                {
                    Console.WriteLine("X");
                }
            }
        }


    }

}