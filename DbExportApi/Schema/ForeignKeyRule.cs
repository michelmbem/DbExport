using System;

namespace DbExport.Schema
{
    [Flags]
    public enum ForeignKeyRule
    {
        None        = 0,
        Restrict    = 1,
        Cascade     = 2,
        SetNull     = 4,
        SetDefault  = 8
    }
}
