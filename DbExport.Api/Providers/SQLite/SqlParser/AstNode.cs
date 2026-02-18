using System.Collections.Generic;

namespace DbExport.Providers.SQLite.SqlParser;

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

public enum UnaryOperator
{
    NONE,
    PLUS,
    MINUS,
    NOT
}

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

public class AstNode
{
    public AstNode(AstNodeKind kind)
    {
        Kind = kind;
        Children = [];
    }

    public AstNode(AstNodeKind kind, object data)
    {
        Kind = kind;
        Data = data;
        Children = [];
    }

    public AstNode(AstNodeKind kind, object data, params AstNode[] children)
    {
        Kind = kind;
        Data = data;
        Children = [..children];
    }

    public AstNodeKind Kind { get; }
    
    public object Data { get; }
    
    public List<AstNode> Children { get; }
}