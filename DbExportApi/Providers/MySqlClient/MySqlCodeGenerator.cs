using System.IO;
using DbExport.Schema;

namespace DbExport.Providers.MySqlClient
{
    public class MySqlCodeGenerator : CodeGenerator
    {
        #region Constructors

        public MySqlCodeGenerator()
        {
        }

        public MySqlCodeGenerator(TextWriter output)
            : base(output)
        {
        }

        public MySqlCodeGenerator(string path)
            : base(path)
        {
        }

        #endregion

        #region New Properties

        public MySqlOptions MySqlOptions
        {
            get { return ExportOptions == null ? null : ExportOptions.ProviderSpecific as MySqlOptions; }
        }

        #endregion

        #region Overriden Properties

        public override string ProviderName
        {
            get { return "MySql.Data.MySqlClient"; }
        }

        protected override bool SupportsRowVersion
        {
            get { return true; }
        }

        #endregion

        #region Overriden Methods

        public override void VisitColumn(Column column)
        {
            base.VisitColumn(column);

            var visitIdentities = ExportOptions == null || (ExportOptions.Flags & ExportFlags.ExportIdentities) != ExportFlags.ExportNothing;

            if (visitIdentities && column.IsIdentity)
                Write(" AUTO_INCREMENT");
        }

        protected override string GetTypeName(Column column)
        {
            switch (column.ColumnType)
            {
                case ColumnType.Boolean:
                    return "tinyint(1)";
                case ColumnType.TinyInt:
                    return "tinyint";
                case ColumnType.UnsignedTinyInt:
                    return "tinyint unsigned";
                case ColumnType.SmallInt:
                    return "smallint";
                case ColumnType.UnsignedSmallInt:
                    return "smallint unsigned";
                case ColumnType.Integer:
                    return "int";
                case ColumnType.UnsignedInt:
                    return "int unsigned";
                case ColumnType.BigInt:
                    return "bigint";
                case ColumnType.UnsignedBigInt:
                    return "bigint unsigned";
                case ColumnType.Currency:
                    return "decimal(16, 4)";
                case ColumnType.Decimal:
                    if (column.Precision == 0) return "decimal";
                    if (column.Scale == 0) return "decimal(" + column.Precision + ")";
                    return "decimal(" + column.Precision + ", " + column.Scale + ")";
                case ColumnType.SinglePrecision:
                    return "float";
                case ColumnType.DoublePrecision:
                case ColumnType.Interval:
                    return "double";
                case ColumnType.Date:
                    return "date";
                case ColumnType.Time:
                    return "time";
                case ColumnType.DateTime:
                    return "datetime";
                case ColumnType.Char:
                case ColumnType.NChar:
                    return "char(" + column.Size + ")";
                case ColumnType.VarChar:
                case ColumnType.NVarChar:
                    return "varchar(" + column.Size + ")";
                case ColumnType.Text:
                case ColumnType.NText:
                case ColumnType.Xml:
                    return "longtext";
                case ColumnType.Bit:
                    return "bit(" + column.Size + ")";
                case ColumnType.Blob:
                    return "longblob";
                case ColumnType.Guid:
                    return "char(36)";
                case ColumnType.RowVersion:
                    return "timestamp";
                default:
                    return column.NativeType;
            }
        }

        protected override void WriteTableCreationSuffix(Table table)
        {
            string engine = null, charset = null, sortOrder = null;
            
            if (MySqlOptions != null)
            {
                engine = MySqlOptions.StorageEngine;
                charset = MySqlOptions.CharacterSet;
                sortOrder = MySqlOptions.SortOrder;
            }

            if (!string.IsNullOrEmpty(engine))
            {
                WriteLine();
                WriteLine("ENGINE = {0}", engine);

                if (!string.IsNullOrEmpty(charset))
                {
                    if (string.IsNullOrEmpty(sortOrder))
                        Write("DEFAULT CHARSET = {0}", charset);
                    else
                        Write("DEFAULT CHARSET = {0} COLLATE = {1}", charset, sortOrder);
                }
            }
            else if (!string.IsNullOrEmpty(charset))
            {
                WriteLine();

                if (string.IsNullOrEmpty(sortOrder))
                    Write("DEFAULT CHARSET = {0}", charset);
                else
                    Write("DEFAULT CHARSET = {0} COLLATE = {1}", charset, sortOrder);
            }
        }

        #endregion
    }
}
