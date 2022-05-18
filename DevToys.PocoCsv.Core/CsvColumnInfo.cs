using System.Data;

namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// CsvUtils.GetCsvSchema result
    /// </summary>
    public class CsvColumnInfo
    {
        /// <summary>
        /// Best Database type
        /// </summary>
        public DbType DatabaseType { get; set; }

        /// <summary>
        /// Best C# Type
        /// </summary>
        public NetType DotNetType { get; set; }

        /// <summary>
        /// Column index
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Last Column
        /// </summary>
        public bool IsLast { get; set; }

        /// <summary>
        /// Column Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Column is Nullable.
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// Best Database type
        /// </summary>
        public SqlDbType SqlDatabaseType { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}