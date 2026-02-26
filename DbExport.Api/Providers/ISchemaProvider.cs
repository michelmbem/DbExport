namespace DbExport.Providers;

public interface ISchemaProvider
{
    string ProviderName { get; }

    string ConnectionString { get; }

    string DatabaseName { get; }

    NameOwnerPair[] GetTableNames();

    string[] GetColumnNames(string tableName, string tableOwner);

    string[] GetIndexNames(string tableName, string tableOwner);

    string[] GetFKNames(string tableName, string tableOwner);

    MetaData GetTableMeta(string tableName, string tableOwner);

    MetaData GetColumnMeta(string tableName, string tableOwner, string columnName);

    MetaData GetIndexMeta(string tableName, string tableOwner, string indexName);

    MetaData GetForeignKeyMeta(string tableName, string tableOwner, string fkName);

    NameOwnerPair[] GetTypeNames() => [];

    MetaData GetTypeMeta(string typeName, string typeOwner) => [];
}