namespace DbExport;

/// <summary>
/// Defines an interface for accepting visitors, allowing external operations to be performed on implementing classes without modifying their structure.
/// This is a key component of the Visitor design pattern, enabling separation of concerns and enhancing flexibility in extending functionality.
/// </summary>
public interface IVisitorAcceptor
{
    /// <summary>
    /// Accepts a visitor, allowing it to perform operations on the implementing class.
    /// The visitor will typically have specific methods for handling different types of elements in the object structure.
    /// </summary>
    /// <param name="visitor">The visitor instance that will perform operations on the implementing class.</param>
    void AcceptVisitor(IVisitor visitor);
}