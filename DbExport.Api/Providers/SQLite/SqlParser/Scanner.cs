using System;
using System.Text;

namespace DbExport.Providers.SQLite.SqlParser;

public class Scanner
{
    private string input;
    private int offset;

    public Scanner(string input)
    {
        this.input = input;
        offset = 0;
    }

    public Token NextToken()
    {
        while (offset < input.Length && char.IsWhiteSpace(input[offset]))
            ++offset;

        if (offset >= input.Length)
            return new Token(TokenId.EOL);

        var c = input[offset];
        switch (c)
        {
            case '(':
                ++offset;
                return new Token(TokenId.LPAREN);
            case ')':
                ++offset;
                return new Token(TokenId.RPAREN);
            case ',':
                ++offset;
                return new Token(TokenId.COMMA);
            case ';':
                ++offset;
                return new Token(TokenId.SEMICOLON);
            case '+':
                ++offset;
                return new Token(TokenId.PLUS);
            case '-':
                ++offset;
                return new Token(TokenId.MINUS);
            case '*':
                ++offset;
                return new Token(TokenId.TIMES);
            case '/':
                ++offset;
                return new Token(TokenId.DIV);
            case '%':
                ++offset;
                return new Token(TokenId.MOD);
            case '&':
                ++offset;
                return new Token(TokenId.KW_AND);
            case '|':
                if (offset < input.Length - 1)
                {
                    switch (input[offset + 1])
                    {
                        case '|':
                            offset += 2;
                            return new Token(TokenId.CONCAT);
                        default:
                            ++offset;
                            return new Token(TokenId.KW_OR);
                    }
                }

                ++offset;
                return new Token(TokenId.KW_OR);
            case '=':
                if (offset < input.Length - 1 && input[offset + 1] == '=')
                    offset += 2;
                else
                    ++offset;
                return new Token(TokenId.EQ);
            case '!':
                if (offset >= input.Length - 1 || input[offset + 1] != '=')
                    throw new FormatException("Unrecognized symbol " + c);
                offset += 2;
                return new Token(TokenId.NEQ);
            case '<':
                if (offset < input.Length - 1)
                {
                    switch (input[offset + 1])
                    {
                        case '=':
                            offset += 2;
                            return new Token(TokenId.LE);
                        case '<':
                            offset += 2;
                            return new Token(TokenId.SHL);
                        default:
                            ++offset;
                            return new Token(TokenId.LT);
                    }
                }

                ++offset;
                return new Token(TokenId.LT);
            case '>':
                if (offset < input.Length - 1)
                {
                    switch (input[offset + 1])
                    {
                        case '=':
                            offset += 2;
                            return new Token(TokenId.GE);
                        case '>':
                            offset += 2;
                            return new Token(TokenId.SHR);
                        default:
                            ++offset;
                            return new Token(TokenId.GT);
                    }
                }

                ++offset;
                return new Token(TokenId.GT);
            case >= '0' and <= '9':
                return LiteralNumber();
            case '\'':
                return LiteralChar();
            case '[':
                return SafeIdentifier();
            default:
                if (c == '_' || char.IsLetter(c)) return Identifier();
                throw new FormatException("Unexpected symbol " + c + " encountered at " + offset);
        }
    }

    private Token LiteralNumber()
    {
        var n = offset;
        for (; n < input.Length && char.IsDigit(input[n]); ++n);

        if (n < input.Length && input[n] == '.')
        {
            ++n;
            for (; n < input.Length && char.IsDigit(input[n]); ++n);
        }

        if (n < input.Length && (input[n] == 'e' || input[n] == 'E'))
        {
            ++n;
            if (n < input.Length && (input[n] == '+' || input[n] == '-'))
                ++n;
            for (; n < input.Length && char.IsDigit(input[n]); ++n);
        }

        var text = input.Substring(offset, n - offset);
        offset = n;
	
        return new Token(TokenId.LT_NUM, decimal.Parse(text));
    }

    private Token LiteralChar()
    {
        var sb = new StringBuilder();
        var loop = true;
        var n = offset + 1;

        while (loop && n < input.Length)
        {
            if (input[n] == '\'')
                if (n < input.Length - 1 && input[n + 1] == '\'')
                {
                    sb.Append('\'');
                    n += 2;
                }
                else
                    loop = false;
            else
            {
                sb.Append(input[n]);
                ++n;
            }
        }

        if (n >= input.Length)
            throw new FormatException("Unexpected end of line at " + n);

        offset = n + 1;

        return new Token(TokenId.LT_CHAR, sb.ToString());
    }

    private Token SafeIdentifier()
    {
        var n = offset + 1;
        for (; n < input.Length && input[n] != ']'; ++n) ;

        if (n >= input.Length)
            throw new FormatException("Unexpected end of line at " + n);

        var text = input.Substring(offset + 1, n - offset - 1);
        offset = n + 1;

        return new Token(TokenId.IDENT, text);
    }

    private Token Identifier()
    {
        var n = offset;
        for (; n < input.Length && (input[n] == '_' || char.IsLetterOrDigit(input[n])); ++n);

        var len = n - offset;
        var text = input.Substring(offset, len);
        offset = n;

        return text.ToUpper() switch
        {
            "CREATE" => new Token(TokenId.KW_CREATE),
            "TABLE" => new Token(TokenId.KW_TABLE),
            "PRIMARY" => new Token(TokenId.KW_PRIMARY),
            "KEY" => new Token(TokenId.KW_KEY),
            "CONSTRAINT" => new Token(TokenId.KW_CONSTRAINT),
            "FOREIGN" => new Token(TokenId.KW_FOREIGN),
            "ON" => new Token(TokenId.KW_ON),
            "UPDATE" => new Token(TokenId.KW_UPDATE),
            "DELETE" => new Token(TokenId.KW_DELETE),
            "CASCADE" => new Token(TokenId.KW_CASCADE),
            "NOT" => new Token(TokenId.KW_NOT),
            "NULL" => new Token(TokenId.KW_NULL),
            "UNIQUE" => new Token(TokenId.KW_UNIQUE),
            "DEFAULT" => new Token(TokenId.KW_DEFAULT),
            "REFERENCES" => new Token(TokenId.KW_REFERENCES),
            "AUTOINCREMENT" => new Token(TokenId.KW_AUTOINC),
            "CHECK" => new Token(TokenId.KW_CHECK),
            "LIKE" => new Token(TokenId.KW_LIKE),
            "IN" => new Token(TokenId.KW_IN),
            "GLOB" => new Token(TokenId.KW_GLOB),
            "AND" => new Token(TokenId.KW_AND),
            "OR" => new Token(TokenId.KW_OR),
            "BIT" or "TINYINT" or "SMALLINT" or "INT" or "INTEGER" or "BIGINT" or "FLOAT" or "DOUBLE" or "REAL"
                or "DECIMAL" or "NUMERIC" or "MONEY" or "CHAR" or "VARCHAR" or "TEXT" or "NCHAR" or "NVARCHAR"
                or "NTEXT" or "DATETIME" or "IMAGE" or "BLOB" or "GUID" => new Token(TokenId.TYPE, text),
            _ => new Token(TokenId.IDENT, text)
        };
    }
}