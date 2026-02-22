using System.IO;
using System.Linq;
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

    protected override bool GeneratesRowVersion => true;

    #endregion

    #region Overriden Methods

    public override void VisitColumn(Column column)
    {
        base.VisitColumn(column);

        var visitIdentities = ExportOptions == null || ExportOptions.HasFlag(ExportFlags.ExportIdentities);
        
        if (visitIdentities && column.IsIdentity) Write(" AUTO_INCREMENT");
    }

    protected override string GetTypeName(ColumnType type, string nativeType, short size, byte precision, byte scale)
    {
        return type switch
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
           ColumnType.Decimal when precision == 0 => "decimal",
           ColumnType.Decimal when scale == 0 => $"decimal({precision})",
           ColumnType.Decimal => $"decimal({precision}, {scale})",
           ColumnType.SinglePrecision => "float",
           ColumnType.DoublePrecision or ColumnType.Interval => "double",
           ColumnType.Date => "date",
           ColumnType.Time => "time",
           ColumnType.DateTime => "datetime",
           ColumnType.Char or ColumnType.NChar => $"char({size})",
           ColumnType.VarChar or ColumnType.NVarChar => size > 0 ? $"varchar({size})" : "longtext",
           ColumnType.Text or ColumnType.NText or ColumnType.Xml => "longtext",
           ColumnType.Bit => size > 0 ? $"bit({size})" : "longblob",
           ColumnType.Blob => "longblob",
           ColumnType.Guid => "char(36)",
           ColumnType.RowVersion => "tinyblob",
           ColumnType.Geometry => "geometry",
           _ => nativeType
        };
    }

    protected override string GetTypeReference(DataType dataType)
    {
        if (dataType.PossibleValues.IsEmpty)
            return base.GetTypeReference(dataType);
        
        var members = string.Join(", ", dataType.PossibleValues.Select(v => Format(v, ColumnType.VarChar)));
        
        return dataType.IsEnumerated
             ? $"enum({string.Join(", ", members)})"
             : $"set({string.Join(", ", members)})";
    }

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