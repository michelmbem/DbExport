using System.Collections.Generic;

namespace DbExport.Providers;

public interface ISchemaProvider
{
    string ProviderName { get; }

    string ConnectionString { get; }

    string DatabaseName { get; }

    string[] GetTableNames();

    string[] GetColumnNames(string tableName);

    string[] GetIndexNames(string tableName);

    string[] GetFKNames(string tableName);

    Dictionary<string, object> GetTableMeta(string tableName);

    Dictionary<string, object> GetColumnMeta(string tableName, string columnName);

    Dictionary<string, object> GetIndexMeta(string tableName, string indexName);

    Dictionary<string, object> GetForeignKeyMeta(string tableName, string fkName);
}