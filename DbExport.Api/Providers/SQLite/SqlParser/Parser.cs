using System;
using System.Collections.Generic;
using DbExport.Schema;

namespace DbExport.Providers.SQLite.SqlParser;

/// <summary>
/// The Parser class is responsible for parsing SQL statements into an abstract syntax tree (AST).
/// It serves as a foundational component for processing SQL schemas specific to SQLite.
/// </summary>
public class Parser
{
    /// <summary>
    /// Represents the lexical scanner used by the <see cref="Parser"/> class
    /// to tokenize input data during SQLite SQL schema parsing.
    /// This scanner reads the input string and provides the next token in the sequence.
    /// </summary>
    private readonly Scanner scanner;
    
    /// <summary>
    /// Represents the current token being processed by the parser.
    /// This token is obtained from the scanner and is used to identify and match
    /// specific tokens in the SQL statements being parsed.
    /// </summary>
    private Token token;

    /// <summary>
    /// Initializes a new instance of the <see cref="Parser"/> class.
    /// </summary>
    /// <param name="scanner">The scanner used to read the input string and provide tokens.</param>
    public Parser(Scanner scanner)
    {
        this.scanner = scanner;
        Skip();
    }
    
    #region DDL parsing

    /// <summary>
    /// Parses a SQL "CREATE TABLE" statement and constructs the corresponding abstract syntax tree (AST) node.
    /// </summary>
    /// <returns>An <see cref="AstNode"/> representing the structure of the "CREATE TABLE" statement,
    /// including its table name, column specifications, and any associated constraints or primary keys.</returns>
    public AstNode CreateTable()
    {
        Match(TokenId.KW_CREATE);
        Match(TokenId.KW_TABLE);
        var tableName = Match(TokenId.IDENT).Data.ToString();
        Match(TokenId.LPAREN);
        var colSpecList = ColumnSpecList();

        List<AstNode> children = [colSpecList];
        var loop = true;

        while (loop)
        {
            AstNode child;
            switch (token.Id)
            {
                case TokenId.KW_PRIMARY:
                    child = PrimaryKeySpec();
                    children.Add(child);
                    if (token.Id == TokenId.COMMA) Skip();
                    break;
                case TokenId.KW_CONSTRAINT:
                    child = ConstraintSpec();
                    children.Add(child);
                    if (token.Id == TokenId.COMMA) Skip();
                    break;
                default:
                    loop = false;
                    break;
            }
        }

        Match(TokenId.RPAREN);

        return new AstNode(AstNodeKind.CREATE_TBL, tableName, [..children]);
    }

    /// <summary>
    /// Parses a list of column specifications from a CREATE TABLE statement in SQL.
    /// Each column specification represented in the parsed output may include details about
    /// column names, types, constraints, and other column-specific attributes.
    /// </summary>
    /// <returns>An <see cref="AstNode"/> of kind <see cref="AstNodeKind.COLSPECLIST"/> containing all parsed column specifications as children.</returns>
    private AstNode ColumnSpecList()
    {
        List<AstNode> colSpecs = [];
        var colSpec = ColumnSpec();

        while (colSpec != null)
        {
            colSpecs.Add(colSpec);
            colSpec = TryMatch(TokenId.COMMA, out _) ? ColumnSpec() : null;
        }

        return new AstNode(AstNodeKind.COLSPECLIST, null, colSpecs.ToArray());
    }

