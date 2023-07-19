using Delegates;
using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    public enum ReadOrWrite
    {
        Read = 0,
        Write = 1
    }

    /// <summary>
    /// Base class for BaseCsvReader and BaseCsvWriter
    /// </summary>
    public abstract class BaseCsv
    {
        /// <summary>
        /// Property Set by contructor, either File or Stream is used.
        /// </summary>
        protected string _File = null;

        /// <summary>
        /// Property Set by contructor, either File or Stream is used.
        /// </summary>
        protected Stream _Stream = null;

        /// <summary>
        /// Separator to use. Default: ','
        /// </summary>
        protected char _Separator = ',';

        /// <summary>
        /// Stream buffer size, Default: 1024
        /// </summary>
        protected int _BufferSize = 1024;

        /// <summary>
        /// Culture info to use for serialization.
        /// </summary>
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// The character encoding to use.
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.Default;

        internal protected PropertyInfo[] _Properties = null;

        internal protected Boolean[] _IsNullable = null;

        internal protected ICustomCsvParse<string>[] _CustomParserString = null;
        internal protected ICustomCsvParse<Guid>[] _CustomParserGuid = null;
        internal protected ICustomCsvParse<Boolean>[] _CustomParserBoolean = null;
        internal protected ICustomCsvParse<DateTime>[] _CustomParserDateTime = null;
        internal protected ICustomCsvParse<DateTimeOffset>[] _CustomParserDateTimeOffset = null;
        internal protected ICustomCsvParse<TimeSpan>[] _CustomParserTimeSpan = null;
        internal protected ICustomCsvParse<Byte>[] _CustomParserByte = null;
        internal protected ICustomCsvParse<SByte>[] _CustomParserSByte = null;
        internal protected ICustomCsvParse<Int16>[] _CustomParserInt16 = null;
        internal protected ICustomCsvParse<Int32>[] _CustomParserInt32 = null;
        internal protected ICustomCsvParse<Int64>[] _CustomParserInt64 = null;
        internal protected ICustomCsvParse<Single>[] _CustomParserSingle = null;
        internal protected ICustomCsvParse<Decimal>[] _CustomParserDecimal = null;
        internal protected ICustomCsvParse<Double>[] _CustomParserDouble = null;
        internal protected ICustomCsvParse<UInt16>[] _CustomParserUInt16 = null;
        internal protected ICustomCsvParse<UInt32>[] _CustomParserUInt32 = null;
        internal protected ICustomCsvParse<UInt64>[] _CustomParserUInt64 = null;
        internal protected ICustomCsvParse<BigInteger>[] _CustomParserBigInteger = null;

        internal protected ICustomCsvParse<Guid?>[] _CustomParserGuidNullable = null;
        internal protected ICustomCsvParse<Boolean?>[] _CustomParserBooleanNullable = null;
        internal protected ICustomCsvParse<DateTime?>[] _CustomParserDateTimeNullable = null;
        internal protected ICustomCsvParse<DateTimeOffset?>[] _CustomParserDateTimeOffsetNullable = null;
        internal protected ICustomCsvParse<TimeSpan?>[] _CustomParserTimeSpanNullable = null;
        internal protected ICustomCsvParse<Byte?>[] _CustomParserByteNullable = null;
        internal protected ICustomCsvParse<SByte?>[] _CustomParserSByteNullable = null;
        internal protected ICustomCsvParse<Int16?>[] _CustomParserInt16Nullable = null;
        internal protected ICustomCsvParse<Int32?>[] _CustomParserInt32Nullable = null;
        internal protected ICustomCsvParse<Int64?>[] _CustomParserInt64Nullable = null;
        internal protected ICustomCsvParse<Single?>[] _CustomParserSingleNullable = null;
        internal protected ICustomCsvParse<Decimal?>[] _CustomParserDecimalNullable = null;
        internal protected ICustomCsvParse<Double?>[] _CustomParserDoubleNullable = null;
        internal protected ICustomCsvParse<UInt16?>[] _CustomParserUInt16Nullable = null;
        internal protected ICustomCsvParse<UInt32?>[] _CustomParserUInt32Nullable = null;
        internal protected ICustomCsvParse<UInt64?>[] _CustomParserUInt64Nullable = null;
        internal protected ICustomCsvParse<BigInteger?>[] _CustomParserBigIntegerNullable = null;

        internal protected Func<object, object[], object>[] _CustomParserCall = null;

        internal protected CsvAttribute _CsvAttribute;


        internal protected void InitCustomCsvParseArrays(int size)
        {
            _CustomParserString = new ICustomCsvParse<string>[size];
            _CustomParserGuid = new ICustomCsvParse<Guid>[size];
            _CustomParserBoolean = new ICustomCsvParse<Boolean>[size];
            _CustomParserDateTime = new ICustomCsvParse<DateTime>[size];
            _CustomParserDateTimeOffset = new ICustomCsvParse<DateTimeOffset>[size];
            _CustomParserTimeSpan = new ICustomCsvParse<TimeSpan>[size];
            _CustomParserByte = new ICustomCsvParse<Byte>[size];
            _CustomParserSByte = new ICustomCsvParse<SByte>[size];
            _CustomParserInt16 = new ICustomCsvParse<Int16>[size];
            _CustomParserInt32 = new ICustomCsvParse<Int32>[size];
            _CustomParserInt64 = new ICustomCsvParse<Int64>[size];
            _CustomParserSingle = new ICustomCsvParse<Single>[size];
            _CustomParserDecimal = new ICustomCsvParse<Decimal>[size];
            _CustomParserDouble = new ICustomCsvParse<Double>[size];
            _CustomParserUInt16 = new ICustomCsvParse<UInt16>[size];
            _CustomParserUInt32 = new ICustomCsvParse<UInt32>[size];
            _CustomParserUInt64 = new ICustomCsvParse<UInt64>[size];
            _CustomParserBigInteger = new ICustomCsvParse<BigInteger>[size];

            _CustomParserGuidNullable = new ICustomCsvParse<Guid?>[size];
            _CustomParserBooleanNullable = new ICustomCsvParse<Boolean?>[size];
            _CustomParserDateTimeNullable = new ICustomCsvParse<DateTime?>[size];
            _CustomParserDateTimeOffsetNullable = new ICustomCsvParse<DateTimeOffset?>[size];
            _CustomParserTimeSpanNullable = new ICustomCsvParse<TimeSpan?>[size];
            _CustomParserByteNullable = new ICustomCsvParse<Byte?>[size];
            _CustomParserSByteNullable = new ICustomCsvParse<SByte?>[size];
            _CustomParserInt16Nullable = new ICustomCsvParse<Int16?>[size];
            _CustomParserInt32Nullable = new ICustomCsvParse<Int32?>[size];
            _CustomParserInt64Nullable = new ICustomCsvParse<Int64?>[size];
            _CustomParserSingleNullable = new ICustomCsvParse<Single?>[size];
            _CustomParserDecimalNullable = new ICustomCsvParse<Decimal?>[size];
            _CustomParserDoubleNullable = new ICustomCsvParse<Double?>[size];
            _CustomParserUInt16Nullable = new ICustomCsvParse<UInt16?>[size];
            _CustomParserUInt32Nullable = new ICustomCsvParse<UInt32?>[size];
            _CustomParserUInt64Nullable = new ICustomCsvParse<UInt64?>[size];
            _CustomParserBigIntegerNullable = new ICustomCsvParse<BigInteger?>[size];

            _CustomParserCall = new Func<object, object[], object>[size];
        }

        internal protected void SetCustomParserType(int index, Type customAttributeType, string propertyname)
        {
            if (TypeUtils.HasInterface<ICustomCsvParse<string>>(customAttributeType))
            {
                _CustomParserString[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<string>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Guid>>(customAttributeType))
            {
                _CustomParserGuid[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Guid>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Boolean>>(customAttributeType))
            {
                _CustomParserBoolean[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Boolean>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<DateTime>>(customAttributeType))
            {
                _CustomParserDateTime[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<DateTime>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<DateTimeOffset>>(customAttributeType))
            {
                _CustomParserDateTimeOffset[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<DateTimeOffset>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<TimeSpan>>(customAttributeType))
            {
                _CustomParserTimeSpan[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<TimeSpan>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Byte>>(customAttributeType))
            {
                _CustomParserByte[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Byte>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<SByte>>(customAttributeType))
            {
                _CustomParserSByte[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<SByte>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Int16>>(customAttributeType))
            {
                _CustomParserInt16[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Int16>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Int32>>(customAttributeType))
            {
                _CustomParserInt32[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Int32>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Int64>>(customAttributeType))
            {
                _CustomParserInt64[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Int64>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Single>>(customAttributeType))
            {
                _CustomParserSingle[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Single>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Decimal>>(customAttributeType))
            {
                _CustomParserDecimal[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Decimal>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Double>>(customAttributeType))
            {
                _CustomParserDouble[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Double>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<UInt16>>(customAttributeType))
            {
                _CustomParserUInt16[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<UInt16>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<UInt32>>(customAttributeType))
            {
                _CustomParserUInt32[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<UInt32>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<UInt64>>(customAttributeType))
            {
                _CustomParserUInt64[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<UInt64>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<BigInteger>>(customAttributeType))
            {
                _CustomParserBigInteger[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<BigInteger>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Guid?>>(customAttributeType))
            {
                _CustomParserGuidNullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Guid?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Boolean?>>(customAttributeType))
            {
                _CustomParserBooleanNullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Boolean?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<DateTime?>>(customAttributeType))
            {
                _CustomParserDateTimeNullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<DateTime?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<DateTimeOffset?>>(customAttributeType))
            {
                _CustomParserDateTimeOffsetNullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<DateTimeOffset?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<TimeSpan?>>(customAttributeType))
            {
                _CustomParserTimeSpanNullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<TimeSpan?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Byte?>>(customAttributeType))
            {
                _CustomParserByteNullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Byte?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<SByte?>>(customAttributeType))
            {
                _CustomParserSByteNullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<SByte?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Int16?>>(customAttributeType))
            {
                _CustomParserInt16Nullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Int16?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Int32?>>(customAttributeType))
            {
                _CustomParserInt32Nullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Int32?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Int64?>>(customAttributeType))
            {
                _CustomParserInt64Nullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Int64?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Single?>>(customAttributeType))
            {
                _CustomParserSingleNullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Single?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Decimal?>>(customAttributeType))
            {
                _CustomParserDecimalNullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Decimal?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Double?>>(customAttributeType))
            {
                _CustomParserDoubleNullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Double?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<UInt16?>>(customAttributeType))
            {
                _CustomParserUInt16Nullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<UInt16?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<UInt32?>>(customAttributeType))
            {
                _CustomParserUInt32Nullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<UInt32?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<UInt64?>>(customAttributeType))
            {
                _CustomParserUInt64Nullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<UInt64?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<BigInteger?>>(customAttributeType))
            {
                _CustomParserBigIntegerNullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<BigInteger?>;
            }
            else
            {
                throw new TypeLoadException($"PreParser type must implement PreParse interface. Property: {propertyname}");
            }
        }



        internal protected void InitCsvAttribute(Type type, int size, ReadOrWrite readOrWrite)
        {
            _CsvAttribute = type.GetCustomAttribute<CsvAttribute>();
            if (_CsvAttribute == null)
            {
                return;
            }

            for (int index = 0; index < size; index++)
            {
                if (_CsvAttribute.DefaultCustomParserTypeString != null)
                {
                    _CustomParserString[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeString) as ICustomCsvParse<string>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeString, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeString, "Write", typeof(string));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeGuid != null)
                {
                    _CustomParserGuid[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeGuid) as ICustomCsvParse<Guid>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeGuid, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeGuid, "Write", typeof(Guid));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeBoolean != null)
                {
                    _CustomParserBoolean[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeBoolean) as ICustomCsvParse<Boolean>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBoolean, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBoolean, "Write", typeof(Boolean));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeDateTime != null)
                {
                    _CustomParserDateTime[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDateTime) as ICustomCsvParse<DateTime>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTime, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTime, "Write", typeof(DateTime));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeDateTimeOffset != null)
                {
                    _CustomParserDateTimeOffset[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDateTimeOffset) as ICustomCsvParse<DateTimeOffset>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTimeOffset, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTimeOffset, "Write", typeof(DateTimeOffset));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeTimeSpan != null)
                {
                    _CustomParserTimeSpan[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeTimeSpan) as ICustomCsvParse<TimeSpan>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeTimeSpan, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeTimeSpan, "Write", typeof(TimeSpan));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeByte != null)
                {
                    _CustomParserByte[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeByte) as ICustomCsvParse<Byte>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeByte, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeByte, "Write", typeof(Byte));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeSByte != null)
                {
                    _CustomParserSByte[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeSByte) as ICustomCsvParse<SByte>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSByte, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSByte, "Write", typeof(SByte));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt16 != null)
                {
                    _CustomParserInt16[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt16) as ICustomCsvParse<Int16>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt16, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt16, "Write", typeof(Int16));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt32 != null)
                {
                    _CustomParserInt32[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt32) as ICustomCsvParse<Int32>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt32, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt32, "Write", typeof(Int32));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt64 != null)
                {
                    _CustomParserInt64[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt64) as ICustomCsvParse<Int64>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt64, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt64, "Write", typeof(Int64));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeSingle != null)
                {
                    _CustomParserSingle[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeSingle) as ICustomCsvParse<Single>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSingle, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSingle, "Write", typeof(Single));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeDecimal != null)
                {
                    _CustomParserDecimal[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDecimal) as ICustomCsvParse<Decimal>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDecimal, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDecimal, "Write", typeof(Decimal));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeDouble != null)
                {
                    _CustomParserDouble[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDouble) as ICustomCsvParse<Double>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDouble, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDouble, "Write", typeof(Double));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt16 != null)
                {
                    _CustomParserUInt16[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt16) as ICustomCsvParse<UInt16>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt16, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt16, "Write", typeof(UInt16));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt32 != null)
                {
                    _CustomParserUInt32[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt32) as ICustomCsvParse<UInt32>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt32, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt32, "Write", typeof(UInt32));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt64 != null)
                {
                    _CustomParserUInt64[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt64) as ICustomCsvParse<UInt64>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt64, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt64, "Write", typeof(UInt64));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeBigInteger != null)
                {
                    _CustomParserBigInteger[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeBigInteger) as ICustomCsvParse<BigInteger>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBigInteger, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBigInteger, "Write", typeof(BigInteger));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeGuidNullable != null)
                {
                    _CustomParserGuidNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeGuidNullable) as ICustomCsvParse<Guid?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeGuidNullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeGuidNullable, "Write", typeof(Guid?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeBooleanNullable != null)
                {
                    _CustomParserBooleanNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeBooleanNullable) as ICustomCsvParse<Boolean?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBooleanNullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBooleanNullable, "Write", typeof(Boolean?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeDateTimeNullable != null)
                {
                    _CustomParserDateTimeNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDateTimeNullable) as ICustomCsvParse<DateTime?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTimeNullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTimeNullable, "Write", typeof(DateTime?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeDateTimeOffsetNullable != null)
                {
                    _CustomParserDateTimeOffsetNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDateTimeOffsetNullable) as ICustomCsvParse<DateTimeOffset?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTimeOffsetNullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTimeOffsetNullable, "Write", typeof(DateTimeOffset?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeTimeSpanNullable != null)
                {
                    _CustomParserTimeSpanNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeTimeSpanNullable) as ICustomCsvParse<TimeSpan?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeTimeSpanNullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeTimeSpanNullable, "Write", typeof(TimeSpan?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeByteNullable != null)
                {
                    _CustomParserByteNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeByteNullable) as ICustomCsvParse<Byte?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeByteNullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeByteNullable, "Write", typeof(Byte?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeSByteNullable != null)
                {
                    _CustomParserSByteNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeSByteNullable) as ICustomCsvParse<SByte?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSByteNullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSByteNullable, "Write", typeof(SByte?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt16Nullable != null)
                {
                    _CustomParserInt16Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt16Nullable) as ICustomCsvParse<Int16?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt16Nullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt16Nullable, "Write", typeof(Int16?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt32Nullable != null)
                {
                    _CustomParserInt32Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt32Nullable) as ICustomCsvParse<Int32?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt32Nullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt32Nullable, "Write", typeof(Int32?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt64Nullable != null)
                {
                    _CustomParserInt64Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt64Nullable) as ICustomCsvParse<Int64?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt64Nullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt64Nullable, "Write", typeof(Int64?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeSingleNullable != null)
                {
                    _CustomParserSingleNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeSingleNullable) as ICustomCsvParse<Single?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSingleNullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSingleNullable, "Write", typeof(Single?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeDecimalNullable != null)
                {
                    _CustomParserDecimalNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDecimalNullable) as ICustomCsvParse<Decimal?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDecimalNullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDecimalNullable, "Write", typeof(Decimal?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeDoubleNullable != null)
                {
                    _CustomParserDoubleNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDoubleNullable) as ICustomCsvParse<Double?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDoubleNullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDoubleNullable, "Write", typeof(Double?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt16Nullable != null)
                {
                    _CustomParserUInt16Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt16Nullable) as ICustomCsvParse<UInt16?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt16Nullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt16Nullable, "Write", typeof(UInt16?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt32Nullable != null)
                {
                    _CustomParserUInt32Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt32Nullable) as ICustomCsvParse<UInt32?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt32Nullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt32Nullable, "Write", typeof(UInt32?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt64Nullable != null)
                {
                    _CustomParserUInt64Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt64Nullable) as ICustomCsvParse<UInt64?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt64Nullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt64Nullable, "Write", typeof(UInt64?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeBigIntegerNullable != null)
                {
                    _CustomParserBigIntegerNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeBigIntegerNullable) as ICustomCsvParse<BigInteger?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBigIntegerNullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        _CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBigIntegerNullable, "Write", typeof(BigInteger?));
                    }
                }
            }
        }
    }
}