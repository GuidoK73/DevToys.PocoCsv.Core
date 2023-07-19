using Delegates;
using System;
using System.Collections.Generic;
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
        private NetTypeComplete[] _PropertyTypes = new NetTypeComplete[0];
        
        private Func<T, object>[] _PropertyGetterByteArray = new Func<T, object>[0];
        private Func<T, string>[] _PropertyGetterString = new Func<T, string>[0];
        private Func<T, Guid>[] _PropertyGetterGuid = new Func<T, Guid>[0];
        private Func<T, Boolean>[] _PropertyGetterBoolean = new Func<T, Boolean>[0];
        private Func<T, DateTime>[] _PropertyGetterDateTime = new Func<T, DateTime>[0];
        private Func<T, DateTimeOffset>[] _PropertyGetterDateTimeOffset = new Func<T, DateTimeOffset>[0];
        private Func<T, TimeSpan>[] _PropertyGetterTimeSpan = new Func<T, TimeSpan>[0];
        private Func<T, Byte>[] _PropertyGetterByte = new Func<T, Byte>[0];
        private Func<T, SByte>[] _PropertyGetterSByte = new Func<T, SByte>[0];
        private Func<T, Int16>[] _PropertyGetterInt16 = new Func<T, Int16>[0];
        private Func<T, Int32>[] _PropertyGetterInt32 = new Func<T, Int32>[0];
        private Func<T, Int64>[] _PropertyGetterInt64 = new Func<T, Int64>[0];
        private Func<T, Single>[] _PropertyGetterSingle = new Func<T, Single>[0];
        private Func<T, Decimal>[] _PropertyGetterDecimal = new Func<T, Decimal>[0];
        private Func<T, Double>[] _PropertyGetterDouble = new Func<T, Double>[0];
        private Func<T, UInt16>[] _PropertyGetterUInt16 = new Func<T, UInt16>[0];
        private Func<T, UInt32>[] _PropertyGetterUInt32 = new Func<T, UInt32>[0];
        private Func<T, UInt64>[] _PropertyGetterUInt64 = new Func<T, UInt64>[0];
        private Func<T, BigInteger>[] _PropertyGetterBigInteger = new Func<T, BigInteger>[0];

        private Func<T, Guid?>[] _PropertyGetterGuidNullable = new Func<T, Guid?>[0];
        private Func<T, Boolean?>[] _PropertyGetterBooleanNullable = new Func<T, Boolean?>[0];
        private Func<T, DateTime?>[] _PropertyGetterDateTimeNullable = new Func<T, DateTime?>[0];
        private Func<T, DateTimeOffset?>[] _PropertyGetterDateTimeOffsetNullable = new Func<T, DateTimeOffset?>[0];
        private Func<T, TimeSpan?>[] _PropertyGetterTimeSpanNullable = new Func<T, TimeSpan?>[0];
        private Func<T, Byte?>[] _PropertyGetterByteNullable = new Func<T, Byte?>[0];
        private Func<T, SByte?>[] _PropertyGetterSByteNullable = new Func<T, SByte?>[0];
        private Func<T, Int16?>[] _PropertyGetterInt16Nullable = new Func<T, Int16?>[0];
        private Func<T, Int32?>[] _PropertyGetterInt32Nullable = new Func<T, Int32?>[0];
        private Func<T, Int64?>[] _PropertyGetterInt64Nullable = new Func<T, Int64?>[0];
        private Func<T, Single?>[] _PropertyGetterSingleNullable = new Func<T, Single?>[0];
        private Func<T, Decimal?>[] _PropertyGetterDecimalNullable = new Func<T, Decimal?>[0];
        private Func<T, Double?>[] _PropertyGetterDoubleNullable = new Func<T, Double?>[0];
        private Func<T, UInt16?>[] _PropertyGetterUInt16Nullable = new Func<T, UInt16?>[0];
        private Func<T, UInt32?>[] _PropertyGetterUInt32Nullable = new Func<T, UInt32?>[0];
        private Func<T, UInt64?>[] _PropertyGetterUInt64Nullable = new Func<T, UInt64?>[0];
        private Func<T, BigInteger?>[] _PropertyGetterBigIntegerNullable = new Func<T, BigInteger?>[0];

        private string[] _Formatters = new string[0];
        private string[] _OutputNullValues = new string[0];
        private StreamWriter _StreamWriter;
        private int _MaxColumnIndex = 0;

        private const char _CR = '\r';
        private const char _LF = '\n';
        private const string _CRLF = "\r\n";

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvWriter(string file)
        {
            _File = file;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvWriter(Stream stream)
        {
            _Stream = stream;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvWriter(string file, Encoding encoding, CultureInfo culture, char separator = ',', int buffersize = -1)
        {
            Culture = culture;
            Separator = separator;
            Encoding = encoding;
            _BufferSize = buffersize;
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
            _BufferSize = buffersize;
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

                    if (targetType == typeof(String))
                    {
                        WriteString(row, index);
                    }
                    else
                    {
                        WriteValueOthers(targetType, row, index, _formatter, _outputNullValue);
                    }
                }
                if (index < _MaxColumnIndex)
                {
                    _StreamWriter.Write(Separator);
                }
            }
        }


        private void WriteValueOthers(Type targetType, T row, int index, string formatter, string outputNullValue)
        {
            if (!_IsNullable[index])
            {
                WriteValueOthersNormal(targetType, row, index, formatter, outputNullValue);
            }
            else
            {
                WriteValueOthersNormalNullable(targetType, row, index, formatter, outputNullValue);
            }
        }

        private void WriteValueOthersNormalNullable(Type targetType, T row, int index, string formatter, string outputNullValue)
        {
            if (targetType == typeof(Guid?))
            {
                WriteGuidNull(row, index, outputNullValue, formatter);
            }
            else if (targetType == typeof(Boolean?))
            {
                WriteBooleanNull(row, index, outputNullValue);
            }
            else if (targetType == typeof(DateTime?))
            {
                WriteDateTimeNull(row, index, outputNullValue, formatter);
            }
            else if (targetType == typeof(DateTimeOffset?))
            {
                WriteDateTimeOffsetNull(row, index, outputNullValue, formatter);
            }
            else if (targetType == typeof(TimeSpan?))
            {
                WriteTimeSpanNull(row, index, outputNullValue, formatter);
            }
            else if (targetType == typeof(Byte?))
            {
                WriteByteNull(row, index, outputNullValue, formatter);
            }
            else if (targetType == typeof(SByte?))
            {
                WriteSByteNull(row, index, outputNullValue, formatter);
            }
            else if (targetType == typeof(Int16?))
            {
                WriteInt16Null(row, index, outputNullValue, formatter);
            }
            else if (targetType == typeof(Int32?))
            {
                WriteInt32Null(row, index, outputNullValue, formatter);
            }
            else if (targetType == typeof(Int64?))
            {
                WriteInt64Null(row, index, outputNullValue, formatter);
            }
            else if (targetType == typeof(Single?))
            {
                WriteSingleNull(row, index, outputNullValue, formatter);
            }
            else if (targetType == typeof(Decimal?))
            {
                WriteDecimalNull(row, index, outputNullValue, formatter);
            }
            else if (targetType == typeof(Double?))
            {
                WriteDoubleNull(row, index, outputNullValue, formatter);
            }
            else if (targetType == typeof(UInt16?))
            {
                WriteUInt16Null(row, index, outputNullValue, formatter);
            }
            else if (targetType == typeof(UInt32?))
            {
                WriteUInt32Null(row, index, outputNullValue, formatter);
            }
            else if (targetType == typeof(UInt64?))
            {
                WriteUInt64Null(row, index, outputNullValue, formatter);
            }
            else if (targetType == typeof(BigInteger?))
            {
                WriteBigIntegerNull(row, index, outputNullValue, formatter);
            }
        }

        private void WriteValueOthersNormal(Type targetType, T row, int index, string formatter, string outputNullValue)
        {
            if (targetType == typeof(Guid))
            {
                WriteGuid(row, index, formatter);
            }
            else if (targetType == typeof(Boolean))
            {
                WriteBoolean(row, index);
            }
            else if (targetType == typeof(DateTime))
            {
                WriteDateTime(row, index, formatter);
            }
            else if (targetType == typeof(DateTimeOffset))
            {
                WriteDateTimeOffset(row, index, formatter);
            }
            else if (targetType == typeof(TimeSpan))
            {
                WriteTimeSpan(row, index, formatter);
            }
            else if (targetType == typeof(Byte))
            {
                WriteByte(row, index, formatter);
            }
            else if (targetType == typeof(SByte))
            {
                WriteSByte(row, index, formatter);
            }
            else if (targetType == typeof(Int16))
            {
                WriteInt16(row, index, formatter);
            }
            else if (targetType == typeof(Int32))
            {
                WriteInt32(row, index, formatter);
            }
            else if (targetType == typeof(Int64))
            {
                WriteInt64(row, index, formatter);
            }
            else if (targetType == typeof(Single))
            {
                WriteSingle(row, index, formatter);
            }
            else if (targetType == typeof(Decimal))
            {
                WriteDecimal(row, index, formatter);
            }
            else if (targetType == typeof(Double))
            {
                WriteDouble(row, index, formatter);
            }
            else if (targetType == typeof(UInt16))
            {
                WriteUInt16(row, index, formatter);
            }
            else if (targetType == typeof(UInt32))
            {
                WriteUInt32(row, index, formatter);
            }
            else if (targetType == typeof(UInt64))
            {
                WriteUInt64(row, index, formatter);
            }
            else if (targetType == typeof(BigInteger))
            {
                WriteBigInteger(row, index, formatter);
            }
            else if (targetType == typeof(byte[]))
            {
                WriteByteArray(row, index);
            }
        }

        private void WriteString(T row, int index)
        {
            string _value = _PropertyGetterString[index](row);
            if (_value != null)
            {
                _StreamWriter.Write(CsvUtils.Esc(_Separator, _value.ToString()));
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



        /// <summary>
        /// Initialize and open the CSV Stream Writer.
        /// </summary>
        public void Open()
        {
            Init();
            if (_Stream != null)
            {
                _StreamWriter = new StreamWriter(stream: _Stream, encoding: Encoding, bufferSize: _BufferSize);
            }
            if (!string.IsNullOrEmpty(_File))
            {
                _StreamWriter = new StreamWriter(path: _File, append: true, encoding: Encoding, bufferSize: _BufferSize);
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

            //Dictionary<int, PropertyInfo> _properties = _type.GetProperties()
            //    .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
            //    .Select(p => new { Value = p, Key = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index })
            //   .ToDictionary(p => p.Key, p => p.Value);

            var _properties = _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => new { Property = p, Index = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index, Attrib = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute) });


            int _max = _properties.Max(p => p.Index);
            _MaxColumnIndex = _max;

            _Properties = new PropertyInfo[_max + 1];

            _PropertyTypes = new NetTypeComplete[_max + 1];

            _IsNullable = new Boolean[_max + 1];


            _PropertyGetterByteArray = new Func<T, object>[_max + 1];
            _PropertyGetterString = new Func<T, string>[_max + 1];
            _PropertyGetterGuid = new Func<T, Guid>[_max + 1];
            _PropertyGetterBoolean = new Func<T, Boolean>[_max + 1];
            _PropertyGetterDateTime = new Func<T, DateTime>[_max + 1];
            _PropertyGetterDateTimeOffset = new Func<T, DateTimeOffset>[_max + 1];
            _PropertyGetterTimeSpan = new Func<T, TimeSpan>[_max + 1];
            _PropertyGetterByte = new Func<T, Byte>[_max + 1];
            _PropertyGetterSByte = new Func<T, SByte>[_max + 1];
            _PropertyGetterInt16 = new Func<T, Int16>[_max + 1];
            _PropertyGetterInt32 = new Func<T, Int32>[_max + 1];
            _PropertyGetterInt64 = new Func<T, Int64>[_max + 1];
            _PropertyGetterSingle = new Func<T, Single>[_max + 1];
            _PropertyGetterDecimal = new Func<T, Decimal>[_max + 1];
            _PropertyGetterDouble = new Func<T, Double>[_max + 1];
            _PropertyGetterUInt16 = new Func<T, UInt16>[_max + 1];
            _PropertyGetterUInt32 = new Func<T, UInt32>[_max + 1];
            _PropertyGetterUInt64 = new Func<T, UInt64>[_max + 1];
            _PropertyGetterBigInteger = new Func<T, BigInteger>[_max + 1];

            _PropertyGetterGuidNullable = new Func<T, Guid?>[_max + 1];
            _PropertyGetterBooleanNullable = new Func<T, Boolean?>[_max + 1];
            _PropertyGetterDateTimeNullable = new Func<T, DateTime?>[_max + 1];
            _PropertyGetterDateTimeOffsetNullable = new Func<T, DateTimeOffset?>[_max + 1];
            _PropertyGetterTimeSpanNullable = new Func<T, TimeSpan?>[_max + 1];
            _PropertyGetterByteNullable = new Func<T, Byte?>[_max + 1];
            _PropertyGetterSByteNullable = new Func<T, SByte?>[_max + 1];
            _PropertyGetterInt16Nullable = new Func<T, Int16?>[_max + 1];
            _PropertyGetterInt32Nullable = new Func<T, Int32?>[_max + 1];
            _PropertyGetterInt64Nullable = new Func<T, Int64?>[_max + 1];
            _PropertyGetterSingleNullable = new Func<T, Single?>[_max + 1];
            _PropertyGetterDecimalNullable = new Func<T, Decimal?>[_max + 1];
            _PropertyGetterDoubleNullable = new Func<T, Double?>[_max + 1];
            _PropertyGetterUInt16Nullable = new Func<T, UInt16?>[_max + 1];
            _PropertyGetterUInt32Nullable = new Func<T, UInt32?>[_max + 1];
            _PropertyGetterUInt64Nullable = new Func<T, UInt64?>[_max + 1];
            _PropertyGetterBigIntegerNullable = new Func<T, BigInteger?>[_max + 1];

            InitCustomCsvParseArrays(_max + 1);

            InitCsvAttribute(_type, _max + 1, ReadOrWrite.Write);


            foreach (var property in _properties)
            {
                int _index = property.Index;
                Type _propertyType = property.Property.PropertyType;

                _Properties[_index] = property.Property;

                _IsNullable[property.Index] = Nullable.GetUnderlyingType(_propertyType) != null;

                if (property.Attrib.CustomParserType != null)
                {
                    SetCustomParserType(property.Index, property.Attrib.CustomParserType, property.Property.Name);

                    _CustomParserCall[property.Index] = DelegateFactory.InstanceMethod(property.Attrib.CustomParserType, "Write", property.Property.PropertyType);

                }

                if (_propertyType == typeof(string))
                {
                    _PropertyGetterString[_index] = DelegateFactory.PropertyGet<T, string>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.String;
                }
                if (_propertyType == typeof(Guid))
                {
                    _PropertyGetterGuid[_index] = DelegateFactory.PropertyGet<T, Guid>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.Guid;
                }
                if (_propertyType == typeof(Boolean))
                {
                    _PropertyGetterBoolean[_index] = DelegateFactory.PropertyGet<T, Boolean>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.Boolean;
                }
                if (_propertyType == typeof(DateTime))
                {
                    _PropertyGetterDateTime[_index] = DelegateFactory.PropertyGet<T, DateTime>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.DateTime;
                }
                if (_propertyType == typeof(DateTimeOffset))
                {
                    _PropertyGetterDateTimeOffset[_index] = DelegateFactory.PropertyGet<T, DateTimeOffset>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.DateTimeOffset;
                }
                if (_propertyType == typeof(TimeSpan))
                {
                    _PropertyGetterTimeSpan[_index] = DelegateFactory.PropertyGet<T, TimeSpan>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.TimeSpan;
                }
                if (_propertyType == typeof(Byte))
                {
                    _PropertyGetterByte[_index] = DelegateFactory.PropertyGet<T, Byte>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.Byte;
                }
                if (_propertyType == typeof(SByte))
                {
                    _PropertyGetterSByte[_index] = DelegateFactory.PropertyGet<T, SByte>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.SByte;
                }
                if (_propertyType == typeof(Int16))
                {
                    _PropertyGetterInt16[_index] = DelegateFactory.PropertyGet<T, Int16>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.Int16;
                }
                if (_propertyType == typeof(Int32))
                {
                    _PropertyGetterInt32[_index] = DelegateFactory.PropertyGet<T, Int32>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.Int32;
                }
                if (_propertyType == typeof(Int64))
                {
                    _PropertyGetterInt64[_index] = DelegateFactory.PropertyGet<T, Int64>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.Int64;
                }
                if (_propertyType == typeof(Single))
                {
                    _PropertyGetterSingle[_index] = DelegateFactory.PropertyGet<T, Single>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.Single;
                }
                if (_propertyType == typeof(Decimal))
                {
                    _PropertyGetterDecimal[_index] = DelegateFactory.PropertyGet<T, Decimal>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.Decimal;
                }
                if (_propertyType == typeof(Double))
                {
                    _PropertyGetterDouble[_index] = DelegateFactory.PropertyGet<T, Double>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.Double;
                }
                if (_propertyType == typeof(UInt16))
                {
                    _PropertyGetterUInt16[_index] = DelegateFactory.PropertyGet<T, UInt16>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.UInt16;
                }
                if (_propertyType == typeof(UInt32))
                {
                    _PropertyGetterUInt32[_index] = DelegateFactory.PropertyGet<T, UInt32>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.UInt32;
                }
                if (_propertyType == typeof(UInt64))
                {
                    _PropertyGetterUInt64[_index] = DelegateFactory.PropertyGet<T, UInt64>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.UInt64;
                }
                if (_propertyType == typeof(BigInteger))
                {
                    _PropertyGetterBigInteger[_index] = DelegateFactory.PropertyGet<T, BigInteger>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.BigInteger;
                }
                if (_propertyType == typeof(Guid?))
                {
                    _PropertyGetterGuidNullable[_index] = DelegateFactory.PropertyGet<T, Guid?>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.GuidNullable;
                }
                if (_propertyType == typeof(Boolean?))
                {
                    _PropertyGetterBooleanNullable[_index] = DelegateFactory.PropertyGet<T, Boolean?>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.BooleanNullable;
                }
                if (_propertyType == typeof(DateTime?))
                {
                    _PropertyGetterDateTimeNullable[_index] = DelegateFactory.PropertyGet<T, DateTime?>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.DateTimeNullable;
                }
                if (_propertyType == typeof(DateTimeOffset?))
                {
                    _PropertyGetterDateTimeOffsetNullable[_index] = DelegateFactory.PropertyGet<T, DateTimeOffset?>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.DateTimeOffsetNullable;
                }
                if (_propertyType == typeof(TimeSpan?))
                {
                    _PropertyGetterTimeSpanNullable[_index] = DelegateFactory.PropertyGet<T, TimeSpan?>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.TimeSpanNullable;
                }
                if (_propertyType == typeof(Byte?))
                {
                    _PropertyGetterByteNullable[_index] = DelegateFactory.PropertyGet<T, Byte?>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.ByteNullable;
                }
                if (_propertyType == typeof(SByte?))
                {
                    _PropertyGetterSByteNullable[_index] = DelegateFactory.PropertyGet<T, SByte?>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.SByteNullable;
                }
                if (_propertyType == typeof(Int16?))
                {
                    _PropertyGetterInt16Nullable[_index] = DelegateFactory.PropertyGet<T, Int16?>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.Int16Nullable;
                }
                if (_propertyType == typeof(Int32?))
                {
                    _PropertyGetterInt32Nullable[_index] = DelegateFactory.PropertyGet<T, Int32?>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.Int32Nullable;
                }
                if (_propertyType == typeof(Int64?))
                {
                    _PropertyGetterInt64Nullable[_index] = DelegateFactory.PropertyGet<T, Int64?>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.Int64Nullable;
                }
                if (_propertyType == typeof(Single?))
                {
                    _PropertyGetterSingleNullable[_index] = DelegateFactory.PropertyGet<T, Single?>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.SingleNullable;
                }
                if (_propertyType == typeof(Decimal?))
                {
                    _PropertyGetterDecimalNullable[_index] = DelegateFactory.PropertyGet<T, Decimal?>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.DecimalNullable;
                }
                if (_propertyType == typeof(Double?))
                {
                    _PropertyGetterDoubleNullable[_index] = DelegateFactory.PropertyGet<T, Double?>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.DoubleNullable;
                }
                if (_propertyType == typeof(UInt16?))
                {
                    _PropertyGetterUInt16Nullable[_index] = DelegateFactory.PropertyGet<T, UInt16?>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.UInt16Nullable;
                }
                if (_propertyType == typeof(UInt32?))
                {
                    _PropertyGetterUInt32Nullable[_index] = DelegateFactory.PropertyGet<T, UInt32?>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.UInt32Nullable;
                }
                if (_propertyType == typeof(UInt64?))
                {
                    _PropertyGetterUInt64Nullable[_index] = DelegateFactory.PropertyGet<T, UInt64?>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.UInt64Nullable;
                }
                if (_propertyType == typeof(BigInteger?))
                {
                    _PropertyGetterBigIntegerNullable[_index] = DelegateFactory.PropertyGet<T, BigInteger?>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.BigIntegerNullable;
                }
                if (_propertyType == typeof(byte[]))
                {
                    _PropertyGetterByteArray[_index] = DelegateFactory.PropertyGet<T, object>(property.Property.Name);
                    _PropertyTypes[_index] = NetTypeComplete.ByteArray;
                }
            }

            _Formatters = new string[_max + 1]; 

            Dictionary<int, string> _formatters = _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => new { Value = p, Key = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index, (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).OutputFormat })
                .ToDictionary(p => p.Key, p => p.OutputFormat);

            foreach (int key in _formatters.Keys)
            {
                _Formatters[key] = _formatters[key];
            }

            _OutputNullValues = new string[_max + 1];

            Dictionary<int, string> _outputNullValues = _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => new { Value = p, Key = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index, (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).OutputNullValue })
                .ToDictionary(p => p.Key, p => p.OutputNullValue);

            foreach (int key in _outputNullValues.Keys)
            {
                _OutputNullValues[key] = _outputNullValues[key];
            }

        }
    }
}