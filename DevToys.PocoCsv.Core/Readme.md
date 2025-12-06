# DevToys.PocoCsv.Core 

DevToys.PocoCsv.Core is a class library to read and write to Csv very fast.
It contains CsvStreamReader, CsvStreamWriter and Serialization classes CsvReader<T> and CsvWriter<T>.\
It provides plenty of options on how you would either read from or write to CSV files.

- Extremely fast.
- Handle unlimited file sizes.
- RFC 4180 compliant.
- Sequential read with ReadAsEnumerable().
- Csv schema Retrieval with CsvUtils.GetCsvSchema().
- DataTable import and export.
- Works for all encoding types.
- CsvWriter<T> / CsvReader<T> for serialization of object from and to streams or files.
- StreamReader / StreamWriter for writing CSV from and to streams or files.
- CsvSerializer to serialize/deserialize objects to and from strings. (NEW in v5)


# Topics 
<a href="#CsvStreamReader">CsvStreamReader</a>\
<a href="#CsvStreamWriter">CsvStreamWriter</a>\
<a href="#CsvReaderT">CsvReader&lt;T&gt;</a>\
<a href="#CsvWriterT">CsvWriter&lt;T&gt;</a>\
<a href="#CsvSerializer">CsvSerializer</a>\
<a href="#ColumnAttribute">ColumnAttribute</a>\
<a href="#CustomParserType">CustomParserType</a>\
<a href="#CsvAttribute">CsvAttribute</a>\
<a href="#DataTable">DataTable</a>\
<a href="#RowSampling">RowSampling</a>\
<a href="#PlainObjects">Plain Object Serialization</a>\
<a href="#CsvDataTypeObject">CsvDataTypeObject</a>\
<a href="#DataTable">DataTable Import / Export</a>\
<a href="#ExtensionMethods">ExtensionMethods</a>




<H1 id="CsvStreamReader">CsvStreamReader</H1>

~~~cs
    string _file = @"C:\Temp\data.csv";
    using (CsvStreamReader _reader = new CsvStreamReader(_file))
    {
        while (!_reader.EndOfStream)
        {
            string[] _resultArray = _reader.ReadCsvLine();
        }
    }
~~~

or 

~~~cs
    string _file = @"C:\Temp\data.csv";
    using (CsvStreamReader _reader = new CsvStreamReader(_file))
    {
        _reader.SetColumnIndexes(2,5); // only include column 2 and 5 in the result array. This is optional.

        foreach (string[] items in _reader.ReadAsEnumerable())
        {
            
        }
    }
~~~

or use the string[] decontruct extension methods (max 10 parameters)

~~~cs
    using (CsvStreamReader _reader = new CsvStreamReader(_file))
    {
        foreach(var (first, second, third) in _reader.ReadAsEnumerable())
        {

        }
    }
~~~

or use the Dictionary functions\
Note: this option may have some performance degradation.

~~~cs
    using (CsvStreamReader _reader = new CsvStreamReader(_file))
    {
        while (!_reader.EndOfStream)
        {            
            Dictionary<string,string> _values = _reader.ReadCsvLineAsDictionary(); 
            string _id = _values["Id"];
            string _name = _values["Name"];
        }
    }
~~~


| Methods / Property               | Description                                                                                 |
| :-                               | :-                                                                                          |
| **CurrentLine**                  | Returns the current line number                                                             |
| **DetectSeparator()**            | Detect the separator by sampling first 10 rows. Position is moved to start after execution. |
| **EndOfStream**                  | Indicates the stream has ended.                                                             |
| **GetCsvSchema()**               | Returns a schema for the CSV with best fitted types to use.                                 |
| **GetCsvSeparator()**            | Detects and sets CSV Separator.                                                             |
| **MoveToStart()**                | Move reader to the start position 0                                                         |
| **Position**                     | Get / Sets the position.                                                                    |
| **ReadAsEnumerable()**           | Each iteration will read the next row from stream or file                                   |
| **ReadCsvLine()**                | Reads the CSV line into string array, and advances to the next.                             |
| **ReadCsvLineAsDictionary()**    | Assumes first line is the Header with column names.                                         |
| **ReadAsEnumerableDictionary()** | Assumes first line is the Header with column names.                                         |
| **ReadLine()**                   | Perform ReadCsvLine.                                                                        |
| **ResetColumnIndexes()**         | Reset the column indexes to default, including all columns in the result array.             |
| **Separator**                    | Get / Sets the Separator character to use.                                                  |
| **SetColumnIndexes()**           | Limit the result array for ReadCsvLine to only these columns.                               |
| **Skip()**                       | Use to skip first row without materializing, usefull for skipping header.                   |


