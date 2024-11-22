using System;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Csv Class Attribute, use this to set default custom parsers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class CsvAttribute : Attribute
    {
        /// <summary>
        /// File name only, can be used for the reader and writer when a directory is used in the contructor instead of a full file path.
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Type implementing ICustomCsvParse string
        /// </summary>
        public Type DefaultCustomParserTypeString { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse Guid
        /// </summary>
        public Type DefaultCustomParserTypeGuid { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse Boolean
        /// </summary>
        public Type DefaultCustomParserTypeBoolean { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse DateTime
        /// </summary>
        public Type DefaultCustomParserTypeDateTime { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse DateTimeOffset
        /// </summary>
        public Type DefaultCustomParserTypeDateTimeOffset { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse TimeSpan
        /// </summary>
        public Type DefaultCustomParserTypeTimeSpan { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse Byte
        /// </summary>
        public Type DefaultCustomParserTypeByte { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse SByte
        /// </summary>
        public Type DefaultCustomParserTypeSByte { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse Int16
        /// </summary>
        public Type DefaultCustomParserTypeInt16 { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse Int32
        /// </summary>
        public Type DefaultCustomParserTypeInt32 { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse Int64
        /// </summary>
        public Type DefaultCustomParserTypeInt64 { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse Single
        /// </summary>
        public Type DefaultCustomParserTypeSingle { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse Decimal
        /// </summary>
        public Type DefaultCustomParserTypeDecimal { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse Double
        /// </summary>
        public Type DefaultCustomParserTypeDouble { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse UInt16
        /// </summary>
        public Type DefaultCustomParserTypeUInt16 { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse UInt32
        /// </summary>
        public Type DefaultCustomParserTypeUInt32 { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse UInt64
        /// </summary>
        public Type DefaultCustomParserTypeUInt64 { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse BigInteger
        /// </summary>
        public Type DefaultCustomParserTypeBigInteger { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse Guid?
        /// </summary>
        public Type DefaultCustomParserTypeGuidNullable { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse Boolean?
        /// </summary>
        public Type DefaultCustomParserTypeBooleanNullable { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse DateTime?
        /// </summary>
        public Type DefaultCustomParserTypeDateTimeNullable { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse DateTimeOffset?
        /// </summary>
        public Type DefaultCustomParserTypeDateTimeOffsetNullable { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse TimeSpan?
        /// </summary>
        public Type DefaultCustomParserTypeTimeSpanNullable { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse Byte?
        /// </summary>
        public Type DefaultCustomParserTypeByteNullable { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse SByte?
        /// </summary>
        public Type DefaultCustomParserTypeSByteNullable { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse Int16?
        /// </summary>
        public Type DefaultCustomParserTypeInt16Nullable { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse Int32?
        /// </summary>
        public Type DefaultCustomParserTypeInt32Nullable { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse Int64?
        /// </summary>
        public Type DefaultCustomParserTypeInt64Nullable { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse Single?
        /// </summary>
        public Type DefaultCustomParserTypeSingleNullable { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse Decimal?
        /// </summary>
        public Type DefaultCustomParserTypeDecimalNullable { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse Double?
        /// </summary>
        public Type DefaultCustomParserTypeDoubleNullable { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse UInt16?
        /// </summary>
        public Type DefaultCustomParserTypeUInt16Nullable { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse UInt32?
        /// </summary>
        public Type DefaultCustomParserTypeUInt32Nullable { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse UInt64?
        /// </summary>
        public Type DefaultCustomParserTypeUInt64Nullable { get; set; } = null;

        /// <summary>
        /// Type implementing ICustomCsvParse BigInteger?
        /// </summary>
        public Type DefaultCustomParserTypeBigIntegerNullable { get; set; } = null;
    }
}