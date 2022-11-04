using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Enumerate Csv Stream Reader over dynamic. Slower for large sets.
    /// </summary>
    public sealed class CsvReaderDynamic : IDisposable
    {
        /// <summary>
        /// After Read, before Serialize. use this to prepare row values for serialization.
        /// </summary>
        private readonly string[] _CurrentRow = null;
        private readonly string _File = null;
        private CsvStreamReader _Reader;
        private string[] _FirstRow = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file"></param>
        public CsvReaderDynamic(string file)
        {
            _File = file;
        }

        /// <summary>
        /// Csv Seperator to use default ','
        /// </summary>
        public char Separator { get; set; } = ',';

        /// <summary>
        /// Indicates whether to look for byte order marks at the beginning of the file.
        /// </summary>
        public bool DetectEncodingFromByteOrderMarks { get; set; } = true;

        /// <summary>
        /// The character encoding to use.
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.Default;

        /// <summary>
        /// Dynamic property names will be as first header.
        /// </summary>
        public bool FirstRowIsHeader { get; set; } = true;

        /// <summary>
        /// Releases all resources used by the System.IO.TextReader object.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Close();
        }

        /// <summary>
        /// Initialize and open the CSV Stream Reader.
        /// </summary>
        public void Open()
        {
            _Reader = new CsvStreamReader(_File, Encoding, DetectEncodingFromByteOrderMarks) { Separator = Separator };
            Init();
        }

        /// <summary>
        /// Close the CSV stream reader
        /// </summary>
        public void Close() => _Reader.Close();

        /// <summary>
        /// Each iteration will read the next row.
        /// </summary>
        public IEnumerable<dynamic> Rows()
        {
            _Reader.BaseStream.Position = 0;

            int _rowNumber = -1;

            while (!_Reader.EndOfCsvStream)
            {
                _rowNumber++;
                yield return CreateObject(_Reader.ReadCsvLine().ToArray());
            }
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
            if (_Reader.BaseStream.Position == 0)
            {
                _result = _Reader.ReadCsvLine().ToArray();
                _Reader.BaseStream.Position = 0;
            }
            else
            {
                _result = _CurrentRow;
            }
            return _result;
        }

        private void Init()
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