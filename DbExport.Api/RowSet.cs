using System;
using System.Data.Common;

namespace DbExport;

/// <summary>
/// A record that encapsulates a database connection, command, and data reader.
/// It implements IDisposable to ensure that all resources are properly released when the RowSet is disposed.
/// </summary>
/// <param name="Connection">The database connection associated with the RowSet.</param>
/// <param name="Command">The database command associated with the RowSet.</param>
/// <param name="DataReader">The data reader associated with the RowSet.</param>
public record RowSet(DbConnection Connection, DbCommand Command, DbDataReader DataReader) : IDisposable
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Dispose()
    {
        DataReader.Dispose();
        Command.Dispose();
        Connection.Dispose();
        GC.SuppressFinalize(this);
    }
}
