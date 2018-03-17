namespace DbExport.Providers.MySqlClient
{
    public class MySqlOptions
    {
        public string StorageEngine { get; set; }

        public string CharacterSet { get; set; }

        public string SortOrder { get; set; }

        public static string[] StorageEngines
        {
            get { return new[] {"", "MyISAM", "InnoDB" }; }
        }

        public static string[] CharacterSets
        {
            get
            {
                return new[] { "", "armscii8", "ascii", "big5", "binary", "cp1250", "cp1251", "cp1256", "cp1257", "cp850",
                               "cp852", "cp866", "cp932", "dec8", "eucjpms", "euckr", "gb2312", "gbk", "geostd8", "greek",
                               "hebrew", "hp8", "keybcs2", "koi8r", "koi8u", "latin1", "latin2", "latin5", "latin7",
                               "macce", "macroman", "sjis", "swe7", "tis620", "ucs2", "ujis", "utf8" };
            }
        }

        public static string[] GetSortOrders(string charset)
        {
            switch (charset)
            {
                case "armscii8":
                    return new[] { "armscii8_bin", "armscii8_general_ci" };
                case "ascii":
                    return new[] { "ascii_bin", "ascii_general_ci" };
                case "big5":
                    return new[] { "big5_bin", "big5_chinese_ci" };
                case "binary":
                    return new[] { "binary" };
                case "cp1250":
                    return new[] { "cp1250_bin", "cp1250_croatian_ci", "cp1250_czech_cs", "cp1250_general_ci", "cp1250_polish_ci" };
                case "cp1251":
                    return new[] { "cp1251_bin", "cp1251_bulgarian_ci", "cp1251_general_ci", "cp1251_general_cs", "cp1251_ukrainian_ci" };
                case "cp1256":
                    return new[] { "cp1256_bin", "cp1256_general_ci" };
                case "cp1257":
                    return new[] { "cp1257_bin", "cp1257_general_ci", "cp1257_lithuanian_ci" };
                case "cp850":
                    return new[] { "cp850_bin", "cp850_general_ci" };
                case "cp852":
                    return new[] { "cp852_bin", "cp852_general_ci" };
                case "cp866":
                    return new[] { "cp866_bin", "cp866_general_ci" };
                case "cp932":
                    return new[] { "cp932_bin", "cp932_japanese_ci" };
                case "dec8":
                    return new[] { "dec8_bin", "dec8_swedish_ci" };
                case "eucjpms":
                    return new[] { "eucjpms_bin", "eucjpms_japanese_ci" };
                case "euckr":
                    return new[] { "euckr_bin", "euckr_korean_ci" };
                case "gb2312":
                    return new[] { "gb2312_bin", "gb2312_chinese_ci" };
                case "gbk":
                    return new[] { "gbk_bin", "gbk_chinese_ci" };
                case "geostd8":
                    return new[] { "geostd8_bin", "geostd8_general_ci" };
                case "greek":
                    return new[] { "greek_bin", "greek_general_ci" };
                case "hebrew":
                    return new[] { "hebrew_bin", "hebrew_general_ci" };
                case "hp8":
                    return new[] { "hp8_bin", "hp8_english_ci" };
                case "keybcs2":
                    return new[] { "keybcs2_bin", "keybcs2_general_ci" };
                case "koi8r":
                    return new[] { "koi8r_bin", "koi8r_general_ci" };
                case "koi8u":
                    return new[] { "koi8u_bin", "koi8u_general_ci" };
                case "latin1":
                    return new[] { "latin1_bin", "latin1_danish_ci", "latin1_general_ci", "latin1_general_cs", "latin1_german1_ci",
                                   "latin1_german2_ci", "latin1_spanish_ci", "latin1_swedish_ci" };
                case "latin2":
                    return new[] { "latin2_bin", "latin2_croatian_ci", "latin2_czech_cs", "latin2_general_ci", "latin2_hungarian_ci" };
                case "latin5":
                    return new[] { "latin5_bin", "latin5_turkish_ci" };
                case "latin7":
                    return new[] { "latin7_bin", "latin7_estonian_cs", "latin7_general_ci", "latin7_general_cs" };
                case "macce":
                    return new[] { "macce_bin", "macce_general_ci" };
                case "macroman":
                    return new[] { "macroman_bin", "macroman_general_ci" };
                case "sjis":
                    return new[] { "sjis_bin", "sjis_japanese_ci" };
                case "swe7":
                    return new[] { "swe7_bin", "swe7_swedish_ci" };
                case "tis620":
                    return new[] { "tis620_bin", "tis620_thai_ci" };
                case "ucs2":
                    return new[] { "ucs2_bin", "ucs2_czech_ci", "ucs2_danish_ci", "ucs2_esperanto_ci", "ucs2_estonian_ci",
                                   "ucs2_general_ci", "ucs2_hungarian_ci", "ucs2_icelandic_ci", "ucs2_latvian_ci",
                                   "ucs2_lithuanian_ci", "ucs2_persian_ci", "ucs2_polish_ci", "ucs2_roman_ci",
                                   "ucs2_romanian_ci", "ucs2_slovak_ci", "ucs2_slovenian_ci", "ucs2_spanish2_ci",
                                   "ucs2_spanish_ci", "ucs2_swedish_ci", "ucs2_turkish_ci", "ucs2_unicode_ci" };
                case "ujis":
                    return new[] { "ujis_bin", "ujis_japanese_ci" };
                case "utf8":
                    return new[] { "utf8_bin", "utf8_czech_ci", "utf8_danish_ci", "utf8_esperanto_ci", "utf8_estonian_ci",
                                   "utf8_general_ci", "utf8_hungarian_ci", "utf8_icelandic_ci", "utf8_latvian_ci",
                                   "utf8_lithuanian_ci", "utf8_persian_ci", "utf8_polish_ci", "utf8_roman_ci",
                                   "utf8_romanian_ci", "utf8_slovak_ci", "utf8_slovenian_ci", "utf8_spanish2_ci",
                                   "utf8_spanish_ci", "utf8_swedish_ci", "utf8_turkish_ci", "utf8_unicode_ci" };
                default:
                    return new[] {""};
            }
        }
    }
}
