using System;
using System.Data.Common;
using System.Globalization;
using System.Text;
using DbExport.Schema;

namespace DbExport.Providers.Access
{
    public class AccessSchemaBuilder : IVisitor
    {
        private readonly StringBuilder queryBuilder = new StringBuilder();
        private readonly string connectionString;
        private ADODB.Connection connection;
        
        public AccessSchemaBuilder(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public ExportOptions ExportOptions { get; set; }

        #region IVisitor Members

        public void VisitDatabase(Database database)
        {
            var visitSchema = ExportOptions == null || ExportOptions.ExportSchema;
            var visitData = ExportOptions == null || ExportOptions.ExportData;
            var visitFKs = ExportOptions == null || (ExportOptions.Flags & ExportFlags.ExportForeignKeys) != ExportFlags.ExportNothing;
            var visitIdent = ExportOptions == null || (ExportOptions.Flags & ExportFlags.ExportIdentities) != ExportFlags.ExportNothing;

            var catalog = new ADOX.Catalog();

            if (visitSchema)
            {
                connection = (ADODB.Connection) catalog.Create(connectionString);

                foreach (Table table in database.Tables)
                {
                    if (table.Checked)
                        table.AcceptVisitor(this);
                }
            }
            else
            {
                connection = new ADODB.Connection {ConnectionString = connectionString};
                connection.Open();
                catalog.ActiveConnection = connection;
            }

            if (visitData)
                foreach (Table table in database.Tables)
                {
                    if (table.Checked)
                    {
                        using (DbDataReader dr = SqlHelper.OpenTable(table, visitIdent, false))
                        {
                            while (dr.Read())
                                ImportRecord(table, dr);

                            dr.Close();
                        }
                    }
                }

            if (visitSchema && visitFKs)
                foreach (Table table in database.Tables)
                {
                    if (table.Checked)
                    {
                        foreach (ForeignKey fk in table.ForeignKeys)
                        { 
                            if (fk.RelatedTable.Checked)
                                fk.AcceptVisitor(this);
                        }
                    }
                }
        }

        public void VisitTable(Table table)
        {
            var visitPKs = ExportOptions == null || (ExportOptions.Flags & ExportFlags.ExportPrimaryKeys) != ExportFlags.ExportNothing;
            var visitIndexes = ExportOptions == null || (ExportOptions.Flags & ExportFlags.ExportIndexes) != ExportFlags.ExportNothing;

            Write("CREATE TABLE {0} (", Escape(table.Name));
            
            for (int i = 0; i < table.Columns.Count; ++i)
            {
                if (i > 0) Write(",");
                table.Columns[i].AcceptVisitor(this);
            }

            if (visitPKs && table.HasPrimaryKey)
            {
                Write(",");
                table.PrimaryKey.AcceptVisitor(this);
            }

            Write(")");
            ExecuteQuery();

            if (visitIndexes)
                foreach (Index index in table.Indexes)
                {
                    if (index.MatchesKey || index.Columns.Count <= 0) continue;
                    index.AcceptVisitor(this);
                }
        }

        public void VisitColumn(Column column)
        {
            var visitDefaults = ExportOptions == null || (ExportOptions.Flags & ExportFlags.ExportDefaults) != ExportFlags.ExportNothing;

            Write("{0} {1}", Escape(column.Name), GetTypeName(column, ExportOptions));
            
            if (column.IsRequired) Write(" NOT NULL");

            if (visitDefaults && !Utility.IsEmpty(column.DefaultValue))
                Write(" DEFAULT {0}", Format(column.DefaultValue, column.ColumnType));
        }

        public void VisitPrimaryKey(PrimaryKey primaryKey)
        {
            Write("PRIMARY KEY (");

            for (int i = 0; i < primaryKey.Columns.Count; ++i)
            {
                if (i > 0) Write(", ");
                Write(Escape(primaryKey.Columns[i].Name));
            }

            Write(")");
        }

        public void VisitIndex(Index index)
        {
            Write("CREATE");
            if (index.Unique) Write(" UNIQUE");
            Write(" INDEX {0} ON {1} (", GetKeyName(index), Escape(index.Table.Name));

            for (int i = 0; i < index.Columns.Count; ++i)
            {
                if (i > 0) Write(", ");
                Write(Escape(index.Columns[i].Name));
            }

            Write(")");
            ExecuteQuery();
        }

        public void VisitForeignKey(ForeignKey foreignKey)
        {
            Write("ALTER TABLE {0} ADD CONSTRAINT {1} FOREIGN KEY (",
                  Escape(foreignKey.Table.Name), GetKeyName(foreignKey));

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
                Write(" ON UPDATE " + GetRuleText(foreignKey.UpdateRule));

            if (foreignKey.DeleteRule != ForeignKeyRule.None &&
                foreignKey.DeleteRule != ForeignKeyRule.Restrict)
                Write(" ON DELETE " + GetRuleText(foreignKey.DeleteRule));

            ExecuteQuery();
        }

        #endregion

        #region Utility

        private static string Escape(string name)
        {
            return Utility.Escape(name, "System.Data.OleDb");
        }

        private static string GetKeyName(Key key)
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

            return Escape(key.Name);
        }

