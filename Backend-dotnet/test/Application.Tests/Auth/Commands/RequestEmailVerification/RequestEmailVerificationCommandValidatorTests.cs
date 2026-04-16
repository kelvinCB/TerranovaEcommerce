using Application.Auth.Commands.RequestEmailVerification;

namespace Application.Tests.Auth.Commands.RequestEmailVerification;

[Trait("Layer", "Application")]
public sealed class RequestEmailVerificationCommandValidatorTests
{
    private readonly RequestEmailVerificationCommandValidator _validator = new();

    [Fact]
    [Trait("Auth", "Commands/RequestEmailVerificationCommandValidator/Validate")]
    public void Validate_ShouldPass_WhenCommandIsValid()
    {
        var command = new RequestEmailVerificationCommand("test@example.com");

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [Trait("Auth", "Commands/RequestEmailVerificationCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenEmailIsEmpty(string email)
    {
        var command = new RequestEmailVerificationCommand(email);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(RequestEmailVerificationCommand.Email));
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("test")]
    [InlineData("test@")]
    [InlineData("@example.com")]
    [Trait("Auth", "Commands/RequestEmailVerificationCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenEmailFormatIsInvalid(string email)
    {
        var command = new RequestEmailVerificationCommand(email);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(RequestEmailVerificationCommand.Email));
    }
}
