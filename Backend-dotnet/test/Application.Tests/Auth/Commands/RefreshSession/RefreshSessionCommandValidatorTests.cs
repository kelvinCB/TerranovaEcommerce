using Application.Auth.Commands.RefreshSession;

namespace Application.Tests.Auth.Commands.RefreshSession;

[Trait("Layer", "Application")]
public sealed class RefreshSessionCommandValidatorTests
{
    private readonly RefreshSessionCommandValidator _validator = new();

    [Fact]
    [Trait("Auth", "Commands/RefreshSessionCommandValidator/Validate")]
    public void Validate_ShouldPass_WhenCommandIsValid()
    {
        var command = new RefreshSessionCommand("valid-refresh-token");

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [Trait("Auth", "Commands/RefreshSessionCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenRefreshTokenIsEmpty(string refreshToken)
    {
        var command = new RefreshSessionCommand(refreshToken);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(RefreshSessionCommand.RefreshToken));
    }

    [Fact]
    [Trait("Auth", "Commands/RefreshSessionCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenRefreshTokenExceedsMaximumLength()
    {
        var command = new RefreshSessionCommand(new string('a', 513));

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(RefreshSessionCommand.RefreshToken));
    }
}