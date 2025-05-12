using DevToys.PocoCsv.Core;
using DevToys.PocoCsv.Core.CsvDataTypeObject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using DevToys.PocoCsv.UnitTests.Models;
using System.Security.Cryptography.X509Certificates;
using static System.Net.Mime.MediaTypeNames;

namespace DevToys.PocoCsv.UnitTests
{
    public enum TestEnum
    {
        XXX = 1,
        YYY = 2
    }
    ///
    /// <summary>
    /// This tests are actual tests for maintaining correct behaviour.
    /// </summary>
    [TestClass]
    public class UnitTestValidate
    {

        [TestMethod]
        public void Validate()
        {
            ValidateCRLFStreamReaderAndWriter();
            TestTypeFields();
            ValidateCsvReader();
            ValidateCsvReaderEmptyLineBehaviour();
            TestEncoding();
            TestColIndex();
            TestReaderSimple();
            TestCsvDataTypeObject();
            CustomParseTest();

            // CsvSerializer tests
            ValidateCRLFCsvSerializer();
            TestTypeFieldsCsvSerializer();
            TestColIndexCsvSerializer();
        }

        public class SimpleObjectA
        {
            public int Id { get; set; }
            public string Field1 { get; set; }
            public string Field2 { get; set; }
        }

        public class SimpleObjectB
        {
            public int Id { get; set; }
            public string Field2 { get; set; }
            public string Field3 { get; set; }
        }

        [TestMethod]
        public void TestCsvDataTypeObject()
        {
            var _CsvDto1 = new CsvDataTypeObject() { Field01 = "01", Field02 = "02", Field03 = "03", Field04 = "04", Field05 = "05" };
            CsvDataTypeObject _CsvDto2 = "01,02,03,04,05"; // Implicit csv string to csv data type object.

            Assert.IsTrue(_CsvDto1 == _CsvDto2);

            string _csv = _CsvDto1; // implicit convert csv data type object to csv string.

            Assert.AreEqual(_csv, "01,02,03,04,05");

            _CsvDto1 = "1,2,4";
            _CsvDto2 = "1,2,3";
            Assert.IsTrue(_CsvDto1 > _CsvDto2);

            _CsvDto1 = "1,2,3";
            _CsvDto2 = "1,2,4";
            Assert.IsTrue(_CsvDto1 < _CsvDto2);

            _CsvDto1 = "1,2,3";
            _CsvDto2 = null;
            Assert.IsTrue(_CsvDto1 > _CsvDto2);

            _CsvDto1 = null;
            _CsvDto2 = "1,2,3";
            Assert.IsTrue(_CsvDto1 < _CsvDto2);

            _CsvDto1 = null;
            _CsvDto2 = null;
            Assert.IsTrue(_CsvDto1 == _CsvDto2);

            _CsvDto1 = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50";
            _CsvDto2 = _CsvDto1.Clone() as CsvDataTypeObject;
            Assert.IsTrue(_CsvDto1 == _CsvDto2);

            _CsvDto1 = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,aaa";
            _CsvDto2 = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,bbb";
            Assert.IsTrue(_CsvDto1 < _CsvDto2);

        }


        [TestMethod]
        public void TestReaderSimple()
        {
            string _file = System.IO.Path.GetTempFileName();

            using (CsvWriter<SimpleObjectA> _writer = new(_file) { Separator = ',' })
            {
                _writer.WriteHeader();
                _writer.Write(SimpleData(1));
            }

            string _text = File.ReadAllText(_file);

            using (CsvReader<SimpleObjectB> _reader = new(_file))
            {
                List<SimpleObjectB> _materialized = _reader.ReadAsEnumerable().ToList();

                var _item = _materialized.FirstOrDefault();
                Assert.IsNotNull(_item);

                Assert.AreEqual(_item.Id, 0);
                Assert.AreEqual(_item.Field2, "b0");
                Assert.AreEqual(_item.Field3, null);
            }
        }

        private IEnumerable<SimpleObjectA> SimpleData(int count = 50)
        {
            for (int ii = 0; ii < count; ii++)
            {
                yield return new SimpleObjectA() { Id = ii, Field1 = $"A{ii}", Field2 = $"b{ii}" };
            }
        }


        [TestMethod]
        public void ValidateCsvReader()
        {
            string file = System.IO.Path.GetTempFileName();

            using (CsvWriter<CsvSimpleSmall> _writer = new CsvWriter<CsvSimpleSmall>(file) { Separator = ',' })
            {
                _writer.CRLFMode = CRLFMode.CRLF;
                _writer.Write(ROWA);
                _writer.CRLFMode = CRLFMode.LF;
                _writer.Write(ROWB);
                _writer.CRLFMode = CRLFMode.CR;
                _writer.Write(ROWC);
                _writer.CRLFMode = CRLFMode.CRLF;
                _writer.Write(ROWD);
                _writer.CRLFMode = CRLFMode.LF;
                _writer.Write(ROWE);
                _writer.CRLFMode = CRLFMode.CR;
                _writer.Write(ROWF);
            }

            string _text = File.ReadAllText(file);

            using (CsvReader<CsvSimpleSmall> _reader = new CsvReader<CsvSimpleSmall>(file) { Separator = ',' })
            {
                var _rowA = _reader.Read();
                var _rowB = _reader.Read();
                var _rowC = _reader.Read();
                var _rowD = _reader.Read();
                var _rowE = _reader.Read();
                var _rowF = _reader.Read();


                Assert.AreEqual(_rowA.AfBij, "111");
                Assert.AreEqual(_rowA.Rekening, "AA");
                Assert.AreEqual(_rowA.Tegenrekening, "X");

                Assert.AreEqual(_rowB.AfBij, "222");
                Assert.AreEqual(_rowB.Rekening, "BB");
                Assert.AreEqual(_rowB.Tegenrekening, "Y");

                Assert.AreEqual(_rowC.AfBij, "333");
                Assert.AreEqual(_rowC.Rekening, "CC");
                Assert.AreEqual(_rowC.Tegenrekening, "Z");


                Assert.AreEqual(_rowD.AfBij, "AAA1 \"\" ");
                Assert.AreEqual(_rowD.Rekening, "BBB\r\n");
                Assert.AreEqual(_rowD.Tegenrekening, "AAA,BBB");

                Assert.AreEqual(_rowE.AfBij, "AAA2 \"\"");
                Assert.AreEqual(_rowE.Rekening, "BBB\r\n \"");
                Assert.AreEqual(_rowE.Tegenrekening, "CCC,DDD");

                Assert.AreEqual(_rowF.AfBij, "AAA3 \",\r\"");
                Assert.AreEqual(_rowF.Rekening, "BBB\r,\n \"");
                Assert.AreEqual(_rowF.Tegenrekening, "EEE,FFF");

            }
        }

