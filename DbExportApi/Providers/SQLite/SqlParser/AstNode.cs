using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbExport.Providers.SQLite.SqlParser
{
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
            ChildNodes = new List<AstNode>();
        }

        public AstNode(AstNodeKind kind, object data)
        {
            Kind = kind;
            Data = data;
            ChildNodes = new List<AstNode>();
        }

        public AstNode(AstNodeKind kind, object data, params AstNode[] childNodes)
        {
            Kind = kind;
            Data = data;
            ChildNodes = new List<AstNode>(childNodes);
        }

        public AstNodeKind Kind { get; private set; }
        public object Data { get; private set; }
        public List<AstNode> ChildNodes { get; private set; }
    }
}
