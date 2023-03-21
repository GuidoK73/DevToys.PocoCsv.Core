using Delegates;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Write T to Csv Stream from an IEnumerable source.
    /// </summary>
    public class CsvWriter<T> : BaseCsv, IDisposable where T : new()
    {
        private Dictionary<int, PropertyInfo> _Properties = new();
        private Dictionary<int, Func<object, object>> _PropertyGetters = new();
        private Dictionary<int, string> _Formatters = new();
        private Dictionary<int, string> _OutputNullValues = new();
        private StreamWriter _StreamWriter;
        private List<int> _ColumnIndexes;
        private int _MaxColumnIndex = 0;

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
        public virtual void Close()
        {
            _StreamWriter.Close();
        }

        /// <summary>
        /// Write header with property names of T.
        /// </summary>
        public void WriteHeader()
        {
            for (int ii = 0; ii <= _MaxColumnIndex; ii++)
            {
                if (_Properties.ContainsKey(ii))
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
            if (_StreamWriter.BaseStream.Position > 0)
            {
                _StreamWriter.Write("\r\n");
            }
            for (int ii = 0; ii <= _MaxColumnIndex; ii++)
            {
                if (_Properties.ContainsKey(ii))
                {
                    var _property = _Properties[ii];
                    var _propertyGetter = _PropertyGetters[ii];
                    var _value = _propertyGetter(row);
                    string _formatter = null;
                    string _outputNullValue = null;
                    if (_Formatters.ContainsKey(ii))
                    {
                        _formatter = _Formatters[ii];
                    }
                    if (_OutputNullValues.ContainsKey(ii))
                    {
                        _outputNullValue = _OutputNullValues[ii];
                    }

                    Type targetType = _property.PropertyType;

                    if (targetType == typeof(String))
                    {
                        if (_value != null)
                        {
                            _StreamWriter.Write(Esc(_value.ToString()));
                        }
                        else
                        {
                            _StreamWriter.Write(string.Empty);
                        }
                    }
                    else if (targetType == typeof(Decimal))
                    {
                        WriteDecimal((Decimal)_value, _formatter);
                    }
                    else if (targetType == typeof(Double))
                    {
                        WriteDouble((Double)_value, _formatter);
                    }
                    else if (targetType == typeof(Int32))
                    {
                        WriteInt32((Int32)_value, _formatter);
                    }
                    else if (targetType == typeof(Int64))
                    {
                        WriteInt64((Int64)_value, _formatter);
                    }
                    else if (targetType == typeof(Single))
                    {
                        WriteSingle((Single)_value, _formatter);
                    }
                    else if (targetType == typeof(byte[]))
                    {
                        var _byteVakye = (byte[])_value;
                        _StreamWriter.Write(Convert.ToBase64String(_byteVakye));
                    }
                    else if (targetType == typeof(DateTime))
                    {
                        WriteDateTime((DateTime)_value, _formatter);
                    }
                    else if (targetType == typeof(DateTimeOffset))
                    {
                        WriteDateTimeOffset((DateTimeOffset)_value, _formatter);
                    }
                    else if (targetType == typeof(TimeSpan))
                    {
                        WriteTimeSpan((TimeSpan)_value, _formatter);
                    }
                    else if (targetType == typeof(Guid))
                    {
                        _StreamWriter.Write(_value.ToString());
                    }
                    else if (targetType == typeof(Boolean))
                    {
                        _StreamWriter.Write(_value.ToString());
                    }
                    else if (targetType == typeof(Byte))
                    {
                        WriteByte((Byte)_value, _formatter);
                    }
                    else if (targetType == typeof(SByte))
                    {
                        WriteSByte((SByte)_value, _formatter);
                    }
                    else if (targetType == typeof(Int16))
                    {
                        WriteInt16((Int16)_value, _formatter);
                    }
                    else if (targetType == typeof(UInt16))
                    {
                        WriteUInt16((UInt16)_value, _formatter);
                    }
                    else if (targetType == typeof(UInt32))
                    {
                        WriteUInt32((UInt32)_value, _formatter);
                    }
                    else if (targetType == typeof(UInt64))
                    {
                        WriteUInt64((UInt64)_value, _formatter);
                    }
                    else if (targetType == typeof(Guid?))
                    {
                        WriteGuidNull((Guid?)_value, _outputNullValue);
                    }
                    else if (targetType == typeof(Boolean?))
                    {
                        WriteBooleanNull((Boolean?)_value, _outputNullValue);
                    }
                    else if (targetType == typeof(DateTime?))
                    {
                        WriteDateTimeNull((DateTime?)_value, _formatter, _outputNullValue);
                    }
                    else if (targetType == typeof(DateTimeOffset?))
                    {
                        WriteDateTimeOffsetNull((DateTimeOffset?)_value, _formatter, _outputNullValue);
                    }
                    else if (targetType == typeof(TimeSpan?))
                    {
                        WriteTimeSpanNull((TimeSpan?)_value, _formatter, _outputNullValue);
                    }
                    else if (targetType == typeof(Byte?))
                    {
                        WriteByteNull((Byte?)_value, _formatter, _outputNullValue);
                    }
                    else if (targetType == typeof(SByte?))
                    {
                        WriteSByteNull((SByte?)_value, _formatter, _outputNullValue);
                    }
                    else if (targetType == typeof(Int16?))
                    {
                        WriteInt16Null((Int16?)_value, _formatter, _outputNullValue);
                    }
                    else if (targetType == typeof(Int32?))
                    {
                        WriteInt32Null((Int32?)_value, _formatter, _outputNullValue);
                    }
                    else if (targetType == typeof(Int64?))
                    {
                        WriteInt64Null((Int64?)_value, _formatter, _outputNullValue);
                    }
                    else if (targetType == typeof(Single?))
                    {
                        WriteSingleNull((Single?)_value, _formatter, _outputNullValue);
                    }
                    else if (targetType == typeof(Decimal?))
                    {
                        WriteDecimalNull((Decimal?)_value, _formatter, _outputNullValue);
                    }
                    else if (targetType == typeof(Double?))
                    {
                        WriteDoubleNull((Double?)_value, _formatter, _outputNullValue);
                    }
                    else if (targetType == typeof(UInt16?))
                    {
                        WriteUInt16Null((UInt16?)_value, _formatter, _outputNullValue);
                    }
                    else if (targetType == typeof(UInt32?))
                    {
                        WriteUInt32Null((UInt32?)_value, _formatter, _outputNullValue);
                    }
                    else if (targetType == typeof(UInt64?))
                    {
                        WriteUInt64Null((UInt64?)_value, _formatter, _outputNullValue);
                    }
                    else
                    {
                        _StreamWriter.Write(TypeUtils.Convert(_value, typeof(string), Culture));
                    }
                    if (ii < _MaxColumnIndex)
                    {
                        _StreamWriter.Write(Separator);
                    }
                }
            }
        }

        private void WriteGuidNull(Guid? value, string _nullValueDefault)
        {
            if (value.HasValue)
            {
                _StreamWriter.Write(Esc(value.Value.ToString()));
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(Esc(_nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteBooleanNull(Boolean? value, string _nullValueDefault)
        {
            if (value.HasValue)
            {
                _StreamWriter.Write(value.Value.ToString());
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(Esc(_nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteDateTimeNull(DateTime? value, string formatter, string _nullValueDefault)
        {
            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(Esc(_nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteDateTimeOffsetNull(DateTimeOffset? value, string formatter, string _nullValueDefault)
        {
            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(Esc(_nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteTimeSpanNull(TimeSpan? value, string formatter, string _nullValueDefault)
        {
            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(Esc(value.Value.ToString()));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(Esc(_nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteByteNull(Byte? value, string formatter, string _nullValueDefault)
        {
            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(Esc(_nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteSByteNull(SByte? value, string formatter, string _nullValueDefault)
        {
            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(Esc(_nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteInt16Null(Int16? value, string formatter, string _nullValueDefault)
        {
            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(Esc(_nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteInt32Null(Int32? value, string formatter, string _nullValueDefault)
        {
            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(Esc(_nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteInt64Null(Int64? value, string formatter, string _nullValueDefault)
        {
            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(Esc(_nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteSingleNull(Single? value, string formatter, string _nullValueDefault)
        {
            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(Esc(_nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteDecimalNull(Decimal? value, string formatter, string _nullValueDefault)
        {
            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(Esc(_nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteDoubleNull(Double? value, string formatter, string _nullValueDefault)
        {
            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(Esc(_nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteUInt16Null(UInt16? value, string formatter, string _nullValueDefault)
        {
            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(Esc(_nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteUInt32Null(UInt32? value, string formatter, string _nullValueDefault)
        {
            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(Esc(_nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteUInt64Null(UInt64? value, string formatter, string _nullValueDefault)
        {
            if (value.HasValue)
            {
                if (!string.IsNullOrEmpty(formatter))
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(formatter, Culture)));
                }
                else
                {
                    _StreamWriter.Write(Esc(value.Value.ToString(Culture)));
                }
            }
            else if (_nullValueDefault != null)
            {
                _StreamWriter.Write(Esc(_nullValueDefault));
            }
            else
            {
                _StreamWriter.Write(string.Empty);
            }
        }

        private void WriteDecimal(Decimal value, string formatter)
        {
            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(Esc(value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(Esc(value.ToString(Culture)));
            }
        }

        private void WriteDateTime(DateTime value, string formatter)
        {
            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(Esc(value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(Esc(value.ToString(Culture)));
            }
        }

        private void WriteDateTimeOffset(DateTimeOffset value, string formatter)
        {
            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(Esc(value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(Esc(value.ToString(Culture)));
            }
        }

        private void WriteTimeSpan(TimeSpan value, string formatter)
        {
            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(Esc(value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(Esc(value.ToString()));
            }
        }

        private void WriteByte(Byte value, string formatter)
        {
            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(Esc(value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(Esc(value.ToString(Culture)));
            }
        }

        private void WriteSByte(SByte value, string formatter)
        {
            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(Esc(value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(Esc(value.ToString(Culture)));
            }
        }

        private void WriteInt16(Int16 value, string formatter)
        {
            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(Esc(value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(Esc(value.ToString(Culture)));
            }
        }

        private void WriteInt32(Int32 value, string formatter)
        {
            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(Esc(value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(Esc(value.ToString(Culture)));
            }
        }

        private void WriteInt64(Int64 value, string formatter)
        {
            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(Esc(value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(Esc(value.ToString(Culture)));
            }
        }

        private void WriteSingle(Single value, string formatter)
        {
            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(Esc(value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(Esc(value.ToString(Culture)));
            }
        }

        private void WriteDouble(Double value, string formatter)
        {
            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(Esc(value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(Esc(value.ToString(Culture)));
            }
        }

        private void WriteUInt16(UInt16 value, string formatter)
        {
            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(Esc(value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(Esc(value.ToString(Culture)));
            }
        }

        private void WriteUInt32(UInt32 value, string formatter)
        {
            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(Esc(value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(Esc(value.ToString(Culture)));
            }
        }

        private void WriteUInt64(UInt64 value, string formatter)
        {
            if (!string.IsNullOrEmpty(formatter))
            {
                _StreamWriter.Write(Esc(value.ToString(formatter, Culture)));
            }
            else
            {
                _StreamWriter.Write(Esc(value.ToString(Culture)));
            }
        }

        private string Esc(string s)
        {
            if (s.IndexOfAny(new char[] { '\r', '\n', '"', Separator }) == -1)
            {
                return s;
            }
            return $"\"{s.Replace("\"", "\"\"")}\"";
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
        ///
        /// </summary>
        private void Init()
        {
            if (_Properties.Count > 0)
                return;

            var _type = typeof(T);

            _Properties = _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => new { Value = p, Key = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index })
                .ToDictionary(p => p.Key, p => p.Value);

            _PropertyGetters = _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => new { Value = p, Key = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index })
                .ToDictionary(p => p.Key, p => _type.PropertyGet(p.Value.Name));

            _Formatters = _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => new { Value = p, Key = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index, (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).OutputFormat })
                .ToDictionary(p => p.Key, p => p.OutputFormat);


            _OutputNullValues = _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => new { Value = p, Key = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index, (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).OutputNullValue })
                .ToDictionary(p => p.Key, p => p.OutputNullValue);


            _ColumnIndexes = _Properties.Keys.ToList();
            _MaxColumnIndex = _ColumnIndexes.Max();
        }
    }
}