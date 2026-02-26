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

public static partial class Utility
{
    private static readonly CultureInfo CI = CultureInfo.InvariantCulture;
    
    public static Encoding Encoding { get; set; } = Encoding.UTF8;
    
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

    public static string TransformConnectionString(string connectionString, Func<string, string, string> transformer)
    {
        var properties = ParseConnectionString(connectionString);
        return string.Join(";", properties.Select(p => $"{p.Key}={transformer(p.Key, p.Value)}"));
    }

    public static string SanitizeConnectionString(string connectionString) =>
        TransformConnectionString(connectionString, (key, value) =>
            PasswordRegex().IsMatch(key) ? new string('*', value.Length) : value);

    public static DbConnection GetConnection(string providerName, string connectionString)
    {
        var connection = DbProviderFactories.GetFactory(providerName).CreateConnection() ??
            throw new InvalidOperationException($"Cannot create connection for provider '{providerName}'");

        connection.ConnectionString = connectionString;

        return connection;
    }

    public static DbConnection GetConnection(Database database) =>
        GetConnection(database.ProviderName, database.ConnectionString);
    
    public static Dictionary<string, TValue> EmptyDictionary<TValue>() => new(StringComparer.OrdinalIgnoreCase);
    
    public static string[] Split(string input, char separator) =>
        input.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    public static string Escape(string name, string providerName) =>
        providerName switch
        {
            ProviderNames.ACCESS or ProviderNames.SQLSERVER or ProviderNames.SQLITE => $"[{name}]",
            ProviderNames.MYSQL => $"`{name}`",
            _ => $"\"{name}\""
        };

    public static bool IsEmpty(object value) =>
        value switch
        {
            null or DBNull => true,
            string s => string.IsNullOrWhiteSpace(s),
            Array a => a.Length == 0,
            _ => false
        };

    public static bool IsBoolean(object value) =>
        Convert.ToString(value, CI)?.ToLower() is "false" or "true";

    public static bool IsNumeric(object value, out decimal converted) =>
        decimal.TryParse(Convert.ToString(value, CI), NumberStyles.Any, CI, out converted);

    public static bool IsDate(object value, out DateTime converted) =>
        DateTime.TryParse(Convert.ToString(value, CI), CI, DateTimeStyles.None, out converted);

    public static byte ToByte(object value) =>
        byte.TryParse(Convert.ToString(value, CI), NumberStyles.Any, CI, out var result) ? result : default;

    public static short ToInt16(object value) =>
        short.TryParse(Convert.ToString(value, CI), NumberStyles.Any, CI, out var result) ? result : default;

    public static string QuotedStr(object value, char quote = '\'') =>
        $"{quote}{value.ToString()?.Replace(quote.ToString(), new string(quote, 2))}{quote}";
    
    public static string UnquotedStr(object value, char quote = '\'') =>
        value.ToString()?[1..^1].Replace(new string(quote, 2), quote.ToString());

    public static string BinToHex(byte[] bytes) =>
        bytes.Select(b => b.ToString("x2"))
             .Aggregate(new StringBuilder(), (a, b) => a.Append(b))
             .ToString();

    public static string ToBitString(byte[] bytes) =>
        bytes.Select(b => ToBaseN(b, 2))
             .Aggregate(new StringBuilder(), (a, b) => a.Append(b))
             .ToString();

    public static byte[] FromBitString(string value)
    {
        var bytes = new byte[(value.Length + 7) / 8];
        value = value.PadLeft(8 * bytes.Length, '0');

        for (int i = 0, j = 0; i < bytes.Length; ++i, j += 8)
            bytes[i] = FromBaseN(value.Substring(j, 8), 2);

        return bytes;
    }

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

    public static byte FromBaseN(string value, byte n) =>
        value.Aggregate<char, byte>(0, (current, next) => (byte)(n * current + Char2Digit(next)));

    private static char Digit2Char(int i) => i switch
    {
        >= 0 and <= 9 => (char)('0' + i),
        _ => (char)('A' + i - 10)
    };

    private static int Char2Digit(char c) => c switch
    {
        >= '0' and <= '9' => c - '0',
        _ => c - 'A' + 10
    };

    [GeneratedRegex(@"\b(password|pwd)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex PasswordRegex();
}