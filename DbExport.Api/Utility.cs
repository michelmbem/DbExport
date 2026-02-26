using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DbExport.Providers;
using DbExport.Schema;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using SQLitePCL;
#if WINDOWS
using System.Data.OleDb;
#endif

namespace DbExport;

/// <summary>
/// A collection of helper methods and properties for database operations, string manipulation, and type conversion within the application.
/// </summary>
public static partial class Utility
{
    private static readonly CultureInfo CI = CultureInfo.InvariantCulture;

    /// <summary>
    /// The default encoding used for reading and writing data within the application.
    /// This can be set to a different encoding if needed, but defaults to UTF-8 for
    /// broad compatibility with various data sources and formats.
    /// </summary>
    public static Encoding Encoding { get; set; } = Encoding.UTF8;

    #region Database utilities

    /// <summary>
    /// Registers database provider factories for supported database types,
    /// enabling ADO.NET support for those providers within the application.
    /// </summary>
    public static void RegisterDbProviderFactories()
    {
        Batteries.Init();

#if WINDOWS
#pragma warning disable CA1416 // Verify platform compatibility
        DbProviderFactories.RegisterFactory(ProviderNames.ACCESS, OleDbFactory.Instance);
#pragma warning restore CA1416 // Verify platform compatibility
#endif
        DbProviderFactories.RegisterFactory(ProviderNames.SQLSERVER, SqlClientFactory.Instance);
        DbProviderFactories.RegisterFactory(ProviderNames.ORACLE, OracleClientFactory.Instance);
        DbProviderFactories.RegisterFactory(ProviderNames.MYSQL, MySqlClientFactory.Instance);
        DbProviderFactories.RegisterFactory(ProviderNames.POSTGRESQL, NpgsqlFactory.Instance);
        DbProviderFactories.RegisterFactory(ProviderNames.FIREBIRD, FirebirdClientFactory.Instance);
        DbProviderFactories.RegisterFactory(ProviderNames.SQLITE, SQLiteFactory.Instance);
    }

    /// <summary>
    /// Parses a connection string into a dictionary of key-value pairs.
    /// </summary>
    /// <remarks>
    /// If a key is present without a corresponding value, it will be stored with an empty string as its value.
    /// Keys are trimmed of whitespace, and values are unquoted if they are enclosed in double quotes.
    /// </remarks>
    /// <param name="connectionString">The connection string to be parsed, formatted as key-value pairs separated by semicolons.</param>
    /// <returns>
    /// A dictionary containing the parsed key-value pairs from the connection string.
    /// Each key corresponds to a setting name, and each value corresponds to the setting's value.
    /// </returns>
    public static Dictionary<string, string> ParseConnectionString(string connectionString)
    {
        var properties = EmptyDictionary<string>();

        foreach (var setting in Split(connectionString, ';'))
        {
            var members = Split(setting, '=');
            var (key, value) = (members[0], members.Length > 1 ? members[1] : string.Empty);
            if (key.Length == 0) continue;
            if (value.StartsWith('"')) value = UnquotedStr(value, '"');
            properties[key] = value;
        }

        return properties;
    }

    /// <summary>
    /// Transforms the specified connection string by applying a provided transformation function to each property value.
    /// </summary>
    /// <remarks>
    /// The method parses the input connection string into its constituent properties before applying the transformation function to each value.
    /// </remarks>
    /// <param name="connectionString">
    /// The connection string to be transformed, formatted as a semicolon-separated list of key-value pairs.</param>
    /// <param name="transformer">
    /// A function that takes a property key and its corresponding value, and returns the transformed value for that property.
    /// </param>
    /// <returns>A new connection string with the transformed property values, formatted as a semicolon-separated list.</returns>
    public static string TransformConnectionString(string connectionString, Func<string, string, string> transformer)
    {
        var properties = ParseConnectionString(connectionString);
        return string.Join(";", properties.Select(p => $"{p.Key}={transformer(p.Key, p.Value)}"));
    }

    /// <summary>
    /// Returns a sanitized version of the specified connection string with sensitive information, such as passwords, masked to prevent exposure.
    /// </summary>
    /// <remarks>
    /// This method identifies password-related keys in the connection string using a regular expression and masks their values.
    /// Use this method to safely display or log connection strings without revealing sensitive credentials.
    /// </remarks>
    /// <param name="connectionString">
    /// The connection string to be sanitized. May contain sensitive information that should be masked before logging or displaying.
    /// </param>
    /// <returns>A connection string in which password values are replaced with asterisks of the same length as the original value.</returns>
    public static string SanitizeConnectionString(string connectionString) =>
        TransformConnectionString(connectionString, (key, value) =>
            PasswordRegex().IsMatch(key) ? new string('*', value.Length) : value);

