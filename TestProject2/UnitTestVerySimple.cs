using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace TestProject2
{
    public class CsvVerySimple
    {
        [Column(Index = 0)]
        public string Column1 { get; set; }

        [Column(Index = 1)]
        public string Column2 { get; set; }

        [Column(Index = 2)]
        public string Column3 { get; set; }

        [Column(Index = 5)]
        public string Column5 { get; set; }
    }

    [TestClass]
    public class UnitTestVerySimple
    {
        [TestMethod]
        public void TestReaderSimple()
        {
            string file = Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "data.csv");

            using (CsvReader<CsvVerySimple> _reader = new(file))
            {
                _reader.Open();
                foreach (CsvVerySimple row in _reader.Rows())
                {
                    Console.WriteLine(row.Column1);
                }
            }
        }

        [TestMethod]
        public void TestX()
        {
            Guid _myGuid = Guid.NewGuid();

            string _test = _myGuid.ToString();
            Console.WriteLine("X");
        }
    }

    public static class Extensions
    {
        public static string ToString(this Guid value)
        {
            return value.ToString().ToUpper();
        }
    }
}