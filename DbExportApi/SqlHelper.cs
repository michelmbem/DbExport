using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;
using DbExport.Schema;

namespace DbExport
{
    public delegate TResult FetchFunc<TResult>(DbDataReader dataReader);

    public class SqlHelper : IDisposable
    {
        private readonly DbConnection connection;
        private readonly bool disposeConnection;
        
        public SqlHelper(DbConnection connection)
        {
            this.connection = connection;
            disposeConnection = false;
        }

        public SqlHelper(string providerName, string connectionString)
            : this(Utility.GetConnection(providerName, connectionString))
        {
            disposeConnection = true;
        }

        public SqlHelper(Database database)
            : this(Utility.GetConnection(database))
        {
            disposeConnection = true;
        }

        public string ProviderName
        {
            get
            {
                var fullName = connection.GetType().FullName;
                var lastDot = fullName.LastIndexOf('.');
                return fullName.Substring(0, lastDot);
            }
        }

        public void Dispose()
        {
            if (disposeConnection)
                connection.Dispose();
            GC.SuppressFinalize(this);
        }

        public TResult Query<TResult>(string sql, FetchFunc<TResult> fetchFunc)
        {
            TResult result;

            try
            {
                connection.Open();

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;

                    using (DbDataReader dataReader = command.ExecuteReader())
                    {
                        result = fetchFunc(dataReader);
                        dataReader.Close();
                    }
                }
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        public object QueryScalar(string sql)
        {
            object result;

            try
            {
                connection.Open();

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    result = command.ExecuteScalar();
                }
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        public int Execute(string sql)
        {
            int result;

            try
            {
                connection.Open();

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    result = command.ExecuteNonQuery();
                }
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        public void ExecuteScript(string script)
        {
            switch (ProviderName)
            {
                case "System.Data.SqlClient":
                    ExecuteSqlScript(script);
                    break;
                case "System.Data.OracleClient":
                    ExecuteOracleScript(script);
                    break;
                default:
                    Execute(script);
                    break;
            }
        }

        public static DbDataReader OpenTable(Table table, bool skipIdentity, bool skipRowVersion)
        {
            var sb = new StringBuilder("SELECT ");
            var comma = false;

            foreach (Column column in table.Columns)
            {
                if ((skipIdentity && column.IsIdentity) ||
                    (skipRowVersion && column.ColumnType == ColumnType.RowVersion)) continue;
                if (comma) sb.Append(", ");
                sb.Append(Utility.Escape(column.Name, table.Database.ProviderName));
                comma = true;
            }

            sb.Append(" FROM ").Append(Utility.Escape(table.Name, table.Database.ProviderName));

            var connection = Utility.GetConnection(table.Database);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = sb.ToString();

            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public static object[] FetchIndex(DbDataReader dataReader)
        {
            object[] result = null;

            if (dataReader.Read())
            {
                result = new object[dataReader.FieldCount];
                dataReader.GetValues(result);
            }

            return result;
        }

        public static Dictionary<string, object> FetchAssoc(DbDataReader dataReader)
        {
            Dictionary<string, object> result = null;

            if (dataReader.Read())
            {
                result = new Dictionary<string, object>();
                for (int i = 0; i < dataReader.FieldCount; ++i)
                    result[dataReader.GetName(i)] = dataReader.GetValue(i);
            }

            return result;
        }

        public static List<object[]> FetchIndexList(DbDataReader dataReader)
        {
            var result = new List<object[]>();

            while (dataReader.Read())
            {
                var values = new object[dataReader.FieldCount];
                dataReader.GetValues(values);
                result.Add(values);
            }

            return result;
        }

        public static List<Dictionary<string, object>> FetchAssocList(DbDataReader dataReader)
        {
            var result = new List<Dictionary<string, object>>();

            while (dataReader.Read())
            {
                var values = new Dictionary<string, object>();
                for (int i = 0; i < dataReader.FieldCount; ++i)
                    values[dataReader.GetName(i)] = dataReader.GetValue(i);
                result.Add(values);
            }

            return result;
        }

        public static List<object> FetchList(DbDataReader dataReader)
        {
            var result = new List<object>();

            while (dataReader.Read())
                result.Add(dataReader.GetValue(0));

            return result;
        }

        private void ExecuteSqlScript(string script)
        {
            var match = Regex.Match(script, @"CREATE DATABASE (.+)\s+GO", RegexOptions.IgnoreCase);

            if (match.Success)
            {
                var sqlCreateDb = match.Value.Substring(0, match.Length - 2).TrimEnd();
                Execute(sqlCreateDb);
                script = script.Substring(match.Index + match.Length);
            }

            script = Regex.Replace(script, @"[\r\n]GO[\r\n]", ";\n");
            Execute(script);
        }

        private void ExecuteOracleScript(string script)
        {
            throw new InvalidOperationException("The Oracle Data Provider for .Net does not support running commands in batch mode.\r\n" +
                "Please save the script to a file then run it using the \"SQL*Plus\" custom command from the \"Tools\" menu.");
        }
    }
}
