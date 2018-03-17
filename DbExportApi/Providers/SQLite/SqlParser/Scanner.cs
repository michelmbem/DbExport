using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbExport.Providers.SQLite.SqlParser
{
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
                    else
                    {
                        ++offset;
                        return new Token(TokenId.KW_OR);
                    }
                case '=':
                    if (offset < input.Length - 1 && input[offset + 1] == '=')
                        offset += 2;
                    else
                        ++offset;
                    return new Token(TokenId.EQ);
                case '!':
                    if (offset < input.Length - 1 && input[offset + 1] == '=')
                    {
                        offset += 2;
                        return new Token(TokenId.NEQ);
                    }
                    throw new ApplicationException("Unrecognized symbol " + c);
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
                    else
                    {
                        ++offset;
                        return new Token(TokenId.LT);
                    }
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
                    else
                    {
                        ++offset;
                        return new Token(TokenId.GT);
                    }
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return LiteralNumber();
                case '\'':
                    return LiteralChar();
                case '[':
                    return SafeIdentifier();
                default:
                    if (c == '_' || char.IsLetter(c)) return Identifier();
                    throw new ApplicationException("Unexpected symbol " + c + " encountered at " + offset);
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
                throw new ApplicationException("Unexpected end of line at " + n);

            offset = n + 1;

            return new Token(TokenId.LT_CHAR, sb.ToString());
        }

        private Token SafeIdentifier()
        {
            var n = offset + 1;
            for (; n < input.Length && input[n] != ']'; ++n) ;

            if (n >= input.Length)
                throw new ApplicationException("Unexpected end of line at " + n);

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

            switch (text.ToUpper())
            {
                case "CREATE":
                    return new Token(TokenId.KW_CREATE);
                case "TABLE":
                    return new Token(TokenId.KW_TABLE);
                case "PRIMARY":
                    return new Token(TokenId.KW_PRIMARY);
                case "KEY":
                    return new Token(TokenId.KW_KEY);
                case "CONSTRAINT":
                    return new Token(TokenId.KW_CONSTRAINT);
                case "FOREIGN":
                    return new Token(TokenId.KW_FOREIGN);
                case "ON":
                    return new Token(TokenId.KW_ON);
                case "UPDATE":
                    return new Token(TokenId.KW_UPDATE);
                case "DELETE":
                    return new Token(TokenId.KW_DELETE);
                case "CASCADE":
                    return new Token(TokenId.KW_CASCADE);
                case "NOT":
                    return new Token(TokenId.KW_NOT);
                case "NULL":
                    return new Token(TokenId.KW_NULL);
                case "UNIQUE":
                    return new Token(TokenId.KW_UNIQUE);
                case "DEFAULT":
                    return new Token(TokenId.KW_DEFAULT);
                case "REFERENCES":
                    return new Token(TokenId.KW_REFERENCES);
                case "AUTOINCREMENT":
                    return new Token(TokenId.KW_AUTOINC);
                case "CHECK":
                    return new Token(TokenId.KW_CHECK);
                case "LIKE":
                    return new Token(TokenId.KW_LIKE);
                case "IN":
                    return new Token(TokenId.KW_IN);
                case "GLOB":
                    return new Token(TokenId.KW_GLOB);
                case "AND":
                    return new Token(TokenId.KW_AND);
                case "OR":
                    return new Token(TokenId.KW_OR);
                case "BIT":
                case "TINYINT":
                case "SMALLINT":
                case "INT":
                case "INTEGER":
                case "BIGINT":
                case "FLOAT":
                case "DOUBLE":
                case "REAL":
                case "DECIMAL":
                case "NUMERIC":
                case "MONEY":
                case "CHAR":
                case "VARCHAR":
                case "TEXT":
                case "NCHAR":
                case "NVARCHAR":
                case "NTEXT":
                case "DATETIME":
                case "IMAGE":
                case "BLOB":
                case "GUID":
                    return new Token(TokenId.TYPE, text);
                default:
                    return new Token(TokenId.IDENT, text);
            }
        }
    }
}
