using System;

namespace DbExport;

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

public class ExportOptions
{
    public bool ExportSchema { get; set; }

    public bool ExportData { get; set; }

    public ExportFlags Flags { get; set; }

    public dynamic ProviderSpecific { get; set; }
    
    public void SetFlag(ExportFlags flag, bool value) => Flags = value ? Flags | flag : Flags & ~flag;
    
    public bool HasFlag(ExportFlags flag) => Flags.HasFlag(flag);
}