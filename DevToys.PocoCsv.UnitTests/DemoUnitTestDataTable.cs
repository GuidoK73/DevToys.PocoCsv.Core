using DevToys;
using DevToys.PocoCsv.Core;
using DevToys.PocoCsv.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DevToys.PocoCsv.UnitTests.Models;
using System.IO;
using System.Text;
using static System.Net.WebRequestMethods;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;

namespace DevToys.PocoCsv.UnitTests
{
    [TestClass]
    public class DemoUnitTestDataTable
    {
        [TestMethod]
        public void TestDataTable()
        {
            string _file = System.IO.Path.GetTempFileName();

            using (CsvWriter<CsvSimple> _writer = new(_file) { Separator = ',' })
            {
                _writer.Open();
                _writer.WriteHeader();
                _writer.Write(LargeData());
            }

            var _w = new StopWatch();

            _w.Start();

            DataTable _table = new DataTable();
            _table.ImportCsv(_file, ',', true);


            _w.Stop();
            Console.WriteLine(_w.Duration);

            _file = System.IO.Path.GetTempFileName();
            _table.ExportCsv(_file, ',');

        }

        [TestMethod]
        public void BufferedReading()
        {
            string _file = @"D:\largedata.csv";

            //using (CsvWriter<CsvSimple> _writer = new(_file) { Separator = ',' })
            //{
            //    _writer.Open();
            //    _writer.WriteHeader();
            //    _writer.Write(LargeData());
            //}

            StreamReader _StreamReader = new StreamReader(path: _file, encoding: Encoding.Default, detectEncodingFromByteOrderMarks: true, bufferSize: 4096);

            var _w = new StopWatch();

            _w.Start();

            int _pos = 0;

            while (true)
            {
                int _byte = _StreamReader.BaseStream.ReadByte();
                if (_byte == -1)
                    break;
                _pos++;
            }

            _w.Stop();
            Console.WriteLine(_w.Duration);

            _w.Start();

            _StreamReader.BaseStream.Position = 0;

            BufferedReader _buffer = new BufferedReader(_StreamReader.BaseStream);
            _pos = 0;
            while (true)
            {
                int _ret = _buffer.ReadByte();
                if (_ret == -1)
                {
                    break;
                }
                _pos++;

            }

            _w.Stop();
            Console.WriteLine(_w.Duration);

        }


        public class BufferedReader
        {
            private Stream _Stream;
            private byte[] _Buffer;
            private bool _Eos = false;
            private int _BufferPos = 0;
            private byte _RetValue = 0;

            

            public BufferedReader(Stream stream, int bufferSize = 4096)
            {
                _Stream = stream;
                _Buffer = new byte[bufferSize];
                ReadBuffer();
            }

            public int ReadByte()
            {
                if (_BufferPos >= _Buffer.Length)
                {
                    if (_Eos)
                    {
                        return -1;
                    }
                    _BufferPos = 0;
                    ReadBuffer();
                }

                _RetValue = _Buffer[_BufferPos];
                _BufferPos++;
                return _RetValue;
            }

            private void ReadBuffer()
            {
                _BufferPos = 0;
                int _byte = _Stream.Read(_Buffer, 0, _Buffer.Length);
                if (_byte == 0)
                {
                    _Eos = true;
                }
            }
        }



        private IEnumerable<CsvSimple> LargeData()
        {
            for (int ii = 0; ii < 1000000; ii++)
            {
                CsvSimple _line = new()
                {
                    AfBij = "bij",
                    Bedrag = "100",
                    Code = "test",
                    Datum = "20200203",
                    Mededelingen = $"test {ii}",
                    Rekening = "3434",
                    Tegenrekening = "3423424",
                    NaamOmschrijving = $"bla,bl,errwerwe,rrwer \"a fdsfd\"dsfsf \" cvcxv \" sddsfsds \" \" ssdgfgdg \" tuytuuyuyu {ii}",
                    MutatieSoort = "Bij"
                };
                yield return _line;
            }
        }
    }
}