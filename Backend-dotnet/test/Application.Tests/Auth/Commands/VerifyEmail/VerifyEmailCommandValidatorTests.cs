using Application.Auth.Commands.VerifyEmail;

namespace Application.Tests.Auth.Commands.VerifyEmail;

[Trait("Layer", "Application")]
public sealed class VerifyEmailCommandValidatorTests
{
    private readonly VerifyEmailCommandValidator _validator = new();

    [Fact]
    [Trait("Auth", "Commands/VerifyEmailCommandValidator/Validate")]
    public void Validate_ShouldPass_WhenCommandIsValid()
    {
        var command = new VerifyEmailCommand("test@example.com", "123456");

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [Trait("Auth", "Commands/VerifyEmailCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenEmailIsEmpty(string email)
    {
        var command = new VerifyEmailCommand(email, "123456");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(VerifyEmailCommand.Email));
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("test")]
    [InlineData("test@")]
    [InlineData("@example.com")]
    [Trait("Auth", "Commands/VerifyEmailCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenEmailFormatIsInvalid(string email)
    {
        var command = new VerifyEmailCommand(email, "123456");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(VerifyEmailCommand.Email));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [Trait("Auth", "Commands/VerifyEmailCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenCodeIsEmpty(string code)
    {
        var command = new VerifyEmailCommand("test@example.com", code);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(VerifyEmailCommand.Code));
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("1234567")]
    [Trait("Auth", "Commands/VerifyEmailCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenCodeLengthIsInvalid(string code)
    {
        var command = new VerifyEmailCommand("test@example.com", code);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(VerifyEmailCommand.Code));
    }

    [Theory]
    [InlineData("12345a")]
    [InlineData("12 456")]
    [InlineData("ABCDEF")]
    [Trait("Auth", "Commands/VerifyEmailCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenCodeContainsNonDigits(string code)
    {
        var command = new VerifyEmailCommand("test@example.com", code);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(VerifyEmailCommand.Code));
    }
}
