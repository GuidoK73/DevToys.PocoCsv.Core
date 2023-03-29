using Delegates;
using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace TestProject2
{



    public sealed class Csv
    {
        [Column(Index = 0)]
        public string Datum { get; set; }

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
        public Decimal Bedrag { get; set; }

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

            string file = @"F:\DEV\GIT\DevToys.PocoCsv.Core\TestProject2\data.csv";

            using (CsvReader<Csv> _reader = new(file))
            {
                _reader.Open();

                _reader.Culture = CultureInfo.GetCultureInfo("nl-NL");

                _reader.Skip(1);

                var _x = _reader.ReadAsEnumerable().ToList();

                _reader.MoveToStart();
                _reader.Skip(1);

                var _bij = _reader.ReadAsEnumerable().Where(p => p.AfBij == "Af").ToList();
            }
        }


    }
}