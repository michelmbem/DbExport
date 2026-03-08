using DbExport.Providers;

namespace DbExport.Tests;

public class UtilityTest
{
    [Fact]
    public void ParseConnectionStringTest()
    {
        // Arrange
        const string connectionString = "Server=localhost;Database=test;User Id=sa;Password=secret;";
        
        // Act
        var actual = Utility.ParseConnectionString(connectionString);
        
        // Assert
        Assert.Equal(4, actual.Count);
        Assert.Equal("localhost", actual["server"]);
        Assert.Equal("test", actual["dataBase"]);
        Assert.Equal("sa", actual["USER ID"]);
        Assert.Equal("secret", actual["PassWord"]);
    }

    [Fact]
    public void TransformConnectionStringTest()
    {
        // Arrange
        const string connectionString = "Server=localhost;Database=test;User Id=sa;Password=secret;";
        const string expected = "Server=localhost;Database=test;User Id=???;Password=***";
        
        // Act
        var actual = Utility.TransformConnectionString(
            connectionString,
            (key, value) => key switch
            {
                "User Id" => "???",
                "Password" => "***",
                _ => value
            });
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SanitizeConnectionStringTest()
    {
        // Arrange
        const string connectionString = "Server=localhost;Database=test;User Id=sa;Password=secret;";
        const string expected = "Server=localhost;Database=test;User Id=sa;Password=******";
        
        // Act
        var actual = Utility.SanitizeConnectionString(connectionString);
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(ProviderNames.SQLSERVER, "Order Date", "[Order Date]")]
    [InlineData(ProviderNames.ORACLE, "Order Date", "\"Order Date\"")]
    [InlineData(ProviderNames.MYSQL, "Order Date", "`Order Date`")]
    [InlineData(ProviderNames.POSTGRESQL, "Order Date", "\"Order Date\"")]
    [InlineData(ProviderNames.SQLITE, "Order Date", "[Order Date]")]
    [InlineData(ProviderNames.FIREBIRD, "Order Date", "\"Order Date\"")]
    public void EscapeTest(string providerName, string name, string expected)
    {
        // Act
        var actual = Utility.Escape(name, providerName);
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(ProviderNames.SQLSERVER, "OrderId", "@OrderId")]
    [InlineData(ProviderNames.ORACLE, "OrderId", ":OrderId")]
    [InlineData(ProviderNames.MYSQL, "OrderId", "@OrderId")]
    [InlineData(ProviderNames.POSTGRESQL, "OrderId", "@OrderId")]
    [InlineData(ProviderNames.SQLITE, "OrderId", "@OrderId")]
    [InlineData(ProviderNames.FIREBIRD, "OrderId", ":OrderId")]
    public void ToParameterNameTest(string providerName, string name, string expected)
    {
        // Act
        var actual = Utility.ToParameterName(name, providerName);
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void EmptyDictionaryTest()
    {
        // Act
        var dict = Utility.EmptyDictionary<string>();
        
        // Assert
        Assert.Empty(dict);
        
        dict.Add("key", "value");
        Assert.Equal(dict["KEY"], dict["key"]);
        Assert.Equal(dict["Key"], dict["key"]);
        Assert.Equal(dict["kEy"], dict["key"]);
    }

    [Fact]
    public void SplitTest()
    {
        // Arrange
        const string value = "1, 2, ,3 , 4 , 5,";
        
        // Act
        var actual = Utility.Split(value, ',');
        
        // Assert
        Assert.Equal(["1", "2", "3", "4", "5"], actual);
    }

    [Fact]
    public void IsEmptyTest()
    {
        // Arrange
        (object? obj, bool expected)[] data =
        [
            (null, true),
            (0, false),
            ("", true),
            (Array.Empty<int>(), true),
            ("  ", true),
            (DateTime.Now, false),
            (new[] {2.0, 0.5}, false),
            (Convert.DBNull, true),
            ("Hello!!", false),
        ];
        
        // Assert
        foreach (var (obj, expected) in data)
            Assert.Equal(expected, Utility.IsEmpty(obj));
    }

    [Fact]
    public void IsBooleanTest()
    {
        // Arrange
        (object? obj, bool expected)[] data =
        [
            (null, false),
            (0, false),
            ("false", true),
            ('F', false),
            (false, true),
            (Convert.DBNull, false),
            ("TRUE", true),
        ];
        
        // Assert
        foreach (var (obj, expected) in data)
            Assert.Equal(expected, Utility.IsBoolean(obj, out _));
    }

    [Fact]
    public void IsNumericTest()
    {
        // Arrange
        (object? obj, bool expected)[] data =
        [
            (null, false),
            (10M, true),
            ("Yo!!", false),
            ('7', true),
            (false, false),
            (Convert.DBNull, false),
            ("1975e+13", true),
            (Array.Empty<float>(), false),
            ("76 Jolliet", false),
        ];
        
        // Assert
        foreach (var (obj, expected) in data)
            Assert.Equal(expected, Utility.IsNumeric(obj, out _));
    }

    [Fact]
    public void IsDateTest()
    {
        // Arrange
        (object? obj, bool expected)[] data =
        [
            (null, false),
            (.5, false),
            ("Yo!!", false),
            ('7', false),
            (false, false),
            (Convert.DBNull, false),
            ("1975-12-31", true),
            ("1975-12-31T14:30:51", true),
            ("18:15:33.719", true),
            (DateTime.Today, true),
            (Array.Empty<int>(), false),
        ];
        
        // Assert
        foreach (var (obj, expected) in data)
            Assert.Equal(expected, Utility.IsDate(obj, out _));
    }

    [Fact]
    public void ToByteTest()
    {
        // Arrange
        (object? obj, byte)[] data =
        [
            (null, 0),
            (10M, 10),
            ("Yo!!", 0),
            ('7', 7),
            (false, 0),
            (Convert.DBNull, 0),
            ("1.97e+2", 197),
            (Array.Empty<float>(), 0),
            ("76 Jolliet", 0),
        ];
        
        // Assert
        foreach (var (obj, expected) in data)
            Assert.Equal(expected, Utility.ToByte(obj));
    }

    [Fact]
    public void ToInt16Test()
    {
        // Arrange
        (object? obj, short)[] data =
        [
            (null, 0),
            (5232f, 5232),
            ("Hi dude!", 0),
            ('A', 0),
            (DBNull.Value, 0),
            ("0.333E+4", 3330),
            (new[] {7, 9, 0}, 0),
            ("0xFF1", 0),
        ];
        
        // Assert
        foreach (var (obj, expected) in data)
            Assert.Equal(expected, Utility.ToInt16(obj));
    }

    [Fact]
    public void QuotedStrTest()
    {
        // Arrange
        const string original = "It's a sunny day";
        const string expected = "'It''s a sunny day'";
        
        // Act
        var actual = Utility.QuotedStr(original);
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void UnquotedStrTest()
    {
        // Arrange
        const string original = "'It''s a sunny day'";
        const string expected = "It's a sunny day";
        
        // Act
        var actual = Utility.UnquotedStr(original);
        
        // Assert
        Assert.Equal(expected, actual);   
    }

    [Fact]
    public void UnquotedStrTest2()
    {
        // Arrange
        const string original = "\"The famous \"\"All-in-white\"\" party\"";
        const string expected = "The famous \"All-in-white\" party";
        
        // Act
        var actual = Utility.UnquotedStr(original, '"');
        
        // Assert
        Assert.Equal(expected, actual);   
    }

    [Fact]
    public void UnquotedStrTest3()
    {
        // Arrange
        const string original = "Sunny days";
        
        // Assert
        Assert.Equal(original, Utility.UnquotedStr(original));   
    }

    [Fact]
    public void BinToHexTest()
    {
        // Arrange
        var bytes = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15};
        const string expected = "0102030405060708090a0b0c0d0e0f";
        
        // Act
        var actual = Utility.BinToHex(bytes);
        
        // Assert
        Assert.Equal(expected, actual);   
    }

    [Fact]
    public void ToBitStringTest()
    {
        // Arrange
        var bytes = new byte[] {1, 2, 3};
        const string expected = "11011";
        
        // Act
        var actual = Utility.ToBitString(bytes);
        
        // Assert
        Assert.Equal(expected, actual);   
    }
    
    [Fact]
    public void FromBitStringTest()
    {
        // Arrange
        const string value = "11111001011";
        byte[] expected = [7, 203];

        // Act
        var actual = Utility.FromBitString(value);

        // Assert
        Assert.Equal(BitConverter.ToString(expected), BitConverter.ToString(actual));
    }

    [Theory]
    [InlineData("11000000", 2, 192)]
    [InlineData("FE", 16, 254)]
    [InlineData("143", 8, 99)]
    public void FromBaseNTest(string value, byte n, int expected)
    {
        // Act
        var actual = Utility.FromBaseN(value, n);

        // Assert
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData(192, 2, "11000000")]
    [InlineData(254, 16, "FE")]
    [InlineData(99, 8, "143")]
    public void ToBaseNTest(byte b, byte n, string expected)
    {
        // Act
        var actual = Utility.ToBaseN(b, n);

        // Assert
        Assert.Equal(expected, actual);
    }
}