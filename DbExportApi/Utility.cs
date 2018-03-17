using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using DbExport.Schema;

namespace DbExport
{
    public static class Utility
    {
        public const int DEFAULT_CODE_PAGE = 65001; // UTF-8

        public static Encoding Encoding { get; set; }

        public static Dictionary<string, string> ParseConnectionString(string connectionString)
        {
            var properties = new Dictionary<string, string>();
            var settings = connectionString.Split(';');

            foreach (string setting in settings)
            {
                var members = setting.Trim().Split('=');
                var key = members[0];
                var value = members[1];
                if (!(string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value)))
                {
                    if (value.StartsWith("\""))
                        value = value.Substring(1, value.Length - 2);
                    properties[key.ToLower().Trim()] = value.Trim();
                }
            }

            return properties;
        }

        public static string GetRealProviderName(string providerName)
        {
            switch (providerName)
            {
                case "LocalDB":
                    return "System.Data.SqlClient";
                default:
                    return providerName;
            }
        }

        public static DbConnection GetConnection(string providerName, string connectionString)
        {
            var factory = DbProviderFactories.GetFactory(GetRealProviderName(providerName));
            var connection = factory.CreateConnection();
            connection.ConnectionString = connectionString;

            return connection;
        }

        public static DbConnection GetConnection(Database database)
        {
            return GetConnection(database.ProviderName, database.ConnectionString);
        }

        public static string Escape(string name, string providerName)
        {
            switch (providerName)
            {
                case "System.Data.OleDb":
                case "LocalDB":
                case "System.Data.SqlClient":
                case "System.Data.SQLite":
                    return "[" + name + "]";
                case "MySql.Data.MySqlClient":
                    return "`" + name + "`";
                default:
                    return "\"" + name + "\"";
            }
        }

        public static bool IsEmpty(object value)
        {
            if (value == null) return true;
            if (value is string) return ((string) value).Trim().Length <= 0;
            if (value is Array) return ((Array) value).Length <= 0;
            return value == DBNull.Value || value.Equals(0);
        }

        public static bool IsBoolean(object value)
        {
            var str = Convert.ToString(value).ToLower();
            return str == "false" || str == "true";
        }

        public static bool IsNumeric(object value)
        {
            var numeric = true;

            try
            {
                Convert.ToDecimal(value);
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
                Convert.ToDateTime(value);
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
            }
            catch (OverflowException)
            {
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
            }
            catch (OverflowException)
            {
            }

            return res;
        }

        public static string QuotedStr(object value)
        {
            return "'" + value.ToString().Replace("'", "''") + "'";
        }

        public static string BinToHex(byte[] bytes)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
                sb.Append(bytes[i].ToString("x2"));

            return sb.ToString();
        }

        public static byte[] Hex2Bin(string value)
        {
            var bytes = new byte[value.Length / 2];

            for (int i = 0, j = 0; i < bytes.Length; ++i, j += 2)
                bytes[i] = byte.Parse(value.Substring(j, 2), NumberStyles.HexNumber);

            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < bytes.Length; ++i)
            {
                var b = bytes[i];

                switch (b)
                {
                    case 39: // single quote
                        sb.Append("''");
                        break;
                    case 92: // backslash
                        sb.Append(@"\\\\");
                        break;
                    default:
                        if (32 <= b && b < 127)
                            sb.Append((char) b);
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
                throw new FormatException("The string terminates incorectly");
            }

            return bytes.ToArray();
        }

        public static string ToBitString(byte[] bytes)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
                sb.Append(ToBaseN(bytes[i], 2));

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
                sb.Insert(0, b % n);
                b /= n;
            }

            return sb.ToString();
        }

        public static byte FromBaseN(string value, byte n)
        {
            byte b = 0;

            for (int i = 0; i < value.Length; ++i)
                b = (byte) (n * b + value[i] - '0');

            return b;
        }
    }
}
