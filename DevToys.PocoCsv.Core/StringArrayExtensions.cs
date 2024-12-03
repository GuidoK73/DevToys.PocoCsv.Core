namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Deconstruct extension methods for string[]
    /// </summary>
    public static class CsvStreamReaderExtensions
    {
        /// <summary>
        /// Join array to Csv Line.
        /// </summary>
        public static string JoinCsv(this string[] values, char separator = ',')
        {
            return CsvUtils.JoinCsvLine(separator, values);
        }

        public static string[] SplitCsv(this string value, char separator)
        {
            return CsvUtils.SplitCsvLine(value, separator);
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
    }
}