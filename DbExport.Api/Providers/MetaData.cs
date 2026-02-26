using System;
using System.Collections.Generic;

namespace DbExport.Providers;

/// <summary>
/// Represents a collection of metadata key-value pairs.
/// This class is used to store additional information about the database schema items such as tables, table columns and/or keys.
/// The keys are case-insensitive, allowing for flexible access to the metadata values.
/// </summary>
public class MetaData : Dictionary<string, object>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MetaData"/> class.
    /// </summary>
    public MetaData() : base(StringComparer.OrdinalIgnoreCase) { }
}
