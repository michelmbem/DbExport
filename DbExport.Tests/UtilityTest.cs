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


    [Fact]
    public void FromBaseNTest()
    {
        // Arrange
        var value = "1010";
        byte n = 2;
        byte expected = 10;

        // Act
        var actual = Utility.FromBaseN(value, n);

        Assert.Equal(expected, actual);
    }


    [Fact]
    public void ToBaseNTest()
    {
        // Arrange
        byte b = 192;
        byte n = 2;
        var expected = "11000000";

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
        byte[] expected = {7, 203};

        // Act
        var actual = Utility.FromBitString(value);

        // Assert
        Assert.Equal(BitConverter.ToString(expected), BitConverter.ToString(actual));
    }
}