    /// <summary>
    /// Creates and returns a database connection using the specified provider name and connection string.
    /// </summary>
    /// <remarks>
    /// Ensure that the provider name is valid and that the connection string is correctly formatted for the provider being used.
    /// </remarks>
    /// <param name="providerName">
    /// The name of the database provider to use for creating the connection. This must match a registered provider name.
    /// </param>
    /// <param name="connectionString">
    /// The connection string used to establish the connection to the database. It must be a valid connection string for the specified provider.
    /// </param>
    /// <returns>A DbConnection object that represents the established connection to the database.</returns>
    /// <exception cref="InvalidOperationException">Thrown if a connection cannot be created for the specified provider name.</exception>
    public static DbConnection GetConnection(string providerName, string connectionString)
    {
        var connection = DbProviderFactories.GetFactory(providerName).CreateConnection() ??
            throw new InvalidOperationException($"Cannot create connection for provider '{providerName}'");

        connection.ConnectionString = connectionString;

        return connection;
    }

    /// <summary>
    /// Creates and returns a database connection using the specified database configuration.
    /// </summary>
    /// <remarks>
    /// The connection is created based on the provider name and connection string provided by the database object.
    /// Ensure that the database object is properly configured before calling this method.
    /// </remarks>
    /// <param name="database">
    /// The database object containing the provider name and connection string used to establish the connection.
    /// Cannot be null.
    /// </param>
    /// <returns>A DbConnection instance representing the established connection to the database.</returns>
    public static DbConnection GetConnection(Database database) =>
        GetConnection(database.ProviderName, database.ConnectionString);

    #endregion

    #region General utilities

    /// <summary>
    /// Creates and returns an empty dictionary with string keys that uses a case-insensitive string comparer.
    /// </summary>
    /// <remarks>
    /// This method is useful for scenarios where a default empty dictionary is needed with case-insensitive key comparison.
    /// The returned dictionary can be used as a placeholder or default value without incurring additional instantiation overhead.
    /// </remarks>
    /// <typeparam name="TValue">The type of the values stored in the dictionary.</typeparam>
    /// <returns>
    /// An empty dictionary with string keys and values of type TValue. The dictionary uses a case-insensitive comparer for its keys.
    /// </returns>
    public static Dictionary<string, TValue> EmptyDictionary<TValue>() => new(StringComparer.OrdinalIgnoreCase);
    
    /// <summary>
    /// Splits the specified string into an array of substrings based on the provided separator character,
    /// removing empty entries and trimming whitespace from each substring.
    /// </summary>
    /// <remarks>
    /// This method uses StringSplitOptions.RemoveEmptyEntries and StringSplitOptions.TrimEntries to
    /// ensure that the resulting array contains only non-empty, trimmed substrings.
    /// </remarks>
    /// <param name="input">The string to be split into substrings. Cannot be null.</param>
    /// <param name="separator">The character that delimits the substrings in the input string.</param>
    /// <returns>
    /// An array of strings containing the substrings from the input string.
    /// The array will be empty if the input string is null, empty, or contains only separator characters.
    /// </returns>
    public static string[] Split(string input, char separator) =>
        input.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    /// <summary>
    /// Escapes the specified identifier name according to the syntax conventions of the given database provider.
    /// </summary>
    /// <remarks>
    /// For ACCESS, SQLSERVER, and SQLITE providers, the identifier is enclosed in square brackets.
    /// For MYSQL, it is enclosed in backticks. For any other provider, the identifier is enclosed in double quotes.
    /// </remarks>
    /// <param name="name">The identifier name to be escaped, such as a table or column name.</param>
    /// <param name="providerName">The name of the database provider that determines the escaping format to use.</param>
    /// <returns>
    /// A string containing the escaped identifier, formatted according to the requirements of the specified database provider.
    /// </returns>
    public static string Escape(string name, string providerName) =>
        providerName switch
        {
            ProviderNames.ACCESS or ProviderNames.SQLSERVER or ProviderNames.SQLITE => $"[{name}]",
            ProviderNames.MYSQL => $"`{name}`",
            _ => $"\"{name}\""
        };

    /// <summary>
    /// Determines whether the specified object is considered empty.
    /// </summary>
    /// <remarks>This method is useful for validating input values before processing. It handles various types
    /// of objects, providing a consistent way to check for emptiness.</remarks>
    /// <param name="value">The object to evaluate for emptiness. This can be null, a string, or an array.</param>
    /// <returns>
    /// Returns <see langword="true"/> if the object is null, DBNull, an empty string, or an empty array;
    /// otherwise, returns <see langword="false"/>.
    /// </returns>
    public static bool IsEmpty(object value) =>
        value switch
        {
            null or DBNull => true,
            string s => string.IsNullOrWhiteSpace(s),
            Array a => a.Length == 0,
            _ => false
        };

