using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DevToys.PocoCsv.Core.Extensions
{
    /// <summary>
    /// Cv extensions for DataTable.
    /// </summary>
    public static class DataTableExtensions
    {
        /// <summary>
        /// Import Csv into DataTable
        /// </summary>
        /// <param name="table"></param>
        /// <param name="file"></param>
        /// <param name="separator">null for auto detect.</param>
        /// <param name="setSchema">default true, in case of importing multiple of the same documents set it to false on the second call.</param>
        /// <param name="skipRows">First row is usually the header.</param>
        public static void ImportCsv(this DataTable table, string file, char? separator = ',', bool setSchema = true, int skipRows = 1)
        {
            int _rowcounter = 0;

            using (CsvStreamReader _reader = new CsvStreamReader(file))
            {
                if (separator == null)
                {
                    _reader.DetectSeparator();
                }
                else
                {
                    _reader.Separator = separator.Value;
                }

                if (setSchema)
                {
                    table.Clear();

                    IEnumerable<CsvColumnInfo> _schema = _reader.GetCsvSchema(20);

                    foreach (var _column in _schema)
                    {
                        table.Columns.Add(_column.Name, typeof(string));
                    }
                }

                table.BeginLoadData();
                while (!_reader.EndOfStream)
                {
                    var _values = _reader.ReadCsvLine();
                    if (_rowcounter >= skipRows)
                    {
                        table.Rows.Add(_values);
                    }
                    _rowcounter++;
                }
                table.EndLoadData();
            }
        }

        /// <summary>
        /// Export datatable to Csv file
        /// </summary>
        /// <param name="table"></param>
        /// <param name="file"></param>
        /// <param name="separator"></param>
        public static void ExportCsv(this DataTable table, string file, char separator = ',')
        {
            using (CsvStreamWriter _writer = new CsvStreamWriter(file))
            {
                _writer.Separator = separator;
                foreach (DataRow row in table.Rows)
                {
                    _writer.WriteCsvLine(row.ItemArray);
                }
                _writer.Flush();
            }
        }
    }
}