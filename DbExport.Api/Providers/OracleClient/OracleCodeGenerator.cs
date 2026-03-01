using System;
using System.IO;
using DbExport.Schema;

namespace DbExport.Providers.OracleClient;

/// <summary>
/// Generates Oracle-specific SQL code for database schema objects and related functionality.
/// </summary>
public class OracleCodeGenerator : CodeGenerator
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the OracleCodeGenerator class.
    /// </summary>
    public OracleCodeGenerator() { }

    /// <summary>
    /// Initializes a new instance of the OracleCodeGenerator class with the specified TextWriter for output.
    /// </summary>
    /// <param name="output">The TextWriter to which the generated SQL will be written. Must not be null.</param>
    public OracleCodeGenerator(TextWriter output) : base(output) { }

    /// <summary>
    /// Initializes a new instance of the OracleCodeGenerator class that writes output to a file at the specified path.
    /// </summary>
    /// <param name="path">The file path where the generated SQL will be written. Must not be null or empty.</param>
    public OracleCodeGenerator(string path) : base(path) { }

    #endregion

    #region Overriden Properties

    public override string ProviderName => ProviderNames.ORACLE;

    protected override bool SupportsDbCreation => false;

    #endregion

    #region Overriden Methods

    public override void VisitColumn(Column column)
    {
        Write("{0} {1}", Escape(column.Name), GetTypeName(column));

        var visitIdentities = ExportOptions?.HasFlag(ExportFlags.ExportIdentities) == true;

        if (visitIdentities && column.IsIdentity)
            Write($" GENERATED ALWAYS AS IDENTITY (START WITH {column.IdentitySeed}, INCREMENT BY {column.IdentityIncrement})");
        else
        {
            var visitDefaults = ExportOptions?.HasFlag(ExportFlags.ExportDefaults) == true;
            if (visitDefaults && !Utility.IsEmpty(column.DefaultValue))
                Write(" DEFAULT {0}", Format(column.DefaultValue, column.ColumnType));

            if (column.IsRequired) Write(" NOT NULL");
        }
    }

    protected override string GetTypeName(IDataItem item) =>
        item.ColumnType switch
        {
            ColumnType.Boolean => "NUMBER(1)",
            ColumnType.TinyInt or ColumnType.UnsignedTinyInt => "NUMBER(3)",
            ColumnType.SmallInt or ColumnType.UnsignedSmallInt => "NUMBER(5)",
            ColumnType.Integer or ColumnType.UnsignedInt => "NUMBER(10)",
            ColumnType.BigInt or ColumnType.UnsignedBigInt => "NUMBER(20)",
            ColumnType.Currency => "NUMBER(19, 4)",
            ColumnType.Decimal when item.Precision == 0 => "NUMBER",
            ColumnType.Decimal when item.Scale == 0 => $"NUMBER({item.Precision})",
            ColumnType.Decimal => $"NUMBER({item.Precision}, {item.Scale})",
            ColumnType.SinglePrecision => "NUMBER(9, 7)",
            ColumnType.DoublePrecision => "NUMBER(17, 15)",
            ColumnType.Date or ColumnType.Time or ColumnType.DateTime => "DATE",
            ColumnType.Char => $"CHAR({item.Size})",
            ColumnType.NChar => $"NCHAR({item.Size})",
            ColumnType.VarChar => item.Size > 0 ? $"VARCHAR2({item.Size})" : "CLOB",
            ColumnType.NVarChar => item.Size > 0 ? $"NVARCHAR2({item.Size})" : "NCLOB",
            ColumnType.Text => "CLOB",
            ColumnType.NText => "NCLOB",
            ColumnType.Bit or ColumnType.Blob or ColumnType.RowVersion => "BLOB",
            ColumnType.File => "BFILE",
            ColumnType.Guid => "CHAR(36)",
            ColumnType.Xml => "XMLType",
            ColumnType.Json => "JSON",
            ColumnType.Geometry => "SDO_GEOMETRY",
            _ => item.NativeType
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
            ColumnType.DateTime => $"TO_DATE({base.Format(value, columnType)}, 'YYYY-MM-DD HH24:MI:SS')",
            ColumnType.Date => $"TO_DATE({base.Format(value, columnType)}, 'YYYY-MM-DD')",
            ColumnType.Time => $"TO_DATE({base.Format(value, columnType)}, 'HH24:MI:SS')",
            ColumnType.RowVersion when value is DateTime =>
                $"TO_DATE({base.Format(value, ColumnType.DateTime)}, 'YYYY-MM-DD HH24:MI:SS')",
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