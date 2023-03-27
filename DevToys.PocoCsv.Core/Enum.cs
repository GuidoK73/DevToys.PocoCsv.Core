namespace DevToys.PocoCsv.Core
{
    public enum CRLFMode
    {
        /// <summary>
        /// \r\n = CR + LF → Used as a new line character in Windows
        /// </summary>
        CRLF = 0,

        /// <summary>
        /// \r = CR(Carriage Return) → Used as a new line character in Mac OS before X
        /// </summary>
        CR = 1,

        /// <summary>
        ///  \n = LF(Line Feed) → Used as a new line character in Unix/Mac OS X
        /// </summary>
        LF = 2,
    }
}