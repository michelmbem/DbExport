using System;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using DbExport.Schema;

namespace DbExport.Providers.Access;

/// <summary>
/// Responsible for building and exporting database schema and data for Access databases.
/// Implements the <see cref="DbExport.IVisitor"/> interface to support the visiting pattern for database schema elements.
/// Handles schema creation, data export, foreign keys, indexes, identities, primary keys, and more based on configuration.
/// </summary>
public class AccessSchemaBuilder(string connectionString) : IVisitor
{
    /// <summary>
    /// A <see cref="StringBuilder"/> instance used to construct and store
    /// SQL queries dynamically during the schema and data export process.
    /// It accumulates query text and provides methods for appending or formatting
    /// SQL statements. The constructed queries are executed against the database
    /// and then cleared after execution to allow for reuse.
    /// </summary>
    private readonly StringBuilder queryBuilder = new();

    /// <summary>
    /// Represents an instance of the <see cref="ADODB.Connection"/> class used to establish
    /// and manage the connection to an Access database. This connection serves as the
    /// primary communication channel between the application and the database, enabling
    /// operations such as schema creation, data exporting, and executing SQL commands.
    /// </summary>
    private ADODB.Connection connection;

    /// <summary>
    /// Gets or sets the export options that control the behavior of the schema and data export process.
    /// </summary>
    public ExportOptions ExportOptions { get; set; }

    #region IVisitor Members

    public void VisitDatabase(Database database)
    {
        var visitSchema = ExportOptions?.ExportSchema == true;
        var visitData = ExportOptions?.ExportData == true;
        var visitFKs = ExportOptions?.HasFlag(ExportFlags.ExportForeignKeys) == true;
        var visitIdent = ExportOptions?.HasFlag(ExportFlags.ExportIdentities) == true;

        var catalog = new ADOX.Catalog();

        if (visitSchema)
        {
            connection = (ADODB.Connection)catalog.Create(connectionString);

            foreach (Table table in database.Tables.Where(t => t.IsChecked))
                table.AcceptVisitor(this);
        }
        else
        {
            connection = new ADODB.Connection { ConnectionString = connectionString };
            connection.Open();
            catalog.ActiveConnection = connection;
        }

        if (visitData)
            foreach (Table table in database.Tables.Where(t => t.IsChecked))
            {
                using var rowSet = SqlHelper.OpenTable(table, visitIdent, false);

                while (rowSet.DataReader.Read())
                    ImportRecord(table, rowSet.DataReader);
            }

        if (!visitSchema || !visitFKs) return;

        foreach (var fk in database.Tables.Where(t => t.IsChecked)
            .SelectMany(t => t.ForeignKeys.Where(IsSelected)))
        {
            fk.AcceptVisitor(this);
        }
    }

    public void VisitTable(Table table)
    {
        var visitPKs = ExportOptions?.HasFlag(ExportFlags.ExportPrimaryKeys) == true;
        var visitIndexes = ExportOptions?.HasFlag(ExportFlags.ExportIndexes) == true;

        Write("CREATE TABLE {0} (", Escape(table.Name));

        bool comma = false;

        foreach (var column in table.Columns.Where(c => c.IsChecked))
        {
            if (comma) Write(",");
            column.AcceptVisitor(this);
            comma = true;
        }

        if (visitPKs && table.HasPrimaryKey && table.PrimaryKey.AllColumnsAreChecked)
        {
            Write(",");
            table.PrimaryKey.AcceptVisitor(this);
        }

        Write(")");
        ExecuteQuery();

        if (!visitIndexes) return;

        foreach (Index index in table.Indexes.Where(IsSelected))
        {
            index.AcceptVisitor(this);
        }
    }

