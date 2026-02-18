using System.Collections.Generic;

namespace DbExport.Providers;

public interface ISchemaProvider
{
    string ProviderName { get; }

    string ConnectionString { get; }

    string DatabaseName { get; }

    (string, string)[] GetTableNames();

    string[] GetColumnNames(string tableName, string owner);

    string[] GetIndexNames(string tableName, string owner);

    string[] GetFKNames(string tableName, string owner);

    Dictionary<string, object> GetTableMeta(string tableName, string owner);

    Dictionary<string, object> GetColumnMeta(string tableName, string owner, string columnName);

    Dictionary<string, object> GetIndexMeta(string tableName, string owner, string indexName);

    Dictionary<string, object> GetForeignKeyMeta(string tableName, string owner, string fkName);
}