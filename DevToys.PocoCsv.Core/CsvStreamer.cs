using System;
using System.IO;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    public sealed class CsvStreamer
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

        private readonly StringBuilder _sb = new(63);
        private char _char;
        private int _byte;

        public void ReadRow(Stream stream, Action<int, string> columnRead)
        {
            var _state = State.First;
            _sb.Length = 0;
            _byte = 0;
            int _column = 0;
            while (_byte > -1)
            {
                _byte = stream.ReadByte();
                _char = (char)_byte;
                if (_byte == -1 || (_state == State.Normal && (_char == '\r' || _char == '\n')))
                {
                    columnRead(_column, _sb.ToString().Trim('"'));
                    _column++;
                    break;
                }
                if (_state == State.First)
                {
                    _state = State.Normal;
                    if (_char == '\n')
                    {
                        continue;
                    }
                }
                if (_state == State.Normal && _char == Separator)
                {
                    columnRead(_column, _sb.ToString().Trim('"'));
                    _column++;
                    _sb.Length = 0;
                    continue;
                }
                if (_char == '"')
                {
                    _state = (_state == State.Normal) ? State.Escaped : State.Normal;
                }

                _sb.Append(_char);
            }
        }
    }
}
