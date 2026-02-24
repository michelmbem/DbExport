using System;
using System.Collections.Immutable;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using DbExport.Providers.Firebird;
using DbExport.Providers.MySqlClient;
using DbExport.Providers.Npgsql;
using DbExport.Providers.OracleClient;
using DbExport.Providers.SqlClient;
using DbExport.Providers.SQLite;
using DbExport.Schema;

namespace DbExport.Providers;

public abstract class CodeGenerator : IVisitor, IDisposable
{
    private readonly bool closeOutput;
    private int indentation;
    private int textColumn;

    #region Constructors

    protected CodeGenerator(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        Output = output;
    }

    protected CodeGenerator() : this(Console.Out) { }

    protected CodeGenerator(string path) : this(File.AppendText(path))
    {
        closeOutput = true;
    }

    #endregion

    #region Static Methods

    public static CodeGenerator Get(string providerName, TextWriter output) =>
        providerName switch
        {
            ProviderNames.SQLSERVER => new SqlCodeGenerator(output),
            ProviderNames.ORACLE => new OracleCodeGenerator(output),
            ProviderNames.MYSQL => new MySqlCodeGenerator(output),
            ProviderNames.POSTGRESQL => new NpgsqlCodeGenerator(output),
            ProviderNames.FIREBIRD => new FirebirdCodeGenerator(output),
            ProviderNames.SQLITE => new SQLiteCodeGenerator(output),
            _ => throw new ArgumentException(null, nameof(providerName))
        };

    #endregion

    #region Properties

    public abstract string ProviderName { get; }

    public ExportOptions ExportOptions { get; set; }

    public TextWriter Output { get; }

    protected virtual bool SupportsDbCreation => true;

    protected virtual bool GeneratesRowVersion => false;

