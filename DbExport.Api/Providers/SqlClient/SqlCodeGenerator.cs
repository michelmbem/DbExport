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

        var visitIdentities = ExportOptions?.HasFlag(ExportFlags.ExportIdentities) == true;

        if (visitIdentities && column.IsIdentity)
            Write(" IDENTITY({0}, {1})", column.IdentitySeed, column.IdentityIncrement);
    }

    public override void VisitDataType(DataType dataType)
    {
        WriteLine($"CREATE TYPE dbo.{dataType.Name} FROM {GetTypeName(dataType)};");
        WriteLine();
    }

    protected override string GetTypeName(IDataItem item) =>
        item.ColumnType switch
        {
            ColumnType.Boolean => "bit",
            ColumnType.UnsignedTinyInt => "tinyint",
            ColumnType.TinyInt or ColumnType.SmallInt => "smallint",
            ColumnType.UnsignedSmallInt or ColumnType.Integer => "int",
            ColumnType.UnsignedInt or ColumnType.BigInt => "bigint",
            ColumnType.UnsignedBigInt => "decimal(20)",
            ColumnType.Currency => "money",
            ColumnType.SinglePrecision => "real",
            ColumnType.DoublePrecision or ColumnType.Interval => "float",
            ColumnType.VarChar or ColumnType.NVarChar => item.Size > 0
                ? $"{item.ColumnType.ToString().ToLower()}({item.Size})"
                : $"{item.ColumnType.ToString().ToLower()}(max)",
            ColumnType.Bit => item.Size > 0 ? $"varbinary ({(item.Size + 7) / 8})" : "varbinary(max)",
            ColumnType.Blob => "image",
            ColumnType.Guid => "uniqueidentifier",
            ColumnType.RowVersion => "timestamp",
            ColumnType.Json => "nvarchar(max)",
            ColumnType.Decimal or ColumnType.DateTime or ColumnType.Date or ColumnType.Time or
                ColumnType.Char or ColumnType.NChar or ColumnType.Text or ColumnType.NText or
                ColumnType.Xml or ColumnType.Geometry => base.GetTypeName(item),
            _ => item.NativeType
        };

    protected override string GetTypeReference(DataType dataType) => $"dbo.{dataType.Name}";

    protected override string Format(object value, ColumnType columnType)
    {
        if (value == null || value == DBNull.Value) return "NULL";

        return columnType switch
        {
            ColumnType.DateTime => $"CONVERT(datetime, {base.Format(value, columnType)}, 20)",
            ColumnType.Date  => $"CONVERT(datetime, {base.Format(value, columnType)}, 23)",
            ColumnType.Time => $"CONVERT(datetime, {base.Format(value, columnType)}, 24)",
            ColumnType.Bit => base.Format(value, ColumnType.Blob),
            ColumnType.RowVersion when value is DateTime =>
                $"CONVERT(datetime, {base.Format(value, ColumnType.DateTime)}, 20)",
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