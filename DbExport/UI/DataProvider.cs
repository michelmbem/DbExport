namespace DbExport.UI
{
    public class DataProvider
    {
        private static readonly DataProvider[] providers = {
            new DataProvider("System.Data.OleDb", "Access (.mdb files)"),
            new DataProvider("LocalDB", "SQL Server data file"),
            new DataProvider("System.Data.SqlClient", "SQL Server (2005 and above)"),
            new DataProvider("Oracle.ManagedDataAccess.Client", "Oracle (10g and above)"),
            new DataProvider("MySql.Data.MySqlClient", "MySQL (5.0 and above)"),
            new DataProvider("Npgsql", "PostgreSQL (8.0 and above)"),
            new DataProvider("System.Data.SQLite", "SQLite 3")
        };

        public DataProvider(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public static DataProvider[] All
        {
            get { return providers; }
        }

        public static DataProvider Get(string name)
        {
            foreach (DataProvider provider in providers)
            {
                if (provider.Name != name) continue;
                return provider;
            }

            return null;
        }
    }
}
