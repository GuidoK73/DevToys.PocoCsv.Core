using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Enumerate Csv Stream Reader over T.
    /// Properties needs to be marked with ColumnAttribute
    /// </summary>
    public sealed class CsvReader<T> : BaseCsvReader<T>, IDisposable where T : class, new()
    {
        private StreamReader _Stream = null;
        private readonly string _File = null;
        private const int _CR = '\r';
        private const int _LF = '\n';
        private const int _ESCAPE = '"';
        private const int _TERMINATOR = -1;
        private State _state = State.Normal;
        private int _Separator = ',';
        private int _colPosition = -1; // char position within column
        private int _linePosition = 0;
        private int _byte = 0;
        private int _nextByte = 0;
        private int _IndexesIndex = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public CsvReader(StreamReader stream, char separator = ',', int buffersize = 1024)
        {
            _Stream = stream;
            _Separator = separator;
            Encoding = Encoding.Default;
            DetectEncodingFromByteOrderMarks = true;
            _buffer = new StringBuilder(buffersize);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">File or directory, in case of directory, filename will be generated based on T or the FileName property from the CsvAttribute will be used.</param>
        /// <param name="separator">The separator to use, default: ','</param>
        /// <param name="buffersize"></param>
        public CsvReader(string path, char separator = ',', int buffersize = 1024)
        {
            _File = path;
            if (Directory.Exists(_File))
            {
                var _attrib = typeof(T).GetCustomAttribute<CsvAttribute>();
                if (_attrib != null && !string.IsNullOrEmpty(_attrib.FileName))
                {
                    _File = Path.Combine(path.TrimEnd(new char[] { '\\' }), $"{_attrib.FileName}");
                }
                else
                {
                    _File = Path.Combine(path.TrimEnd(new char[] { '\\' }), $"{typeof(T).Name}.csv");
                }
            }

            _Separator = separator;
            Encoding = Encoding.Default;
            DetectEncodingFromByteOrderMarks = true;
            _buffer = new StringBuilder(buffersize);
        }

        /// <summary>
        /// Csv Seperator to use default ','
        /// </summary>
        public char Separator
        {
            get => (char)_Separator;
            set => _Separator = value;
        }

        /// <summary>
        /// How should empty lines be treated.
        /// </summary>
        public EmptyLineBehaviour EmptyLineBehaviour { get; set; } = EmptyLineBehaviour.DefaultInstance;

        /// <summary>
        /// Indicates there are read conversion errors.
        /// </summary>
        public bool HasErrors => _Errors.Count > 0;

        /// <summary>
        /// Returns collection of error messages.
        /// </summary>
        public IEnumerable<CsvReadError> Errors => _Errors;

        /// <summary>
        /// Indicates the stream has ended.
        /// </summary>
        public bool EndOfStream => _byte == _TERMINATOR;

        /// <summary>
        /// the character encoding to use. (Use when DetectEncodingFromByteOrderMarks does not yield proper results.)
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// indicates whether to look for byte order marks at the beginning of the file. Default: true
        /// </summary>
        public bool DetectEncodingFromByteOrderMarks { get; set; } = true;

        /// <summary>
        /// Releases all resources used by the System.IO.TextReader object.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Close();
        }

        /// <summary>
        /// Close the CSV stream reader
        /// </summary>
        public void Close()
        {
            if (_Stream == null)
            {
                return;
            }
            _Stream.BaseStream.Flush();
            _Stream.Close();
        }

        /// <summary>
        /// Flush all buffers.
        /// </summary>
        public void Flush() => _Stream.BaseStream.Flush();

        /// <summary>
        /// Detect the separator by sampling first 10 rows. Position is moved to start after execution.
        /// </summary>
        public void DetectSeparator()
        {
            AutoOpen();

            var _reader = new CsvStreamReader(_Stream.BaseStream);
            var _succes = CsvUtils.GetCsvSeparator(_reader, out char separator, 10);
            if (_succes)
            {
                Separator = separator;
            }
            MoveToStart();
        }

        /// <summary>
        /// Moves the reader to the start position.
        /// </summary>
        public void MoveToStart()
        {
            AutoOpen();

            _CurrentLine = 0;
            _byte = 0;
            _Stream.BaseStream.Position = 0;
        }

        /// <summary>
        /// Skip 1 or more rows.
        /// </summary>
        public void Skip(int rows)
        {
            AutoOpen();

            int ii = 0;

            while (!EndOfStream)
            {
                if (ii >= rows)
                {
                    break;
                }
                Skip();
                ii++;
            }
        }

        /// <summary>
        /// Skip a single row.
        /// </summary>
        public void Skip()
        {
            AutoOpen();

            _byte = 0;
            _CollIndex = 0;
            _state = State.Normal;
            _nextByte = 0;

            while (true)
            {
                _byte = _Stream.Read();
                if (_state == State.Normal)
                {
                    if (_byte == _Separator)
                    {
                        continue;
                    }
                    else if (_byte == _CR)
                    {
                        _nextByte = _Stream.Peek();
                        if (_nextByte == _LF)
                        {
                            continue;
                        }
                        _CurrentLine++;
                        break;
                    }
                    else if (_byte == _LF)
                    {
                        _CurrentLine++;
                        break;
                    }
                    else if (_byte == _ESCAPE)
                    {
                        _state = State.Escaped;
                        continue;
                    }
                    else if (_byte == _TERMINATOR)
                    {
                        break;
                    }
                    continue;
                }
                else if (_state == State.Escaped)
                {
                    if (_byte == _TERMINATOR)
                    {
                        break;
                    }
                    else if (_byte == _ESCAPE)
                    {
                        _nextByte = _Stream.Peek();
                        if (_nextByte == _Separator || _nextByte == _CR || _nextByte == _LF)
                        {
                            _state = State.Normal;
                            continue;
                        }
                        else if (_nextByte == _TERMINATOR)
                        {
                            break;
                        }
                        else if (_nextByte == _ESCAPE)
                        {
                            _state = State.EscapedEscape;
                            continue;
                        }
                    }
                    continue;
                }
                else if (_state == State.EscapedEscape)
                {
                    _state = State.Escaped;
                    continue;
                }
            }
        }

        /// <summary>
        /// MoveToStart then skip first row.
        /// </summary>
        public string[] SkipHeader()
        {
            return ReadHeader();
        }

        /// <summary>
        /// MoveToStart and reads the header as a string[] array.
        /// </summary>
        public override string[] ReadHeader()
        {
            MoveToStart();
            return ReadCsvLine();
        }

        /// <summary>
        /// Each iteration will read the next row from stream or file
        /// </summary>
        public IEnumerable<T> ReadAsEnumerable()
        {
            AutoOpen();

            while (!EndOfStream)
            {
                yield return Read();
            }
        }

        /// <summary>
        /// Reads the Csv line into object of type T and advances to the next.
        /// </summary>
        //  Called each line.
        public T Read()
        {
            AutoOpen();

            T _result = new T();

        SkipEmptyLineAndReadNext:

            _state = State.Normal;
            _buffer.Length = 0; // Clear the string buffer.
            _linePosition = 0;
            _byte = 0;
            _nextByte = 0;
            _CollIndex = 0;
            _colPosition = -1;

            while (true)
            {
                _byte = _Stream.Read();
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
                        _nextByte = _Stream.Peek();
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
                        AppendChar();
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
                        _nextByte = _Stream.Peek();
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
                        AppendChar();
                    }
                    continue;
                }
                else if (_state == State.EscapedEscape)
                {
                    _linePosition++;
                    _colPosition++;
                    if (_CollIndex < _propertyCount)
                    {
                        AppendChar();
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
                        throw new CsvReadException($"Could not read empty line. Linenumber: {_CurrentLine}, Reader Position: {_Stream.BaseStream.Position}.");
                }
            }
            return _result;
        }

        /// <summary>
        /// Reads the CSV line into string array, and advances to the next.
        /// </summary>
        /// <param name="selectIndexes">Optionally select the indexes you need,</param>
        public string[] ReadCsvLine(params int[] selectIndexes)
        {
            if (selectIndexes == null)
            {
                selectIndexes = new int[0];
            }

            AutoOpen();

            _CsvLineReaderResult.Clear();
            _state = State.Normal;
            _buffer.Length = 0; // Clear the string buffer.
            _byte = 0;
            _nextByte = 0;
            _CollIndex = 0;
            _IndexesIndex = 0;

            while (true)
            {
                _byte = _Stream.Read();
                if (_state == State.Normal)
                {
                    if (_byte == _Separator)
                    {
                        // End of field
                        if (selectIndexes.Length == 0 || _IndexesIndex < selectIndexes.Length && selectIndexes[_IndexesIndex] == _CollIndex)
                        {
                            _CsvLineReaderResult.Add(_buffer.ToString());
                            _IndexesIndex++;
                        }
                        _buffer.Length = 0;
                        _CollIndex++;
                        continue;
                    }
                    else if (_byte == _CR)
                    {
                        _nextByte = _Stream.Peek();
                        if (_nextByte == _LF)
                        {
                            continue; // goes to else if (_byte == '\n')
                        }
                        // end of line.
                        if (selectIndexes.Length == 0 || _IndexesIndex < selectIndexes.Length && selectIndexes[_IndexesIndex] == _CollIndex)
                        {
                            _CsvLineReaderResult.Add(_buffer.ToString());
                        }
                        _buffer.Length = 0;
                        break;
                    }
                    else if (_byte == _LF)
                    {
                        // end of line.
                        if (selectIndexes.Length == 0 || _IndexesIndex < selectIndexes.Length && selectIndexes[_IndexesIndex] == _CollIndex)
                        {
                            _CsvLineReaderResult.Add(_buffer.ToString());
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
                        // End of field
                        if (selectIndexes.Length == 0 || _IndexesIndex < selectIndexes.Length && selectIndexes[_IndexesIndex] == _CollIndex)
                        {
                            _CsvLineReaderResult.Add(_buffer.ToString());
                        }
                        _buffer.Length = 0;
                        return _CsvLineReaderResult.ToArray();
                    }
                    _buffer.Append((char)_byte);
                    continue;
                }
                else if (_state == State.Escaped)
                {
                    // ',' and '\r' and "" are part of the value.
                    if (_byte == _TERMINATOR)
                    {
                        // End of field
                        // Set the value
                        _buffer.Clear();
                        break; // end the while loop.
                    }
                    else if (_byte == _ESCAPE)
                    {
                        // " aaa "" ","bbb", "ccc""","ddd """" "
                        _nextByte = _Stream.Peek();
                        if (_nextByte == _Separator || _nextByte == _CR || _nextByte == _LF)
                        {
                            // this quote is followed by a , so it ends the escape. we continue to next itteration where we read a ',' in nomral mode.
                            _state = State.Normal;
                            continue;
                        }
                        if (_nextByte == _TERMINATOR)
                        {
                            // this quote is followed by a , so it ends the escape. we continue to next itteration where we read a ',' in nomral mode.
                            if (selectIndexes.Length == 0 || _IndexesIndex < selectIndexes.Length && selectIndexes[_IndexesIndex] == _CollIndex)
                            {
                                _CsvLineReaderResult.Add(_buffer.ToString());
                            }
                            break;
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
            }
            return _CsvLineReaderResult.ToArray();
        }

        // Called each character.
        private void AppendChar()
        {
            if (_ICustomCsvParseBase[_CollIndex] == null)
            {
                _buffer.Append((char)_byte);
                return;
            }
            _ICustomCsvParseBase[_CollIndex].Reading(_buffer, _CurrentLine, _CollIndex, _Stream.BaseStream.Position, _linePosition, _colPosition, (char)_byte);
        }

        /// <summary>
        /// Open the reader
        /// </summary>
        [Obsolete("No longer required to call Open() command.", false)]
        public void Open()
        {
            if (_Stream == null && string.IsNullOrEmpty(_File))
            {
                throw new IOException("No file or stream specified in constructor.");
            }
            if (!string.IsNullOrEmpty(_File))
            {
                if (!File.Exists(_File))
                {
                    throw new FileNotFoundException($"File '{_File}' not found.");
                }
                _Stream = new StreamReader(path: _File, encoding: Encoding, detectEncodingFromByteOrderMarks: true);
            }
            Init();
        }

        private void AutoOpen()
        {
            if (_IsInitializing)
            {
                return;
            }
            if (_Properties == null)
            {
                // Not initialized.
#pragma warning disable CS0618 // Type or member is obsolete
                Open();  // Initialize and Open the Stream.
#pragma warning restore CS0618 // Type or member is obsolete
                return;
            }
            if (_Stream == null)
            {
                // Initialized but the Close() method has been called.
                throw new IOException("Reader is closed!");
            }
        }
    }
}