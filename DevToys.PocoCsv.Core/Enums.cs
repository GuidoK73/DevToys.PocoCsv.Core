namespace DevToys.PocoCsv.Core
{
    internal enum State 
    {
        Normal = 0, // Field separator: ',' Line ending: "\r\n" or '\r' or '\n', Switch to Escape mode: '"'
        Escaped = 1, // '\r' and '\n' and ',' and "" are seen as part of the value.
        EscapedEscape = 2,
    }

    /// <summary>
    /// How should empty lines be treated.
    /// </summary>
    public enum EmptyLineBehaviour
    {
        /// <summary>
        /// Return a new instance of T
        /// </summary>
        DefaultInstance = 0,

        /// <summary>
        /// Return Null value for object.
        /// </summary>
        NullValue = 1,

        /// <summary>
        /// Skip the line and read the next.
        /// </summary>
        SkipAndReadNext = 2,

        /// <summary>
        /// Add an entry in the LogErrors 
        /// </summary>
        LogError = 3,

        /// <summary>
        /// Throw an Exception
        /// </summary>
        ThrowException = 4
    }


    /// <summary>
    /// How will the write behave with null objects.
    /// </summary>
    public enum WriteNullValueBehaviour
    {
        /// <summary>
        /// Ignore the value
        /// </summary>
        Skip = 0,

        /// <summary>
        /// Write an empty line
        /// </summary>
        EmptyLine = 1
    }

    /// <summary>
    /// Indicate how lines are ended when writing CSV Files.
    /// </summary>
    public enum CRLFMode
    {
        /// <summary>
        /// \r\n = CR + LF → Used as a new line character in Windows
        /// </summary>
        CRLF = 0,

        /// <summary>
        /// \r = CR(Carriage Return) → Used as a new line character in Mac OS before X
        /// </summary>
        CR = 1,

        /// <summary>
        ///  \n = LF(Line Feed) → Used as a new line character in Unix/Mac OS X
        /// </summary>
        LF = 2,
    }

    /// <summary>
    /// Represents a DotNet Type returned by TypeUtils.GetNetType
    /// </summary>
    public enum NetType
    {
        /// <summary>Unknown Net Type</summary>
        Unknown = 0,

        /// <summary>Type is String</summary>
        String = 1,

        /// <summary>Type is Guid</summary>
        Guid = 2,

        /// <summary>Type is Bool</summary>
        Boolean = 3,

        /// <summary>Type is DateTime</summary>
        DateTime = 4,

        /// <summary>Type is DateTime Offset</summary>
        DateTimeOffset = 5,

        /// <summary>Type is TimeSpan</summary>
        TimeSpan = 6,

        /// <summary>Type is Byte</summary>
        Byte = 7,

        /// <summary>Type is SByte</summary>
        SByte = 8,

        /// <summary>Type is Int16</summary>
        Int16 = 9,

        /// <summary>Type is Int32</summary>
        Int32 = 10,

        /// <summary>Type is Int64</summary>
        Int64 = 11,

        /// <summary>Type is Single</summary>
        Single = 12,

        /// <summary>Type is Decimal</summary>
        Decimal = 13,

        /// <summary>Type is Double</summary>
        Double = 14,

        /// <summary>Type is UInt16</summary>
        UInt16 = 15,

        /// <summary>Type is UInt32</summary>
        UInt32 = 16,

        /// <summary>Type is UInt64</summary>
        UInt64 = 17,

        /// <summary>Type is Enum</summary>
        Enum = 18,

        /// <summary>Type is BigInteger </summary>
        BigInteger = 19,

        /// <summary>Type is Null</summary>
        Null = 99
    }


    /// <summary></summary>
    public enum NetTypeComplete
    {
        /// <summary></summary>
        String = 0,

        /// <summary></summary>
        Guid = 1,

        /// <summary></summary>
        Boolean = 2,

        /// <summary></summary>
        DateTime = 3,

        /// <summary></summary>
        DateTimeOffset = 4,

        /// <summary></summary>
        TimeSpan = 5,

        /// <summary></summary>
        Byte = 6,

        /// <summary></summary>
        SByte = 7,

        /// <summary></summary>
        Int16 = 8,

        /// <summary></summary>
        Int32 = 9,

        /// <summary></summary>
        Int64 = 10,

        /// <summary></summary>
        Single = 11,

        /// <summary></summary>
        Decimal = 12,

        /// <summary></summary>
        Double = 13,

        /// <summary></summary>
        UInt16 = 14,

        /// <summary></summary>
        UInt32 = 15,

        /// <summary></summary>
        UInt64 = 16,

        /// <summary></summary>
        Enum = 17,

        /// <summary></summary>
        GuidNullable = 18,

        /// <summary></summary>
        BooleanNullable = 19,

        /// <summary></summary>
        DateTimeNullable = 20,

        /// <summary></summary>
        DateTimeOffsetNullable = 21,

        /// <summary></summary>
        TimeSpanNullable = 22,

        /// <summary></summary>
        ByteNullable = 23,

        /// <summary></summary>
        SByteNullable = 24,

        /// <summary></summary>
        Int16Nullable = 25,

        /// <summary></summary>
        Int32Nullable = 26,

        /// <summary></summary>
        Int64Nullable = 27,

        /// <summary></summary>
        SingleNullable = 28,

        /// <summary></summary>
        DecimalNullable = 29,

        /// <summary></summary>
        DoubleNullable = 30,

        /// <summary></summary>
        UInt16Nullable = 31,

        /// <summary></summary>
        UInt32Nullable = 32,

        /// <summary></summary>
        UInt64Nullable = 33,

        /// <summary></summary>
        ByteArray = 34,

        /// <summary></summary>
        BigInteger = 35,

        /// <summary></summary>
        BigIntegerNullable = 36,

        /// <summary></summary>
        EnumNullable = 37,

        /// <summary></summary>
        Unknown = 999
    }
}