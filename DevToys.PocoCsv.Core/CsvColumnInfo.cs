using System.Data;

namespace DevToys.Poco.Csv.Core
{
    /// <summary>
    /// CsvUtils.GetCsvSchema result
    /// </summary>
    public sealed class CsvColumnInfo
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

        /// <summary>
        /// Returns Name
        /// </summary>
        public override string ToString()
        {
            return Name;
        }
    }
}