<H1 id="CsvStreamWriter">CsvStreamWriter</H1>

~~~cs
    string file = @"D:\Temp\test.csv";
    using (CsvStreamWriter _writer = new CsvStreamWriter(file))
    {
        var _line = new string[] { "Row 1", "Row A,A", "Row 3", "Row B" };
        _writer.WriteCsvLine(_line);
    }
~~~


| Item                 | Description                                                                                                                                                                                                                                                |
| :-                   | :-                                                                                                                                                                                                                                                         |
| **Separator**        | Csv Seperator to use default ','                                                                                                                                                                                                                           |
| **CRLFMode**         | Determine which mode to use for new lines.<li>CR + LF ? Used as a new line character in Windows.</li><li>CR(Carriage Return) ? Used as a new line character in Mac OS before X.</li><li>LF(Line Feed) ? Used as a new line character in Unix/Mac OS X</li> |
| **WriteCsvLine()**   | Write an array of strings to the Csv Stream and escapes when nececary.                                                                                                                                                                                     |
| **SetColumnIndexes() | Limit the output columns from the source array                                                                                                                                                                                                             |


<H1 id="CsvReaderT">CsvReader&lt;T&gt;</H1>

# 

The CsvReader is a full type CSV deserializer.\
All simple types are allowed to be used as property type, including byte[]. All other complex types are ignored.

~~~cs
    public class Data
    {
        [Column(Index = 0)]
        public string Column1 { get; set; }

        [Column(Index = 1)]
        public decimal Column2 { get; set; }

        [Column(Index = 2)]
        public string Column3 { get; set; }

        [Column(Index = 5)]
        public string Column5 { get; set; }
    }
    
    string file = @"D:\Temp\data.csv";

    using (CsvReader<Data> _reader = new(file))
    {        
        _reader.Culture = CultureInfo.GetCultureInfo("en-us") ;
        _reader.SkipHeader();
        var _data = _reader.ReadAsEnumerable().Where(p => p.Column1.Contains("16"));
        var _materialized = _data.ToList();
    }    
~~~

The reader is for performance reasons unrelated to the CsvStreamReader.

The reader does not care about the number of columns in a row, as long as the highest index on the Column Attribute does not exceed the number of columns in a row.\
You only specify the column indexes you need.


