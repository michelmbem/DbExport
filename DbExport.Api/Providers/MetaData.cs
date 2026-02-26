using System;
using System.Collections.Generic;

namespace DbExport.Providers;

public class MetaData : Dictionary<string, object>
{
    public MetaData() : base(StringComparer.OrdinalIgnoreCase) { }
}
