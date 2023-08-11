// See https://aka.ms/new-console-template for more information
using ConsoleApp1;
using ConsoleApp1.Objects;


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

using (DevToys.PocoCsv.Core.CsvWriter<CsvObject> _writer = new(file) { Separator = ',' })
{
    _writer.Open();
    _writer.WriteHeader();
    _writer.Write(_DevToysHelper.LargeData());
}

_w.Stop();
Console.WriteLine(_w.Duration);


// ############################################################
// DevToys.PocoCsv.Core by Guidok73



Console.WriteLine("---------------------");
Console.WriteLine("DevToys.PocoCsv.Core by Guidok73");

_w.Start();

using (var _reader = new DevToys.PocoCsv.Core.CsvReader<CsvObject>(file))
{
    _reader.Open();
    _reader.Skip();
    var x = _reader.ReadAsEnumerable().ToList();
}

_w.Stop();
Console.WriteLine(_w.Duration);