| Methods / Property                   | Description                                                                                                                                                                                                                                                                                                                                                       |
| :-                                   | :-                                                                                                                                                                                                                                                                                                                                                                |
| **BufferSize**                       | Stream buffer size, Default: 1024.                                                                                                                                                                                                                                                                                                                                |
| **Close()**                          | Close the CSV stream reader                                                                                                                                                                                                                                                                                                                                       |
| **CurrentLine**                      | Returns the current line number.                                                                                                                                                                                                                                                                                                                                  |
| **DetectSeparator()**                | To auto set the separator (looks for commonly used separators in first 10 lines).                                                                                                                                                                                                                                                                                 |
| **DetectEncodingFromByteOrderMarks** | indicates whether to look for byte order marks at the beginning of the file.                                                                                                                                                                                                                                                                                      |
| **Dispose()**                        | Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.                                                                                                                                                                                                                                                          |
| **EmptyLineBehaviour**               | EmptyLineBehaviour: <li>DefaultInstance: Return a new instance of T (Default)</li><li>NullValue: Return Null value for object.</li><li>SkipAndReadNext: if empty line has occurred, the reader will move to the next line.</li><li>LogError: Create an entry in Errors collecion</li><li>ThrowException: throw an exception when an empty line has occurred.</li> |
| **Encoding**                         | The character encoding to use.                                                                                                                                                                                                                                                                                                                                    |
| **EndOfStream**                      | Returns true when end of stream is reached. Use this when you are using Read() / Skip() or partially ReadAsEnumerable()                                                                                                                                                                                                                                           |
| **Errors**                           | Returns a list of errors when HasErrors returned true                                                                                                                                                                                                                                                                                                             |
| **Flush()**                          | Flushes all buffers.                                                                                                                                                                                                                                                                                                                                              |
| **HasErrors**                        | Indicates there are errors                                                                                                                                                                                                                                                                                                                                        |
| **IgnoreColumnAttributes**           | All properties are handled in order of property occurrence and mapped directly to their respective index. Only use when CsvWriter has this set to true as well. (ColumnAttribute is ignored.)                                                                                                                                                                     |
| **MoveToStart()**                    | Moves the reader to the start position, Skip() and Take() alter the start positions use MoveToStart() to reset the position.                                                                                                                                                                                                                                      |
| **Open()**                           | Opens the Reader. (this method is optional, the reader will auto open when nececary)                                                                                                                                                                                                                                                                              |
| **Read()**                           | Reads current row into T and advances the reader to the next row.                                                                                                                                                                                                                                                                                                 |
| **ReadCsvLine()**                    | Reads current row into a string[] just like the CsvStreamReader. and advances the reader to the next row.                                                                                                                                                                                                                                                         |
| **ReadHeader()**                     | Moves to start and performs a ReadCsvLine()                                                                                                                                                                                                                                                                                                                       |
| **ReadAsEnumerable()**               | Reads and deserializes each csv file line per iteration in the collection, this allows for querying large size files. It starts from the current position, if you used Skip(), Read() or SkipHeader() the current position is determined by those methods.                                                                                                        |
| **Separator**                        | Set the separator to use (default ',')                                                                                                                                                                                                                                                                                                                            |
| **Skip(int rows)**                   | Skip and advances the reader to the next row without interpreting it. This is much faster then IEnumerable.Skip().                                                                                                                                                                                                                                                |
| **SkipHeader()**                     | Ensures stream is at start then skips the first row.                                                                                                                                                                                                                                                                                                              |


The path given to the constructor can be a specific file or directory, in case a directory is given, the filename will be expected as [TYPENAME].csv, or based on the FileName given by the CsvAttribute.

(Skip does not deserialize, that's why it's faster then normal IEnumerable operations).


<H1 id="CsvWriterT">CsvWriter&lt;T&gt;</H1>

The CsvReader is a full type CSV serializer.\
All simple types are allowed to be used as property type, including byte[]. All other complex types are ignored.

~~~cs
    public class Data
    {
        [Column(Index = 0)]
        public string Column1 { get; set; }

        [Column(Index = 1)]
        public decimal Column2 { get; set; }

        [Column(Index = 2)]
        public string Column3 { get; set; }

        [Column(Index = 5)]
        public string Column5 { get; set; }
    }


    private IEnumerable<CsvSimple> LargeData()
    {
        for (int ii = 0; ii < 10000; ii++)
        {
            Data _line = new()
            {
                Column1 = "bij",
                Column2 = 109.59M,
                Column3 = "test",
                Column5 = $"{ii}",
                
            };
            yield return _line;
        }
    }
    
    
    string file = @"D:\largedata.csv";
    using (CsvWriter<CsvSimple> _writer = new(file) { Separator = ',', Append = true })
    {
        _writer.Culture = CultureInfo.GetCultureInfo("en-us");
        _writer.Write(LargeData());
    }
      
~~~


