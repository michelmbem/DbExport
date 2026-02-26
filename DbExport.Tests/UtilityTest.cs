namespace DbExport.Tests;

public class UtilityTest
{
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