using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbExport.Providers.SQLite.SqlParser
{
    public enum TokenId
    {
        LT_NUM,
        LT_CHAR,
        IDENT,
        TYPE,
        KW_CREATE,
        KW_TABLE,
        KW_PRIMARY,
        KW_KEY,
        KW_CONSTRAINT,
        KW_FOREIGN,
        KW_ON,
        KW_UPDATE,
        KW_DELETE,
        KW_CASCADE,
        KW_NOT,
        KW_NULL,
        KW_UNIQUE,
        KW_DEFAULT,
        KW_REFERENCES,
        KW_AUTOINC,
        KW_CHECK,
        KW_IN,
        KW_LIKE,
        KW_AND,
        KW_OR,
        KW_GLOB,
        LPAREN,
        RPAREN,
        COMMA,
        SEMICOLON,
        EQ,
        NEQ,
        LT,
        LE,
        GT,
        GE,
        PLUS,
        MINUS,
        TIMES,
        DIV,
        MOD,
        SHL,
        SHR,
        CONCAT,
        EOL
    }

    public class Token
    {
        public Token(TokenId id)
        {
            Id = id;
        }

        public Token(TokenId id, object data)
        {
            Id = id;
            Data = data;
        }

        public TokenId Id { get; private set; }
        public object Data { get; private set; }
    }
}
