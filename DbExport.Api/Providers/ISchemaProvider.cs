using System.Collections.Generic;

namespace DbExport.Providers;

public interface ISchemaProvider
{
    string ProviderName { get; }

    string ConnectionString { get; }

    string DatabaseName { get; }

    (string, string)[] GetTableNames();

    string[] GetColumnNames(string tableName, string tableOwner);

    string[] GetIndexNames(string tableName, string tableOwner);

    string[] GetFKNames(string tableName, string tableOwner);

    Dictionary<string, object> GetTableMeta(string tableName, string tableOwner);

    Dictionary<string, object> GetColumnMeta(string tableName, string tableOwner, string columnName);

    Dictionary<string, object> GetIndexMeta(string tableName, string tableOwner, string indexName);

    Dictionary<string, object> GetForeignKeyMeta(string tableName, string tableOwner, string fkName);

    (string, string)[] GetTypeNames() => [];

    Dictionary<string, object> GetTypeMeta(string typeName, string typeOwner) => [];
}