    /// <summary>
    /// Parses a column specification from the input and returns an abstract syntax tree (AST) node
    /// representing the column definition in the SQL statement.
    /// </summary>
    /// <returns>An <see cref="AstNode"/> representing the column specification,
    /// or null if no valid column specification is found.</returns>
    private AstNode ColumnSpec()
    {
        if (!TryMatch(TokenId.IDENT, out var tok)) return null;

        var colName = tok.Data.ToString();
        var typeName = string.Empty;
        var (prec, scale) = (0, 0);

        if (TryMatch(TokenId.TYPE, out tok))
        {
            typeName = tok.Data.ToString();

            if (TryMatch(TokenId.LPAREN, out _))
            {
                prec = Convert.ToInt32(Match(TokenId.LT_NUM).Data);
                
                if (TryMatch(TokenId.COMMA, out _))
                    scale = Convert.ToInt32(Match(TokenId.LT_NUM).Data);
                
                if (TryMatch(TokenId.COMMA, out _))
                    Match(TokenId.LT_NUM); // Appearently, SQLite allows that syntax

                Match(TokenId.RPAREN);
            }
        }

        var loop = true;
        var allowDbNull = true;
        var unique = false;
        var pk = false;
        var autoinc = false;
        object defaultValue = null;

        while (loop)
        {
            switch (token.Id)
            {
                case TokenId.KW_NOT:
                    Skip();
                    Match(TokenId.KW_NULL);
                    allowDbNull = false;
                    break;
                case TokenId.KW_NULL:
                    Skip();
                    break;
                case TokenId.KW_UNIQUE:
                    Skip();
                    unique = true;
                    break;
                case TokenId.KW_PRIMARY:
                    Skip();
                    Match(TokenId.KW_KEY);
                    pk = true;
                    if (TryMatch(TokenId.KW_AUTOINC, out _))
                        autoinc = true;
                    break;
                case TokenId.KW_DEFAULT:
                    Skip();
                    if (!TryMatch(TokenId.LT_NUM, out tok))
                        tok = Match(TokenId.LT_CHAR);
                    defaultValue = tok.Data;
                    break;
                default:
                    loop = false;
                    break;
            }
        }

        MetaData attributes = new()
        {
            ["COLUMN_NAME"] = colName,
            ["TYPE_NAME"] = typeName,
            ["PRECISION"] = prec,
            ["SCALE"] = scale,
            ["ALLOW_DBNULL"] = allowDbNull,
            ["UNIQUE"] = unique,
            ["PRIMARY_KEY"] = pk,
            ["AUTO_INCREMENT"] = autoinc,
            ["DEFAULT_VALUE"] = defaultValue
        };

        return new AstNode(AstNodeKind.COLSPEC, attributes);

    }

    /// <summary>
    /// Parses and generates an abstract syntax tree (AST) node representing a primary key specification
    /// within a SQL CREATE TABLE statement.
    /// </summary>
    /// <returns>An <see cref="AstNode"/> of type <c>AstNodeKind.PKSPEC</c>, containing metadata and
    /// child nodes for the primary key column references.</returns>
    private AstNode PrimaryKeySpec()
    {
        Match(TokenId.KW_PRIMARY);
        Match(TokenId.KW_KEY);
        Match(TokenId.LPAREN);
        var pkColumnList = ColumnRefList();
        Match(TokenId.RPAREN);

        return new AstNode(AstNodeKind.PKSPEC, null, pkColumnList);
    }

    /// <summary>
    /// Parses and returns an abstract syntax tree (AST) node representing a constraint specification in the SQL statement.
    /// </summary>
    /// <returns>An <see cref="AstNode"/> representing the parsed constraint specification.</returns>
    /// <exception cref="SyntaxException">
    /// Thrown when the syntax of the constraint specification is invalid or unsupported.
    /// </exception>
    private AstNode ConstraintSpec()
    {
        Match(TokenId.KW_CONSTRAINT);
        var constraintName = Match(TokenId.IDENT).Data.ToString();

        return token.Id switch
        {
            TokenId.KW_UNIQUE => UniqueKeySpec(constraintName),
            TokenId.KW_FOREIGN => ForeignKeySpec(constraintName),
            TokenId.KW_CHECK => CheckSpec(constraintName),
            _ => throw new SyntaxException($"Invalid constraint specification '{token.Id}'")
        };
    }

    /// <summary>
    /// Creates a unique key specification node in the abstract syntax tree (AST),
    /// representing a UNIQUE constraint on one or more columns in a SQL table.
    /// </summary>
    /// <param name="ukName">The name of the UNIQUE constraint being defined.</param>
    /// <returns>
    /// An <see cref="AstNode"/> representing the unique key specification,
    /// which includes the constraint name and a list of column references.
    /// </returns>
    private AstNode UniqueKeySpec(string ukName)
    {
        Match(TokenId.KW_UNIQUE);
        Match(TokenId.LPAREN);
        var colNames = ColumnRefList();
        Match(TokenId.RPAREN);

        return new AstNode(AstNodeKind.UKSPEC, ukName, colNames);
    }

