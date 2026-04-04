using Application.Auth.Commands.RequestPasswordReset;

namespace Application.Tests.Auth.Commands.RequestPasswordReset;

[Trait("Layer", "Application")]
public sealed class RequestPasswordResetCommandValidatorTests
{
    private readonly RequestPasswordResetCommandValidator _validator = new();

    [Fact]
    [Trait("Auth", "Commands/RequestPasswordResetCommandValidator/Validate")]
    public void Validate_ShouldPass_WhenCommandIsValid()
    {
        var command = new RequestPasswordResetCommand("test@example.com");

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [Trait("Auth", "Commands/RequestPasswordResetCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenEmailAddressIsEmpty(string emailAddress)
    {
        var command = new RequestPasswordResetCommand(emailAddress);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(RequestPasswordResetCommand.EmailAddress));
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("test")]
    [InlineData("test@")]
    [InlineData("@example.com")]
    [Trait("Auth", "Commands/RequestPasswordResetCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenEmailAddressFormatIsInvalid(string emailAddress)
    {
        var command = new RequestPasswordResetCommand(emailAddress);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(RequestPasswordResetCommand.EmailAddress));
    }
}
