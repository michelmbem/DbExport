using System;

namespace DbExport.Providers.SQLite.SqlParser;

public class SyntaxException(string message) : Exception(message);
