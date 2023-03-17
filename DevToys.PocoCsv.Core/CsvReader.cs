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
    /// Enumerate Csv Stream Reader over T.
    /// Properties needs to be marked with ColumnAttribute
    /// </summary>
    public sealed class CsvReader<T> : BaseCsvReader where T : new()
    {
        private PropertyInfo[] _Properties = null;
        private Action<object, object>[] _PropertySetters = null;

        private Action<T, string>[] _PropertySettersString = null;
        private Action<T, Guid>[] _PropertySettersGuid = null;
        private Action<T, Boolean>[] _PropertySettersBoolean = null;
        private Action<T, DateTime>[] _PropertySettersDateTime = null;
        private Action<T, DateTimeOffset>[] _PropertySettersDateTimeOffset = null;
        private Action<T, TimeSpan>[] _PropertySettersTimeSpan = null;
        private Action<T, Byte>[] _PropertySettersByte = null;
        private Action<T, SByte>[] _PropertySettersSByte = null;
        private Action<T, Int16>[] _PropertySettersInt16 = null;
        private Action<T, Int32>[] _PropertySettersInt32 = null;
        private Action<T, Int64>[] _PropertySettersInt64 = null;
        private Action<T, Single>[] _PropertySettersSingle = null;
        private Action<T, Decimal>[] _PropertySettersDecimal = null;
        private Action<T, Double>[] _PropertySettersDouble = null;
        private Action<T, UInt16>[] _PropertySettersUInt16 = null;
        private Action<T, UInt32>[] _PropertySettersUInt32 = null;
        private Action<T, UInt64>[] _PropertySettersUInt64 = null;


        /// <summary>
        /// Constructor
        /// </summary>
        public CsvReader(string file) : base(file)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvReader(Stream stream) : base(stream)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvReader(Stream stream, Encoding encoding, char separator = ',', bool detectEncodingFromByteOrderMarks = true, int buffersize = 1024) : base(stream, encoding, separator, detectEncodingFromByteOrderMarks, buffersize)
        {
            _Streamer.Separator = separator;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvReader(string file, Encoding encoding, char separator = ',', bool detectEncodingFromByteOrderMarks = true, int buffersize = 1024) : base(file, encoding, separator, detectEncodingFromByteOrderMarks, buffersize)
        {
            _Streamer.Separator = separator;
        }

        /// <summary>
        ///
        /// </summary>
        [Obsolete("Use ReadAsEnumerable() or Read() instead.")]
        public IEnumerable<T> Rows() => ReadAsEnumerable();

        /// <summary>
        /// Detect the separator by sampling first 10 rows.
        /// </summary>
        public void DetectSeparator()
        {
            CsvStreamReader _reader = new CsvStreamReader(_StreamReader.BaseStream);
            bool _succes = CsvUtils.GetCsvSeparator(_reader, out _Separator, 10);
            if (_succes)
            {
                Separator = _Separator;
            }
            _StreamReader.BaseStream.Position = 0;
        }

        /// <summary>
        /// Each iteration will read the next row from stream or file
        /// </summary>
        public IEnumerable<T> ReadAsEnumerable()
        {
            if (_StreamReader == null)
            {
                throw new IOException("Reader is closed!");
            }

            _StreamReader.BaseStream.Position = 0;

            while (!EndOfStream)
            {
                yield return Read();
            }
        }

        /// <summary>
        /// Use to skip to skip first row without serializing, usefull for skipping header.
        /// </summary>
        public void Skip(int rows = 1)
        {
            for (int ii = 0; ii < rows; ii++)
            {
                _Streamer.ReadRow(_StreamReader.BaseStream, (columnIndex, value) => { }); // do nothing with read.
            }
        }


        #region Performance Read

        private enum State
        {
            First = 0,
            Normal = 1,
            Escaped = 2
        }



        private readonly StringBuilder _sb = new(63);
        private char _char;
        private int _byte;
        private string _value;

        /// <summary>
        /// reads the CsvLine
        /// </summary>
        public T Read()
        {
            T _result = new();
            int _columnIndex = 0;

            var _state = State.First;
            _sb.Length = 0;
            _byte = 0;
            while (_byte > -1)
            {
                _byte = _StreamReader.BaseStream.ReadByte();
                _char = (char)_byte;
                if (_byte == -1 || (_state == State.Normal && (_char == '\r' || _char == '\n')))
                {
                    _value = _sb.ToString().Trim('"');
                    SetValue(_columnIndex, _value, Culture, _result);
                    _columnIndex++;
                    break;
                }
                if (_state == State.First)
                {
                    _state = State.Normal;
                    if (_char == '\n')
                    {
                        continue;
                    }
                }
                if (_state == State.Normal && _char == Separator)
                {
                    _value = _sb.ToString().Trim('"');
                    SetValue(_columnIndex, _value, Culture, _result);            
                    _sb.Length = 0;
                    _columnIndex++;
                    continue;
                }
                if (_char == '"')
                {
                    _state = (_state == State.Normal) ? State.Escaped : State.Normal;
                }

                _sb.Append(_char);
            }
            return _result;
        }

        #endregion Performance Read

        private void SetValue(int index, string value, CultureInfo culture, T targetObject)
        {
            if (_Properties[index] == null)
            {
                return;
            }

            Type targetType = Nullable.GetUnderlyingType(_Properties[index].PropertyType) ?? _Properties[index].PropertyType;
            bool succes = false;

            

            if (targetType == typeof(string))
            {
                _PropertySettersString[index](targetObject, value);
                return;
            }

            if (targetType == typeof(Guid))
            {
                Guid _value;
                succes = Guid.TryParse(value, out _value);
                if (succes)
                {
                    _PropertySettersGuid[index](targetObject, _value);
                }
                return;
            }

            if (targetType == typeof(Boolean))
            {
                Boolean _value;
                succes = Boolean.TryParse(value, out _value);
                if (succes)
                {
                    _PropertySettersBoolean[index](targetObject, _value);
                }
                return;
            }

            if (targetType == typeof(DateTime))
            {
                DateTime _value;
                succes = DateTime.TryParse(value, out _value);
                if (succes)
                {
                    _PropertySettersDateTime[index](targetObject, _value);
                }
                return;
            }

            if (targetType == typeof(DateTimeOffset))
            {
                DateTimeOffset _value;
                succes = DateTimeOffset.TryParse(value, out _value);
                if (succes)
                {
                    _PropertySettersDateTimeOffset[index](targetObject, _value);
                }
                return;
            }

            if (targetType == typeof(TimeSpan))
            {
                TimeSpan _value;
                succes = TimeSpan.TryParse(value, out _value);
                if (succes)
                {
                    _PropertySettersTimeSpan[index](targetObject, _value);
                }
                return;
            }

            if (targetType == typeof(Byte))
            {
                Byte _value;
                succes = Byte.TryParse(value, out _value);
                if (succes)
                {
                    _PropertySettersByte[index](targetObject, _value);
                }
                return;
            }

            if (targetType == typeof(SByte))
            {
                SByte _value;
                succes = SByte.TryParse(value, out _value);
                if (succes)
                {
                    _PropertySettersSByte[index](targetObject, _value);
                }
                return;
            }

            if (targetType == typeof(Int16))
            {
                Int16 _value;
                succes = Int16.TryParse(value, out _value);
                if (succes)
                {
                    _PropertySettersInt16[index](targetObject, _value);
                }
                return;
            }

            if (targetType == typeof(Int32))
            {
                Int32 _value;
                succes = Int32.TryParse(value, out _value);
                if (succes)
                {
                    _PropertySettersInt32[index](targetObject, _value);
                }
                return;
            }

            if (targetType == typeof(Int64))
            {
                Int64 _value;
                succes = Int64.TryParse(value, out _value);
                if (succes)
                {
                    _PropertySettersInt64[index](targetObject, _value);
                }
                return;
            }

            if (targetType == typeof(Single))
            {
                Single _value;
                succes = Single.TryParse(value, out _value);
                if (succes)
                {
                    _PropertySettersSingle[index](targetObject, _value);
                }
                return;
            }

            if (targetType == typeof(Decimal))
            {
                Decimal _value;
                succes = Decimal.TryParse(value, out _value);
                if (succes)
                {
                    _PropertySettersDecimal[index](targetObject, _value);
                }
                return;
            }

            if (targetType == typeof(Double))
            {
                Double _value;
                succes = Double.TryParse(value, out _value);
                if (succes)
                {
                    _PropertySettersDouble[index](targetObject, _value);
                }
                return;
            }

            if (targetType == typeof(UInt16))
            {
                UInt16 _value;
                succes = UInt16.TryParse(value, out _value);
                if (succes)
                {
                    _PropertySettersUInt16[index](targetObject, _value);
                }
                return;
            }

            if (targetType == typeof(UInt32))
            {
                UInt32 _value;
                succes = UInt32.TryParse(value, out _value);
                if (succes)
                {
                    _PropertySettersUInt32[index](targetObject, _value);
                }
                return;
            }

            if (targetType == typeof(UInt64))
            {
                UInt64 _value;
                succes = UInt64.TryParse(value, out _value);
                if (succes)
                {
                    _PropertySettersUInt64[index](targetObject, _value);
                }
                return;
            }

            if (targetType.IsEnum)
            {
                _PropertySetters[index](targetObject, Enum.Parse(targetType, value));
                return;
            }
            if (targetType == typeof(byte[]))
            {
                byte[] _byteValue = Convert.FromBase64String(value);
                _PropertySetters[index](targetObject, _byteValue);
                return;
            }

        }

        /// <summary>
        /// Open the reader
        /// </summary>
        public override void Open()
        {
            Init();

            if (_Stream == null && string.IsNullOrEmpty(_File))
            {
                throw new IOException("No file or stream specified in constructor.");
            }

            if (_Stream != null)
            {
                _StreamReader = new StreamReader(stream: _Stream, encoding: Encoding, detectEncodingFromByteOrderMarks: DetectEncodingFromByteOrderMarks, bufferSize: _BufferSize);
            }
            if (!string.IsNullOrEmpty(_File))
            {
                if (!File.Exists(_File))
                {
                    throw new FileNotFoundException($"File '{_File}' not found.");
                }
                _StreamReader = new StreamReader(path: _File, encoding: Encoding, detectEncodingFromByteOrderMarks: DetectEncodingFromByteOrderMarks, bufferSize: _BufferSize);
            }
        }

        /// <summary>
        /// Initialize the CsvReader
        /// </summary>
        protected override void Init()
        {
            if (_Properties != null)
                return;

            var _type = typeof(T);

            int _max = _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index).Max();

            _Properties = new PropertyInfo[_max + 1];
            _PropertySetters = new Action<object, object>[_max + 1];
            _PropertySettersString = new Action<T, string>[_max + 1];


            foreach (var _property in _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => new { Property = p, Index = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index })
                )
            {
                if (_property.Property.PropertyType == typeof(string))
                {
                    _PropertySettersString[_property.Index] = DelegateFactory.PropertySet<T, string>(_property.Property.Name);
                }
                if (_property.Property.PropertyType == typeof(Guid))
                {
                    _PropertySettersGuid[_property.Index] = DelegateFactory.PropertySet<T, Guid>(_property.Property.Name);
                }
                if (_property.Property.PropertyType == typeof(Boolean))
                {
                    _PropertySettersBoolean[_property.Index] = DelegateFactory.PropertySet<T, Boolean>(_property.Property.Name);
                }
                if (_property.Property.PropertyType == typeof(DateTime))
                {
                    _PropertySettersDateTime[_property.Index] = DelegateFactory.PropertySet<T, DateTime>(_property.Property.Name);
                }
                if (_property.Property.PropertyType == typeof(DateTimeOffset))
                {
                    _PropertySettersDateTimeOffset[_property.Index] = DelegateFactory.PropertySet<T, DateTimeOffset>(_property.Property.Name);
                }
                if (_property.Property.PropertyType == typeof(TimeSpan))
                {
                    _PropertySettersTimeSpan[_property.Index] = DelegateFactory.PropertySet<T, TimeSpan>(_property.Property.Name);
                }
                if (_property.Property.PropertyType == typeof(Byte))
                {
                    _PropertySettersByte[_property.Index] = DelegateFactory.PropertySet<T, Byte>(_property.Property.Name);
                }
                if (_property.Property.PropertyType == typeof(SByte))
                {
                    _PropertySettersSByte[_property.Index] = DelegateFactory.PropertySet<T, SByte>(_property.Property.Name);
                }
                if (_property.Property.PropertyType == typeof(Int16))
                {
                    _PropertySettersInt16[_property.Index] = DelegateFactory.PropertySet<T, Int16>(_property.Property.Name);
                }
                if (_property.Property.PropertyType == typeof(Int32))
                {
                    _PropertySettersInt32[_property.Index] = DelegateFactory.PropertySet<T, Int32>(_property.Property.Name);
                }
                if (_property.Property.PropertyType == typeof(Int64))
                {
                    _PropertySettersInt64[_property.Index] = DelegateFactory.PropertySet<T, Int64>(_property.Property.Name);
                }
                if (_property.Property.PropertyType == typeof(Single))
                {
                    _PropertySettersSingle[_property.Index] = DelegateFactory.PropertySet<T, Single>(_property.Property.Name);
                }
                if (_property.Property.PropertyType == typeof(Decimal))
                {
                    _PropertySettersDecimal[_property.Index] = DelegateFactory.PropertySet<T, Decimal>(_property.Property.Name);
                }
                if (_property.Property.PropertyType == typeof(Double))
                {
                    _PropertySettersDouble[_property.Index] = DelegateFactory.PropertySet<T, Double>(_property.Property.Name);
                }
                if (_property.Property.PropertyType == typeof(UInt16))
                {
                    _PropertySettersUInt16[_property.Index] = DelegateFactory.PropertySet<T, UInt16>(_property.Property.Name);
                }
                if (_property.Property.PropertyType == typeof(UInt32))
                {
                    _PropertySettersUInt32[_property.Index] = DelegateFactory.PropertySet<T, UInt32>(_property.Property.Name);
                }
                if (_property.Property.PropertyType == typeof(UInt64))
                {
                    _PropertySettersUInt64[_property.Index] = DelegateFactory.PropertySet<T, UInt64>(_property.Property.Name);
                }


                _Properties[_property.Index] = _property.Property;

                _PropertySetters[_property.Index] = _type.PropertySet(_property.Property.Name);
            }
        }
    }
}