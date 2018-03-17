using System;

namespace DbExport.Schema
{
    [Flags]
    public enum ColumnAttribute
    {
        None        = 0,
        Required    = 1,
        Computed    = 2,
        Identity    = 4,
        PKColumn    = 8,
        FKColumn    = 16,
        IXColumn    = 32,
        Numeric     = 64,
        Alphabetic  = 128,
        FixedLength = 256,
        Unsigned    = 512,
        Unicode     = 1024,
        Temporal    = 2048,
        Binary      = 4096
    }
}
