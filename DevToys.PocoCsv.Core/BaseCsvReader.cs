using Delegates;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    public abstract class BaseCsvReader<T> : BaseCsv where T : class, new()
    {
        protected ImmutableArray<Action<T, Int32>> _PropertySettersEnum;
        protected ImmutableArray<Action<object, object>> _PropertySetters;
        protected ImmutableArray<Action<T, string>> _PropertySettersString;
        protected ImmutableArray<Action<T, Guid>> _PropertySettersGuid;
        protected ImmutableArray<Action<T, Boolean>> _PropertySettersBoolean;
        protected ImmutableArray<Action<T, DateTime>> _PropertySettersDateTime;
        protected ImmutableArray<Action<T, DateTimeOffset>> _PropertySettersDateTimeOffset;
        protected ImmutableArray<Action<T, TimeSpan>> _PropertySettersTimeSpan;
        protected ImmutableArray<Action<T, Byte>> _PropertySettersByte;
        protected ImmutableArray<Action<T, SByte>> _PropertySettersSByte;
        protected ImmutableArray<Action<T, Int16>> _PropertySettersInt16;
        protected ImmutableArray<Action<T, Int32>> _PropertySettersInt32;
        protected ImmutableArray<Action<T, Int64>> _PropertySettersInt64;
        protected ImmutableArray<Action<T, Single>> _PropertySettersSingle;
        protected ImmutableArray<Action<T, Decimal>> _PropertySettersDecimal;
        protected ImmutableArray<Action<T, Double>> _PropertySettersDouble;
        protected ImmutableArray<Action<T, UInt16>> _PropertySettersUInt16;
        protected ImmutableArray<Action<T, UInt32>> _PropertySettersUInt32;
        protected ImmutableArray<Action<T, UInt64>> _PropertySettersUInt64;
        protected ImmutableArray<Action<T, BigInteger>> _PropertySettersBigInteger;
        protected ImmutableArray<Action<T, Guid?>> _PropertySettersGuidNull;
        protected ImmutableArray<Action<T, Boolean?>> _PropertySettersBooleanNull;
        protected ImmutableArray<Action<T, DateTime?>> _PropertySettersDateTimeNull;
        protected ImmutableArray<Action<T, DateTimeOffset?>> _PropertySettersDateTimeOffsetNull;
        protected ImmutableArray<Action<T, TimeSpan?>> _PropertySettersTimeSpanNull;
        protected ImmutableArray<Action<T, Byte?>> _PropertySettersByteNull;
        protected ImmutableArray<Action<T, SByte?>> _PropertySettersSByteNull;
        protected ImmutableArray<Action<T, Int16?>> _PropertySettersInt16Null;
        protected ImmutableArray<Action<T, Int32?>> _PropertySettersInt32Null;
        protected ImmutableArray<Action<T, Int64?>> _PropertySettersInt64Null;
        protected ImmutableArray<Action<T, Single?>> _PropertySettersSingleNull;
        protected ImmutableArray<Action<T, Decimal?>> _PropertySettersDecimalNull;
        protected ImmutableArray<Action<T, Double?>> _PropertySettersDoubleNull;
        protected ImmutableArray<Action<T, UInt16?>> _PropertySettersUInt16Null;
        protected ImmutableArray<Action<T, UInt32?>> _PropertySettersUInt32Null;
        protected ImmutableArray<Action<T, UInt64?>> _PropertySettersUInt64Null;
        protected ImmutableArray<Action<T, BigInteger?>> _PropertySettersBigIntegerNull;

        protected readonly List<CsvReadError> _Errors = new List<CsvReadError>();
        protected readonly List<string> _CsvLineReaderResult = new List<string>();
        protected StringBuilder _buffer = new StringBuilder(1024);
        protected bool _IsInitializing = false;
        protected int _propertyCount = 0;
        protected int _CurrentLine = 0;
        protected int _CollIndex = 0; // column index.

        /// <summary>
        /// All properties are handled in order of property occurrence and mapped directly to their respective index. Only use when CsvWriter has this set to true as well. (ColumnAttribute is ignored.)
        /// </summary>
        public bool IgnoreColumnAttributes { get; set; } = false;

        /// <summary>
        /// Culture info to use for serialization.
        /// </summary>
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// Returns current Line position.
        /// </summary>
        public int CurrentLine { get => _CurrentLine; }

        public abstract string[] ReadHeader();

        // Called each field value. (if assigned)
        protected void SetValue(T targetObject)
        {
            switch (_IsNullable[_CollIndex])
            {
                case false:
                    switch (_PropertyTypes[_CollIndex])
                    {
                        case NetTypeComplete.String: SetValueString(targetObject); break;
                        case NetTypeComplete.Decimal: SetValueDecimal(targetObject); break;
                        case NetTypeComplete.Int32: SetValueInt32(targetObject); break;
                        case NetTypeComplete.Double: SetValueDouble(targetObject); break;
                        case NetTypeComplete.DateTime: SetValueDateTime(targetObject); break;
                        case NetTypeComplete.Int64: SetValueInt64(targetObject); break;
                        case NetTypeComplete.Guid: SetValueGuid(targetObject); break;
                        case NetTypeComplete.Single: SetValueSingle(targetObject); break;
                        case NetTypeComplete.Boolean: SetValueBoolean(targetObject); break;
                        case NetTypeComplete.TimeSpan: SetValueTimeSpan(targetObject); break;
                        case NetTypeComplete.Int16: SetValueInt16(targetObject); break;
                        case NetTypeComplete.Byte: SetValueByte(targetObject); break;
                        case NetTypeComplete.DateTimeOffset: SetValueDateTimeOffset(targetObject); break;
                        case NetTypeComplete.SByte: SetValueSByte(targetObject); break;
                        case NetTypeComplete.UInt16: SetValueUInt16(targetObject); break;
                        case NetTypeComplete.UInt32: SetValueUInt32(targetObject); break;
                        case NetTypeComplete.UInt64: SetValueUInt64(targetObject); break;
                        case NetTypeComplete.BigInteger: SetValueBigInteger(targetObject); break;
                        case NetTypeComplete.ByteArray: SetValueByteArray(targetObject); break;
                        case NetTypeComplete.Enum: SetValueEnum(targetObject); break;
                    }
                    return;
                case true:
                    switch (_PropertyTypes[_CollIndex])
                    {
                        case NetTypeComplete.DecimalNullable: SetValueDecimalNull(targetObject); break;
                        case NetTypeComplete.Int32Nullable: SetValueInt32Null(targetObject); break;
                        case NetTypeComplete.DoubleNullable: SetValueDoubleNull(targetObject); break;
                        case NetTypeComplete.DateTimeNullable: SetValueDateTimeNull(targetObject); break;
                        case NetTypeComplete.Int64Nullable: SetValueInt64Null(targetObject); break;
                        case NetTypeComplete.GuidNullable: SetValueGuidNull(targetObject); break;
                        case NetTypeComplete.SingleNullable: SetValueSingleNull(targetObject); break;
                        case NetTypeComplete.BooleanNullable: SetValueBooleanNull(targetObject); break;
                        case NetTypeComplete.TimeSpanNullable: SetValueTimeSpanNull(targetObject); break;
                        case NetTypeComplete.Int16Nullable: SetValueInt16Null(targetObject); break;
                        case NetTypeComplete.ByteNullable: SetValueByteNull(targetObject); break;
                        case NetTypeComplete.DateTimeOffsetNullable: SetValueDateTimeOffsetNull(targetObject); break;
                        case NetTypeComplete.SByteNullable: SetValueSByteNull(targetObject); break;
                        case NetTypeComplete.UInt16Nullable: SetValueUInt16Null(targetObject); break;
                        case NetTypeComplete.UInt32Nullable: SetValueUInt32Null(targetObject); break;
                        case NetTypeComplete.UInt64Nullable: SetValueUInt64Null(targetObject); break;
                        case NetTypeComplete.BigIntegerNullable: SetValueBigIntegerNull(targetObject); break;
                    }
                    return;
            }
        }


        #region Value Setters

        private void SetValueString(T targetObject)
        {
            if (_CustomParserString[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                _PropertySettersString[_CollIndex](targetObject, _buffer.ToString());
            }
            else
            {
                try
                {
                    String _customParserValue = (String)_CustomParserCall[_CollIndex](_CustomParserString[_CollIndex], new object[] { _buffer });
                    _PropertySettersString[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueEnum(T targetObject)
        {
            bool succes = int.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out int _value);
            if (succes)
            {
                _PropertySettersEnum[_CollIndex](targetObject, _value);
            }
            else
            {
                _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
            }
        }

        private void SetValueDecimal(T targetObject)
        {
            if (_CustomParserDecimal[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                bool succes = Decimal.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out decimal _value);
                if (succes)
                {
                    _PropertySettersDecimal[_CollIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    Decimal _customParserValue = (Decimal)_CustomParserCall[_CollIndex](_CustomParserDecimal[_CollIndex], new object[] { _buffer });
                    _PropertySettersDecimal[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueInt32(T targetObject)
        {
            if (_CustomParserInt32[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                bool succes = Int32.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out int _value);
                if (succes)
                {
                    _PropertySettersInt32[_CollIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    Int32 _customParserValue = (Int32)_CustomParserCall[_CollIndex](_CustomParserInt32[_CollIndex], new object[] { _buffer });
                    _PropertySettersInt32[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueInt64(T targetObject)
        {
            if (_CustomParserInt64[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                bool succes = Int64.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out long _value);
                if (succes)
                {
                    _PropertySettersInt64[_CollIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    Int64 _customParserValue = (Int64)_CustomParserCall[_CollIndex](_CustomParserInt64[_CollIndex], new object[] { _buffer });
                    _PropertySettersInt64[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
                return;
            }
        }

        private void SetValueDouble(T targetObject)
        {
            if (_CustomParserDouble[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                bool succes = Double.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out double _value);
                if (succes)
                {
                    _PropertySettersDouble[_CollIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    Double _customParserValue = (Double)_CustomParserCall[_CollIndex](_CustomParserDouble[_CollIndex], new object[] { _buffer });
                    _PropertySettersDouble[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueDateTime(T targetObject)
        {
            if (_CustomParserDateTime[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                bool succes = DateTime.TryParse(_buffer.ToString(), Culture, DateTimeStyles.None, out DateTime _value);
                if (succes)
                {
                    _PropertySettersDateTime[_CollIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    DateTime _customParserValue = (DateTime)_CustomParserCall[_CollIndex](_CustomParserDateTime[_CollIndex], new object[] { _buffer });
                    _PropertySettersDateTime[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueGuid(T targetObject)
        {
            if (_CustomParserGuid[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                bool succes = Guid.TryParse(_buffer.ToString(), out Guid _value);
                if (succes)
                {
                    _PropertySettersGuid[_CollIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    Guid _customParserValue = (Guid)_CustomParserCall[_CollIndex](_CustomParserGuid[_CollIndex], new object[] { _buffer });
                    _PropertySettersGuid[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueSingle(T targetObject)
        {
            if (_CustomParserSingle[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                bool succes = Single.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out float _value);
                if (succes)
                {
                    _PropertySettersSingle[_CollIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    Single _customParserValue = (Single)_CustomParserCall[_CollIndex](_CustomParserSingle[_CollIndex], new object[] { _buffer });
                    _PropertySettersSingle[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueBoolean(T targetObject)
        {
            if (_CustomParserBoolean[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                bool succes = Boolean.TryParse(_buffer.ToString(), out Boolean _value);
                if (succes)
                {
                    _PropertySettersBoolean[_CollIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    bool _customParserValue = (bool)_CustomParserCall[_CollIndex](_CustomParserBoolean[_CollIndex], new object[] { _buffer });
                    _PropertySettersBoolean[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueTimeSpan(T targetObject)
        {
            if (_CustomParserTimeSpan[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                bool succes = TimeSpan.TryParse(_buffer.ToString(), Culture, out TimeSpan _value);
                if (succes)
                {
                    _PropertySettersTimeSpan[_CollIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    TimeSpan _customParserValue = (TimeSpan)_CustomParserCall[_CollIndex](_CustomParserTimeSpan[_CollIndex], new object[] { _buffer });
                    _PropertySettersTimeSpan[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueInt16(T targetObject)
        {
            if (_CustomParserInt16[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                bool succes = Int16.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out Int16 _value);
                if (succes)
                {
                    _PropertySettersInt16[_CollIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    Int16 _customParserValue = (Int16)_CustomParserCall[_CollIndex](_CustomParserInt16[_CollIndex], new object[] { _buffer });
                    _PropertySettersInt16[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }

        }

        private void SetValueByte(T targetObject)
        {
            if (_CustomParserByte[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                bool succes = Byte.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out Byte _value);
                if (succes)
                {
                    _PropertySettersByte[_CollIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    Byte _customParserValue = (Byte)_CustomParserCall[_CollIndex](_CustomParserByte[_CollIndex], new object[] { _buffer });
                    _PropertySettersByte[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueDateTimeOffset(T targetObject)
        {
            if (_CustomParserDateTimeOffset[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                bool succes = DateTimeOffset.TryParse(_buffer.ToString(), Culture, DateTimeStyles.None, out DateTimeOffset _value);
                if (succes)
                {
                    _PropertySettersDateTimeOffset[_CollIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    DateTimeOffset _customParserValue = (DateTimeOffset)_CustomParserCall[_CollIndex](_CustomParserDateTimeOffset[_CollIndex], new object[] { _buffer });
                    _PropertySettersDateTimeOffset[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueByteArray(T targetObject)
        {
            byte[] _byteValue = null;
            try
            {
                _byteValue = Convert.FromBase64String(_buffer.ToString());
            }
            catch (Exception ex)
            {
                _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
            }
            _PropertySetters[_CollIndex](targetObject, _byteValue);
        }

        private void SetValueSByte(T targetObject)
        {
            if (_CustomParserSByte[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                bool succes = SByte.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out SByte _value);
                if (succes)
                {
                    _PropertySettersSByte[_CollIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    SByte _customParserValue = (SByte)_CustomParserCall[_CollIndex](_CustomParserSByte[_CollIndex], new object[] { _buffer });
                    _PropertySettersSByte[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueUInt16(T targetObject)
        {
            if (_CustomParserUInt16[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                bool succes = UInt16.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out UInt16 _value);
                if (succes)
                {
                    _PropertySettersUInt16[_CollIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    UInt16 _customParserValue = (UInt16)_CustomParserCall[_CollIndex](_CustomParserUInt16[_CollIndex], new object[] { _buffer });
                    _PropertySettersUInt16[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueUInt32(T targetObject)
        {
            if (_CustomParserUInt32[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                bool succes = UInt32.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out UInt32 _value);
                if (succes)
                {
                    _PropertySettersUInt32[_CollIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    UInt32 _customParserValue = (UInt32)_CustomParserCall[_CollIndex](_CustomParserUInt32[_CollIndex], new object[] { _buffer });
                    _PropertySettersUInt32[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueUInt64(T targetObject)
        {
            if (_CustomParserUInt64[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                bool succes = UInt64.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out UInt64 _value);
                if (succes)
                {
                    _PropertySettersUInt64[_CollIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    UInt64 _customParserValue = (UInt64)_CustomParserCall[_CollIndex](_CustomParserUInt64[_CollIndex], new object[] { _buffer });
                    _PropertySettersUInt64[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueBigInteger(T targetObject)
        {
            if (_CustomParserBigInteger[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                bool succes = BigInteger.TryParse(_buffer.ToString(), NumberStyles.Any, Culture, out BigInteger _value);
                if (succes)
                {
                    _PropertySettersBigInteger[_CollIndex](targetObject, _value);
                }
                else
                {
                    _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
            else
            {
                try
                {
                    BigInteger _customParserValue = (BigInteger)_CustomParserCall[_CollIndex](_CustomParserBigInteger[_CollIndex], new object[] { _buffer });
                    _PropertySettersBigInteger[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        #endregion

        #region Value Setters Nullable

        private void SetValueGuidNull(T targetObject)
        {
            if (_CustomParserGuidNullable[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = Guid.TryParse(_buffer.ToString(), out Guid _value);
                    if (succes)
                    {
                        _PropertySettersGuidNull[_CollIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersGuidNull[_CollIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    Guid? _customParserValue = (Guid?)_CustomParserCall[_CollIndex](_CustomParserGuidNullable[_CollIndex], new object[] { _buffer });
                    _PropertySettersGuidNull[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueBooleanNull(T targetObject)
        {
            if (_CustomParserBooleanNullable[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = Boolean.TryParse(_buffer.ToString(), out Boolean _value);
                    if (succes)
                    {
                        _PropertySettersBooleanNull[_CollIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersBooleanNull[_CollIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    bool? _customParserValue = (bool?)_CustomParserCall[_CollIndex](_CustomParserBooleanNullable[_CollIndex], new object[] { _buffer });
                    _PropertySettersBooleanNull[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueDateTimeNull(T targetObject)
        {
            if (_CustomParserDateTimeNullable[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = DateTime.TryParse(_buffer.ToString(), out DateTime _value);
                    if (succes)
                    {
                        _PropertySettersDateTimeNull[_CollIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersDateTimeNull[_CollIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    DateTime? _customParserValue = (DateTime?)_CustomParserCall[_CollIndex](_CustomParserDateTimeNullable[_CollIndex], new object[] { _buffer });
                    _PropertySettersDateTimeNull[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueDateTimeOffsetNull(T targetObject)
        {
            if (_CustomParserDateTimeOffsetNullable[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = DateTimeOffset.TryParse(_buffer.ToString(), out DateTimeOffset _value);
                    if (succes)
                    {
                        _PropertySettersDateTimeOffsetNull[_CollIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersDateTimeOffsetNull[_CollIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    DateTimeOffset? _customParserValue = (DateTimeOffset?)_CustomParserCall[_CollIndex](_CustomParserDateTimeOffsetNullable[_CollIndex], new object[] { _buffer });
                    _PropertySettersDateTimeOffsetNull[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueTimeSpanNull(T targetObject)
        {
            if (_CustomParserTimeSpanNullable[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = TimeSpan.TryParse(_buffer.ToString(), out TimeSpan _value);
                    if (succes)
                    {
                        _PropertySettersTimeSpanNull[_CollIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersTimeSpanNull[_CollIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    TimeSpan? _customParserValue = (TimeSpan?)_CustomParserCall[_CollIndex](_CustomParserTimeSpanNullable[_CollIndex], new object[] { _buffer });
                    _PropertySettersTimeSpanNull[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueByteNull(T targetObject)
        {
            if (_CustomParserByteNullable[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = Byte.TryParse(_buffer.ToString(), out Byte _value);
                    if (succes)
                    {
                        _PropertySettersByteNull[_CollIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersByteNull[_CollIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    byte? _customParserValue = (byte?)_CustomParserCall[_CollIndex](_CustomParserByteNullable[_CollIndex], new object[] { _buffer });
                    _PropertySettersByteNull[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueSByteNull(T targetObject)
        {
            if (_CustomParserSByteNullable[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = SByte.TryParse(_buffer.ToString(), out SByte _value);
                    if (succes)
                    {
                        _PropertySettersSByteNull[_CollIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersSByteNull[_CollIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    SByte? _customParserValue = (SByte?)_CustomParserCall[_CollIndex](_CustomParserSByteNullable[_CollIndex], new object[] { _buffer });
                    _PropertySettersSByteNull[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueInt16Null(T targetObject)
        {
            if (_CustomParserInt16Nullable[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = Int16.TryParse(_buffer.ToString(), out Int16 _value);
                    if (succes)
                    {
                        _PropertySettersInt16Null[_CollIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersInt16Null[_CollIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    Int16? _customParserValue = (Int16?)_CustomParserCall[_CollIndex](_CustomParserInt16Nullable[_CollIndex], new object[] { _buffer });
                    _PropertySettersInt16Null[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueInt32Null(T targetObject)
        {
            if (_CustomParserInt32Nullable[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = Int32.TryParse(_buffer.ToString(), out Int32 _value);
                    if (succes)
                    {
                        _PropertySettersInt32Null[_CollIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersInt32Null[_CollIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    Int32? _customParserValue = (Int32?)_CustomParserCall[_CollIndex](_CustomParserInt32Nullable[_CollIndex], new object[] { _buffer });
                    _PropertySettersInt32Null[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueInt64Null(T targetObject)
        {
            if (_CustomParserInt64Nullable[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = Int64.TryParse(_buffer.ToString(), out Int64 _value);
                    if (succes)
                    {
                        _PropertySettersInt64Null[_CollIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersInt64Null[_CollIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    Int64? _customParserValue = (Int64?)_CustomParserCall[_CollIndex](_CustomParserInt64Nullable[_CollIndex], new object[] { _buffer });
                    _PropertySettersInt64Null[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueSingleNull(T targetObject)
        {
            if (_CustomParserSingleNullable[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = Single.TryParse(_buffer.ToString(), out Single _value);
                    if (succes)
                    {
                        _PropertySettersSingleNull[_CollIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersSingleNull[_CollIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    Single? _customParserValue = (Single?)_CustomParserCall[_CollIndex](_CustomParserSingleNullable[_CollIndex], new object[] { _buffer });
                    _PropertySettersSingleNull[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueDecimalNull(T targetObject)
        {
            if (_CustomParserDecimalNullable[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = Decimal.TryParse(_buffer.ToString(), out Decimal _value);
                    if (succes)
                    {
                        _PropertySettersDecimalNull[_CollIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersDecimalNull[_CollIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    Decimal? _customParserValue = (Decimal?)_CustomParserCall[_CollIndex](_CustomParserDecimalNullable[_CollIndex], new object[] { _buffer });
                    _PropertySettersDecimalNull[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueDoubleNull(T targetObject)
        {
            if (_CustomParserDoubleNullable[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = Double.TryParse(_buffer.ToString(), out Double _value);
                    if (succes)
                    {
                        _PropertySettersDoubleNull[_CollIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersDoubleNull[_CollIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    Double? _customParserValue = (Double?)_CustomParserCall[_CollIndex](_CustomParserDoubleNullable[_CollIndex], new object[] { _buffer });
                    _PropertySettersDoubleNull[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueUInt16Null(T targetObject)
        {
            if (_CustomParserUInt16Nullable[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = UInt16.TryParse(_buffer.ToString(), out UInt16 _value);
                    if (succes)
                    {
                        _PropertySettersUInt16Null[_CollIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersUInt16Null[_CollIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    UInt16? _customParserValue = (UInt16?)_CustomParserCall[_CollIndex](_CustomParserUInt16Nullable[_CollIndex], new object[] { _buffer });
                    _PropertySettersUInt16Null[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueUInt32Null(T targetObject)
        {
            if (_CustomParserUInt32Nullable[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = UInt32.TryParse(_buffer.ToString(), out UInt32 _value);
                    if (succes)
                    {
                        _PropertySettersUInt32Null[_CollIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersUInt32Null[_CollIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    UInt32? _customParserValue = (UInt32?)_CustomParserCall[_CollIndex](_CustomParserUInt32Nullable[_CollIndex], new object[] { _buffer });
                    _PropertySettersUInt32Null[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueUInt64Null(T targetObject)
        {
            if (_CustomParserUInt64Nullable[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = UInt64.TryParse(_buffer.ToString(), out UInt64 _value);
                    if (succes)
                    {
                        _PropertySettersUInt64Null[_CollIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersUInt64Null[_CollIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    UInt64? _customParserValue = (UInt64?)_CustomParserCall[_CollIndex](_CustomParserUInt64Nullable[_CollIndex], new object[] { _buffer });
                    _PropertySettersUInt64Null[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        private void SetValueBigIntegerNull(T targetObject)
        {
            if (_CustomParserBigIntegerNullable[_CollIndex] == null || _CustomParserCall[_CollIndex] == null)
            {
                if (_buffer.Length > 0)
                {
                    bool succes = BigInteger.TryParse(_buffer.ToString(), out BigInteger _value);
                    if (succes)
                    {
                        _PropertySettersBigIntegerNull[_CollIndex](targetObject, _value);
                    }
                    else
                    {
                        _Errors.Add(new CsvReadError() { ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                    }
                }
                else
                {
                    _PropertySettersBigIntegerNull[_CollIndex](targetObject, null);
                }
            }
            else
            {
                try
                {
                    BigInteger? _customParserValue = (BigInteger?)_CustomParserCall[_CollIndex](_CustomParserBigIntegerNullable[_CollIndex], new object[] { _buffer });
                    _PropertySettersBigIntegerNull[_CollIndex](targetObject, _customParserValue);
                }
                catch (Exception ex)
                {
                    _Errors.Add(new CsvReadError() { Exception = ex, ColumnIndex = _CollIndex, PropertyName = _Properties[_CollIndex].Name, PropertyType = _Properties[_CollIndex].PropertyType, Value = _buffer.ToString(), LineNumber = CurrentLine });
                }
            }
        }

        #endregion Value Setters

        /// <summary>
        /// Initialize the CsvReader
        /// </summary>
        protected void Init()
        {
            _IsInitializing = true;
            if (_Properties != null)
                return;

            var _type = typeof(T);

            var _propertyAttributeCollection = _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => new { Property = p, Index = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index, Attrib = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute) })
                .ToList();

            var _duplicates = _propertyAttributeCollection
                .GroupBy(p => p.Index)
                .Select(group => new { Key = group.Key, Count = group.Count() })
                .Where(p => p.Count > 1)
                .ToList();

            if (_duplicates.Count() > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"Duplicate indexes are not allowed on Column attributes for {typeof(T).Name}. Indexes: ");
                foreach (var dup in _duplicates)
                {
                    sb.Append(dup.Key);
                    sb.Append(", ");
                }
                sb.Length -= 2;
                throw new CsvException(sb.ToString());
            }

            if (IgnoreColumnAttributes == true || _propertyAttributeCollection.Count == 0)
            {
                var _header = ReadHeader();

                var _headerNameIndexDict = _header.Select((value, index) => new { Index = index, Value = value }).ToDictionary(p => p.Value, p => p.Index);

                _propertyAttributeCollection = _type.GetProperties()
                    .Select(p => new { Property = p, Index = _headerNameIndexDict.ContainsKey(p.Name) ? _headerNameIndexDict[p.Name] : -1, Attrib = new ColumnAttribute() { Index = _headerNameIndexDict.ContainsKey(p.Name) ? _headerNameIndexDict[p.Name] : -1 } })
                    .Where(p => p.Index > -1)
                    .ToList();
            }

            int _max = _propertyAttributeCollection.Select(p => p.Index).Max();

            InitCustomCsvParseArrays(_max + 1);
            InitCsvAttributeRead(_type, _max + 1);

            var _properties = new PropertyInfo[_max + 1];
            var _propertyTypes = new NetTypeComplete[_max + 1];
            var _propertySetters = new Action<object, object>[_max + 1];
            var _isNullable = new Boolean[_max + 1];
            var _isAssigned = new Boolean[_max + 1];
            var _propertySettersEnum = new Action<T, Int32>[_max + 1];
            var _propertySettersString = new Action<T, String>[_max + 1];
            var _propertySettersGuid = new Action<T, Guid>[_max + 1];
            var _propertySettersBoolean = new Action<T, Boolean>[_max + 1];
            var _propertySettersDateTime = new Action<T, DateTime>[_max + 1];
            var _propertySettersDateTimeOffset = new Action<T, DateTimeOffset>[_max + 1];
            var _propertySettersTimeSpan = new Action<T, TimeSpan>[_max + 1];
            var _propertySettersByte = new Action<T, Byte>[_max + 1];
            var _propertySettersSByte = new Action<T, SByte>[_max + 1];
            var _propertySettersInt16 = new Action<T, Int16>[_max + 1];
            var _propertySettersInt32 = new Action<T, Int32>[_max + 1];
            var _propertySettersInt64 = new Action<T, Int64>[_max + 1];
            var _propertySettersSingle = new Action<T, Single>[_max + 1];
            var _propertySettersDecimal = new Action<T, Decimal>[_max + 1];
            var _propertySettersDouble = new Action<T, Double>[_max + 1];
            var _propertySettersUInt16 = new Action<T, UInt16>[_max + 1];
            var _propertySettersUInt32 = new Action<T, UInt32>[_max + 1];
            var _propertySettersUInt64 = new Action<T, UInt64>[_max + 1];
            var _propertySettersBigInteger = new Action<T, BigInteger>[_max + 1];
            var _propertySettersGuidNull = new Action<T, Guid?>[_max + 1];
            var _propertySettersBooleanNull = new Action<T, Boolean?>[_max + 1];
            var _propertySettersDateTimeNull = new Action<T, DateTime?>[_max + 1];
            var _propertySettersDateTimeOffsetNull = new Action<T, DateTimeOffset?>[_max + 1];
            var _propertySettersTimeSpanNull = new Action<T, TimeSpan?>[_max + 1];
            var _propertySettersByteNull = new Action<T, Byte?>[_max + 1];
            var _propertySettersSByteNull = new Action<T, SByte?>[_max + 1];
            var _propertySettersInt16Null = new Action<T, Int16?>[_max + 1];
            var _propertySettersInt32Null = new Action<T, Int32?>[_max + 1];
            var _propertySettersInt64Null = new Action<T, Int64?>[_max + 1];
            var _propertySettersSingleNull = new Action<T, Single?>[_max + 1];
            var _propertySettersDecimalNull = new Action<T, Decimal?>[_max + 1];
            var _propertySettersDoubleNull = new Action<T, Double?>[_max + 1];
            var _propertySettersUInt16Null = new Action<T, UInt16?>[_max + 1];
            var _propertySettersUInt32Null = new Action<T, UInt32?>[_max + 1];
            var _propertySettersUInt64Null = new Action<T, UInt64?>[_max + 1];
            var _propertySettersBigIntegerNull = new Action<T, BigInteger?>[_max + 1];

            foreach (var property in _propertyAttributeCollection)
            {
                Type propertyType = property.Property.PropertyType;

                _isNullable[property.Index] = Nullable.GetUnderlyingType(propertyType) != null;
                _isAssigned[property.Index] = true;

                if (property.Attrib.CustomParserType != null)
                {
                    SetCustomParserType(property.Index, property.Attrib.CustomParserType, property.Property.Name);
                    __CustomParserCall[property.Index] = DelegateFactory.InstanceMethod(property.Attrib.CustomParserType, "Read", typeof(StringBuilder));
                    __ICustomCsvParseBase[property.Index] = __CustomParser[property.Index];
                }

                if (propertyType == typeof(string))
                {
                    _propertySettersString[property.Index] = DelegateFactory.PropertySet<T, string>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.String;
                }
                else if (propertyType == typeof(Guid))
                {
                    _propertySettersGuid[property.Index] = DelegateFactory.PropertySet<T, Guid>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Guid;
                }
                else if (propertyType == typeof(Boolean))
                {
                    _propertySettersBoolean[property.Index] = DelegateFactory.PropertySet<T, Boolean>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Boolean;
                }
                else if (propertyType == typeof(DateTime))
                {
                    _propertySettersDateTime[property.Index] = DelegateFactory.PropertySet<T, DateTime>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.DateTime;
                }
                else if (propertyType == typeof(DateTimeOffset))
                {
                    _propertySettersDateTimeOffset[property.Index] = DelegateFactory.PropertySet<T, DateTimeOffset>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.DateTimeOffset;
                }
                else if (propertyType == typeof(TimeSpan))
                {
                    _propertySettersTimeSpan[property.Index] = DelegateFactory.PropertySet<T, TimeSpan>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.TimeSpan;
                }
                else if (propertyType == typeof(Byte))
                {
                    _propertySettersByte[property.Index] = DelegateFactory.PropertySet<T, Byte>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Byte;
                }
                else if (propertyType == typeof(SByte))
                {
                    _propertySettersSByte[property.Index] = DelegateFactory.PropertySet<T, SByte>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.SByte;
                }
                else if (propertyType == typeof(Int16))
                {
                    _propertySettersInt16[property.Index] = DelegateFactory.PropertySet<T, Int16>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Int16;
                }
                else if (propertyType == typeof(Int32))
                {
                    _propertySettersInt32[property.Index] = DelegateFactory.PropertySet<T, Int32>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Int32;
                }
                else if (propertyType == typeof(Int64))
                {
                    _propertySettersInt64[property.Index] = DelegateFactory.PropertySet<T, Int64>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Int64;
                }
                else if (propertyType == typeof(Single))
                {
                    _propertySettersSingle[property.Index] = DelegateFactory.PropertySet<T, Single>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Single;
                }
                else if (propertyType == typeof(Decimal))
                {
                    _propertySettersDecimal[property.Index] = DelegateFactory.PropertySet<T, Decimal>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Decimal;
                }
                else if (propertyType == typeof(Double))
                {
                    _propertySettersDouble[property.Index] = DelegateFactory.PropertySet<T, Double>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Double;
                }
                else if (propertyType == typeof(UInt16))
                {
                    _propertySettersUInt16[property.Index] = DelegateFactory.PropertySet<T, UInt16>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.UInt16;
                }
                else if (propertyType == typeof(UInt32))
                {
                    _propertySettersUInt32[property.Index] = DelegateFactory.PropertySet<T, UInt32>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.UInt32;
                }
                else if (propertyType == typeof(UInt64))
                {
                    _propertySettersUInt64[property.Index] = DelegateFactory.PropertySet<T, UInt64>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.UInt64;
                }
                else if (propertyType == typeof(BigInteger))
                {
                    _propertySettersBigInteger[property.Index] = DelegateFactory.PropertySet<T, BigInteger>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.BigInteger;
                }
                else if (propertyType == typeof(Guid?))
                {
                    _propertySettersGuidNull[property.Index] = DelegateFactory.PropertySet<T, Guid?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.GuidNullable;
                }
                else if (propertyType == typeof(Boolean?))
                {
                    _propertySettersBooleanNull[property.Index] = DelegateFactory.PropertySet<T, Boolean?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.BooleanNullable;
                }
                else if (propertyType == typeof(DateTime?))
                {
                    _propertySettersDateTimeNull[property.Index] = DelegateFactory.PropertySet<T, DateTime?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.DateTimeNullable;
                }
                else if (propertyType == typeof(DateTimeOffset?))
                {
                    _propertySettersDateTimeOffsetNull[property.Index] = DelegateFactory.PropertySet<T, DateTimeOffset?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.DateTimeOffsetNullable;
                }
                else if (propertyType == typeof(TimeSpan?))
                {
                    _propertySettersTimeSpanNull[property.Index] = DelegateFactory.PropertySet<T, TimeSpan?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.TimeSpanNullable;
                }
                else if (propertyType == typeof(Byte?))
                {
                    _propertySettersByteNull[property.Index] = DelegateFactory.PropertySet<T, Byte?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.ByteNullable;
                }
                else if (propertyType == typeof(SByte?))
                {
                    _propertySettersSByteNull[property.Index] = DelegateFactory.PropertySet<T, SByte?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.SByteNullable;
                }
                else if (propertyType == typeof(Int16?))
                {
                    _propertySettersInt16Null[property.Index] = DelegateFactory.PropertySet<T, Int16?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Int16Nullable;
                }
                else if (propertyType == typeof(Int32?))
                {
                    _propertySettersInt32Null[property.Index] = DelegateFactory.PropertySet<T, Int32?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Int32Nullable;
                }
                else if (propertyType == typeof(Int64?))
                {
                    _propertySettersInt64Null[property.Index] = DelegateFactory.PropertySet<T, Int64?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Int64Nullable;
                }
                else if (propertyType == typeof(Single?))
                {
                    _propertySettersSingleNull[property.Index] = DelegateFactory.PropertySet<T, Single?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.SingleNullable;
                }
                else if (propertyType == typeof(Decimal?))
                {
                    _propertySettersDecimalNull[property.Index] = DelegateFactory.PropertySet<T, Decimal?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.DecimalNullable;
                }
                else if (propertyType == typeof(Double?))
                {
                    _propertySettersDoubleNull[property.Index] = DelegateFactory.PropertySet<T, Double?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.DoubleNullable;
                }
                else if (propertyType == typeof(UInt16?))
                {
                    _propertySettersUInt16Null[property.Index] = DelegateFactory.PropertySet<T, UInt16?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.UInt16Nullable;
                }
                else if (propertyType == typeof(UInt32?))
                {
                    _propertySettersUInt32Null[property.Index] = DelegateFactory.PropertySet<T, UInt32?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.UInt32Nullable;
                }
                else if (propertyType == typeof(UInt64?))
                {
                    _propertySettersUInt64Null[property.Index] = DelegateFactory.PropertySet<T, UInt64?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.UInt64Nullable;
                }
                else if (propertyType == typeof(BigInteger?))
                {
                    _propertySettersBigIntegerNull[property.Index] = DelegateFactory.PropertySet<T, BigInteger?>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.BigIntegerNullable;
                }
                else if (propertyType == typeof(byte[]))
                {
                    _propertySetters[property.Index] = _type.PropertySet(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.ByteArray;
                }
                else if (propertyType.IsEnum)
                {
                    _propertySettersEnum[property.Index] = DelegateFactory.PropertySet<T, int>(property.Property.Name);
                    _propertyTypes[property.Index] = NetTypeComplete.Enum;
                }
                else
                {
                    throw new CsvException($"Property: {property.Property} Type: {propertyType.Name} not supported.");
                }

                _properties[property.Index] = property.Property;
                _propertySetters[property.Index] = _type.PropertySet(property.Property.Name);
            }


            _propertyCount = _properties.Length;

            _IsNullable = _isNullable.ToImmutableArray();
            _IsAssigned = _isAssigned.ToImmutableArray();
            _Properties = _properties.ToImmutableArray();
            _PropertySettersEnum = _propertySettersEnum.ToImmutableArray();
            _PropertySetters = _propertySetters.ToImmutableArray();
            _PropertySettersString = _propertySettersString.ToImmutableArray();
            _PropertySettersGuid = _propertySettersGuid.ToImmutableArray();
            _PropertySettersBoolean = _propertySettersBoolean.ToImmutableArray();
            _PropertySettersDateTime = _propertySettersDateTime.ToImmutableArray();
            _PropertySettersDateTimeOffset = _propertySettersDateTimeOffset.ToImmutableArray();
            _PropertySettersTimeSpan = _propertySettersTimeSpan.ToImmutableArray();
            _PropertySettersByte = _propertySettersByte.ToImmutableArray();
            _PropertySettersSByte = _propertySettersSByte.ToImmutableArray();
            _PropertySettersInt16 = _propertySettersInt16.ToImmutableArray();
            _PropertySettersInt32 = _propertySettersInt32.ToImmutableArray();
            _PropertySettersInt64 = _propertySettersInt64.ToImmutableArray();
            _PropertySettersSingle = _propertySettersSingle.ToImmutableArray();
            _PropertySettersDecimal = _propertySettersDecimal.ToImmutableArray();
            _PropertySettersDouble = _propertySettersDouble.ToImmutableArray();
            _PropertySettersUInt16 = _propertySettersUInt16.ToImmutableArray();
            _PropertySettersUInt32 = _propertySettersUInt32.ToImmutableArray();
            _PropertySettersUInt64 = _propertySettersUInt64.ToImmutableArray();
            _PropertySettersBigInteger = _propertySettersBigInteger.ToImmutableArray();
            _PropertySettersGuidNull = _propertySettersGuidNull.ToImmutableArray();
            _PropertySettersBooleanNull = _propertySettersBooleanNull.ToImmutableArray();
            _PropertySettersDateTimeNull = _propertySettersDateTimeNull.ToImmutableArray();
            _PropertySettersDateTimeOffsetNull = _propertySettersDateTimeOffsetNull.ToImmutableArray();
            _PropertySettersTimeSpanNull = _propertySettersTimeSpanNull.ToImmutableArray();
            _PropertySettersByteNull = _propertySettersByteNull.ToImmutableArray();
            _PropertySettersSByteNull = _propertySettersSByteNull.ToImmutableArray();
            _PropertySettersInt16Null = _propertySettersInt16Null.ToImmutableArray();
            _PropertySettersInt32Null = _propertySettersInt32Null.ToImmutableArray();
            _PropertySettersInt64Null = _propertySettersInt64Null.ToImmutableArray();
            _PropertySettersSingleNull = _propertySettersSingleNull.ToImmutableArray();
            _PropertySettersDecimalNull = _propertySettersDecimalNull.ToImmutableArray();
            _PropertySettersDoubleNull = _propertySettersDoubleNull.ToImmutableArray();
            _PropertySettersUInt16Null = _propertySettersUInt16Null.ToImmutableArray();
            _PropertySettersUInt32Null = _propertySettersUInt32Null.ToImmutableArray();
            _PropertySettersUInt64Null = _propertySettersUInt64Null.ToImmutableArray();
            _PropertySettersBigIntegerNull = _propertySettersBigIntegerNull.ToImmutableArray();
            _PropertyTypes = _propertyTypes.ToImmutableArray();

            base.InitImmutableArray();
            _IsInitializing = false;
        }

        private void InitCsvAttributeRead(Type type, int size)
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
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeString, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeGuid != null)
                {
                    __CustomParserGuid[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeGuid) as ICustomCsvParse<Guid>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeGuid, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeBoolean != null)
                {
                    __CustomParserBoolean[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeBoolean) as ICustomCsvParse<Boolean>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBoolean, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeDateTime != null)
                {
                    __CustomParserDateTime[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDateTime) as ICustomCsvParse<DateTime>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTime, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeDateTimeOffset != null)
                {
                    __CustomParserDateTimeOffset[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDateTimeOffset) as ICustomCsvParse<DateTimeOffset>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTimeOffset, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeTimeSpan != null)
                {
                    __CustomParserTimeSpan[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeTimeSpan) as ICustomCsvParse<TimeSpan>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeTimeSpan, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeByte != null)
                {
                    __CustomParserByte[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeByte) as ICustomCsvParse<Byte>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeByte, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeSByte != null)
                {
                    __CustomParserSByte[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeSByte) as ICustomCsvParse<SByte>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSByte, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt16 != null)
                {
                    __CustomParserInt16[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt16) as ICustomCsvParse<Int16>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt16, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt32 != null)
                {
                    __CustomParserInt32[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt32) as ICustomCsvParse<Int32>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt32, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt64 != null)
                {
                    __CustomParserInt64[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt64) as ICustomCsvParse<Int64>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt64, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeSingle != null)
                {
                    __CustomParserSingle[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeSingle) as ICustomCsvParse<Single>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSingle, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeDecimal != null)
                {
                    __CustomParserDecimal[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDecimal) as ICustomCsvParse<Decimal>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDecimal, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeDouble != null)
                {
                    __CustomParserDouble[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDouble) as ICustomCsvParse<Double>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDouble, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt16 != null)
                {
                    __CustomParserUInt16[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt16) as ICustomCsvParse<UInt16>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt16, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt32 != null)
                {
                    __CustomParserUInt32[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt32) as ICustomCsvParse<UInt32>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt32, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt64 != null)
                {
                    __CustomParserUInt64[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt64) as ICustomCsvParse<UInt64>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt64, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeBigInteger != null)
                {
                    __CustomParserBigInteger[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeBigInteger) as ICustomCsvParse<BigInteger>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBigInteger, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeGuidNullable != null)
                {
                    __CustomParserGuidNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeGuidNullable) as ICustomCsvParse<Guid?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeGuidNullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeBooleanNullable != null)
                {
                    __CustomParserBooleanNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeBooleanNullable) as ICustomCsvParse<Boolean?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBooleanNullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeDateTimeNullable != null)
                {
                    __CustomParserDateTimeNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDateTimeNullable) as ICustomCsvParse<DateTime?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTimeNullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeDateTimeOffsetNullable != null)
                {
                    __CustomParserDateTimeOffsetNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDateTimeOffsetNullable) as ICustomCsvParse<DateTimeOffset?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDateTimeOffsetNullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeTimeSpanNullable != null)
                {
                    __CustomParserTimeSpanNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeTimeSpanNullable) as ICustomCsvParse<TimeSpan?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeTimeSpanNullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeByteNullable != null)
                {
                    __CustomParserByteNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeByteNullable) as ICustomCsvParse<Byte?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeByteNullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeSByteNullable != null)
                {
                    __CustomParserSByteNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeSByteNullable) as ICustomCsvParse<SByte?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSByteNullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt16Nullable != null)
                {
                    __CustomParserInt16Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt16Nullable) as ICustomCsvParse<Int16?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt16Nullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt32Nullable != null)
                {
                    __CustomParserInt32Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt32Nullable) as ICustomCsvParse<Int32?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt32Nullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeInt64Nullable != null)
                {
                    __CustomParserInt64Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeInt64Nullable) as ICustomCsvParse<Int64?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeInt64Nullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeSingleNullable != null)
                {
                    __CustomParserSingleNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeSingleNullable) as ICustomCsvParse<Single?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeSingleNullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeDecimalNullable != null)
                {
                    __CustomParserDecimalNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDecimalNullable) as ICustomCsvParse<Decimal?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDecimalNullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeDoubleNullable != null)
                {
                    __CustomParserDoubleNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeDoubleNullable) as ICustomCsvParse<Double?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeDoubleNullable, "Read", typeof(StringBuilder));

                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt16Nullable != null)
                {
                    __CustomParserUInt16Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt16Nullable) as ICustomCsvParse<UInt16?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt16Nullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt32Nullable != null)
                {
                    __CustomParserUInt32Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt32Nullable) as ICustomCsvParse<UInt32?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt32Nullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeUInt64Nullable != null)
                {
                    __CustomParserUInt64Nullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeUInt64Nullable) as ICustomCsvParse<UInt64?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeUInt64Nullable, "Read", typeof(StringBuilder));
                }
                if (_CsvAttribute.DefaultCustomParserTypeBigIntegerNullable != null)
                {
                    __CustomParserBigIntegerNullable[index] = Activator.CreateInstance(_CsvAttribute.DefaultCustomParserTypeBigIntegerNullable) as ICustomCsvParse<BigInteger?>;
                    __CustomParserCall[index] = DelegateFactory.InstanceMethod(_CsvAttribute.DefaultCustomParserTypeBigIntegerNullable, "Read", typeof(StringBuilder));
                }
            }
        }

    }
}
