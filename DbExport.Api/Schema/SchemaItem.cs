using System;

namespace DbExport.Schema;

public abstract class SchemaItem : IVisitorAcceptor
{
    protected SchemaItem(SchemaItem parent, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Parent = parent;
        Name = name;
    }

    public SchemaItem Parent { get; }

    public string Name { get; }

    public virtual string FullName => Name;

    #region Implementation of IVisitorAcceptor

    public abstract void AcceptVisitor(IVisitor visitor);

    #endregion

    public override string ToString() => $"{GetType().Name}[{FullName}]";
}