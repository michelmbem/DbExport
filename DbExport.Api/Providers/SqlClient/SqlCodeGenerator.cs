using System;
using System.IO;
using DbExport.Schema;

namespace DbExport.Providers.SqlClient;

public class SqlCodeGenerator : CodeGenerator
{
    #region Constructors

    public SqlCodeGenerator() { }

    public SqlCodeGenerator(TextWriter output) : base(output) { }

    public SqlCodeGenerator(string path) : base(path) { }

    #endregion

    #region New Properties

    public bool IsLocalDb { get; set; }

    #endregion

    #region Overriden Properties

    public override string ProviderName => ProviderNames.SQLSERVER;

    protected override bool SupportsDbCreation => !IsLocalDb;

    protected override bool GeneratesRowVersion => true;

    #endregion

    #region Overriden Methods

    public override void VisitColumn(Column column)
    {
        base.VisitColumn(column);

        var visitIdentities = ExportOptions == null || ExportOptions.HasFlag(ExportFlags.ExportIdentities);

        if (visitIdentities && column.IsIdentity)
            Write(" Identity({0}, {1})", column.IdentitySeed, column.IdentityIncrement);
    }

    protected override string GetTypeName(ColumnType type, string nativeType, short size, byte precision, byte scale) =>
        type switch
        {
            ColumnType.Boolean => "bit",
            ColumnType.UnsignedTinyInt => "tinyint",
            ColumnType.TinyInt or ColumnType.SmallInt => "smallint",
            ColumnType.UnsignedSmallInt or ColumnType.Integer => "int",
            ColumnType.UnsignedInt or ColumnType.BigInt => "bigint",
            ColumnType.UnsignedBigInt => "decimal(20)",
            ColumnType.Currency => "money",
            ColumnType.Decimal when precision == 0 => "decimal",
            ColumnType.Decimal when scale == 0 => $"decimal({precision})",
            ColumnType.Decimal => $"decimal({precision}, {scale})",
            ColumnType.SinglePrecision => "real",
            ColumnType.DoublePrecision or ColumnType.Interval => "float",
            ColumnType.Char or ColumnType.NChar => $"{type.ToString().ToLower()}({size})",
            ColumnType.VarChar or ColumnType.NVarChar =>
                size > 0 ? $"{type.ToString().ToLower()}({size})" : $"{type.ToString().ToLower()}(max)",
            ColumnType.DateTime or ColumnType.Date or ColumnType.Time or ColumnType.Text or
                ColumnType.NText or ColumnType.Xml or ColumnType.Geometry => type.ToString().ToLower(),
            ColumnType.Bit => size > 0 ? $"varbinary ({(size + 7) / 8})" : "varbinary(max)",
            ColumnType.Blob => "image",
            ColumnType.Guid => "uniqueidentifier",
            ColumnType.RowVersion => "timestamp",
            ColumnType.Json => "nvarchar(max)",
            _ => nativeType
        };

    protected override string GetTypeReference(DataType dataType) => dataType.Name;

    protected override string Format(object value, ColumnType columnType)
    {
        if (value == null || value == DBNull.Value) return "NULL";

        return columnType switch
        {
            ColumnType.Date or ColumnType.Time or ColumnType.DateTime =>
                $"CONVERT(datetime, {base.Format(value, columnType)}, 20)",
            ColumnType.Bit => base.Format(value, ColumnType.Blob),
            _ => base.Format(value, columnType)
        };
    }

    protected override void WriteDelimiter()
    {
        WriteLine();
        WriteLine("GO");
    }

    protected override void WriteDbCreationDirective(Database database)
    {
        Write("CREATE DATABASE {0}", Escape(database.Name));
        WriteDelimiter();
        WriteLine();

        Write("CHECKPOINT");
        WriteDelimiter();
        WriteLine();

        Write("USE {0}", Escape(database.Name));
        WriteDelimiter();
        WriteLine();

        Write("IF db_name() <> '{0}'\n\tRAISERROR('Invalid database context, retry later!', 22, 127) WITH LOG",
              database.Name);
        WriteDelimiter();
        WriteLine();
    }

    #endregion
}