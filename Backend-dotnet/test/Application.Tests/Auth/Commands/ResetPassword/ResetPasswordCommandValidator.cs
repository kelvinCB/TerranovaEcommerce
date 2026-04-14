using Application.Auth.Commands.ResetPassword;

namespace Application.Tests.Auth.Commands.ResetPassword;

[Trait("Layer", "Application")]
public sealed class ResetPasswordCommandValidatorTests
{
    private readonly ResetPasswordCommandValidator _validator = new();

    [Fact]
    [Trait("Auth", "Commands/ResetPasswordCommandValidator/Validate")]
    public void Validate_ShouldPass_WhenCommandIsValid()
    {
        var command = new ResetPasswordCommand("test@example.com", "123456", "NewPassword123");

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [Trait("Auth", "Commands/ResetPasswordCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenEmailIsEmpty(string email)
    {
        var command = new ResetPasswordCommand(email, "123456", "NewPassword123");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(ResetPasswordCommand.Email));
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("test")]
    [InlineData("test@")]
    [InlineData("@example.com")]
    [Trait("Auth", "Commands/ResetPasswordCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenEmailFormatIsInvalid(string email)
    {
        var command = new ResetPasswordCommand(email, "123456", "NewPassword123");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(ResetPasswordCommand.Email));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [Trait("Auth", "Commands/ResetPasswordCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenCodeIsEmpty(string code)
    {
        var command = new ResetPasswordCommand("test@example.com", code, "NewPassword123");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(ResetPasswordCommand.Code));
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("1234567")]
    [Trait("Auth", "Commands/ResetPasswordCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenCodeLengthIsInvalid(string code)
    {
        var command = new ResetPasswordCommand("test@example.com", code, "NewPassword123");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(ResetPasswordCommand.Code));
    }

    [Theory]
    [InlineData("12345a")]
    [InlineData("12 456")]
    [InlineData("ABCDEF")]
    [Trait("Auth", "Commands/ResetPasswordCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenCodeIsNotNumeric(string code)
    {
        var command = new ResetPasswordCommand("test@example.com", code, "NewPassword123");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(ResetPasswordCommand.Code));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [Trait("Auth", "Commands/ResetPasswordCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenNewPasswordIsEmpty(string newPassword)
    {
        var command = new ResetPasswordCommand("test@example.com", "123456", newPassword);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(ResetPasswordCommand.NewPassword));
    }
}