    /// <summary>
    /// Checks if the specified object can be interpreted as a boolean value.
    /// </summary>
    /// <param name="value">The object to check for boolean representation.</param>
    /// <returns><c langword="true"/> if the object can be interpreted as a boolean value; otherwise, <c langword="false"/>.</returns>
    public static bool IsBoolean(object value) =>
        Convert.ToString(value, CI)?.ToLower() is "false" or "true";

    /// <summary>
    /// Determines whether the specified object can be interpreted as a numeric value.
    /// </summary>
    /// <param name="value">The object to evaluate for numeric representation.</param>
    /// <param name="converted">When this method returns, contains the decimal representation of the value if conversion is successful;
    /// otherwise, it is set to zero.</param>
    /// <returns>true if the value can be successfully converted to a numeric type; otherwise, false.</returns>
    public static bool IsNumeric(object value, out decimal converted) =>
        decimal.TryParse(Convert.ToString(value, CI), NumberStyles.Any, CI, out converted);

    /// <summary>
    /// Determines whether the specified object can be interpreted as a date/time value.
    /// </summary>
    /// <param name="value">The object to evaluate for date/time representation.</param>
    /// <param name="converted">When this method returns, contains the DateTime representation
    /// of the value if conversion is successful; otherwise, it is set to DateTime.Min.</param>
    /// <returns>true if the value can be successfully converted to a date/time type; otherwise, false.</returns>
    public static bool IsDate(object value, out DateTime converted) =>
        DateTime.TryParse(Convert.ToString(value, CI), CI, DateTimeStyles.None, out converted);

    /// <summary>
    /// Converts the specified object to its equivalent byte value, if possible.
    /// </summary>
    /// <param name="value">The object to convert.</param>
    /// <returns>The byte value of the specified object if the conversion succeeds; otherwise, 0.</returns>
    public static byte ToByte(object value) =>
        byte.TryParse(Convert.ToString(value, CI), NumberStyles.Any, CI, out var result) ? result : default;

    /// <summary>
    /// Converts the specified object to its equivalent 16-bit signed integer using the current culture's formatting
    /// conventions.
    /// </summary>
    /// <param name="value">The object to convert.</param>
    /// <returns>A 16-bit signed integer equivalent to the numeric value contained in the specified object,
    /// or zero if the conversion is unsuccessful.</returns>
    public static short ToInt16(object value) =>
        short.TryParse(Convert.ToString(value, CI), NumberStyles.Any, CI, out var result) ? result : default;

    /// <summary>
    /// Formats the specified value as a string enclosed in the specified quote character, escaping any existing quotes
    /// within the value.
    /// </summary>
    /// <remarks>If the value is null, it will be treated as an empty string. The method replaces any
    /// occurrences of the quote character within the value with two instances of that character to ensure proper
    /// formatting.</remarks>
    /// <param name="value">The value to be formatted as a quoted string. This can be any object that can be converted to a string.</param>
    /// <param name="quote">The character used to enclose the value. The default is a single quote (').</param>
    /// <returns>A string that represents the value enclosed in the specified quote character, with any existing quotes in the
    /// value escaped.</returns>
    public static string QuotedStr(object value, char quote = '\'') =>
        $"{quote}{value.ToString()?.Replace(quote.ToString(), new string(quote, 2))}{quote}";
    
    /// <summary>
    /// Removes surrounding quote characters from the specified string representation and replaces any consecutive quote
    /// characters with a single instance.
    /// </summary>
    /// <remarks>This method is useful for sanitizing input strings that may have been quoted for formatting
    /// purposes. It assumes that the input string is well-formed and contains at least one character.</remarks>
    /// <param name="value">The object to process. The object's string representation must contain surrounding quote characters. Cannot be
    /// null.</param>
    /// <param name="quote">The character used to denote quotes in the string. Defaults to a single quote (').</param>
    /// <returns>A string with the surrounding quote characters removed and any consecutive quote characters replaced with a
    /// single instance. Returns an empty string if the input is null or does not contain surrounding quotes.</returns>
    public static string UnquotedStr(object value, char quote = '\'') =>
        value.ToString()?[1..^1].Replace(new string(quote, 2), quote.ToString());

    /// <summary>
    /// Converts a byte array to its hexadecimal string representation.
    /// </summary>
    /// <remarks>The resulting string concatenates the hexadecimal representations of each byte in the order
    /// they appear in the array. This method is useful for displaying binary data in a readable format, such as for
    /// logging or serialization.</remarks>
    /// <param name="bytes">The array of bytes to convert. Cannot be null.</param>
    /// <returns>A string containing the hexadecimal values of the input bytes, with each byte represented as a two-digit
    /// lowercase hexadecimal number.</returns>
    public static string BinToHex(byte[] bytes) =>
        bytes.Select(b => b.ToString("x2"))
             .Aggregate(new StringBuilder(), (a, b) => a.Append(b))
             .ToString();

