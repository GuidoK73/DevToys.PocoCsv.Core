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
        private readonly StringBuilder _sbValue = new(127);
        private char _char;
        internal int _byte = 0;
        private const char _CR = '\r';
        private const char _LF = '\n';
        private const char _ESCAPE = '"';
        private InfiniteLoopQueue<long> _takeLastQueue;

        /// <summary>
        /// Char separator to use default: ','
        /// </summary>
        public char Separator { get; set; } = ',';

        /// <summary>
        /// Indicates the stream has ended.
        /// </summary>
        public bool EndOfStream => _byte == -1;

        //  \r = CR(Carriage Return) → Used as a new line character in Mac OS before X
        //  \n = LF(Line Feed) → Used as a new line character in Unix/Mac OS X
        //  \r\n = CR + LF → Used as a new line character in Windows

        
        /// <summary>
        /// Move stream to a position.
        /// </summary>
        public void MoveToPosition(Stream stream, long position)
        {
            stream.Position = position;
            _byte = 0;
        }

        /// <summary>
        /// Move stream to start position.
        /// </summary>
        public void MoveToStart(Stream stream)
        {
            MoveToPosition(stream, 0);
        }

        /// <summary>
        /// Read CSV row from stream
        /// </summary>
        /// <param name="stream"></param>
        public IEnumerable<string> ReadRow(Stream stream)
        {
            var _state = State.Normal;
            _sbValue.Length = 0;
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
                else if ((_state == State.Normal && _char == _CR))
                {
                    // PEEK
                    _byte = stream.ReadByte();
                    _char = (char)_byte;
                    if (_char != _LF)
                    {
                        stream.Position--;
                        _rowEnd = true; // We have a single \r without following \n.
                    }
                    // IF char is a normal char we can just continue from this point.
                }
                if ((_state == State.Normal && _char == _LF))
                {
                    _rowEnd = true; // WE HAVE A SINGLE \n either preceded or not by \r
                }
                if (_rowEnd)
                {
                    if (_trimLast)
                    {
                        if (_sbValue.Length > 0 && _sbValue[_sbValue.Length - 1] == _ESCAPE)
                        {
                            _sbValue.Length--;
                        }
                    }
                    yield return _sbValue.ToString();
                    break; // END
                }
                if (_char == Separator)
                {
                    if (_state == State.Normal)
                    {
                        if (_trimLast)
                        {
                            if (_sbValue.Length > 0 && _sbValue[_sbValue.Length - 1] == _ESCAPE)
                            {
                                _sbValue.Length--;
                            }
                        }
                        yield return _sbValue.ToString();
                        _column++;
                        _sbValue.Length = 0;
                        continue; // NEXT FIELD
                    }
                }
                else if (_char == _ESCAPE)
                {
                    _state = (_state == State.Normal) ? State.Escaped : State.Normal;
                    if (_sbValue.Length == 0)
                    {
                        _trimLast = true; // .Trim() is costly on large sets.
                        continue;
                    }
                }
                _sbValue.Append(_char);
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
                else if ((_state == State.Normal && _char == _CR))
                {
                    // PEEK
                    _byte = stream.ReadByte();
                    _char = (char)_byte;
                    if (_char != _LF)
                    {
                        break;
                    }
                }
                if ((_state == State.Normal && _char == _LF))
                {
                    break;
                }
                if (_char == _ESCAPE)
                {
                    _state = (_state == State.Normal) ? State.Escaped : State.Normal;
                }
            }
        }

        /// <summary>
        /// Move to a last row position before number of rows.
        /// </summary>
        public void MoveToLast(Stream stream, int rows)
        {
            _takeLastQueue = new InfiniteLoopQueue<long>(rows);

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
                else if ((_state == State.Normal && _char == _CR))
                {
                    // PEEK
                    _byte = stream.ReadByte();
                    _char = (char)_byte;
                    if (_char != _LF)
                    {
                        _takeLastQueue.Add(stream.Position);
                    }
                }
                if ((_state == State.Normal && _char == _LF))
                {
                    _takeLastQueue.Add(stream.Position);
                    continue;
                }

                if (_char == _ESCAPE)
                {
                    _state = (_state == State.Normal) ? State.Escaped : State.Normal;
                }
            }

            stream.Position = _takeLastQueue.GetQueue()[0]; // Get first position of Queue to move to the file position of last x rows.
        }
    }
}