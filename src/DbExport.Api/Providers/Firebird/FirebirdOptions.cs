namespace DbExport.Providers.Firebird;

/// <summary>
/// Represents configuration options for Firebird database operations.
/// </summary>
public class FirebirdOptions
{
    /// <summary>
    /// Gets or sets the file system path to the directory where Firebird database files
    /// will be created and stored. This property is essential for specifying the
    /// location of the database files during database operations.
    /// </summary>
    public string DataDirectory { get; set; }
    
    /// <summary>
    /// Gets or sets the default character set to be used for encoding text data.
    /// </summary>
    public string DefaultCharSet { get; set; }
    
    /// <summary>
    /// Gets or sets the page size for writing data to disk.
    /// </summary>
    public static int PageSize { get; set; } = 4096;
    
    /// <summary>
    /// Gets or sets a value indicating whether to force writes to disk.
    /// </summary>
    public static bool ForcedWrites { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to overwrite existing files when exporting data.
    /// </summary>
    public static bool Overwrite { get; set; }

    #region Collections

    /// <summary>
    /// Gets a list of supported character sets for Firebird databases.
    /// </summary>
    public static string[] CharacterSets { get; } =
    [
        // Unicode
        "UTF8",
        "UTF16",
        "UNICODE_FSS",

        // ASCII / Special
        "ASCII",
        "NONE",
        "OCTETS",
        "NEXT",
        "CYRL",
        "KOI8R",
        "KOI8U",
        "TIS620",

        // ISO-8859
        "ISO8859_1",
        "ISO8859_2",
        "ISO8859_3",
        "ISO8859_4",
        "ISO8859_5",
        "ISO8859_6",
        "ISO8859_7",
        "ISO8859_8",
        "ISO8859_9",
        "ISO8859_13",
        "ISO8859_15",

        // Windows code pages
        "WIN1250",
        "WIN1251",
        "WIN1252",
        "WIN1253",
        "WIN1254",
        "WIN1255",
        "WIN1256",
        "WIN1257",

        // DOS code pages
        "DOS437",
        "DOS737",
        "DOS775",
        "DOS850",
        "DOS852",
        "DOS857",
        "DOS858",
        "DOS860",
        "DOS861",
        "DOS862",
        "DOS863",
        "DOS864",
        "DOS865",
        "DOS866",
        "DOS869",

        // Asian
        "GB_2312",
        "GBK",
        "BIG_5",
        "SJIS_0208",
        "EUCJ_0208",
        "KSC_5601"
    ];
    
    #endregion

    /// <summary>
    /// Converts the FirebirdOptions properties and their current values into a Markdown table representation.
    /// </summary>
    /// <returns>
    /// A string containing a Markdown-formatted table with the FirebirdOptions properties and their values.
    /// </returns>
    public string ToMarkdown() => $"""
                                   | Property | Value |
                                   |----------|-------|
                                   | Data directory | {DataDirectory} |
                                   | Default character set | {DefaultCharSet} |
                                   | Page size | {PageSize} |
                                   | Forced writes | {(ForcedWrites ? "Yes" : "No")} |
                                   | Overwrite file if it exists | {(Overwrite ? "Yes" : "No")} |
                                   """;
}