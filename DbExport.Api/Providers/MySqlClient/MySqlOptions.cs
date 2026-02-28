namespace DbExport.Providers.MySqlClient;

public sealed class CharacterSet(string name, string[] collations, string defaultCollation)
{
    public string Name { get; } = name;

    public string[] Collations { get; } = collations;

    public string DefaultCollation { get; } = defaultCollation;

    public override bool Equals(object obj) => obj is CharacterSet other && Name == other.Name;

    public override int GetHashCode() => Name.GetHashCode();

    public override string ToString() => Name;
}

public class MySqlOptions
{
    public string StorageEngine { get; set; }

    public CharacterSet CharacterSet { get; set; }

    public string Collation { get; set; }

    public bool IsMariaDb { get; set; }

    #region Collections

    public static string[] StorageEngines { get; } =
    [
        "InnoDB",
        "MyISAM",
        "MEMORY",
        "CSV",
        "ARCHIVE",
        "BLACKHOLE",
        "FEDERATED",
        "MERGE",
        "NDB"
    ];

    public static CharacterSet[] CharacterSets { get; } =
    [
        new(
            "armscii8",
            ["armscii8_bin", "armscii8_general_ci"],
            "armscii8_general_ci"
        ),
        new(
            "ascii",
            ["ascii_bin", "ascii_general_ci"],
            "ascii_general_ci"
        ),
        new(
            "big5",
            ["big5_bin", "big5_chinese_ci"],
            "big5_chinese_ci"
        ),
        new(
            "binary",
            ["binary"],
            "binary"
        ),
        new(
            "cp1250",
            [
                "cp1250_bin",
                "cp1250_croatian_ci",
                "cp1250_czech_cs",
                "cp1250_general_ci",
                "cp1250_polish_ci"
            ],
            "cp1250_general_ci"
        ),
        new(
            "cp1251",
            [
                "cp1251_bin",
                "cp1251_bulgarian_ci",
                "cp1251_general_ci",
                "cp1251_general_cs",
                "cp1251_ukrainian_ci"
            ],
            "cp1251_general_ci"
        ),
        new(
            "cp1256",
            ["cp1256_bin", "cp1256_general_ci"],
            "cp1256_general_ci"
        ),
        new(
            "cp1257",
            ["cp1257_bin", "cp1257_general_ci", "cp1257_lithuanian_ci"],
            "cp1257_general_ci"
        ),
        new(
            "cp850",
            ["cp850_bin", "cp850_general_ci"],
            "cp850_general_ci"
        ),
        new(
            "cp852",
            ["cp852_bin", "cp852_general_ci"],
            "cp852_general_ci"
        ),
        new(
            "cp866",
            ["cp866_bin", "cp866_general_ci"],
            "cp866_general_ci"
        ),
        new(
            "cp932",
            ["cp932_bin", "cp932_japanese_ci"],
            "cp932_japanese_ci"
        ),
        new(
            "dec8",
            ["dec8_bin", "dec8_swedish_ci"],
            "dec8_swedish_ci"
        ),
        new(
            "eucjpms",
            ["eucjpms_bin", "eucjpms_japanese_ci"],
            "eucjpms_japanese_ci"
        ),
        new(
            "euckr",
            ["euckr_bin", "euckr_korean_ci"],
            "euckr_korean_ci"
        ),
        new(
            "gb18030",
            ["gb18030_bin", "gb18030_chinese_ci", "gb18030_unicode_520_ci"],
            "gb18030_unicode_520_ci"
        ),
        new(
            "gb2312",
            ["gb2312_bin", "gb2312_chinese_ci"],
            "gb2312_chinese_ci"
        ),
        new(
            "gbk",
            ["gbk_bin", "gbk_chinese_ci"],
            "gbk_chinese_ci"
        ),
        new(
            "geostd8",
            ["geostd8_bin", "geostd8_general_ci"],
            "geostd8_general_ci"
        ),
        new(
            "greek",
            ["greek_bin", "greek_general_ci"],
            "greek_general_ci"
        ),
        new(
            "hebrew",
            ["hebrew_bin", "hebrew_general_ci"],
            "hebrew_general_ci"
        ),
        new(
            "hp8",
            ["hp8_bin", "hp8_english_ci"],
            "hp8_english_ci"
        ),
        new(
            "keybcs2",
            ["keybcs2_bin", "keybcs2_general_ci"],
            "keybcs2_general_ci"
        ),
        new(
            "koi8r",
            ["koi8r_bin", "koi8r_general_ci"],
            "koi8r_general_ci"
        ),
        new(
            "koi8u",
            ["koi8u_bin", "koi8u_general_ci"],
            "koi8u_general_ci"
        ),
        new(
            "latin1",
            [
                "latin1_bin",
                "latin1_danish_ci",
                "latin1_general_ci",
                "latin1_general_cs",
                "latin1_german1_ci",
                "latin1_german2_ci",
                "latin1_spanish_ci",
                "latin1_swedish_ci"
            ],
            "latin1_swedish_ci"
        ),
        new(
            "latin2",
            [
                "latin2_bin",
                "latin2_croatian_ci",
                "latin2_czech_cs",
                "latin2_general_ci",
                "latin2_hungarian_ci"
            ],
            "latin2_general_ci"
        ),
        new(
            "latin5",
            ["latin5_bin", "latin5_turkish_ci"],
            "latin5_turkish_ci"
        ),
        new(
            "latin7",
            [
                "latin7_bin",
                "latin7_estonian_cs",
                "latin7_general_ci",
                "latin7_general_cs"
            ],
            "latin7_general_ci"
        ),
        new(
            "macce",
            ["macce_bin", "macce_general_ci"],
            "macce_general_ci"
        ),
        new(
            "macroman",
            ["macroman_bin", "macroman_general_ci"],
            "macroman_general_ci"
        ),
        new(
            "sjis",
            ["sjis_bin", "sjis_japanese_ci"],
            "sjis_japanese_ci"
        ),
        new(
            "swe7",
            ["swe7_bin", "swe7_swedish_ci"],
            "swe7_swedish_ci"
        ),
        new(
            "tis620",
            ["tis620_bin", "tis620_thai_ci"],
            "tis620_thai_ci"
        ),
        new(
            "ucs2",
            [
                "ucs2_bin",
                "ucs2_croatian_ci",
                "ucs2_czech_ci",
                "ucs2_danish_ci",
                "ucs2_esperanto_ci",
                "ucs2_estonian_ci",
                "ucs2_general_ci",
                "ucs2_general_mysql500_ci",
                "ucs2_german2_ci",
                "ucs2_hungarian_ci",
                "ucs2_icelandic_ci",
                "ucs2_latvian_ci",
                "ucs2_lithuanian_ci",
                "ucs2_persian_ci",
                "ucs2_polish_ci",
                "ucs2_roman_ci",
                "ucs2_romanian_ci",
                "ucs2_sinhala_ci",
                "ucs2_slovak_ci",
                "ucs2_slovenian_ci",
                "ucs2_spanish_ci",
                "ucs2_spanish2_ci",
                "ucs2_swedish_ci",
                "ucs2_turkish_ci",
                "ucs2_unicode_ci"
            ],
            "ucs2_unicode_ci"
        ),
        new(
            "utf8",
            [
                "utf8_bin",
                "utf8_croatian_ci",
                "utf8_czech_ci",
                "utf8_danish_ci",
                "utf8_esperanto_ci",
                "utf8_estonian_ci",
                "utf8_general_ci",
                "utf8_general_mysql500_ci",
                "utf8_german2_ci",
                "utf8_hungarian_ci",
                "utf8_icelandic_ci",
                "utf8_latvian_ci",
                "utf8_lithuanian_ci",
                "utf8_persian_ci",
                "utf8_polish_ci",
                "utf8_roman_ci",
                "utf8_romanian_ci",
                "utf8_sinhala_ci",
                "utf8_slovak_ci",
                "utf8_slovenian_ci",
                "utf8_spanish_ci",
                "utf8_spanish2_ci",
                "utf8_swedish_ci",
                "utf8_turkish_ci",
                "utf8_unicode_ci"
            ],
            "utf8_general_ci"
        ),
        new(
            "utf8mb3",
            ["utf8mb3_bin", "utf8mb3_general_ci", "utf8mb3_unicode_ci"],
            "utf8mb3_general_ci"
        ),
        new(
            "utf8mb4",
            [
                "utf8mb4_bin",
                "utf8mb4_0900_ai_ci",
                "utf8mb4_0900_as_ci",
                "utf8mb4_0900_as_cs",
                "utf8mb4_0900_bin",
                "utf8mb4_general_ci",
                "utf8mb4_unicode_ci"
            ],
            "utf8mb4_0900_ai_ci"
        )
    ];

    #endregion

    public string ToMarkdown() => $"""
                                  | Property | Value |
                                  |----------|-------|
                                  | Storage Engine | {StorageEngine} |
                                  | Character Set | {CharacterSet} |
                                  | Collation | {Collation} |
                                  | Optimize SQL for MariaDB | {(IsMariaDb ? "Yes" : "No")} |
                                  """;
}