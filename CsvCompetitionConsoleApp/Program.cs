// See https://aka.ms/new-console-template for more information
using ConsoleApp1;
using ConsoleApp1.Objects;
using Csv;
using CsvHelper.Configuration;
using DataAccess;
using DevToys.Poco.Csv.Core;
using LINQtoCSV;
using LumenWorks.Framework.IO.Csv;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using TinyCsvParser;
using TinyCsvParser.Mapping;


Console.WriteLine("Csv Reader Competition.");
Console.WriteLine("Let's read 10 million rows with 9 columns and deserialize it to an object collection. Let's compare some existing readers on the market.");


StopWatch _w = new();

string file = @"D:\largedata.csv";
if (File.Exists(file))
{
    File.Delete(file);
}

_w.Start();


Console.WriteLine("---------------------");
Console.WriteLine("Creating Data File");

var _DevToysHelper = new DevToysHelper();

using (CsvWriter<CsvObject> _writer = new(file) { Separator = ',' })
{
    _writer.Open();
    _writer.WriteHeader();
    _writer.Write(_DevToysHelper.LargeData());
}

_w.Stop();
Console.WriteLine(_w.Duration);

// ############################################################
// LinqToCsv by Matt Perdeck

Console.WriteLine("---------------------");
Console.WriteLine("CsvTools by Mike Stall");

_w.Start();

var dt = DataTable.New.ReadLazy(file);
var _result = dt.Rows.Select(p => new CsvObject() 
    { 
        Datum = p["Datum"],
        NaamOmschrijving = p["NaamOmschrijving"],
        Rekening = p["Rekening"],
        Tegenrekening = p["Tegenrekening"],
        Code = p["Code"],
        AfBij = p["AfBij"],
        Bedrag = p["Bedrag"],
        MutatieSoort = p["MutatieSoort"],
        Mededelingen = p["Mededelingen"]
    }
).ToList();


_w.Stop();
Console.WriteLine(_w.Duration);


// ############################################################
// LinqToCsv by Matt Perdeck

Console.WriteLine("---------------------");
Console.WriteLine("LinqToCsv by Matt Perdeck");

_w.Start();

var inputFileDescription = new CsvFileDescription
{
    SeparatorChar = ',',
    FirstLineHasColumnNames = true
};
var cc = new CsvContext();

IEnumerable<CsvObjectLinqToSql> products = cc.Read<CsvObjectLinqToSql>(file, inputFileDescription);

var _productsMaterialized = products.ToList();

_w.Stop();
Console.WriteLine(_w.Duration);







// ############################################################
// DevToys.Poco.Csv.Core by Guidok73



Console.WriteLine("---------------------");
Console.WriteLine("DevToys.Poco.Csv.Core by Guidok73");

_w.Start();

using (var _reader = new CsvReader<CsvObject>(file))
{
    _reader.Open();
    _reader.Skip();
    var x = _reader.ReadAsEnumerable().ToList();
}

_w.Stop();
Console.WriteLine(_w.Duration);


// ############################################################

Console.WriteLine("---------------------");
Console.WriteLine("TinyCsvParser by Philipp Wagner");

_w.Start();

var csvParserOptions = new CsvParserOptions(false, ';');
var csvMapper = new CsvPersonMapping();
var csvParser = new CsvParser<CsvObject>(csvParserOptions, csvMapper);

var result = csvParser
    .ReadFromFile(file, Encoding.ASCII)
    .ToList();

_w.Stop();
Console.WriteLine(_w.Duration);


// ############################################################

Console.WriteLine("---------------------");
Console.WriteLine("CsvHelper by Josh Close");

_w.Start();

var config = new CsvConfiguration(CultureInfo.InvariantCulture);
config.HasHeaderRecord = false;
config.MissingFieldFound = null;

using (var reader = new StreamReader(file))
using (var csv = new CsvHelper.CsvReader(reader, config))
{

    var records = csv.GetRecords<CsvObject>().ToList();
}

_w.Stop();
Console.WriteLine(_w.Duration);

// ############################################################
// Steve Hansen Csv
// The firat Nuget packet you see when searching github.
// this one needs some additional coding to compare it with the others who have serialization out of the box.

Console.WriteLine("---------------------");
Console.WriteLine("Csv by Steve Hansen");

_w.Start();

var _SteveHansenCsv = new SteveHansenCsv();

using (var fileStream = File.OpenRead(file))
{
    var _SteveHansenCsvResult = _SteveHansenCsv.ReadCsvItems(fileStream).ToList();

}

_w.Stop();
Console.WriteLine(_w.Duration);


// ############################################################
// LumenWorksCsvReader

Console.WriteLine("---------------------");
Console.WriteLine("LumenWorksCsvReader by Sébastien Lorion, Paul Hatcher");

_w.Start();
var _LumenWorksCsvReaderHelper = new LumenWorksCsvReaderHelper();
using (var csv = new CachedCsvReader(new StreamReader(file), true))
{
    var _LumenWorksCsvReaderResult = _LumenWorksCsvReaderHelper.ReadCsvItems(csv).ToList();
}

_w.Stop();
Console.WriteLine(_w.Duration);

