﻿using DbExport.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbExport.Providers.SQLite.SqlParser
{
    public class Parser
    {
        private Scanner scanner;
        private Token token;

        public Parser(Scanner scanner)
        {
            this.scanner = scanner;
            Skip();
        }

        public AstNode CreateTable()
        {
            Match(TokenId.KW_CREATE);
            Match(TokenId.KW_TABLE);
            var tableName = Match(TokenId.IDENT).Data.ToString();
            Match(TokenId.LPAREN);
            var colSpecList = ColumnSpecList();

            var childNodes = new List<AstNode>() { colSpecList };
            AstNode childNode;
            var loop = true;

            while (loop)
            {
                switch (token.Id)
                {
                    case TokenId.KW_PRIMARY:
                        childNode = PrimaryKeySpec();
                        childNodes.Add(childNode);
                        if (token.Id == TokenId.COMMA)
                            Skip();
                        break;
                    case TokenId.KW_CONSTRAINT:
                        childNode = ConstraintSpec();
                        childNodes.Add(childNode);
                        if (token.Id == TokenId.COMMA)
                            Skip();
                        break;
                    default:
                        loop = false;
                        break;
                }
            }

            Match(TokenId.RPAREN);

            return new AstNode(AstNodeKind.CREATE_TBL, tableName, childNodes.ToArray());
        }

        private AstNode ColumnSpecList()
        {
            var colSpecs = new List<AstNode>();
            var colSpec = ColumnSpec();
            Token tok;

            while (colSpec != null)
            {
                colSpecs.Add(colSpec);
                if (TryMatch(TokenId.COMMA, out tok))
                    colSpec = ColumnSpec();
                else
                    colSpec = null;
            }

            return new AstNode(AstNodeKind.COLSPECLIST, null, colSpecs.ToArray());
        }

        private AstNode ColumnSpec()
        {
            Token tok;
            if (!TryMatch(TokenId.IDENT, out tok)) return null;

            var colName = tok.Data.ToString();
            var typeName = string.Empty;
            var prec = 0;
            var scale = 0;

            if (TryMatch(TokenId.TYPE, out tok))
            {
                typeName = tok.Data.ToString();

                if (TryMatch(TokenId.LPAREN, out tok))
                {
                    prec = Convert.ToInt32(Match(TokenId.LT_NUM).Data);
                    if (TryMatch(TokenId.COMMA, out tok))
                        scale = Convert.ToInt32(Match(TokenId.LT_NUM).Data);
                    if (TryMatch(TokenId.COMMA, out tok))
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
                        if (TryMatch(TokenId.KW_AUTOINC, out tok))
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

            var attributes = new Dictionary<string, object>();
            attributes["COLUMN_NAME"] = colName;
            attributes["TYPE_NAME"] = typeName;
            attributes["PRECISION"] = prec;
            attributes["SCALE"] = scale;
            attributes["ALLOW_DBNULL"] = allowDbNull;
            attributes["UNIQUE"] = unique;
            attributes["PRIMARY_KEY"] = pk;
            attributes["AUTO_INCREMENT"] = autoinc;
            attributes["DEFAULT_VALUE"] = defaultValue;

            return new AstNode(AstNodeKind.COLSPEC, attributes);

        }

        private AstNode PrimaryKeySpec()
        {
            Match(TokenId.KW_PRIMARY);
            Match(TokenId.KW_KEY);
            Match(TokenId.LPAREN);
            var pkColumnList = ColumnRefList();
            Match(TokenId.RPAREN);

            return new AstNode(AstNodeKind.PKSPEC, null, pkColumnList);
        }

        private AstNode ConstraintSpec()
        {
            Match(TokenId.KW_CONSTRAINT);
            var constraintName = Match(TokenId.IDENT).Data.ToString();

            switch (token.Id)
            {
                case TokenId.KW_UNIQUE:
                    return UniqueKeySpec(constraintName);
                case TokenId.KW_FOREIGN:
                    return ForeignKeySpec(constraintName);
                case TokenId.KW_CHECK:
                    return CheckSpec(constraintName);
                default:
                    throw new ApplicationException("Invalid constraint specification " + token.Id);
            }
        }

        private AstNode UniqueKeySpec(string ukName)
        {
            Match(TokenId.KW_UNIQUE);
            Match(TokenId.LPAREN);
            var colNames = ColumnRefList();
            Match(TokenId.RPAREN);

            return new AstNode(AstNodeKind.UKSPEC, ukName, colNames);
        }

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

            Token tok;
            var updateRule = ForeignKeyRule.None;
            var deleteRule = ForeignKeyRule.None;

            while (TryMatch(TokenId.KW_ON, out tok))
            {
                if (TryMatch(TokenId.KW_UPDATE, out tok))
                {
                    Match(TokenId.KW_CASCADE);
                    updateRule = ForeignKeyRule.Cascade;
                }
                else if (TryMatch(TokenId.KW_DELETE, out tok))
                {
                    Match(TokenId.KW_CASCADE);
                    deleteRule = ForeignKeyRule.Cascade;
                }
            }

            var attributes = new Dictionary<string, object>();
            attributes["CONSTRAINT_NAME"] = fkName;
            attributes["TARGET_TABLE_NAME"] = targetTableName;
            attributes["UPDATE_RULE"] = updateRule;
            attributes["DELETE_RULE"] = deleteRule;

            return new AstNode(AstNodeKind.FKSPEC, attributes, fkColNames, pkColNames);
        }

        private AstNode CheckSpec(string chkName)
        {
            Match(TokenId.KW_CHECK);
            var predicate = Expression(); // Should be a predicate

            return new AstNode(AstNodeKind.CHKSPEC, chkName, predicate);
        }

        #region Expressions parsing

        public AstNode Expression()
        {
            var node = Predicate();
            if (node != null)
                node = MorePredicates(node);
            return node;
        }

        private AstNode ExpressionList()
        {
            Token tok;
            var expressions = new List<AstNode>();

            var expression = Expression();
            while (expression != null)
            {
                expressions.Add(expression);
                if (TryMatch(TokenId.COMMA, out tok))
                    expression = Expression();
                else
                    expression = null;
            }

            return new AstNode(AstNodeKind.EXPLIST, null, expressions.ToArray());
        }

        private AstNode Predicate()
        {
            var node = Relation();
            if (node != null)
                node = MoreRelations(node);
            return node;
        }

        private AstNode MorePredicates(AstNode predicate)
        {
            var sign = token.Id;
            if (sign != TokenId.KW_AND && sign != TokenId.KW_OR)
                return predicate;

            Skip();
            var node = Expression();
            if (node == null)
                throw new ApplicationException("Syntax error after " + sign + ". An expression was expected");

            switch (sign)
            {
                case TokenId.KW_AND:
                    return new AstNode(AstNodeKind.BINEXPR, BinaryOperator.AND, predicate, node);
                default:
                    return new AstNode(AstNodeKind.BINEXPR, BinaryOperator.OR, predicate, node);
            }
        }

        private AstNode Relation()
        {
            var node = Term();
            if (node != null)
                node = MoreTerms(node);
            return node;
        }

        private AstNode MoreRelations(AstNode relation)
        {
            var sign = token.Id;
            if (sign != TokenId.EQ && sign != TokenId.NEQ && sign != TokenId.LT &&
                sign != TokenId.LE && sign != TokenId.GT && sign != TokenId.GE &&
                sign != TokenId.KW_IN && sign != TokenId.KW_LIKE)
                return relation;

            Skip();
            var node = Predicate();
            if (node == null)
                throw new ApplicationException("Syntax error after " + sign + ". An expression was expected");

            switch (sign)
            {
                case TokenId.EQ:
                    return new AstNode(AstNodeKind.BINEXPR, BinaryOperator.EQ, relation, node);
                case TokenId.NEQ:
                    return new AstNode(AstNodeKind.BINEXPR, BinaryOperator.NEQ, relation, node);
                case TokenId.LT:
                    return new AstNode(AstNodeKind.BINEXPR, BinaryOperator.LT, relation, node);
                case TokenId.LE:
                    return new AstNode(AstNodeKind.BINEXPR, BinaryOperator.LE, relation, node);
                case TokenId.GT:
                    return new AstNode(AstNodeKind.BINEXPR, BinaryOperator.GT, relation, node);
                case TokenId.GE:
                    return new AstNode(AstNodeKind.BINEXPR, BinaryOperator.GE, relation, node);
                case TokenId.KW_IN:
                    return new AstNode(AstNodeKind.BINEXPR, BinaryOperator.IN, relation, node);
                default:
                    return new AstNode(AstNodeKind.BINEXPR, BinaryOperator.LIKE, relation, node);
            }
        }

        private AstNode Term()
        {
            var node = Factor();
            if (node != null)
                node = MoreFactors(node);
            return node;
        }

        private AstNode MoreTerms(AstNode term)
        {
            var sign = token.Id;
            if (sign != TokenId.PLUS && sign != TokenId.MINUS && sign != TokenId.CONCAT)
                return term;

            Skip();
            var node = Relation();
            if (node == null)
                throw new ApplicationException("Syntax error after " + sign + ". An expression was expected");

            switch (sign)
            {
                case TokenId.PLUS:
                    return new AstNode(AstNodeKind.BINEXPR, BinaryOperator.ADD, term, node);
                case TokenId.MINUS:
                    return new AstNode(AstNodeKind.BINEXPR, BinaryOperator.SUB, term, node);
                default:
                    return new AstNode(AstNodeKind.BINEXPR, BinaryOperator.CONCAT, term, node);
            }
        }

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
                    node = Factor();
                    if (node == null)
                        throw new ApplicationException("Syntax error after " + tok.Id + ". An expression was expected");
                    return new AstNode(AstNodeKind.UNEXPR, UnaryOperator.PLUS, node);
                case TokenId.MINUS:
                    tok = token;
                    Skip();
                    node = Factor();
                    if (node == null)
                        throw new ApplicationException("Syntax error after " + tok.Id + ". An expression was expected");
                    return new AstNode(AstNodeKind.UNEXPR, UnaryOperator.MINUS, node);
                case TokenId.KW_NOT:
                    tok = token;
                    Skip();
                    node = Factor();
                    if (node == null)
                        throw new ApplicationException("Syntax error after " + tok.Id + ". An expression was expected");
                    return new AstNode(AstNodeKind.UNEXPR, UnaryOperator.NOT, node);
                default:
                    throw new ApplicationException("Unexpected token: " + token.Id);
            }
        }

        private AstNode MoreFactors(AstNode factor)
        {
            var sign = token.Id;
            if (sign != TokenId.TIMES && sign != TokenId.DIV && sign != TokenId.MOD)
                return factor;

            Skip();
            var node = Term();
            if (node == null)
                throw new ApplicationException("Syntax error after " + sign + ". An expression was expected");

            switch (sign)
            {
                case TokenId.TIMES:
                    return new AstNode(AstNodeKind.BINEXPR, BinaryOperator.MUL, factor, node);
                case TokenId.DIV:
                    return new AstNode(AstNodeKind.BINEXPR, BinaryOperator.DIV, factor, node);
                default:
                    return new AstNode(AstNodeKind.BINEXPR, BinaryOperator.MOD, factor, node);
            }
        }

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

        private void Skip()
        {
            token = scanner.NextToken();
        }

        private Token Match(TokenId tokenId, Predicate<Token> cond)
        {
            if (token.Id != tokenId)
                throw new ApplicationException("Encountered " + token.Id + " while expecting " + tokenId);

            if (cond != null && !cond.Invoke(token))
                throw new ApplicationException("Encountered " + tokenId + " without satisfying " + cond);

            var prevTok = token;
            Skip();

            return prevTok;
        }

        private Token Match(TokenId tokenId)
        {
            return Match(tokenId, null);
        }

        private bool TryMatch(TokenId tokenId, Predicate<Token> cond, out Token found)
        {
            found = null;

            if (token.Id == tokenId)
            {
                if (cond == null || cond.Invoke(token))
                {
                    found = token;
                    Skip();

                    return true;
                }
            }

            return false;
        }

        private bool TryMatch(TokenId tokenId, out Token found)
        {
            return TryMatch(tokenId, null, out found);
        }
    }
}
