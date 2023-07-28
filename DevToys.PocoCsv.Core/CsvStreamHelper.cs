using System.IO;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    ///
    /// </summary>
    internal sealed class CsvStreamHelper
    {
        private char _char;
        internal int _byte = 0;
        const char _CR = '\r';
        const char _LF = '\n';
        const char _ESCAPE = '"';
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
        /// Current Line
        /// </summary>
        public int CurrentLine { get; set; }

        /// <summary>
        /// Move stream to a position.
        /// </summary>
        internal void MoveToPosition(Stream stream, long position)
        {
            stream.Position = position;
            _byte = 0;
        }

        /// <summary>
        /// Move stream to start position.
        /// </summary>
        public void MoveToStart(Stream stream) => MoveToPosition(stream, 0);

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
                        stream.Position--;
                        CurrentLine++;
                        break;
                    }
                }
                if ((_state == State.Normal && _char == _LF))
                {
                    CurrentLine++;
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
            MoveToStart(stream);

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
                        stream.Position--;
                        CurrentLine++;
                        _takeLastQueue.Add(stream.Position);
                    }
                }
                if ((_state == State.Normal && _char == _LF))
                {
                    _takeLastQueue.Add(stream.Position);
                    CurrentLine++;
                    continue;
                }
                if (_char == _ESCAPE)
                {
                    _state = (_state == State.Normal) ? State.Escaped : State.Normal;
                }
            }
            _byte = -2;


            var _queuePosition = _takeLastQueue.GetQueue();
            CurrentLine -= _queuePosition.Length;
            stream.Position = _queuePosition[0]; // Get first position of Queue to move to the file position of last x rows.
        }
    }
}