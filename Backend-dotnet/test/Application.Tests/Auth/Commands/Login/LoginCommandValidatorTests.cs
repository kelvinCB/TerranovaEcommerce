using Application.Auth.Commands.Login;

namespace Application.Tests.Auth.Commands.Login;

[Trait("Layer", "Application")]
public sealed class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    [Trait("Auth", "Commands/LoginCommandValidator/Validate")]
    public void Validate_ShouldPass_WhenCommandIsValid()
    {
        var command = new LoginCommand("test@example.com", "TestPassword123");

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [Trait("Auth", "Commands/LoginCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenEmailAddressIsEmpty(string emailAddress)
    {
        var command = new LoginCommand(emailAddress, "TestPassword123");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(LoginCommand.EmailAddress));
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("test")]
    [InlineData("test@")]
    [InlineData("@example.com")]
    [Trait("Auth", "Commands/LoginCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenEmailAddressFormatIsInvalid(string emailAddress)
    {
        var command = new LoginCommand(emailAddress, "TestPassword123");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(LoginCommand.EmailAddress));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [Trait("Auth", "Commands/LoginCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenPasswordIsEmpty(string password)
    {
        var command = new LoginCommand("test@example.com", password);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(LoginCommand.Password));
    }
}