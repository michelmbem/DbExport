namespace DbExport.Tests;

public class UtilityTest
{
    [Fact]
    public void GetBytesTest()
    {
        // Arrange
        var value = @"\\\\banane \\340 gogo\n";
        byte[] expected = {92, 98, 97, 110, 97, 110, 101, 32, 224, 32, 103, 111, 103, 111, 10};

        // Act
        var actual = Utility.GetBytes(value);

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


    [Fact]
    public void FromBitStringTest()
    {
        // Arrange
        var value = "11111001011";
        byte[] expected = [7, 203];

        // Act
        var actual = Utility.FromBitString(value);

        // Assert
        Assert.Equal(BitConverter.ToString(expected), BitConverter.ToString(actual));
    }
}