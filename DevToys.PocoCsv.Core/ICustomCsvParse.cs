using System.Text;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Implement Custom parser by assigning the CustomParserType on ColumnAttribute
    /// </summary>
    public interface ICustomCsvParse
    {
        /// <summary>
        /// This method is called when using CsvReader. It can be used as a support function to the Read function when reading per char might be a performance requirement.
        /// c is the character to use in the result text, escaping has already been done at this point.
        /// </summary>
        /// <param name="value">Stringbuilder to be appended with c ( value.Append(c); )</param>
        /// <param name="line">Current line number.</param>
        /// <param name="colIndex">Current column index.</param>
        /// <param name="readerPos">Reader position in document.</param>
        /// <param name="linePos">Position on csv line.</param>
        /// <param name="colPos">Position within column.</param>
        /// <param name="c">Current read char.</param>
        void Reading(StringBuilder value, int line, int colIndex, long readerPos, int linePos, int colPos, char c) => value.Append(c);
    }

    /// <summary>
    /// Implement Custom parser by assigning the CustomParserType on ColumnAttribute
    /// </summary>
    public interface ICustomCsvParse<T> : ICustomCsvParse 
    {
        /// <summary>
        /// This function is called when using CsvReader
        /// </summary>
        /// <returns>Return value must be the same as the property type the CustomParser is placed on.</returns>
        T Read(StringBuilder value);

        /// <summary>
        /// This function is called when using CsvWriter
        /// T value must be the same as the property type the CustomParser is placed on.
        /// </summary>
        string Write(T value);
    }
}