    /// <summary>
    /// Creates and returns a specification for a foreign key constraint.
    /// </summary>
    /// <param name="fkName">The name of the foreign key constraint.</param>
    /// <returns>An <see cref="AstNode"/> that represents the foreign key constraint specification.</returns>
    private AstNode ForeignKeySpec(string fkName)
    {
        Match(TokenId.KW_FOREIGN);
        Match(TokenId.KW_KEY);
        Match(TokenId.LPAREN);
        var fkColNames = ColumnRefList();
        Match(TokenId.RPAREN);
        Match(TokenId.KW_REFERENCES);
        var targetTableName = Match(TokenId.IDENT).Data.ToString();
        Match(TokenId.LPAREN);
        var pkColNames = ColumnRefList();
        Match(TokenId.RPAREN);

        var updateRule = ForeignKeyRule.None;
        var deleteRule = ForeignKeyRule.None;

        while (TryMatch(TokenId.KW_ON, out _))
        {
            if (TryMatch(TokenId.KW_UPDATE, out _))
            {
                Match(TokenId.KW_CASCADE);
                updateRule = ForeignKeyRule.Cascade;
            }
            else if (TryMatch(TokenId.KW_DELETE, out _))
            {
                Match(TokenId.KW_CASCADE);
                deleteRule = ForeignKeyRule.Cascade;
            }
        }

        MetaData attributes = new()
        {
            ["CONSTRAINT_NAME"] = fkName,
            ["TARGET_TABLE_NAME"] = targetTableName,
            ["UPDATE_RULE"] = updateRule,
            ["DELETE_RULE"] = deleteRule
        };

        return new AstNode(AstNodeKind.FKSPEC, attributes, fkColNames, pkColNames);
    }

    /// <summary>
    /// Creates an abstract syntax tree (AST) node representing a SQL CHECK constraint specification.
    /// </summary>
    /// <param name="chkName">The name of the CHECK constraint being defined.</param>
    /// <returns>An <see cref="AstNode"/> representing the CHECK constraint, containing its name and predicate.</returns>
    private AstNode CheckSpec(string chkName)
    {
        Match(TokenId.KW_CHECK);
        var predicate = Expression(); // Should be a predicate

        return new AstNode(AstNodeKind.CHKSPEC, chkName, predicate);
    }
    
    #endregion

    #region Expressions parsing

    /// <summary>
    /// Parses and constructs an abstract syntax tree (AST) node representing a logical or relational expression.
    /// </summary>
    /// <returns>An <see cref="AstNode"/> representing the parsed expression, or <c>null</c> if the input is invalid or incomplete.</returns>
    public AstNode Expression()
    {
        var node = Predicate();
        if (node != null)
            node = MorePredicates(node);
        return node;
    }

    /// <summary>
    /// Parses a list of SQL expressions, separated by commas, and constructs an AST node
    /// representing the collection of expressions.
    /// </summary>
    /// <returns>
    /// An <see cref="AstNode"/> instance of type <see cref="AstNodeKind.EXPLIST"/>,
    /// which encapsulates the parsed expressions within its child nodes.
    /// </returns>
    private AstNode ExpressionList()
    {
        Token tok;
        var expressions = new List<AstNode>();

        var expression = Expression();
        while (expression != null)
        {
            expressions.Add(expression);
            expression = TryMatch(TokenId.COMMA, out tok) ? Expression() : null;
        }

        return new AstNode(AstNodeKind.EXPLIST, null, expressions.ToArray());
    }

    /// <summary>
    /// Parses a predicate from the input, which typically combines one or more relational expressions
    /// or logical constructs to evaluate a condition within a SQL statement.
    /// </summary>
    /// <returns>An <see cref="AstNode"/> representing the parsed predicate, or null if no valid predicate is found.</returns>
    private AstNode Predicate()
    {
        var node = Relation();
        if (node != null)
            node = MoreRelations(node);
        return node;
    }