Methods / Properties:

| Item                           | Description                                                                                                                                                                                                                                                |
| :-                             | :-                                                                                                                                                                                                                                                         |
| **FileMode**                   | Determine whether to create a new file or append to existing files.                                                                                                                                                                                        |
| **Open()**                     | Opens the Writer. (this method is optional, the writer will auto open when nececary)                                                                                                                                                                       |
| **WriteHeader()**              | Write header with property names of T.                                                                                                                                                                                                                     |
| **Write(IEnumerable<T> rows)** | Writes data to Csv while consuming rows.                                                                                                                                                                                                                   |
| **Flush()**                    | Flushes all buffers.                                                                                                                                                                                                                                       |
| **IgnoreColumnAttributes**     | All properties are handled in order of property occurrence and mapped directly to their respective index. (ColumnAttribute is ignored.)                                                                                                                    |
| **Separator**                  | Set the separator to use (default ',')                                                                                                                                                                                                                     |
| **CRLFMode**                   | Determine which mode to use for new lines.<li>CR + LF ? Used as a new line character in Windows.</li><li>CR(Carriage Return) ? Used as a new line character in Mac OS before X.</li><li>LF(Line Feed) ? Used as a new line character in Unix/Mac OS X</li> |
| **NullValueBehaviour**         | Determine what to do with writing null objects.<li>Skip, Ignore the object</li><li>Empty Line, Write an empty line</li>                                                                                                                                    |
| **Culture**                    | Sets the default Culture for decimal / double conversions etc. For more complex conversions use the ICustomCsvParse interface.                                                                                                                             |
| **Encoding**                   | The character encoding to use.                                                                                                                                                                                                                             |


The path given to the constructor can be a specific file or directory, in case a directory is given, the filename will be generated based on the T typename, or based on the FileName given by the CsvAttribute.

The writer is for performance reasons unrelated to the CsvStreamWriter.


<H1 id="CsvSerializer">CsvSerializer</H1>

Class to serialize and deserialize to and from strings or StringBuilders.


Example 1:

~~~cs

    public class TestSerializerObject
    {
        [Column(Index = 0)]
        public int Id { get; set; }
        [Column(Index = 1)]
        public string Field1 { get; set; }
        [Column(Index = 2)]
        public string Field2 { get; set; }
    }

    private IEnumerable<TestSerializerObject> SimpleData(int count = 10)
    {
        for (int ii = 0; ii < count; ii++)
        {
            yield return new TestSerializerObject() { Id = ii, Field1 = $"A{ii}", Field2 = $"b{ii}" };
        }
    }

    public void Test()
    {
        CsvSerializer serializer = new CsvSerializer(new CsvSerializerSettings() { Header = true } );

        string _data = serializer.Serialize(SimpleData());

        var _resultData = serializer.Deserialize<TestSerializerObject>(_data).ToList();
    }

~~~

Example 2:

~~~cs

    CsvSerializer _serializer = new CsvSerializer();

    StringBuilder _sb = new StringBuilder();
    _sb.AppendCsvLine(',', "a", "b", "c");
    _sb.AppendCsvLine(',', "1", "2", "3");
    _sb.AppendCsvLine(',', "d", "e", "f");

    List<string[]> _result = _serializer.Deserialize(_sb).ToList();

    StringBuilder _sbResult = new StringBuilder();
    _serializer.Serialize(_result, ref _sbResult);

    string _stringResult = _sbResult.ToString();

~~~

Example 3:\
using the CsvSerializer to read and write to files.

Important note: ALWAYS consume the full file or collection to ensure proper file disposal.

FileInfo or DirectoryInfo can be used, in case a directory is given, the filename will be expected as [TYPENAME].csv, or based on the FileName given by the CsvAttribute.


