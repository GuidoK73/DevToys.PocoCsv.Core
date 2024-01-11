using DevToys.PocoCsv.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TestProject2.Models;

namespace TestProject2
{
    [TestClass]
    public class TestCustomParserCompressor
    {
        [TestMethod()]
        public void TestCompressor()
        {

            var _p1 = new PathObject("Contracts[0].Plugins[0].Children[0].Actions[9].SubTasks[0].FileLists[1].Files[0].Items[0].AddressLine1", "", "", DiffType.DifferentValue, true);
            var _p2 = new PathObject("Contracts[0].Plugins[0].Children[0].Actions[9].SubTasks[0].FileLists[1].Files[0].Items[0].AddressLine10", "", "", DiffType.DifferentValue, true);
            var _p3 = new PathObject("Contracts[0].Plugins[0].Children[0].Actions[9].SubTasks[0].FileLists[1].Files[0].Items[0].AddressLine2", "", "", DiffType.DifferentValue, true);
            var _p4 = new PathObject("AddressLine5", "", "", DiffType.DifferentValue, true);           
            var _p5 = new PathObject("AddressLine6", "", "", DiffType.DifferentValue, true);
            var _p6 = new PathObject("Contracts[0].Plugins[0].Children[0].Actions[9].SubTasks[0].FileLists[1].Files[0].Items[0].AddressLine1", "", "", DiffType.DifferentValue, true);

            string file = System.IO.Path.GetTempFileName();

            using (CsvWriter<PathObject> _writer = new CsvWriter<PathObject>(file) { Separator = '~' })
            {
                _writer.Open();
                _writer.Write(_p1);
                _writer.Write(_p2);
                _writer.Write(_p3);
                _writer.Write(_p4);
                _writer.Write(_p5);
                _writer.Write(_p6);
            }

            string _text = File.ReadAllText(file);

            using (CsvReader<PathObject> _reader = new CsvReader<PathObject>(file) { Separator = '~' } )
            {
                _reader.Open();
                List<PathObject> _result = _reader.ReadAsEnumerable().ToList();
                Console.WriteLine("X");
            }
        }

        [TestMethod()]
        public void TestCompressor2()
        {
            string _file = @"D:\Temp\3a33bfa6-29c3-4a11-8b87-17dada117a46_03436024-6bb8-46c3-8bca-1fadd336b5dd_dcaa37c3-2ca9-41bf-b921-f5ddb77ac135_20240108135701.result";

            string _text = File.ReadAllText(_file);

            List<PathObject> _result = new List<PathObject>();

            using (CsvReader<PathObject> _reader = new CsvReader<PathObject>(_file) { Separator = '~' })
            {
                _reader.EmptyLineBehaviour = EmptyLineBehaviour.ThrowException;
                _reader.Open();

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                while (!_reader.EndOfStream)
                {
                    var _item = _reader.Read();
                    _result.Add(_item);
                }

                stopwatch.Stop();

                Console.WriteLine(stopwatch.Elapsed.ToString());
            }
        }
    }





    public enum DiffType
    {
        None,
        PathOnlyInFile1,
        PathOnlyInFile2,
        DifferentTokenType,
        DifferentValue,
    }

    public class PathObject
    {
        public PathObject()
        {
        }
        public PathObject(string path, string valueF1, string valueF2, DiffType diffType = DiffType.None, bool isExcluded = false)
        {
            Path = path;
            ValueF1 = valueF1;
            ValueF2 = valueF2;
            DiffType = diffType;
            //System.Diagnostics.Debug.WriteLine($"Path: {Path} ValueF1: {ValueF1} ValueF2: {ValueF2} DiffType: {diffType.ToString()}");
            IsExcluded = isExcluded;
        }
        [Column(Index = 0, CustomParserType = typeof(ParsePathCompressor))]
        public string Path { get; set; }
        [Column(Index = 1, CustomParserType = typeof(ParseBool))]
        public bool IsCollection { get; set; }
        [Column(Index = 2, CustomParserType = typeof(ParseBool))]
        public bool IsItem { get; set; }
        [Column(Index = 3)]
        public string ValueF1 { get; set; }
        [Column(Index = 4)]
        public string ValueF2 { get; set; }
        [Column(Index = 5)]
        public DiffType DiffType { get; set; }
        [Column(Index = 6, CustomParserType = typeof(ParseBool))]
        public bool IsExcluded { get; set; }

    }


    public class ParseBool : ICustomCsvParse<bool>
    {
        public bool Read(StringBuilder value)
        {
            switch (value.ToString())
            {
                case "True":
                case "1":
                    return true;
            }
            return false;
        }

        public void Reading(int colIndex, int cellPosition, char c)
        { }

        public string Write(bool value) => value == true ? "1" : "0";
    }

    public class ParsePathCompressor : ICustomCsvParse<string>
    {
        StringBuilder _ResultBuilder = new StringBuilder(256);

        StringBuilder _PrevBuilder = new StringBuilder(256);

        private char[] _PreviousEncode = null;



        public string Read(StringBuilder source)
        {
            string _value = _ResultBuilder.ToString();
            _PrevBuilder.Clear();
            _PrevBuilder.Append(_ResultBuilder);
            _ResultBuilder.Clear();
            return _value;
        }

        public void Reading(int colIndex, int cellPosition, char c)
        {
            if (c == '%')
            {
                string _number = _ResultBuilder.ToString();
                _ResultBuilder.Clear();
                bool _succes = int.TryParse(_number, out int _length);
                if (_succes)
                {
                    _ResultBuilder.Append(_PrevBuilder, 0, _length);
                }
                return;
            }

            _ResultBuilder.Append(c);
        }

        public string Write(string value)
        {
            _ResultBuilder.Clear();

            if (_PreviousEncode == null || _PreviousEncode.Length == 0)
            {
                _PreviousEncode = value.ToCharArray();
                return value.ToString();
            }

            char[] _currentEncode = value.ToCharArray();

            int _min = (_currentEncode.Length < _PreviousEncode.Length) ? _currentEncode.Length : _PreviousEncode.Length;
            int _position = 0;

            for (_position = 0; _position < _min; _position++)
            {
                if (_currentEncode[_position] != _PreviousEncode[_position])
                {
                    break;
                }
            }

            if (_position > 0)
            {
                _ResultBuilder.Append(_position.ToString());
                _ResultBuilder.Append("%");
                _ResultBuilder.Append(_currentEncode, _position, _currentEncode.Length - _position);
                _PreviousEncode = value.ToCharArray(); // #Should it be uncompressed?
                return _ResultBuilder.ToString();
            }

            _PreviousEncode = value.ToCharArray();
            return value;
        }
    }
}
