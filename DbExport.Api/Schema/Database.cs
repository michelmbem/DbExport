namespace DbExport.Schema;

public class Database(string name, string providerName, string connectionString) : SchemaItem(null, name)
{
    public string ProviderName { get; } = providerName;

    public string ConnectionString { get; } = connectionString;
    
    public DataTypeCollection DataTypes { get; } = [];

    public TableCollection Tables { get; } = [];

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitDatabase(this);
    }
}