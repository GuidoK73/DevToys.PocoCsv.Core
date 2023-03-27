using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    ///
    /// </summary>
    internal sealed class CsvStreamHelper
    {
        /// <summary>
        ///
        /// </summary>
        public char Separator { get; set; } = ',';

        private enum State
        {
            First = 0,
            Normal = 1,
            Escaped = 2
        }

        private readonly StringBuilder _sb = new(127);
        private char _char;
        internal int _byte = 0;

        public bool EndOfStream => _byte == -1;

        //  \r = CR(Carriage Return) → Used as a new line character in Mac OS before X
        //  \n = LF(Line Feed) → Used as a new line character in Unix/Mac OS X
        //  \r\n = CR + LF → Used as a new line character in Windows

        /// <summary>
        /// Read CSV row from stream
        /// </summary>
        /// <param name="stream"></param>
        public IEnumerable<string> ReadRow(Stream stream)
        {
            var _state = State.First;
            _sb.Length = 0;
            _byte = 0;
            int _column = 0;
            bool _trimLast = false;
            bool _rowEnd = false;

            while (_byte > -1)
            {
                _byte = stream.ReadByte();
                _char = (char)_byte;
                if (_byte == -1)
                {
                    _rowEnd = true;
                }
                if ((_state == State.Normal && _char == '\r'))
                {
                    // PEEK
                    _byte = stream.ReadByte();
                    _char = (char)_byte;
                    if (_char != '\n')
                    {
                        _rowEnd = true; // We have a single \r without following \n.
                    }
                    // IF char is a normal char we can just continue from this point.
                }
                if ((_state == State.Normal && _char == '\n'))
                {
                    _rowEnd = true; // WE HAVE A SINGLE \n either preceded or not by \r
                }
                if (_rowEnd)
                {
                    if (_trimLast)
                    {
                        if (_sb.Length > 0 && _sb[_sb.Length - 1] == '"')
                        {
                            _sb.Length--;
                        }
                    }
                    yield return _sb.ToString();
                    break; // END
                }
                if (_state == State.First)
                {
                    if (_trimLast)
                    {
                        if (_sb.Length > 0)
                        {
                            _sb.Length--;
                        }
                        _trimLast = false;
                    }
                    _state = State.Normal;
                    if (_char == '\n')
                    {
                        continue;
                    }
                }
                if (_char == Separator)
                {
                    if (_state == State.Normal)
                    {
                        if (_trimLast)
                        {
                            if (_sb.Length > 0 && _sb[_sb.Length - 1] == '"')
                            {
                                _sb.Length--;
                            }
                        }
                        yield return _sb.ToString();
                        _column++;
                        _sb.Length = 0;
                        continue; // NEXT FIELD
                    }
                }
                if (_char == '"')
                {
                    _state = (_state == State.Normal) ? State.Escaped : State.Normal;
                    if (_sb.Length == 0)
                    {
                        _trimLast = true; // .Trim() is costly on large sets.
                        continue;
                    }
                }
                _sb.Append(_char);
            }
        }

        /// <summary>
        /// Skip a CSV row
        /// </summary>
        /// <param name="stream"></param>
        public void Skip(Stream stream)
        {
            var _state = State.Normal;
            _byte = 0;

            while (_byte > -1)
            {
                _byte = stream.ReadByte();
                _char = (char)_byte;
                if (_byte == -1)
                {
                    break;
                }
                if ((_state == State.Normal && _char == '\r'))
                {
                    // PEEK
                    _byte = stream.ReadByte();
                    _char = (char)_byte;
                    if (_char != '\n')
                    {
                        break;
                    }
                }
                if ((_state == State.Normal && _char == '\n'))
                {
                    break;
                }
                if (_char == '"')
                {
                    _state = (_state == State.Normal) ? State.Escaped : State.Normal;
                }
            }
        }

        private FixedQueue<long> _takeLastQueue;

        public void SeekLast(Stream stream, int rows)
        {
            _takeLastQueue = new FixedQueue<long>(rows);

            var _state = State.Normal;
            _byte = 0;

            while (_byte > -1)
            {
                _byte = stream.ReadByte();

                _char = (char)_byte;
                if (_byte == -1)
                {
                    break;
                }
                if ((_state == State.Normal && _char == '\r'))
                {
                    // PEEK
                    _byte = stream.ReadByte();
                    _char = (char)_byte;
                    if (_char != '\n')
                    {
                        _takeLastQueue.Add(stream.Position);
                    }
                }
                if ((_state == State.Normal && _char == '\n'))
                {
                    _takeLastQueue.Add(stream.Position);
                    continue;
                }

                if (_char == '"')
                {
                    _state = (_state == State.Normal) ? State.Escaped : State.Normal;
                }
            }

            stream.Position = _takeLastQueue.GetCollection()[0];
        }
    }
}