    protected virtual bool RequireInlineConstraints => false;

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && closeOutput)
            Output.Dispose();
    }

    #endregion

    #region IVisitor Members

    public virtual void VisitDatabase(Database database)
    {
        var visitSchema = ExportOptions?.ExportSchema == true;
        var visitData = ExportOptions?.ExportData == true;
        var visitFKs = ExportOptions?.HasFlag(ExportFlags.ExportForeignKeys) == true;
        var visitIdent = ExportOptions?.HasFlag(ExportFlags.ExportIdentities) == true;

        WriteComment("Database: {0}", database.Name);
        WriteComment("Generated on: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        WriteComment("Author: {0}", Environment.UserName);
        WriteLine();

        if (visitSchema)
        {
            if (SupportsDbCreation)
                WriteDbCreationDirective(database);

            foreach (var dataType in database.DataTypes.Where(dataType => dataType.IsChecked))
            {
                dataType.AcceptVisitor(this);
            }

            foreach (var table in database.Tables.Where(table => table.IsChecked))
            {
                table.AcceptVisitor(this);
            }
        }

        if (visitData)
        {
            WriteDataMigrationPrefix();
            
            foreach (var table in database.Tables.Where(table => table.IsChecked))
            {
                var (dr, con) = SqlHelper.OpenTable(table, visitIdent, GeneratesRowVersion);
                var rowsInserted = false;

                using (con)
                using (dr)
                    while (dr.Read())
                    {
                        WriteInsertDirective(table, dr);
                        rowsInserted = true;
                    }
                
                if (rowsInserted) WriteLine();
            }
            
            WriteDataMigrationSuffix();
        }

        if (!visitSchema || !visitFKs || RequireInlineConstraints) return;
        
        foreach (var table in database.Tables)
        {
            if (!table.IsChecked) continue;
            
            var fkExported = false;

            foreach (var fk in table.ForeignKeys.Where(IsSelected))
            {
                fk.AcceptVisitor(this);
                fkExported = true;
            }

            if (fkExported) WriteLine();
        }
    }

    public virtual void VisitTable(Table table)
    {
        var visitPKs = ExportOptions?.HasFlag(ExportFlags.ExportPrimaryKeys) == true;
        var visitIndexes = ExportOptions?.HasFlag(ExportFlags.ExportIndexes) == true;
        var visitFKs = ExportOptions?.HasFlag(ExportFlags.ExportForeignKeys) == true;

        WriteLine("CREATE TABLE {0} (", Escape(table.Name));
        Indent();

        bool comma = false;

        foreach (var column in table.Columns.Where(c => c.IsChecked))
        {
            if (comma) WriteLine(",");
            column.AcceptVisitor(this);
            comma = true;
        }

        if (visitPKs && table.HasPrimaryKey && table.PrimaryKey.AllColumnsAreChecked)
            table.PrimaryKey.AcceptVisitor(this);
            
        if (visitFKs && RequireInlineConstraints)
            foreach (var fk in table.ForeignKeys.Where(IsSelected))
            {
                fk.AcceptVisitor(this);
            }

        WriteLine();
        Unindent();
        Write(")");
        WriteTableCreationSuffix(table);
        WriteDelimiter();
        WriteLine();

        if (!visitIndexes) return;
        
        var indexVisited = false;

        foreach (var index in table.Indexes.Where(IsSelected))
        {
            index.AcceptVisitor(this);
            indexVisited = true;
        }

        if (indexVisited) WriteLine();
    }

    public virtual void VisitColumn(Column column)
    {
        var visitDefaults = ExportOptions?.HasFlag(ExportFlags.ExportDefaults) == true;

        Write("{0} {1}", Escape(column.Name), GetTypeName(column));
            
        if (column.IsRequired) Write(" NOT NULL");

        if (visitDefaults && !Utility.IsEmpty(column.DefaultValue))
            Write(" DEFAULT {0}", Format(column.DefaultValue, column.ColumnType));
    }

    public virtual void VisitPrimaryKey(PrimaryKey primaryKey)
    {
        WriteLine(",");
        Write("PRIMARY KEY (");

        for (var i = 0; i < primaryKey.Columns.Count; ++i)
        {
            if (i > 0) Write(", ");
            Write(Escape(primaryKey.Columns[i].Name));
        }

        Write(")");
    }

    public virtual void VisitIndex(Index index)
    {
        Write("CREATE");
        if (index.IsUnique) Write(" UNIQUE");
        Write(" INDEX {0} ON {1} (", GetKeyName(index), Escape(index.Table.Name));

        for (var i = 0; i < index.Columns.Count; ++i)
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

        for (var i = 0; i < foreignKey.Columns.Count; ++i)
        {
            if (i > 0) Write(", ");
            Write(Escape(foreignKey.Columns[i].Name));
        }

        Write(") REFERENCES {0} (", Escape(foreignKey.RelatedTableName));

        for (var i = 0; i < foreignKey.Columns.Count; ++i)
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

    public virtual void VisitDataType(DataType dataType) { }

    #endregion

    #region Virtual Methods

    protected virtual string Escape(string name) => Utility.Escape(name, ProviderName);

    protected virtual string GetTypeName(Column column)
    {
        if (column.ColumnType == ColumnType.UserDefined)
        {
            var dataType = column.DataType;
            if (dataType != null) return GetTypeReference(dataType);
        }

        return GetTypeName((IDataItem)column);
    }

    protected virtual string GetTypeName(IDataItem item) => IDataItem.GetFullTypeName(item, false);

    protected virtual string GetTypeReference(DataType dataType) => GetTypeName(dataType);

    protected virtual string GetKeyName(Key key) => Escape(key.Name);

    protected virtual string Format(object value, ColumnType columnType)
    {
        if (value == null || value == DBNull.Value) return "NULL";

        switch (columnType)
        {
            case ColumnType.Char or ColumnType.NChar or ColumnType.VarChar or ColumnType.NVarChar or
                ColumnType.Text or ColumnType.NText or ColumnType.Xml or ColumnType.Json or
                ColumnType.Guid or ColumnType.Geometry:
                return Utility.QuotedStr(value);
            case ColumnType.DateTime:
            case ColumnType.RowVersion when value is DateTime:
                return $"'{value:yyyy-MM-dd HH:mm:ss}'";
            case ColumnType.Date:
                return $"'{value:yyyy-MM-dd}'";
            case ColumnType.Time when value is TimeSpan:
                return $"'{value:c}'";
            case ColumnType.Time:
                return $"'{value:HH:mm:ss}'";
            case ColumnType.Bit:
                return $"B'{Utility.ToBitString((byte[])value)}'";
            case ColumnType.Blob:
            case ColumnType.RowVersion when value is byte[]:
            {
                var bytes = (byte[])value;
                return bytes.Length <= 0 ? "''" : $"0x{Utility.BinToHex(bytes)}";
            }
            case ColumnType.Boolean:
            case var _ when Utility.IsBoolean(value):
                return Convert.ToBoolean(value) ? "1" : "0";
            default:
                return Convert.ToString(value, CultureInfo.InvariantCulture);
        }
    }

    protected virtual void WriteComment(string format, params object[] args)
    {
        WriteLine($"-- {format}", args);
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

    protected virtual void WriteTableCreationSuffix(Table table) { }

    protected virtual void WriteDataMigrationPrefix() { }

    protected virtual void WriteDataMigrationSuffix() { }

    protected virtual void WriteUpdateRule(ForeignKeyRule updateRule)
    {
        Write($" ON UPDATE {GetForeignKeyRuleText(updateRule)}");
    }

    protected virtual void WriteDeleteRule(ForeignKeyRule deleteRule)
    {
        Write($" ON DELETE {GetForeignKeyRuleText(deleteRule)}");
    }

    protected virtual string GetForeignKeyRuleText(ForeignKeyRule rule) =>
        rule switch
        {
            ForeignKeyRule.None => "NO ACTION",
            ForeignKeyRule.Restrict => "RESTRICT",
            ForeignKeyRule.Cascade => "CASCADE",
            ForeignKeyRule.SetDefault => "SET DEFAULT",
            ForeignKeyRule.SetNull => "SET NULL",
            _ => string.Empty
        };

    protected virtual void WriteInsertDirective(Table table, DbDataReader dr)
    {
        var skipIdentity = ExportOptions?.HasFlag(ExportFlags.ExportIdentities) == true;
        var insertableColumns = table.Columns.Where(IsSelected).ToImmutableList();
        
        Write("INSERT INTO {0} (", Escape(table.Name));

        var comma = false;
        foreach (var column in insertableColumns)
        {
            if (comma) Write(", ");
            Write(Escape(column.Name));
            comma = true;
        }

        Write(") VALUES (");
        
        comma = false;
        foreach (var column in insertableColumns)
        {
            var columnType = column.ColumnType == ColumnType.UserDefined
                ? column.DataType?.ColumnType ?? ColumnType.VarChar
                : column.ColumnType;
            
            if (comma) Write(", ");
            Write(Format(dr[column.Name], columnType));
            comma = true;
        }

        Write(")");
        WriteDelimiter();

        bool IsSelected(Column c) => !((skipIdentity && c.IsIdentity) ||
                                       (GeneratesRowVersion && c.ColumnType == ColumnType.RowVersion));
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
            for (var i = 0; i < indentation; ++i)
                Output.Write('\t');

        Output.Write(c);
        textColumn = c == '\n' ? 0 : textColumn + 1;
    }

    protected void Write(string s)
    {
        foreach (var c in s)
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

    private static bool IsSelected(Index index) =>
        index.IsChecked && !index.MatchesKey && index.Columns.Count > 0 && index.AllColumnsAreChecked;

    private static bool IsSelected(ForeignKey fk) =>
        fk.IsChecked && fk.AllColumnsAreChecked && fk.RelatedTable is { IsChecked: true };

    #endregion
}