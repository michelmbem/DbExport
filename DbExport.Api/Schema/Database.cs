namespace DbExport.Schema;

/// <summary>
/// Represents a database within the schema, containing metadata about its provider, connection information,
/// data types, and tables. It serves as the root schema item for database-related operations.
/// </summary>
/// <remarks>
/// The <c>Database</c> class encapsulates the essential information about a database, including its name,
/// provider name, connection string, and collections of data types and tables. It implements the
/// <c>ISchemaItem</c> interface and provides methods to accept a visitor pattern for traversing the database's
/// structure.
/// </remarks>
/// <param name="name">The name of the database.</param>
/// <param name="providerName">The name of the database provider used to connect to the database.</param>
/// <param name="connectionString">The connection string used to connect to the database.</param>
public class Database(string name, string providerName, string connectionString) : SchemaItem(null, name)
{
    /// <summary>
    /// The name of the database provider used to connect to the database.
    /// </summary>
    public string ProviderName { get; } = providerName;

    /// <summary>
    /// The connection string used to connect to the database.
    /// </summary>
    public string ConnectionString { get; } = connectionString;
    
    /// <summary>
    /// The collection of data types defined in the database.
    /// </summary>
    public DataTypeCollection DataTypes { get; } = [];

    /// <summary>
    /// The collection of tables in the database.
    /// </summary>
    public TableCollection Tables { get; } = [];

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitDatabase(this);
    }
}