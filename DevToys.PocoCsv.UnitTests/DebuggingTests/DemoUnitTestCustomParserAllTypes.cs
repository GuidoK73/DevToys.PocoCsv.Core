using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using DevToys.PocoCsv.UnitTests.Models;

namespace DevToys.PocoCsv.UnitTests
{

    [TestClass]
    public class DemoUnitTestCustomParserAllTypes
    {
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
                    _DateTimeValue = DateTime.Now,
                    _BooleanValue = true,
                    _BooleanValueNull = null,
                    _ByteValue = 2,
                    _ByteValueNull = null,
                    _DateTimeOffsetValueNull = null,
                    _DateTimeOffsetValue = DateTimeOffset.Now,
                    _DateTimeValueNull = null,
                    _DecimalValue = 5,
                    _DecimalValueNull = null,
                    _DoubleValue = 6.8,
                    _DoubleValueNull = null,
                    _GuidValue = Guid.NewGuid(),
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
                    _Enum = Models.TestEnum.Something
                };

                _writer.Write(_x);

                var _x2 = new CsvAllTypes()
                {
                    _BigIntegerValue = 1,
                    _BigIntegerValueNull = 1,
                    _DateTimeValue = DateTime.Now,
                    _BooleanValue = true,
                    _BooleanValueNull = false,
                    _ByteValue = 2,
                    _ByteValueNull = 3,
                    _DateTimeOffsetValueNull = DateTimeOffset.Now,
                    _DateTimeOffsetValue = DateTimeOffset.Now,
                    _DateTimeValueNull = DateTime.Now,
                    _DecimalValue = 5.65M,
                    _DecimalValueNull = 4.23M,
                    _DoubleValue = 6.8,
                    _DoubleValueNull = 10.89,
                    _GuidValue = Guid.NewGuid(),
                    _GuidValueNull = Guid.Empty,
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
                    _Enum = Models.TestEnum.None
                };

                _writer.Write(_x2);

                _writer.Flush();

            }


            string _text = File.ReadAllText(_file);



            using (var _reader = new CsvReader<CsvAllTypes>(_file))
            {
                _reader.Skip(); // Slip header.
                var _rows = _reader.ReadAsEnumerable().ToArray(); // Materialize.

                Console.Write("X");
            }
        }
    }
}