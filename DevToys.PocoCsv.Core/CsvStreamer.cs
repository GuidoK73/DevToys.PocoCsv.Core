using System;
using System.IO;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// 
    /// </summary>
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

        private readonly StringBuilder _sb = new(127);
        private char _char;
        private int _byte;

        /// <summary>
        /// Read CSV row from stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="columnRead">Action executes for each column</param>
        public void ReadRow(Stream stream, Action<int, string> columnRead)
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
                        if (_sb.Length > 0 && _sb[_sb.Length] == '"')
                        {
                            _sb.Length--;
                        }
                        _trimLast = false;
                    }
                    columnRead(_column, _sb.ToString());
                    _column++;
                    break;
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
                    if (_sb.Length == 0)
                    {
                        _trimLast = true; // .Trim() is costly on large sets. so we will change the length of the stringbuilder and skip adding first escape char.
                        continue;
                    }
                }

                _sb.Append(_char);
            }
        }
    }
}