    /// <summary>
    /// Processes additional predicates in an expression by combining the current predicate with subsequent expressions
    /// using logical operators (AND/OR).
    /// </summary>
    /// <param name="predicate">The initial predicate node that will be combined with subsequent logical expressions.</param>
    /// <returns>
    /// An <see cref="AstNode"/> representing the combined expression with logical operators, or the original predicate
    /// if no further logical operators are found.
    /// </returns>
    /// <exception cref="SyntaxException">
    /// Thrown when an expression is expected but not found after a logical operator.
    /// </exception>
    private AstNode MorePredicates(AstNode predicate)
    {
        var sign = token.Id;
        if (sign != TokenId.KW_AND && sign != TokenId.KW_OR)
            return predicate;

        Skip();
        var node = Expression() ??
            throw new SyntaxException($"Syntax error after '{sign}'. An expression was expected");

        return sign switch
        {
            TokenId.KW_AND => new AstNode(AstNodeKind.BINEXPR, BinaryOperator.AND, predicate, node),
            _ => new AstNode(AstNodeKind.BINEXPR, BinaryOperator.OR, predicate, node)
        };
    }

    /// <summary>
    /// Processes a relational expression within the SQL parser to construct the corresponding abstract syntax tree (AST) node.
    /// Handles the parsing of terms and combines them into relational components as needed.
    /// </summary>
    /// <returns>An <see cref="AstNode"/> representing the parsed relational expression, or null if the expression could not be parsed.</returns>
    private AstNode Relation()
    {
        var node = Term();
        if (node != null)
            node = MoreTerms(node);
        return node;
    }

    /// <summary>
    /// Processes additional relations for the given node and builds an abstract syntax tree (AST) node
    /// representing a binary expression based on the current token.
    /// </summary>
    /// <param name="relation">The initial <see cref="AstNode"/> representing the left-hand side of the expression.</param>
    /// <returns>An <see cref="AstNode"/> representing a binary expression if additional relations are found;
    /// otherwise, the original <paramref name="relation"/> is returned.</returns>
    /// <exception cref="SyntaxException">Thrown when an expected expression is missing after a binary operator.</exception>
    private AstNode MoreRelations(AstNode relation)
    {
        var sign = token.Id;
        if (sign != TokenId.EQ && sign != TokenId.NEQ && sign != TokenId.LT &&
            sign != TokenId.LE && sign != TokenId.GT && sign != TokenId.GE &&
            sign != TokenId.KW_IN && sign != TokenId.KW_LIKE)
            return relation;

        Skip();
        var node = Predicate() ??
            throw new SyntaxException($"Syntax error after '{sign}'. An expression was expected");

        return sign switch
        {
            TokenId.EQ => new AstNode(AstNodeKind.BINEXPR, BinaryOperator.EQ, relation, node),
            TokenId.NEQ => new AstNode(AstNodeKind.BINEXPR, BinaryOperator.NEQ, relation, node),
            TokenId.LT => new AstNode(AstNodeKind.BINEXPR, BinaryOperator.LT, relation, node),
            TokenId.LE => new AstNode(AstNodeKind.BINEXPR, BinaryOperator.LE, relation, node),
            TokenId.GT => new AstNode(AstNodeKind.BINEXPR, BinaryOperator.GT, relation, node),
            TokenId.GE => new AstNode(AstNodeKind.BINEXPR, BinaryOperator.GE, relation, node),
            TokenId.KW_IN => new AstNode(AstNodeKind.BINEXPR, BinaryOperator.IN, relation, node),
            _ => new AstNode(AstNodeKind.BINEXPR, BinaryOperator.LIKE, relation, node)
        };
    }

    /// <summary>
    /// Parses and constructs an abstract syntax tree (AST) node for a term within an expression.
    /// A term is a fundamental unit of arithmetic or logical expressions, typically represented
    /// by factors and their interactions through binary operators such as multiplication, division, or modulus.
    /// </summary>
    /// <returns>An <see cref="AstNode"/> representing the parsed term. Returns null if no term is found.</returns>
    private AstNode Term()
    {
        var node = Factor();
        if (node != null) node = MoreFactors(node);
        return node;
    }

