using System;
using System.IO;
using DbExport.Schema;

namespace DbExport.Providers.OracleClient
{
    public class OracleCodeGenerator : CodeGenerator
    {
        #region Constructors

        public OracleCodeGenerator()
        {
        }

        public OracleCodeGenerator(TextWriter output)
            : base(output)
        {
        }

        public OracleCodeGenerator(string path)
            : base(path)
        {
        }

        #endregion

        #region Overriden Properties

        public override string ProviderName
        {
            get { return "Oracle.ManagedDataAccess.Client"; }
        }

        protected override bool SupportsDbCreation
        {
            get { return false; }
        }

        #endregion

        #region Overriden Methods

        protected override string GetTypeName(Column column)
        {
            switch (column.ColumnType)
            {
                case ColumnType.Boolean:
                    return "NUMBER(1)";
                case ColumnType.TinyInt:
                case ColumnType.UnsignedTinyInt:
                    return "NUMBER(3)";
                case ColumnType.SmallInt:
                case ColumnType.UnsignedSmallInt:
                    return "NUMBER(5)";
                case ColumnType.Integer:
                case ColumnType.UnsignedInt:
                    return "NUMBER(10)";
                case ColumnType.BigInt:
                case ColumnType.UnsignedBigInt:
                    return "NUMBER(19)";
                case ColumnType.Currency:
                    return "NUMBER(16, 4)";
                case ColumnType.Decimal:
                    if (column.Precision == 0) return "NUMBER";
                    if (column.Scale == 0) return "NUMBER(" + column.Precision + ")";
                    return "NUMBER(" + column.Precision + ", " + column.Scale + ")";
                case ColumnType.SinglePrecision:
                    return "NUMBER(12, 4)";
                case ColumnType.DoublePrecision:
                    return "NUMBER(24, 8)";
                case ColumnType.Date:
                case ColumnType.Time:
                case ColumnType.DateTime:
                    return "DATE";
                case ColumnType.Char:
                    return "CHAR(" + column.Size + ")";
                case ColumnType.NChar:
                    return "NCHAR(" + column.Size + ")";
                case ColumnType.VarChar:
                    return "VARCHAR2(" + column.Size + ")";
                case ColumnType.NVarChar:
                    return "NVARCHAR2(" + column.Size + ")";
                case ColumnType.Text:
                    return "CLOB";
                case ColumnType.NText:
                    return "NCLOB";
                case ColumnType.Bit:
                case ColumnType.Blob:
                case ColumnType.RowVersion:
                    return "BLOB";
                case ColumnType.File:
                    return "BFILE";
                case ColumnType.Guid:
                    return "CHAR(36)";
                case ColumnType.Xml:
                    return "XMLType";
                default:
                    return column.NativeType;
            }
        }

        protected override string GetKeyName(Key key)
        {
            if (key is Index)
            {
                var index = key.Table.Indexes.IndexOf((Index) key);
                return Escape(key.Table.Name + "_IX" + (index + 1));
            }

            if (key is ForeignKey)
            {
                var index = key.Table.ForeignKeys.IndexOf((ForeignKey) key);
                return Escape(key.Table.Name + "_FK" + (index + 1));
            }

            return base.GetKeyName(key);
        }

        protected override string Format(object value, ColumnType columnType)
        {
            if (value == null || value == DBNull.Value) return "NULL";

            switch (columnType)
            {
                case ColumnType.Date:
                case ColumnType.Time:
                case ColumnType.DateTime:
                    return "TO_DATE(" + base.Format(value, columnType) + ", 'YYYY-MM-DD HH24:MI:SS')";
                case ColumnType.Bit:
                case ColumnType.Blob:
                    return "EMPTY_BLOB()";
                case ColumnType.RowVersion:
                    if (value is DateTime) goto case ColumnType.DateTime;
                    goto case ColumnType.Blob;
                default:
                    return base.Format(value, columnType);
            }
        }

        protected override void WriteUpdateRule(ForeignKeyRule updateRule)
        {
        }

        protected override void WriteDeleteRule(ForeignKeyRule deleteRule)
        {
            if (deleteRule != ForeignKeyRule.Cascade) return;
            Write(" ON DELETE CASCADE");
        }

        #endregion
    }
}
