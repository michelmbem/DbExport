using System;
using System.Globalization;
using System.Text;

namespace DbExport.Providers.SQLite.SqlParser;

/// <summary>
/// Represents a lexical scanner for parsing input strings into tokens.
/// This class is designed to process and tokenize input data for use
/// with higher-level parsers in the context of SQLite SQL parsing.
/// </summary>
public class Scanner(string input)
{
    /// <summary>
    /// Indicates the current position within the input string being processed by the scanner.
    /// </summary>
    /// <remarks>
    /// This variable is used as an index to track the character position during tokenization.
    /// It is incremented as characters are consumed and tokens are identified.
    /// </remarks>
    private int offset;

    /// <summary>
    /// Retrieves the next token from the input string while skipping any leading whitespace.
    /// Analyzes the input to identify and return the corresponding token for various symbols,
    /// operators, numeric literals, character literals, or identifiers.
    /// Throws a <see cref="LexicalException"/> for unsupported or invalid symbols.
    /// </summary>
    /// <returns>
    /// A <see cref="Token"/> object representing the next token in the input sequence.
    /// </returns>
    /// <exception cref="LexicalException">
    /// Thrown if an unrecognized or invalid symbol is encountered in the input.
    /// </exception>
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
            case '.':
                ++offset;
                return new Token(TokenId.DOT);
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
                    throw new LexicalException("Unrecognized symbol " + c);
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
            case '\'':
                return LiteralChar();
            case '[':
                return SafeIdentifier(']');
            case '`' or '"':
                return SafeIdentifier(c);
            case >= '0' and <= '9':
                return LiteralNumber();
            case '_':
            case {} when char.IsLetter(c):
                return Identifier();
            default:
                throw new LexicalException("Unexpected symbol " + c + " encountered at " + offset);
        }
    }

    /// <summary>
    /// Parses a numeric literal from the input string starting at the current offset position.
    /// Handles integer, floating-point, and scientific notation formats.
    /// Advances the offset to the position immediately following the parsed number.
    /// </summary>
    /// <returns>
    /// A <see cref="Token"/> object containing the parsed numeric literal and its value.
    /// </returns>
    /// <exception cref="FormatException">
    /// Thrown if the numeric literal has an invalid format and cannot be parsed into a decimal.
    /// </exception>
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
	
        return new Token(TokenId.LT_NUM, decimal.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Parses a character literal from the input string. Handles escaped single quotes
    /// by treating double single quotes ('') as a single embedded quote within the literal.
    /// Throws a <see cref="LexicalException"/> if the end of the input is reached
    /// without properly terminating the character literal.
    /// </summary>
    /// <returns>
    /// A <see cref="Token"/> object representing the parsed character literal. The token's
    /// value contains the extracted character data without the surrounding quotes.
    /// </returns>
    /// <exception cref="LexicalException">
    /// Thrown when the input string ends unexpectedly while parsing the character literal.
    /// </exception>
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
            throw new LexicalException("Unexpected end of line at " + n);

        offset = n + 1;

        return new Token(TokenId.LT_CHAR, sb.ToString());
    }

    /// <summary>
    /// Parses a safely quoted identifier from the input string, using a specified quote character
    /// to determine the bounds of the identifier. Handles square brackets, backticks, or double quotes
    /// as valid quoting characters.
    /// </summary>
    /// <param name="quote">
    /// The character used to enclose the identifier, such as '[', '`', or '"'.
    /// </param>
    /// <returns>
    /// A <see cref="Token"/> object representing the parsed identifier with its associated text.
    /// </returns>
    /// <exception cref="LexicalException">
    /// Thrown if the closing quote character is not found before the end of the input string.
    /// </exception>
    private Token SafeIdentifier(char quote)
    {
        var n = offset + 1;
        for (; n < input.Length && input[n] != quote; ++n) ;

        if (n >= input.Length)
            throw new LexicalException("Unexpected end of line at " + n);

        var text = input.Substring(offset + 1, n - offset - 1);
        offset = n + 1;

        return new Token(TokenId.IDENT, text);
    }

    /// <summary>
    /// Extracts the next identifier or keyword from the input string beginning at the current position.
    /// Identifies and returns a token for known SQL keywords, types, or an identifier if no match is found.
    /// The position in the input string is advanced beyond the recognized token.
    /// </summary>
    /// <returns>
    /// A <see cref="Token"/> object representing an identifier, keyword, or data type recognized from the input string.
    /// </returns>
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
            "IF" => new Token(TokenId.KW_IF),
            "EXISTS" => new Token(TokenId.KW_EXISTS),
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
            "WITHOUT" => new Token(TokenId.KW_WITHOUT),
            "ROWID" => new Token(TokenId.KW_ROWID),
            _ => new Token(TokenId.IDENT, text)
        };
    }
}