~~~cs

        var _file = new FileInfo(Path.GetTempFileName());
        CsvSerializer _serializer = new CsvSerializer();

        _serializer.SerializeObject<TestSerializerObject>(_file, SimpleData());
        List<TestSerializerObject> _result = _serializer.DeserializeObject<TestSerializerObject>(_file).ToList(); // ALWAYS consume to the en of file.

        List<string[]> _resultString = _serializer.Deserialize(_file).ToList();


~~~

both examples work with string or StringBuilder.

#### CsvSerializerSettings

| Item            | Description                          |
| :-              | :-                                   |
| Header          | Write header or skip header          |
| Separator       | Separator to use.                    |
| DetectSeparator | Automatic separator detection.       |
| Culture         | Culture to use.                      |
| CRLFMode        | CRLF mode to use for the Serializer. |
| BufferSize      | Buffersize to use                    |


<H1 id="ColumnAttribute">ColumnAttribute</H1>


The column attribute defines the properties to be serialized or deserialized.


| Item                 | Description                                                                                                                                                                                   |
| :-                   | :-                                                                                                                                                                                            |
| **Index**            | Defines the index position within the CSV document. Numbers can be skipped for the reader to ignore certain columns, for the writer numbers can also be skipped which leads to empty columns. |
| **Header**           | Defines the header text, this property only applies to the CsvWriter, if not specified, the property name is used.                                                                            |
| **OutputFormat**     | Apply a string format, depending on the Property type. This property is for CsvWriter only.                                                                                                   |
| **OutputNullValue**  | Defines the value to write as a default for null, This property is for CsvWriter only.                                                                                                        |
| **CustomParserType** | CustomParserType allows for custom parsing of values to a specific type.                                                                                                                      |


<H1 id="CustomParserType">CustomParserType</H1>

CustomParserType allows the Reader<T> and Writer<T> and the CsvSerializer to use a custom parsing for a specific field.

In general you can use the Column attribute on any simple type and the value will be converted if possible.
When using the reader you might have csv's from third party sources where columns might require some extra conversion, 
this is where Custom parsers come in handy.

Custom Parsers will run as singleton per specified column in the specific Reader<T> or Writer<T>.

All values and characters at this point are unescaped / escaped as required by the CSV standards.

| Interface Method | Description                                                                                                                                                                                                                                                                                                                                                                   |
| :-               | :-                                                                                                                                                                                                                                                                                                                                                                            |
| Read             | This function is called when using CsvReader</br> Return value must be the same as the property type the CustomParser is placed on.                                                                                                                                                                                                                                           |
| Reading          | This method is called when using CsvReader. It can be used as a support function to the Read function when reading per char might be a performance requirement.</br> don't implement this interface when you don't need it.</br>c is the character to use in the result text and should be appended to the value StringBuilder, escaping has already been done at this point. |
| Write            | This function is called when using CsvWriter</br> T value must be the same as the property type the CustomParser is placed on.                                                                                                                                                                                                                                                |

~~~cs

    public sealed class ParseBoolean : ICustomCsvParse<bool?>
    {
        // for CsvReader
        public bool? Read(StringBuilder value)
        {
            switch (value.ToString().ToLower())
            {
                case "on":
                case "true":
                case "yes":
                case "1":
                    return true;
                case "off":
                case "false":
                case "no":
                case "0":
                    return false;
            }
            return null;
        }

        // this is the default implementation for Reading method in order to work. you can normally leave this out.
        public void Reading(StringBuilder value, int line, int colIndex, long readerPos, int linePos, int colPos, char c)  
        {
            value.Append(c);
        }

        // for CsvWriter
        public string Write(bool? value)
        {
            if (value.HasValue)
            {
                if (value == true)
                {
                    return "1";
                }
                return "0";
            }
            return string.Empty;
        }
    }

~~~

~~~cs

    public class ParseDecimal : ICustomCsvParse<Decimal>
    {
        private CultureInfo _culture;

        public ParseDecimal()
        {
            _culture = CultureInfo.GetCultureInfo("en-us");
        }

        public Decimal Read(StringBuilder value) => Decimal.Parse(value.ToString(), _culture);

        public string Write(Decimal value) => value.ToString(_culture);
    }

