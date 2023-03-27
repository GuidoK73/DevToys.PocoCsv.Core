# DevToys.PocoCsv.Core 

## One of the fastest csv reader deserialzer available.

DevToys.PocoCsv.Core is a class library to read and write to Csv.
It contains CsvStreamReader, CsvStreamWriter and Serialization classes CsvReader<T> and CsvWriter<T>.

- Read/write serialize/deserialize data to and from Csv.
- Use Linq to query large CSV files with CsvReader<T>.ReadAsEnumerable().
- Use CsvWriter<T>.Write() to write large data tables to Csv.
- Retrieve schema for a csv file with CsvUtils.GetCsvSchema() which can be used to create a poco object.

# CsvStreamReader
~~~cs
    string file = "C:\Temp\data.csv";
    using (CsvStreamReader _reader = new CsvStreamReader(file))
    {
        _reader.Separator = ',';
        while (!_reader.EndOfStream)
        {
            List<string> _values = _reader.ReadCsvLine().ToList();
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
    
    string file = @"D:\Temp\data.csv");

    using (CsvReader<Data> _reader = new(file))
    {        
        _reader.Open();
        _reader.Separator = ','; // or use _reader.DetectSeparator(); 
        var _data = Reader.ReadAsEnumerable().Where(p => p.Column1.Contains("16"));
        var _materialized = _data.ToList();

    }    
~~~
- **Open()**\
Opens the Reader.
- **Separator**\
Set the separator to use (default ',');
- **ReadAsEnumerable()**\
Reads and deserializes each csv file line per iteration in the collection, this allows for querying mega sized files.
- **DetectSeparator()**\
To auto set the separator (looks for commonly used separators in first 10 lines).
- **Skip(int rows)**\
Skip and advances the reader to the next row without interpret it. This is much faster then IEnumerable.Skip(). 
- **Last(int rows)**\
Last seeks the csv document for the last x entries. this is much faster then IEnumerable.Last().
- **Read()**\
Reads current row into T and advances the reader to the next row. 
- **MoveToStart()**\
Moves the reader to the start position, Skip() and Take() alter the start positions use MoveToStart() to reset the position.


# CsvWriter\<T\>
~~~cs
    private IEnumerable<CsvSimple> LargeData()
    {
        for (int ii = 0; ii < 10000000; ii++)
        {
            CsvSimple _line = new()
            {
                AfBij = "bij",
                Bedrag = "100",
                Code = "test",
                Datum = "20200203",
                Mededelingen = $"test {ii}",
                Rekening = "3434",
                Tegenrekening = "3423424"
            };
            yield return _line;
        }
    }
    
    
    string file = @"D:\largedata.csv";
    using (CsvWriter<CsvSimple> _writer = new(file) { Separator = ',', Append = true })
    {
        _writer.Open();
        _writer.Write(LargeData());
    }
      
~~~

- **Open()**\
Opens the Writer.
- **Separator**\
Set the separator to use (default ',');
- **WriteHeader()**\
Write header with property names of T.
- **Write(IEnumerable<T> rows)**\
Writes data to Csv while consuming rows.
- **CRLFMode**\
Determine which mode to use for new lines.
    - CR + LF → Used as a new line character in Windows.
    - CR(Carriage Return) → Used as a new line character in Mac OS before X.
    - LF(Line Feed) → Used as a new line character in Unix/Mac OS X

# ColumnAttribute

The column attribute defines the properties to be serialized or deserialized.

- **Index**\
Defines the index position within the CSV document. Numbers can be skipped for the reader to ignore certain columns, for the writer numbers can also be skipped which leads to empty columns.
- **Header**\
Defines the header text, this property only applies to the CsvWriter, if not specified, the property name is used.
- **OutputFormat**\
Apply a string format, depending on the Property type. This property is for CsvWriter only.
- **OutputNullValue**\
Defines the value to write as a default for null, This property is for CsvWriter only.

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

        // Take last 10 records.
        _result1 = _reader.Last(10).ToList();
    }

~~~

Mind you on the fact that Skip and Take andvances the reader to the next position.\
executing another _reader.ReadAsEnumerable().Where(p => p...).ToList() will Query from position 21. 

Use MoveToStart() to move the reader to the starting position.

_reader.Skip() is different then _reader.ReadAsEnumerable().Skip() as the first does not materialize to T and is faster.