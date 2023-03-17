# DevToys.PocoCsv.Core 

DevToys.PocoCsv.Core is a class library to read and write to Csv.
It contains CsvStreamReader, CsvStreamWriter and Serialization classes CsvReader<T> and CsvWriter<T>.

- Read/write serialize/deserialize data to and from Csv fast.
- Use Linq to query large CSV files with CsvReader<T>.ReadAsEnumerable().
- Use CsvWriter<T>.Write() to write large data tables to Csv.
- Retrieve schema for a csv file with CsvUtils.GetCsvSchema() which can be used to create a poco object.

Three flavors for reading and writing CSV files:
1. Stream
2. &lt;T&gt; Serialize / Deserialize to T
3. dynamic. Use dynamic to Read or Write CSV, this is slightly slower then <T>


# CsvStreamReader
~~~cs
    string file = "C:\Temp\data.csv";
    using (CsvStreamReader _reader = new CsvStreamReader(file))
    {
        _reader.Separator = ',';
        while (!_reader.EndOfCsvStream)
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
Note: It's possible to use typed properties, but it is limited to basic conversions for the specified culture.


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

# CsvReaderDynamic
~~~cs
    string file = @"C:\Temp\data.csv";
    using (CsvReaderDynamic _reader = new(file))
    {
        _reader.FirstRowIsHeader = true;
        _reader.Open();
        foreach (dynamic row in _reader.ReadAsEnumerable())
        {
            ...
        }
    }
~~~

# CsvWriterDynamic
~~~cs
    string file = @"C:temp\data.csv");

    using (CsvWriterDynamic _writer = new(file))
    {
        dynamic row = new ExpandoObject();
        row.Id = 124;
        row.Name = "Name";

        List<dynamic> _data = new List<dynamic>();
        _data.Add(row);

        _writer.Write(_data);
    }
~~~