~~~

~~~cs

    public sealed class CsvPreParseTestObject
    {
        [Column(Index = 0, CustomParserType = typeof(ParseBoolean) )]
        public Boolean? IsOk { get; set; }

        [Column(Index = 1)]
        public string Name { get; set; }

        [Column(Index = 3, CustomParserType = typeof(ParseDecimal))]
        public Decimal Price { get; set; }
    }

~~~

~~~cs

    using (var _reader = new CsvReader<CsvPreParseTestObject>(_file))
    {
        _reader.Skip(); // Slip header.
        var _rows = _reader.ReadAsEnumerable().ToArray(); // Materialize.
    }

~~~

<H1 id="CsvAttribute">CsvAttribute</H1>

the CsvAttribute can be set at defaults for CustomParserType, these CustomParserTypes will be applied to all properties of that specific type.\
until they are overruled at property level.


| Item                                              | Description                                                                                                                                           |
| :-                                                | :-                                                                                                                                                    | 
| **FileName**                                      | When a directory is specified on the constructor for CsvReader or CsvWriter instead of a file, this FileName will be used within specified directory. | 
| **DefaultCustomParserTypeString**                 | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeGuid**                   | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeBoolean**                | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeDateTime**               | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeDateTimeOffset**         | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeTimeSpan**               | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeByte**                   | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeSByte**                  | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeInt16**                  | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeInt32**                  | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeInt64**                  | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeSingle**                 | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeDecimal**                | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeDouble**                 | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeUInt16**                 | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeUInt32**                 | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeUInt64**                 | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeBigInteger**             | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeGuidNullable**           | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeBooleanNullable**        | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeDateTimeNullable**       | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeDateTimeOffsetNullable** | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeTimeSpanNullable**       | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeByteNullable**           | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeSByteNullable**          | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeInt16Nullable**          | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeInt32Nullable**          | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeInt64Nullable**          | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeSingleNullable**         | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeDecimalNullable**        | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeDoubleNullable**         | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeUInt16Nullable**         | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeUInt32Nullable**         | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeUInt64Nullable**         | Default Custom Parse Type                                                                                                                             | 
| **DefaultCustomParserTypeBigIntegerNullable**     | Default Custom Parse Type                                                                                                                             | 



~~~cs

    public class Parsestring : ICustomCsvParse<string>
    {
        public string Read(StringBuilder value)
        {
            return value.ToString();
        }
        public string Write(string value)
        {
            return value;
        }
    }

    [Csv( DefaultCustomParserTypeString = typeof(Parsestring))]
    public class CsvAllTypes
    {
        [Column(Index = 0, OutputFormat = "", OutputNullValue = "")]
        public string _stringValue { get; set; }

        [Column(Index = 35, OutputFormat = "", OutputNullValue = "")]
        public string _stringValue2 { get; set; }

        [Column(Index = 1, CustomParserType = typeof(ParseGuid), OutputFormat = "", OutputNullValue = "")]
        public Guid _GuidValue { get; set; }
   }

~~~

<H1 id="RowSampling">RowSampling: Sampling only a few rows without reading entire csv.</H1>

~~~cs

    List<CsvSimple> _result1;
    List<CsvSimple> _result2;

    string file = @"D:\largedata.csv";
    _w.Start();

    using (CsvReader<CsvSimple> _reader = new CsvReader<CsvSimple>(file))
    {
        _reader.Skip(); // skip the Header row.

        // Materializes 20 records but returns 10.
        _result1 = _reader.ReadAsEnumerable().Skip(10).Take(10).ToList(); 
        
        // Materialize only 10 records.
        _reader.Skip(10);
        _result1 = _reader.ReadAsEnumerable().Take(10).ToList();

    }

~~~

Mind you on the fact that Skip and Take andvances the reader to the next position.\
executing another _reader.ReadAsEnumerable().Where(p => p...).ToList() will Query from position 21. 

