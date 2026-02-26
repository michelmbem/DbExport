using System;

namespace DbExport;

/// <summary>
/// Flags to specify what aspects of the database to export. These can be combined using bitwise operations.
/// </summary>
[Flags]
public enum ExportFlags
{
    ExportNothing       = 0,
    ExportPrimaryKeys   = 1,
    ExportForeignKeys   = 2,
    ExportIndexes       = 4,
    ExportDefaults      = 8,
    ExportIdentities    = 16
}

/// <summary>
/// Options for exporting a database, including what to export and any provider-specific settings.
/// </summary>
public class ExportOptions
{
    public bool ExportSchema { get; set; }

    public bool ExportData { get; set; }

    public ExportFlags Flags { get; set; }

    public dynamic ProviderSpecific { get; set; }
    
    public void SetFlag(ExportFlags flag, bool value) => Flags = value ? Flags | flag : Flags & ~flag;
    
    public bool HasFlag(ExportFlags flag) => Flags.HasFlag(flag);
}