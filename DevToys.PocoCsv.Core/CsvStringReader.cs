using System.Collections.Generic;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Enumerate Csv string over T.
    /// Properties needs to be marked with ColumnAttribute
    /// </summary>
    public sealed class CsvStringReader<T> : BaseCsvReader<T> where T : class, new()
    {
        private const int _CR = '\r';
        private const int _ESCAPE = '"';
        private const int _LF = '\n';
        private const int _TERMINATOR = -1;
        private int _byte = 0;

        // column index.
        private int _colPosition = -1;

        private int _BufferSize = 1024;

        private StringBuilder _Data = new StringBuilder();

        // char position within column
        private int _linePosition = 0;

        private int _nextByte = 0;
        private int _Position = 0;

        private int _Separator = ',';
        private State _state = State.Normal;

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvStringReader(StringBuilder data, char separator = ',', int buffersize = 1024)
        {
            _Data = data;
            _Separator = separator;
            _BufferSize = buffersize;
        }

        /// <summary>
        /// Auto detect the separator.
        /// </summary>
        public bool DetectSeparator { get; set; } = false;

        /// <summary>
        /// How should empty lines be treated.
        /// </summary>
        public EmptyLineBehaviour EmptyLineBehaviour { get; set; } = EmptyLineBehaviour.DefaultInstance;

        /// <summary>
        /// Returns collection of error messages.
        /// </summary>
        public IEnumerable<CsvReadError> Errors => _Errors;

        /// <summary>
        /// Indicates there are read conversion errors.
        /// </summary>
        public bool HasErrors => _Errors.Count > 0;

        /// <summary>
        /// Csv Seperator to use default ','
        /// </summary>
        public char Separator
        {
            get => (char)_Separator;
            set => _Separator = value;
        }

        /// <summary>
        /// Each iteration will read the next row from stream or file
        /// </summary>
        public IEnumerable<T> ReadAsEnumerable()
        {
            InitSeparator();
            Init();

            int _endPosition = _Data.Length;
            while (_Position < _endPosition)
            {
                yield return Read();
            }
        }

        // Called each character.
        private void AppendChar(int position)
        {
            if (_ICustomCsvParseBase[_CollIndex] == null)
            {
                _buffer.Append((char)_byte);
                return;
            }
            _ICustomCsvParseBase[_CollIndex].Reading(_buffer, _CurrentLine, _CollIndex, position, _linePosition, _colPosition, (char)_byte);
        }

        private void InitSeparator()
        {
            if (_Properties == null)
            {
                if (DetectSeparator)
                {
                    CsvUtils.GetCsvSeparator(_Data, out char _separator);
                    _Separator = _separator;
                }
            }
        }

        private void MoveToStart()
        {
            _Position = 0;
        }

        private int Peek(ref StringBuilder data, int position)
        {
            if (position + 1 > data.Length - 1)
            {
                return _TERMINATOR;
            }
            return (int)data[position + 1];
        }

        /// <summary>
        /// Reads the Csv line into object of type T and advances to the next.
        /// </summary>
        //  Called each line.
        private T Read()
        {
            InitSeparator();
            Init();

            T _result = new T();

        SkipEmptyLineAndReadNext:

            _state = State.Normal;
            _buffer.Length = 0; // Clear the string buffer.
            _linePosition = 0;
            _byte = 0;
            _nextByte = 0;
            _CollIndex = 0;
            _colPosition = -1;
            int _resultPosition = _Position;

            for (int ii = _Position; ii < _Data.Length; ii++)
            {
                _resultPosition = ii;
                _byte = (int)_Data[ii];
                if (_state == State.Normal)
                {
                    if (_byte == _Separator)
                    {
                        // End of field
                        if (_CollIndex < _propertyCount && _IsAssigned[_CollIndex])
                        {
                            SetValue(_result);
                        }
                        _CollIndex++;
                        _buffer.Length = 0;
                        continue;
                    }
                    else if (_byte == _CR)
                    {
                        // in case of CR, peek next char, when next char is LF, then skip the CR and let newline happen on LF, otherwise newline happens on CR.
                        _nextByte = Peek(ref _Data, ii);
                        if (_nextByte == _LF)
                        {
                            continue;
                        }
                        // end of line.
                        if (_CollIndex < _propertyCount && _IsAssigned[_CollIndex])
                        {
                            SetValue(_result);
                        }
                        _CollIndex = 0;
                        _buffer.Length = 0;
                        _CurrentLine++;
                        break;
                    }
                    else if (_byte == _LF)
                    {
                        // end of line.
                        if (_CollIndex < _propertyCount && _IsAssigned[_CollIndex])
                        {
                            SetValue(_result);
                        }
                        _CollIndex = 0;
                        _buffer.Length = 0;
                        _CurrentLine++;
                        break;
                    }
                    else if (_byte == _ESCAPE)
                    {
                        // switch mode
                        _state = State.Escaped;
                        continue; // do not add this char. (TRIM)
                    }
                    else if (_byte == _TERMINATOR)
                    {
                        // End of field
                        if (_CollIndex < _propertyCount && _IsAssigned[_CollIndex])
                        {
                            SetValue(_result);
                        }
                        _buffer.Length = 0;
                        break; // end the while loop.
                    }
                    _linePosition++;
                    _colPosition++;
                    if (_CollIndex < _propertyCount)
                    {
                        AppendChar(ii);
                    }
                    continue;
                }
                else if (_state == State.Escaped)
                {
                    // ',' and '\r' and "" are part of the value.
                    if (_byte == _TERMINATOR)
                    {
                        // In a proper CSV this would not occur.
                        if (_CollIndex < _propertyCount && _IsAssigned[_CollIndex])
                        {
                            SetValue(_result);
                        }
                        _buffer.Clear();
                        break; // end the while loop.
                    }
                    else if (_byte == _ESCAPE)
                    {
                        // " aaa "" ","bbb", "ccc""","ddd """" "
                        _nextByte = Peek(ref _Data, ii);
                        if (_nextByte == _Separator || _nextByte == _CR || _nextByte == _LF)
                        {
                            // this quote is followed by a , so it ends the escape. we continue to next itteration where we read a ',' in nomral mode.
                            _state = State.Normal;
                            continue;
                        }
                        else if (_nextByte == _TERMINATOR)
                        {
                            if (_CollIndex < _propertyCount && _IsAssigned[_CollIndex])
                            {
                                SetValue(_result);
                            }
                            break;
                        }
                        else if (_nextByte == _ESCAPE)
                        {
                            _state = State.EscapedEscape;
                            continue; // Also do not add this char, we add it when we are in EscapedEscape mode and from their we turn back to normal Escape.  (basically adding one of two)
                        }
                    }
                    _linePosition++;
                    _colPosition++;
                    if (_CollIndex < _propertyCount)
                    {
                        AppendChar(ii);
                    }
                    continue;
                }
                else if (_state == State.EscapedEscape)
                {
                    _linePosition++;
                    _colPosition++;
                    if (_CollIndex < _propertyCount)
                    {
                        AppendChar(ii);
                    }
                    _state = State.Escaped;
                    continue;
                }
            }

            if (_buffer.Length > 0)
            {
                if (_CollIndex < _propertyCount && _IsAssigned[_CollIndex])
                {
                    SetValue(_result);
                }
                _buffer.Length = 0;
            }

            _Position = _resultPosition + 1;
            if (_linePosition == 0)
            {
                switch (EmptyLineBehaviour)
                {
                    case EmptyLineBehaviour.NullValue:
                        return default;

                    case EmptyLineBehaviour.SkipAndReadNext:
                        if (_byte != _TERMINATOR)
                        {
                            goto SkipEmptyLineAndReadNext;
                        }
                        return default;

                    case EmptyLineBehaviour.LogError:
                        _Errors.Add(new CsvReadError() { ColumnIndex = 0, LineNumber = _CurrentLine });
                        return default;

                    case EmptyLineBehaviour.ThrowException:
                        throw new CsvReadException($"Could not read empty line. Linenumber: {_CurrentLine}");
                }
            }
            return _result;
        }

        public override string[] ReadHeader()
        {
            MoveToStart();
            return ReadLine(_Data, (char)_Separator);
        }

        private string[] ReadLine(StringBuilder data, char seperator, params int[] selectIndexes)
        {
            StringBuilder _buffer = new StringBuilder(_BufferSize);

            int _byte = 0;
            int _nextByte = 0;
            State _state = State.Normal;
            List<string> _result = new List<string>();
            int _collIndex = 0;
            int _selectIndexesIndex = 0;

            int _resultPosition = _Position;

            for (int ii = _Position; ii < data.Length; ii++)
            {
                _resultPosition = ii;
                _byte = (int)data[ii];
                if (_state == State.Normal)
                {
                    if (_byte == seperator)
                    {
                        // End of field'
                        if (selectIndexes.Length == 0 || _selectIndexesIndex < selectIndexes.Length && selectIndexes[_selectIndexesIndex] == _collIndex)
                        {
                            _result.Add(_buffer.ToString());
                            _selectIndexesIndex++;
                        }
                        _buffer.Length = 0;
                        _collIndex++;
                        continue;
                    }
                    else if (_byte == _CR)
                    {
                        _nextByte = Peek(ref data, ii);
                        if (_nextByte == _LF)
                        {
                            continue; // goes to else if (_byte == '\n')
                        }
                        // end of line.
                        if (selectIndexes.Length == 0 || _selectIndexesIndex < selectIndexes.Length && selectIndexes[_selectIndexesIndex] == _collIndex)
                        {
                            _result.Add(_buffer.ToString());
                        }
                        _buffer.Length = 0;
                        break;
                    }
                    else if (_byte == _LF)
                    {
                        // end of line.
                        if (selectIndexes.Length == 0 || _selectIndexesIndex < selectIndexes.Length && selectIndexes[_selectIndexesIndex] == _collIndex)
                        {
                            _result.Add(_buffer.ToString());
                        }
                        _buffer.Length = 0;
                        break;
                    }
                    else if (_byte == _ESCAPE)
                    {
                        // switch mode
                        _state = State.Escaped;
                        continue; // do not add this char. (TRIM)
                    }
                    else if (_byte == _TERMINATOR)
                    {
                        break; // end the while loop.
                    }
                    _buffer.Append((char)_byte);
                    continue;
                }
                else if (_state == State.Escaped)
                {
                    // ',' and '\r' and "" are part of the value.
                    if (_byte == _TERMINATOR)
                    {
                        break; // end the while loop.
                    }
                    else if (_byte == _ESCAPE)
                    {
                        // " aaa "" ","bbb", "ccc""","ddd """" "
                        _nextByte = Peek(ref data, ii);
                        if (_nextByte == seperator || _nextByte == _CR || _nextByte == _LF)
                        {
                            // this quote is followed by a , so it ends the escape. we continue to next itteration where we read a ',' in nomral mode.
                            _state = State.Normal;
                            continue;
                        }
                        else if (_nextByte == _ESCAPE)
                        {
                            _state = State.EscapedEscape;
                            continue; // Also do not add this char, we add it when we are in EscapedEscape mode and from their we turn back to normal Escape.  (basically adding one of two)
                        }
                    }
                    _buffer.Append((char)_byte);
                    continue;
                }
                else if (_state == State.EscapedEscape)
                {
                    _buffer.Append((char)_byte);
                    _state = State.Escaped;
                    continue;
                }
                else if (_byte == _TERMINATOR)
                {
                    break;
                }
            }
            _resultPosition++;
            _Position = _resultPosition;
            if (_buffer.Length > 0)
            {
                if (selectIndexes.Length == 0 || _selectIndexesIndex < selectIndexes.Length && selectIndexes[_selectIndexesIndex] == _collIndex)
                {
                    _result.Add(_buffer.ToString());
                }
                _buffer.Length = 0;
            }
            return _result.ToArray();
        }
    }
}