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

public static class Utility
{
    public const int DEFAULT_CODE_PAGE = 65001; // UTF-8

    public static Encoding Encoding { get; set; }
    
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
        
        var properties = new Dictionary<string, string>();

        foreach (var setting in connectionString.Split(';', splitOptions))
        {
            var members = setting.Split('=', splitOptions);
            var (key, value) = (members[0], members.Length > 1 ? members[1] : string.Empty);
            if (string.IsNullOrWhiteSpace(key)) continue;
            if (value.StartsWith('"')) value = value[1..^1].Replace("\"\"", "\"");
            properties[key.ToLower()] = value;
        }

        return properties;
    }

    public static string SanitizeConnectionString(string connectionString)
    {
        const StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
        
        var sb = new StringBuilder();

        foreach (var setting in connectionString.Split(';', splitOptions))
        {
            var members = setting.Split('=', splitOptions);
            var (key, value) = (members[0], members.Length > 1 ? members[1] : string.Empty);
            if (string.IsNullOrWhiteSpace(key)) continue;
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
            null => true,
            string s => string.IsNullOrWhiteSpace(s),
            Array array => array.Length == 0,
            _ => value == DBNull.Value || value.Equals(0)
        };

    public static bool IsBoolean(object value) => Convert.ToString(value)?.ToLower() is "false" or "true";

    public static bool IsNumeric(object value)
    {
        var numeric = true;

        try
        {
            _ = Convert.ToDecimal(value);
        }
        catch
        {
            numeric = false;
        }

        return numeric;
    }

    public static bool IsDate(object value)
    {
        var date = true;

        try
        {
            _ = Convert.ToDateTime(value);
        }
        catch
        {
            date = false;
        }

        return date;
    }

    public static byte ToByte(object value)
    {
        byte res = 0;

        try
        {
            res = Convert.ToByte(value);
        }
        catch (InvalidCastException)
        {
            // ignore
        }
        catch (OverflowException)
        {
            // ignore
        }

        return res;
    }

    public static short ToInt16(object value)
    {
        short res = 0;

        try
        {
            res = Convert.ToInt16(value);
        }
        catch (InvalidCastException)
        {
            // ignore
        }
        catch (OverflowException)
        {
            // ignore
        }

        return res;
    }

    public static string QuotedStr(object value, char quote = '\'') =>
        $"{quote}{value.ToString().Replace(quote.ToString(), new string(quote, 2))}{quote}";

    public static string BinToHex(byte[] bytes)
    {
        var sb = new StringBuilder();

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
        var sb = new StringBuilder();

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
                default:
                    if (b is >= 32 and < 127)
                        sb.Append((char)b);
                    else
                        sb.Append(@"\\").Append(ToBaseN(b, 8).PadLeft(3, '0'));
                    break;
            }
        }

        return sb.ToString();
    }

    public static byte[] GetBytes(string value)
    {
        var bytes = new List<byte>();
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
                                else if (Regex.IsMatch(value.Substring(counter + 2, 3), @"\d{3}"))
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

        return bytes.ToArray();
    }

    public static string ToBitString(byte[] bytes)
    {
        var sb = new StringBuilder();

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

        var sb = new StringBuilder();
            
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
}