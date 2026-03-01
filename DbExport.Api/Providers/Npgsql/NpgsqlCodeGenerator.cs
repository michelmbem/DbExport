using System;
using System.IO;
using System.Linq;
using DbExport.Schema;

namespace DbExport.Providers.Npgsql;

/// <summary>
/// Provides functionality for generating database-specific code targeting Npgsql (PostgreSQL).
/// This class extends the <see cref="CodeGenerator"/> base class and overrides certain methods
/// to tailor code generation to the PostgreSQL database platform.
/// </summary>
public class NpgsqlCodeGenerator : CodeGenerator
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the NpgsqlCodeGenerator class.
    /// </summary>
    public NpgsqlCodeGenerator() { }

    /// <summary>
    /// Initializes a new instance of the NpgsqlCodeGenerator class with the specified TextWriter for output.
    /// </summary>
    /// <param name="output">The TextWriter to which the generated SQL will be written. Must not be null.</param>
    public NpgsqlCodeGenerator(TextWriter output) : base(output) { }

    /// <summary>
    /// Initializes a new instance of the NpgsqlCodeGenerator class that writes output to a file at the specified path.
    /// </summary>
    /// <param name="path">The file path where the generated SQL will be written. Must not be null or empty.</param>
    public NpgsqlCodeGenerator(string path) : base(path) { }

    #endregion

    #region Overriden Properties

    public override string ProviderName => ProviderNames.POSTGRESQL;

    #endregion

    #region Overriden Methods

    public override void VisitDataType(DataType dataType)
    {
        if (dataType.PossibleValues.IsEmpty)
        {
            Write($"CREATE DOMAIN {dataType.Name} AS {GetTypeName(dataType)}");
            
            if (!dataType.IsNullable) Write(" NOT NULL");
            
            var visitDefaults = ExportOptions?.HasFlag(ExportFlags.ExportDefaults) == true;
            if (visitDefaults && !Utility.IsEmpty(dataType.DefaultValue))
                Write(" DEFAULT {0}", Format(dataType.DefaultValue, dataType.ColumnType));
        }
        else
        {
            var members = dataType.PossibleValues.Select(v => Format(v, dataType.ColumnType));
            Write($"CREATE TYPE {dataType.Name} AS ENUM ({string.Join(", ", members)})");
        }

        WriteDelimiter();
        WriteLine();
    }

    protected override string GetTypeName(Column column)
    {
        var visitIdentities = ExportOptions?.HasFlag(ExportFlags.ExportIdentities) == true;
        
        if (visitIdentities && column.IsIdentity)
            return column.ColumnType switch
            {
                ColumnType.BigInt or ColumnType.UnsignedBigInt => "bigserial",
                _ => "serial"
            };
        
        return base.GetTypeName(column);
    }

    protected override string GetTypeName(IDataItem item) =>
        item.ColumnType switch
        {
            ColumnType.Boolean => "boolean",
            ColumnType.TinyInt or ColumnType.UnsignedTinyInt or ColumnType.SmallInt => "smallint",
            ColumnType.UnsignedSmallInt or ColumnType.Integer => "integer",
            ColumnType.UnsignedInt or ColumnType.BigInt => "bigint",
            ColumnType.UnsignedBigInt => "numeric(20)",
            ColumnType.Currency => "numeric(19, 4)",
            ColumnType.Decimal when item.Precision == 0 => "numeric",
            ColumnType.Decimal when item.Scale == 0 => $"numeric({item.Precision})",
            ColumnType.Decimal => $"numeric({item.Precision}, {item.Scale})",
            ColumnType.SinglePrecision => "real",
            ColumnType.DoublePrecision => "double precision",
            ColumnType.Date => "date",
            ColumnType.Time => "time",
            ColumnType.DateTime => "timestamp",
            ColumnType.Interval => "interval",
            ColumnType.Char or ColumnType.NChar => $"character({item.Size})",
            ColumnType.VarChar or ColumnType.NVarChar => item.Size > 0 ? $"character varying({item.Size})" : "text",
            ColumnType.Text or ColumnType.NText => "text",
            ColumnType.Bit => item.Size > 0 ? $"bit varying({item.Size})" : "bytea",
            ColumnType.Blob or ColumnType.RowVersion => "bytea",
            ColumnType.Guid => "uuid",
            ColumnType.Xml => "xml",
            ColumnType.Json => "jsonb",
            ColumnType.Geometry => "geometry",
            _ => item.NativeType
        };

    protected override string GetTypeReference(DataType dataType) =>
        dataType.PossibleValues.IsEmpty || dataType.IsEnumerated
            ? $"public.{dataType.Name}"
            : $"public.{dataType.Name}[]";

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
                $@"E'\\x{Utility.BinToHex((byte[])value)}'::bytea",
            _ => base.Format(value, columnType)
        };
    }

    protected override void WriteDbCreationDirective(Database database)
    {
        WriteLine($"CREATE DATABASE {database.Name};");
        WriteLine();
        
        WriteLine($@"\c {database.Name};");
        WriteLine();
    }

    #endregion
}