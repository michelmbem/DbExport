using System.Collections.Generic;

namespace DbExport.Schema;

/// <summary>
/// Represents the primary key of a database table.
/// A primary key uniquely identifies each record in the table
/// and enforces entity integrity within the database schema.
/// </summary>
public class PrimaryKey : Key
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrimaryKey"/> class.
    /// </summary>
    /// <param name="table">The table that owns the primary key.</param>
    /// <param name="name">The name of the primary key constraint.</param>
    /// <param name="columnNames">The names of the columns that make up the primary key.</param>
    public PrimaryKey(Table table, string name, IEnumerable<string> columnNames) :
        base(table, name, columnNames)
    {
        foreach (var column in Columns)
            column.SetAttribute(ColumnAttributes.PKColumn);
    }

    /// <summary>
    /// Gets a value indicating whether the primary key is computed.
    /// </summary>
    public bool IsComputed => Columns.Count == 1 && Columns[0].IsComputed;

    /// <summary>
    /// Gets a value indicating whether the primary key is an identity column.
    /// </summary>
    public bool IsIdentity => Columns.Count == 1 && Columns[0].IsIdentity;

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitPrimaryKey(this);
    }
}