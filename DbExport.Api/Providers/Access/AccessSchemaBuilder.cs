using System;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using DbExport.Schema;

namespace DbExport.Providers.Access;

public class AccessSchemaBuilder(string connectionString) : IVisitor
{
    private readonly StringBuilder queryBuilder = new();
    private ADODB.Connection connection;

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

    private static bool IsSelected(Index index) =>
        index.IsChecked && !index.MatchesKey && index.Columns.Count > 0 && index.AllColumnsAreChecked;

    private static bool IsSelected(ForeignKey fk) =>
        fk.IsChecked && fk.AllColumnsAreChecked && fk.RelatedTable is { IsChecked: true };

    private static string Escape(string name) => Utility.Escape(name, ProviderNames.ACCESS);

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

    private static string GetTypeName(Column column, ExportOptions options)
    {
        if (column.ColumnType == ColumnType.UserDefined)
        {
            var dataType = column.DataType;
            if (dataType != null) return GetTypeName(dataType);
        }
        
        return options?.HasFlag(ExportFlags.ExportIdentities) == true && column.IsIdentity
             ? "counter"
             : GetTypeName((IDataItem)column);
    }

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

    private static string Format(object value, ColumnType columnType)
    {
        if (value == null || value == DBNull.Value) return "NULL";

        switch (columnType)
        {
            case ColumnType.Boolean:
            case { } when Utility.IsBoolean(value):
                return Convert.ToBoolean(value) ? "1" : "0";
            case ColumnType.Char or ColumnType.NChar or ColumnType.VarChar or ColumnType.NVarChar or
                 ColumnType.Text or ColumnType.NText or ColumnType.Xml or ColumnType.Json or
                 ColumnType.Guid or ColumnType.Geometry:
                return Utility.QuotedStr(value);
            case ColumnType.DateTime:
            case ColumnType.RowVersion when value is DateTime:
                return $"#{value:yyyy-MM-dd HH:mm:ss}#";
            case ColumnType.Date:
                return $"#{value:yyyy-MM-dd}#";
            case ColumnType.Time when value is TimeSpan:
                return $"#{value:c}#";
            case ColumnType.Time:
                return $"#{value:HH:mm:ss}#";
            case ColumnType.Bit or ColumnType.Blob:
            case ColumnType.RowVersion when value is byte[]:
            {
                var bytes = (byte[]) value;
                if (bytes.Length == 0) return "''";
                return "0x" + Utility.BinToHex(bytes);
            }
            default:
                return Convert.ToString(value, CultureInfo.InvariantCulture);
        }
    }

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
        connection.Execute(queryBuilder.ToString(), out _);
        queryBuilder.Clear();
    }

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
