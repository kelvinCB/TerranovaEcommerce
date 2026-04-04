using Application.Auth.Commands.ChangePassword;

namespace Application.Tests.Auth.Commands.ChangePassword;

[Trait("Layer", "Application")]
public sealed class ChangePasswordCommandValidatorTests
{
    private readonly ChangePasswordCommandValidator _validator = new();

    [Fact]
    [Trait("Auth", "Commands/ChangePasswordCommandValidator/Validate")]
    public void Validate_ShouldPass_WhenCommandIsValid()
    {
        var command = new ChangePasswordCommand(
            Ulid.NewUlid(),
            "CurrentPassword123",
            "NewPassword123");

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    [Trait("Auth", "Commands/ChangePasswordCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenUserIdIsEmpty()
    {
        var command = new ChangePasswordCommand(
            Ulid.Empty,
            "CurrentPassword123",
            "NewPassword123");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(ChangePasswordCommand.UserId));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [Trait("Auth", "Commands/ChangePasswordCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenCurrentPasswordIsEmpty(string currentPassword)
    {
        var command = new ChangePasswordCommand(
            Ulid.NewUlid(),
            currentPassword,
            "NewPassword123");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(ChangePasswordCommand.CurrentPassword));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [Trait("Auth", "Commands/ChangePasswordCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenNewPasswordIsEmpty(string newPassword)
    {
        var command = new ChangePasswordCommand(
            Ulid.NewUlid(),
            "CurrentPassword123",
            newPassword);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(ChangePasswordCommand.NewPassword));
    }

    [Fact]
    [Trait("Auth", "Commands/ChangePasswordCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenNewPasswordIsEqualToCurrentPassword()
    {
        var command = new ChangePasswordCommand(
            Ulid.NewUlid(),
            "SamePassword123",
            "SamePassword123");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(ChangePasswordCommand.NewPassword));
    }

    [Fact]
    [Trait("Auth", "Commands/ChangePasswordCommandValidator/Validate")]
    public void Validate_ShouldFail_WhenMultipleFieldsAreInvalid()
    {
        var command = new ChangePasswordCommand(
            Ulid.Empty,
            string.Empty,
            string.Empty);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(ChangePasswordCommand.UserId));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(ChangePasswordCommand.CurrentPassword));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(ChangePasswordCommand.NewPassword));
    }
}