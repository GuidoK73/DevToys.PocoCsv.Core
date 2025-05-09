﻿using System;
using System.Text;

namespace DevToys.PocoCsv.Core.CustomParsers
{
    /// <summary>
    /// Converts a field to upper case.
    /// </summary>
    public sealed class CustomUpperCaseParser : ICustomCsvParse
    {
        /// <summary>
        /// 
        /// </summary>
        public void Reading(StringBuilder value, int line, int colIndex, long readerPos, int linePos, int colPos, char c)
        {
            value.Append(Char.ToUpper(c));
        }
    }
}
