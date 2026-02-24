using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects;

public class PasswordHashTest
{
    private const int MinimumPasswordHashLength = 64;

    [Fact]
    public void PasswordHash_ShouldCreatePasswordHash_WhenValueIsValid()
    {
        // Arrange
        var validPasswordHash = "a".PadLeft(MinimumPasswordHashLength, 'a'); // Create a string of 64 characters

        // Act
        var passwordHash = PasswordHash.From(validPasswordHash);

        // Assert
        Assert.Equal(validPasswordHash, passwordHash.Value);
    }

    [Fact]
    public void PasswordHash_ShouldThrowException_WhenValueIsNull()
    {
        // Arrange
        string? passwordHash = default;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => PasswordHash.From(passwordHash!)); // Force non-nullable string
        Assert.Contains("is required", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void PasswordHash_ShouldThrowException_WhenValueIsNotLongerThan63Characters()
    {
        // Arrange
        var shortPasswordHash = "a".PadLeft(MinimumPasswordHashLength - 1, 'a'); // Create a string of 63 characters

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => PasswordHash.From(shortPasswordHash));
        Assert.Contains("must be at least", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void PasswordHash_ShouldThrowException_WhenValueHasWhitespace()
    {
        // Arrange
        var whitespacePasswordHash = "a a".PadLeft(MinimumPasswordHashLength, 'a'); // Create a string of 64 characters with whitespace

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => PasswordHash.From(whitespacePasswordHash));
        Assert.Contains("cannot contain whitespace", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void PasswordHash_ToString_ShouldReturnMaskedValue()
    {
        // Arrange
        var validPasswordHash = "a".PadLeft(MinimumPasswordHashLength, 'a'); // Create a string of 64 characters
        var passwordHash = PasswordHash.From(validPasswordHash);

        // Act
        var result = passwordHash.ToString();

        // Assert
        Assert.Equal("PasswordHash(***)", result);
    }
}
