using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects;

public class EmailTest
{
    [Fact]
    public void Email_ShouldCreateEmail_WhenValueIsValid()
    {
        // Arrange
        var validEmail = "test@example.com";

        // Act
        var email = Email.Create(validEmail);

        // Assert
        Assert.Equal(validEmail, email.Value);
    }

    [Fact]
    public void Email_ShouldThrowArgumentException_WhenValueIsNull()
    {
        // Arrange
        string? nullEmail = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Email.Create(nullEmail!)); // Force non-nullable for testing
        Assert.Contains("is required", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Email_ShouldThrowArgumentException_WhenValueHasSpaces()
    {
        // Arrange
        var emailWithSpaces = "test @example.com";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Email.Create(emailWithSpaces));
        Assert.Contains("cannot contain spaces", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Email_ShouldThrowArgumentException_WhenValueHasMultipleAtSymbols()
    {
        // Arrange
        var emailWithMultipleAt = "test@@example.com";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Email.Create(emailWithMultipleAt));
        Assert.Contains("format is invalid", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Email_ShouldThrowArgumentException_WhenValueHasNoAtSymbol()
    {
        // Arrange
        var emailWithNoAt = "testexample.com";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Email.Create(emailWithNoAt));
        Assert.Contains("format is invalid", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Email_ShouldThrowArgumentException_WhenValueHasInvalidFormat()
    {
        // Arrange
        var invalidEmail = "test@.com";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Email.Create(invalidEmail));
        Assert.Contains("format is invalid", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
