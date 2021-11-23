﻿using Delegates;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    public class CsvWriter<T> : IDisposable where T : new()
    {

        private readonly string _File = null;
        private Dictionary<int, PropertyInfo> _Properties = new();
        private Dictionary<int, Func<object, object>> _ActionGetters = new();

        private CsvStreamWriter _Writer;


        public CsvWriter(string file)
        {
            _File = file;
        }
        /// <summary>
        /// Culture info to use for serialization.
        /// </summary>
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        public char Separator { get; set; } = ',';

        public bool Append { get; set; } = true;

        public Encoding Encoding { get; set; } = Encoding.Default;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _Writer.Close();
        }

        public void Open()
        {
            Init();
            _Writer = new CsvStreamWriter(_File, Append, Encoding) { Separator = Separator };
        }

        public void Write(IEnumerable<T> rows)
        {
            if (Append)
            {
                FileInfo _info = new(_File);
                _Writer.BaseStream.Position = _info.Length;
            }
            else
            {
                _Writer.BaseStream.Position = 0;
            }

            List<int> columnIndexes = _Properties.Keys.ToList();
            int _max = columnIndexes.Max();

            string[] _data = new string[_max + 1];

            foreach (T item in rows)
            {
                for (int ii = 0; ii <= _max; ii++)
                {
                    if (_Properties.ContainsKey(ii))
                    {
                        var _actionGet = _ActionGetters[ii];
                        var _value = _actionGet(item);
                        _data[ii] = (string)Convert(_value, typeof(string), Culture);
                        //_data[ii] = (string)Convert(_Properties[ii].GetValue(item), typeof(string), Culture);
                    }
                    else
                    {
                        _data[ii] = string.Empty;
                    }
                }
                _Writer.WriteCsvLine(_data);
            }
        }

        private static object Convert(object value, Type target, CultureInfo culture)
        {
            target = Nullable.GetUnderlyingType(target) ?? target;
            return (target.IsEnum) ? Enum.Parse(target, value.ToString()) : System.Convert.ChangeType(value, target, culture);
        }

        private void Init()
        {
            if (_Properties.Count > 0)
                return;

            var _type = typeof(T);

            _Properties = _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => new { Value = p, Key = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index })
                .ToDictionary(p => p.Key, p => p.Value);

            _ActionGetters = _type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(ColumnAttribute)) != null)
                .Select(p => new { Value = p, Key = (p.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Index })
                .ToDictionary(p => p.Key, p => _type.PropertyGet(p.Value.Name));
        }
    }
}