using System;
using System.IO;
using DbExport.Schema;

namespace DbExport.Providers.Npgsql;

public class NpgsqlCodeGenerator : CodeGenerator
{
    #region Constructors

    public NpgsqlCodeGenerator() { }

    public NpgsqlCodeGenerator(TextWriter output) : base(output) { }

    public NpgsqlCodeGenerator(string path) : base(path) { }

    #endregion

    #region New Properties

    public string DbOwner { get; set; }

    #endregion

    #region Overriden Properties

    public override string ProviderName => ProviderNames.POSTGRESQL;

    protected override bool SupportsDbCreation => false;

    #endregion

    #region Overriden Methods

    protected override string GetTypeName(Column column)
    {
        var visitIdentities = ExportOptions == null || ExportOptions.HasFlag(ExportFlags.ExportIdentities);

        if (visitIdentities && column.IsIdentity) return "serial";

        return column.ColumnType switch
        {
            ColumnType.Boolean => "boolean",
            ColumnType.TinyInt or ColumnType.UnsignedTinyInt or ColumnType.SmallInt => "smallint",
            ColumnType.UnsignedSmallInt or ColumnType.Integer => "integer",
            ColumnType.UnsignedInt or ColumnType.BigInt => "bigint",
            ColumnType.UnsignedBigInt => "numeric(20)",
            ColumnType.Currency => "numeric(19, 4)",
            ColumnType.Decimal when column.Precision == 0 => "numeric",
            ColumnType.Decimal when column.Scale == 0 => $"numeric({column.Precision})",
            ColumnType.Decimal => $"numeric({column.Precision}, {column.Scale})",
            ColumnType.SinglePrecision => "real",
            ColumnType.DoublePrecision => "double precision",
            ColumnType.Date => "date",
            ColumnType.Time => "time",
            ColumnType.DateTime => "timestamp",
            ColumnType.Interval => "interval",
            ColumnType.Char or ColumnType.NChar => $"char({column.Size})",
            ColumnType.VarChar or ColumnType.NVarChar => $"character varying({column.Size})",
            ColumnType.Text or ColumnType.NText or ColumnType.Xml or ColumnType.HierarchyId => "text",
            ColumnType.Bit => $"bit varying({column.Size})",
            ColumnType.Blob or ColumnType.RowVersion => "bytea",
            ColumnType.Guid => "uuid",
            ColumnType.Geometry => "geometry",
            ColumnType.Geography => "geography",
            _ => column.NativeType
        };
    }

    protected override string GetKeyName(Key key)
    {
        switch (key)
        {
            case Index ix:
            {
                var index = ix.Table.Indexes.IndexOf(ix);
                return Escape($"{ix.Table.Name}_ix{index + 1}");
            }
            case ForeignKey fk:
            {
                var index = fk.Table.ForeignKeys.IndexOf(fk);
                return Escape($"{fk.Table.Name}_fk{index + 1}");
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
            ColumnType.Boolean => (bool)value ? "true" : "false",
            ColumnType.RowVersion when value is DateTime => base.Format(value, ColumnType.DateTime),
            ColumnType.Bit or ColumnType.Blob or ColumnType.RowVersion =>
                "E'" + Utility.GetString((byte[])value) + "'::bytea",
            _ => base.Format(value, columnType)
        };
    }

    protected override void WriteTableCreationSuffix(Table table)
    {
        WriteLine(" WITH (");
        Indent();
        WriteLine("OIDS = FALSE");
        Unindent();
        Write(")");

        if (string.IsNullOrEmpty(DbOwner)) return;
        
        WriteDelimiter();
        Write("ALTER TABLE {0} OWNER TO {1}", Escape(table.Name), DbOwner);
    }

    #endregion
}