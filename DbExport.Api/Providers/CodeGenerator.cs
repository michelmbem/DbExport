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

/// <summary>
/// Base class for code generators that produce SQL scripts for database schema and data export.
/// This class implements the visitor pattern to traverse the database schema and generate appropriate SQL statements.
/// Derived classes should override the visit methods to provide provider-specific SQL generation logic.
/// The class also manages output writing and supports options for controlling the export process, such as whether to include schema, data, foreign keys, etc.
/// The class implements IDisposable to allow for proper resource management of the output stream, especially when writing to files.
/// </summary>
public abstract class CodeGenerator : IVisitor, IDisposable
{
    private readonly bool closeOutput;
    private int indentation;
    private int textColumn;

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the CodeGenerator class with the specified TextWriter for output.
    /// </summary>
    /// <param name="output">The TextWriter to which the generated SQL will be written. Must not be null.</param>
    protected CodeGenerator(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        Output = output;
    }

    /// <summary>
    /// Initializes a new instance of the CodeGenerator class that writes output to the console.
    /// </summary>
    protected CodeGenerator() : this(Console.Out) { }

    /// <summary>
    /// Initializes a new instance of the CodeGenerator class that writes output to a file at the specified path.
    /// </summary>
    /// <param name="path">The file path where the generated SQL will be written.
    /// The file will be created if it does not exist, or appended to if it does.
    /// Must not be null or empty.</param>
    protected CodeGenerator(string path) : this(File.AppendText(path))
    {
        closeOutput = true;
    }

    #endregion

    #region Static Methods

    /// <summary>
    /// Factory method to create an instance of a CodeGenerator subclass based on the provided database provider name.
    /// </summary>
    /// <param name="providerName">The name of the database provider for which to create a code generator. Supported values include:
    /// "Microsoft.Data.SqlClient" for SQL Server, "Oracle.ManagedDataAccess.Client" for Oracle, "MySqlConnector" for MySQL,
    /// "Npgsql" for PostgreSQL, "FirebirdSql.Data.FirebirdClient" for Firebird, and "System.Data.SQLite" for SQLite.
    /// Must not be null or empty.</param>
    /// <param name="output">The TextWriter to which the generated SQL will be written. Must not be null.</param>
    /// <returns>A CodeGenerator instance specific to the given provider name, initialized with the provided output TextWriter.</returns>
    /// <exception cref="ArgumentException">When the providerName is not recognized as a supported database provider.</exception>
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

    /// <summary>
    /// Gets the name of the database provider for which this code generator is designed to generate SQL scripts.
    /// </summary>
    public abstract string ProviderName { get; }

    /// <summary>
    /// Gets or sets the export options that control the behavior of the code generation process,
    /// such as whether to include schema, data, foreign keys, identities, etc.
    /// </summary>
    public ExportOptions ExportOptions { get; set; }

    /// <summary>
    /// Gets the TextWriter to which the generated SQL will be written.
    /// This property is initialized through the constructor and is used by the code generator to output the generated SQL statements.
    /// </summary>
    public TextWriter Output { get; }

    /// <summary>
    /// Gets a value indicating whether this code generator supports generating a CREATE DATABASE statement as part of the export process.
    /// </summary>
    protected virtual bool SupportsDbCreation => true;

    /// <summary>
    /// Gets a value indicating whether this code generator generates row version columns as part of the data export process.
    /// </summary>
    protected virtual bool GeneratesRowVersion => false;

    /// <summary>
    /// Gets a value indicating whether this code generator requires foreign key constraints to be included inline within the CREATE TABLE statements,
    /// as opposed to being generated as separate ALTER TABLE statements after the tables are created.
    /// </summary>
    protected virtual bool RequireInlineConstraints => false;

    #endregion

