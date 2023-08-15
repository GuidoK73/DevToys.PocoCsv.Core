# DevToys.PocoCsv.Core 

## One of the fastest csv reader deserialzer available.

DevToys.PocoCsv.Core is a class library to read and write to Csv.
It contains CsvStreamReader, CsvStreamWriter and Serialization classes CsvReader<T> and CsvWriter<T>.

Read/write serialize/deserialize data to and from Csv.

- RFC 4180 compliant.
- Auto separator detection.
- Auto line feed/break detection.
- Sequential read with ReadAsEnumerable().
- Csv schema Retrieval with CsvUtils.GetCsvSchema().
- DataTable import and export.
- Deserialiser / serializer.
- Stream reader / writer.


# CsvStreamReader
~~~cs
    string _file = "C:\Temp\data.csv";
    using (CsvStreamReader _reader = new CsvStreamReader(_file))
    {
        while (!_reader.EndOfStream)
        {
            string[] _values = _reader.ReadCsvLine();
        }
    }
~~~

or 

~~~cs
    string _file = "C:\Temp\data.csv";
    using (CsvStreamReader _reader = new CsvStreamReader(_file))
    {
        foreach (string[] items in _reader.ReadAsEnumerable())
        {
            
        }
    }
~~~


# CsvStreamWriter
~~~cs
    string file = @"D:\Temp\test.csv";
    using (CsvStreamWriter _writer = new CsvStreamWriter(file))
    {
        var _line = new string[] { "Row 1", "Row A,A", "Row 3", "Row B" };
        _writer.WriteCsvLine(_line);
    }
~~~

# CsvReader\<T\>
this reader is faster then CsvStreamReader, it is optamized to deserialize the rows to objects.

~~~cs
    public class Data
    {
        [Column(Index = 0)]
        public string Column1 { get; set; }

        [Column(Index = 1)]
        public string Column2 { get; set; }

        [Column(Index = 2)]
        public string Column3 { get; set; }

        [Column(Index = 5)]
        public string Column5 { get; set; }
    }
    
    string file = @"D:\Temp\data.csv";

    using (CsvReader<Data> _reader = new(file))
    {        
        _reader.Culture = CultureInfo.GetCultureInfo("en-us") ;
        _reader.Open();
        _reader.SkipHeader();
        var _data = _reader.ReadAsEnumerable().Where(p => p.Column1.Contains("16"));
        var _materialized = _data.ToList();
    }    
~~~


|Methods / Property|Description|
|:-|:-|
|**BufferSize**|Stream buffer size, Default: 1024.|
|**Close()**|Close the CSV stream reader|
|**Culture**|Sets the default Culture for decimal / double conversions etc. For more complex conversions use the ICustomCsvParse interface.|
|**CurrentLine**|Returns the current line number.|
|**DetectEncodingFromByteOrderMarks**|Indicates whether to look for byte order marks at the beginning of the file.|
|**DetectSeparator()**|To auto set the separator (looks for commonly used separators in first 10 lines).|
|**Dispose()**|Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.|
|**EmptyLineBehaviour**|EmptyLineBehaviour: <li>DefaultInstance: Return a new instance of T (Default)</li><li>NullValue: Return Null value for object.</li><li>SkipAndReadNext: if empty line has occurred, the reader will move to the next line.</li><li>LogError: Create an entry in Errors collecion</li><li>ThrowException: throw an exception when an empty line has occurred.</li> |
|**Encoding**|The character encoding to use.|
|**EndOfStream**|Returns true when end of stream is reached. Use this when you are using Read() / Skip() or partially ReadAsEnumerable() |
|**Errors**|Returns a list of errors when HasErrors returned true|
|**Flush()**|Flushes all buffers.|
|**HasErrors**|Indicates there are errors|
|**Last(int rows)**|Last seeks the csv document for the last x entries. this is much faster then IEnumerable.Last().|
|**MoveToStart()**|Moves the reader to the start position, Skip() and Take() alter the start positions use MoveToStart() to reset the position.|
|**Open()**|Opens the Reader.|
|**Read()**|Reads current row into T and advances the reader to the next row. |
|**ReadAsEnumerable()**|Reads and deserializes each csv file line per iteration in the collection, this allows for querying mega sized files. It starts from the current position, if you used Skip(), Read() or SkipHeader() the current position is determined by those methods.|
|**Separator**|Set the separator to use (default ',')|
|**Skip(int rows)**|Skip and advances the reader to the next row without interpreting it. This is much faster then IEnumerable.Skip(). |
|**SkipHeader()**|Ensures stream is at start then skips the first row.|

