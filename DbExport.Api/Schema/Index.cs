using System.Collections.Generic;
using System.Linq;

namespace DbExport.Schema;

/// <summary>
/// Represents a database index associated with a table. An index is used to
/// enhance the performance of database queries by providing quick access to
/// rows in a table based on the values of one or more columns.
/// </summary>
/// <remarks>
/// The Index class inherits from the Key class, which represents a set of
/// columns in the table that define a constraint or relationship. The Index
/// can be unique, primary, or secondary and can be matched against primary
/// or foreign key constraints in the table.
/// </remarks>
public class Index : Key
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Index"/> class.
    /// </summary>
    /// <param name="table">The table that owns the index.</param>
    /// <param name="name">The name of the index.</param>
    /// <param name="columnNames">The names of the columns that make up the index.</param>
    /// <param name="unique">Whether the index is unique.</param>
    /// <param name="primaryKey">Whether the index is a primary key.</param>
    public Index(Table table, string name, IEnumerable<string> columnNames, bool unique, bool primaryKey) :
        base(table, name, columnNames)
    {
        IsUnique = unique;
        IsPrimaryKey = primaryKey;

        foreach (var column in Columns)
            column.SetAttribute(ColumnAttributes.IXColumn);
    }

    /// <summary>
    /// Gets a value indicating whether the index is unique.
    /// </summary>
    public bool IsUnique { get; }

    /// <summary>
    /// Gets a value indicating whether the index is a primary key.
    /// </summary>
    public bool IsPrimaryKey { get; }

    /// <summary>
    /// Gets a value indicating whether the index matches a primary key constraint.
    /// </summary>
    public bool MatchesPrimaryKey => Table.HasPrimaryKey && MatchesSignature(Table.PrimaryKey);

    /// <summary>
    /// Gets a value indicating whether the index matches a foreign key constraint.
    /// </summary>
    public bool MatchesForeignKey => Table.HasForeignKey && Table.ForeignKeys.Any(MatchesSignature);

    /// <summary>
    /// Gets a value indicating whether the index matches a primary or foreign key constraint.
    /// </summary>
    public bool MatchesKey => MatchesPrimaryKey || MatchesForeignKey;

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitIndex(this);
    }
}