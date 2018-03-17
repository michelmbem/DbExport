namespace DbExport.Schema
{
    public abstract class ColumnSet : SchemaItem
    {
        private readonly ColumnCollection columns = new ColumnCollection();
        
        protected ColumnSet(SchemaItem parent, string name)
            : base(parent, name)
        {
        }

        public ColumnCollection Columns
        {
            get { return columns; }
        }
    }
}
