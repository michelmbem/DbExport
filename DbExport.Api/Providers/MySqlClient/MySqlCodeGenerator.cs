using System.IO;
using DbExport.Schema;

namespace DbExport.Providers.MySqlClient;

public class MySqlCodeGenerator : CodeGenerator
{
    #region Constructors

    public MySqlCodeGenerator() { }

    public MySqlCodeGenerator(TextWriter output) : base(output) { }

    public MySqlCodeGenerator(string path) : base(path) { }

    #endregion

    #region New Properties

    public MySqlOptions MySqlOptions => (MySqlOptions)ExportOptions?.ProviderSpecific;

    #endregion

    #region Overriden Properties

    public override string ProviderName => ProviderNames.MYSQL;

    protected override bool SupportsRowVersion => true;

    #endregion

    #region Overriden Methods

    public override void VisitColumn(Column column)
    {
        base.VisitColumn(column);

        var visitIdentities = ExportOptions == null || ExportOptions.HasFlag(ExportFlags.ExportIdentities);
        
        if (visitIdentities && column.IsIdentity) Write(" AUTO_INCREMENT");
    }

    protected override string GetTypeName(Column column) =>
        column.ColumnType switch
        {
            ColumnType.Boolean => "tinyint(1)",
            ColumnType.TinyInt => "tinyint",
            ColumnType.UnsignedTinyInt => "tinyint unsigned",
            ColumnType.SmallInt => "smallint",
            ColumnType.UnsignedSmallInt => "smallint unsigned",
            ColumnType.Integer => "int",
            ColumnType.UnsignedInt => "int unsigned",
            ColumnType.BigInt => "bigint",
            ColumnType.UnsignedBigInt => "bigint unsigned",
            ColumnType.Currency => "decimal(19, 4)",
            ColumnType.Decimal when column.Precision == 0 => "decimal",
            ColumnType.Decimal when column.Scale == 0 => $"decimal({column.Precision})",
            ColumnType.Decimal => $"decimal({column.Precision}, {column.Scale})",
            ColumnType.SinglePrecision => "float",
            ColumnType.DoublePrecision or ColumnType.Interval => "double",
            ColumnType.Date => "date",
            ColumnType.Time => "time",
            ColumnType.DateTime => "datetime",
            ColumnType.Char or ColumnType.NChar => $"char({column.Size})",
            ColumnType.VarChar or ColumnType.NVarChar => $"varchar({column.Size})",
            ColumnType.Text or ColumnType.NText or ColumnType.Xml => "longtext",
            ColumnType.Bit => $"bit({column.Size})",
            ColumnType.Blob => "longblob",
            ColumnType.Guid => "char(36)",
            ColumnType.RowVersion => "tinyblob",
            _ => column.NativeType
        };

    protected override void WriteTableCreationSuffix(Table table)
    {
        var engine = MySqlOptions?.StorageEngine;
        var charset = MySqlOptions?.CharacterSet.Name;
        var collation = MySqlOptions?.Collation;

        if (!string.IsNullOrEmpty(engine))
        {
            WriteLine();
            WriteLine("ENGINE = {0}", engine);
            
            if (string.IsNullOrEmpty(charset)) return;
            Write("DEFAULT CHARSET = {0}", charset);
            
            if (string.IsNullOrEmpty(collation)) return;
            Write(" COLLATE = {0}", collation);
        }
        else if (!string.IsNullOrEmpty(charset))
        {
            WriteLine();
            Write("DEFAULT CHARSET = {0}", charset);
            
            if (string.IsNullOrEmpty(collation)) return;
            Write("DEFAULT CHARSET = {0} COLLATE = {1}", charset, collation);
        }
    }

    #endregion
}