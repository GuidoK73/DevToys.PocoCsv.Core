using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Deconstruct extension methods for string[]
    /// </summary>
    public static class CsvExtensionMethods
    {
        /// <summary>
        /// Add Csv Line to StringBuilders
        /// </summary>
        public static StringBuilder AppendCsv(this StringBuilder sb, char separator, params string[] values)
        {
            foreach (string value in values)
            {
                sb.Append(CsvUtils.Escape(value));
                sb.Append(',');
            }
            sb.Length--;
            return sb;
        }

        /// <summary>
        /// Add Csv Line to StringBuilders
        /// </summary>
        public static StringBuilder AppendCsvLine(this StringBuilder sb, char separator, params string[] values)
        {
            foreach (string value in values)
            {
                sb.Append(CsvUtils.Escape(value));
                sb.Append(',');
            }
            sb.Length--;
            sb.AppendLine();
            return sb;
        }

        /// <summary>
        /// string Deconstruct for 10 parameters.
        /// </summary>
        public static void Deconstruct(this string[] array, out string first, out string second, out string third, out string fourth, out string fifth, out string sixth, out string seventh, out string eighth, out string ninth, out string tenth)
        {
            first = array[0];
            second = array[1];
            third = array[2];
            fourth = array[3];
            fifth = array[4];
            sixth = array[5];
            seventh = array[6];
            eighth = array[7];
            ninth = array[8];
            tenth = array[9];
        }

        /// <summary>
        /// string Deconstruct for 9 parameters.
        /// </summary>
        public static void Deconstruct(this string[] array, out string first, out string second, out string third, out string fourth, out string fifth, out string sixth, out string seventh, out string eighth, out string ninth)
        {
            first = array[0];
            second = array[1];
            third = array[2];
            fourth = array[3];
            fifth = array[4];
            sixth = array[5];
            seventh = array[6];
            eighth = array[7];
            ninth = array[8];
        }

        /// <summary>
        /// string Deconstruct for 8 parameters.
        /// </summary>
        public static void Deconstruct(this string[] array, out string first, out string second, out string third, out string fourth, out string fifth, out string sixth, out string seventh, out string eighth)
        {
            first = array[0];
            second = array[1];
            third = array[2];
            fourth = array[3];
            fifth = array[4];
            sixth = array[5];
            seventh = array[6];
            eighth = array[7];
        }

        /// <summary>
        /// string Deconstruct for 7 parameters.
        /// </summary>
        public static void Deconstruct(this string[] array, out string first, out string second, out string third, out string fourth, out string fifth, out string sixth, out string seventh)
        {
            first = array[0];
            second = array[1];
            third = array[2];
            fourth = array[3];
            fifth = array[4];
            sixth = array[5];
            seventh = array[6];
        }

        /// <summary>
        /// string Deconstruct for 6 parameters.
        /// </summary>
        public static void Deconstruct(this string[] array, out string first, out string second, out string third, out string fourth, out string fifth, out string sixth)
        {
            first = array[0];
            second = array[1];
            third = array[2];
            fourth = array[3];
            fifth = array[4];
            sixth = array[5];
        }

        /// <summary>
        /// string Deconstruct for 5 parameters.
        /// </summary>
        public static void Deconstruct(this string[] array, out string first, out string second, out string third, out string fourth, out string fifth)
        {
            first = array[0];
            second = array[1];
            third = array[2];
            fourth = array[3];
            fifth = array[4];
        }

        /// <summary>
        /// string Deconstruct for 4 parameters.
        /// </summary>
        public static void Deconstruct(this string[] array, out string first, out string second, out string third, out string fourth)
        {
            first = array[0];
            second = array[1];
            third = array[2];
            fourth = array[3];
        }

        /// <summary>
        /// string Deconstruct for 3 parameters.
        /// </summary>
        public static void Deconstruct(this string[] array, out string first, out string second, out string third)
        {
            first = array[0];
            second = array[1];
            third = array[2];
        }

        /// <summary>
        /// string Deconstruct for 2 parameters.
        /// </summary>
        public static void Deconstruct(this string[] array, out string first, out string second)
        {
            first = array[0];
            second = array[1];
        }

        /// <summary>
        /// string Deconstruct for 1 parameters.
        /// </summary>
        public static void Deconstruct(this string[] array, out string first)
        {
            first = array[0];
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
        /// Join array to Csv Line.
        /// </summary>
        public static string JoinCsv(this string[] values, char separator = ',')
        {
            return CsvUtils.JoinCsvLine(separator, values);
        }

        /// <summary>
        /// Split string on Csv rules.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string[] SplitCsv(this string value, char separator)
        {
            return CsvUtils.SplitCsvLine(value, separator);
        }
    }
}