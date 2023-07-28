using System.Text;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Implement Custom parser by assigning the CustomParserType on ColumnAttribute
    /// </summary>
    public interface ICustomCsvParse<T>
    {
        /// <summary>
        /// This method is called when using CsvReader
        /// </summary>
        /// <returns>Return value must be the same as the property type the CustomParser is placed on.</returns>
        T Read(StringBuilder value);

        /// <summary>
        /// This method is called when using CsvWriter
        /// T value must be the same as the property type the CustomParser is placed on.
        /// </summary>
        string Write(T value);
    }
}