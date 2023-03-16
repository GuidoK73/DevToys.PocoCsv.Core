using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Enumerate Csv Stream Reader over dynamic. Slower for large sets.
    /// </summary>
    public sealed class CsvReaderDynamic : BaseCsvReader
    {
        /// <summary>
        /// After Read, before Serialize. use this to prepare row values for serialization.
        /// </summary>
        private readonly string[] _CurrentRow = null;
        private string[] _FirstRow = null;

        /// <summary>
        /// 
        /// </summary>
        public CsvReaderDynamic(string file) : base(file)
        { }

        /// <summary>
        /// 
        /// </summary>
        public CsvReaderDynamic(Stream stream) : base(stream)
        { }

        /// <summary>
        /// 
        /// </summary>
        public CsvReaderDynamic(Stream stream, Encoding encoding, char separator = ',', bool detectEncodingFromByteOrderMarks = true) : base(stream, encoding, separator, detectEncodingFromByteOrderMarks)
        { }

        /// <summary>
        /// 
        /// </summary>
        public CsvReaderDynamic(string file, Encoding encoding, char separator = ',', bool detectEncodingFromByteOrderMarks = true) : base(file, encoding, separator, detectEncodingFromByteOrderMarks)
        { }

        /// <summary>
        /// Dynamic property names will be as first header.
        /// </summary>
        public bool FirstRowIsHeader { get; set; } = true;


        /// <summary>
        /// Initialize and open the CSV Stream Reader.
        /// </summary>
        public override void Open()
        {
            if (_Stream == null && string.IsNullOrEmpty(_File))
            {
                throw new IOException("No file or stream specified in constructor.");
            }
            if (_Stream != null)
            {
                _StreamReader = new CsvStreamReader(stream: _Stream, encoding : Encoding, detectEncodingFromByteOrderMarks: DetectEncodingFromByteOrderMarks, bufferSize: _BufferSize) { Separator = Separator };
            }
            if (!string.IsNullOrEmpty(_File))
            {
                if (!File.Exists(_File))
                {
                    throw new FileNotFoundException($"File '{_File}' not found.");
                }
                _StreamReader = new CsvStreamReader(path: _File,encoding: Encoding, detectEncodingFromByteOrderMarks : DetectEncodingFromByteOrderMarks, bufferSize : _BufferSize) { Separator = Separator };
            }
            Init();
        }

        /// <summary>
        /// 
        /// </summary>
        [Obsolete("Use ReadAsEnumerable() or Read() instead.")]
        public IEnumerable<dynamic> Rows() => ReadAsEnumerable();

        /// <summary>
        /// Each iteration will read the next row from stream or file.
        /// </summary>
        public IEnumerable<dynamic> ReadAsEnumerable()
        {
            if (_StreamReader == null)
            {
                throw new IOException("Reader is closed!");
            }

            _StreamReader.BaseStream.Position = 0;

            int _rowNumber = -1;

            while (!_StreamReader.EndOfCsvStream)
            {
                _rowNumber++;
                yield return CreateObject(_StreamReader.ReadCsvLine().ToArray());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public dynamic Read()
        {
            if (_StreamReader == null)
            {
                throw new IOException("Reader is closed!");
            }

            return CreateObject(_StreamReader.ReadCsvLine().ToArray());
        }

        private dynamic CreateObject(string[] reader)
        {
            dynamic dataobject;
            var dataobject2 = new ExpandoObject();
            var dataobject3 = dataobject2 as IDictionary<string, Object>;

            for (int ii = 0; ii < _FirstRow.Length; ii++)
            {
                var _name = FirstRowIsHeader ? CleanString(_FirstRow[ii]) : _FirstRow[ii];
                var _value = (reader != null && reader.Length >= ii) ? reader[ii] : string.Empty;
                dataobject3.Add(_name, _value);
            }

            dataobject = dataobject3;
            return dataobject;
        }

        private static string CleanString(string name)
        {
            char[] chars = name.ToCharArray().Where(c => char.IsLetter(c) || c == '_' || c == ' ').Select(c => c == ' ' ? '_' : c).ToArray();
            return new string(chars);
        }

        /// <summary>
        /// Reads first row as a string Array
        /// </summary>
        public string[] SampleRow()
        {
            string[] _result;
            if (_StreamReader.BaseStream.Position == 0)
            {
                _result = _StreamReader.ReadCsvLine().ToArray();
                _StreamReader.BaseStream.Position = 0;
            }
            else
            {
                _result = _CurrentRow;
            }
            return _result;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Init()
        {
            if (_FirstRow != null)
                return;

            _FirstRow = SampleRow();
            if (!FirstRowIsHeader)
            {
                for (int ii = 0; ii < _FirstRow.Length; ii++)
                    _FirstRow[ii] = $"_{ii}";
            }
        }
    }
}