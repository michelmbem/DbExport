using System;
using System.IO;
using DbExport.Schema;

namespace DbExport.Providers.SqlClient
{
    public class SqlCodeGenerator : CodeGenerator
    {
        #region Constructors

        public SqlCodeGenerator()
        {
        }

        public SqlCodeGenerator(TextWriter output)
            : base(output)
        {
        }

        public SqlCodeGenerator(string path)
            : base(path)
        {
        }

        #endregion

        #region Overriden Properties

        public override string ProviderName
        {
            get { return "System.Data.SqlClient"; }
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
                Write(" Identity({0}, {1})", column.IdentitySeed, column.IdentityIncrement);
        }

        protected override string GetTypeName(Column column)
        {
            switch (column.ColumnType)
            {
                case ColumnType.Boolean:
                    return "bit";
                case ColumnType.UnsignedTinyInt:
                    return "tinyint";
                case ColumnType.TinyInt:
                case ColumnType.SmallInt:
                    return "smallint";
                case ColumnType.UnsignedSmallInt:
                case ColumnType.Integer:
                    return "int";
                case ColumnType.UnsignedInt:
                case ColumnType.BigInt:
                    return "bigint";
                case ColumnType.UnsignedBigInt:
                    return "decimal(20)";
                case ColumnType.Currency:
                    return "money";
                case ColumnType.Decimal:
                    if (column.Precision == 0) return "decimal";
                    if (column.Scale == 0) return "decimal(" + column.Precision + ")";
                    return "decimal(" + column.Precision + ", " + column.Scale + ")";
                case ColumnType.SinglePrecision:
                    return "real";
                case ColumnType.DoublePrecision:
                case ColumnType.Interval:
                    return "float";
                case ColumnType.Date:
                case ColumnType.Time:
                case ColumnType.DateTime:
                    return "datetime";
                case ColumnType.Char:
                case ColumnType.NChar:
                case ColumnType.VarChar:
                case ColumnType.NVarChar:
                    return column.ColumnType.ToString().ToLower() + "(" + column.Size + ")";
                case ColumnType.Text:
                case ColumnType.NText:
                    return column.ColumnType.ToString().ToLower();
                case ColumnType.Bit:
                    return "binary (" + ((column.Size + 7) / 8) + ")";
                case ColumnType.Blob:
                    return "image";
                case ColumnType.Guid:
                    return "uniqueidentifier";
                case ColumnType.RowVersion:
                    return "timestamp";
                case ColumnType.Xml:
                    return "xml";
                default:
                    return column.NativeType;
            }
        }

        protected override string Format(object value, ColumnType columnType)
        {
            if (value == null || value == DBNull.Value) return "NULL";

            switch (columnType)
            {
                case ColumnType.Date:
                case ColumnType.Time:
                case ColumnType.DateTime:
                    return "CONVERT(datetime, " + base.Format(value, columnType) + ", 20)";
                case ColumnType.Bit:
                    return base.Format(value, ColumnType.Blob);
                default:
                    return base.Format(value, columnType);
            }
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

            Write("IF db_name() <> '{0}'\n\tRAISERROR('Invalid database context, retry later!', 22, 127) WITH LOG", database.Name);
            WriteDelimiter();
            WriteLine();
        }

        #endregion
    }
}
