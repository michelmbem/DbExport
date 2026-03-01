namespace DbExport.Schema;

/// <summary>
/// Defines the actions to be taken when a foreign key constraint is violated
/// or when referenced data is modified or deleted.
/// </summary>
public enum ForeignKeyRule
{
    /// <summary>
    /// No action is taken.
    /// </summary>
    None,

    /// <summary>
    /// Specifies that the action is restricted, meaning the operation
    /// causing the foreign key constraint violation cannot proceed.
    /// </summary>
    Restrict,
    
    /// <summary>
    /// Specifies that the action is cascaded, meaning that the operation
    /// causing the foreign key constraint violation will be propagated to
    /// the referenced table.
    /// </summary>
    Cascade,
    
    /// <summary>
    /// Specifies that the action is set null, meaning that the foreign key
    /// column value will be set to NULL.
    /// </summary>
    SetNull,
    
    /// <summary>
    /// Specifies that the action is set default, meaning that the foreign key
    /// column value will be set to the default value of the referenced column.
    /// </summary>
    SetDefault
}