(Skip and Last do not deserialize, that's why they are faster then normal IEnumerable operations).

# CsvWriter\<T\>
this writer is faster then CsvStreamWriter, it is optamized to serialize the objects to rows.

~~~cs
    public class Data
    {
        [Column(Index = 0)]
        public string Column1 { get; set; }

        [Column(Index = 1)]
        public string Column2 { get; set; }

        [Column(Index = 2)]
        public string Column3 { get; set; }

        [Column(Index = 5)]
        public string Column5 { get; set; }
    }


    private IEnumerable<CsvSimple> LargeData()
    {
        for (int ii = 0; ii < 10000000; ii++)
        {
            Data _line = new()
            {
                Column1 = "bij",
                Column2 = "100",
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
        _writer.Open();
        _writer.Write(LargeData());
    }
      
~~~


Methods / Properties:

|Item|Description|
|:-|:-|
|**Open()**|Opens the Writer.|
|**WriteHeader()**|Write header with property names of T.|
|**Write(IEnumerable<T> rows)**|Writes data to Csv while consuming rows.|
|**Flush()**|Flushes all buffers.|
|**Separator**|Set the separator to use (default ',')|
|**CRLFMode**|Determine which mode to use for new lines.<li>CR + LF → Used as a new line character in Windows.</li><li>CR(Carriage Return) → Used as a new line character in Mac OS before X.</li><li>LF(Line Feed) → Used as a new line character in Unix/Mac OS X</li>|
|**NullValueBehaviour**|Determine what to do with writing null objects.<li>Skip, Ignore the object</li><li>Empty Line, Write an empty line</li>|
|**Culture**|Sets the default Culture for decimal / double conversions etc. For more complex conversions use the ICustomCsvParse interface.|
|**Encoding**|The character encoding to use.|


# ColumnAttribute

The column attribute defines the properties to be serialized or deserialized.


|Item|Description|
|:-|:-|
|**Index**|Defines the index position within the CSV document. Numbers can be skipped for the reader to ignore certain columns, for the writer numbers can also be skipped which leads to empty columns.|
|**Header**|Defines the header text, this property only applies to the CsvWriter, if not specified, the property name is used.|
|**OutputFormat**|Apply a string format, depending on the Property type. This property is for CsvWriter only.|
|**OutputNullValue**|Defines the value to write as a default for null, This property is for CsvWriter only.|
|**CustomParserType**|CustomParserType allows for custom parsing of values to a specific type.|


# CustomParserType
CustomParserType allows the Reader<T> and Writer<T> to use a custom parsing for a specific field.


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

    public class ParsePrice : ICustomCsvParse<Decimal>
    {
        private CultureInfo _culture;

        public ParseDecimal()
        {
            _culture = CultureInfo.GetCultureInfo("en-us");
        }

        public Decimal Read(StringBuilder value) => Decimal.Parse(value.ToString(), _culture);

        public string Write(Decimal value) => value.ToString(_culture);
    }


    public sealed class CsvPreParseTestObject
    {
        [Column(Index = 0, CustomParserType = typeof(ParseBoolean) )]
        public Boolean? IsOk { get; set; }

        [Column(Index = 1)]
        public string Name { get; set; }

        [Column(Index = 3, CustomParserType = typeof(ParsePrice))]
        public Decimal Price { get; set; }
    }


    using (var _reader = new CsvReader<CsvPreParseTestObject>(_file))
    {
        _reader.Open();
        _reader.Skip(); // Slip header.
        var _rows = _reader.ReadAsEnumerable().ToArray(); // Materialize.
    }

~~~

Custom Parsers will run as singleton per specified column in the specific Reader<T>.

# CsvAttribute

the CsvAttribute can be set at defaults for CustomParserType, these CustomParserTypes will be applied to all properties of that specific type.\
until they are overruled at property level.


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

# Other Examples

~~~cs

    public class Data
    {
        [Column(Index = 0)]
        public string Collumn1 { get; set; }
        
        [Column(Index = 1)]
        public string Collumn2 { get; set; }
        
        [Column(Index = 2, Header = "Test" )]
        public byte[] Collumn3 { get; set; }

        [Column(Index = 3)]
        public DateTime TestDateTime { get; set; }
        
        [Column(Index = 4)]
        public DateTime? TestDateTimeNull { get; set; }

        [Column(Index = 5)]
        public Int32 TestInt { get; set; }
        
        [Column(Index = 6, OutputNullValue = "[NULL]")]
        public Int32? TestIntNull { get; set; }
    }

~~~

~~~cs
 
    private IEnumerable<Data> GetTestData()
    {
        yield return new Data
        {
            Collumn1 = "01", 
            Collumn2 = "AA",
            Collumn3 = new byte[3] { 2, 4, 6 },
            TestDateTime = DateTime.Now,
            TestDateTimeNull = DateTime.Now,
            TestInt = 100,
            TestIntNull = 200
        };
        yield return new Data
        {
            Collumn1 = "01",
            Collumn2 = "AA",
            Collumn3 = new byte[3] { 2, 4, 6 },
            TestDateTime = DateTime.Now,
            TestDateTimeNull = DateTime.Now,
            TestInt = 100,
            TestIntNull = 200
        };
        yield return new Data
        {
            Collumn1 = "04",
            Collumn2 = "BB",
            Collumn3 = new byte[3] { 8, 9, 10 },
            TestDateTime = DateTime.Now,
            TestDateTimeNull = null,
            TestInt = 300,
            TestIntNull = null
        };
    }

    public static string StreamToString(Stream stream)
    {
        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
        {
            stream.Position = 0;
            return reader.ReadToEnd();
        }
    }

~~~

~~~cs

    List<Data> _result = new List<Data>();

    using (MemoryStream _stream = new MemoryStream())
    {
        using (CsvWriter<Data> _csvWriter = new CsvWriter<Data>(_stream))
        using (CsvReader<Data> _csvReader = new CsvReader<Data>(_stream))
        {
            _csvWriter.Separator = ';';
            _csvWriter.Open();
            _csvWriter.WriteHeader();
            _csvWriter.Write(GetTestData());

            _csvReader.Open();
            _csvReader.DetectSeparator(); // Auto detect separator.
            _csvReader.Skip(); // Skip header. 
            _result = _csvReader.ReadAsEnumerable().Where(p => p.Collumn2 == "AA").ToList();
        }
    }

~~~

~~~cs

    string _result;

    using (MemoryStream _stream = new MemoryStream())
    {
        using (CsvWriter<Data> _csvWriter = new CsvWriter<Data>(_stream))
        {
            _csvWriter.Separator = ',';
            _csvWriter.Open();
            _csvWriter.WriteHeader();
            _csvWriter.Write(GetTestData());

            _result = StreamToString(_stream);
        }
    }    

~~~

# Sampling only a few rows without reading entire csv.

~~~cs

    List<CsvSimple> _result1;
    List<CsvSimple> _result2;

    string file = @"D:\largedata.csv";
    _w.Start();

    using (CsvReader<CsvSimple> _reader = new CsvReader<CsvSimple>(file))
    {
        _reader.Open();

        _reader.Skip(); // skip the Header row.

        // Materializes 20 records but returns 10.
        _result1 = _reader.ReadAsEnumerable().Skip(10).Take(10).ToList(); 
        
        // Materialize only 10 records.
        _reader.Skip(10);
        _result1 = _reader.ReadAsEnumerable().Take(10).ToList();

        // Take last 10 records. Without serializing everything before it.
        _result1 = _reader.Last(10).ToList();
    }

~~~

Mind you on the fact that Skip and Take andvances the reader to the next position.\
executing another _reader.ReadAsEnumerable().Where(p => p...).ToList() will Query from position 21. 

Use MoveToStart() to move the reader to the starting position.

_reader.Skip() is different then _reader.ReadAsEnumerable().Skip() as the first does not materialize to T and is faster.


# DataTable Import / Export

~~~cs

    // Import
    var _file = @"C:\data.csv";
    var _table = new DataTable();
    _table.ImportCsv(_file, ',', true);

    // Export
    _file = @"C:\data2.csv";
    _table.ExportCsv(_file, ',');

~~~