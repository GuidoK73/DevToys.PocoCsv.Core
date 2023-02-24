using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TestProject2
{
    public class Data
    {
        [Column(Index = 0)]
        public string Collumn1 { get; set; }
        [Column(Index = 1)]
        public string Collumn2 { get; set; }
    }


    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestReaderComplex()
        {
            string _file = @"D:\Temp\test.csv";

            if (File.Exists(_file))
            {
                File.Delete(_file);
            }

            List<Data> _data = new List<Data>();
            _data.Add(new Data { Collumn1 = "01", Collumn2 = "AA" });
            _data.Add(new Data { Collumn1 = "02", Collumn2 = "BB" });
            _data.Add(new Data { Collumn1 = "03", Collumn2 = "CC" });

                              
            using (CsvWriter<Data> _csvWriter = new CsvWriter<Data>(_file))
            {
                _csvWriter.Open();
                _csvWriter.Write(_data);
            }

            using (CsvReader<Data> _csvReader = new CsvReader<Data>(_file))
            {
                _csvReader.Open();
                List<Data> _data2 = _csvReader.Rows().Where(p => p.Collumn1 != "02").ToList();
            }

       }

    }
}