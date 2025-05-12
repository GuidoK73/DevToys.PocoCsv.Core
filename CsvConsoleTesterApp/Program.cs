// See https://aka.ms/new-console-template for more information
using ConsoleApp1;
using ConsoleApp1.Objects;
using DevToys.PocoCsv.Core.CsvDataTypeObject;


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
    _writer.WriteHeader();
    _writer.Write(_DevToysHelper.LargeData());
}

_w.Stop();
Console.WriteLine("Writer {0}", _w.Duration);


_w.Start();
using (var _reader = new DevToys.PocoCsv.Core.CsvReader<CsvObject>(file, ','))
{
    var _rows = _reader.ReadAsEnumerable().ToList(); // Materialize.
}

_w.Stop();
var _Duration1 = _w.Duration;
Console.WriteLine("Reader Large {0}", _w.Duration);


_w.Start();

using (var _reader = new DevToys.PocoCsv.Core.CsvReader<CsvDataTypeObject>(file, ','))
{
    var _rows = _reader.ReadAsEnumerable().ToList(); // Materialize.
}

_w.Stop();
var _Duration1b = _w.Duration;
Console.WriteLine("Reader Large 2 {0}", _w.Duration);

_w.Start();

using (var _reader = new DevToys.PocoCsv.Core.CsvReader<CsvObjectSmall>(file))
{
    var _rows = _reader.ReadAsEnumerable().ToList(); // Materialize.
}

_w.Stop();
var _Duration2 = _w.Duration;
Console.WriteLine("Reader small {0}", _w.Duration);



