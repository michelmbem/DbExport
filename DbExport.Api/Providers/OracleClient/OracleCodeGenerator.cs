using System;
using System.IO;
using DbExport.Schema;

namespace DbExport.Providers.OracleClient;

public class OracleCodeGenerator : CodeGenerator
{
    #region Constructors

    public OracleCodeGenerator() { }

    public OracleCodeGenerator(TextWriter output) : base(output) { }

    public OracleCodeGenerator(string path) : base(path) { }

    #endregion

    #region Overriden Properties

    public override string ProviderName => ProviderNames.ORACLE;

    protected override bool SupportsDbCreation => false;

    #endregion

    #region Overriden Methods

    public override void VisitColumn(Column column)
    {
        base.VisitColumn(column);

        var visitIdentities = ExportOptions == null || ExportOptions.HasFlag(ExportFlags.ExportIdentities);
        
        if (visitIdentities && column.IsIdentity)
            Write($" GENERATED ALWAYS AS IDENTITY (START WITH {column.IdentitySeed}, INCREMENT BY {column.IdentityIncrement})");
    }

    protected override string GetTypeName(Column column) =>
        column.ColumnType switch
        {
            ColumnType.Boolean => "NUMBER(1)",
            ColumnType.TinyInt or ColumnType.UnsignedTinyInt => "NUMBER(3)",
            ColumnType.SmallInt or ColumnType.UnsignedSmallInt => "NUMBER(5)",
            ColumnType.Integer or ColumnType.UnsignedInt => "NUMBER(10)",
            ColumnType.BigInt or ColumnType.UnsignedBigInt => "NUMBER(20)",
            ColumnType.Currency => "NUMBER(19, 4)",
            ColumnType.Decimal when column.Precision == 0 => "NUMBER",
            ColumnType.Decimal when column.Scale == 0 => $"NUMBER({column.Precision})",
            ColumnType.Decimal => $"NUMBER({column.Precision}, {column.Scale})",
            ColumnType.SinglePrecision => "NUMBER(9, 7)",
            ColumnType.DoublePrecision => "NUMBER(17, 15)",
            ColumnType.Date or ColumnType.Time or ColumnType.DateTime => "DATE",
            ColumnType.Char => $"CHAR({column.Size})",
            ColumnType.NChar => $"NCHAR({column.Size})",
            ColumnType.VarChar => $"VARCHAR2({column.Size})",
            ColumnType.NVarChar => $"NVARCHAR2({column.Size})",
            ColumnType.Text or ColumnType.HierarchyId => "CLOB",
            ColumnType.NText => "NCLOB",
            ColumnType.Bit or ColumnType.Blob or ColumnType.RowVersion => "BLOB",
            ColumnType.File => "BFILE",
            ColumnType.Guid => "CHAR(36)",
            ColumnType.Xml => "XMLType",
            ColumnType.Geometry or ColumnType.Geography => "SDO_GEOMETRY",
            _ => column.NativeType
        };

    protected override string GetKeyName(Key key)
    {
        switch (key)
        {
            case Index ix:
            {
                var index = ix.Table.Indexes.IndexOf(ix);
                return Escape($"{key.Table.Name}_IX{index + 1}");
            }
            case ForeignKey fk:
            {
                var index = fk.Table.ForeignKeys.IndexOf(fk);
                return Escape($"{key.Table.Name}_FK{index + 1}");
            }
            default:
                return base.GetKeyName(key);
        }
    }

    protected override string Format(object value, ColumnType columnType)
    {
        if (value == null || value == DBNull.Value) return "NULL";

        return columnType switch
        {
            ColumnType.Date or ColumnType.Time or ColumnType.DateTime =>
                $"TO_DATE({base.Format(value, columnType)}, 'YYYY-MM-DD HH24:MI:SS')",
            ColumnType.RowVersion when value is DateTime =>
                $"TO_DATE({base.Format(value, columnType)}, 'YYYY-MM-DD HH24:MI:SS')",
            ColumnType.Bit or ColumnType.Blob or ColumnType.RowVersion => "EMPTY_BLOB()",
            _ => base.Format(value, columnType)
        };
    }

    protected override void WriteUpdateRule(ForeignKeyRule updateRule) { }

    protected override void WriteDeleteRule(ForeignKeyRule deleteRule)
    {
        if (deleteRule != ForeignKeyRule.Cascade) return;
        Write(" ON DELETE CASCADE");
    }

    #endregion
}