        private static string GetTypeName(Column column, ExportOptions options)
        {
            var visitIdentities = options == null || (options.Flags & ExportFlags.ExportIdentities) != ExportFlags.ExportNothing;

            if (visitIdentities && column.IsIdentity) return "counter";

            switch (column.ColumnType)
            {
                case ColumnType.Boolean:
                    return "bit";
                case ColumnType.UnsignedTinyInt:
                    return "byte";
                case ColumnType.TinyInt:
                case ColumnType.SmallInt:
                    return "integer";
                case ColumnType.UnsignedSmallInt:
                case ColumnType.Integer:
                    return "long";
                case ColumnType.UnsignedInt:
                    return "decimal(10)";
                case ColumnType.BigInt:
                case ColumnType.UnsignedBigInt:
                    return "decimal(20)";
                case ColumnType.SinglePrecision:
                    return "single";
                case ColumnType.DoublePrecision:
                case ColumnType.Interval:
                    return "double";
                case ColumnType.Currency:
                    return "currency";
                case ColumnType.Decimal:
                    if (column.Precision == 0) return "decimal";
                    if (column.Scale == 0) return "decimal(" + column.Precision + ")";
                    return "decimal(" + column.Precision + ", " + column.Scale + ")";
                case ColumnType.Date:
                case ColumnType.Time:
                case ColumnType.DateTime:
                    return "datetime";
                case ColumnType.Char:
                case ColumnType.NChar:
                case ColumnType.VarChar:
                case ColumnType.NVarChar:
                    return 0 < column.Size && column.Size <= 255 ? "text(" + column.Size + ")" : "text";
                case ColumnType.Text:
                case ColumnType.NText:
                case ColumnType.Xml:
                    return "text";
                case ColumnType.Bit:
                case ColumnType.Blob:
                case ColumnType.RowVersion:
                    return "oleobject";
                case ColumnType.Guid:
                    return "uniqueidentifier";
                default:
                    return column.NativeType;
            }
        }

        private static string Format(object value, ColumnType columnType)
        {
            if (value == null || value == DBNull.Value) return "NULL";

            switch (columnType)
            {
                case ColumnType.Boolean:
                    return Convert.ToBoolean(value) ? "1" : "0";
                case ColumnType.Char:
                case ColumnType.NChar:
                case ColumnType.VarChar:
                case ColumnType.NVarChar:
                case ColumnType.Text:
                case ColumnType.NText:
                case ColumnType.Guid:
                case ColumnType.Xml:
                    return Utility.QuotedStr(value);
                case ColumnType.Date:
                case ColumnType.Time:
                case ColumnType.DateTime:
                    return "#" + ((DateTime) value).ToString("yyyy-MM-dd HH:mm:ss") + "#";
                case ColumnType.Bit:
                case ColumnType.Blob:
                    var bytes = (byte[]) value;
                    if (bytes.Length <= 0) return "''";
                    return "0x" + Utility.BinToHex(bytes);
                case ColumnType.RowVersion:
                    if (value is DateTime) goto case ColumnType.DateTime;
                    goto case ColumnType.Blob;
                default:
                    if (Utility.IsBoolean(value)) goto case ColumnType.Boolean;
                    return Convert.ToString(value, CultureInfo.GetCultureInfo("en-US"));
            }
        }

        private static string GetRuleText(ForeignKeyRule rule)
        {
            switch (rule)
            {
                case ForeignKeyRule.Cascade:
                    return "CASCADE";
                case ForeignKeyRule.SetDefault:
                    return "SET DEFAULT";
                case ForeignKeyRule.SetNull:
                    return "SET NULL";
                default:
                    return string.Empty;
            }
        }

        private void Write(string s)
        {
            queryBuilder.Append(s);
        }

        private void Write(string format, params object[] values)
        {
            queryBuilder.AppendFormat(format, values);
        }

        private void ExecuteQuery()
        {
            object recs;

            connection.Execute(queryBuilder.ToString(), out recs);
            queryBuilder.Remove(0, queryBuilder.Length);
        }

        private void ImportRecord(Table table, DbDataReader dr)
        {
            var skipIdentity = ExportOptions == null || (ExportOptions.Flags & ExportFlags.ExportIdentities) != ExportFlags.ExportNothing;
            var comma = false;

            Write("INSERT INTO {0} (", Escape(table.Name));

            foreach (Column column in table.Columns)
            {
                if (skipIdentity && column.IsIdentity) continue;
                if (comma) Write(", ");
                Write(Escape(column.Name));
                comma = true;
            }

            Write(") VALUES (");
            comma = false;

            foreach (Column column in table.Columns)
            {
                if (skipIdentity && column.IsIdentity) continue;
                if (comma) Write(", ");
                Write(Format(dr[column.Name], column.ColumnType));
                comma = true;
            }

            Write(")");
            ExecuteQuery();
        }

        #endregion
    }
}
