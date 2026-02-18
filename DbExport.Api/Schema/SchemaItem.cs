using System;

namespace DbExport.Schema;

public abstract class SchemaItem : IVisitorAcceptor
{
    protected SchemaItem(SchemaItem parent, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("The name must be set a non-empty value");

        Parent = parent;
        Name = name;
    }

    public SchemaItem Parent { get; }

    public string Name { get; }

    public virtual string FullName => Name;

    #region Implementation of IVisitorAcceptor

    public virtual void AcceptVisitor(IVisitor visitor)
    {
    }

    #endregion

    public override string ToString() => $"{GetType().Name}[{FullName}]";
}