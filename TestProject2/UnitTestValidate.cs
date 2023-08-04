using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace TestProject2
{
    /// <summary>
    /// This tests are actual tests for maintaining correct behaviour.
    /// </summary>
    [TestClass]
    public class UnitTestValidate
    {
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
        };

        #endregion Data

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
                _writer.Open();
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
                _reader.Open();
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

                _reader.MoveToStart();

                // Reading the header into object should not crash. regardless all type conversion.
                _row = _reader.Read();
                Assert.AreEqual("StringValue", _row.StringValue);

                _reader.Skip();

                // Reading Empty row
                _reader.EmptyLineBehaviour = EmptyLineBehaviour.DefaultInstance;
                _row = _reader.Read();
                Assert.AreEqual(Guid.Empty, _row.GuidValue); // Testing empty

                _reader.MoveToStart();
                _reader.EmptyLineBehaviour = EmptyLineBehaviour.NullValue;

                // Testing Last() Method
                CsvTypesData[] _rows = _reader.Last(2).ToArray();
                Assert.AreEqual(null, _rows[0]);
                Assert.AreEqual("Line 2", _rows[1].StringValue);

                _reader.MoveToStart();

                _reader.Skip();
                List<CsvTypesData> _data = _reader.ReadAsEnumerable().Where(p => p != null).ToList();
                Assert.AreEqual("Line 1", _data[0].StringValue);
                Assert.AreEqual("Line 2", _data[1].StringValue);

                string _testData = @"StringValue,GuidValue,BooleanValue,DateTimeValue,DateTimeOffsetValue,TimeSpanValue,ByteValue,SByteValue,Int16Value,Int32Value,Int64Value,SingleValue,DecimalValue,DoubleValue,UInt16Value,UInt32Value,UInt64Value,GuidValueNullable,BooleanValueNullable,DateTimeValueNullable,DateTimeOffsetValueNullable,TimeSpanValueNullable,ByteValueNullable,SByteValueNullable,Int16ValueNullable,Int32ValueNullable,Int64ValueNullable,SingleValueNullable,DecimalValueNullable,DoubleValueNullable,UInt16ValueNullable,UInt32ValueNullable,UInt64ValueNullable,ByteArray
Line 1,Guid Value?,Is this yes?,31/31/2023 00:00:00,31/03/2023 00:00:00 +02:00,1.02:03:04.0050000,3,5,8,10,12,15,""10,00"",""10,5"",22,24,26,94a6bfe9-abb4-46b6-bd7d-8f047b9ba480,False,31/03/2023 00:00:00,31/03/2023 00:00:00 +02:00,6.07:08:09.0100000,4,6,9,11,13,16,""11,23"",""10,45"",23,25,29,AQID

Line 2,94a6bfe9-abb4-46b6-bd7d-8f047b9ba480,True,31/03/2023 00:00:00,31/03/2023 00:00:00 +02:00,Wrong Time,3,5,8,10,12,15,""10,00"",""10,5"",22,24,26,,,,,,,,,,,,,,,,,";

                List<CsvTypesData> _result = new List<CsvTypesData>();

                byte[] byteArray = Encoding.Default.GetBytes(_testData);

                using (MemoryStream _stream = new MemoryStream(byteArray))
                {
                    using (CsvReader<CsvTypesData> _csvReader = new CsvReader<CsvTypesData>(_stream))
                    {
                        _csvReader.EmptyLineBehaviour = EmptyLineBehaviour.NullValue;
                        _csvReader.Open();
                        _csvReader.DetectSeparator();
                        _csvReader.Skip();
                        _result = _csvReader.ReadAsEnumerable().ToList();

                        Assert.AreEqual(4, _csvReader.Errors.Count());
                    }
                }
            }
        }
    }

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
}