using System.Text;

namespace DevToys.PocoCsv.Core
{

    public interface ICustomCsvParse
    {
        /// <summary>
        /// This method is called when using CsvReader. It can be used as a support function to the Read function when reading per char might be a performance requirement.
        /// if not used, leave the method body empty.
        /// c is the character to use in the result text, escaping has already been done at this point.
        /// </summary>
        /// <param name="colIndex"></param>
        /// <param name="cellPosition"></param>
        /// <param name="c">c is the character to use in the result text, escaping has already been done at this point.</param>
        void Reading(int colIndex, int cellPosition, char c);
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