    public void VisitColumn(Column column)
    {
        var visitDefaults = ExportOptions?.HasFlag(ExportFlags.ExportDefaults) == true;

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
        if (index.IsUnique) Write(" UNIQUE");
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

    /// <summary>
    /// Determines whether the specified index is selected for processing based on its properties.
    /// </summary>
    /// <param name="index">The index to evaluate.</param>
    /// <returns>
    /// A boolean value indicating whether the index is selected. Returns true if the index is checked,
    /// does not match any primary or foreign key, has at least one column, and all its columns are checked;
    /// otherwise, false.
    /// </returns>
    private static bool IsSelected(Index index) =>
        index.IsChecked && !index.MatchesKey && index.Columns.Count > 0 && index.AllColumnsAreChecked;

    /// <summary>
    /// Determines whether the specified foreign key is selected for processing based on its properties.
    /// </summary>
    /// <param name="fk">The foreign key to evaluate.</param>
    /// <returns>
    /// A boolean value indicating whether the foreign key is selected. Returns true if the foreign key is checked,
    /// all its columns are checked, and its related table is checked; otherwise, false.
    /// </returns>
    private static bool IsSelected(ForeignKey fk) =>
        fk.IsChecked && fk.AllColumnsAreChecked && fk.RelatedTable is { IsChecked: true };

    /// <summary>
    /// Escapes a provided object name to ensure proper formatting and prevent syntax conflicts in the context of the Access database provider.
    /// </summary>
    /// <param name="name">The object name to escape, such as a table or column name.</param>
    /// <returns>
    /// A string representing the escaped object name, enclosed in brackets ([ ]) for compatibility with the Access database provider.
    /// </returns>
    private static string Escape(string name) => Utility.Escape(name, ProviderNames.ACCESS);

    /// <summary>
    /// Generates the formatted name for a database key (index or foreign key) by appending a suffix
    /// based on its type and position in the table's list of keys.
    /// </summary>
    /// <param name="key">The key for which to generate the name. This can be an instance of the Index or ForeignKey class.</param>
    /// <returns>
    /// A string representing the escaped and formatted name of the key, including a unique suffix
    /// (_IX for indexes and _FK for foreign keys), or the original escaped name if the key type is unrecognized.
    /// </returns>
    private static string GetKeyName(Key key)
    {
        switch (key)
        {
            case Index ix:
            {
                var index = key.Table.Indexes.IndexOf(ix);
                return Escape(key.Table.Name + "_IX" + (index + 1));
            }
            case ForeignKey fk:
            {
                var index = key.Table.ForeignKeys.IndexOf(fk);
                return Escape(key.Table.Name + "_FK" + (index + 1));
            }
            default:
                return Escape(key.Name);
        }
    }

    /// <summary>
    /// Retrieves the appropriate type name for the given database column based on its properties and specified export options.
    /// </summary>
    /// <param name="column">The database column for which the type name is to be determined.</param>
    /// <param name="options">The export options that influence the type name generation, such as identity flag and other related settings.</param>
    /// <returns>
    /// A string representing the type name for the specified database column. For user-defined column types, the user-defined data type name is returned.
    /// For identity columns with export options enabling identity flag, "counter" is returned; otherwise, the general type name based on the column's configuration is returned.
    /// </returns>
    private static string GetTypeName(Column column, ExportOptions options)
    {
        if (column.ColumnType == ColumnType.UserDefined)
        {
            var dataType = column.DataType;
            if (dataType != null) return GetTypeName(dataType);
        }
        
        return options?.HasFlag(ExportFlags.ExportIdentities) == true && column.IsIdentity
             ? "counter"
             : GetTypeName(column);
    }

    /// <summary>
    /// Retrieves the appropriate database type name for the specified data item based on its properties.
    /// </summary>
    /// <param name="item">The data item whose database type name is to be determined.</param>
    /// <returns>
    /// A string representing the database type name. The result depends on the column type, size, precision, scale,
    /// and other properties of the item.
    /// </returns>
    private static string GetTypeName(IDataItem item) =>
        item.ColumnType switch
        {
            ColumnType.Boolean => "bit",
            ColumnType.UnsignedTinyInt => "byte",
            ColumnType.TinyInt or ColumnType.SmallInt => "integer",
            ColumnType.UnsignedSmallInt or ColumnType.Integer => "long",
            ColumnType.UnsignedInt => "decimal(10)",
            ColumnType.BigInt or ColumnType.UnsignedBigInt => "decimal(20)",
            ColumnType.SinglePrecision => "single",
            ColumnType.DoublePrecision or ColumnType.Interval => "double",
            ColumnType.Currency => "currency",
            ColumnType.Decimal when item.Precision == 0 => "decimal",
            ColumnType.Decimal when item.Precision > 28 => $"decimal(28, {item.Scale})",
            ColumnType.Decimal when item.Scale == 0 => $"decimal({item.Precision})",
            ColumnType.Decimal => $"decimal({item.Precision}, {item.Scale})",
            ColumnType.Date or ColumnType.Time or ColumnType.DateTime => "datetime",
            ColumnType.Char or ColumnType.NChar or ColumnType.VarChar or ColumnType.NVarChar =>
                0 < item.Size && item.Size <= 255 ? $"text({item.Size})" : "text",
            ColumnType.Text or ColumnType.NText or ColumnType.Xml or ColumnType.Json or ColumnType.Geometry => "text",
            ColumnType.Bit or ColumnType.Blob or ColumnType.RowVersion => "oleobject",
            ColumnType.Guid => "uniqueidentifier",
            _ => item.NativeType,
        };

    /// <summary>
    /// Formats a given value based on its specified column type to a string representation compatible with Access database formats.
    /// </summary>
    /// <param name="value">The value to be formatted. It can be any object, including null or DBNull.</param>
    /// <param name="columnType">The data type of the column which defines how the value should be formatted.</param>
    /// <returns>
    /// A string representation of the value formatted according to the specified column type.
    /// For example, boolean values are formatted as "1" or "0", date and time values are formatted in Access-compatible patterns,
    /// and binary objects are converted to a hexadecimal string.
    /// </returns>
    private static string Format(object value, ColumnType columnType)
    {
        if (value == null || value == DBNull.Value) return "NULL";

        return columnType switch
        {
            ColumnType.Boolean => Convert.ToBoolean(value) ? "1" : "0",
            { } when Utility.IsBoolean(value) => Convert.ToBoolean(value) ? "1" : "0",
            ColumnType.Char or ColumnType.NChar or ColumnType.VarChar or ColumnType.NVarChar or
                ColumnType.Text or ColumnType.NText or ColumnType.Xml or ColumnType.Json or
                ColumnType.Guid or ColumnType.Geometry => Utility.QuotedStr(value),
            ColumnType.DateTime => $"#{value:yyyy-MM-dd HH:mm:ss}#",
            ColumnType.RowVersion when value is DateTime => $"#{value:yyyy-MM-dd HH:mm:ss}#",
            ColumnType.Date => $"#{value:yyyy-MM-dd}#",
            ColumnType.Time when value is TimeSpan => $"#{value:c}#",
            ColumnType.Time => $"#{value:HH:mm:ss}#",
            ColumnType.Bit or ColumnType.Blob => ByteArrayToHexString(value),
            ColumnType.RowVersion when value is byte[] => ByteArrayToHexString(value),
            _ => Convert.ToString(value, CultureInfo.InvariantCulture)
        };
    }

    /// <summary>
    /// Converts a byte array to its hexadecimal string representation.
    /// </summary>
    /// <param name="value">The object containing the byte array to be converted.</param>
    /// <returns>
    /// A string representing the byte array in hexadecimal format.
    /// Returns "''" if the array is empty, otherwise returns the string prefixed with "0x".
    /// </returns>
    private static string ByteArrayToHexString(object value)
    {
        var bytes = (byte[])value;
        if (bytes.Length == 0) return "''";
        return "0x" + Utility.BinToHex(bytes);
    }

    /// <summary>
    /// Converts a specified <see cref="ForeignKeyRule"/> value into its corresponding SQL rule text representation.
    /// </summary>
    /// <param name="rule">The <see cref="ForeignKeyRule"/> to be converted. This represents the behavior applied to
    /// foreign key constraints during an update or delete operation.</param>
    /// <returns>
    /// A string representing the SQL rule text corresponding to the provided <see cref="ForeignKeyRule"/>.
    /// Returns "CASCADE" for <see cref="ForeignKeyRule.Cascade"/>, "SET DEFAULT" for <see cref="ForeignKeyRule.SetDefault"/>,
    /// "SET NULL" for <see cref="ForeignKeyRule.SetNull"/>, and an empty string for <see cref="ForeignKeyRule.None"/> or
    /// unspecified values.
    /// </returns>
    private static string GetRuleText(ForeignKeyRule rule)
    {
        return rule switch
        {
            ForeignKeyRule.Cascade => "CASCADE",
            ForeignKeyRule.SetDefault => "SET DEFAULT",
            ForeignKeyRule.SetNull => "SET NULL",
            _ => string.Empty,
        };
    }

    /// <summary>
    /// Appends the specified string to the internal query builder.
    /// </summary>
    /// <param name="s">The string to append to the query.</param>
    private void Write(string s)
    {
        queryBuilder.Append(s);
    }

    /// <summary>
    /// Appends formatted text to the internal query builder.
    /// </summary>
    /// <param name="format">The composite format string.</param>
    /// <param name="values">An array of objects to format and include in the output text.</param>
    private void Write(string format, params object[] values)
    {
        queryBuilder.AppendFormat(format, values);
    }

    /// <summary>
    /// Executes the SQL query constructed in the internal query builder and resets the query builder for further use.
    /// </summary>
    /// <remarks>
    /// The method sends the current SQL command stored in the query builder to the database for execution
    /// using the established connection. After execution, the query builder is cleared to prepare for the next operation.
    /// </remarks>
    private void ExecuteQuery()
    {
        connection.Execute(queryBuilder.ToString(), out _);
        queryBuilder.Clear();
    }

    /// <summary>
    /// Constructs and executes an SQL INSERT statement to import a record into the specified table
    /// using data read from the provided data reader.
    /// </summary>
    /// <param name="table">The table into which the record will be imported. Contains metadata such as column definitions and table name.</param>
    /// <param name="dr">A data reader that provides access to the source record data. Column values in the reader are used to populate the INSERT statement.</param>
    private void ImportRecord(Table table, DbDataReader dr)
    {
        var skipIdentity = ExportOptions?.HasFlag(ExportFlags.ExportIdentities) == true;
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
            
            var columnType = column.ColumnType == ColumnType.UserDefined
                ? column.DataType?.ColumnType ?? ColumnType.VarChar
                : column.ColumnType;
            
            if (comma) Write(", ");
            Write(Format(dr[column.Name], columnType));
            comma = true;
        }

        Write(")");
        ExecuteQuery();
    }

    #endregion
}
