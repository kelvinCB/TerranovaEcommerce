using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects;

[Trait("Layer", "Domain")]
public class CodeTest
{
    [Fact]
    [Trait("Code", "From")]
    public void From_ShouldCreateCode_WhenValueIsValid()
    {
        // Arrange
        var validCode = "123456";

        // Act
        var code = Code.From(validCode);

        // Assert
        Assert.NotNull(code);
        Assert.Equal(validCode, code.Value);
    }

    [Fact]
    [Trait("Code", "From")]
    public void From_ShouldThrowException_WhenValueIsNull()
    {
        // Act and Assert
        var exception = Assert.Throws<ArgumentException>(() => Code.From(null!)); // Force non-nullable for testing

        Assert.Contains("Cannot be null", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("Code", "From")]
    public void From_ShouldThrowException_WhenValueIsWhitespace()
    {
        // Act and Assert
        var exception = Assert.Throws<ArgumentException>(() => Code.From("  "));

        Assert.Contains("Cannot be null or whitespace", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("12345 ")]
    [InlineData(" 1234  ")]
    [Trait("Code", "From")]
    public void From_ShouldThrowException_WhenValueHasInvalidLength(string invalidCode)
    {
        // Act and Assert
        var exception = Assert.Throws<ArgumentException>(() => Code.From(invalidCode));

        Assert.Contains($"The code must be at least", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("Code", "From")]
    public void From_ShouldThrowException_WhenValueContainsWhitespace()
    {
        // Act and Assert
        var exception = Assert.Throws<ArgumentException>(() => Code.From("123 456"));

        Assert.Contains("Cannot contain whitespace characters", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("Code", "ToString")]
    public void ToString_ShouldReturnCensoredCodeValue()
    {
        // Arrange
        var code = Code.From("123456");

        // Act
        var result = code.ToString();

        // Assert
        Assert.Equal("Code(****)", result);
    }
    
}