        [TestMethod]
        public void TestColIndex()
        {
            // Test using another class for reading without using all indexes.

            string file = System.IO.Path.GetTempFileName();

            List<CsvSimple> _fullType = new List<CsvSimple>();
            List<CsvSimpleSmall> _smallType = new List<CsvSimpleSmall>();


            using (CsvWriter<CsvSimple> _writer = new CsvWriter<CsvSimple>(file) { Separator = ',' })
            {
                _fullType = CsvSimpleData(5).ToList();
                _writer.Write(_fullType);
            }

            string _text = File.ReadAllText(file);

            using (CsvReader<CsvSimpleSmall> _reader = new CsvReader<CsvSimpleSmall>(file) { Separator = ',' })
            {
                _smallType = _reader.ReadAsEnumerable().ToList();
            }

            for (int ii = 0; ii < 5; ii++)
            {
                Assert.AreEqual(_smallType[ii].AfBij, _fullType[ii].AfBij);
                Assert.AreEqual(_smallType[ii].Tegenrekening, _fullType[ii].Tegenrekening);
                Assert.AreEqual(_smallType[ii].Rekening, _fullType[ii].Rekening);
            }
        }

        [TestMethod]
        public void TestEncoding()
        {
            string fileUTF32 = System.IO.Path.GetTempFileName();
            string fileUTF8 = System.IO.Path.GetTempFileName();
            string fileASCII = System.IO.Path.GetTempFileName();

            using (CsvWriter<CsvSimple> _writer = new CsvWriter<CsvSimple>(path: fileUTF32,  encoding : Encoding.UTF32,  culture: CultureInfo.InvariantCulture,  separator: ',' ))
            {
                var _data = CsvSimpleData(5);
                _writer.Write(_data);
            }
            using (CsvWriter<CsvSimple> _writer = new CsvWriter<CsvSimple>(path: fileUTF8, encoding: Encoding.UTF8, culture: CultureInfo.InvariantCulture, separator: ',' ))
            {
                var _data = CsvSimpleData(5);
                _writer.Write(_data);
            }
            using (CsvWriter<CsvSimple> _writer = new CsvWriter<CsvSimple>(path: fileASCII, encoding: Encoding.ASCII, culture: CultureInfo.InvariantCulture, separator: ','))
            {
                var _data = CsvSimpleData(5);
                _writer.Write(_data);
            }

            string _UTF32 = File.ReadAllText(fileUTF32);

            List<CsvSimpleSmall> _dataUTF32;
            List<CsvSimpleSmall> _dataUTF8;
            List<CsvSimpleSmall> _dataASCII;

            using (CsvReader<CsvSimpleSmall> _reader = new CsvReader<CsvSimpleSmall>(fileUTF32) { Separator = ',' })
            {
                _dataUTF32 = _reader.ReadAsEnumerable().ToList();
                Console.WriteLine('X');
            }
            using (CsvReader<CsvSimpleSmall> _reader = new CsvReader<CsvSimpleSmall>(fileUTF8) { Separator = ',' })
            {
                _dataUTF8 = _reader.ReadAsEnumerable().ToList();
                Console.WriteLine('X');
            }
            using (CsvReader<CsvSimpleSmall> _reader = new CsvReader<CsvSimpleSmall>(fileASCII) { Separator = ',' })
            {
                _dataASCII = _reader.ReadAsEnumerable().ToList();
                Console.WriteLine('X');
            }

            for (int ii = 0; ii < 5; ii++)
            {
                Assert.AreEqual(_dataUTF32[ii].AfBij, _dataUTF8[ii].AfBij);
                Assert.AreEqual(_dataUTF32[ii].AfBij, _dataASCII[ii].AfBij);

                Assert.AreEqual(_dataUTF32[ii].Rekening, _dataUTF8[ii].Rekening);
                Assert.AreEqual(_dataUTF32[ii].Rekening, _dataASCII[ii].Rekening);

                Assert.AreEqual(_dataUTF32[ii].Tegenrekening, _dataUTF8[ii].Tegenrekening);
                Assert.AreEqual(_dataUTF32[ii].Tegenrekening, _dataASCII[ii].Tegenrekening);

            }

        }


