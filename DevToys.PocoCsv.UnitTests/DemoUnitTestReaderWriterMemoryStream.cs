using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DevToys.PocoCsv.UnitTests
{
    public class Data
    {
        [Column(Index = 0)]
        public string Collumn1 { get; set; }

        [Column(Index = 1)]
        public string Collumn2 { get; set; }

        [Column(Index = 2, Header = "Test")]
        public byte[] Collumn3 { get; set; }

        [Column(Index = 3)]
        public DateTime TestDateTime { get; set; }

        [Column(Index = 4)]
        public DateTime? TestDateTimeNull { get; set; }

        [Column(Index = 5)]
        public Int32 TestInt { get; set; }

        [Column(Index = 6, OutputNullValue = "[NULL]")]
        public Int32? TestIntNull { get; set; }
    }

    [TestClass]
    public class DemoUnitTestReaderWriterMemoryStream
    {
        private IEnumerable<Data> GetTestData()
        {
            yield return new Data
            {
                Collumn1 = "01",
                Collumn2 = "AA",
                Collumn3 = new byte[3] { 2, 4, 6 },
                TestDateTime = DateTime.Now,
                TestDateTimeNull = DateTime.Now,
                TestInt = 100,
                TestIntNull = 200
            };
            yield return new Data
            {
                Collumn1 = "01",
                Collumn2 = "AA",
                Collumn3 = new byte[3] { 2, 4, 6 },
                TestDateTime = DateTime.Now,
                TestDateTimeNull = DateTime.Now,
                TestInt = 100,
                TestIntNull = 200
            };
            yield return new Data
            {
                Collumn1 = "04",
                Collumn2 = "BB",
                Collumn3 = new byte[3] { 8, 9, 10 },
                TestDateTime = DateTime.Now,
                TestDateTimeNull = null,
                TestInt = 300,
                TestIntNull = 600
            };
        }

        [TestMethod]
        public void TestReaderWriterMemoryStream()
        {
            string _result;

            using (MemoryStream _stream = new MemoryStream())
            {
                using (CsvWriter<Data> _csvWriter = new CsvWriter<Data>(_stream) { Separator = ',' })
                using (CsvReader<Data> _csvReader = new CsvReader<Data>(_stream) { Separator = ',' })
                {
                    _csvWriter.Open();
                    _csvWriter.WriteHeader();
                    _csvWriter.Write(GetTestData());

                    _result = StreamToString(_stream);
                }
            }
        }



        [TestMethod]
        public void TestReaderWriterMemoryStream2()
        {
            List<Data> _result = new List<Data>();

            using (MemoryStream _stream = new MemoryStream())
            {
                using (CsvWriter<Data> _csvWriter = new CsvWriter<Data>(_stream))
                using (CsvReader<Data> _csvReader = new CsvReader<Data>(_stream))
                {
                    _csvWriter.CRLFMode = CRLFMode.CRLF; // Default
                    _csvWriter.Separator = ';';
                    _csvWriter.Open();
                    _csvWriter.WriteHeader();
                    _csvWriter.Write(GetTestData());

                    _csvReader.Open();
                    _csvReader.DetectSeparator();
                    _csvReader.Skip();
                    _result = _csvReader.ReadAsEnumerable().ToList();
                }
            }
        }

        [TestMethod]
        public void TestReaderWriterMemoryStream3()
        {
            List<Data> _result = new List<Data>();

            using (MemoryStream _stream = new MemoryStream())
            {
                using (CsvWriter<Data> _csvWriter = new CsvWriter<Data>(_stream))
                using (CsvReader<Data> _csvReader = new CsvReader<Data>(_stream))
                {
                    _csvWriter.CRLFMode = CRLFMode.CR;
                    _csvWriter.Separator = ';';
                    _csvWriter.Open();
                    _csvWriter.WriteHeader();
                    _csvWriter.Write(GetTestData());

                    _csvReader.Open();
                    _csvReader.DetectSeparator();
                    _csvReader.Skip();
                    _result = _csvReader.ReadAsEnumerable().ToList();
                }
            }
        }

        [TestMethod]
        public void TestReaderWriterMemoryStream4()
        {
            List<Data> _result = new List<Data>();

            using (MemoryStream _stream = new MemoryStream())
            {
                using (CsvWriter<Data> _csvWriter = new CsvWriter<Data>(_stream))
                using (CsvReader<Data> _csvReader = new CsvReader<Data>(_stream))
                {
                    _csvWriter.CRLFMode = CRLFMode.LF;
                    _csvWriter.Separator = ';';
                    _csvWriter.Open();
                    _csvWriter.WriteHeader();
                    _csvWriter.Write(GetTestData());

                    _csvReader.Open();
                    _csvReader.DetectSeparator();
                    _csvReader.Skip();
                    _result = _csvReader.ReadAsEnumerable().ToList();
                }
            }
        }

        private string _distortedData = @"Collumn1,Collumn2,Test,TestDateTime,TestDateTimeNull,TestInt,TestIntNull

01,AA,AgQG,29/03/2023 14:21:39,29/03/2023 14:21:39,100,200
01,AA,AgQG,29/03/2023 14:21:39,29/03/2023 14:21:39,100,200
04,BB,CAkK,29/03/2023 14:21:39,,300,600";

        [TestMethod]
        public void TestReaderWriterMemoryStream5()
        {
            List<Data> _result = new List<Data>();

            byte[] byteArray = Encoding.Default.GetBytes(_distortedData);

            using (MemoryStream _stream = new MemoryStream(byteArray))
            {
                using (CsvReader<Data> _csvReader = new CsvReader<Data>(_stream))
                {
                    _csvReader.EmptyLineBehaviour = EmptyLineBehaviour.NullValue;
                    _csvReader.Open();
                    _csvReader.DetectSeparator();
                    _csvReader.Skip();
                    _result = _csvReader.ReadAsEnumerable().ToList();
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