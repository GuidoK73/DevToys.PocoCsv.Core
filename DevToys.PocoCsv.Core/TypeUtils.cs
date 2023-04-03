using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    internal static class TypeUtils
    {
        public const char _DOUBLEQUOTE = '"';
        public const char _SINGLEQUOTE = '\'';

        public static bool HasInterface(Type type, Type interfacetype) => (type.GetInterfaces().Where(p => p == interfacetype).FirstOrDefault() != null);

        public static bool HasInterface<T>(Type type) => (type.GetInterfaces().Where(p => p == typeof(T)).FirstOrDefault() != null);


        public static bool IsNumericType(Type type) => IsNumericType(GetNetType(type));

        public static bool IsNumericType(NetType netType)
        {
            switch (netType)
            {
                case NetType.Byte:
                case NetType.SByte:
                case NetType.Int16:
                case NetType.Int32:
                case NetType.Int64:
                case NetType.Single:
                case NetType.Decimal:
                case NetType.Double:
                case NetType.UInt16:
                case NetType.UInt32:
                case NetType.UInt64:
                    return true;
                default:
                    break;
            }
            return false;
        }

        public static int GetNumericTypeLevel(NetType netType)
        {
            switch (netType)
            {
                case NetType.Byte:
                    return 1;

                case NetType.SByte:
                    return 2;

                case NetType.Int16:
                    return 3;

                case NetType.Int32:
                    return 4;

                case NetType.Int64:
                    return 5;

                case NetType.Single:
                    return 6;

                case NetType.Decimal:
                    return 7;

                case NetType.Double:
                    return 8;

                case NetType.UInt16:
                    return 9;

                case NetType.UInt32:
                    return 10;

                case NetType.UInt64:
                    return 11;

                default:
                    break;
            }
            return 0;
        }

        public static Type GetType(NetType netType)
        {
            switch (netType)
            {
                case NetType.Unknown:
                    return typeof(object);

                case NetType.String:
                    return typeof(String);

                case NetType.Guid:
                    return typeof(Guid);

                case NetType.Boolean:
                    return typeof(Boolean);

                case NetType.DateTime:
                    return typeof(DateTime);

                case NetType.DateTimeOffset:
                    return typeof(DateTimeOffset);

                case NetType.TimeSpan:
                    return typeof(TimeSpan);

                case NetType.Byte:
                    return typeof(Byte);

                case NetType.SByte:
                    return typeof(SByte);

                case NetType.Int16:
                    return typeof(Int16);

                case NetType.Int32:
                    return typeof(Int32);

                case NetType.Int64:
                    return typeof(Int64);

                case NetType.Single:
                    return typeof(Single);

                case NetType.Decimal:
                    return typeof(Decimal);

                case NetType.Double:
                    return typeof(Double);

                case NetType.UInt16:
                    return typeof(UInt16);

                case NetType.UInt32:
                    return typeof(UInt32);

                case NetType.UInt64:
                    return typeof(UInt64);

                case NetType.Enum:
                    return typeof(Enum);

                default:
                    break;
            }
            return typeof(object);
        }

        public static NetType GetNetType(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type.IsEnum)
                return NetType.Enum;

            if (type == typeof(String))
                return NetType.String; // Alternative: char[], char, NText, NChar, NText, NVarChar, Varchar

            if (type == typeof(Guid))
                return NetType.Guid;

            if (type == typeof(Boolean))
                return NetType.Boolean;

            if (type == typeof(DateTime))
                return NetType.DateTime; // Alternative: SmallDateTime or DateTime2

            if (type == typeof(DateTimeOffset))
                return NetType.DateTimeOffset;

            if (type == typeof(TimeSpan))
                return NetType.TimeSpan;

            if (type == typeof(Byte))
                return NetType.Byte;

            if (type == typeof(SByte))
                return NetType.SByte;

            if (type == typeof(Int16))
                return NetType.Int16;

            if (type == typeof(Int32))
                return NetType.Int32;

            if (type == typeof(Int64))
                return NetType.Int64;

            if (type == typeof(UInt16))
                return NetType.UInt16; // one larger to fit unsigned maximum

            if (type == typeof(UInt32))
                return NetType.UInt32; // one larger to fit unsigned maximum

            if (type == typeof(UInt64))
                return NetType.UInt64; // one larger to fit unsigned maximum

            if (type == typeof(Single))
                return NetType.Single;

            if (type == typeof(Decimal))
                return NetType.Decimal; // Alternative: Money, SmallMoney

            if (type == typeof(Double))
                return NetType.Double;

            return NetType.String;
        }

        public static object Convert(object value, Type target, CultureInfo culture)
        {
            target = Nullable.GetUnderlyingType(target) ?? target;
            return (target.IsEnum) ? Enum.Parse(target, value.ToString()) : System.Convert.ChangeType(value, target, culture);
        }


        /// <summary>
        /// For use in array of string values, it can be used to detect the best fitting type for a string value.
        /// </summary>
        public static NetType BestNetType(NetType currentType, string newValue)
        {
            object _value = TypeUtils.GetTypedValue(newValue);
            NetType _newType = TypeUtils.GetNetType(_value.GetType());
            if (string.IsNullOrEmpty(newValue))
            {
                return NetType.Null;
            }
            if (currentType == NetType.Unknown)
            {
                return _newType;
            }
            if (TypeUtils.IsNumericType(currentType) && TypeUtils.IsNumericType(_newType))
            {
                int _newTypeLevel = TypeUtils.GetNumericTypeLevel(_newType);
                int _currentTypeLevel = TypeUtils.GetNumericTypeLevel(currentType);
                if (_newTypeLevel >= _currentTypeLevel)
                {
                    return _newType;
                }
                return currentType;
            }
            if (_newType != currentType && currentType == NetType.Null)
            {
                return _newType;
            }
            if (_newType != currentType && _newType == NetType.Null)
            {
                return currentType;
            }

            if (_newType != currentType)
            {
                return NetType.String;
            }

            return currentType;
        }

        /// <summary>
        /// Returns a type value for a given string value.
        /// </summary>
        public static object GetTypedValue(string value, bool precise = false)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }

            CultureInfo _culture = CultureInfo.InvariantCulture;

            bool _succes = DateTime.TryParse(value.Trim(), _culture, DateTimeStyles.None, out DateTime _date);
            if (_succes)
            {
                return _date;
            }
            _succes = Guid.TryParse(value.Trim(), out Guid _guid);
            if (_succes)
            {
                return _guid;
            }
            _succes = Boolean.TryParse(value.Trim(), out bool _boolean);
            if (_succes)
            {
                return _boolean;
            }
            if (precise)
            {
                _succes = Byte.TryParse(value.Trim(), out byte _byte);
                if (_succes)
                {
                    return _byte;
                }
            }

            if (precise)
            {
                _succes = Int16.TryParse(value.Trim(), out Int16 _int16); // short
                if (_succes)
                {
                    return _int16;
                }
            }

            _succes = Int32.TryParse(value.Trim(), out Int32 _int32); // int
            if (_succes)
            {
                return _int32;
            }

            if (precise)
            {
                _succes = Int64.TryParse(value.Trim(), out Int64 _int64); // long
                if (_succes)
                {
                    return _int64;
                }
            }

            if (precise)
            {
                _succes = Double.TryParse(value.Trim(), out Double _double); // long
                if (_succes)
                {
                    return _double;
                }
                _succes = Single.TryParse(value.Trim(), out Single _single);
                if (_succes)
                {
                    return _single;
                }
            }

            _succes = Decimal.TryParse(value.Trim(), NumberStyles.Any, _culture, out Decimal _decimal);
            if (_succes)
            {
                return _decimal;
            }
            _succes = TimeSpan.TryParse(value.Trim(), out TimeSpan _timespan);
            if (_succes)
            {
                return _timespan;
            }

            return value;
        }

        public static object GetTypedValue(string value, NetType nettype)
        {
            CultureInfo _culture = CultureInfo.InvariantCulture;
            bool _succes;
            switch (nettype)
            {
                case NetType.DateTime:
                    _succes = DateTime.TryParse(value.Trim(), _culture, DateTimeStyles.None, out DateTime _date);
                    if (_succes)
                    {
                        return _date;
                    }
                    if (string.IsNullOrEmpty(value))
                    {
                        return null;
                    }
                    break;

                case NetType.Guid:
                    _succes = Guid.TryParse(value.Trim(), out Guid _guid);
                    if (_succes)
                    {
                        return _guid;
                    }
                    if (string.IsNullOrEmpty(value))
                    {
                        return null;
                    }
                    break;

                case NetType.Boolean:
                    _succes = Boolean.TryParse(value.Trim(), out bool _boolean);
                    if (_succes)
                    {
                        return _boolean;
                    }
                    if (string.IsNullOrEmpty(value))
                    {
                        return null;
                    }
                    break;

                case NetType.Byte:
                    _succes = Byte.TryParse(value.Trim(), out byte _byte);
                    if (_succes)
                    {
                        return _byte;
                    }
                    if (string.IsNullOrEmpty(value))
                    {
                        return null;
                    }
                    break;

                case NetType.Single:
                    _succes = Single.TryParse(value.Trim(), out Single _single);
                    if (_succes)
                    {
                        return _single;
                    }
                    if (string.IsNullOrEmpty(value))
                    {
                        return null;
                    }
                    break;

                case NetType.Int16:
                    _succes = Int16.TryParse(value.Trim(), out Int16 _int16); // short
                    if (_succes)
                    {
                        return _int16;
                    }
                    if (string.IsNullOrEmpty(value))
                    {
                        return null;
                    }
                    break;

                case NetType.Int32:
                    _succes = Int32.TryParse(value.Trim(), out Int32 _int32); // int
                    if (_succes)
                    {
                        return _int32;
                    }
                    if (string.IsNullOrEmpty(value))
                    {
                        return null;
                    }
                    break;

                case NetType.Int64:
                    _succes = Int64.TryParse(value.Trim(), out Int64 _int64); // long
                    if (_succes)
                    {
                        return _int64;
                    }
                    if (string.IsNullOrEmpty(value))
                    {
                        return null;
                    }
                    break;

                case NetType.Double:
                    _succes = Double.TryParse(value.Trim(), out Double _double); // long
                    if (_succes)
                    {
                        return _double;
                    }
                    if (string.IsNullOrEmpty(value))
                    {
                        return null;
                    }
                    break;

                case NetType.Decimal:
                    _succes = Decimal.TryParse(value.Trim(), NumberStyles.Any, _culture, out Decimal _decimal);
                    if (_succes)
                    {
                        return _decimal;
                    }
                    if (string.IsNullOrEmpty(value))
                    {
                        return null;
                    }
                    break;

                case NetType.TimeSpan:
                    _succes = TimeSpan.TryParse(value.Trim(), out TimeSpan _timespan);
                    if (_succes)
                    {
                        return _timespan;
                    }
                    if (string.IsNullOrEmpty(value))
                    {
                        return null;
                    }
                    break;

                case NetType.String:
                    if (string.IsNullOrEmpty(value))
                    {
                        return null;
                    }
                    return value;
            }
            return value;
        }

        public static string CSharpTypeConstructorCode(object value, NetType netType)
        {
            if (value == null)
            {
                return "null";
            }
            switch (netType)
            {
                case NetType.DateTimeOffset:
                case NetType.DateTime:
                    DateTime _datetime = (DateTime)value;
                    if (_datetime.Hour == 0 && _datetime.Minute == 0 && _datetime.Second == 0)
                    {
                        return $"new DateTime({_datetime.Year},{_datetime.Month}, {_datetime.Day})";
                    }
                    if (_datetime.Second == 0)
                    {
                        return $"new DateTime({_datetime.Year},{_datetime.Month}, {_datetime.Day}, {_datetime.Hour}, {_datetime.Minute} )";
                    }
                    return $"new DateTime({_datetime.Year},{_datetime.Month}, {_datetime.Day}, {_datetime.Hour}, {_datetime.Minute}, {_datetime.Second} )";

                case NetType.String:
                    return $"\"{ value.ToString().Replace("\"", "\\\"") }\"";

                case NetType.Boolean:
                    return value.ToString().ToLower();

                case NetType.TimeSpan:
                    TimeSpan _ts = (TimeSpan)value;
                    return $"new TimeSpan({_ts.Days},{_ts.Hours},{_ts.Minutes},{_ts.Seconds},{_ts.Milliseconds} )";

                case NetType.Int16:
                case NetType.Int32:
                case NetType.Int64:
                case NetType.Byte:
                    return value.ToString();

                case NetType.Decimal:
                    decimal _decimal = (decimal)value;
                    return _decimal.ToString(CultureInfo.InvariantCulture);

                case NetType.Double:
                    double _double = (double)value;
                    return _double.ToString(CultureInfo.InvariantCulture);

                case NetType.Single:
                    Single _single = (Single)value;
                    return _single.ToString(CultureInfo.InvariantCulture);

                case NetType.Guid:
                    Guid _guid = (Guid)value;
                    return $"Guid.Parse(\"{_guid}\")";
            }

            return value.ToString();
        }

        public static string ToSqlAscii(string value)
        {
            string s = NormalizeQuotes(value); // do not accept strange quotes.
            return s.Replace("'", "''");
        }

        public static string NormalizeQuotes(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            var _array = value.ToCharArray();
            for (int ii = 0; ii < _array.Length; ii++)
            {
                if (_array[ii] == '‘' || _array[ii] == '’' || _array[ii] == '‚')
                    _array[ii] = _SINGLEQUOTE;

                if (_array[ii] == '“' || _array[ii] == '”' || _array[ii] == '„' || _array[ii] == '¨')
                    _array[ii] = _DOUBLEQUOTE;
            }
            var _s = new string(_array);
            return _s.Replace("'", "''");
        }

        public static string SqlTypeConstructorCode(object value, NetType netType)
        {
            if (value == null)
            {
                return "null";
            }
            switch (netType)
            {
                case NetType.DateTimeOffset:
                case NetType.DateTime:
                    DateTime _datetime = (DateTime)value;
                    return String.Format("'{0:0000}{1:00}{2:00} {3:00}:{4:00}:{5:00}'", _datetime.Year, _datetime.Month, _datetime.Day, _datetime.Hour, _datetime.Minute, _datetime.Second);

                case NetType.TimeSpan:
                    TimeSpan _ts = (TimeSpan)value;
                    return _ts.Ticks.ToString();

                case NetType.String:
                    return $"'{ ToSqlAscii(value.ToString()) }'";

                case NetType.Boolean:
                    bool _bool = (bool)value;
                    if (_bool)
                        return "1";

                    return "0";

                case NetType.Int16:
                case NetType.Int32:
                case NetType.Int64:
                case NetType.Byte:
                    return value.ToString();

                case NetType.Decimal:
                    decimal _decimal = (decimal)value;
                    return _decimal.ToString(CultureInfo.InvariantCulture);

                case NetType.Double:
                    double _double = (double)value;
                    return _double.ToString(CultureInfo.InvariantCulture);

                case NetType.Single:
                    Single _single = (Single)value;
                    return _single.ToString(CultureInfo.InvariantCulture);

                case NetType.Guid:
                    Guid _guid = (Guid)value;
                    return $"'{_guid}'";
            }

            return value.ToString();
        }

        /// <summary>
        /// Returns SqlDBType for a primitive C# type.
        /// </summary>
        public static DbType GetDbType(NetType type)
        {
            if (type == NetType.String)
                return DbType.String; // Alternative: char[], char, NText, NChar, NText, NVarChar, Varchar

            if (type == NetType.Guid)
                return DbType.Guid;

            if (type == NetType.Boolean)
                return DbType.Boolean;

            if (type == NetType.DateTime)
                return DbType.DateTime; // Alternative: SmallDateTime or DateTime2

            if (type == NetType.DateTimeOffset)
                return DbType.DateTimeOffset;

            if (type == NetType.TimeSpan)
                return DbType.Time;

            if (type == NetType.Byte)
                return DbType.Byte;

            if (type == NetType.SByte)
                return DbType.SByte;

            if (type == NetType.Int16)
                return DbType.Int16;

            if (type == NetType.Int32)
                return DbType.Int32;

            if (type == NetType.Int64)
                return DbType.Int64;

            if (type == NetType.UInt16)
                return DbType.UInt16; // one larger to fit unsigned maximum

            if (type == NetType.UInt32)
                return DbType.UInt32; // one larger to fit unsigned maximum

            if (type == NetType.UInt64)
                return DbType.UInt64; // one larger to fit unsigned maximum

            if (type == NetType.Single)
                return DbType.Single;

            if (type == NetType.Decimal)
                return DbType.Decimal; // Alternative: Money, SmallMoney

            if (type == NetType.Double)
                return DbType.Double;

            if (type == NetType.Enum)
                return DbType.Int32;

            return DbType.String;
        }

        public static SqlDbType GetSqlDbType(NetType type)
        {
            switch (type)
            {
                case NetType.Int64:
                    return SqlDbType.BigInt;

                case NetType.Boolean:
                    return SqlDbType.Bit;

                case NetType.DateTime:
                    return SqlDbType.DateTime;

                case NetType.Decimal:
                    return SqlDbType.Decimal;

                case NetType.Single:
                    return SqlDbType.Real;

                case NetType.Int32:
                    return SqlDbType.Int;

                case NetType.String:
                    return SqlDbType.NVarChar;

                case NetType.Int16:
                    return SqlDbType.SmallInt;

                case NetType.TimeSpan:
                    return SqlDbType.Time;

                case NetType.Byte:
                    return SqlDbType.TinyInt;

                case NetType.Guid:
                    return SqlDbType.UniqueIdentifier;

                default:
                    break;
            }

            return SqlDbType.NVarChar;
        }

        public static string SqlTypeDefinition(NetType type, string name, bool allowNull, bool autoIncrement, int columnLength)
        {
            var sb = new StringBuilder();

            sb.Append('[');
            sb.Append(name);
            sb.Append("] ");

            string _columnLength = columnLength == 0 ? "MAX" : columnLength.ToString();

            switch (type)
            {
                case NetType.String:
                    sb.Append($"NVARCHAR({_columnLength}) ");
                    break;

                case NetType.Guid:
                    sb.Append($"UNIQUEIDENTIFIER ");
                    break;

                case NetType.Boolean:
                    sb.Append($"BIT ");
                    break;

                case NetType.DateTime:
                    sb.Append($"DATETIME ");
                    break;

                case NetType.DateTimeOffset:
                    sb.Append($"DATETIMEOFFSET ");
                    break;

                case NetType.TimeSpan:
                    sb.Append($"BIGINT -- ( TimeSpan.Ticks ) ");
                    break;

                case NetType.Byte:
                    sb.Append($"TINYINT ");
                    break;
                //case NetType.ByteArray:
                //    sb.Append($"IMAGE ");
                //    break;
                case NetType.Int16:
                    sb.Append($"SMALLINT ");
                    break;

                case NetType.Int32:
                    sb.Append($"INT ");
                    break;

                case NetType.Int64:
                    sb.Append($"BIGINT ");
                    break;

                case NetType.Single:
                    sb.Append($"REAL ");
                    break;

                case NetType.Double:
                    sb.Append($"FLOAT ");
                    break;

                case NetType.Decimal:
                    sb.Append($"DECIMAL(28,8) ");
                    break;
            }

            if (autoIncrement)
                sb.Append($"IDENTITY(1,1) ");

            if (allowNull)
                sb.Append("NULL");
            else
                sb.Append("NOT NULL");

            return sb.ToString();
        }
    }
}