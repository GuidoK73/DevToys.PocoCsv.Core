using System.Data;

namespace DevToys.PocoCsv.Core
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

        /// <summary>
        /// Decontructor.
        /// </summary>
        /// <param name="databaseType"></param>
        /// <param name="dotNetType"></param>
        /// <param name="index"></param>
        /// <param name="isLast"></param>
        /// <param name="name"></param>
        /// <param name="nullable"></param>
        /// <param name="sqlDatabaseType"></param>
        public void Decontruct(out DbType databaseType, out NetType dotNetType, out int index, out bool isLast, out string name, out bool nullable, out SqlDbType sqlDatabaseType)
        {
            databaseType = DatabaseType;
            dotNetType = DotNetType;
            index = Index;
            isLast = IsLast;
            name = Name;
            nullable = Nullable;
            sqlDatabaseType = SqlDatabaseType;
        }

    }
}