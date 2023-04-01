using System.Text;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Implement Custom parser by assigning the CustomParserType on ColumnAttribute
    /// </summary>
    public interface ICustomCsvParse
    {
        /// <summary>
        /// Implement custom parse.
        /// </summary>
        /// <returns>Return value must be the same as the property type the CustomParser is placed on.</returns>
        public object Parse(StringBuilder value);
    }
}