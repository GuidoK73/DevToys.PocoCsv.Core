using DevToys.PocoCsv.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TestProject2.Models
{

    [Csv( DefaultCustomParserTypeString = typeof(Parsestring))]
    public class CsvAllTypes
    {
        [Column(Index = 0, OutputFormat = "", OutputNullValue = "")]
        public string _stringValue { get; set; }

        [Column(Index = 35, OutputFormat = "", OutputNullValue = "")]
        public string _stringValue2 { get; set; }

        [Column(Index = 1, CustomParserType = typeof(ParseGuid), OutputFormat = "", OutputNullValue = "")]
        public Guid _GuidValue { get; set; }

        [Column(Index = 2, CustomParserType = typeof(ParseBoolean), OutputFormat = "", OutputNullValue = "")]
        public Boolean _BooleanValue { get; set; }

        [Column(Index = 3, CustomParserType = typeof(ParseDateTime), OutputFormat = "", OutputNullValue = "")]
        public DateTime _DateTimeValue { get; set; }

        [Column(Index = 4, CustomParserType = typeof(ParseDateTimeOffset), OutputFormat = "", OutputNullValue = "")]
        public DateTimeOffset _DateTimeOffsetValue { get; set; }

        [Column(Index = 5, CustomParserType = typeof(ParseTimeSpan), OutputFormat = "", OutputNullValue = "")]
        public TimeSpan _TimeSpanValue { get; set; }

        [Column(Index = 6, CustomParserType = typeof(ParseByte), OutputFormat = "", OutputNullValue = "")]
        public Byte _ByteValue { get; set; }

        [Column(Index = 7, CustomParserType = typeof(ParseSByte), OutputFormat = "", OutputNullValue = "")]
        public SByte _SByteValue { get; set; }

        [Column(Index = 8, CustomParserType = typeof(ParseInt16), OutputFormat = "", OutputNullValue = "")]
        public Int16 _Int16Value { get; set; }

        [Column(Index = 9, CustomParserType = typeof(ParseInt32), OutputFormat = "", OutputNullValue = "")]
        public Int32 _Int32Value { get; set; }

        [Column(Index = 10, CustomParserType = typeof(ParseInt64), OutputFormat = "", OutputNullValue = "")]
        public Int64 _Int64Value { get; set; }

        [Column(Index = 11, CustomParserType = typeof(ParseSingle), OutputFormat = "", OutputNullValue = "")]
        public Single _SingleValue { get; set; }

        [Column(Index = 12, CustomParserType = typeof(ParseDecimal), OutputFormat = "", OutputNullValue = "")]
        public Decimal _DecimalValue { get; set; }

        [Column(Index = 13, CustomParserType = typeof(ParseDouble), OutputFormat = "", OutputNullValue = "")]
        public Double _DoubleValue { get; set; }

        [Column(Index = 14, CustomParserType = typeof(ParseUInt16), OutputFormat = "", OutputNullValue = "")]
        public UInt16 _UInt16Value { get; set; }

        [Column(Index = 15, CustomParserType = typeof(ParseUInt32), OutputFormat = "", OutputNullValue = "")]
        public UInt32 _UInt32Value { get; set; }

        [Column(Index = 16, CustomParserType = typeof(ParseUInt64), OutputFormat = "", OutputNullValue = "")]
        public UInt64 _UInt64Value { get; set; }

        [Column(Index = 17, CustomParserType = typeof(ParseBigInteger), OutputFormat = "", OutputNullValue = "")]
        public BigInteger _BigIntegerValue { get; set; }

        [Column(Index = 18, CustomParserType = typeof(ParseGuidNull), OutputFormat = "", OutputNullValue = "")]
        public Guid? _GuidValueNull { get; set; }

        [Column(Index = 19, CustomParserType = typeof(ParseBooleanNull), OutputFormat = "", OutputNullValue = "")]
        public Boolean? _BooleanValueNull { get; set; }

        [Column(Index = 20, CustomParserType = typeof(ParseDateTimeNull), OutputFormat = "", OutputNullValue = "")]
        public DateTime? _DateTimeValueNull { get; set; }

        [Column(Index = 21, CustomParserType = typeof(ParseDateTimeOffsetNull), OutputFormat = "", OutputNullValue = "")]
        public DateTimeOffset? _DateTimeOffsetValueNull { get; set; }

        [Column(Index = 22, CustomParserType = typeof(ParseTimeSpanNull), OutputFormat = "", OutputNullValue = "")]
        public TimeSpan? _TimeSpanValueNull { get; set; }

        [Column(Index = 23, CustomParserType = typeof(ParseByteNull), OutputFormat = "", OutputNullValue = "")]
        public Byte? _ByteValueNull { get; set; }

        [Column(Index = 24, CustomParserType = typeof(ParseSByteNull), OutputFormat = "", OutputNullValue = "")]
        public SByte? _SByteValueNull { get; set; }

        [Column(Index = 25, CustomParserType = typeof(ParseInt16Null), OutputFormat = "", OutputNullValue = "")]
        public Int16? _Int16ValueNull { get; set; }

        [Column(Index = 26, CustomParserType = typeof(ParseInt32Null), OutputFormat = "", OutputNullValue = "")]
        public Int32? _Int32ValueNull { get; set; }

        [Column(Index = 27, CustomParserType = typeof(ParseInt64Null), OutputFormat = "", OutputNullValue = "")]
        public Int64? _Int64ValueNull { get; set; }

        [Column(Index = 28, CustomParserType = typeof(ParseSingleNull), OutputFormat = "", OutputNullValue = "")]
        public Single? _SingleValueNull { get; set; }

        [Column(Index = 29, CustomParserType = typeof(ParseDecimalNull), OutputFormat = "", OutputNullValue = "")]
        public Decimal? _DecimalValueNull { get; set; }

        [Column(Index = 30, CustomParserType = typeof(ParseDoubleNull), OutputFormat = "", OutputNullValue = "")]
        public Double? _DoubleValueNull { get; set; }

        [Column(Index = 31, CustomParserType = typeof(ParseUInt16Null), OutputFormat = "", OutputNullValue = "")]
        public UInt16? _UInt16ValueNull { get; set; }

        [Column(Index = 32, CustomParserType = typeof(ParseUInt32Null), OutputFormat = "", OutputNullValue = "")]
        public UInt32? _UInt32ValueNull { get; set; }

        [Column(Index = 33, CustomParserType = typeof(ParseUInt64Null), OutputFormat = "", OutputNullValue = "")]
        public UInt64? _UInt64ValueNull { get; set; }

        [Column(Index = 34, CustomParserType = typeof(ParseBigIntegerNull), OutputFormat = "", OutputNullValue = "")]
        public BigInteger? _BigIntegerValueNull { get; set; }

    }

    public class Parsestring : ICustomCsvParse<string>
    {
        public string Read(StringBuilder value)
        {
            return value.ToString();
        }
        public string Write(string value)
        {
            return value;
        }
    }

    public class ParseGuid : ICustomCsvParse<Guid>
    {
        public Guid Read(StringBuilder value) => Guid.Parse(value.ToString());
        public string Write(Guid value) => value.ToString();
    }

    public class ParseDateTime : ICustomCsvParse<DateTime>
    {
        public DateTime Read(StringBuilder value) => DateTime.Parse(value.ToString());
        public string Write(DateTime value) => value.ToString();
    }

    public class ParseDateTimeOffset : ICustomCsvParse<DateTimeOffset>
    {
        public DateTimeOffset Read(StringBuilder value) => DateTimeOffset.Parse(value.ToString());
        public string Write(DateTimeOffset value) => value.ToString();
    }

    public class ParseTimeSpan : ICustomCsvParse<TimeSpan>
    {
        public TimeSpan Read(StringBuilder value) => TimeSpan.Parse(value.ToString());
        public string Write(TimeSpan value) => value.ToString();
    }

    public class ParseByte : ICustomCsvParse<Byte>
    {
        public Byte Read(StringBuilder value) => Byte.Parse(value.ToString());
        public string Write(Byte value) => value.ToString();
    }

    public class ParseSByte : ICustomCsvParse<SByte>
    {
        public SByte Read(StringBuilder value) => SByte.Parse(value.ToString());
        public string Write(SByte value) => value.ToString();
    }

    public class ParseInt16 : ICustomCsvParse<Int16>
    {
        public Int16 Read(StringBuilder value) => Int16.Parse(value.ToString());
        public string Write(Int16 value) => value.ToString();
    }

    public class ParseInt32 : ICustomCsvParse<Int32>
    {
        public Int32 Read(StringBuilder value) => Int32.Parse(value.ToString());
        public string Write(Int32 value) => value.ToString();
    }

    public class ParseInt64 : ICustomCsvParse<Int64>
    {
        public Int64 Read(StringBuilder value) => Int64.Parse(value.ToString());
        public string Write(Int64 value) => value.ToString();
    }

    public class ParseSingle : ICustomCsvParse<Single>
    {
        public Single Read(StringBuilder value) => Single.Parse(value.ToString());
        public string Write(Single value) => value.ToString();
    }

    public class ParseDecimal : ICustomCsvParse<Decimal>
    {
        private CultureInfo _culture;

        public ParseDecimal()
        {
            _culture = CultureInfo.GetCultureInfo("en-us");
        }

        public Decimal Read(StringBuilder value) => Decimal.Parse(value.ToString(), _culture);
        public string Write(Decimal value) => value.ToString(_culture);
    }

    public class ParseDouble : ICustomCsvParse<Double>
    {
        private CultureInfo _culture;

        public ParseDouble()
        {
            _culture = CultureInfo.GetCultureInfo("en-us");
        }

        public Double Read(StringBuilder value) => Double.Parse(value.ToString(), _culture);
        public string Write(Double value) => value.ToString(_culture);
    }

    public class ParseUInt16 : ICustomCsvParse<UInt16>
    {
        public UInt16 Read(StringBuilder value) => UInt16.Parse(value.ToString());
        public string Write(UInt16 value) => value.ToString();
    }

    public class ParseUInt32 : ICustomCsvParse<UInt32>
    {
        public UInt32 Read(StringBuilder value) => UInt32.Parse(value.ToString());
        public string Write(UInt32 value) => value.ToString();
    }

    public class ParseUInt64 : ICustomCsvParse<UInt64>
    {
        public UInt64 Read(StringBuilder value) => UInt64.Parse(value.ToString());
        public string Write(UInt64 value) => value.ToString();
    }


    public class ParseBigInteger : ICustomCsvParse<BigInteger>
    {
        public BigInteger Read(StringBuilder value) => BigInteger.Parse(value.ToString());
        public string Write(BigInteger value) => value.ToString();
    }

    public class ParseBigIntegerNull : ICustomCsvParse<BigInteger?>
    {
        public BigInteger? Read(StringBuilder value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            return BigInteger.Parse(value.ToString());
        }

        public string Write(BigInteger? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }
            return value.Value.ToString();
        }
    }


    public class ParseGuidNull : ICustomCsvParse<Guid?>
    {
        public Guid? Read(StringBuilder value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            return Guid.Parse(value.ToString());
        }

        public string Write(Guid? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }
            return value.Value.ToString();
        }
    }

    public class ParseBooleanNull : ICustomCsvParse<Boolean?>
    {
        public Boolean? Read(StringBuilder value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            return Boolean.Parse(value.ToString());
        }

        public string Write(Boolean? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }
            return value.Value.ToString();
        }
    }

    public class ParseDateTimeNull : ICustomCsvParse<DateTime?>
    {
        public DateTime? Read(StringBuilder value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            return DateTime.Parse(value.ToString());
        }

        public string Write(DateTime? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }
            return value.Value.ToString();
        }
    }

    public class ParseDateTimeOffsetNull : ICustomCsvParse<DateTimeOffset?>
    {
        public DateTimeOffset? Read(StringBuilder value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            return DateTimeOffset.Parse(value.ToString());
        }

        public string Write(DateTimeOffset? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }
            return value.Value.ToString();
        }
    }

    public class ParseTimeSpanNull : ICustomCsvParse<TimeSpan?>
    {
        public TimeSpan? Read(StringBuilder value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            return TimeSpan.Parse(value.ToString());
        }

        public string Write(TimeSpan? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }
            return value.Value.ToString();
        }
    }

    public class ParseByteNull : ICustomCsvParse<Byte?>
    {
        public Byte? Read(StringBuilder value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            return Byte.Parse(value.ToString());
        }

        public string Write(Byte? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }
            return value.Value.ToString();
        }
    }

    public class ParseSByteNull : ICustomCsvParse<SByte?>
    {
        public SByte? Read(StringBuilder value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            return SByte.Parse(value.ToString());
        }

        public string Write(SByte? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }
            return value.Value.ToString();
        }
    }

    public class ParseInt16Null : ICustomCsvParse<Int16?>
    {
        public Int16? Read(StringBuilder value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            return Int16.Parse(value.ToString());
        }

        public string Write(Int16? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }
            return value.Value.ToString();
        }
    }

    public class ParseInt32Null : ICustomCsvParse<Int32?>
    {
        public Int32? Read(StringBuilder value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            return Int32.Parse(value.ToString());
        }

        public string Write(Int32? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }
            return value.Value.ToString();
        }
    }

    public class ParseInt64Null : ICustomCsvParse<Int64?>
    {
        public Int64? Read(StringBuilder value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            return Int64.Parse(value.ToString());
        }

        public string Write(Int64? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }
            return value.Value.ToString();
        }
    }

    public class ParseSingleNull : ICustomCsvParse<Single?>
    {
        public Single? Read(StringBuilder value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            return Single.Parse(value.ToString());
        }

        public string Write(Single? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }
            return value.Value.ToString();
        }
    }

    public class ParseDecimalNull : ICustomCsvParse<Decimal?>
    {
        public Decimal? Read(StringBuilder value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            return Decimal.Parse(value.ToString());
        }

        public string Write(Decimal? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }
            return value.Value.ToString();
        }
    }

    public class ParseDoubleNull : ICustomCsvParse<Double?>
    {
        public Double? Read(StringBuilder value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            return Double.Parse(value.ToString());
        }

        public string Write(Double? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }
            return value.Value.ToString();
        }
    }

    public class ParseUInt16Null : ICustomCsvParse<UInt16?>
    {
        public UInt16? Read(StringBuilder value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            return UInt16.Parse(value.ToString());
        }

        public string Write(UInt16? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }
            return value.Value.ToString();
        }
    }

    public class ParseUInt32Null : ICustomCsvParse<UInt32?>
    {
        public UInt32? Read(StringBuilder value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            return UInt32.Parse(value.ToString());
        }

        public string Write(UInt32? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }
            return value.Value.ToString();
        }
    }

    public class ParseUInt64Null : ICustomCsvParse<UInt64?>
    {
        public UInt64? Read(StringBuilder value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            return UInt64.Parse(value.ToString());
        }

        public string Write(UInt64? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }
            return value.Value.ToString();
        }
    }


    public class ParseBoolean : ICustomCsvParse<bool>
    {
        public bool Read(StringBuilder value)
        {
            switch (value.ToString().ToLower())
            {
                case "on":
                case "true":
                case "yes":
                case "1":
                    return true;
                case "off":
                case "false":
                case "no":
                case "0":
                    return false;
            }
            return false;
        }

        public string Write(bool value)
        {
            if (value == true)
            {
                return "yes";
            }
            return "no";
        }
    }
}
