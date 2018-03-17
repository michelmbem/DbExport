using System;
using System.Data.Common;
using System.Globalization;
using System.IO;
using DbExport.Schema;

namespace DbExport.Providers
{
    public abstract class CodeGenerator : IVisitor, IDisposable
    {
        private readonly TextWriter output;
        private readonly bool closeOutput;
        private int indentation;
        private int textColumn;
        
        #region Constructors

        protected CodeGenerator(TextWriter output)
        {
            if (output == null)
                throw new ArgumentNullException("output");
            this.output = output;
        }

        protected CodeGenerator()
            : this(Console.Out)
        {
        }

        protected CodeGenerator(string path)
            : this(File.AppendText(path))
        {
            closeOutput = true;
        }

        #endregion

        #region Static Methods

        public static CodeGenerator Get(string providerName, TextWriter output)
        {
            switch (providerName)
            {
                case "System.Data.SqlClient":
                    return new SqlClient.SqlCodeGenerator(output);
                case "System.Data.OracleClient":
                    return new OracleClient.OracleCodeGenerator(output);
                case "MySql.Data.MySqlClient":
                    return new MySqlClient.MySqlCodeGenerator(output);
                case "Npgsql":
                    return new Npgsql.NpgsqlCodeGenerator(output);
                case "System.Data.SQLite":
                    return new SQLite.SQLiteCodeGenerator(output);
                default:
                    throw new ArgumentException("providerName");
            }
        }

        #endregion

        #region Properties

        public abstract string ProviderName { get; }

        public ExportOptions ExportOptions { get; set; }

        public TextWriter Output
        {
            get { return output; }
        }

        protected virtual bool SupportsDbCreation
        {
            get { return true; }
        }

        protected virtual bool SupportsRowVersion
        {
            get { return false; }
        }

        protected virtual bool RequireInlineConstraints
        {
            get { return false; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (closeOutput)
            {
                output.Close();
                GC.SuppressFinalize(this);
            }
        }

        #endregion

        #region IVisitor Members

        public virtual void VisitDatabase(Database database)
        {
            var visitSchema = ExportOptions == null || ExportOptions.ExportSchema;
            var visitData = ExportOptions == null || ExportOptions.ExportData;
            var visitFKs = ExportOptions == null || (ExportOptions.Flags & ExportFlags.ExportForeignKeys) != ExportFlags.ExportNothing;
            var visitIdent = ExportOptions == null || (ExportOptions.Flags & ExportFlags.ExportIdentities) != ExportFlags.ExportNothing;

            WriteComment("Database: {0}", database.Name);
            WriteComment("Generated on: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            WriteComment("Author: {0}", Environment.UserName);
            WriteLine();

            if (visitSchema)
            {
                if (SupportsDbCreation)
                    WriteDbCreationDirective(database);

                foreach (Table table in database.Tables)
                {
                    if (table.Checked)
                        table.AcceptVisitor(this);
                }
            }

            if (visitData)
                foreach (Table table in database.Tables)
                {
                    if (table.Checked)
                    {
                        var rowsInserted = false;

                        using (DbDataReader dr = SqlHelper.OpenTable(table, visitIdent, SupportsRowVersion))
                        {
                            while (dr.Read())
                            {
                                WriteInsertDirective(table, dr);
                                rowsInserted = true;
                            }

                            dr.Close();
                        }

                        if (rowsInserted) WriteLine();
                    }
                }

            if (visitSchema && visitFKs && !RequireInlineConstraints)
                foreach (Table table in database.Tables)
                {
                    if (table.Checked)
                    {
                        var fkExported = false;

                        foreach (ForeignKey fk in table.ForeignKeys)
                        {
                            if (fk.RelatedTable.Checked)
                            {
                                fk.AcceptVisitor(this);
                                fkExported = true;
                            }
                        }

                        if (fkExported)
                            WriteLine();
                    }
                }
        }

        public virtual void VisitTable(Table table)
        {
            var visitPKs = ExportOptions == null || (ExportOptions.Flags & ExportFlags.ExportPrimaryKeys) != ExportFlags.ExportNothing;
            var visitIndexes = ExportOptions == null || (ExportOptions.Flags & ExportFlags.ExportIndexes) != ExportFlags.ExportNothing;
            var visitFKs = ExportOptions == null || (ExportOptions.Flags & ExportFlags.ExportForeignKeys) != ExportFlags.ExportNothing;

            WriteLine("CREATE TABLE {0} (", Escape(table.Name));
            Indent();

            for (int i = 0; i < table.Columns.Count; ++i)
            {
                if (i > 0) WriteLine(",");
                table.Columns[i].AcceptVisitor(this);
            }

            if (visitPKs && table.HasPrimaryKey)
                table.PrimaryKey.AcceptVisitor(this);
            
            if (visitFKs && RequireInlineConstraints)
                foreach (ForeignKey fk in table.ForeignKeys)
                    fk.AcceptVisitor(this);

            WriteLine();
            Unindent();
            Write(")");
            WriteTableCreationSuffix(table);
            WriteDelimiter();
            WriteLine();

            if (visitIndexes)
            {
                var indexVisited = false;

                foreach (Index index in table.Indexes)
                {
                    if (index.MatchesKey || index.Columns.Count <= 0) continue;
                    index.AcceptVisitor(this);
                    indexVisited = true;
                }

                if (indexVisited) WriteLine();
            }
        }

        public virtual void VisitColumn(Column column)
        {
            var visitDefaults = ExportOptions == null || (ExportOptions.Flags & ExportFlags.ExportDefaults) != ExportFlags.ExportNothing;

            Write("{0} {1}", Escape(column.Name), GetTypeName(column));
            
            if (column.IsRequired)
                Write(" NOT NULL");

            if (visitDefaults && !Utility.IsEmpty(column.DefaultValue))
                Write(" DEFAULT {0}", Format(column.DefaultValue, column.ColumnType));
        }

        public virtual void VisitPrimaryKey(PrimaryKey primaryKey)
        {
            WriteLine(",");
            Write("PRIMARY KEY (");

            for (int i = 0; i < primaryKey.Columns.Count; ++i)
            {
                if (i > 0) Write(", ");
                Write(Escape(primaryKey.Columns[i].Name));
            }

            Write(")");
        }

        public virtual void VisitIndex(Index index)
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
            WriteDelimiter();
        }

        public virtual void VisitForeignKey(ForeignKey foreignKey)
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
                WriteUpdateRule(foreignKey.UpdateRule);

            if (foreignKey.DeleteRule != ForeignKeyRule.None &&
                foreignKey.DeleteRule != ForeignKeyRule.Restrict)
                WriteDeleteRule(foreignKey.DeleteRule);

            WriteDelimiter();
        }

