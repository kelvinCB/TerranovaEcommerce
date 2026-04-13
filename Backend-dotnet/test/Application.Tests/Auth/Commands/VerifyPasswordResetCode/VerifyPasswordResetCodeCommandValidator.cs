using Application.Auth.Commands.VerifyPasswordResetCode;

namespace Application.Tests.Auth.Commands.VerifyPasswordResetCode;

[Trait("Layer", "Application")]
public sealed class VerifyPasswordResetCodeCommandValidatorTests
{
    private readonly VerifyPasswordResetCodeCommandValidator _validator = new();

    [Fact]
    [Trait("Auth", "Commands/VerifyPasswordResetCodeCommandValidator/Validate")]
    public void Validate_ShouldPass_WhenCommandIsValid()
    {
        var command = new VerifyPasswordResetCodeCommand("test@example.com", "123456");

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [Trait("Auth", "Commands/VerifyPasswordResetCodeCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenEmailIsEmpty(string email)
    {
        var command = new VerifyPasswordResetCodeCommand(email, "123456");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(VerifyPasswordResetCodeCommand.Email));
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("test")]
    [InlineData("test@")]
    [InlineData("@example.com")]
    [Trait("Auth", "Commands/VerifyPasswordResetCodeCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenEmailFormatIsInvalid(string email)
    {
        var command = new VerifyPasswordResetCodeCommand(email, "123456");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(VerifyPasswordResetCodeCommand.Email));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [Trait("Auth", "Commands/VerifyPasswordResetCodeCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenCodeIsEmpty(string code)
    {
        var command = new VerifyPasswordResetCodeCommand("test@example.com", code);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(VerifyPasswordResetCodeCommand.Code));
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("1234567")]
    [Trait("Auth", "Commands/VerifyPasswordResetCodeCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenCodeLengthIsInvalid(string code)
    {
        var command = new VerifyPasswordResetCodeCommand("test@example.com", code);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(VerifyPasswordResetCodeCommand.Code));
    }

    [Theory]
    [InlineData("12345a")]
    [InlineData("12 456")]
    [InlineData("ABCDEF")]
    [Trait("Auth", "Commands/VerifyPasswordResetCodeCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenCodeContainsNonDigits(string code)
    {
        var command = new VerifyPasswordResetCodeCommand("test@example.com", code);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(VerifyPasswordResetCodeCommand.Code));
    }
}