using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects;

[Trait("Layer", "Domain")]
public class PhoneNumberTest
{
    [Fact]
    [Trait("PhoneNumber", "Create")]
    public void Create_ShouldCreatePhoneNumber_WhenValueIsValid()
    {
        // Arrange
        var validPhoneNumber = "+8298881212";

        // Act
        var phoneNumber = PhoneNumber.Create(validPhoneNumber);

        // Assert
        Assert.NotNull(phoneNumber);
        Assert.Equal("+8298881212", phoneNumber.Value);
    }

    [Fact]
    [Trait("PhoneNumber", "Create")]
    public void Create_ShouldNormalizePhoneNumber_WhenValueIsValidWithoutPlus()
    {
        // Arrange
        var validPhoneNumberWithoutPlus = "18298881212";

        // Act
        var phoneNumber = PhoneNumber.Create(validPhoneNumberWithoutPlus);

        // Assert
        Assert.NotNull(phoneNumber);
        Assert.Equal("+18298881212", phoneNumber.Value);
    }

    [Fact]
    [Trait("PhoneNumber", "Create")]
    public void Create_ShouldThrowException_WhenValueIsNull()
    {
        // Arrange
        string? nullPhoneNumber = default;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => PhoneNumber.Create(nullPhoneNumber!)); // Force non-nullable for testing
        Assert.Contains("is required", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [Trait("PhoneNumber", "Create")]
    public void Create_ShouldThrowException_WhenValueIsEmptyOrWhitespace(string input)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => PhoneNumber.Create(input));
        Assert.Contains("is required", exception.Message);
    }

    [Fact]
    [Trait("PhoneNumber", "Create")]
    public void Create_ShouldTrimValue()
    {
        var phoneNumber = PhoneNumber.Create("  +8298881212  ");

        Assert.Equal("+8298881212", phoneNumber.Value);
    }

    [Theory]
    [InlineData("abc123")]
    [InlineData("+0")]
    [InlineData("+1 8298881212")]
    [InlineData("+1-829-888-1212")]
    [Trait("PhoneNumber", "Create")]
    public void Create_ShouldThrowException_WhenValueIsInvalid(string input)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => PhoneNumber.Create(input));
        Assert.Contains("format is invalid", exception.Message);
    }

    [Fact]
    [Trait("PhoneNumber", "Create")]
    public void Create_ShouldReturnValue_WhenToStringIsCalled()
    {
        // Arrange
        var validPhoneNumber = "+8298881212";
        var phoneNumber = PhoneNumber.Create(validPhoneNumber);

        // Act
        var phoneNumberToString = phoneNumber.ToString();

        // Assert
        Assert.Equal("+8298881212", phoneNumberToString);
    }
}
