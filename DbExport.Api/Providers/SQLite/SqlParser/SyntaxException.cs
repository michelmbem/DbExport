using System;

namespace DbExport.Providers.SQLite.SqlParser;

/// <summary>
/// Represents an exception specific to syntax errors encountered during SQL parsing.
/// </summary>
/// <remarks>
/// This exception is thrown to indicate that the input SQL expression contains invalid or unsupported syntax.
/// It is primarily used in the context of SQL parsing logic within the SQLite provider.
/// </remarks>
/// <param name="message">The error message describing the syntax error.</param>
public class SyntaxException(string message) : Exception(message);
