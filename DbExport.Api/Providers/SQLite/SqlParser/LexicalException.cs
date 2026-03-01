using System;

namespace DbExport.Providers.SQLite.SqlParser;

/// <summary>
/// Represents an exception that occurs during lexical analysis of input strings.
/// </summary>
/// <param name="message">The error message describing the lexical analysis error.</param>
public class LexicalException(string message) : Exception(message);
