using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DbExport.Providers;
using DbExport.Schema;

namespace DbExport;

public static partial class Utility
{
    public static Encoding Encoding { get; set; } = Encoding.UTF8;
    
    public static void RegisterDbProviderFactories()
    {
        SQLitePCL.Batteries.Init();

#if WINDOWS
#pragma warning disable CA1416 // Verify platform compatibility
        DbProviderFactories.RegisterFactory(ProviderNames.ACCESS, System.Data.OleDb.OleDbFactory.Instance);
#pragma warning restore CA1416 // Verify platform compatibility
#endif
        DbProviderFactories.RegisterFactory(ProviderNames.SQLSERVER, Microsoft.Data.SqlClient.SqlClientFactory.Instance);
        DbProviderFactories.RegisterFactory(ProviderNames.ORACLE, Oracle.ManagedDataAccess.Client.OracleClientFactory.Instance);
        DbProviderFactories.RegisterFactory(ProviderNames.MYSQL, MySql.Data.MySqlClient.MySqlClientFactory.Instance);
        DbProviderFactories.RegisterFactory(ProviderNames.POSTGRESQL, Npgsql.NpgsqlFactory.Instance);
        DbProviderFactories.RegisterFactory(ProviderNames.SQLITE, System.Data.SQLite.SQLiteFactory.Instance);
    }

    public static Dictionary<string, string> ParseConnectionString(string connectionString)
    {
        const StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
        
        Dictionary<string, string> properties = [];

        foreach (var setting in connectionString.Split(';', splitOptions))
        {
            var members = setting.Split('=', splitOptions);
            var (key, value) = (members[0], members.Length > 1 ? members[1] : string.Empty);
            if (key.Length == 0) continue;
            if (value.StartsWith('"')) value = value[1..^1].Replace("\"\"", "\"");
            properties[key.ToLower()] = value;
        }

        return properties;
    }

    public static string SanitizeConnectionString(string connectionString)
    {
        const StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
        
        StringBuilder sb = new();

        foreach (var setting in connectionString.Split(';', splitOptions))
        {
            var members = setting.Split('=', splitOptions);
            var (key, value) = (members[0], members.Length > 1 ? members[1] : string.Empty);
            if (key.Length == 0) continue;
            if (key.Equals("password", StringComparison.OrdinalIgnoreCase) ||
                key.Equals("pwd", StringComparison.OrdinalIgnoreCase))
                sb.Append($"{key}={new string('*', value.Length)};");
            else
                sb.Append($"{key}={value};");
        }

        return sb.ToString();
    }

    public static DbConnection GetConnection(string providerName, string connectionString)
    {
        var connection = DbProviderFactories.GetFactory(providerName).CreateConnection() ??
            throw new InvalidOperationException($"Cannot create connection for provider '{providerName}'");

        connection.ConnectionString = connectionString;

        return connection;
    }

    public static DbConnection GetConnection(Database database) =>
        GetConnection(database.ProviderName, database.ConnectionString);

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
            _ => value.Equals(0)
        };

    public static bool IsBoolean(object value) => Convert.ToString(value)?.ToLower() is "false" or "true";

    public static bool IsNumeric(object value) =>
        decimal.TryParse(value?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out _);

    public static bool IsDate(object value) =>
        DateTime.TryParse(value?.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out _);

    public static byte ToByte(object value) =>
        byte.TryParse(value?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var res) ? res : (byte)0;

    public static short ToInt16(object value) =>
        short.TryParse(value?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var res) ? res : (short)0;

    public static string QuotedStr(object value, char quote = '\'') =>
        $"{quote}{value.ToString()?.Replace(quote.ToString(), new string(quote, 2))}{quote}";
    
    public static string UnquotedStr(object value, char quote = '\'') =>
        value.ToString()?[1..^1].Replace(new string(quote, 2), quote.ToString());

    public static string BinToHex(byte[] bytes)
    {
        StringBuilder sb = new();

        foreach (var b in bytes)
            sb.Append(b.ToString("x2"));

        return sb.ToString();
    }

    public static byte[] Hex2Bin(string value)
    {
        var bytes = new byte[value.Length / 2];

        for (int i = 0, j = 0; i < bytes.Length; ++i, j += 2)
            bytes[i] = byte.Parse(value.Substring(j, 2), NumberStyles.AllowHexSpecifier);

        return bytes;
    }

    public static string GetString(byte[] bytes)
    {
        StringBuilder sb = new();

        foreach (var b in bytes)
        {
            switch (b)
            {
                case 39: // single quote
                    sb.Append("''");
                    break;
                case 92: // backslash
                    sb.Append(@"\\\\");
                    break;
                case >= 32 and < 127:
                    sb.Append((char)b);
                    break;
                default:
                    sb.Append(@"\\").Append(ToBaseN(b, 8).PadLeft(3, '0'));
                    break;
            }
        }

        return sb.ToString();
    }

    public static byte[] GetBytes(string value)
    {
        List<byte> bytes = [];
        var counter = 0;

        try
        {
            while (counter < value.Length)
            {
                var c = value[counter];
                switch (c)
                {
                    case '\'':
                        switch (value[counter + 1])
                        {
                            case '\'':
                                bytes.Add(39);
                                counter += 2;
                                break;
                            default:
                                throw new FormatException("Single quotes must be doubled");
                        }
                        break;
                    case '\\':
                        switch (value[counter + 1])
                        {
                            case 'a':
                                bytes.Add(7);
                                counter += 2;
                                break;
                            case 'b':
                                bytes.Add(8);
                                counter += 2;
                                break;
                            case 't':
                                bytes.Add(9);
                                counter += 2;
                                break;
                            case 'n':
                                bytes.Add(10);
                                counter += 2;
                                break;
                            case 'v':
                                bytes.Add(11);
                                counter += 2;
                                break;
                            case 'f':
                                bytes.Add(12);
                                counter += 2;
                                break;
                            case 'r':
                                bytes.Add(13);
                                counter += 2;
                                break;
                            case '\"':
                                bytes.Add(34);
                                counter += 2;
                                break;
                            case '\'':
                                bytes.Add(39);
                                counter += 2;
                                break;
                            case '\\':
                                if (value.Substring(counter, 4) == @"\\\\")
                                {
                                    bytes.Add(92);
                                    counter += 4;
                                }
                                else if (ThreeDigitsRegex().IsMatch(value.AsSpan(counter + 2, 3)))
                                {
                                    bytes.Add(FromBaseN(value.Substring(counter + 2, 3), 8));
                                    counter += 5;
                                }
                                else
                                    throw new FormatException("Invalid escape sequence");
                                break;
                            default:
                                throw new FormatException("Invalid escape sequence");
                        }
                        break;
                    default:
                        bytes.Add((byte) c);
                        ++counter;
                        break;
                }
            }
        }
        catch (IndexOutOfRangeException)
        {
            throw new FormatException("The string terminates incorrectly");
        }

        return [..bytes];
    }

    public static string ToBitString(byte[] bytes)
    {
        StringBuilder sb = new();

        foreach (var b in bytes)
            sb.Append(ToBaseN(b, 2));

        return sb.ToString();
    }

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
    
    [GeneratedRegex(@"\d{3}", RegexOptions.Compiled)]
    private static partial Regex ThreeDigitsRegex();
}