        #endregion

        #region Virtual Methods

        protected virtual string Escape(string name)
        {
            return Utility.Escape(name, ProviderName);
        }

        protected virtual string GetTypeName(Column column)
        {
            var typeName = column.ColumnType.ToString().ToLower();

            if (typeName.StartsWith("unsigned"))
                return typeName.Substring(8) + " unsigned";

            if (column.ColumnType == ColumnType.Bit || typeName.EndsWith("char"))
                return typeName + "(" + column.Size + ")";

            if (column.ColumnType == ColumnType.Decimal)
            {
                if (column.Precision == 0) return typeName;
                if (column.Scale == 0) return typeName + "(" + column.Precision + ")";
                return typeName + "(" + column.Precision + ", " + column.Scale + ")";
            }

            return typeName;
        }

        protected virtual string GetKeyName(Key key)
        {
            return Escape(key.Name);
        }

        protected virtual string Format(object value, ColumnType columnType)
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
                    return "'" + ((DateTime) value).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                case ColumnType.Bit:
                    return "B'" + Utility.ToBitString((byte[]) value) + "'";
                case ColumnType.Blob:
                    var bytes = (byte[]) value;
                    if (bytes.Length <= 0) return "''";
                    return "0x" + Utility.BinToHex(bytes);
                default:
                    if (Utility.IsBoolean(value)) goto case ColumnType.Boolean;
                    return Convert.ToString(value, CultureInfo.GetCultureInfo("en-US"));
            }
        }

        protected virtual void WriteComment(string format, params object[] args)
        {
            WriteLine("-- " + format, args);
        }

        protected virtual void WriteDelimiter()
        {
            WriteLine(";");
        }

        protected virtual void WriteDbCreationDirective(Database database)
        {
            Write("CREATE DATABASE {0}", Escape(database.Name));
            WriteDelimiter();
            WriteLine();
            Write("USE {0}", Escape(database.Name));
            WriteDelimiter();
            WriteLine();
        }

        protected virtual void WriteTableCreationSuffix(Table table)
        {
        }

        protected virtual void WriteUpdateRule(ForeignKeyRule updateRule)
        {
            Write(" ON UPDATE " + GetForeignKeyRuleText(updateRule));
        }

        protected virtual void WriteDeleteRule(ForeignKeyRule deleteRule)
        {
            Write(" ON DELETE " + GetForeignKeyRuleText(deleteRule));
        }

        protected virtual string GetForeignKeyRuleText(ForeignKeyRule rule)
        {
            switch (rule)
            {
                case ForeignKeyRule.None:
                    return "NO ACTION";
                case ForeignKeyRule.Restrict:
                    return "RESTRICT";
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

        protected virtual void WriteInsertDirective(Table table, DbDataReader dr)
        {
            var skipIdentity = ExportOptions == null || (ExportOptions.Flags & ExportFlags.ExportIdentities) != ExportFlags.ExportNothing;
            var comma = false;

            Write("INSERT INTO {0} (", Escape(table.Name));

            foreach (Column column in table.Columns)
            {
                if ((skipIdentity && column.IsIdentity) ||
                    (SupportsRowVersion && column.ColumnType == ColumnType.RowVersion)) continue;
                if (comma) Write(", ");
                Write(Escape(column.Name));
                comma = true;
            }

            Write(") VALUES (");
            comma = false;
            
            foreach (Column column in table.Columns)
            {
                if ((skipIdentity && column.IsIdentity) ||
                    (SupportsRowVersion && column.ColumnType == ColumnType.RowVersion)) continue;
                if (comma) Write(", ");
                Write(Format(dr[column.Name], column.ColumnType));
                comma = true;
            }

            Write(")");
            WriteDelimiter();
        }

        #endregion

        #region Utility

        protected void Indent()
        {
            ++indentation;
        }

        protected void Unindent()
        {
            if (indentation <= 0) return;
            --indentation;
        }

        protected void Write(char c)
        {
            if (c == '\r') return;

            if (textColumn == 0)
                for (int i = 0; i < indentation; ++i)
                    output.Write('\t');

            output.Write(c);
            textColumn = c == '\n' ? 0 : textColumn + 1;
        }

        protected void Write(string s)
        {
            foreach (char c in s)
                Write(c);
        }

        protected void Write(string format, params object[] values)
        {
            Write(string.Format(format, values));
        }

        protected void WriteLine()
        {
            Write(Environment.NewLine);
        }

        protected void WriteLine(string s)
        {
            Write(s + Environment.NewLine);
        }

        protected void WriteLine(string format, params object[] values)
        {
            Write(format + Environment.NewLine, values);
        }

        #endregion
    }
}
