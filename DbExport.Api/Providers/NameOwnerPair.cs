namespace DbExport.Providers;

/// <summary>
/// Represents a pair of name and owner, where the owner is optional.
/// This can be used to represent database objects that have an associated owner,
/// such as tables or views, where the owner may not always be specified.
/// </summary>
/// <param name="Name">The name of the database object, such as a table or view.</param>
/// <param name="Owner">The owner of the database object, which is optional and defaults to an empty string if not provided.</param>
public record NameOwnerPair(string Name, string Owner = "");
