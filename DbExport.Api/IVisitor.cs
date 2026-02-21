using DbExport.Schema;

namespace DbExport;

public interface IVisitor
{
    void VisitDatabase(Database database);

    void VisitTable(Table table);

    void VisitColumn(Column column);

    void VisitPrimaryKey(PrimaryKey primaryKey);

    void VisitIndex(Index index);

    void VisitForeignKey(ForeignKey foreignKey);
    
    void VisitDataType(DataType dataType) { }
}