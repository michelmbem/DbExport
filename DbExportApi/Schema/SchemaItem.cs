using System;

namespace DbExport.Schema
{
    public abstract class SchemaItem : IVisitorAcceptor
    {
        protected SchemaItem(SchemaItem parent, string name)
        {
            if (name == null || name.Trim().Length <= 0)
                throw new ArgumentException("The name must be set a non-empty value");

            Parent = parent;
            Name = name;
        }

        public SchemaItem Parent { get; private set; }

        public string Name { get; private set; }

        #region Implementation of IVisitorAcceptor

        public virtual void AcceptVisitor(IVisitor visitor)
        {
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (!(obj is SchemaItem)) return false;
            var other = (SchemaItem) obj;
            return GetType() == other.GetType() && Name == other.Name;
        }

        public override int GetHashCode()
        {
            return 13 * GetType().GetHashCode() + Name.GetHashCode();
        }

        public override string ToString()
        {
            return GetType().Name + " : " + Name;
        }
    }
}
