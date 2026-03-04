using System;

namespace DbExport.Providers;

/// <summary>
/// Represents a pair consisting of a name and an optional owner, commonly used to define
/// database objects such as tables or types along with their associated schema or owner information.
/// </summary>
public sealed class NameOwnerPair
{   
    /// <summary>
    /// Gets or sets the name of the object.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the owner of the object, or null if not applicable.
    /// </summary>
    public string Owner { get; set; }

    /// <summary>
    /// Deconstructs the current <see cref="NameOwnerPair"/> instance into separate name and owner values.
    /// </summary>
    /// <param name="name">Outputs the name value of the current pair.</param>
    /// <param name="owner">Outputs the owner value of the current pair.</param>
    public void Deconstruct(out string name, out string owner)
    {
        name = Name;
        owner = Owner;
    }

    /// <inheritdoc/>
    public override bool Equals(object obj) =>
        obj is NameOwnerPair other && Name == other.Name && Owner == other.Owner;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Name, Owner);

    /// <inheritdoc/>
    public override string ToString() => $"{Name}{(Owner != null ? $" ({Owner})" : "")}";
}
