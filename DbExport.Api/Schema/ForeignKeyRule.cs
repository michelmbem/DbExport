namespace DbExport.Schema;

public enum ForeignKeyRule
{
    None,
    Restrict,
    Cascade,
    SetNull,
    SetDefault
}