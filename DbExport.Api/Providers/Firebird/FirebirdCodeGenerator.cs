using System;
using System.IO;
using DbExport.Schema;

namespace DbExport.Providers.Firebird;

public class FirebirdCodeGenerator : CodeGenerator
{
    #region Constructors

    public FirebirdCodeGenerator() { }

    public FirebirdCodeGenerator(TextWriter output) : base(output) { }

    public FirebirdCodeGenerator(string path) : base(path) { }

    #endregion

    #region New Properties

    public string DbName { get; set; }

    #endregion

    #region Overriden Properties

    public override string ProviderName => ProviderNames.FIREBIRD;

    #endregion

    #region Overriden Methods

    public override void VisitColumn(Column column)
    {
        base.VisitColumn(column);

        var visitIdentities = ExportOptions?.HasFlag(ExportFlags.ExportIdentities) == true;

        if (visitIdentities && column.IsIdentity)
            Write(" GENERATED ALWAYS AS IDENTITY");
    }

    public override void VisitDataType(DataType dataType)
    {
        Write($"CREATE DOMAIN {Escape(dataType.Name)} AS {GetTypeName(dataType)}");

        if (!dataType.IsNullable) Write(" NOT NULL");

        var visitDefaults = ExportOptions?.HasFlag(ExportFlags.ExportDefaults) == true;
        if (visitDefaults && !Utility.IsEmpty(dataType.DefaultValue))
            Write(" DEFAULT {0}", Format(dataType.DefaultValue, dataType.ColumnType));

        WriteLine(";");
        WriteLine();
    }

    protected override string GetTypeName(IDataItem item) =>
        item.ColumnType switch
        {
            ColumnType.Boolean => "BOOLEAN",
            ColumnType.TinyInt or ColumnType.UnsignedTinyInt or ColumnType.SmallInt => "SMALLINT",
            ColumnType.UnsignedSmallInt or ColumnType.Integer => "INTEGER",
            ColumnType.UnsignedInt or ColumnType.BigInt => "BIGINT",
            ColumnType.UnsignedBigInt => "DECIMAL(20)",
            ColumnType.Currency => "DECIMAL(18, 4)",
            ColumnType.Decimal when item.Scale > 0 => $"DECIMAL({item.Precision}, {item.Scale})",
            ColumnType.Decimal when item.Precision > 0 => $"DECIMAL({item.Precision})",
            ColumnType.Decimal => "DECIMAL",
            ColumnType.SinglePrecision => "FLOAT",
            ColumnType.DoublePrecision => "DOUBLE PRECISION",
            ColumnType.Date => "DATE",
            ColumnType.Time => "TIME",
            ColumnType.DateTime => "TIMESTAMP",
            ColumnType.Char or ColumnType.NChar => $"CHAR({item.Size})",
            ColumnType.VarChar or ColumnType.NVarChar when item.Size > 0 => $"VARCHAR({item.Size})",
            ColumnType.VarChar or ColumnType.NVarChar or ColumnType.Text or ColumnType.NText or
                ColumnType.Xml or ColumnType.Json or ColumnType.Geometry => "BLOB SUB_TYPE TEXT",
            ColumnType.Bit => $"CHAR({(item.Size + 7) / 8}) CHARACTER SET OCTETS",
            ColumnType.Blob or ColumnType.File or ColumnType.RowVersion => "BLOB",
            ColumnType.Guid => "CHAR(16) CHARACTER SET OCTETS",
            _ => item.NativeType ?? "VARCHAR(255)"
        };

    protected override string GetTypeReference(DataType dataType) => Escape(dataType.Name);

    protected override string GetKeyName(Key key)
    {
        switch (key)
        {
            case Index ix:
            {
                var index = ix.Table.Indexes.IndexOf(ix);
                return Escape($"{ix.Table.Name}_IX{index + 1}");
            }
            case ForeignKey fk:
            {
                var index = fk.Table.ForeignKeys.IndexOf(fk);
                return Escape($"{fk.Table.Name}_FK{index + 1}");
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
            ColumnType.Boolean => Convert.ToBoolean(value) ? "TRUE" : "FALSE",
            ColumnType.DateTime => $"TIMESTAMP {base.Format(value, columnType)}",
            ColumnType.Date => $"DATE {base.Format(value, columnType)}",
            ColumnType.Time => $"TIME {base.Format(value, columnType)}",
            ColumnType.Blob or ColumnType.RowVersion =>
                $"X'{Utility.BinToHex((byte[])value)}'",
            _ => base.Format(value, columnType)
        };
    }

    protected override void WriteDbCreationDirective(Database database)
    {
        WriteLine($"CREATE DATABASE '{DbName}' DEFAULT CHARACTER SET UTF8;");
        WriteLine();
    }

    protected override void WriteUpdateRule(ForeignKeyRule updateRule)
    {
        // Firebird does NOT support ON UPDATE
    }

    protected override void WriteDeleteRule(ForeignKeyRule deleteRule)
    {
        if (deleteRule != ForeignKeyRule.None &&
            deleteRule != ForeignKeyRule.Restrict)
            Write($" ON DELETE {GetForeignKeyRuleText(deleteRule)}");
    }

    #endregion
}