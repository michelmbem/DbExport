using System;
using System.IO;
using DbExport.Schema;

namespace DbExport.Providers.SQLite;

/// <summary>
/// Generates SQLite-specific SQL scripts for database schema and data migrations.
/// This class is designed to provide a SQLite-compatible implementation of the base
/// <see cref="CodeGenerator"/> functionalities. It facilitates the generation of
/// database schema definitions, constraints, and data migration scripts tailored for
/// SQLite databases.
/// </summary>
public class SQLiteCodeGenerator : CodeGenerator
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the SQLiteCodeGenerator class.
    /// </summary>
    public SQLiteCodeGenerator() { }

    /// <summary>
    /// Initializes a new instance of the SQLiteCodeGenerator class with the specified TextWriter for output.
    /// </summary>
    /// <param name="output">The TextWriter to which the generated SQL will be written. Must not be null.</param>
    public SQLiteCodeGenerator(TextWriter output) : base(output) { }

    /// <summary>
    /// Initializes a new instance of the SQLiteCodeGenerator class that writes output to a file at the specified path.
    /// </summary>
    /// <param name="path">The file path where the generated SQL will be written. Must not be null or empty.</param>
    public SQLiteCodeGenerator(string path) : base(path) { }

    #endregion

    #region Overriden Properties

    public override string ProviderName => ProviderNames.SQLITE;

    protected override bool SupportsDbCreation => false;

    protected override bool RequireInlineConstraints => true;

    #endregion

    #region Overriden Methods

    public override void VisitColumn(Column column)
    {
        var visitIdentities = ExportOptions?.HasFlag(ExportFlags.ExportIdentities) == true;

        if (visitIdentities && column.IsIdentity)
            Write("{0} integer NOT NULL UNIQUE", Escape(column.Name));
        else
            base.VisitColumn(column);
    }

    public override void VisitForeignKey(ForeignKey foreignKey)
    {
        WriteLine(",");
        Write("CONSTRAINT {0} FOREIGN KEY (", GetKeyName(foreignKey));

        for (var i = 0; i < foreignKey.Columns.Count; ++i)
        {
            if (i > 0) Write(", ");
            Write(Escape(foreignKey.Columns[i].Name));
        }

        Write(") REFERENCES {0} (", Escape(foreignKey.RelatedTableName));

        for (var i = 0; i < foreignKey.Columns.Count; ++i)
        {
            if (i > 0) Write(", ");
            Write(Escape(foreignKey.RelatedColumnNames[i]));
        }

        Write(")");

        if (foreignKey.UpdateRule != ForeignKeyRule.None &&
            foreignKey.UpdateRule != ForeignKeyRule.Restrict)
            WriteUpdateRule(foreignKey.UpdateRule);

        if (foreignKey.DeleteRule != ForeignKeyRule.None &&
            foreignKey.DeleteRule != ForeignKeyRule.Restrict)
            WriteDeleteRule(foreignKey.DeleteRule);
    }

    protected override void WriteDataMigrationPrefix()
    {
        WriteLine("PRAGMA foreign_keys = OFF;");
        WriteLine();
    }

    protected override void WriteDataMigrationSuffix()
    {
        WriteLine("PRAGMA foreign_keys = ON;");
    }

    protected override string GetTypeName(IDataItem item) =>
        item.ColumnType switch
        {
            ColumnType.Boolean => "bit",
            ColumnType.TinyInt or ColumnType.UnsignedTinyInt => "tinyint",
            ColumnType.SmallInt or ColumnType.UnsignedSmallInt => "smallint",
            ColumnType.Integer or ColumnType.UnsignedInt => "integer",
            ColumnType.BigInt or ColumnType.UnsignedBigInt => "bigint",
            ColumnType.Currency => "money",
            ColumnType.Decimal when item.Precision == 0 => "decimal",
            ColumnType.Decimal when item.Scale == 0 => $"numeric({item.Precision})",
            ColumnType.Decimal => $"numeric({item.Precision}, {item.Scale})",
            ColumnType.SinglePrecision => "float",
            ColumnType.DoublePrecision or ColumnType.Interval => "real",
            ColumnType.Date or ColumnType.Time or ColumnType.DateTime => "datetime",
            ColumnType.Xml or ColumnType.Json => "ntext",
            ColumnType.Bit or ColumnType.Blob or ColumnType.RowVersion => "image",
            ColumnType.Char or ColumnType.NChar or ColumnType.VarChar or ColumnType.NVarChar or
            ColumnType.Text or ColumnType.NText or ColumnType.Guid or ColumnType.Geometry => base.GetTypeName(item),
            _ => item.NativeType
        };

    protected override string Format(object value, ColumnType columnType)
    {
        if (value == null || value == DBNull.Value) return "NULL";

        return columnType switch
        {
            ColumnType.Bit or ColumnType.Blob or ColumnType.RowVersion =>
                $"X'{Utility.BinToHex((byte[])value)}'",
            _ => base.Format(value, columnType)
        };
    }

    #endregion
}