        private IEnumerable<CsvSimple> CsvSimpleData(int count)
        {
            // 
            for (int ii = 0; ii < count; ii++)
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
                    NaamOmschrijving = $"bla,bla {ii}",
                    MutatieSoort = "Bij"
                };
                yield return _line;
            }
        }

        [TestMethod]
        public void ValidateCsvReaderEmptyLineBehaviour()
        {
            string file = System.IO.Path.GetTempFileName();

            using (CsvWriter<CsvTypesData> _writer = new(file) { Separator = ',' })
            {
                //_writer.Culture = CultureInfo.GetCultureInfo("en-US");
                _writer.WriteHeader();
                _writer.NullValueBehaviour = WriteNullValueBehaviour.EmptyLine;
                _writer.CRLFMode = CRLFMode.CRLF;
                _writer.WriteHeader();
                _writer.CRLFMode = CRLFMode.LF;
                _writer.Write(ROW1);
                _writer.Write((CsvTypesData)null); // Write empty row. (Mode is EmptyLine)
                _writer.CRLFMode = CRLFMode.CR;
                _writer.Write((CsvTypesData)null); // Write empty row. (Mode is EmptyLine)
                _writer.Write((CsvTypesData)null); // Write empty row. (Mode is EmptyLine)
                _writer.CRLFMode = CRLFMode.CRLF;
                _writer.Write(ROW2);
            }

            string _text = File.ReadAllText(file);

            using (CsvReader<CsvTypesData> _reader = new(file) { Separator = ',' })
            {
                _reader.EmptyLineBehaviour = EmptyLineBehaviour.NullValue;
                _reader.MoveToStart();
                _reader.SkipHeader();
                _reader.Skip();
                var _items = _reader.ReadAsEnumerable().ToList();
                Assert.AreEqual(5, _items.Count);
                Assert.AreEqual(_items[1], null);
                Assert.AreEqual(_items[2], null);
                Assert.AreEqual(_items[3], null);

                _reader.EmptyLineBehaviour = EmptyLineBehaviour.SkipAndReadNext;
                _reader.MoveToStart();
                _reader.SkipHeader();
                _reader.Skip();
                _items = _reader.ReadAsEnumerable().ToList();
                Assert.AreEqual(2, _items.Count);



                Assert.AreEqual(2, _items.Count);
            }
        }

        /// <summary>
        /// This TestMethod tests the CsvStreamReader and the CsvStreamWriter
        /// 1. Testing varying cases of line breaks.
        /// 2. Testing Escaping ',' '\r' '\n'
        /// 3. Testing DetectSeparator()
        /// 4. Testing Escaping of Alternate Separators
        /// </summary>
        [TestMethod]
        public void ValidateCRLFStreamReaderAndWriter()
        {
            string file = System.IO.Path.GetTempFileName();

            using (CsvStreamWriter _writer = new CsvStreamWriter(file))
            {
                _writer.Separator = ','; // Alternate separator

                _writer.CRLFMode = CRLFMode.CRLF;
                _writer.WriteCsvLine("Row 1", "Row A,A\rA", "A", "A1");
                _writer.WriteCsvLine("Row 2", "Row B,B\nB", "B", "B2");
                _writer.CRLFMode = CRLFMode.LF;
                _writer.WriteCsvLine("Row 3", "Row C;C", "C", "C3");
                _writer.WriteCsvLine("Row 4", "Row D;D", "D", "D4");
                _writer.CRLFMode = CRLFMode.CR;
                _writer.WriteCsvLine("Row 5", "Row E,E", "E", "E5");
                _writer.WriteCsvLine("Row 6", "Row F,F\r\nF,F", "F", "F6");

                _writer.WriteCsvLine("Row F\";\"F\r\nF,F\"", "Row 7", "\"", "F6\" ");
                _writer.WriteCsvLine("A\" ", "\"\"\"", "B\"", "\"\"\"");

                _writer.Flush();
            }

            string _text = File.ReadAllText(file);

            using (CsvStreamReader _reader = new CsvStreamReader(file))
            {
                _reader.Separator = ',';
                //_reader.DetectSeparator();

                int _row = 0;
                while (!_reader.EndOfStream)
                {
                    string[] _values = _reader.ReadCsvLine();

                    if (_row == 0)
                    {
                        Assert.AreEqual(_values[0], "Row 1");
                        Assert.AreEqual(_values[1], "Row A,A\rA");
                        Assert.AreEqual(_values[2], "A");
                        Assert.AreEqual(_values[3], "A1");
                    }
                    if (_row == 1)
                    {
                        Assert.AreEqual(_values[0], "Row 2");
                        Assert.AreEqual(_values[1], "Row B,B\nB");
                        Assert.AreEqual(_values[2], "B");
                        Assert.AreEqual(_values[3], "B2");
                    }
                    if (_row == 2)
                    {
                        Assert.AreEqual(_values[0], "Row 3");
                        Assert.AreEqual(_values[1], "Row C;C");
                        Assert.AreEqual(_values[2], "C");
                        Assert.AreEqual(_values[3], "C3");
                    }
                    if (_row == 3)
                    {
                        Assert.AreEqual(_values[0], "Row 4");
                        Assert.AreEqual(_values[1], "Row D;D");
                        Assert.AreEqual(_values[2], "D");
                        Assert.AreEqual(_values[3], "D4");
                    }
                    if (_row == 4)
                    {
                        Assert.AreEqual(_values[0], "Row 5");
                        Assert.AreEqual(_values[1], "Row E,E");
                        Assert.AreEqual(_values[2], "E");
                        Assert.AreEqual(_values[3], "E5");
                    }
                    if (_row == 5)
                    {
                        Assert.AreEqual(_values[0], "Row 6");
                        Assert.AreEqual(_values[1], "Row F,F\r\nF,F");
                        Assert.AreEqual(_values[2], "F");
                        Assert.AreEqual(_values[3], "F6");
                    }
                    if (_row == 6)
                    {
                        //_writer.WriteCsvLine("Row F\";\"F\r\nF,F\"", "Row 7", "\"", "F6\" ");


                        Assert.AreEqual(_values[0], "Row F\";\"F\r\nF,F\"");
                        Assert.AreEqual(_values[1], "Row 7");
                        Assert.AreEqual(_values[2], "\"");
                        Assert.AreEqual(_values[3], "F6\" ");
                    }
                    if (_row == 7)
                    {
                        Assert.AreEqual(_values[0], "A\" ");
                        Assert.AreEqual(_values[1], "\"\"\"");
                        Assert.AreEqual(_values[2], "B\"");
                        Assert.AreEqual(_values[3], "\"\"\"");
                    }
                    _row++;
                }
            }
        }


        private const char _CR = '\r';
        private const char _LF = '\n';
        private const string _CRLF = "\r\n";

        /// <summary>
        /// This TestMethod tests the CsvStreamReader and the CsvStreamWriter
        /// 1. Testing varying cases of line breaks.
        /// 2. Testing Escaping ',' '\r' '\n'
        /// 3. Testing DetectSeparator()
        /// 4. Testing Escaping of Alternate Separators
        /// </summary>
        [TestMethod]
        public void ValidateCRLFCsvSerializer()
        {
            StringBuilder _sb = new StringBuilder();

            _sb.AppendCsvValues(',', "Row 1", "Row A,A\rA", "A", "A1").Append(_CRLF);                
            _sb.AppendCsvValues(',', "Row 2", "Row B,B\nB", "B", "B2").Append(_CRLF);
            _sb.AppendCsvValues(',', "Row 3", "Row C;C", "C", "C3").Append(_LF);
            _sb.AppendCsvValues(',', "Row 4", "Row D;D", "D", "D4").Append(_LF);
            _sb.AppendCsvValues(',', "Row 5", "Row E,E", "E", "E5").Append(_CR);
            _sb.AppendCsvValues(',', "Row 6", "Row F,F\r\nF,F", "F", "F6").Append(_CR);
            _sb.AppendCsvValues(',', "Row F\";\"F\r\nF,F\"", "Row 7", "\"", "F6\" ").Append(_CR);
            _sb.AppendCsvValues(',', "A\" ", "\"\"\"", "B\"", "\"\"\"").Append(_CR);

            string _text = _sb.ToString();

            CsvSerializer serializer = new CsvSerializer(new CsvSerializerSettings() { Separator = ',' });

            List<string[]> _data = serializer.Deserialize(_text).ToList();

            Assert.AreEqual(_data[0][0], "Row 1");
            Assert.AreEqual(_data[0][1], "Row A,A\rA");
            Assert.AreEqual(_data[0][2], "A");
            Assert.AreEqual(_data[0][3], "A1");

            Assert.AreEqual(_data[1][0], "Row 2");
            Assert.AreEqual(_data[1][1], "Row B,B\nB");
            Assert.AreEqual(_data[1][2], "B");
            Assert.AreEqual(_data[1][3], "B2");

            Assert.AreEqual(_data[2][0], "Row 3");
            Assert.AreEqual(_data[2][1], "Row C;C");
            Assert.AreEqual(_data[2][2], "C");
            Assert.AreEqual(_data[2][3], "C3");

            Assert.AreEqual(_data[3][0], "Row 4");
            Assert.AreEqual(_data[3][1], "Row D;D");
            Assert.AreEqual(_data[3][2], "D");
            Assert.AreEqual(_data[3][3], "D4");

            Assert.AreEqual(_data[4][0], "Row 5");
            Assert.AreEqual(_data[4][1], "Row E,E");
            Assert.AreEqual(_data[4][2], "E");
            Assert.AreEqual(_data[4][3], "E5");

            Assert.AreEqual(_data[5][0], "Row 6");
            Assert.AreEqual(_data[5][1], "Row F,F\r\nF,F");
            Assert.AreEqual(_data[5][2], "F");
            Assert.AreEqual(_data[5][3], "F6");

            Assert.AreEqual(_data[6][0], "Row F\";\"F\r\nF,F\"");
            Assert.AreEqual(_data[6][1], "Row 7");
            Assert.AreEqual(_data[6][2], "\"");
            Assert.AreEqual(_data[6][3], "F6\" ");

            Assert.AreEqual(_data[7][0], "A\" ");
            Assert.AreEqual(_data[7][1], "\"\"\"");
            Assert.AreEqual(_data[7][2], "B\"");
            Assert.AreEqual(_data[7][3], "\"\"\"");
        }


        [TestMethod]
        public void TestIsCsv()
        {
            StringBuilder _sb = new StringBuilder();
            _sb.AppendCsvLine(',', "1", "2", "3");
            _sb.AppendCsvLine(',', "4", "5", "6");

            bool _isCsv = CsvUtils.IsCsv(_sb, ',');


            string _file = System.IO.Path.GetTempFileName();
            File.WriteAllText(_file, _sb.ToString());


            using (CsvStreamReader _reader = new CsvStreamReader(_file))
            {
                _isCsv = CsvUtils.IsCsv(_reader, ',');
            }


        }


        [TestMethod]
        public void TestColIndexCsvSerializer()
        {
            // Test using another class for reading without using all indexes.
            CsvSerializer serializer = new CsvSerializer();

            List<CsvSimple> _fullType = CsvSimpleData(5).ToList();

            string _text = serializer.SerializeObject<CsvSimple>(_fullType);

            List<CsvSimpleSmall> _smallType = serializer.DeserializeObject<CsvSimpleSmall>(_text).ToList();

            for (int ii = 0; ii < 5; ii++)
            {
                Assert.AreEqual(_smallType[ii].AfBij, _fullType[ii].AfBij);
                Assert.AreEqual(_smallType[ii].Tegenrekening, _fullType[ii].Tegenrekening);
                Assert.AreEqual(_smallType[ii].Rekening, _fullType[ii].Rekening);
            }
        }

        [TestMethod]
        public void CustomParseTest()
        {
            string _file = System.IO.Path.GetTempFileName();

            using (var _writer = new CsvWriter<CsvAllTypes>(_file))
            {
                _writer.WriteHeader();
                _writer.Culture = CultureInfo.GetCultureInfo("en-us");

                var _x = new CsvAllTypes()
                {
                    _BigIntegerValue = 1,
                    _BigIntegerValueNull = null,
                    _DateTimeValue = new DateTime(2025, 10,10, 12, 44, 23),
                    _BooleanValue = true,
                    _BooleanValueNull = null,
                    _ByteValue = 2,
                    _ByteValueNull = null,
                    _DateTimeOffsetValueNull = null,
                    _DateTimeOffsetValue = new DateTime(2025, 10, 10, 12, 44, 23),
                    _DateTimeValueNull = null,
                    _DecimalValue = 5,
                    _DecimalValueNull = null,
                    _DoubleValue = 6.8,
                    _DoubleValueNull = null,
                    _GuidValue = Guid.Parse("68f92586-2306-4f77-b5bc-e412ac017504"),
                    _GuidValueNull = null,
                    _Int16Value = 16,
                    _Int16ValueNull = null,
                    _Int32Value = 17,
                    _Int32ValueNull = null,
                    _Int64Value = 18,
                    _Int64ValueNull = null,
                    _SByteValue = 19,
                    _SByteValueNull = null,
                    _SingleValue = 20,
                    _SingleValueNull = null,
                    _stringValue = "Test",
                    _TimeSpanValue = TimeSpan.MaxValue,
                    _TimeSpanValueNull = null,
                    _UInt16Value = 21,
                    _UInt16ValueNull = null,
                    _UInt32Value = 22,
                    _UInt32ValueNull = null,
                    _UInt64Value = 23,
                    _UInt64ValueNull = null,
                    _Enum = Models.TestEnum.Something,
                    _LowerCaseParserReading = "ABC123",
                };

                _writer.Write(_x);

                var _x2 = new CsvAllTypes()
                {
                    _BigIntegerValue = 1,
                    _BigIntegerValueNull = 1,
                    _DateTimeValue = new DateTime(2025, 10, 10, 12, 44, 23),
                    _BooleanValue = true,
                    _BooleanValueNull = false,
                    _ByteValue = 2,
                    _ByteValueNull = 3,
                    _DateTimeOffsetValueNull = new DateTime(2025, 10, 10, 12, 44, 23),
                    _DateTimeOffsetValue = new DateTime(2025, 10, 10, 12, 44, 23),
                    _DateTimeValueNull = new DateTime(2025, 10, 10, 12, 44, 23),
                    _DecimalValue = 5.65M,
                    _DecimalValueNull = 4.23M,
                    _DoubleValue = 6.8,
                    _DoubleValueNull = 10.89,
                    _GuidValue = Guid.Parse("68f92586-2306-4f77-b5bc-e412ac017505"),
                    _GuidValueNull = Guid.Parse("68f92586-2306-4f77-b5bc-e412ac017506"),
                    _Int16Value = 16,
                    _Int16ValueNull = 26,
                    _Int32Value = 17,
                    _Int32ValueNull = 27,
                    _Int64Value = 18,
                    _Int64ValueNull = 28,
                    _SByteValue = 19,
                    _SByteValueNull = 29,
                    _SingleValue = 20,
                    _SingleValueNull = 30,
                    _stringValue = "Test2",
                    _TimeSpanValue = TimeSpan.MaxValue,
                    _TimeSpanValueNull = TimeSpan.MinValue,
                    _UInt16Value = 21,
                    _UInt16ValueNull = 41,
                    _UInt32Value = 22,
                    _UInt32ValueNull = 42,
                    _UInt64Value = 23,
                    _UInt64ValueNull = 43,
                    _Enum = Models.TestEnum.None,
                    _LowerCaseParserReading = "aBc123"
                };

                _writer.Write(_x2);

                _writer.Flush();

            }


            string _text = File.ReadAllText(_file);


            CsvAllTypes[] _rows;

            using (var _reader = new CsvReader<CsvAllTypes>(_file))
            {
                _reader.Skip(); // Slip header.
                _rows = _reader.ReadAsEnumerable().ToArray(); // Materialize.

                Assert.AreEqual(_rows[0]._BigIntegerValue, 1);
                Assert.AreEqual(_rows[0]._BigIntegerValueNull, null);
                Assert.AreEqual(_rows[0]._DateTimeValue, new DateTime(2025, 10, 10, 12, 44, 23));
                Assert.AreEqual(_rows[0]._BooleanValue, true);
                Assert.AreEqual(_rows[0]._BooleanValueNull, null);
                Assert.AreEqual(_rows[0]._ByteValue, 2);
                Assert.AreEqual(_rows[0]._ByteValueNull, null);
                Assert.AreEqual(_rows[0]._DateTimeOffsetValueNull, null);
                Assert.AreEqual(_rows[0]._DateTimeOffsetValue, new DateTime(2025, 10, 10, 12, 44, 23));
                Assert.AreEqual(_rows[0]._DateTimeValueNull, null);
                Assert.AreEqual(_rows[0]._DecimalValue, 5);
                Assert.AreEqual(_rows[0]._DecimalValueNull, null);
                Assert.AreEqual(_rows[0]._DoubleValue, 6.8);
                Assert.AreEqual(_rows[0]._DoubleValueNull, null);
                Assert.AreEqual(_rows[0]._GuidValue, Guid.Parse("68f92586-2306-4f77-b5bc-e412ac017504"));
                Assert.AreEqual(_rows[0]._GuidValueNull, null);
                Assert.AreEqual(_rows[0]._Int16Value, 16);
                Assert.AreEqual(_rows[0]._Int16ValueNull, null);
                Assert.AreEqual(_rows[0]._Int32Value, 17);
                Assert.AreEqual(_rows[0]._Int32ValueNull, null);
                Assert.AreEqual(_rows[0]._Int64Value, 18);
                Assert.AreEqual(_rows[0]._Int64ValueNull, null);
                Assert.AreEqual(_rows[0]._SByteValue, 19);
                Assert.AreEqual(_rows[0]._SByteValueNull, null);
                Assert.AreEqual(_rows[0]._SingleValue, 20);
                Assert.AreEqual(_rows[0]._SingleValueNull, null);
                Assert.AreEqual(_rows[0]._stringValue, "Test");
                Assert.AreEqual(_rows[0]._TimeSpanValue, TimeSpan.MaxValue);
                Assert.AreEqual(_rows[0]._TimeSpanValueNull, null);
                Assert.AreEqual(_rows[0]._UInt16Value, (uint)21);
                Assert.AreEqual(_rows[0]._UInt16ValueNull, null);
                Assert.AreEqual(_rows[0]._UInt32Value, (uint)22);
                Assert.AreEqual(_rows[0]._UInt32ValueNull, null);
                Assert.AreEqual(_rows[0]._UInt64Value, (uint)23);
                Assert.AreEqual(_rows[0]._UInt64ValueNull, null);
                Assert.AreEqual(_rows[0]._Enum, Models.TestEnum.Something);
                Assert.AreEqual(_rows[0]._LowerCaseParserReading, "abc123");


                Assert.AreEqual(_rows[1]._BigIntegerValue, 1);
                Assert.AreEqual(_rows[1]._BigIntegerValueNull, 1);
                Assert.AreEqual(_rows[1]._DateTimeValue, new DateTime(2025, 10, 10, 12, 44, 23));
                Assert.AreEqual(_rows[1]._BooleanValue, true);
                Assert.AreEqual(_rows[1]._BooleanValueNull, false);
                Assert.AreEqual(_rows[1]._ByteValue, 2);
                Assert.AreEqual(_rows[1]._ByteValueNull, (byte?)3);
                Assert.AreEqual(_rows[1]._DateTimeOffsetValueNull, new DateTime(2025, 10, 10, 12, 44, 23));
                Assert.AreEqual(_rows[1]._DateTimeOffsetValue, new DateTime(2025, 10, 10, 12, 44, 23));
                Assert.AreEqual(_rows[1]._DateTimeValueNull, new DateTime(2025, 10, 10, 12, 44, 23));
                Assert.AreEqual(_rows[1]._DecimalValue, 5.65M);
                Assert.AreEqual(_rows[1]._DecimalValueNull, 4.23M);
                Assert.AreEqual(_rows[1]._DoubleValue, 6.8);
                Assert.AreEqual(_rows[1]._DoubleValueNull, 10.89);
                Assert.AreEqual(_rows[1]._GuidValue, Guid.Parse("68f92586-2306-4f77-b5bc-e412ac017505"));
                Assert.AreEqual(_rows[1]._GuidValueNull, Guid.Parse("68f92586-2306-4f77-b5bc-e412ac017506"));
                Assert.AreEqual(_rows[1]._Int16Value, 16);
                Assert.AreEqual(_rows[1]._Int16ValueNull, (Int16?)26);
                Assert.AreEqual(_rows[1]._Int32Value, 17);
                Assert.AreEqual(_rows[1]._Int32ValueNull, 27);
                Assert.AreEqual(_rows[1]._Int64Value, 18);
                Assert.AreEqual(_rows[1]._Int64ValueNull, 28);
                Assert.AreEqual(_rows[1]._SByteValue, 19);
                Assert.AreEqual(_rows[1]._SByteValueNull, (SByte?)29);
                Assert.AreEqual(_rows[1]._SingleValue, 20);
                Assert.AreEqual(_rows[1]._SingleValueNull, 30);
                Assert.AreEqual(_rows[1]._stringValue, "Test2");
                Assert.AreEqual(_rows[1]._TimeSpanValue, TimeSpan.MaxValue);
                Assert.AreEqual(_rows[1]._TimeSpanValueNull, TimeSpan.MinValue);
                Assert.AreEqual(_rows[1]._UInt16Value, 21);
                Assert.AreEqual(_rows[1]._UInt16ValueNull, (UInt16?)41);
                Assert.AreEqual(_rows[1]._UInt32Value, (UInt32)22);
                Assert.AreEqual(_rows[1]._UInt32ValueNull, (UInt32?)42);
                Assert.AreEqual(_rows[1]._UInt64Value, (UInt64)23);
                Assert.AreEqual(_rows[1]._UInt64ValueNull, (UInt64?)43);
                Assert.AreEqual(_rows[1]._Enum, Models.TestEnum.None);
                Assert.AreEqual(_rows[1]._LowerCaseParserReading, "abc123");
            }
        }


        /// <summary>
        /// This TestMethod tests the CsvWriter<T> and CsvReader<T>
        /// 1. Test DetectSeparator
        /// 2. Test all supported property types
        /// 3. Test all supported property types as nullable
        /// 4. Test Skip() method
        /// 5. Test Empty line in csv
        /// 6. Test MoveToStart()
        /// 7. Test Read Header into object (should not crash).
        /// 8. Test Last() method.
        /// 9. Test varying cases of line breaks.
        /// 10. Test ReadAsEnumerable()
        /// 11. Test Reader.CurrentLine
        /// </summary>
        [TestMethod]
        public void TestTypeFields()
        {
            string file = System.IO.Path.GetTempFileName();

            using (CsvWriter<CsvTypesData> _writer = new(file) { Separator = ',' })
            {
                //_writer.Culture = CultureInfo.GetCultureInfo("en-US");
                _writer.NullValueBehaviour = WriteNullValueBehaviour.EmptyLine;
                _writer.CRLFMode = CRLFMode.CRLF;
                _writer.WriteHeader();
                _writer.CRLFMode = CRLFMode.LF;
                _writer.Write(ROW1);
                _writer.Write((CsvTypesData)null); // Write empty row. (Mode is EmptyLine)
                _writer.CRLFMode = CRLFMode.CR;
                _writer.Write((CsvTypesData)null); // Write empty row. (Mode is EmptyLine)
                _writer.Write((CsvTypesData)null); // Write empty row. (Mode is EmptyLine)
                _writer.CRLFMode = CRLFMode.CRLF;
                _writer.Write(ROW2);
            }

            string _text = File.ReadAllText(file);

            using (CsvReader<CsvTypesData> _reader = new(file) { Separator = ',' })
            {
                //_reader.Culture = CultureInfo.GetCultureInfo("en-US");

                _reader.DetectSeparator();
                _reader.EmptyLineBehaviour = EmptyLineBehaviour.NullValue;

                Assert.AreEqual(0, _reader.CurrentLine);

                _reader.Skip();

                Assert.AreEqual(1, _reader.CurrentLine);

                CsvTypesData _row = _reader.Read();
                Assert.AreEqual(2, _reader.CurrentLine);

                Assert.AreEqual("Line 1", _row.StringValue);
                Assert.AreEqual(Guid.Parse("94a6bfe9-abb4-46b6-bd7d-8f047b9ba480"), _row.GuidValue);
                Assert.AreEqual(Guid.Parse("94a6bfe9-abb4-46b6-bd7d-8f047b9ba480"), _row.GuidValueNullable);
                Assert.AreEqual(true, _row.BooleanValue);
                Assert.AreEqual(false, _row.BooleanValueNullable);
                Assert.AreEqual(new DateTime(2023, 3, 31), _row.DateTimeValue);
                Assert.AreEqual(new DateTime(2023, 3, 31), _row.DateTimeOffsetValueNullable);
                Assert.AreEqual(new DateTime(2023, 3, 31), _row.DateTimeOffsetValue);
                Assert.AreEqual(new DateTime(2023, 3, 31), _row.DateTimeValueNullable);
                Assert.AreEqual(new TimeSpan(1, 2, 3, 4, 5), _row.TimeSpanValue);
                Assert.AreEqual(new TimeSpan(6, 7, 8, 9, 10), _row.TimeSpanValueNullable);
                Assert.AreEqual((byte)3, _row.ByteValue);
                Assert.AreEqual((byte?)4, _row.ByteValueNullable);
                Assert.AreEqual((sbyte)5, _row.SByteValue);
                Assert.AreEqual((sbyte?)6, _row.SByteValueNullable);
                Assert.AreEqual((short)8, _row.Int16Value);
                Assert.AreEqual((short?)9, _row.Int16ValueNullable);
                Assert.AreEqual(10, _row.Int32Value);
                Assert.AreEqual(11, _row.Int32ValueNullable);
                Assert.AreEqual((long)12, _row.Int64Value);
                Assert.AreEqual((long?)13, _row.Int64ValueNullable);
                Assert.AreEqual((UInt16)22, _row.UInt16Value);
                Assert.AreEqual((UInt16?)23, _row.UInt16ValueNullable);
                Assert.AreEqual((UInt32)24, _row.UInt32Value);
                Assert.AreEqual((UInt32?)25, _row.UInt32ValueNullable);
                Assert.AreEqual((UInt64)26, _row.UInt64Value);
                Assert.AreEqual((UInt64?)29, _row.UInt64ValueNullable);
                Assert.AreEqual(TestEnum.XXX, _row.TestEnumItem);


                _row = _reader.Read();
                Assert.AreEqual(null, _row);
                Assert.AreEqual(3, _reader.CurrentLine);

                // Skip 2 more empty rows.
                _reader.Skip(2);
                Assert.AreEqual(5, _reader.CurrentLine);

                _row = _reader.Read();
                Assert.AreEqual("Line 2", _row.StringValue);
                Assert.AreEqual(Guid.Parse("90a6bfe9-abb4-46b6-bd7d-8f047b9ba480"), _row.GuidValue);
                Assert.AreEqual(null, _row.GuidValueNullable);
                Assert.AreEqual(true, _row.BooleanValue);
                Assert.AreEqual(null, _row.BooleanValueNullable);
                Assert.AreEqual(new DateTime(2023, 3, 31), _row.DateTimeValue);
                Assert.AreEqual(null, _row.DateTimeOffsetValueNullable);
                Assert.AreEqual(new DateTime(2023, 3, 31), _row.DateTimeOffsetValue);
                Assert.AreEqual(null, _row.DateTimeValueNullable);
                Assert.AreEqual(new TimeSpan(1, 2, 3, 4, 5), _row.TimeSpanValue);
                Assert.AreEqual(null, _row.TimeSpanValueNullable);
                Assert.AreEqual((byte)3, _row.ByteValue);
                Assert.AreEqual(null, _row.ByteValueNullable);
                Assert.AreEqual((sbyte)5, _row.SByteValue);
                Assert.AreEqual(null, _row.SByteValueNullable);
                Assert.AreEqual((short)8, _row.Int16Value);
                Assert.AreEqual(null, _row.Int16ValueNullable);
                Assert.AreEqual(10, _row.Int32Value);
                Assert.AreEqual(null, _row.Int32ValueNullable);
                Assert.AreEqual((long)12, _row.Int64Value);
                Assert.AreEqual(null, _row.Int64ValueNullable);
                Assert.AreEqual((UInt16)22, _row.UInt16Value);
                Assert.AreEqual(null, _row.UInt16ValueNullable);
                Assert.AreEqual((UInt32)24, _row.UInt32Value);
                Assert.AreEqual(null, _row.UInt32ValueNullable);
                Assert.AreEqual((UInt64)26, _row.UInt64Value);
                Assert.AreEqual(null, _row.UInt64ValueNullable);
                Assert.AreEqual(TestEnum.YYY, _row.TestEnumItem);


                _reader.MoveToStart();

                // Reading the header into object should not crash. regardless all type conversion.
                _row = _reader.Read();
                Assert.AreEqual("StringValue", _row.StringValue);

                _reader.Skip();

                // Reading Empty row
                _reader.EmptyLineBehaviour = EmptyLineBehaviour.DefaultInstance;
                _row = _reader.Read();
                Assert.AreEqual(Guid.Empty, _row.GuidValue); // Testing empty

                _reader.EmptyLineBehaviour = EmptyLineBehaviour.NullValue;

                _reader.MoveToStart();
                var _all = _reader.ReadAsEnumerable().ToList();


                _reader.MoveToStart();
                _reader.EmptyLineBehaviour = EmptyLineBehaviour.NullValue;


                _reader.MoveToStart();

                _reader.Skip();
                List<CsvTypesData> _data = _reader.ReadAsEnumerable().Where(p => p != null).ToList();
                Assert.AreEqual("Line 1", _data[0].StringValue);
                Assert.AreEqual("Line 2", _data[1].StringValue);

//                string _testData = @"StringValue,GuidValue,BooleanValue,DateTimeValue,DateTimeOffsetValue,TimeSpanValue,ByteValue,SByteValue,Int16Value,Int32Value,Int64Value,SingleValue,DecimalValue,DoubleValue,UInt16Value,UInt32Value,UInt64Value,GuidValueNullable,BooleanValueNullable,DateTimeValueNullable,DateTimeOffsetValueNullable,TimeSpanValueNullable,ByteValueNullable,SByteValueNullable,Int16ValueNullable,Int32ValueNullable,Int64ValueNullable,SingleValueNullable,DecimalValueNullable,DoubleValueNullable,UInt16ValueNullable,UInt32ValueNullable,UInt64ValueNullable,ByteArray
//Line 1,Guid Value?,Is this yes?,31/31/2023 00:00:00,31/03/2023 00:00:00 +02:00,1.02:03:04.0050000,3,5,8,10,12,15,""10,00"",""10,5"",22,24,26,94a6bfe9-abb4-46b6-bd7d-8f047b9ba480,False,31/03/2023 00:00:00,31/03/2023 00:00:00 +02:00,6.07:08:09.0100000,4,6,9,11,13,16,""11,23"",""10,45"",23,25,29,AQID

//Line 2,94a6bfe9-abb4-46b6-bd7d-8f047b9ba480,True,31/03/2023 00:00:00,31/03/2023 00:00:00 +02:00,Wrong Time,3,5,8,10,12,15,""10,00"",""10,5"",22,24,26,,,,,,,,,,,,,,,,,";

//                List<CsvTypesData> _result = new List<CsvTypesData>();

//                byte[] byteArray = Encoding.Default.GetBytes(_testData);

//                using (MemoryStream _stream = new MemoryStream(byteArray))
//                {
//                    using (CsvReader<CsvTypesData> _csvReader = new CsvReader<CsvTypesData>(_stream))
//                    {
//                        _csvReader.EmptyLineBehaviour = EmptyLineBehaviour.NullValue;
//                        _csvReader.Open();
//                        _csvReader.DetectSeparator();
//                        _csvReader.Skip();
//                        _result = _csvReader.ReadAsEnumerable().ToList();

//                        Assert.AreEqual(4, _csvReader.Errors.Count());
//                    }
//                }
            }
        }



        /// <summary>
        /// This TestMethod tests the CsvWriter<T> and CsvReader<T>
        /// 1. Test DetectSeparator
        /// 2. Test all supported property types
        /// 3. Test all supported property types as nullable
        /// 4. Test Skip() method
        /// 5. Test Empty line in csv
        /// 6. Test MoveToStart()
        /// 7. Test Read Header into object (should not crash).
        /// 8. Test Last() method.
        /// 9. Test varying cases of line breaks.
        /// 10. Test ReadAsEnumerable()
        /// 11. Test Reader.CurrentLine
        /// </summary>
        [TestMethod]
        public void TestTypeFieldsCsvSerializer()
        {
            CsvSerializer csvSerializer = new CsvSerializer(new CsvSerializerSettings() { Separator = ','});
            StringBuilder sb = new StringBuilder();


            csvSerializer.SerializeObject<CsvTypesData>(sb, new List<CsvTypesData>() { ROW1, ROW2 });

            string _text = sb.ToString();

            List<CsvTypesData> _data = csvSerializer.DeserializeObject<CsvTypesData>(_text).ToList();

            Assert.AreEqual("Line 1", _data[0].StringValue);
            Assert.AreEqual(Guid.Parse("94a6bfe9-abb4-46b6-bd7d-8f047b9ba480"), _data[0].GuidValue);
            Assert.AreEqual(Guid.Parse("94a6bfe9-abb4-46b6-bd7d-8f047b9ba480"), _data[0].GuidValueNullable);
            Assert.AreEqual(true, _data[0].BooleanValue);
            Assert.AreEqual(false, _data[0].BooleanValueNullable);
            Assert.AreEqual(new DateTime(2023, 3, 31), _data[0].DateTimeValue);
            Assert.AreEqual(new DateTime(2023, 3, 31), _data[0].DateTimeOffsetValueNullable);
            Assert.AreEqual(new DateTime(2023, 3, 31), _data[0].DateTimeOffsetValue);
            Assert.AreEqual(new DateTime(2023, 3, 31), _data[0].DateTimeValueNullable);
            Assert.AreEqual(new TimeSpan(1, 2, 3, 4, 5), _data[0].TimeSpanValue);
            Assert.AreEqual(new TimeSpan(6, 7, 8, 9, 10), _data[0].TimeSpanValueNullable);
            Assert.AreEqual((byte)3, _data[0].ByteValue);
            Assert.AreEqual((byte?)4, _data[0].ByteValueNullable);
            Assert.AreEqual((sbyte)5, _data[0].SByteValue);
            Assert.AreEqual((sbyte?)6, _data[0].SByteValueNullable);
            Assert.AreEqual((short)8, _data[0].Int16Value);
            Assert.AreEqual((short?)9, _data[0].Int16ValueNullable);
            Assert.AreEqual(10, _data[0].Int32Value);
            Assert.AreEqual(11, _data[0].Int32ValueNullable);
            Assert.AreEqual((long)12, _data[0].Int64Value);
            Assert.AreEqual((long?)13, _data[0].Int64ValueNullable);
            Assert.AreEqual((UInt16)22, _data[0].UInt16Value);
            Assert.AreEqual((UInt16?)23, _data[0].UInt16ValueNullable);
            Assert.AreEqual((UInt32)24, _data[0].UInt32Value);
            Assert.AreEqual((UInt32?)25, _data[0].UInt32ValueNullable);
            Assert.AreEqual((UInt64)26, _data[0].UInt64Value);
            Assert.AreEqual((UInt64?)29, _data[0].UInt64ValueNullable);
            Assert.AreEqual(TestEnum.XXX, _data[0].TestEnumItem);




            Assert.AreEqual("Line 2", _data[1].StringValue);
            Assert.AreEqual(Guid.Parse("90a6bfe9-abb4-46b6-bd7d-8f047b9ba480"), _data[1].GuidValue);
            Assert.AreEqual(null, _data[1].GuidValueNullable);
            Assert.AreEqual(true, _data[1].BooleanValue);
            Assert.AreEqual(null, _data[1].BooleanValueNullable);
            Assert.AreEqual(new DateTime(2023, 3, 31), _data[1].DateTimeValue);
            Assert.AreEqual(null, _data[1].DateTimeOffsetValueNullable);
            Assert.AreEqual(new DateTime(2023, 3, 31), _data[1].DateTimeOffsetValue);
            Assert.AreEqual(null, _data[1].DateTimeValueNullable);
            Assert.AreEqual(new TimeSpan(1, 2, 3, 4, 5), _data[1].TimeSpanValue);
            Assert.AreEqual(null, _data[1].TimeSpanValueNullable);
            Assert.AreEqual((byte)3, _data[1].ByteValue);
            Assert.AreEqual(null, _data[1].ByteValueNullable);
            Assert.AreEqual((sbyte)5, _data[1].SByteValue);
            Assert.AreEqual(null, _data[1].SByteValueNullable);
            Assert.AreEqual((short)8, _data[1].Int16Value);
            Assert.AreEqual(null, _data[1].Int16ValueNullable);
            Assert.AreEqual(10, _data[1].Int32Value);
            Assert.AreEqual(null, _data[1].Int32ValueNullable);
            Assert.AreEqual((long)12, _data[1].Int64Value);
            Assert.AreEqual(null, _data[1].Int64ValueNullable);
            Assert.AreEqual((UInt16)22, _data[1].UInt16Value);
            Assert.AreEqual(null, _data[1].UInt16ValueNullable);
            Assert.AreEqual((UInt32)24, _data[1].UInt32Value);
            Assert.AreEqual(null, _data[1].UInt32ValueNullable);
            Assert.AreEqual((UInt64)26, _data[1].UInt64Value);
            Assert.AreEqual(null, _data[1].UInt64ValueNullable);
            Assert.AreEqual(TestEnum.YYY, _data[1].TestEnumItem);

        }








        #region Data

        private CsvTypesData ROW1 = new()
        {
            

            StringValue = $"Line 1",
            GuidValue = Guid.Parse("94a6bfe9-abb4-46b6-bd7d-8f047b9ba480"),
            GuidValueNullable = Guid.Parse("94a6bfe9-abb4-46b6-bd7d-8f047b9ba480"),
            BooleanValue = true,
            BooleanValueNullable = false,
            DateTimeValue = new DateTime(2023, 3, 31),
            DateTimeOffsetValueNullable = new DateTime(2023, 3, 31),
            DateTimeOffsetValue = new DateTime(2023, 3, 31),
            DateTimeValueNullable = new DateTime(2023, 3, 31),
            TimeSpanValue = new TimeSpan(1, 2, 3, 4, 5),
            TimeSpanValueNullable = new TimeSpan(6, 7, 8, 9, 10),
            ByteValue = 3,
            ByteValueNullable = 4,
            SByteValue = 5,
            SByteValueNullable = 6,
            Int16Value = 8,
            Int16ValueNullable = 9,
            Int32Value = 10,
            Int32ValueNullable = 11,
            Int64Value = 12,
            Int64ValueNullable = 13,
            SingleValue = 15,
            SingleValueNullable = 16,
            DecimalValue = 10.00m,
            DecimalValueNullable = 11.23m,
            DoubleValue = 10.5,
            DoubleValueNullable = 10.45,
            UInt16Value = 22,
            UInt16ValueNullable = 23,
            UInt32Value = 24,
            UInt32ValueNullable = 25,
            UInt64Value = 26,
            UInt64ValueNullable = 29,
            ByteArray = new byte[] { 1, 2, 3 },
            TestEnumItem = TestEnum.XXX
        };

        private CsvTypesData ROW2 = new()
        {
            StringValue = $"Line 2",
            GuidValue = Guid.Parse("90a6bfe9-abb4-46b6-bd7d-8f047b9ba480"),
            GuidValueNullable = null,
            BooleanValue = true,
            BooleanValueNullable = null,
            DateTimeValue = new DateTime(2023, 3, 31),
            DateTimeOffsetValueNullable = null,
            DateTimeOffsetValue = new DateTime(2023, 3, 31),
            DateTimeValueNullable = null,
            TimeSpanValue = new TimeSpan(1, 2, 3, 4, 5),
            TimeSpanValueNullable = null,
            ByteValue = 3,
            ByteValueNullable = null,
            SByteValue = 5,
            SByteValueNullable = null,
            Int16Value = 8,
            Int16ValueNullable = null,
            Int32Value = 10,
            Int32ValueNullable = null,
            Int64Value = 12,
            Int64ValueNullable = null,
            SingleValue = 15,
            SingleValueNullable = null,
            DecimalValue = 10.00m,
            DecimalValueNullable = null,
            DoubleValue = 10.5,
            DoubleValueNullable = null,
            UInt16Value = 22,
            UInt16ValueNullable = null,
            UInt32Value = 24,
            UInt32ValueNullable = null,
            UInt64Value = 26,
            UInt64ValueNullable = null,
            ByteArray = null,
            TestEnumItem = TestEnum.YYY
        };



        private CsvSimpleSmall ROWA = new() { AfBij = "111", Rekening = "AA", Tegenrekening = "X" };
        private CsvSimpleSmall ROWB = new() { AfBij = "222", Rekening = "BB", Tegenrekening = "Y" };
        private CsvSimpleSmall ROWC = new() { AfBij = "333", Rekening = "CC", Tegenrekening = "Z" };

        private CsvSimpleSmall ROWD = new() { AfBij = "AAA1 \"\" ", Rekening = "BBB\r\n",  Tegenrekening = "AAA,BBB" };
        private CsvSimpleSmall ROWE = new() { AfBij = "AAA2 \"\"", Rekening = "BBB\r\n \"", Tegenrekening = "CCC,DDD" };
        private CsvSimpleSmall ROWF = new() { AfBij = "AAA3 \",\r\"", Rekening = "BBB\r,\n \"", Tegenrekening = "EEE,FFF" };
        #endregion Data

    }

    #region Classes

    public class CsvTypesData
    {
        [Column(Index = 0, Header = "StringValue")]
        public string StringValue { get; set; }

        [Column(Index = 1, Header = "GuidValue")]
        public Guid GuidValue { get; set; }

        [Column(Index = 2, Header = "BooleanValue")]
        public Boolean BooleanValue { get; set; }

        [Column(Index = 3, Header = "DateTimeValue")]
        public DateTime DateTimeValue { get; set; }

        [Column(Index = 4, Header = "DateTimeOffsetValue")]
        public DateTimeOffset DateTimeOffsetValue { get; set; }

        [Column(Index = 5, Header = "TimeSpanValue")]
        public TimeSpan TimeSpanValue { get; set; }

        [Column(Index = 6, Header = "ByteValue")]
        public Byte ByteValue { get; set; }

        [Column(Index = 7, Header = "SByteValue")]
        public SByte SByteValue { get; set; }

        [Column(Index = 8, Header = "Int16Value")]
        public Int16 Int16Value { get; set; }

        [Column(Index = 9, Header = "Int32Value")]
        public Int32 Int32Value { get; set; }

        [Column(Index = 10, Header = "Int64Value")]
        public Int64 Int64Value { get; set; }

        [Column(Index = 11, Header = "SingleValue")]
        public Single SingleValue { get; set; }

        [Column(Index = 12, Header = "DecimalValue")]
        public Decimal DecimalValue { get; set; }

        [Column(Index = 13, Header = "DoubleValue")]
        public Double DoubleValue { get; set; }

        [Column(Index = 14, Header = "UInt16Value")]
        public UInt16 UInt16Value { get; set; }

        [Column(Index = 15, Header = "UInt32Value")]
        public UInt32 UInt32Value { get; set; }

        [Column(Index = 16, Header = "UInt64Value")]
        public UInt64 UInt64Value { get; set; }

        [Column(Index = 17, Header = "GuidValueNullable")]
        public Guid? GuidValueNullable { get; set; }

        [Column(Index = 18, Header = "BooleanValueNullable")]
        public Boolean? BooleanValueNullable { get; set; }

        [Column(Index = 19, Header = "DateTimeValueNullable")]
        public DateTime? DateTimeValueNullable { get; set; }

        [Column(Index = 20, Header = "DateTimeOffsetValueNullable")]
        public DateTimeOffset? DateTimeOffsetValueNullable { get; set; }

        [Column(Index = 21, Header = "TimeSpanValueNullable")]
        public TimeSpan? TimeSpanValueNullable { get; set; }

        [Column(Index = 22, Header = "ByteValueNullable")]
        public Byte? ByteValueNullable { get; set; }

        [Column(Index = 23, Header = "SByteValueNullable")]
        public SByte? SByteValueNullable { get; set; }

        [Column(Index = 24, Header = "Int16ValueNullable")]
        public Int16? Int16ValueNullable { get; set; }

        [Column(Index = 25, Header = "Int32ValueNullable")]
        public Int32? Int32ValueNullable { get; set; }

        [Column(Index = 26, Header = "Int64ValueNullable")]
        public Int64? Int64ValueNullable { get; set; }

        [Column(Index = 27, Header = "SingleValueNullable")]
        public Single? SingleValueNullable { get; set; }

        [Column(Index = 28, Header = "DecimalValueNullable")]
        public Decimal? DecimalValueNullable { get; set; }

        [Column(Index = 29, Header = "DoubleValueNullable")]
        public Double? DoubleValueNullable { get; set; }

        [Column(Index = 30, Header = "UInt16ValueNullable")]
        public UInt16? UInt16ValueNullable { get; set; }

        [Column(Index = 31, Header = "UInt32ValueNullable")]
        public UInt32? UInt32ValueNullable { get; set; }

        [Column(Index = 32, Header = "UInt64ValueNullable")]
        public UInt64? UInt64ValueNullable { get; set; }

        [Column(Index = 33, Header = "ByteArray")]
        public byte[] ByteArray { get; set; }

        [Column(Index = 34, Header = "Enum")]
        public TestEnum TestEnumItem { get; set; }
    }


    public class CsvSimpleRow
    {
        [Column(Index = 0, Header = "Row 1")]
        public string Row1 { get; set; }

        [Column(Index = 5, Header = "Row 5")]
        public string Row5 { get; set; }

        [Column(Index = 10, Header = "Row 10")]
        public string Row10 { get; set; }
    }

    #endregion
}