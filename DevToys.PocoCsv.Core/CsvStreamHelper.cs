using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            while (_byte > -1)
            {
                _byte = stream.ReadByte();
                _char = (char)_byte;
                if (_byte == -1 || (_state == State.Normal && (_char == '\r' || _char == '\n')))
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
                if (_byte == -1 || (_state == State.Normal && (_char == '\r' || _char == '\n')))
                {
                    break;
                }
                if (_char == '"')
                {
                    _state = (_state == State.Normal) ? State.Escaped : State.Normal;
                }
            }
        }
    }
}