    /// <summary>
    /// Processes additional terms in an expression, if present, and constructs a binary expression node
    /// incorporating the provided term and the subsequent term.
    /// </summary>
    /// <param name="term">The initial term to use as the left operand of the binary expression.</param>
    /// <returns>
    /// An <see cref="AstNode"/> representing the resulting binary expression, or the original term
    /// if no additional terms are present.
    /// </returns>
    /// <exception cref="SyntaxException">
    /// Thrown when the syntax following the operator is invalid or an expression is expected but not found.
    /// </exception>
    private AstNode MoreTerms(AstNode term)
    {
        var sign = token.Id;
        if (sign != TokenId.PLUS && sign != TokenId.MINUS && sign != TokenId.CONCAT)
            return term;

        Skip();
        var node = Relation() ??
            throw new SyntaxException($"Syntax error after '{sign}'. An expression was expected");

        return sign switch
        {
            TokenId.PLUS => new AstNode(AstNodeKind.BINEXPR, BinaryOperator.ADD, term, node),
            TokenId.MINUS => new AstNode(AstNodeKind.BINEXPR, BinaryOperator.SUB, term, node),
            _ => new AstNode(AstNodeKind.BINEXPR, BinaryOperator.CONCAT, term, node),
        };
    }

    /// <summary>
    /// Parses a single factor from the input stream and returns the corresponding abstract syntax tree (AST) node.
    /// </summary>
    /// <returns>
    /// An <see cref="AstNode"/> representing the parsed factor. Returns null if no valid factor is found.
    /// </returns>
    /// <exception cref="SyntaxException">
    /// Thrown when the input does not conform to the expected factor syntax, such as missing tokens or invalid expressions.
    /// </exception>
    private AstNode Factor()
    {
        Token tok;
        AstNode node;

        switch (token.Id)
        {
            case TokenId.LT_NUM:
                tok = token;
                Skip();
                return new AstNode(AstNodeKind.LTNUM, tok.Data);
            case TokenId.LT_CHAR:
                tok = token;
                Skip();
                return new AstNode(AstNodeKind.LTCHAR, tok.Data);
            case TokenId.IDENT:
                tok = token;
                Skip();
                if (token.Id == TokenId.LPAREN)
                {
                    Skip();
                    node = ExpressionList();
                    Match(TokenId.RPAREN);

                    return new AstNode(AstNodeKind.FNCALL, tok.Data, node);
                }

                return new AstNode(AstNodeKind.COLREF, tok.Data);
            case TokenId.LPAREN:
                Skip();
                node = Expression();
                Match(TokenId.RPAREN);
                return node;
            case TokenId.PLUS:
                tok = token;
                Skip();
                node = Factor() ??
                    throw new SyntaxException($"Syntax error after '{tok.Id}'. An expression was expected");
                return new AstNode(AstNodeKind.UNEXPR, UnaryOperator.PLUS, node);
            case TokenId.MINUS:
                tok = token;
                Skip();
                node = Factor() ??
                    throw new SyntaxException($"Syntax error after '{tok.Id}'. An expression was expected");
                return new AstNode(AstNodeKind.UNEXPR, UnaryOperator.MINUS, node);
            case TokenId.KW_NOT:
                tok = token;
                Skip();
                node = Factor() ??
                    throw new SyntaxException($"Syntax error after '{tok.Id}'. An expression was expected");
                return new AstNode(AstNodeKind.UNEXPR, UnaryOperator.NOT, node);
            default:
                throw new SyntaxException("Unexpected token: " + token.Id);
        }
    }

    /// <summary>
    /// Processes additional binary operations (multiplication, division, or modulus) on a given factor
    /// and returns a binary expression node combining the operations.
    /// </summary>
    /// <param name="factor">The initial factor node to which additional operations will be applied.</param>
    /// <returns>An <see cref="AstNode"/> representing a binary expression that combines the factor and subsequent terms,
    /// or the original factor if no additional binary operations are found.</returns>
    /// <exception cref="SyntaxException">Thrown when an expected expression is missing after a binary operator.</exception>
    private AstNode MoreFactors(AstNode factor)
    {
        var sign = token.Id;
        if (sign != TokenId.TIMES && sign != TokenId.DIV && sign != TokenId.MOD)
            return factor;

        Skip();
        var node = Term() ??
            throw new SyntaxException($"Syntax error after '{sign}'. An expression was expected");

        return sign switch
        {
            TokenId.TIMES => new AstNode(AstNodeKind.BINEXPR, BinaryOperator.MUL, factor, node),
            TokenId.DIV => new AstNode(AstNodeKind.BINEXPR, BinaryOperator.DIV, factor, node),
            _ => new AstNode(AstNodeKind.BINEXPR, BinaryOperator.MOD, factor, node)
        };
    }

