using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// Base class for BaseCsvReader and BaseCsvWriter
    /// </summary>
    public abstract class BaseCsvReaderWriter 
    {
        /// <summary>
        /// 
        /// </summary>
        protected string _File = null;

        /// <summary>
        /// 
        /// </summary>
        protected Stream _Stream = null;

        /// <summary>
        /// 
        /// </summary>
        protected char _Separator = ',';

        /// <summary>
        /// 
        /// </summary>
        protected int _BufferSize = -1;



        /// <summary>
        /// The character encoding to use.
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.Default;



    }
}
