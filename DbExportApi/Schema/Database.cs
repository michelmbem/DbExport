namespace DbExport.Schema
{
    public class Database : SchemaItem
    {
        private readonly TableCollection tables = new TableCollection();

        public Database(string name, string providerName, string connectionString)
            : base(null, name)
        {
            ProviderName = providerName;
            ConnectionString = connectionString;
        }

        public string ProviderName { get; private set; }

        public string ConnectionString { get; private set; }

        public TableCollection Tables
        {
            get { return tables; }
        }

        public override void AcceptVisitor(IVisitor visitor)
        {
            visitor.VisitDatabase(this);
        }
    }
}
