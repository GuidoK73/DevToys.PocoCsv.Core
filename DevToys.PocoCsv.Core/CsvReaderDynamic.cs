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

            while (!EndOfStream)
            {
                _rowNumber++;
                yield return Read();
            }
        }

        public dynamic Read()
        {
            dynamic dataobject;
            var dataobject2 = new ExpandoObject();
            var dataobject3 = dataobject2 as IDictionary<string, Object>;

            _Streamer.ReadRow(_StreamReader.BaseStream, (columnIndex, value) =>
                {
                    var _name = FirstRowIsHeader ? CleanString(_FirstRow[columnIndex]) : _FirstRow[columnIndex];
                    var _value = value;
                    dataobject3.Add(_name, _value);
                }
            );

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
            List<string> _result = new();
            _Streamer.ReadRow(_StreamReader.BaseStream, (columnindex, value) => { _result.Add(value); });
            _StreamReader.BaseStream.Position = 0;
            return _result.ToArray(); ;
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

        /// <summary>
        /// Do a 10 line sample to detect and set separator, it will try ',', ';', '|', '\t', ':'
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
    }
}