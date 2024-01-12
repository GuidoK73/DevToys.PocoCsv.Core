using Delegates;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Write T to Csv Stream from an IEnumerable source.
    /// </summary>
    public sealed class CsvWriter<T> : BaseCsv, IDisposable where T : class, new()
    {
        // Keep boxing at a minimum.
        private ImmutableArray<Func<T, object>> _PropertyGetterByteArray;
        private ImmutableArray<Func<T, Int32>> _PropertyGetterEnum;
        private ImmutableArray<Func<T, string>> _PropertyGetterString;
        private ImmutableArray<Func<T, Guid>> _PropertyGetterGuid;
        private ImmutableArray<Func<T, Boolean>> _PropertyGetterBoolean;
        private ImmutableArray<Func<T, DateTime>> _PropertyGetterDateTime;
        private ImmutableArray<Func<T, DateTimeOffset>> _PropertyGetterDateTimeOffset;
        private ImmutableArray<Func<T, TimeSpan>> _PropertyGetterTimeSpan;
        private ImmutableArray<Func<T, Byte>> _PropertyGetterByte;
        private ImmutableArray<Func<T, SByte>> _PropertyGetterSByte;
        private ImmutableArray<Func<T, Int16>> _PropertyGetterInt16;
        private ImmutableArray<Func<T, Int32>> _PropertyGetterInt32;
        private ImmutableArray<Func<T, Int64>> _PropertyGetterInt64;
        private ImmutableArray<Func<T, Single>> _PropertyGetterSingle;
        private ImmutableArray<Func<T, Decimal>> _PropertyGetterDecimal;
        private ImmutableArray<Func<T, Double>> _PropertyGetterDouble;
        private ImmutableArray<Func<T, UInt16>> _PropertyGetterUInt16;
        private ImmutableArray<Func<T, UInt32>> _PropertyGetterUInt32;
        private ImmutableArray<Func<T, UInt64>> _PropertyGetterUInt64;
        private ImmutableArray<Func<T, BigInteger>> _PropertyGetterBigInteger;

        private ImmutableArray<Func<T, Guid?>> _PropertyGetterGuidNullable;
        private ImmutableArray<Func<T, Boolean?>> _PropertyGetterBooleanNullable;
        private ImmutableArray<Func<T, DateTime?>> _PropertyGetterDateTimeNullable;
        private ImmutableArray<Func<T, DateTimeOffset?>> _PropertyGetterDateTimeOffsetNullable;
        private ImmutableArray<Func<T, TimeSpan?>> _PropertyGetterTimeSpanNullable;
        private ImmutableArray<Func<T, Byte?>> _PropertyGetterByteNullable;
        private ImmutableArray<Func<T, SByte?>> _PropertyGetterSByteNullable;
        private ImmutableArray<Func<T, Int16?>> _PropertyGetterInt16Nullable;
        private ImmutableArray<Func<T, Int32?>> _PropertyGetterInt32Nullable;
        private ImmutableArray<Func<T, Int64?>> _PropertyGetterInt64Nullable;
        private ImmutableArray<Func<T, Single?>> _PropertyGetterSingleNullable;
        private ImmutableArray<Func<T, Decimal?>> _PropertyGetterDecimalNullable;
        private ImmutableArray<Func<T, Double?>> _PropertyGetterDoubleNullable;
        private ImmutableArray<Func<T, UInt16?>> _PropertyGetterUInt16Nullable;
        private ImmutableArray<Func<T, UInt32?>> _PropertyGetterUInt32Nullable;
        private ImmutableArray<Func<T, UInt64?>> _PropertyGetterUInt64Nullable;
        private ImmutableArray<Func<T, BigInteger?>> _PropertyGetterBigIntegerNullable;

        private ImmutableArray<string> _Formatters;
        private ImmutableArray<string> _OutputNullValues;
        private StreamWriter _StreamWriter;
        private int _MaxColumnIndex = 0;
        private const char _CR = '\r';
        private const char _LF = '\n';
        private const string _CRLF = "\r\n";

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvWriter(string file, char separator = ',')
        {
            _File = file;
            Separator = separator;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvWriter(Stream stream, char separator = ',')
        {
            _Stream = stream;
            Separator = separator;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvWriter(string file, Encoding encoding, CultureInfo culture, char separator = ',', int buffersize = -1)
        {
            Culture = culture;
            Separator = separator;
            Encoding = encoding;
            BufferSize = buffersize;
            _File = file;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvWriter(Stream stream, Encoding encoding, CultureInfo culture, char separator = ',', int buffersize = -1)
        {
            _Stream = stream;
            Culture = culture;
            Separator = separator;
            Encoding = encoding;
            BufferSize = buffersize;
        }

        /// <summary>
        /// Csv Seperator to use default ','
        /// </summary>
        public char Separator { get; set; } = ',';

        /// <summary>
        /// \r\n = CR + LF → Used as a new line character in Windows.
        /// \r = CR(Carriage Return) → Used as a new line character in Mac OS before X.
        /// \n = LF(Line Feed) → Used as a new line character in Unix/Mac OS X
        /// </summary>
        public CRLFMode CRLFMode { get; set; } = CRLFMode.CRLF;

        /// <summary>
        /// How will the write behave with null objects.
        /// </summary>
        public WriteNullValueBehaviour NullValueBehaviour { get; set; } = WriteNullValueBehaviour.Skip;

        /// <summary>
        /// All properties are handled in order of property occurrence and mapped directly to their respective index. (ColumnAttribute is ignored.)
        /// </summary>
        public bool IgnoreColumnAttributes { get; set; } = false;

        /// <summary>
        /// Releases all resources used by the System.IO.TextReader object.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Close();
        }

        /// <summary>
        /// Close the CSV stream reader
        /// </summary>
        public void Close()
        {
            if (_StreamWriter == null)
            {
                return;
            }
            _StreamWriter.Flush();
            _StreamWriter.Close();
        }

        /// <summary>
        /// Flush all buffers.
        /// </summary>
        public void Flush()
        {
            _StreamWriter.Flush();
        }

        /// <summary>
        /// Write header with property names of T.
        /// </summary>
        public void WriteHeader()
        {
            if (_Properties.Length == 0)
            {
                throw new IOException("Call Open() method first before writing.");
            }

            for (int ii = 0; ii <= _MaxColumnIndex; ii++)
            {
                if (_Properties[ii] != null)
                {
                    var _property = _Properties[ii];
                    var _name = _property.Name;
                    var _attribute = _property.GetCustomAttribute<ColumnAttribute>();
                    if (_attribute != null && !string.IsNullOrEmpty(_attribute.Header))
                    {
                        _name = _attribute.Header;
                    }
                    _StreamWriter.Write(_name);
                }
                else
                {
                    _StreamWriter.Write("");
                }
                if (ii < _MaxColumnIndex)
                {
                    _StreamWriter.Write(Separator);
                }
                _StreamWriter.Flush();
            }
        }

        /// <summary>
        /// Write IEnumerable T to Csv Stream
        /// </summary>
        public void Write(IEnumerable<T> rows)
        {
            if (_Properties.Length == 0)
            {
                throw new IOException("Call Open() method first before writing.");
            }

            foreach (T row in rows)
            {
                Write(row);
            }
            _StreamWriter.Flush();
        }

        /// <summary>
        /// Write single row to CSV
        /// </summary>
        public void Write(T row)
        {
            if (_Properties.Length == 0)
            {
                throw new IOException("Call Open() method first before writing.");
            }

            if (row == null && NullValueBehaviour == WriteNullValueBehaviour.Skip)
            {
                return;
            }

            if (_StreamWriter.BaseStream.Position > 0)
            {
                if (CRLFMode == CRLFMode.CRLF)
                {
                    _StreamWriter.Write(_CRLF);
                }
                else if (CRLFMode == CRLFMode.CR)
                {
                    _StreamWriter.Write(_CR);
                }
                else if (CRLFMode == CRLFMode.LF)
                {
                    _StreamWriter.Write(_LF);
                }
            }
            if (row == null && NullValueBehaviour == WriteNullValueBehaviour.EmptyLine)
            {
                return;
            }

            for (int index = 0; index <= _MaxColumnIndex; index++)
            {
                if (_Properties[index] != null)
                {
                    var _property = _Properties[index];
                    Type targetType = _property.PropertyType;

                    string _formatter = _Formatters[index];
                    string _outputNullValue = _OutputNullValues[index];
                    WriteValue(row, index, _formatter, _outputNullValue);
                }
                if (index < _MaxColumnIndex)
                {
                    _StreamWriter.Write(Separator);
                }
            }
            if (_StreamWriter.BaseStream.Position == 0)
            {
                _StreamWriter.Flush();
            }
        }

        private void WriteValue(T row, int index, string formatter, string outputNullValue)
        {
            switch (_PropertyTypes[index])
            {
                case NetTypeComplete.String:
                    WriteString(row, index);
                    break;

                case NetTypeComplete.GuidNullable:
                    WriteGuidNull(row, index, outputNullValue, formatter);
                    break;

                case NetTypeComplete.BooleanNullable:
                    WriteBooleanNull(row, index, outputNullValue);
                    break;

                case NetTypeComplete.DateTimeNullable:
                    WriteDateTimeNull(row, index, outputNullValue, formatter);
                    break;

                case NetTypeComplete.DateTimeOffsetNullable:
                    WriteDateTimeOffsetNull(row, index, outputNullValue, formatter);
                    break;

                case NetTypeComplete.TimeSpanNullable:
                    WriteTimeSpanNull(row, index, outputNullValue, formatter);
                    break;

                case NetTypeComplete.ByteNullable:
                    WriteByteNull(row, index, outputNullValue, formatter);
                    break;

                case NetTypeComplete.SByteNullable:
                    WriteSByteNull(row, index, outputNullValue, formatter);
                    break;

                case NetTypeComplete.Int16Nullable:
                    WriteInt16Null(row, index, outputNullValue, formatter);
                    break;

                case NetTypeComplete.Int32Nullable:
                    WriteInt32Null(row, index, outputNullValue, formatter);
                    break;

                case NetTypeComplete.Int64Nullable:
                    WriteInt64Null(row, index, outputNullValue, formatter);
                    break;

                case NetTypeComplete.SingleNullable:
                    WriteSingleNull(row, index, outputNullValue, formatter);
                    break;

                case NetTypeComplete.DecimalNullable:
                    WriteDecimalNull(row, index, outputNullValue, formatter);
                    break;

                case NetTypeComplete.DoubleNullable:
                    WriteDoubleNull(row, index, outputNullValue, formatter);
                    break;

                case NetTypeComplete.UInt16Nullable:
                    WriteUInt16Null(row, index, outputNullValue, formatter);
                    break;

                case NetTypeComplete.UInt32Nullable:
                    WriteUInt32Null(row, index, outputNullValue, formatter);
                    break;

                case NetTypeComplete.UInt64Nullable:
                    WriteUInt64Null(row, index, outputNullValue, formatter);
                    break;

                case NetTypeComplete.BigIntegerNullable:
                    WriteBigIntegerNull(row, index, outputNullValue, formatter);
                    break;

                case NetTypeComplete.Guid:
                    WriteGuid(row, index, formatter);
                    break;

                case NetTypeComplete.Boolean:
                    WriteBoolean(row, index);
                    break;

                case NetTypeComplete.DateTime:
                    WriteDateTime(row, index, formatter);
                    break;

                case NetTypeComplete.DateTimeOffset:
                    WriteDateTimeOffset(row, index, formatter);
                    break;

                case NetTypeComplete.TimeSpan:
                    WriteTimeSpan(row, index, formatter);
                    break;

                case NetTypeComplete.Byte:
                    WriteByte(row, index, formatter);
                    break;

                case NetTypeComplete.SByte:
                    WriteSByte(row, index, formatter);
                    break;

                case NetTypeComplete.Int16:
                    WriteInt16(row, index, formatter);
                    break;

                case NetTypeComplete.Int32:
                    WriteInt32(row, index, formatter);
                    break;

                case NetTypeComplete.Int64:
                    WriteInt64(row, index, formatter);
                    break;

                case NetTypeComplete.Single:
                    WriteSingle(row, index, formatter);
                    break;

                case NetTypeComplete.Decimal:
                    WriteDecimal(row, index, formatter);
                    break;

                case NetTypeComplete.Double:
                    WriteDouble(row, index, formatter);
                    break;

                case NetTypeComplete.UInt16:
                    WriteUInt16(row, index, formatter);
                    break;

                case NetTypeComplete.UInt32:
                    WriteUInt32(row, index, formatter);
                    break;

                case NetTypeComplete.UInt64:
                    WriteUInt64(row, index, formatter);
                    break;

                case NetTypeComplete.BigInteger:
                    WriteBigInteger(row, index, formatter);
                    break;

                case NetTypeComplete.ByteArray:
                    WriteByteArray(row, index);
                    break;

                case NetTypeComplete.Enum:
                    WriteEnum(row, index);
                    break;
            }
        }


        #region Value Writers

        private void WriteString(T row, int index)
        {
            string _value = _PropertyGetterString[index](row);
            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserString[index], new object[] { _value })));
                return;
            }
            if (_value != null)
            {

                _StreamWriter.Write(CsvUtils.Esc(_Separator, _value));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteByteArray(T row, int index)
        {
            byte[] _value = (byte[])_PropertyGetterByteArray[index](row);

            var _byteValue = (byte[])_value;
            if (_byteValue != null)
            {
                _StreamWriter.Write(Convert.ToBase64String(_byteValue));
            }
        }

        private void WriteEnum(T row, int index)
        {
            string _value = _PropertyGetterEnum[index](row).ToString();
            _StreamWriter.Write(_value);
        }

        private void WriteGuid(T row, int index, string formatter)
        {
            Guid value = _PropertyGetterGuid[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserGuid[index], new object[] { value })));
                return;
            }

            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString()));
            }
        }

        private void WriteGuidNull(T row, int index, string formatter, string _nullValueDefault)
        {
            Guid? value = _PropertyGetterGuidNullable[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserGuidNullable[index], new object[] { value })));
                return;
            }

            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString()));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, _nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteBoolean(T row, int index)
        {
            Boolean value = _PropertyGetterBoolean[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserBoolean[index], new object[] { value })));
                return;
            }

            _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(Culture)));
        }

        private void WriteBooleanNull(T row, int index, string _nullValueDefault)
        {
            Boolean? value = _PropertyGetterBooleanNullable[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserBooleanNullable[index], new object[] { value })));
                return;
            }

            if (value.HasValue)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(Culture)));
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, _nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteDateTime(T row, int index, string formatter)
        {
            DateTime value = _PropertyGetterDateTime[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserDateTime[index], new object[] { value })));
                return;
            }

            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(Culture)));
            }
        }

        private void WriteDateTimeNull(T row, int index, string formatter, string _nullValueDefault)
        {
            DateTime? value = _PropertyGetterDateTimeNullable[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserDateTimeNullable[index], new object[] { value })));
                return;
            }

            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, _nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteDateTimeOffset(T row, int index, string formatter)
        {
            DateTimeOffset value = _PropertyGetterDateTimeOffset[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserDateTimeOffset[index], new object[] { value })));
                return;
            }

            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(Culture)));
            }
        }

        private void WriteDateTimeOffsetNull(T row, int index, string formatter, string _nullValueDefault)
        {
            DateTimeOffset? value = _PropertyGetterDateTimeOffsetNullable[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserDateTimeOffsetNullable[index], new object[] { value })));
                return;
            }

            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, _nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteTimeSpan(T row, int index, string formatter)
        {
            TimeSpan value = _PropertyGetterTimeSpan[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserTimeSpan[index], new object[] { value })));
                return;
            }

            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString()));
            }
        }

        private void WriteTimeSpanNull(T row, int index, string formatter, string _nullValueDefault)
        {
            TimeSpan? value = _PropertyGetterTimeSpanNullable[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserTimeSpanNullable[index], new object[] { value })));
                return;
            }

            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString()));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, _nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteByte(T row, int index, string formatter)
        {
            Byte value = _PropertyGetterByte[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserByte[index], new object[] { value })));
                return;
            }

            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(Culture)));
            }
        }

        private void WriteByteNull(T row, int index, string formatter, string _nullValueDefault)
        {
            Byte? value = _PropertyGetterByteNullable[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserByteNullable[index], new object[] { value })));
                return;
            }

            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, _nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteSByte(T row, int index, string formatter)
        {
            SByte value = _PropertyGetterSByte[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserSByte[index], new object[] { value })));
                return;
            }

            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(Culture)));
            }
        }

        private void WriteSByteNull(T row, int index, string formatter, string _nullValueDefault)
        {
            SByte? value = _PropertyGetterSByteNullable[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserSByteNullable[index], new object[] { value })));
                return;
            }

            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, _nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteInt16(T row, int index, string formatter)
        {
            Int16 value = _PropertyGetterInt16[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserInt16[index], new object[] { value })));
                return;
            }

            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(Culture)));
            }
        }

        private void WriteInt16Null(T row, int index, string formatter, string _nullValueDefault)
        {
            Int16? value = _PropertyGetterInt16Nullable[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserInt16Nullable[index], new object[] { value })));
                return;
            }

            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, _nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteInt32(T row, int index, string formatter)
        {
            Int32 value = _PropertyGetterInt32[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserInt32[index], new object[] { value })));
                return;
            }

            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(Culture)));
            }
        }

        private void WriteInt32Null(T row, int index, string formatter, string _nullValueDefault)
        {
            Int32? value = _PropertyGetterInt32Nullable[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserInt32Nullable[index], new object[] { value })));
                return;
            }

            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, _nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteInt64(T row, int index, string formatter)
        {
            Int64 value = _PropertyGetterInt64[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserInt64[index], new object[] { value })));
                return;
            }

            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(Culture)));
            }
        }

        private void WriteInt64Null(T row, int index, string formatter, string _nullValueDefault)
        {
            Int64? value = _PropertyGetterInt64Nullable[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserInt64Nullable[index], new object[] { value })));
                return;
            }

            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, _nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteSingle(T row, int index, string formatter)
        {
            Single value = _PropertyGetterSingle[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserSingle[index], new object[] { value })));
                return;
            }

            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(Culture)));
            }
        }

        private void WriteSingleNull(T row, int index, string formatter, string _nullValueDefault)
        {
            Single? value = _PropertyGetterSingleNullable[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserSingleNullable[index], new object[] { value })));
                return;
            }

            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, _nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteDecimal(T row, int index, string formatter)
        {
            Decimal value = _PropertyGetterDecimal[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserDecimal[index], new object[] { value })));
                return;
            }

            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(Culture)));
            }
        }

        private void WriteDecimalNull(T row, int index, string formatter, string _nullValueDefault)
        {
            Decimal? value = _PropertyGetterDecimalNullable[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserDecimalNullable[index], new object[] { value })));
                return;
            }

            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, _nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteDouble(T row, int index, string formatter)
        {
            Double value = _PropertyGetterDouble[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserDouble[index], new object[] { value })));
                return;
            }

            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(Culture)));
            }
        }

        private void WriteDoubleNull(T row, int index, string formatter, string _nullValueDefault)
        {
            Double? value = _PropertyGetterDoubleNullable[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserDoubleNullable[index], new object[] { value })));
                return;
            }

            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, _nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteUInt16(T row, int index, string formatter)
        {
            UInt16 value = _PropertyGetterUInt16[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserUInt16[index], new object[] { value })));
                return;
            }

            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(Culture)));
            }
        }

        private void WriteUInt16Null(T row, int index, string formatter, string _nullValueDefault)
        {
            UInt16? value = _PropertyGetterUInt16Nullable[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserUInt16Nullable[index], new object[] { value })));
                return;
            }

            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, _nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteUInt32(T row, int index, string formatter)
        {
            UInt32 value = _PropertyGetterUInt32[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserUInt32[index], new object[] { value })));
                return;
            }

            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(Culture)));
            }
        }

        private void WriteUInt32Null(T row, int index, string formatter, string _nullValueDefault)
        {
            UInt32? value = _PropertyGetterUInt32Nullable[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserUInt32Nullable[index], new object[] { value })));
                return;
            }

            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, _nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteUInt64(T row, int index, string formatter)
        {
            UInt64 value = _PropertyGetterUInt64[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserUInt64[index], new object[] { value })));
                return;
            }

            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(Culture)));
            }
        }

        private void WriteBigInteger(T row, int index, string formatter)
        {
            BigInteger value = _PropertyGetterBigInteger[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserBigInteger[index], new object[] { value })));
                return;
            }

            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, value.ToString(Culture)));
            }
        }

        private void WriteBigIntegerNull(T row, int index, string formatter, string _nullValueDefault)
        {
            BigInteger? value = _PropertyGetterBigIntegerNullable[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserBigIntegerNullable[index], new object[] { value })));
                return;
            }

            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, _nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteUInt64Null(T row, int index, string formatter, string _nullValueDefault)
        {
            UInt64? value = _PropertyGetterUInt64Nullable[index](row);

            if (_CustomParserCall[index] != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, (string)_CustomParserCall[index](_CustomParserUInt64Nullable[index], new object[] { value })));
                return;
            }

            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(CsvUtils.Esc(_Separator, value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, _nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        #endregion

        /// <summary>
        /// Initialize and open the CSV Stream Writer.
        /// </summary>
        public void Open()
        {
            Init();
            if (_Stream != null)
            {
                _StreamWriter = new StreamWriter(stream: _Stream, encoding: Encoding, bufferSize: BufferSize);
            }
            if (!string.IsNullOrEmpty(_File))
            {
                _StreamWriter = new StreamWriter(path: _File, append: true, encoding: Encoding, bufferSize: BufferSize);
            }
        }

        /// <summary>
        /// Initialize
        /// </summary>
        private void Init()
        {
            if (_Properties != null)
                return;

            var _type = typeof(T);

            var _properties = _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => new { Property = p, Index = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index, Attrib = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute) });

            if (IgnoreColumnAttributes == true)
            {
                _properties = _type.GetProperties()
                .Select((value, index) => new { Property = value, Index = index, Attrib = new ColumnAttribute() {  Index = index } });
            }

            int _max = _properties.Max(p => p.Index);
            _MaxColumnIndex = _max;

            var _propertiesInfo = new PropertyInfo[_max + 1];

            var _propertyTypes = new NetTypeComplete[_max + 1];

            var _isNullable = new Boolean[_max + 1];
            var _isAssigned = new Boolean[_max + 1];
            var _propertyGetterEnum = new Func<T, Int32>[_max + 1];
            var _propertyGetterByteArray = new Func<T, object>[_max + 1];
            var _propertyGetterString = new Func<T, string>[_max + 1];
            var _propertyGetterGuid = new Func<T, Guid>[_max + 1];
            var _propertyGetterBoolean = new Func<T, Boolean>[_max + 1];
            var _propertyGetterDateTime = new Func<T, DateTime>[_max + 1];
            var _propertyGetterDateTimeOffset = new Func<T, DateTimeOffset>[_max + 1];
            var _propertyGetterTimeSpan = new Func<T, TimeSpan>[_max + 1];
            var _propertyGetterByte = new Func<T, Byte>[_max + 1];
            var _propertyGetterSByte = new Func<T, SByte>[_max + 1];
            var _propertyGetterInt16 = new Func<T, Int16>[_max + 1];
            var _propertyGetterInt32 = new Func<T, Int32>[_max + 1];
            var _propertyGetterInt64 = new Func<T, Int64>[_max + 1];
            var _propertyGetterSingle = new Func<T, Single>[_max + 1];
            var _propertyGetterDecimal = new Func<T, Decimal>[_max + 1];
            var _propertyGetterDouble = new Func<T, Double>[_max + 1];
            var _propertyGetterUInt16 = new Func<T, UInt16>[_max + 1];
            var _propertyGetterUInt32 = new Func<T, UInt32>[_max + 1];
            var _propertyGetterUInt64 = new Func<T, UInt64>[_max + 1];
            var _propertyGetterBigInteger = new Func<T, BigInteger>[_max + 1];

            var _propertyGetterGuidNullable = new Func<T, Guid?>[_max + 1];
            var _propertyGetterBooleanNullable = new Func<T, Boolean?>[_max + 1];
            var _propertyGetterDateTimeNullable = new Func<T, DateTime?>[_max + 1];
            var _propertyGetterDateTimeOffsetNullable = new Func<T, DateTimeOffset?>[_max + 1];
            var _propertyGetterTimeSpanNullable = new Func<T, TimeSpan?>[_max + 1];
            var _propertyGetterByteNullable = new Func<T, Byte?>[_max + 1];
            var _propertyGetterSByteNullable = new Func<T, SByte?>[_max + 1];
            var _propertyGetterInt16Nullable = new Func<T, Int16?>[_max + 1];
            var _propertyGetterInt32Nullable = new Func<T, Int32?>[_max + 1];
            var _propertyGetterInt64Nullable = new Func<T, Int64?>[_max + 1];
            var _propertyGetterSingleNullable = new Func<T, Single?>[_max + 1];
            var _propertyGetterDecimalNullable = new Func<T, Decimal?>[_max + 1];
            var _propertyGetterDoubleNullable = new Func<T, Double?>[_max + 1];
            var _propertyGetterUInt16Nullable = new Func<T, UInt16?>[_max + 1];
            var _propertyGetterUInt32Nullable = new Func<T, UInt32?>[_max + 1];
            var _propertyGetterUInt64Nullable = new Func<T, UInt64?>[_max + 1];
            var _propertyGetterBigIntegerNullable = new Func<T, BigInteger?>[_max + 1];

            InitCustomCsvParseArrays(_max + 1);

            InitCsvAttributeWrite(_type, _max + 1);

            foreach (var property in _properties)
            {
                int _index = property.Index;
                Type _propertyType = property.Property.PropertyType;

                _propertiesInfo[_index] = property.Property;

                _isNullable[property.Index] = Nullable.GetUnderlyingType(_propertyType) != null;
                _isAssigned[property.Index] = true;

                if (property.Attrib.CustomParserType != null)
                {
                    SetCustomParserType(property.Index, property.Attrib.CustomParserType, property.Property.Name);
                    __CustomParserCall[property.Index] = DelegateFactory.InstanceMethod(property.Attrib.CustomParserType, "Write", property.Property.PropertyType);
                }

                if (_propertyType == typeof(string))
                {
                    _propertyGetterString[_index] = DelegateFactory.PropertyGet<T, string>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.String;
                }
                else if (_propertyType == typeof(Guid))
                {
                    _propertyGetterGuid[_index] = DelegateFactory.PropertyGet<T, Guid>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.Guid;
                }
                else if (_propertyType == typeof(Boolean))
                {
                    _propertyGetterBoolean[_index] = DelegateFactory.PropertyGet<T, Boolean>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.Boolean;
                }
                else if (_propertyType == typeof(DateTime))
                {
                    _propertyGetterDateTime[_index] = DelegateFactory.PropertyGet<T, DateTime>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.DateTime;
                }
                else if (_propertyType == typeof(DateTimeOffset))
                {
                    _propertyGetterDateTimeOffset[_index] = DelegateFactory.PropertyGet<T, DateTimeOffset>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.DateTimeOffset;
                }
                else if (_propertyType == typeof(TimeSpan))
                {
                    _propertyGetterTimeSpan[_index] = DelegateFactory.PropertyGet<T, TimeSpan>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.TimeSpan;
                }
                else if (_propertyType == typeof(Byte))
                {
                    _propertyGetterByte[_index] = DelegateFactory.PropertyGet<T, Byte>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.Byte;
                }
                else if (_propertyType == typeof(SByte))
                {
                    _propertyGetterSByte[_index] = DelegateFactory.PropertyGet<T, SByte>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.SByte;
                }
                else if (_propertyType == typeof(Int16))
                {
                    _propertyGetterInt16[_index] = DelegateFactory.PropertyGet<T, Int16>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.Int16;
                }
                else if (_propertyType == typeof(Int32))
                {
                    _propertyGetterInt32[_index] = DelegateFactory.PropertyGet<T, Int32>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.Int32;
                }
                else if (_propertyType == typeof(Int64))
                {
                    _propertyGetterInt64[_index] = DelegateFactory.PropertyGet<T, Int64>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.Int64;
                }
                else if (_propertyType == typeof(Single))
                {
                    _propertyGetterSingle[_index] = DelegateFactory.PropertyGet<T, Single>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.Single;
                }
                else if (_propertyType == typeof(Decimal))
                {
                    _propertyGetterDecimal[_index] = DelegateFactory.PropertyGet<T, Decimal>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.Decimal;
                }
                else if (_propertyType == typeof(Double))
                {
                    _propertyGetterDouble[_index] = DelegateFactory.PropertyGet<T, Double>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.Double;
                }
                else if (_propertyType == typeof(UInt16))
                {
                    _propertyGetterUInt16[_index] = DelegateFactory.PropertyGet<T, UInt16>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.UInt16;
                }
                else if (_propertyType == typeof(UInt32))
                {
                    _propertyGetterUInt32[_index] = DelegateFactory.PropertyGet<T, UInt32>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.UInt32;
                }
                else if (_propertyType == typeof(UInt64))
                {
                    _propertyGetterUInt64[_index] = DelegateFactory.PropertyGet<T, UInt64>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.UInt64;
                }
                else if (_propertyType == typeof(BigInteger))
                {
                    _propertyGetterBigInteger[_index] = DelegateFactory.PropertyGet<T, BigInteger>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.BigInteger;
                }
                else if (_propertyType == typeof(Guid?))
                {
                    _propertyGetterGuidNullable[_index] = DelegateFactory.PropertyGet<T, Guid?>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.GuidNullable;
                }
                else if (_propertyType == typeof(Boolean?))
                {
                    _propertyGetterBooleanNullable[_index] = DelegateFactory.PropertyGet<T, Boolean?>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.BooleanNullable;
                }
                else if (_propertyType == typeof(DateTime?))
                {
                    _propertyGetterDateTimeNullable[_index] = DelegateFactory.PropertyGet<T, DateTime?>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.DateTimeNullable;
                }
                else if (_propertyType == typeof(DateTimeOffset?))
                {
                    _propertyGetterDateTimeOffsetNullable[_index] = DelegateFactory.PropertyGet<T, DateTimeOffset?>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.DateTimeOffsetNullable;
                }
                else if (_propertyType == typeof(TimeSpan?))
                {
                    _propertyGetterTimeSpanNullable[_index] = DelegateFactory.PropertyGet<T, TimeSpan?>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.TimeSpanNullable;
                }
                else if (_propertyType == typeof(Byte?))
                {
                    _propertyGetterByteNullable[_index] = DelegateFactory.PropertyGet<T, Byte?>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.ByteNullable;
                }
                else if (_propertyType == typeof(SByte?))
                {
                    _propertyGetterSByteNullable[_index] = DelegateFactory.PropertyGet<T, SByte?>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.SByteNullable;
                }
                else if (_propertyType == typeof(Int16?))
                {
                    _propertyGetterInt16Nullable[_index] = DelegateFactory.PropertyGet<T, Int16?>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.Int16Nullable;
                }
                else if (_propertyType == typeof(Int32?))
                {
                    _propertyGetterInt32Nullable[_index] = DelegateFactory.PropertyGet<T, Int32?>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.Int32Nullable;
                }
                else if (_propertyType == typeof(Int64?))
                {
                    _propertyGetterInt64Nullable[_index] = DelegateFactory.PropertyGet<T, Int64?>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.Int64Nullable;
                }
                else if (_propertyType == typeof(Single?))
                {
                    _propertyGetterSingleNullable[_index] = DelegateFactory.PropertyGet<T, Single?>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.SingleNullable;
                }
                else if (_propertyType == typeof(Decimal?))
                {
                    _propertyGetterDecimalNullable[_index] = DelegateFactory.PropertyGet<T, Decimal?>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.DecimalNullable;
                }
                else if (_propertyType == typeof(Double?))
                {
                    _propertyGetterDoubleNullable[_index] = DelegateFactory.PropertyGet<T, Double?>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.DoubleNullable;
                }
                else if (_propertyType == typeof(UInt16?))
                {
                    _propertyGetterUInt16Nullable[_index] = DelegateFactory.PropertyGet<T, UInt16?>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.UInt16Nullable;
                }
                else if (_propertyType == typeof(UInt32?))
                {
                    _propertyGetterUInt32Nullable[_index] = DelegateFactory.PropertyGet<T, UInt32?>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.UInt32Nullable;
                }
                else if (_propertyType == typeof(UInt64?))
                {
                    _propertyGetterUInt64Nullable[_index] = DelegateFactory.PropertyGet<T, UInt64?>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.UInt64Nullable;
                }
                else if (_propertyType == typeof(BigInteger?))
                {
                    _propertyGetterBigIntegerNullable[_index] = DelegateFactory.PropertyGet<T, BigInteger?>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.BigIntegerNullable;
                }
                else if (_propertyType == typeof(byte[]))
                {
                    _propertyGetterByteArray[_index] = DelegateFactory.PropertyGet<T, object>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.ByteArray;
                }
                else if (_propertyType.IsEnum)
                {
                    _propertyGetterEnum[_index] = DelegateFactory.PropertyGet<T, int>(property.Property.Name);
                    _propertyTypes[_index] = NetTypeComplete.Enum;
                }
                else
                {
                    throw new CsvException($"Property: {property.Property} Type: {property.Property.Name} not supported.");
                }
            }

            var _formatters = new string[_max + 1];

            Dictionary<int, string> _formattersDict = _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => new { Value = p, Key = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index, (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).OutputFormat })
                .ToDictionary(p => p.Key, p => p.OutputFormat);

            foreach (int key in _formattersDict.Keys)
            {
                _formatters[key] = _formatters[key];
            }

            var _outputNullValues = new string[_max + 1];

            Dictionary<int, string> _outputNullValuesDict = _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => new { Value = p, Key = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index, (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).OutputNullValue })
                .ToDictionary(p => p.Key, p => p.OutputNullValue);

            foreach (int key in _outputNullValuesDict.Keys)
            {
                _outputNullValues[key] = _outputNullValuesDict[key];
            }

            _Properties = _propertiesInfo.ToImmutableArray();
            _IsNullable = _isNullable.ToImmutableArray();
            _IsAssigned = _isAssigned.ToImmutableArray();

            _PropertyGetterEnum = _propertyGetterEnum.ToImmutableArray();
            _PropertyGetterByteArray = _propertyGetterByteArray.ToImmutableArray();
            _PropertyGetterString = _propertyGetterString.ToImmutableArray();
            _PropertyGetterGuid = _propertyGetterGuid.ToImmutableArray();
            _PropertyGetterBoolean = _propertyGetterBoolean.ToImmutableArray();
            _PropertyGetterDateTime = _propertyGetterDateTime.ToImmutableArray();
            _PropertyGetterDateTimeOffset = _propertyGetterDateTimeOffset.ToImmutableArray();
            _PropertyGetterTimeSpan = _propertyGetterTimeSpan.ToImmutableArray();
            _PropertyGetterByte = _propertyGetterByte.ToImmutableArray();
            _PropertyGetterSByte = _propertyGetterSByte.ToImmutableArray();
            _PropertyGetterInt16 = _propertyGetterInt16.ToImmutableArray();
            _PropertyGetterInt32 = _propertyGetterInt32.ToImmutableArray();
            _PropertyGetterInt64 = _propertyGetterInt64.ToImmutableArray();
            _PropertyGetterSingle = _propertyGetterSingle.ToImmutableArray();
            _PropertyGetterDecimal = _propertyGetterDecimal.ToImmutableArray();
            _PropertyGetterDouble = _propertyGetterDouble.ToImmutableArray();
            _PropertyGetterUInt16 = _propertyGetterUInt16.ToImmutableArray();
            _PropertyGetterUInt32 = _propertyGetterUInt32.ToImmutableArray();
            _PropertyGetterUInt64 = _propertyGetterUInt64.ToImmutableArray();
            _PropertyGetterBigInteger = _propertyGetterBigInteger.ToImmutableArray();

            _PropertyGetterGuidNullable = _propertyGetterGuidNullable.ToImmutableArray();
            _PropertyGetterBooleanNullable = _propertyGetterBooleanNullable.ToImmutableArray();
            _PropertyGetterDateTimeNullable = _propertyGetterDateTimeNullable.ToImmutableArray();
            _PropertyGetterDateTimeOffsetNullable = _propertyGetterDateTimeOffsetNullable.ToImmutableArray();
            _PropertyGetterTimeSpanNullable = _propertyGetterTimeSpanNullable.ToImmutableArray();
            _PropertyGetterByteNullable = _propertyGetterByteNullable.ToImmutableArray();
            _PropertyGetterSByteNullable = _propertyGetterSByteNullable.ToImmutableArray();
            _PropertyGetterInt16Nullable = _propertyGetterInt16Nullable.ToImmutableArray();
            _PropertyGetterInt32Nullable = _propertyGetterInt32Nullable.ToImmutableArray();
            _PropertyGetterInt64Nullable = _propertyGetterInt64Nullable.ToImmutableArray();
            _PropertyGetterSingleNullable = _propertyGetterSingleNullable.ToImmutableArray();
            _PropertyGetterDecimalNullable = _propertyGetterDecimalNullable.ToImmutableArray();
            _PropertyGetterDoubleNullable = _propertyGetterDoubleNullable.ToImmutableArray();
            _PropertyGetterUInt16Nullable = _propertyGetterUInt16Nullable.ToImmutableArray();
            _PropertyGetterUInt32Nullable = _propertyGetterUInt32Nullable.ToImmutableArray();
            _PropertyGetterUInt64Nullable = _propertyGetterUInt64Nullable.ToImmutableArray();
            _PropertyGetterBigIntegerNullable = _propertyGetterBigIntegerNullable.ToImmutableArray();

            _PropertyTypes = _propertyTypes.ToImmutableArray();

            _Formatters = _formatters.ToImmutableArray();
            _OutputNullValues = _outputNullValues.ToImmutableArray();

            base.InitImmutableArray();
        }

        private void InitCsvAttributeWrite(Type type, int size)
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
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeString, "Write", typeof(string));
                }
                if (_CsvAttribute.DefaultCustomParserTypeGuid != null)
                {
                    __CustomParserGuid[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeGuid) as ICustomCsvParse<Guid>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeGuid, "Write", typeof(Guid));
                }
                if (_CsvAttribute.DefaultCustomParserTypeBoolean != null)
                {
                    __CustomParserBoolean[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeBoolean) as ICustomCsvParse<Boolean>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBoolean, "Write", typeof(Boolean));
                }
                if (_CsvAttribute.DefaultCustomParserTypeDateTime != null)
                {
                    __CustomParserDateTime[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDateTime) as ICustomCsvParse<DateTime>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTime, "Write", typeof(DateTime));
                }
                if (_CsvAttribute.DefaultCustomParserTypeDateTimeOffset != null)
                {
                    __CustomParserDateTimeOffset[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDateTimeOffset) as ICustomCsvParse<DateTimeOffset>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTimeOffset, "Write", typeof(DateTimeOffset));
                }
                if (_CsvAttribute.DefaultCustomParserTypeTimeSpan != null)
                {
                    __CustomParserTimeSpan[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeTimeSpan) as ICustomCsvParse<TimeSpan>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeTimeSpan, "Write", typeof(TimeSpan));
                }
                if (_CsvAttribute.DefaultCustomParserTypeByte != null)
                {
                    __CustomParserByte[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeByte) as ICustomCsvParse<Byte>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeByte, "Write", typeof(Byte));
                }
                if (_CsvAttribute.DefaultCustomParserTypeSByte != null)
                {
                    __CustomParserSByte[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeSByte) as ICustomCsvParse<SByte>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSByte, "Write", typeof(SByte));
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt16 != null)
                {
                    __CustomParserInt16[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt16) as ICustomCsvParse<Int16>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt16, "Write", typeof(Int16));
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt32 != null)
                {
                    __CustomParserInt32[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt32) as ICustomCsvParse<Int32>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt32, "Write", typeof(Int32));
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt64 != null)
                {
                    __CustomParserInt64[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt64) as ICustomCsvParse<Int64>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt64, "Write", typeof(Int64));
                }
                if (_CsvAttribute.DefaultCustomParserTypeSingle != null)
                {
                    __CustomParserSingle[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeSingle) as ICustomCsvParse<Single>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSingle, "Write", typeof(Single));
                }
                if (_CsvAttribute.DefaultCustomParserTypeDecimal != null)
                {
                    __CustomParserDecimal[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDecimal) as ICustomCsvParse<Decimal>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDecimal, "Write", typeof(Decimal));
                }
                if (_CsvAttribute.DefaultCustomParserTypeDouble != null)
                {
                    __CustomParserDouble[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDouble) as ICustomCsvParse<Double>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDouble, "Write", typeof(Double));
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt16 != null)
                {
                    __CustomParserUInt16[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt16) as ICustomCsvParse<UInt16>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt16, "Write", typeof(UInt16));
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt32 != null)
                {
                    __CustomParserUInt32[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt32) as ICustomCsvParse<UInt32>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt32, "Write", typeof(UInt32));
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt64 != null)
                {
                    __CustomParserUInt64[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt64) as ICustomCsvParse<UInt64>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt64, "Write", typeof(UInt64));
                }
                if (_CsvAttribute.DefaultCustomParserTypeBigInteger != null)
                {
                    __CustomParserBigInteger[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeBigInteger) as ICustomCsvParse<BigInteger>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBigInteger, "Write", typeof(BigInteger));
                }
                if (_CsvAttribute.DefaultCustomParserTypeGuidNullable != null)
                {
                    __CustomParserGuidNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeGuidNullable) as ICustomCsvParse<Guid?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeGuidNullable, "Write", typeof(Guid?));
                }
                if (_CsvAttribute.DefaultCustomParserTypeBooleanNullable != null)
                {
                    __CustomParserBooleanNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeBooleanNullable) as ICustomCsvParse<Boolean?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBooleanNullable, "Write", typeof(Boolean?));
                }
                if (_CsvAttribute.DefaultCustomParserTypeDateTimeNullable != null)
                {
                    __CustomParserDateTimeNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDateTimeNullable) as ICustomCsvParse<DateTime?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTimeNullable, "Write", typeof(DateTime?));
                }
                if (_CsvAttribute.DefaultCustomParserTypeDateTimeOffsetNullable != null)
                {
                    __CustomParserDateTimeOffsetNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDateTimeOffsetNullable) as ICustomCsvParse<DateTimeOffset?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTimeOffsetNullable, "Write", typeof(DateTimeOffset?));
                }
                if (_CsvAttribute.DefaultCustomParserTypeTimeSpanNullable != null)
                {
                    __CustomParserTimeSpanNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeTimeSpanNullable) as ICustomCsvParse<TimeSpan?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeTimeSpanNullable, "Write", typeof(TimeSpan?));
                }
                if (_CsvAttribute.DefaultCustomParserTypeByteNullable != null)
                {
                    __CustomParserByteNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeByteNullable) as ICustomCsvParse<Byte?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeByteNullable, "Write", typeof(Byte?));
                }
                if (_CsvAttribute.DefaultCustomParserTypeSByteNullable != null)
                {
                    __CustomParserSByteNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeSByteNullable) as ICustomCsvParse<SByte?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSByteNullable, "Write", typeof(SByte?));
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt16Nullable != null)
                {
                    __CustomParserInt16Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt16Nullable) as ICustomCsvParse<Int16?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt16Nullable, "Write", typeof(Int16?));
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt32Nullable != null)
                {
                    __CustomParserInt32Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt32Nullable) as ICustomCsvParse<Int32?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt32Nullable, "Write", typeof(Int32?));
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt64Nullable != null)
                {
                    __CustomParserInt64Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt64Nullable) as ICustomCsvParse<Int64?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt64Nullable, "Write", typeof(Int64?));
                }
                if (_CsvAttribute.DefaultCustomParserTypeSingleNullable != null)
                {
                    __CustomParserSingleNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeSingleNullable) as ICustomCsvParse<Single?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSingleNullable, "Write", typeof(Single?));
                }
                if (_CsvAttribute.DefaultCustomParserTypeDecimalNullable != null)
                {
                    __CustomParserDecimalNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDecimalNullable) as ICustomCsvParse<Decimal?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDecimalNullable, "Write", typeof(Decimal?));
                }
                if (_CsvAttribute.DefaultCustomParserTypeDoubleNullable != null)
                {
                    __CustomParserDoubleNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDoubleNullable) as ICustomCsvParse<Double?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDoubleNullable, "Write", typeof(Double?));
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt16Nullable != null)
                {
                    __CustomParserUInt16Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt16Nullable) as ICustomCsvParse<UInt16?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt16Nullable, "Write", typeof(UInt16?));
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt32Nullable != null)
                {
                    __CustomParserUInt32Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt32Nullable) as ICustomCsvParse<UInt32?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt32Nullable, "Write", typeof(UInt32?));
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt64Nullable != null)
                {
                    __CustomParserUInt64Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt64Nullable) as ICustomCsvParse<UInt64?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt64Nullable, "Write", typeof(UInt64?));
                }
                if (_CsvAttribute.DefaultCustomParserTypeBigIntegerNullable != null)
                {
                    __CustomParserBigIntegerNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeBigIntegerNullable) as ICustomCsvParse<BigInteger?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBigIntegerNullable, "Write", typeof(BigInteger?));
                }
            }
        }
    }
}