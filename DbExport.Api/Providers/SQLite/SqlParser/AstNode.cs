using System.Collections.Generic;

namespace DbExport.Providers.SQLite.SqlParser;

/// <summary>
/// Represents the kinds of Abstract Syntax Tree (AST) nodes
/// used in parsing and analyzing SQLite SQL statements.
/// </summary>
public enum AstNodeKind
{
    CREATE_TBL,
    COLSPECLIST,
    COLSPEC,
    PKSPEC,
    FKSPEC,
    UKSPEC,
    CHKSPEC,
    UNEXPR,
    BINEXPR,
    FNCALL,
    COLREF,
    COLREFLIST,
    LTNUM,
    LTCHAR,
    EXPLIST
}

/// <summary>
/// Represents unary operators used in parsing and analyzing SQLite SQL expressions.
/// These operators are applied to a single operand to produce a result.
/// </summary>
public enum UnaryOperator
{
    NONE,
    PLUS,
    MINUS,
    NOT
}

/// <summary>
/// Represents binary operators used in expressions within the SQLite SQL parser.
/// </summary>
public enum BinaryOperator
{
    NONE,
    CONCAT,
    ADD,
    SUB,
    MUL,
    DIV,
    MOD,
    EQ,
    NEQ,
    LT,
    LE,
    GT,
    GE,
    IN,
    LIKE,
    AND,
    OR
}

/// <summary>
/// Represents a node in an Abstract Syntax Tree (AST) structure used for parsing SQLite SQL statements.
/// Each node corresponds to a specific syntactic or semantic element of a SQL expression or statement,
/// including tables, columns, constraints, expressions, and operators.
/// </summary>
public class AstNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AstNode"/> class with the specified kind.
    /// </summary>
    /// <param name="kind">The kind of AST node.</param>
    public AstNode(AstNodeKind kind)
    {
        Kind = kind;
        Children = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AstNode"/> class with the specified kind and data.
    /// </summary>
    /// <param name="kind">The kind of AST node.</param>
    /// <param name="data">The data associated with the node.</param>
    public AstNode(AstNodeKind kind, object data)
    {
        Kind = kind;
        Data = data;
        Children = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AstNode"/> class with the specified kind, data, and children.
    /// </summary>
    /// <param name="kind">The kind of AST node.</param>
    /// <param name="data">The data associated with the node.</param>
    /// <param name="children">The children of the node.</param>
    public AstNode(AstNodeKind kind, object data, params AstNode[] children)
    {
        Kind = kind;
        Data = data;
        Children = [..children];
    }

    /// <summary>
    /// Gets the kind of AST node.
    /// </summary>
    public AstNodeKind Kind { get; }
    
    /// <summary>
    /// Gets the data associated with the node.
    /// </summary>
    public object Data { get; }
    
    /// <summary>
    /// Gets the children of the node.
    /// </summary>
    public List<AstNode> Children { get; }
}