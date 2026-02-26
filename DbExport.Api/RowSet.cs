using System;
using System.Data.Common;

namespace DbExport;

public record RowSet(DbConnection Connection, DbCommand Command, DbDataReader DataReader) : IDisposable
{
    public void Dispose()
    {
        Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (!disposing) return;

        DataReader.Dispose();
        Command.Dispose();
        Connection.Dispose();
    }
}
