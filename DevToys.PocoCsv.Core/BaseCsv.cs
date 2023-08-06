using Delegates;
using System;
using System.Collections.Immutable;
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

        protected internal ImmutableArray<PropertyInfo> _Properties;

        protected internal ImmutableArray<NetTypeComplete> _PropertyTypes;

        protected internal ImmutableArray<Boolean> _IsNullable;

        protected internal ImmutableArray<ICustomCsvParse<string>> _CustomParserString;
        protected internal ImmutableArray<ICustomCsvParse<Guid>> _CustomParserGuid;
        protected internal ImmutableArray<ICustomCsvParse<Boolean>> _CustomParserBoolean;
        protected internal ImmutableArray<ICustomCsvParse<DateTime>> _CustomParserDateTime;
        protected internal ImmutableArray<ICustomCsvParse<DateTimeOffset>> _CustomParserDateTimeOffset;
        protected internal ImmutableArray<ICustomCsvParse<TimeSpan>> _CustomParserTimeSpan;
        protected internal ImmutableArray<ICustomCsvParse<Byte>> _CustomParserByte;
        protected internal ImmutableArray<ICustomCsvParse<SByte>> _CustomParserSByte;
        protected internal ImmutableArray<ICustomCsvParse<Int16>> _CustomParserInt16;
        protected internal ImmutableArray<ICustomCsvParse<Int32>> _CustomParserInt32;
        protected internal ImmutableArray<ICustomCsvParse<Int64>> _CustomParserInt64;
        protected internal ImmutableArray<ICustomCsvParse<Single>> _CustomParserSingle;
        protected internal ImmutableArray<ICustomCsvParse<Decimal>> _CustomParserDecimal;
        protected internal ImmutableArray<ICustomCsvParse<Double>> _CustomParserDouble;
        protected internal ImmutableArray<ICustomCsvParse<UInt16>> _CustomParserUInt16;
        protected internal ImmutableArray<ICustomCsvParse<UInt32>> _CustomParserUInt32;
        protected internal ImmutableArray<ICustomCsvParse<UInt64>> _CustomParserUInt64;
        protected internal ImmutableArray<ICustomCsvParse<BigInteger>> _CustomParserBigInteger;

        protected internal ImmutableArray<ICustomCsvParse<Guid?>> _CustomParserGuidNullable;
        protected internal ImmutableArray<ICustomCsvParse<Boolean?>> _CustomParserBooleanNullable;
        protected internal ImmutableArray<ICustomCsvParse<DateTime?>> _CustomParserDateTimeNullable;
        protected internal ImmutableArray<ICustomCsvParse<DateTimeOffset?>> _CustomParserDateTimeOffsetNullable;
        protected internal ImmutableArray<ICustomCsvParse<TimeSpan?>> _CustomParserTimeSpanNullable;
        protected internal ImmutableArray<ICustomCsvParse<Byte?>> _CustomParserByteNullable;
        protected internal ImmutableArray<ICustomCsvParse<SByte?>> _CustomParserSByteNullable;
        protected internal ImmutableArray<ICustomCsvParse<Int16?>> _CustomParserInt16Nullable;
        protected internal ImmutableArray<ICustomCsvParse<Int32?>> _CustomParserInt32Nullable;
        protected internal ImmutableArray<ICustomCsvParse<Int64?>> _CustomParserInt64Nullable;
        protected internal ImmutableArray<ICustomCsvParse<Single?>> _CustomParserSingleNullable;
        protected internal ImmutableArray<ICustomCsvParse<Decimal?>> _CustomParserDecimalNullable;
        protected internal ImmutableArray<ICustomCsvParse<Double?>> _CustomParserDoubleNullable;
        protected internal ImmutableArray<ICustomCsvParse<UInt16?>> _CustomParserUInt16Nullable;
        protected internal ImmutableArray<ICustomCsvParse<UInt32?>> _CustomParserUInt32Nullable;
        protected internal ImmutableArray<ICustomCsvParse<UInt64?>> _CustomParserUInt64Nullable;
        protected internal ImmutableArray<ICustomCsvParse<BigInteger?>> _CustomParserBigIntegerNullable;

        protected internal ImmutableArray<Func<object, object[], object>> _CustomParserCall;


        private ICustomCsvParse<string>[] __CustomParserString = null;
        private ICustomCsvParse<Guid>[] __CustomParserGuid = null;
        private ICustomCsvParse<Boolean>[] __CustomParserBoolean = null;
        private ICustomCsvParse<DateTime>[] __CustomParserDateTime = null;
        private ICustomCsvParse<DateTimeOffset>[] __CustomParserDateTimeOffset = null;
        private ICustomCsvParse<TimeSpan>[] __CustomParserTimeSpan = null;
        private ICustomCsvParse<Byte>[] __CustomParserByte = null;
        private ICustomCsvParse<SByte>[] __CustomParserSByte = null;
        private ICustomCsvParse<Int16>[] __CustomParserInt16 = null;
        private ICustomCsvParse<Int32>[] __CustomParserInt32 = null;
        private ICustomCsvParse<Int64>[] __CustomParserInt64 = null;
        private ICustomCsvParse<Single>[] __CustomParserSingle = null;
        private ICustomCsvParse<Decimal>[] __CustomParserDecimal = null;
        private ICustomCsvParse<Double>[] __CustomParserDouble = null;
        private ICustomCsvParse<UInt16>[] __CustomParserUInt16 = null;
        private ICustomCsvParse<UInt32>[] __CustomParserUInt32 = null;
        private ICustomCsvParse<UInt64>[] __CustomParserUInt64 = null;
        private ICustomCsvParse<BigInteger>[] __CustomParserBigInteger = null;

        private ICustomCsvParse<Guid?>[] __CustomParserGuidNullable = null;
        private ICustomCsvParse<Boolean?>[] __CustomParserBooleanNullable = null;
        private ICustomCsvParse<DateTime?>[] __CustomParserDateTimeNullable = null;
        private ICustomCsvParse<DateTimeOffset?>[] __CustomParserDateTimeOffsetNullable = null;
        private ICustomCsvParse<TimeSpan?>[] __CustomParserTimeSpanNullable = null;
        private ICustomCsvParse<Byte?>[] __CustomParserByteNullable = null;
        private ICustomCsvParse<SByte?>[] __CustomParserSByteNullable = null;
        private ICustomCsvParse<Int16?>[] __CustomParserInt16Nullable = null;
        private ICustomCsvParse<Int32?>[] __CustomParserInt32Nullable = null;
        private ICustomCsvParse<Int64?>[] __CustomParserInt64Nullable = null;
        private ICustomCsvParse<Single?>[] __CustomParserSingleNullable = null;
        private ICustomCsvParse<Decimal?>[] __CustomParserDecimalNullable = null;
        private ICustomCsvParse<Double?>[] __CustomParserDoubleNullable = null;
        private ICustomCsvParse<UInt16?>[] __CustomParserUInt16Nullable = null;
        private ICustomCsvParse<UInt32?>[] __CustomParserUInt32Nullable = null;
        private ICustomCsvParse<UInt64?>[] __CustomParserUInt64Nullable = null;
        private ICustomCsvParse<BigInteger?>[] __CustomParserBigIntegerNullable = null;

        protected internal Func<object, object[], object>[] __CustomParserCall = null;




        protected internal CsvAttribute _CsvAttribute;

        protected internal void InitCustomCsvParseArrays(int size)
        {
            __CustomParserString = new ICustomCsvParse<string>[size];
            __CustomParserGuid = new ICustomCsvParse<Guid>[size];
            __CustomParserBoolean = new ICustomCsvParse<Boolean>[size];
            __CustomParserDateTime = new ICustomCsvParse<DateTime>[size];
            __CustomParserDateTimeOffset = new ICustomCsvParse<DateTimeOffset>[size];
            __CustomParserTimeSpan = new ICustomCsvParse<TimeSpan>[size];
            __CustomParserByte = new ICustomCsvParse<Byte>[size];
            __CustomParserSByte = new ICustomCsvParse<SByte>[size];
            __CustomParserInt16 = new ICustomCsvParse<Int16>[size];
            __CustomParserInt32 = new ICustomCsvParse<Int32>[size];
            __CustomParserInt64 = new ICustomCsvParse<Int64>[size];
            __CustomParserSingle = new ICustomCsvParse<Single>[size];
            __CustomParserDecimal = new ICustomCsvParse<Decimal>[size];
            __CustomParserDouble = new ICustomCsvParse<Double>[size];
            __CustomParserUInt16 = new ICustomCsvParse<UInt16>[size];
            __CustomParserUInt32 = new ICustomCsvParse<UInt32>[size];
            __CustomParserUInt64 = new ICustomCsvParse<UInt64>[size];
            __CustomParserBigInteger = new ICustomCsvParse<BigInteger>[size];

            __CustomParserGuidNullable = new ICustomCsvParse<Guid?>[size];
            __CustomParserBooleanNullable = new ICustomCsvParse<Boolean?>[size];
            __CustomParserDateTimeNullable = new ICustomCsvParse<DateTime?>[size];
            __CustomParserDateTimeOffsetNullable = new ICustomCsvParse<DateTimeOffset?>[size];
            __CustomParserTimeSpanNullable = new ICustomCsvParse<TimeSpan?>[size];
            __CustomParserByteNullable = new ICustomCsvParse<Byte?>[size];
            __CustomParserSByteNullable = new ICustomCsvParse<SByte?>[size];
            __CustomParserInt16Nullable = new ICustomCsvParse<Int16?>[size];
            __CustomParserInt32Nullable = new ICustomCsvParse<Int32?>[size];
            __CustomParserInt64Nullable = new ICustomCsvParse<Int64?>[size];
            __CustomParserSingleNullable = new ICustomCsvParse<Single?>[size];
            __CustomParserDecimalNullable = new ICustomCsvParse<Decimal?>[size];
            __CustomParserDoubleNullable = new ICustomCsvParse<Double?>[size];
            __CustomParserUInt16Nullable = new ICustomCsvParse<UInt16?>[size];
            __CustomParserUInt32Nullable = new ICustomCsvParse<UInt32?>[size];
            __CustomParserUInt64Nullable = new ICustomCsvParse<UInt64?>[size];
            __CustomParserBigIntegerNullable = new ICustomCsvParse<BigInteger?>[size];

            __CustomParserCall = new Func<object, object[], object>[size];
        }

        protected void InitImmutableArray()
        {
            _CustomParserString = __CustomParserString.ToImmutableArray();
            _CustomParserGuid = __CustomParserGuid.ToImmutableArray();
            _CustomParserBoolean = __CustomParserBoolean.ToImmutableArray();
            _CustomParserDateTime = __CustomParserDateTime.ToImmutableArray();
            _CustomParserDateTimeOffset = __CustomParserDateTimeOffset.ToImmutableArray();
            _CustomParserTimeSpan = __CustomParserTimeSpan.ToImmutableArray();
            _CustomParserByte = __CustomParserByte.ToImmutableArray();
            _CustomParserSByte = __CustomParserSByte.ToImmutableArray();
            _CustomParserInt16 = __CustomParserInt16.ToImmutableArray();
            _CustomParserInt32 = __CustomParserInt32.ToImmutableArray();
            _CustomParserInt64 = __CustomParserInt64.ToImmutableArray();
            _CustomParserSingle = __CustomParserSingle.ToImmutableArray();
            _CustomParserDecimal = __CustomParserDecimal.ToImmutableArray();
            _CustomParserDouble = __CustomParserDouble.ToImmutableArray();
            _CustomParserUInt16 = __CustomParserUInt16.ToImmutableArray();
            _CustomParserUInt32 = __CustomParserUInt32.ToImmutableArray();
            _CustomParserUInt64 = __CustomParserUInt64.ToImmutableArray();
            _CustomParserBigInteger = __CustomParserBigInteger.ToImmutableArray();
            _CustomParserGuidNullable = __CustomParserGuidNullable.ToImmutableArray();
            _CustomParserBooleanNullable = __CustomParserBooleanNullable.ToImmutableArray();
            _CustomParserDateTimeNullable = __CustomParserDateTimeNullable.ToImmutableArray();
            _CustomParserDateTimeOffsetNullable = __CustomParserDateTimeOffsetNullable.ToImmutableArray();
            _CustomParserTimeSpanNullable = __CustomParserTimeSpanNullable.ToImmutableArray();
            _CustomParserByteNullable = __CustomParserByteNullable.ToImmutableArray();
            _CustomParserSByteNullable = __CustomParserSByteNullable.ToImmutableArray();
            _CustomParserInt16Nullable = __CustomParserInt16Nullable.ToImmutableArray();
            _CustomParserInt32Nullable = __CustomParserInt32Nullable.ToImmutableArray();
            _CustomParserInt64Nullable = __CustomParserInt64Nullable.ToImmutableArray();
            _CustomParserSingleNullable = __CustomParserSingleNullable.ToImmutableArray();
            _CustomParserDecimalNullable = __CustomParserDecimalNullable.ToImmutableArray();
            _CustomParserDoubleNullable = __CustomParserDoubleNullable.ToImmutableArray();
            _CustomParserUInt16Nullable = __CustomParserUInt16Nullable.ToImmutableArray();
            _CustomParserUInt32Nullable = __CustomParserUInt32Nullable.ToImmutableArray();
            _CustomParserUInt64Nullable = __CustomParserUInt64Nullable.ToImmutableArray();
            _CustomParserBigIntegerNullable = __CustomParserBigIntegerNullable.ToImmutableArray();
            _CustomParserCall = __CustomParserCall.ToImmutableArray();
        }

        protected internal void SetCustomParserType(int index, Type customAttributeType, string propertyname)
        {
            if (TypeUtils.HasInterface<ICustomCsvParse<string>>(customAttributeType))
            {
                __CustomParserString[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<string>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Guid>>(customAttributeType))
            {
                __CustomParserGuid[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Guid>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Boolean>>(customAttributeType))
            {
                __CustomParserBoolean[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Boolean>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<DateTime>>(customAttributeType))
            {
                __CustomParserDateTime[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<DateTime>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<DateTimeOffset>>(customAttributeType))
            {
                __CustomParserDateTimeOffset[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<DateTimeOffset>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<TimeSpan>>(customAttributeType))
            {
                __CustomParserTimeSpan[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<TimeSpan>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Byte>>(customAttributeType))
            {
                __CustomParserByte[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Byte>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<SByte>>(customAttributeType))
            {
                __CustomParserSByte[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<SByte>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Int16>>(customAttributeType))
            {
                __CustomParserInt16[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Int16>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Int32>>(customAttributeType))
            {
                __CustomParserInt32[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Int32>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Int64>>(customAttributeType))
            {
                __CustomParserInt64[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Int64>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Single>>(customAttributeType))
            {
                __CustomParserSingle[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Single>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Decimal>>(customAttributeType))
            {
                __CustomParserDecimal[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Decimal>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Double>>(customAttributeType))
            {
                __CustomParserDouble[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Double>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<UInt16>>(customAttributeType))
            {
                __CustomParserUInt16[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<UInt16>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<UInt32>>(customAttributeType))
            {
                __CustomParserUInt32[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<UInt32>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<UInt64>>(customAttributeType))
            {
                __CustomParserUInt64[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<UInt64>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<BigInteger>>(customAttributeType))
            {
                __CustomParserBigInteger[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<BigInteger>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Guid?>>(customAttributeType))
            {
                __CustomParserGuidNullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Guid?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Boolean?>>(customAttributeType))
            {
                __CustomParserBooleanNullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Boolean?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<DateTime?>>(customAttributeType))
            {
                __CustomParserDateTimeNullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<DateTime?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<DateTimeOffset?>>(customAttributeType))
            {
                __CustomParserDateTimeOffsetNullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<DateTimeOffset?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<TimeSpan?>>(customAttributeType))
            {
                __CustomParserTimeSpanNullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<TimeSpan?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Byte?>>(customAttributeType))
            {
                __CustomParserByteNullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Byte?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<SByte?>>(customAttributeType))
            {
                __CustomParserSByteNullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<SByte?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Int16?>>(customAttributeType))
            {
                __CustomParserInt16Nullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Int16?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Int32?>>(customAttributeType))
            {
                __CustomParserInt32Nullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Int32?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Int64?>>(customAttributeType))
            {
                __CustomParserInt64Nullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Int64?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Single?>>(customAttributeType))
            {
                __CustomParserSingleNullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Single?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Decimal?>>(customAttributeType))
            {
                __CustomParserDecimalNullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Decimal?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<Double?>>(customAttributeType))
            {
                __CustomParserDoubleNullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<Double?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<UInt16?>>(customAttributeType))
            {
                __CustomParserUInt16Nullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<UInt16?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<UInt32?>>(customAttributeType))
            {
                __CustomParserUInt32Nullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<UInt32?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<UInt64?>>(customAttributeType))
            {
                __CustomParserUInt64Nullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<UInt64?>;
            }
            else if (TypeUtils.HasInterface<ICustomCsvParse<BigInteger?>>(customAttributeType))
            {
                __CustomParserBigIntegerNullable[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse<BigInteger?>;
            }
            else
            {
                throw new TypeLoadException($"PreParser type must implement PreParse interface. Property: {propertyname}");
            }
        }

        protected internal void InitCsvAttribute(Type type, int size, ReadOrWrite readOrWrite)
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
                    __CustomParserString[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeString) as ICustomCsvParse<string>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeString, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeString, "Write", typeof(string));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeGuid != null)
                {
                    __CustomParserGuid[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeGuid) as ICustomCsvParse<Guid>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeGuid, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeGuid, "Write", typeof(Guid));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeBoolean != null)
                {
                    __CustomParserBoolean[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeBoolean) as ICustomCsvParse<Boolean>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBoolean, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBoolean, "Write", typeof(Boolean));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeDateTime != null)
                {
                    __CustomParserDateTime[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDateTime) as ICustomCsvParse<DateTime>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTime, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTime, "Write", typeof(DateTime));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeDateTimeOffset != null)
                {
                    __CustomParserDateTimeOffset[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDateTimeOffset) as ICustomCsvParse<DateTimeOffset>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTimeOffset, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTimeOffset, "Write", typeof(DateTimeOffset));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeTimeSpan != null)
                {
                    __CustomParserTimeSpan[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeTimeSpan) as ICustomCsvParse<TimeSpan>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeTimeSpan, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeTimeSpan, "Write", typeof(TimeSpan));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeByte != null)
                {
                    __CustomParserByte[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeByte) as ICustomCsvParse<Byte>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeByte, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeByte, "Write", typeof(Byte));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeSByte != null)
                {
                    __CustomParserSByte[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeSByte) as ICustomCsvParse<SByte>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSByte, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSByte, "Write", typeof(SByte));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt16 != null)
                {
                    __CustomParserInt16[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt16) as ICustomCsvParse<Int16>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt16, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt16, "Write", typeof(Int16));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt32 != null)
                {
                    __CustomParserInt32[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt32) as ICustomCsvParse<Int32>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt32, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt32, "Write", typeof(Int32));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt64 != null)
                {
                    __CustomParserInt64[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt64) as ICustomCsvParse<Int64>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt64, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt64, "Write", typeof(Int64));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeSingle != null)
                {
                    __CustomParserSingle[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeSingle) as ICustomCsvParse<Single>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSingle, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSingle, "Write", typeof(Single));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeDecimal != null)
                {
                    __CustomParserDecimal[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDecimal) as ICustomCsvParse<Decimal>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDecimal, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDecimal, "Write", typeof(Decimal));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeDouble != null)
                {
                    __CustomParserDouble[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDouble) as ICustomCsvParse<Double>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDouble, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDouble, "Write", typeof(Double));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt16 != null)
                {
                    __CustomParserUInt16[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt16) as ICustomCsvParse<UInt16>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt16, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt16, "Write", typeof(UInt16));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt32 != null)
                {
                    __CustomParserUInt32[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt32) as ICustomCsvParse<UInt32>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt32, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt32, "Write", typeof(UInt32));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt64 != null)
                {
                    __CustomParserUInt64[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt64) as ICustomCsvParse<UInt64>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt64, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt64, "Write", typeof(UInt64));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeBigInteger != null)
                {
                    __CustomParserBigInteger[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeBigInteger) as ICustomCsvParse<BigInteger>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBigInteger, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBigInteger, "Write", typeof(BigInteger));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeGuidNullable != null)
                {
                    __CustomParserGuidNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeGuidNullable) as ICustomCsvParse<Guid?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeGuidNullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeGuidNullable, "Write", typeof(Guid?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeBooleanNullable != null)
                {
                    __CustomParserBooleanNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeBooleanNullable) as ICustomCsvParse<Boolean?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBooleanNullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBooleanNullable, "Write", typeof(Boolean?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeDateTimeNullable != null)
                {
                    __CustomParserDateTimeNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDateTimeNullable) as ICustomCsvParse<DateTime?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTimeNullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTimeNullable, "Write", typeof(DateTime?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeDateTimeOffsetNullable != null)
                {
                    __CustomParserDateTimeOffsetNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDateTimeOffsetNullable) as ICustomCsvParse<DateTimeOffset?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTimeOffsetNullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTimeOffsetNullable, "Write", typeof(DateTimeOffset?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeTimeSpanNullable != null)
                {
                    __CustomParserTimeSpanNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeTimeSpanNullable) as ICustomCsvParse<TimeSpan?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeTimeSpanNullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeTimeSpanNullable, "Write", typeof(TimeSpan?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeByteNullable != null)
                {
                    __CustomParserByteNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeByteNullable) as ICustomCsvParse<Byte?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeByteNullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeByteNullable, "Write", typeof(Byte?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeSByteNullable != null)
                {
                    __CustomParserSByteNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeSByteNullable) as ICustomCsvParse<SByte?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSByteNullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSByteNullable, "Write", typeof(SByte?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt16Nullable != null)
                {
                    __CustomParserInt16Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt16Nullable) as ICustomCsvParse<Int16?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt16Nullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt16Nullable, "Write", typeof(Int16?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt32Nullable != null)
                {
                    __CustomParserInt32Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt32Nullable) as ICustomCsvParse<Int32?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt32Nullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt32Nullable, "Write", typeof(Int32?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt64Nullable != null)
                {
                    __CustomParserInt64Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt64Nullable) as ICustomCsvParse<Int64?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt64Nullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt64Nullable, "Write", typeof(Int64?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeSingleNullable != null)
                {
                    __CustomParserSingleNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeSingleNullable) as ICustomCsvParse<Single?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSingleNullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSingleNullable, "Write", typeof(Single?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeDecimalNullable != null)
                {
                    __CustomParserDecimalNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDecimalNullable) as ICustomCsvParse<Decimal?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDecimalNullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDecimalNullable, "Write", typeof(Decimal?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeDoubleNullable != null)
                {
                    __CustomParserDoubleNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDoubleNullable) as ICustomCsvParse<Double?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDoubleNullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDoubleNullable, "Write", typeof(Double?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt16Nullable != null)
                {
                    __CustomParserUInt16Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt16Nullable) as ICustomCsvParse<UInt16?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt16Nullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt16Nullable, "Write", typeof(UInt16?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt32Nullable != null)
                {
                    __CustomParserUInt32Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt32Nullable) as ICustomCsvParse<UInt32?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt32Nullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt32Nullable, "Write", typeof(UInt32?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt64Nullable != null)
                {
                    __CustomParserUInt64Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt64Nullable) as ICustomCsvParse<UInt64?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt64Nullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt64Nullable, "Write", typeof(UInt64?));
                    }
                }
                if (_CsvAttribute.DefaultCustomParserTypeBigIntegerNullable != null)
                {
                    __CustomParserBigIntegerNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeBigIntegerNullable) as ICustomCsvParse<BigInteger?>;
                    if (readOrWrite == ReadOrWrite.Read)
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBigIntegerNullable, "Read", typeof(StringBuilder));
                    }
                    else
                    {
                        __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBigIntegerNullable, "Write", typeof(BigInteger?));
                    }
                }
            }
        }
    }
}