using System;
using System.Collections.Immutable;
using System.Numerics;
using System.Reflection;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Base class for BaseCsvReader and BaseCsvWriter
    /// </summary>
    public abstract class BaseCsv
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        internal protected ICustomCsvParse[] __CustomParser = null;

        internal protected ICustomCsvParse<string>[] __CustomParserString = null;
        internal protected ICustomCsvParse<Guid>[] __CustomParserGuid = null;
        internal protected ICustomCsvParse<Boolean>[] __CustomParserBoolean = null;
        internal protected ICustomCsvParse<DateTime>[] __CustomParserDateTime = null;
        internal protected ICustomCsvParse<DateTimeOffset>[] __CustomParserDateTimeOffset = null;
        internal protected ICustomCsvParse<TimeSpan>[] __CustomParserTimeSpan = null;
        internal protected ICustomCsvParse<Byte>[] __CustomParserByte = null;
        internal protected ICustomCsvParse<SByte>[] __CustomParserSByte = null;
        internal protected ICustomCsvParse<Int16>[] __CustomParserInt16 = null;
        internal protected ICustomCsvParse<Int32>[] __CustomParserInt32 = null;
        internal protected ICustomCsvParse<Int64>[] __CustomParserInt64 = null;
        internal protected ICustomCsvParse<Single>[] __CustomParserSingle = null;
        internal protected ICustomCsvParse<Decimal>[] __CustomParserDecimal = null;
        internal protected ICustomCsvParse<Double>[] __CustomParserDouble = null;
        internal protected ICustomCsvParse<UInt16>[] __CustomParserUInt16 = null;
        internal protected ICustomCsvParse<UInt32>[] __CustomParserUInt32 = null;
        internal protected ICustomCsvParse<UInt64>[] __CustomParserUInt64 = null;
        internal protected ICustomCsvParse<BigInteger>[] __CustomParserBigInteger = null;

        internal protected ICustomCsvParse<Guid?>[] __CustomParserGuidNullable = null;
        internal protected ICustomCsvParse<Boolean?>[] __CustomParserBooleanNullable = null;
        internal protected ICustomCsvParse<DateTime?>[] __CustomParserDateTimeNullable = null;
        internal protected ICustomCsvParse<DateTimeOffset?>[] __CustomParserDateTimeOffsetNullable = null;
        internal protected ICustomCsvParse<TimeSpan?>[] __CustomParserTimeSpanNullable = null;
        internal protected ICustomCsvParse<Byte?>[] __CustomParserByteNullable = null;
        internal protected ICustomCsvParse<SByte?>[] __CustomParserSByteNullable = null;
        internal protected ICustomCsvParse<Int16?>[] __CustomParserInt16Nullable = null;
        internal protected ICustomCsvParse<Int32?>[] __CustomParserInt32Nullable = null;
        internal protected ICustomCsvParse<Int64?>[] __CustomParserInt64Nullable = null;
        internal protected ICustomCsvParse<Single?>[] __CustomParserSingleNullable = null;
        internal protected ICustomCsvParse<Decimal?>[] __CustomParserDecimalNullable = null;
        internal protected ICustomCsvParse<Double?>[] __CustomParserDoubleNullable = null;
        internal protected ICustomCsvParse<UInt16?>[] __CustomParserUInt16Nullable = null;
        internal protected ICustomCsvParse<UInt32?>[] __CustomParserUInt32Nullable = null;
        internal protected ICustomCsvParse<UInt64?>[] __CustomParserUInt64Nullable = null;
        internal protected ICustomCsvParse<BigInteger?>[] __CustomParserBigIntegerNullable = null;

        internal protected ImmutableArray<PropertyInfo> _Properties;
        internal protected ImmutableArray<NetTypeComplete> _PropertyTypes;
        internal protected ImmutableArray<Boolean> _IsNullable;
        internal protected ImmutableArray<Boolean> _IsAssigned;

        internal protected ImmutableArray<ICustomCsvParse<string>> _CustomParserString;
        internal protected ImmutableArray<ICustomCsvParse<Guid>> _CustomParserGuid;
        internal protected ImmutableArray<ICustomCsvParse<Boolean>> _CustomParserBoolean;
        internal protected ImmutableArray<ICustomCsvParse<DateTime>> _CustomParserDateTime;
        internal protected ImmutableArray<ICustomCsvParse<DateTimeOffset>> _CustomParserDateTimeOffset;
        internal protected ImmutableArray<ICustomCsvParse<TimeSpan>> _CustomParserTimeSpan;
        internal protected ImmutableArray<ICustomCsvParse<Byte>> _CustomParserByte;
        internal protected ImmutableArray<ICustomCsvParse<SByte>> _CustomParserSByte;
        internal protected ImmutableArray<ICustomCsvParse<Int16>> _CustomParserInt16;
        internal protected ImmutableArray<ICustomCsvParse<Int32>> _CustomParserInt32;
        internal protected ImmutableArray<ICustomCsvParse<Int64>> _CustomParserInt64;
        internal protected ImmutableArray<ICustomCsvParse<Single>> _CustomParserSingle;
        internal protected ImmutableArray<ICustomCsvParse<Decimal>> _CustomParserDecimal;
        internal protected ImmutableArray<ICustomCsvParse<Double>> _CustomParserDouble;
        internal protected ImmutableArray<ICustomCsvParse<UInt16>> _CustomParserUInt16;
        internal protected ImmutableArray<ICustomCsvParse<UInt32>> _CustomParserUInt32;
        internal protected ImmutableArray<ICustomCsvParse<UInt64>> _CustomParserUInt64;
        internal protected ImmutableArray<ICustomCsvParse<BigInteger>> _CustomParserBigInteger;

        internal protected ImmutableArray<ICustomCsvParse<Guid?>> _CustomParserGuidNullable;
        internal protected ImmutableArray<ICustomCsvParse<Boolean?>> _CustomParserBooleanNullable;
        internal protected ImmutableArray<ICustomCsvParse<DateTime?>> _CustomParserDateTimeNullable;
        internal protected ImmutableArray<ICustomCsvParse<DateTimeOffset?>> _CustomParserDateTimeOffsetNullable;
        internal protected ImmutableArray<ICustomCsvParse<TimeSpan?>> _CustomParserTimeSpanNullable;
        internal protected ImmutableArray<ICustomCsvParse<Byte?>> _CustomParserByteNullable;
        internal protected ImmutableArray<ICustomCsvParse<SByte?>> _CustomParserSByteNullable;
        internal protected ImmutableArray<ICustomCsvParse<Int16?>> _CustomParserInt16Nullable;
        internal protected ImmutableArray<ICustomCsvParse<Int32?>> _CustomParserInt32Nullable;
        internal protected ImmutableArray<ICustomCsvParse<Int64?>> _CustomParserInt64Nullable;
        internal protected ImmutableArray<ICustomCsvParse<Single?>> _CustomParserSingleNullable;
        internal protected ImmutableArray<ICustomCsvParse<Decimal?>> _CustomParserDecimalNullable;
        internal protected ImmutableArray<ICustomCsvParse<Double?>> _CustomParserDoubleNullable;
        internal protected ImmutableArray<ICustomCsvParse<UInt16?>> _CustomParserUInt16Nullable;
        internal protected ImmutableArray<ICustomCsvParse<UInt32?>> _CustomParserUInt32Nullable;
        internal protected ImmutableArray<ICustomCsvParse<UInt64?>> _CustomParserUInt64Nullable;
        internal protected ImmutableArray<ICustomCsvParse<BigInteger?>> _CustomParserBigIntegerNullable;

        internal protected ImmutableArray<Func<object, object[], object>> _CustomParserCall;
        internal protected ImmutableArray<ICustomCsvParse> _ICustomCsvParseBase;
        internal protected Func<object, object[], object>[] __CustomParserCall = null;
        internal protected ICustomCsvParse[] __ICustomCsvParseBase;
        internal protected CsvAttribute _CsvAttribute;
#pragma warning restore CS1591 

        /// <summary>
        /// 
        /// </summary>
        internal protected void InitCustomCsvParseArrays(int size)
        {
            __CustomParser = new ICustomCsvParse[size];
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
            __ICustomCsvParseBase = new ICustomCsvParse[size];
        }

        /// <summary>
        /// 
        /// </summary>
        internal protected void InitImmutableArray()
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
            _ICustomCsvParseBase = __ICustomCsvParseBase.ToImmutableArray();

        }

        /// <summary>
        /// 
        /// </summary>
        internal protected void SetCustomParserType(int index, Type customAttributeType, string propertyname)
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
            else if (TypeUtils.HasInterface<ICustomCsvParse>(customAttributeType))
            {
                // only the Parsing method will work.
                __CustomParser[index] = Activator.CreateInstance(customAttributeType) as ICustomCsvParse;
            }
            else
            {
                throw new TypeLoadException($"CustomParserType type must implement CustomParserType interface. Property: {propertyname}");
            }
        }
    }
}