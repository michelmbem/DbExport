namespace DbExport.Schema;

/// <summary>
/// Represents an item that can be checked or unchecked.
/// </summary>
public interface ICheckable
{
    /// <summary>
    /// Gets or sets a value indicating whether the item is checked.
    /// </summary>
    bool IsChecked { get; set; }
}