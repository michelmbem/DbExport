using System;

namespace DbExport.Schema;

/// <summary>
/// Represents a base class for all schema items.
/// </summary>
public abstract class SchemaItem : IVisitorAcceptor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SchemaItem"/> class.
    /// </summary>
    /// <param name="parent">A reference to the parent schema item.</param> 
    /// <param name="name">The name of the schema item. Should not be null or whitespace.</param>
    protected SchemaItem(SchemaItem parent, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Parent = parent;
        Name = name;
    }

    /// <summary>
    /// Gets a reference to the parent schema item.
    /// </summary>
    public SchemaItem Parent { get; }

    /// <summary>
    /// Gets the name of the schema item.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the fully qualified name of the schema item, which may include the owner
    /// or other contextual information, depending on the implementation of the derived class.
    /// </summary>
    public virtual string FullName => Name;

    #region Implementation of IVisitorAcceptor

    public abstract void AcceptVisitor(IVisitor visitor);

    #endregion

    public override string ToString() => $"{GetType().Name}[{FullName}]";
}