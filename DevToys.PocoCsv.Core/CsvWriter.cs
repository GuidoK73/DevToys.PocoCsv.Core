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
    public class CsvWriter<T> : BaseCsvWriter where T : new()
    {
        private Dictionary<int, PropertyInfo> _Properties = new();

        private Dictionary<int, Func<object, object>> _PropertyGetters = new();

        private Dictionary<int, string> _Formatters = new();

        private List<int> _ColumnIndexes;
        private int _MaxColumnIndex = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvWriter(string file) : base(file)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvWriter(Stream stream) : base(stream)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvWriter(string file, Encoding encoding, CultureInfo culture, char separator = ',', int buffersize = -1) : base(file, encoding, culture, separator, buffersize)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvWriter(Stream stream, Encoding encoding, CultureInfo culture, char separator = ',', int buffersize = -1) : base(stream, encoding, culture, separator, buffersize)
        { }

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
                    if (_Formatters.ContainsKey(ii))
                    {
                        _formatter = _Formatters[ii];
                    }

                    Type targetType = Nullable.GetUnderlyingType(_property.PropertyType) ?? _property.PropertyType;

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
                    else
                    {
                        _StreamWriter.Write(TypeUtils.Convert(_value, typeof(string), Culture));
                    }
                    if (ii < _MaxColumnIndex - 1)
                    {
                        _StreamWriter.Write(Separator);
                    }
                }
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
        public override void Open()
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
        protected override void Init()
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
                .Select(p => new { Value = p, Key = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index, OutputFormat = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).OutputFormat })
                .ToDictionary(p => p.Key, p => p.OutputFormat);

            _ColumnIndexes = _Properties.Keys.ToList();
            _MaxColumnIndex = _ColumnIndexes.Max();
        }
    }
}