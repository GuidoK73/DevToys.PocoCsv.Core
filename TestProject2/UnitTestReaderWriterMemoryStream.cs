using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
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
        [Column(Index = 2, Header = "Test" )]
        public byte[] Collumn3 { get; set; }

    }


    [TestClass]
    public class UnitTestReaderWriterMemoryStream
    {
        [TestMethod]
        public void TestReaderWriterMemoryStream()
        {

            List<Data> _data = new List<Data>();
            _data.Add(new Data { Collumn1 = "01", Collumn2 = "AA", Collumn3 = new byte[3] { 2, 4, 6 } });
            _data.Add(new Data { Collumn1 = "02", Collumn2 = "BB", Collumn3 = new byte[3] { 1, 2, 3 } });
            _data.Add(new Data { Collumn1 = "03", Collumn2 = "CC", Collumn3 = null});


            List<Data> _data2 = new List<Data>();

            using (MemoryStream _stream = new MemoryStream())
            {

                using (CsvWriter<Data> _csvWriter = new CsvWriter<Data>(_stream))
                using (CsvReader<Data> _csvReader = new CsvReader<Data>(_stream))
                {
                    _csvWriter.Separator = ';';
                    _csvWriter.Open();
                    _csvWriter.WriteHeader();
                    _csvWriter.Write(_data);

                    //string _test = StreamToString(_stream);

                    _csvReader.Open();
                    _csvReader.DetectSeparator();
                    _data2 = _csvReader
                        .ReadAsEnumerable()
                        .Skip(1) // skip header
                        .Where(p => p.Collumn1 != "02")
                        .ToList();
                }
            }
       }


        public static string StreamToString(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                stream.Position = 0;
                return reader.ReadToEnd();
            }
        }

    }
}