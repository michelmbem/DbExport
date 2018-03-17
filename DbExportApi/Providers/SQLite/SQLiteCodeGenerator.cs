using System;
using System.IO;
using DbExport.Schema;

namespace DbExport.Providers.SQLite
{
    public class SQLiteCodeGenerator : CodeGenerator
    {
        #region Constructors

        public SQLiteCodeGenerator()
        {
        }

        public SQLiteCodeGenerator(TextWriter output)
            : base(output)
        {
        }

        public SQLiteCodeGenerator(string path)
            : base(path)
        {
        }

        #endregion

        #region Overriden Properties

        public override string ProviderName
        {
            get { return "System.Data.SQLite"; }
        }

        protected override bool SupportsDbCreation
        {
            get { return false; }
        }

        protected override bool RequireInlineConstraints
        {
            get { return true; }
        }

        #endregion

        #region Overriden Methods

        public override void VisitColumn(Column column)
        {
            var visitIdentities = ExportOptions == null || (ExportOptions.Flags & ExportFlags.ExportIdentities) != ExportFlags.ExportNothing;

            if (visitIdentities && column.IsIdentity)
                Write("{0} integer NOT NULL UNIQUE", Escape(column.Name));
            else
                base.VisitColumn(column);
        }

        public override void VisitForeignKey(ForeignKey foreignKey)
        {
            WriteLine(",");
            Write("CONSTRAINT {0} FOREIGN KEY (", GetKeyName(foreignKey));

            for (int i = 0; i < foreignKey.Columns.Count; ++i)
            {
                if (i > 0) Write(", ");
                Write(Escape(foreignKey.Columns[i].Name));
            }

            Write(") REFERENCES {0} (", Escape(foreignKey.RelatedTableName));

            for (int i = 0; i < foreignKey.Columns.Count; ++i)
            {
                if (i > 0) Write(", ");
                Write(Escape(foreignKey.RelatedColumnNames[i]));
            }

            Write(")");

            if (foreignKey.UpdateRule != ForeignKeyRule.None &&
                foreignKey.UpdateRule != ForeignKeyRule.Restrict)
                WriteUpdateRule(foreignKey.UpdateRule);

            if (foreignKey.DeleteRule != ForeignKeyRule.None &&
                foreignKey.DeleteRule != ForeignKeyRule.Restrict)
                WriteDeleteRule(foreignKey.DeleteRule);
        }

        protected override string GetTypeName(Column column)
        {
            switch (column.ColumnType)
            {
                case ColumnType.Boolean:
                    return "bit";
                case ColumnType.TinyInt:
                case ColumnType.UnsignedTinyInt:
                    return "tinyint";
                case ColumnType.SmallInt:
                case ColumnType.UnsignedSmallInt:
                    return "smallint";
                case ColumnType.Integer:
                case ColumnType.UnsignedInt:
                    return "integer";
                case ColumnType.BigInt:
                case ColumnType.UnsignedBigInt:
                    return "bigint";
                case ColumnType.Currency:
                    return "money";
                case ColumnType.Decimal:
                    if (column.Precision == 0) return "decimal";
                    if (column.Scale == 0) return "numeric(" + column.Precision + ")";
                    return "numeric(" + column.Precision + ", " + column.Scale + ")";
                case ColumnType.SinglePrecision:
                    return "float";
                case ColumnType.DoublePrecision:
                case ColumnType.Interval:
                    return "real";
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
                case ColumnType.Blob:
                case ColumnType.RowVersion:
                    return "image";
                case ColumnType.Guid:
                    return "guid";
                default:
                    return column.NativeType;
            }
        }

        protected override string Format(object value, ColumnType columnType)
        {
            if (value == null || value == DBNull.Value) return "NULL";

            switch (columnType)
            {
                case ColumnType.Bit:
                case ColumnType.Blob:
                case ColumnType.RowVersion:
                    return "X'" + Utility.BinToHex((byte[])value) + "'";
                default:
                    return base.Format(value, columnType);
            }
        }

        #endregion
    }
}