    /// <summary>
    /// Converts an array of bytes to a single binary string representation, where each byte is represented as an
    /// 8-character binary value.
    /// </summary>
    /// <remarks>The resulting string does not include separators between bytes. Each byte is represented as
    /// an 8-character binary string, preserving leading zeros.</remarks>
    /// <param name="bytes">The array of bytes to convert. Cannot be null.</param>
    /// <returns>A string containing the binary representation of the input byte array, with each byte's binary value
    /// concatenated in order.</returns>
    public static string ToBitString(byte[] bytes) =>
        bytes.Select(b => ToBaseN(b, 2))
             .Aggregate(new StringBuilder(), (a, b) => a.Append(b))
             .ToString();

    /// <summary>
    /// Converts a binary string representation into an array of bytes.
    /// </summary>
    /// <remarks>The input string must only contain '0' and '1' characters. If the input string is shorter
    /// than a multiple of 8, it will be padded with leading zeros. This method is useful for converting binary data
    /// represented as a string into a byte array for further processing.</remarks>
    /// <param name="value">The binary string to convert, which must consist of '0' and '1' characters. The string will be padded with
    /// leading zeros to ensure its length is a multiple of 8.</param>
    /// <returns>An array of bytes representing the binary string. Each byte corresponds to 8 bits from the input string.</returns>
    public static byte[] FromBitString(string value)
    {
        var bytes = new byte[(value.Length + 7) / 8];
        value = value.PadLeft(8 * bytes.Length, '0');

        for (int i = 0, j = 0; i < bytes.Length; ++i, j += 8)
            bytes[i] = FromBaseN(value.Substring(j, 8), 2);

        return bytes;
    }

    /// <summary>
    /// Converts the specified byte value to its string representation in the given base.
    /// </summary>
    /// <remarks>Supports bases from 2 to 36, using alphanumeric characters for digits beyond 9.</remarks>
    /// <param name="b">The byte value to convert.</param>
    /// <param name="n">The base for the conversion. Must be between 2 and 36, inclusive.</param>
    /// <returns>A string representing the byte value in the specified base. Returns "0" if the input value is zero.</returns>
    public static string ToBaseN(byte b, byte n)
    {
        if (b == 0) return "0";

        StringBuilder sb = new();
    
        while (b > 0)
        {
            sb.Insert(0, Digit2Char(b % n));
            b /= n;
        }

        return sb.ToString();
    }

    /// <summary>
    /// Converts a string representation of a number in a specified base back to its byte value.
    /// </summary>
    /// <param name="value">The string representation of the number to convert. It should consist of valid digits for the specified base.</param>
    /// <param name="n">The base of the number system used in the input string. Must be between 2 and 36, inclusive.</param>
    /// <returns>A byte value corresponding to the input string interpreted as a number in the specified base. Returns 0 if the input string is empty or invalid.</returns>
    public static byte FromBaseN(string value, byte n) =>
        value.Aggregate<char, byte>(0, (current, next) => (byte)(n * current + Char2Digit(next)));

    /// <summary>
    /// Converts a numeric digit or hexadecimal value to its corresponding character representation.
    /// </summary>
    /// <remarks>The method assumes the input is within the valid range of 0 to 15. Supplying a value outside
    /// this range results in undefined behavior.</remarks>
    /// <param name="i">The integer value to convert. Must be in the range 0 to 15, where values from 0 to 9 are mapped to '0'–'9', and
    /// values from 10 to 15 are mapped to 'A'–'F'.</param>
    /// <returns>A character representing the digit or hexadecimal value corresponding to the input integer.</returns>
    private static char Digit2Char(int i) => i switch
    {
        >= 0 and <= 9 => (char)('0' + i),
        _ => (char)('A' + i - 10)
    };

    /// <summary>
    /// Converts a character representing a digit or hexadecimal value back to its numeric integer representation.
    /// </summary>
    /// <param name="c">The character to convert. Valid inputs are '0'–'9' for values 0 to 9, and 'A'–'F' (case-insensitive) for values 10 to 15.
    /// Any character outside these ranges will be processed based on its ASCII value, which may lead to undefined behavior.</param>
    /// <returns>An integer representing the numeric value of the input character. For '0'–'9', returns 0 to 9; for 'A'–'F', returns 10 to 15;
    /// for other characters, returns a value based on their ASCII code.</returns>
    private static int Char2Digit(char c) => c switch
    {
        >= '0' and <= '9' => c - '0',
        _ => c - 'A' + 10
    };

    #endregion

    #region Regular expressions

    [GeneratedRegex(@"\b(password|pwd)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex PasswordRegex();

    #endregion
}