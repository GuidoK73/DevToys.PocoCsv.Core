using Delegates;
using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace TestProject2
{
    public class Csv
    {
        [Column(Index = 0)]
        public DateTime Datum { get; set; }

        [Column(Index = 1)]
        public string NaamOmschrijving { get; set; }

        [Column(Index = 2)]
        public string Rekening { get; set; }

        [Column(Index = 3)]
        public string Tegenrekening { get; set; }

        [Column(Index = 4)]
        public string Code { get; set; }

        [Column(Index = 5)]
        public string AfBij { get; set; }

        [Column(Index = 6)]
        public string Bedrag { get; set; }

        [Column(Index = 7)]
        public string MutatieSoort { get; set; }

        [Column(Index = 8)]
        public string Mededelingen { get; set; }
    }

    public class TestClass
    {
        public string MyProp { get; set; }
    }

    [TestClass]
    public class UnitTestComplex
    {
        [TestMethod]
        public void TestReaderComplex()
        {
            string file = Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "data.csv");

            using (CsvReader<Csv> _reader = new(file))
            {
                _reader.Open();

                _reader.Culture = CultureInfo.GetCultureInfo("nl-NL");

                foreach (Csv csv in _reader.Rows())
                {
                    Console.WriteLine(csv.Bedrag);
                }

                var _x = _reader.Rows().ToList();
                var _bij = _reader.Rows().Where(p => p.AfBij == "Bij").ToList();
            }
        }

        [TestMethod]
        public void TestDelegates()
        {
            Type _testClassType = typeof(TestClass);
            TestClass _testClassInstance = new TestClass();
            var _action = _testClassType.PropertySet<string>("MyProp");
            _action(_testClassInstance, "New Test Value");

            var _action2 = _testClassType.PropertySet("MyProp");
            _action2(_testClassInstance, "New Test Value 2");

            var _funcGet = _testClassType.PropertyGet("MyProp");
            var _xxx = _funcGet(_testClassInstance);

            Console.Write("X");
        }
    }
}