    #region IDisposable Members

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <param name="disposing">A value indicating whether the method has been called directly or indirectly by a user's code.
    /// If true, both managed and unmanaged resources can be disposed; if false, only unmanaged resources should be released.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing && closeOutput)
            Output.Dispose();
    }

    #endregion

    #region IVisitor Members

    /// <summary>
    /// <inheritdoc />
    /// </summary>
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
                using var rowSet = SqlHelper.OpenTable(table, visitIdent, GeneratesRowVersion);
                var rowsInserted = false;

                while (rowSet.DataReader.Read())
                {
                    WriteInsertDirective(table, rowSet.DataReader);
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

    /// <summary>
    /// <inheritdoc />
    /// </summary>
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

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public virtual void VisitColumn(Column column)
    {
        var visitDefaults = ExportOptions?.HasFlag(ExportFlags.ExportDefaults) == true;

        Write("{0} {1}", Escape(column.Name), GetTypeName(column));
            
        if (column.IsRequired) Write(" NOT NULL");

        if (visitDefaults && !Utility.IsEmpty(column.DefaultValue))
            Write(" DEFAULT {0}", Format(column.DefaultValue, column.ColumnType));
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
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

    /// <summary>
    /// <inheritdoc />
    /// </summary>
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

    /// <summary>
    /// <inheritdoc />
    /// </summary>
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

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public virtual void VisitDataType(DataType dataType) { }

    #endregion

    #region Virtual Methods

    /// <summary>
    /// Escapes the given name (e.g., table name, column name) according to the syntax rules of the target database provider.
    /// </summary>
    /// <param name="name">The name to be escaped. This could be a table name, column name,
    /// or any identifier that may require escaping to avoid conflicts with reserved keywords or special characters.</param>
    /// <returns>A string representing the escaped name, formatted according to the conventions of the target database provider.</returns>
    protected virtual string Escape(string name) => Utility.Escape(name, ProviderName);

    /// <summary>
    /// Gets the SQL type name for the given column, taking into account the column's data type and any provider-specific type mappings.
    /// </summary>
    /// <param name="column">The column for which to determine the SQL type name.
    /// The method will consider the column's ColumnType and, if it is a user-defined type,
    /// its associated DataType to determine the appropriate SQL type name.</param>
    /// <returns>A string representing the SQL type name for the column, formatted according to the conventions of the target database provider.</returns>
    protected virtual string GetTypeName(Column column)
    {
        if (column.ColumnType == ColumnType.UserDefined)
        {
            var dataType = column.DataType;
            if (dataType != null) return GetTypeReference(dataType);
        }

        return GetTypeName((IDataItem)column);
    }

    /// <summary>
    /// Gets the SQL type name for the given data item, which could be a column or a user-defined data type.
    /// </summary>
    /// <param name="item">The data item for which to determine the SQL type name. This could be a column or a user-defined data type.
    /// The method will use the properties of the data item, such as ColumnType, Size, Precision, and Scale,
    /// to determine the appropriate SQL type name, potentially using provider-specific type mappings and formatting rules.</param>
    /// <returns>A string representing the SQL type name for the data item, formatted according to the conventions of the target database provider.</returns>
    protected virtual string GetTypeName(IDataItem item) => IDataItem.GetFullTypeName(item, false);

    /// <summary>
    /// Gets the SQL type reference for a user-defined data type, which may involve referencing
    /// the data type by name or using a specific syntax depending on the target database provider.
    /// </summary>
    /// <param name="dataType">The user-defined data type for which to get the SQL type reference.
    /// This method is called when a column has a ColumnType of UserDefined,
    /// and the column's DataType property is not null. The method will determine how to reference
    /// this user-defined data type in the generated SQL, which may involve using the data type's name
    /// or a specific syntax depending on the conventions of the target database provider.</param>
    /// <returns>A string representing the SQL type reference for the user-defined data type,
    /// formatted according to the conventions of the target database provider.</returns>
    protected virtual string GetTypeReference(DataType dataType) => GetTypeName(dataType);

    /// <summary>
    /// Gets the name to be used for a key (such as an index or foreign key constraint) in the generated SQL.
    /// </summary>
    /// <param name="key">The key for which to get the name. This method is called when generating SQL for indexes and foreign key constraints,
    /// and it determines how to name these keys in the generated SQL. The default implementation returns the escaped name of the key,
    /// but derived classes can override this method to provide different naming conventions or to include additional information
    /// in the key name as needed by the target database provider.</param>
    /// <returns>A string representing the name to be used for the key in the generated SQL,
    /// formatted according to the conventions of the target database provider.</returns>
    protected virtual string GetKeyName(Key key) => Escape(key.Name);

    /// <summary>
    /// Formats a value for inclusion in a SQL statement, taking into account the value's type and the corresponding column type.
    /// </summary>
    /// <param name="value">The value to be formatted for inclusion in a SQL statement. This could be a default value for a column,
    /// a value being inserted into a table, or any other value that needs to be represented as a literal in the generated SQL.
    /// The method will determine how to format this value based on its type and the specified column type,
    /// ensuring that it is correctly represented in the SQL syntax for the target database provider.</param>
    /// <param name="columnType">The column type that corresponds to the value being formatted. This information is used to determine how to format the value,
    /// such as whether to quote it, how to format dates and times, how to represent binary data, etc., according to the conventions of the target database provider.</param>
    /// <returns>A string representing the formatted value, ready to be included as a literal in a SQL statement,
    /// formatted according to the conventions of the target database provider.</returns>
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

    /// <summary>
    /// Writes a comment line to the output, formatted according to the conventions of the target database provider.
    /// </summary>
    /// <param name="format">A format string that describes the content of the comment.
    /// This string can include placeholders for arguments, which will be replaced by the corresponding values in the args parameter.</param>
    /// <param name="args">The arguments to be formatted into the comment string. These values will replace the placeholders in the format string,
    /// allowing for dynamic content to be included in the comment based on the context of the code generation process,
    /// such as database name, generation timestamp, author, etc.</param>
    protected virtual void WriteComment(string format, params object[] args)
    {
        WriteLine($"-- {format}", args);
    }

    /// <summary>
    /// Writes a statement delimiter (such as a semicolon) to the output, according to the syntax rules of the target database provider.
    /// </summary>
    protected virtual void WriteDelimiter()
    {
        WriteLine(";");
    }

    /// <summary>
    /// Writes a CREATE DATABASE statement for the given database, according to the syntax rules of the target database provider.
    /// </summary>
    /// <param name="database">
    /// The database for which to write the CREATE DATABASE statement. This method will generate the appropriate SQL to create the database,
    /// including the database name and any necessary syntax according to the conventions of the target database provider.
    /// This method is called if the code generator supports database creation and if the export options indicate that the schema should be exported.
    /// </param>
    protected virtual void WriteDbCreationDirective(Database database)
    {
        Write("CREATE DATABASE {0}", Escape(database.Name));
        WriteDelimiter();
        WriteLine();
        Write("USE {0}", Escape(database.Name));
        WriteDelimiter();
        WriteLine();
    }

    /// <summary>
    /// Writes any additional SQL syntax that should be included at the end of a CREATE TABLE statement for the given table.
    /// </summary>
    /// <param name="table">The table for which to write the table creation suffix. This method can be overridden by derived classes
    /// to include additional syntax after the closing parenthesis of a CREATE TABLE statement, such as table options,
    /// storage engine specifications, or other provider-specific syntax that should be included when creating a table.</param>
    protected virtual void WriteTableCreationSuffix(Table table) { }

    /// <summary>
    /// Writes any necessary SQL statements or directives that should be included before the data migration (INSERT statements) for the tables.
    /// </summary>
    protected virtual void WriteDataMigrationPrefix() { }

    /// <summary>
    /// Writes any necessary SQL statements or directives that should be included after the data migration (INSERT statements) for the tables.
    /// </summary>
    protected virtual void WriteDataMigrationSuffix() { }

    /// <summary>
    /// Writes the syntax for the ON UPDATE clause of a foreign key constraint, based on the specified update rule.
    /// </summary>
    /// <param name="updateRule">The foreign key rule that specifies the action to be taken when a referenced row is updated.
    /// This method will generate the appropriate SQL syntax for the ON UPDATE clause of a foreign key constraint,
    /// based on the value of the updateRule parameter, which can indicate actions such as CASCADE, SET NULL, SET DEFAULT, etc.,
    /// according to the conventions of the target database provider.</param>
    protected virtual void WriteUpdateRule(ForeignKeyRule updateRule)
    {
        Write($" ON UPDATE {GetForeignKeyRuleText(updateRule)}");
    }

    /// <summary>
    /// Writes the syntax for the ON DELETE clause of a foreign key constraint, based on the specified delete rule.
    /// </summary>
    /// <param name="deleteRule">The foreign key rule that specifies the action to be taken when a referenced row is deleted.
    /// This method will generate the appropriate SQL syntax for the ON DELETE clause of a foreign key constraint,
    /// based on the value of the deleteRule parameter, which can indicate actions such as CASCADE, SET NULL, SET DEFAULT, etc.,
    /// according to the conventions of the target database provider.</param>
    protected virtual void WriteDeleteRule(ForeignKeyRule deleteRule)
    {
        Write($" ON DELETE {GetForeignKeyRuleText(deleteRule)}");
    }

    /// <summary>
    /// Gets the text representation of a foreign key rule (such as ON UPDATE or ON DELETE actions) based on the specified rule.
    /// </summary>
    /// <param name="rule">The foreign key rule for which to get the text representation.</param>
    /// <returns>A string representing the text of the foreign key rule, such as "CASCADE", "SET NULL", "RESTRICT", etc.,
    /// formatted according to the conventions of the target database provider.</returns>
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

    /// <summary>
    /// Writes an INSERT statement for the given table and data reader, generating the appropriate SQL syntax to insert a row of data into the table.
    /// </summary>
    /// <param name="table">The table into which the data will be inserted.
    /// This method will generate an INSERT statement that targets this table,
    /// including the table name and the columns for which data will be inserted.</param>
    /// <param name="dr">A DbDataReader that contains the data to be inserted into the table. This method will read values from this data reader
    /// to generate the VALUES clause of the INSERT statement, formatting each value according to its type and the corresponding column type,
    /// and ensuring that the generated SQL correctly represents the data for insertion into the target database.</param>
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

    /// <summary>
    /// Increases the indentation level for the generated SQL output. This method is typically called when entering a new block of SQL statements,
    /// such as after a CREATE TABLE statement, to ensure that the generated SQL is properly indented for readability.
    /// The indentation level is managed internally, and the Write methods will use this indentation level
    /// to prefix lines with the appropriate number of tabs or spaces according to the conventions of the target database provider.
    /// The Unindent method should be called when exiting a block to decrease the indentation level accordingly.
    /// </summary>
    protected void Indent()
    {
        ++indentation;
    }

    /// <summary>
    /// Decreases the indentation level for the generated SQL output. This method is typically called when exiting a block of SQL statements.
    /// </summary>
    protected void Unindent()
    {
        if (indentation <= 0) return;
        --indentation;
    }

    /// <summary>
    /// Writes a single character to the output, taking care of indentation and line breaks.
    /// </summary>
    /// <param name="c">The character to be written. This method will ensure that the character is properly formatted
    /// and written to the output, taking into account the current indentation level and the presence
    /// of line breaks in the input string.</param>   
    protected void Write(char c)
    {
        if (c == '\r') return;

        if (textColumn == 0)
            for (var i = 0; i < indentation; ++i)
                Output.Write('\t');

        Output.Write(c);
        textColumn = c == '\n' ? 0 : textColumn + 1;
    }

    /// <summary>
    /// Writes a string to the output, taking care of indentation and line breaks.
    /// </summary>
    /// <param name="s">The string to be written. This method will ensure that the string is properly formatted
    /// and written to the output, taking into account the current indentation level and the presence
    /// of line breaks in the input string.</param>
    protected void Write(string s)
    {
        foreach (var c in s)
            Write(c);
    }

    /// <summary>
    /// Writes a formatted string to the output, taking care of indentation and line breaks.
    /// </summary>
    /// <param name="format">The format string that describes the content to be written.</param>
    /// <param name="values">The objects to be formatted and written to the output.</param> 
    protected void Write(string format, params object[] values)
    {
        Write(string.Format(format, values));
    }

    /// <summary>
    /// Writes a line break to the output, taking care of indentation.
    /// </summary>
    protected void WriteLine()
    {
        Write(Environment.NewLine);
    }

    /// <summary>
    /// Writes a string followed by a line break to the output, taking care of indentation.
    /// </summary>
    /// <param name="s">The string to be written.</param>
    protected void WriteLine(string s)
    {
        Write(s + Environment.NewLine);
    }

    /// <summary>
    /// Writes a formatted string followed by a line break to the output, taking care of indentation.
    /// </summary>
    /// <param name="format">The format string that describes the content to be written.</param>
    /// <param name="values">The objects to be formatted and written to the output.</param>
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