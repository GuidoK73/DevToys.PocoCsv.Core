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

        public string Write(bool value) => value == true ? "1" : "0";
    }

    public class ParsePathCompressor : ICustomCsvParse<string>
    {
        StringBuilder _ResultBuilder = new StringBuilder(256);

        private char[] _PreviousEncode = null;

        private char[] _PreviousDecode = null;

        public string Read(StringBuilder source)
        {
            _ResultBuilder.Clear();
            string _value = source.ToString();

            if (_PreviousDecode == null)
            {
                _PreviousDecode = source.ToString().ToCharArray();
                return source.ToString();
            }

            int _index = _value.IndexOf('%');
            if (_index > -1)
            {
                string _part = _value.Substring(0, _index);
                bool _succes = int.TryParse(_part, out int _length);
                if (_succes)
                {
                    _ResultBuilder.Append(_PreviousDecode, 0, _length);
                    _ResultBuilder.Append(_value, _index + 1, _value.Length - _index - 1);
                    _PreviousDecode = _ResultBuilder.ToString().ToCharArray();
                    return _ResultBuilder.ToString();
                }
            }

            _PreviousDecode = _value.ToCharArray();
            return _value;
        }

        public string Write(string value)
        {
            _ResultBuilder.Clear();
            bool exited = false;

            if (_PreviousEncode == null)
            {
                _PreviousEncode = value.ToCharArray();
                return value.ToString();
            }
            char[] _currentEncode = value.ToCharArray();
            int position = 0;

            for (int index = 0; ; index++)
            {
                if (index < _PreviousEncode.Length && index < _currentEncode.Length)
                {
                    if (_currentEncode[index] == _PreviousEncode[index] && exited == false)
                    {
                        position++;
                    }
                    else
                    {
                        exited = true;
                        if (position > 0)
                        {
                            _ResultBuilder.Append(position.ToString());
                            _ResultBuilder.Append("%");
                            position = 0;
                        }
                        _ResultBuilder.Append(_currentEncode[index]);
                    }
                }
                else if (index >= _PreviousEncode.Length && index < _currentEncode.Length)
                {
                    // Prev is shorter
                    if (position > 0)
                    {
                        _ResultBuilder.Append(position.ToString());
                        _ResultBuilder.Append("%");
                        position = 0;
                    }
                    _ResultBuilder.Append(_currentEncode[index]);
                }
                else if (index >= _currentEncode.Length)
                {
                    // Current is shorter
                    break;
                }
            }
            _PreviousEncode = value.ToCharArray();
            return _ResultBuilder.ToString();
        }
    }
}
