namespace DbExport.Providers.SQLite.SqlParser;

/// <summary>
/// Represents the different types of tokens in the SQLite SQL parser.
/// </summary>
public enum TokenId
{
    LT_NUM,
    LT_CHAR,
    IDENT,
    KW_CREATE,
    KW_TABLE,
    KW_IF,
    KW_EXISTS,
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
    KW_WITHOUT,
    KW_ROWID,
    LPAREN,
    RPAREN,
    COMMA,
    SEMICOLON,
    DOT,
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

/// <summary>
/// Represents a token in the SQLite SQL parser.
/// </summary>
public class Token
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Token"/> class with the specified token ID.
    /// </summary>
    /// <param name="id">The ID of the token.</param>
    public Token(TokenId id)
    {
        Id = id;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Token"/> class with the specified token ID and data.
    /// </summary>
    /// <param name="id">The ID of the token.</param>
    /// <param name="data">The data associated with the token.</param>
    public Token(TokenId id, object data)
    {
        Id = id;
        Data = data;
    }

    /// <summary>
    /// Gets the ID of the token.
    /// </summary>
    public TokenId Id { get; }
    
    /// <summary>
    /// Gets the data associated with the token.
    /// </summary>
    public object Data { get; }
}