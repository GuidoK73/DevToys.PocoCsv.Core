using System.Text;

namespace DevToys.PocoCsv.Core.CustomParsers
{
    /// <summary>
    /// Parses boolean values like on,true,yes,1 to True and off,false,no,0 to false, anything else is converted to Null.
    /// </summary>
    public sealed class CustomBooleanParserNullable : ICustomCsvParse<bool?>
    {
        /// <summary>
        /// 
        /// </summary>
        public bool? Read(StringBuilder value)
        {
            switch (value.ToString().ToLower())
            {
                case "on":
                case "true":
                case "yes":
                case "1":
                    return true;

                case "off":
                case "false":
                case "no":
                case "0":
                    return false;

                default:
                    return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Write(bool? value)
        {
            if (value.HasValue)
            {
                if (value == true)
                {
                    return "yes";
                }
                return "no";
            }
            return string.Empty;
        }
    }

    /// <summary>
    /// Parses boolean values like on,true,yes,1 to True and off,false,no,0 to false.
    /// </summary>
    public sealed class CustomBooleanParser : ICustomCsvParse<bool>
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Read(StringBuilder value)
        {
            switch (value.ToString().ToLower())
            {
                case "on":
                case "true":
                case "yes":
                case "1":
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Write(bool value)
        {
            if (value == true)
            {
                return "yes";
            }
            return "no";
        }
    }
}