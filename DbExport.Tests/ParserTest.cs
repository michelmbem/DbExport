using DbExport.Providers.SQLite.SqlParser;
using DbExport.Schema;

namespace DbExport.Tests;

public class ParserTest
{
	[Fact]
    public void TestCreateTable()
    {
        // Arrange
        var input = """
            CREATE TABLE city (
            	ID integer NOT NULL UNIQUE,
            	Name char(35) NOT NULL DEFAULT '',
            	CountryCode char(3) NOT NULL DEFAULT '',
            	District char(20) NOT NULL DEFAULT '',
            	Population integer NOT NULL,
            	PRIMARY KEY (ID),
            	CONSTRAINT city_country_fk FOREIGN KEY (CountryCode) REFERENCES country (Code)
            )
            """;

        // Act
        var parser = new Parser(new Scanner(input));
        var node = parser.CreateTable();

        // Assert
        Assert.Equal(AstNodeKind.CREATE_TBL, node.Kind);
        Assert.Equal(3, node.Children.Count);
        Assert.Equal("city", node.Data);
        
        var colSpecList = node.Children[0];
        Assert.Equal(AstNodeKind.COLSPECLIST, colSpecList.Kind);
        Assert.Equal(5, colSpecList.Children.Count);
        
        var colSpec = colSpecList.Children[0];
        Assert.Equal(AstNodeKind.COLSPEC, colSpec.Kind);
        
        var colAttribs = Assert.IsType<Dictionary<string, object>>(colSpec.Data);
        Assert.Equal("ID", colAttribs["COLUMN_NAME"]);
        Assert.Equal("integer", colAttribs["TYPE_NAME"]);
        Assert.Equal(false, colAttribs["ALLOW_DBNULL"]);
        Assert.Equal(true, colAttribs["UNIQUE"]);
        
        colSpec = colSpecList.Children[1];
        Assert.Equal(AstNodeKind.COLSPEC, colSpec.Kind);
        
        colAttribs = Assert.IsType<Dictionary<string, object>>(colSpec.Data);
        Assert.Equal("Name", colAttribs["COLUMN_NAME"]);
        Assert.Equal("char", colAttribs["TYPE_NAME"]);
        Assert.Equal(35, colAttribs["PRECISION"]);
        Assert.Equal(false, colAttribs["ALLOW_DBNULL"]);
        Assert.Equal("", colAttribs["DEFAULT_VALUE"]);
        
        colSpec = colSpecList.Children[2];
        Assert.Equal(AstNodeKind.COLSPEC, colSpec.Kind);
        
        colAttribs = Assert.IsType<Dictionary<string, object>>(colSpec.Data);
        Assert.Equal("CountryCode", colAttribs["COLUMN_NAME"]);
        Assert.Equal("char", colAttribs["TYPE_NAME"]);
        Assert.Equal(3, colAttribs["PRECISION"]);
        Assert.Equal(false, colAttribs["ALLOW_DBNULL"]);
        Assert.Equal("", colAttribs["DEFAULT_VALUE"]);
        
        colSpec = colSpecList.Children[3];
        Assert.Equal(AstNodeKind.COLSPEC, colSpec.Kind);
        
        colAttribs = Assert.IsType<Dictionary<string, object>>(colSpec.Data);
        Assert.Equal("District", colAttribs["COLUMN_NAME"]);
        Assert.Equal("char", colAttribs["TYPE_NAME"]);
        Assert.Equal(20, colAttribs["PRECISION"]);
        Assert.Equal(false, colAttribs["ALLOW_DBNULL"]);
        Assert.Equal("", colAttribs["DEFAULT_VALUE"]);
        
        colSpec = colSpecList.Children[4];
        Assert.Equal(AstNodeKind.COLSPEC, colSpec.Kind);
        
        colAttribs = Assert.IsType<Dictionary<string, object>>(colSpec.Data);
        Assert.Equal("Population", colAttribs["COLUMN_NAME"]);
        Assert.Equal("integer", colAttribs["TYPE_NAME"]);
        Assert.Equal(false, colAttribs["ALLOW_DBNULL"]);
        
        var pkSpec = node.Children[1];
        Assert.Equal(AstNodeKind.PKSPEC, pkSpec.Kind);
        Assert.Null(pkSpec.Data);
        
        var pkColumns = Assert.Single(pkSpec.Children);
        Assert.Equal(AstNodeKind.COLREFLIST, pkColumns.Kind);
        
        var pkColumn = Assert.Single(pkColumns.Children);
        Assert.Equal(AstNodeKind.COLREF, pkColumn.Kind);
        Assert.Equal("ID", pkColumn.Data);
        Assert.Empty(pkColumn.Children);
        
        var fkSpec = node.Children[2];
        Assert.Equal(AstNodeKind.FKSPEC, fkSpec.Kind);
        
        var fkAttribs = Assert.IsType<Dictionary<string, object>>(fkSpec.Data);
        Assert.Equal("city_country_fk", fkAttribs["CONSTRAINT_NAME"]);
        Assert.Equal("country", fkAttribs["TARGET_TABLE_NAME"]);
        Assert.Equal(ForeignKeyRule.None, fkAttribs["UPDATE_RULE"]);
        Assert.Equal(ForeignKeyRule.None, fkAttribs["DELETE_RULE"]);
        Assert.Equal(2, fkSpec.Children.Count);
        
        var fkColumns = fkSpec.Children[0];
        Assert.Equal(AstNodeKind.COLREFLIST, fkColumns.Kind);
        
        var fkColumn = Assert.Single(fkColumns.Children);
        Assert.Equal(AstNodeKind.COLREF, fkColumn.Kind);
        Assert.Equal("CountryCode", fkColumn.Data);
        Assert.Empty(fkColumn.Children);
        
        pkColumns = fkSpec.Children[1];
        Assert.Equal(AstNodeKind.COLREFLIST, pkColumns.Kind);
        
        pkColumn = Assert.Single(pkColumns.Children);
        Assert.Equal(AstNodeKind.COLREF, pkColumn.Kind);
        Assert.Equal("Code", pkColumn.Data);
        Assert.Empty(pkColumn.Children);
    }
}
