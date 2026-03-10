using System;
using System.IO;
using DbExport.Schema;

namespace DbExport.Providers.DB2;

/// <summary>
/// A specialized implementation of the <see cref="CodeGenerator"/> class
/// for generating DB2-specific database schema and code representations.
/// This class is responsible for transforming database schema information
/// into DB2-compliant SQL code or other DB2-related output.
/// </summary>
public class DB2CodeGenerator : CodeGenerator
{
    #region Constructors

    /// <inheritdoc/>
    public DB2CodeGenerator() { }

    /// <inheritdoc/>
    public DB2CodeGenerator(TextWriter output) : base(output) { }

    /// <inheritdoc/>
    public DB2CodeGenerator(string path) : base(path) { }

    #endregion

    #region Overriden properties

    /// <inheritdoc/>
    public override string ProviderName => ProviderNames.DB2;

    /// <inheritdoc/>
    protected override bool SupportsDbCreation => false;

    #endregion

    #region Overriden methods

    /// <inheritdoc/>
    protected override string GetTypeName(IDataItem item) =>
        item.ColumnType switch
        {
            ColumnType.Boolean or ColumnType.TinyInt or ColumnType.UnsignedTinyInt => "smallint",
            ColumnType.SmallInt or ColumnType.UnsignedSmallInt => "smallint",
            ColumnType.Integer or ColumnType.UnsignedInt => "integer",
            ColumnType.BigInt or ColumnType.UnsignedBigInt => "bigint",
            ColumnType.Currency => "decimal(19,4)",
            ColumnType.Decimal when item.Precision == 0 => "decimal",
            ColumnType.Decimal when item.Scale == 0 => $"decimal({item.Precision})",
            ColumnType.Decimal => $"decimal({item.Precision},{item.Scale})",
            ColumnType.SinglePrecision => "real",
            ColumnType.DoublePrecision => "double",
            ColumnType.Date => "date",
            ColumnType.Time => "time",
            ColumnType.DateTime => "timestamp",
            ColumnType.Char or ColumnType.NChar => $"char({item.Size})",
            ColumnType.VarChar or ColumnType.NVarChar =>
                item.Size > 0 ? $"varchar({item.Size})" : "clob",
            ColumnType.Text or ColumnType.NText => "clob",
            ColumnType.Bit => $"varchar({item.Size})",
            ColumnType.Blob or ColumnType.RowVersion => "blob",
            ColumnType.Guid => "char(16) for bit data",
            ColumnType.Xml => "xml",
            _ => item.NativeType
        };

    /// <inheritdoc/>
    protected override void WriteIdentitySpecification(Column column)
    {
        Write(" GENERATED ALWAYS AS IDENTITY");
    }

    /// <inheritdoc/>
    protected override string Format(object value, ColumnType columnType)
    {
        if (value == null || value == DBNull.Value)
            return "NULL";

        return columnType switch
        {
            ColumnType.Boolean =>
                Convert.ToBoolean(value) ? "1" : "0",
            ColumnType.RowVersion when value is DateTime =>
                base.Format(value, ColumnType.DateTime),
            ColumnType.Blob or ColumnType.RowVersion =>
                $"X'{Utility.BinToHex((byte[])value)}'",
            _ => base.Format(value, columnType)
        };
    }

    #endregion
}