Use MoveToStart() to move the reader to the starting position.

_reader.Skip() is different then _reader.ReadAsEnumerable().Skip() as the first does not materialize to T and is faster.

<H1 id="PlainObjects">Serialize / Deserialize plain C# objects without specific ColumnAttributes</H1>

Mapping will be determined by the Header in the Csv, columns will only be mapped to corresponding property names.

~~~cs
    public class SimpleObject
    {
        public int Id { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
    }
~~~

~~~cs
    private IEnumerable<SimpleObject> Data(int count = 50)
    {
        for (int ii = 0; ii < count; ii++)
        {
            yield return  new SimpleObject() { Id = ii, Field1 = $"A{ii}", Field2 = $"b{ii}" };                
        }
    }
~~~

~~~cs
    string _file = System.IO.Path.GetTempFileName();

    using (CsvWriter<SimpleObject> _writer = new(_file) { Separator = ',' })
    {
        _writer.WriteHeader();
        _writer.Write(Data());
    }

    using (CsvReader<SimpleObject> _reader = new(_file))
    {
        List<SimpleObject> _materialized = _reader.ReadAsEnumerable().ToList();
    }
~~~


<H1 id="CsvDataTypeObject">CsvDataTypeObject</H1>

Convenience class to read up to 50 CsvColumns from a Csv document.

- All fields are string only.
- this object can be usefull if you want to use the CsvReader<T> on unknown csv files.
- Object can be used on any Csv regardless their number of columns. (Column indexes above 50 will be ignored)
- Object has the Deconstruct implemented so you can use shorthands for fields.
- Object is comparable by value.
- Implicit convert from Csv line string to CsvDataTypeObject and back to string.

~~~cs
    using (CsvReader<CsvDataTypeObject> _reader = new(_file))
    {
        foreach (var item in _reader.ReadAsEnumerable())
        {
            string id = item.Field01;
            string name = item.Field02;
        }
    }
~~~

you can use the Deconstruct shorthand:

~~~cs   
    using (CsvReader<CsvDataTypeObject> _reader = new(_file))
    {
        foreach (var (id, name) in _reader.ReadAsEnumerable())
        {            
        }
    }
~~~

if you would like to use it with the writer you can limit the number of output columns with the ColumnLimit property.

~~~cs
    string _file = System.IO.Path.GetTempFileName();

    using (CsvWriter<CsvDataTypeObject> _writer = new(_file) { Separator = ',', ColumnLimit = 5 })
    {
        _writer.WriteHeader();
        _writer.Write(SimpleData(50));
    }

    private IEnumerable<CsvDataTypeObject> SimpleData(int count)
    {
        for (int ii = 0; ii < count; ii++)
        {
            yield return new CsvDataTypeObject() { Field01 = $"A{ii}", Field02 = $"b{ii}", Field03 = $"c{ii}", Field04 = $"d{ii}", Field05 = $"e{ii}" };
        }
    }
~~~

<H1 id="DataTable">DataTable Import / Export</H1>

2 Extension methods on the DataTable object.

~~~cs

    using DevToys.PocoCsv.Core.Extensions;

    // Import
    var _file = @"C:\data.csv";
    var _table = new DataTable();
    _table.ImportCsv(_file, ',', true);

    // Export
    _file = @"C:\data2.csv";
    _table.ExportCsv(_file, ',');

~~~


<H1 id="ExtensionMethods">Extension Methods</H1>

#### StringBuilder
~~~cs
    AppendCsv(char separator, params string[] values)
    AppendCsvLine(char separator, params string[] values)
~~~

#### string[]
~~~cs
    Deconstruct(first ... tenth)
    JoinCsvLine(char separator = ',')
~~~

#### string
~~~cs
    SplitCsvLine(char separator = ',')  
~~~

#### DataTable
~~~cs
     ImportCsv(string file, char? separator = ',', bool setSchema = true, int skipRows = 1)
     ExportCsv(string file, char separator = ',')
~~~


