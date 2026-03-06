using DbExport.Schema;

namespace DbExport;

/// <summary>
/// Defines the Visitor pattern for traversing the database schema. Each method corresponds to a specific schema element,
/// allowing for operations to be performed on databases, tables, columns, keys, and indexes without modifying their classes.
/// This design promotes separation of concerns and makes it easier to add new operations on the schema elements without changing their structure.
/// </summary>
public interface IVisitor
{
    /// <summary>
    /// Visits a Database object, allowing the visitor to perform operations on the database schema.
    /// This method is the entry point for traversing the database structure.
    /// </summary>
    /// <param name="database">The Database object to be visited.</param>
    void VisitDatabase(Database database);

    /// <summary>
    /// Visits a Table object, allowing the visitor to perform operations on the table schema.
    /// </summary>
    /// <param name="table">The Table object to be visited.</param>
    void VisitTable(Table table);

    /// <summary>
    /// Visits a Column object, allowing the visitor to perform operations on the column schema.
    /// </summary>
    /// <param name="column">The Column object to be visited.</param>
    void VisitColumn(Column column);

    /// <summary>
    /// Visits a PrimaryKey object, allowing the visitor to perform operations on the primary key schema.
    /// </summary>
    /// <param name="primaryKey">The PrimaryKey object to be visited.</param>
    void VisitPrimaryKey(PrimaryKey primaryKey);

    /// <summary>
    /// Visits an Index object, allowing the visitor to perform operations on the index schema.
    /// </summary>
    /// <param name="index">The Index object to be visited.</param>
    void VisitIndex(Index index);

    /// <summary>
    /// Visits a ForeignKey object, allowing the visitor to perform operations on the foreign key schema.
    /// </summary>
    /// <param name="foreignKey">The ForeignKey object to be visited.</param>
    void VisitForeignKey(ForeignKey foreignKey);

    /// <summary>
    /// Visits a DataType object, allowing the visitor to perform operations on the data type schema.
    /// </summary>
    /// <param name="dataType">The DataType object to be visited.</param>
    void VisitDataType(DataType dataType) { }
}