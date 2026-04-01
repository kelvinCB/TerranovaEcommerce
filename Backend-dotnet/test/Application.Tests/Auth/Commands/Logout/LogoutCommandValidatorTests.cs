using Application.Auth.Commands.Logout;

namespace Application.Tests.Auth.Commands.Logout;

[Trait("Layer", "Application")]
public sealed class LogoutCommandValidatorTests
{
    private readonly LogoutCommandValidator _validator = new();

    [Fact]
    [Trait("Auth", "Commands/Logout/LogoutCommandValidator/Validate")]
    public void Validate_ShouldPass_WhenCommandIsValid()
    {
        var command = new LogoutCommand("plain-refresh-token");

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [Trait("Auth", "Commands/Logout/LogoutCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenRefreshTokenIsEmpty(string refreshToken)
    {
        var command = new LogoutCommand(refreshToken);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(LogoutCommand.RefreshToken));
    }

    [Fact]
    [Trait("Auth", "Commands/Logout/LogoutCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenRefreshTokenExceedsMaximumLength()
    {
        var command = new LogoutCommand(new string('a', 513));

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(LogoutCommand.RefreshToken));
    }
}