    /// <summary>
    /// Parses and constructs a list of column references from a sequence of tokens.
    /// </summary>
    /// <returns>
    /// An <see cref="AstNode"/> representing a list of column references. Each column reference is represented
    /// as a child node of type <see cref="AstNodeKind.COLREF"/>, and the parent node is of type <see cref="AstNodeKind.COLREFLIST"/>.
    /// </returns>
    private AstNode ColumnRefList()
    {
        var childNodes = new List<AstNode>();
        var name = Match(TokenId.IDENT).Data.ToString();
        childNodes.Add(new AstNode(AstNodeKind.COLREF, name));

        Token tok;
        while (TryMatch(TokenId.COMMA, out tok))
        {
            name = Match(TokenId.IDENT).Data.ToString();
            childNodes.Add(new AstNode(AstNodeKind.COLREF, name));
        }

        return new AstNode(AstNodeKind.COLREFLIST, null, childNodes.ToArray());
    }

    #endregion
    
    #region Utility methods

    /// <summary>
    /// Advances the current token by reading the next token from the associated scanner.
    /// This method is used to progress parsing by skipping over the current token.
    /// </summary>
    private void Skip()
    {
        token = scanner.NextToken();
    }

    /// <summary>
    /// Matches the current token with the specified token ID and optionally checks a condition.
    /// Updates the current token to the next one after a successful match.
    /// </summary>
    /// <param name="tokenId">The expected token ID to match with the current token.</param>
    /// <param name="cond">An optional predicate that specifies an additional condition the token must satisfy. Can be null.</param>
    /// <returns>The matched token if the token ID and condition are satisfied.</returns>
    /// <exception cref="SyntaxException">
    /// Thrown when the current token does not match the specified token ID,
    /// or when the condition (if provided) is not satisfied.
    /// </exception>
    private Token Match(TokenId tokenId, Predicate<Token> cond)
    {
        if (token.Id != tokenId)
            throw new SyntaxException($"Encountered '{token.Id}' while expecting '{tokenId}'");

        if (cond != null && !cond(token))
            throw new SyntaxException($"Encountered '{tokenId}' without satisfying '{cond}'");

        var matched = token;
        Skip();

        return matched;
    }

    /// <summary>
    /// Matches the current token against the expected <see cref="TokenId"/>.
    /// If the token does not match the expected <paramref name="tokenId"/>, a <see cref="SyntaxException"/> is thrown.
    /// If matched successfully, the current token is returned and the parser advances to the next token.
    /// </summary>
    /// <param name="tokenId">The expected <see cref="TokenId"/> to match against the current token.</param>
    /// <returns>The <see cref="Token"/> that matches the specified <paramref name="tokenId"/>.</returns>
    private Token Match(TokenId tokenId) => Match(tokenId, null);

    /// <summary>
    /// Attempts to match the current token with the specified <paramref name="tokenId"/>
    /// and an optional condition provided by <paramref name="cond"/>.
    /// If successful, the matched token is returned in <paramref name="matched"/>.
    /// </summary>
    /// <param name="tokenId">The identifier of the token to match.</param>
    /// <param name="cond">An optional predicate to apply additional conditions to the token.</param>
    /// <param name="matched">The matched token if the method succeeds; otherwise, null.</param>
    /// <returns>True if the token matches the specified identifier and condition, otherwise false.</returns>
    private bool TryMatch(TokenId tokenId, Predicate<Token> cond, out Token matched)
    {
        matched = null;

        if (token.Id != tokenId) return false;
        if (cond?.Invoke(token) == false) return false;
            
        matched = token;
        Skip();
        return true;
    }

    /// <summary>
    /// Attempts to match a specified token ID and retrieve the corresponding token, if found.
    /// </summary>
    /// <param name="tokenId">The ID of the token to be matched.</param>
    /// <param name="matched">The matched token, if successfully found.</param>
    /// <returns>
    /// A boolean value indicating whether the specified token was successfully matched.
    /// </returns>
    private bool TryMatch(TokenId tokenId, out Token matched) => TryMatch(tokenId, null, out matched);
    
    #endregion
}