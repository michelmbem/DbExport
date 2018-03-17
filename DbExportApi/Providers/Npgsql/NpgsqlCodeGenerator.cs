using System;
using System.IO;
using DbExport.Schema;

namespace DbExport.Providers.Npgsql
{
    public class NpgsqlCodeGenerator : CodeGenerator
    {
        #region Constructors

        public NpgsqlCodeGenerator()
        {
        }

        public NpgsqlCodeGenerator(TextWriter output)
            : base(output)
        {
        }

        public NpgsqlCodeGenerator(string path)
            : base(path)
        {
        }

        #endregion

        #region New Properties

        public string DbOwner { get; set; }

        #endregion

        #region Overriden Properties

        public override string ProviderName
        {
            get { return "Npgsql"; }
        }

        protected override bool SupportsDbCreation
        {
            get { return false; }
        }

        #endregion

        #region Overriden Methods

        protected override string GetTypeName(Column column)
        {
            var visitIdentities = ExportOptions == null || (ExportOptions.Flags & ExportFlags.ExportIdentities) != ExportFlags.ExportNothing;

            if (visitIdentities && column.IsIdentity) return "serial";

            switch (column.ColumnType)
            {
                case ColumnType.Boolean:
                    return "boolean";
                case ColumnType.TinyInt:
                case ColumnType.UnsignedTinyInt:
                case ColumnType.SmallInt:
                    return "smallint";
                case ColumnType.UnsignedSmallInt:
                case ColumnType.Integer:
                    return "integer";
                case ColumnType.UnsignedInt:
                case ColumnType.BigInt:
                    return "bigint";
                case ColumnType.UnsignedBigInt:
                    return "numeric(20)";
                case ColumnType.Currency:
                    return "numeric(16, 4)";
                case ColumnType.Decimal:
                    if (column.Precision == 0) return "numeric";
                    if (column.Scale == 0) return "numeric(" + column.Precision + ")";
                    return "numeric(" + column.Precision + ", " + column.Scale + ")";
                case ColumnType.SinglePrecision:
                    return "real";
                case ColumnType.DoublePrecision:
                    return "double precision";
                case ColumnType.Date:
                    return "date";
                case ColumnType.Time:
                    return "time";
                case ColumnType.DateTime:
                    return "timestamp";
                case ColumnType.Interval:
                    return "interval";
                case ColumnType.Char:
                case ColumnType.NChar:
                    return "char(" + column.Size + ")";
                case ColumnType.VarChar:
                case ColumnType.NVarChar:
                    return "character varying(" + column.Size + ")";
                case ColumnType.Text:
                case ColumnType.NText:
                case ColumnType.Xml:
                    return "text";
                case ColumnType.Bit:
                    return "bit varying(" + column.Size + ")";
                case ColumnType.Blob:
                case ColumnType.RowVersion:
                    return "bytea";
                case ColumnType.Guid:
                    return "char(36)";
                default:
                    return column.NativeType;
            }
        }

        protected override string GetKeyName(Key key)
        {
            if (key is Index)
            {
                var index = key.Table.Indexes.IndexOf((Index) key);
                return Escape(key.Table.Name + "_ix" + (index + 1));
            }

            if (key is ForeignKey)
            {
                var index = key.Table.ForeignKeys.IndexOf((ForeignKey) key);
                return Escape(key.Table.Name + "_fk" + (index + 1));
            }

            return base.GetKeyName(key);
        }

        protected override string Format(object value, ColumnType columnType)
        {
            if (value == null || value == DBNull.Value) return "NULL";

            switch (columnType)
            {
                case ColumnType.Boolean:
                    return (bool) value ? "true" : "false";
                case ColumnType.Bit:
                case ColumnType.Blob:
                    return "E'" + Utility.GetString((byte[]) value) + "'::bytea";
                case ColumnType.RowVersion:
                    if (value is DateTime)
                        return base.Format(value, ColumnType.DateTime);
                    goto case ColumnType.Blob;
                default:
                    return base.Format(value, columnType);
            }
        }

        protected override void WriteTableCreationSuffix(Table table)
        {
            WriteLine(" WITH (");
            Indent();
            WriteLine("OIDS = FALSE");
            Unindent();
            Write(")");

            if (!string.IsNullOrEmpty(DbOwner))
            {
                WriteDelimiter();
                Write("ALTER TABLE {0} OWNER TO {1}", Escape(table.Name), DbOwner);
            }
        }

        #endregion
    }
}
