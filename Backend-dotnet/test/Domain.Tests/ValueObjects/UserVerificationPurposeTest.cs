using System.Reflection;
using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects;

[Trait("Layer", "Domain")]
public sealed class UserVerificationPurposeTest
{
    [Fact]
    [Trait("UserVerificationPurpose", "StaticProperties")]
    public void StaticProperties_ShouldExposeExpectedValues()
    {
        Assert.Equal("email_verify", UserVerificationPurpose.EmailVerify.Value);
        Assert.Equal("phone_verify", UserVerificationPurpose.PhoneVerify.Value);
        Assert.Equal("password_reset", UserVerificationPurpose.PasswordReset.Value);
        Assert.Equal("email_change", UserVerificationPurpose.EmailChange.Value);
    }

    [Fact]
    [Trait("UserVerificationPurpose", "ToString")]
    public void ToString_ShouldReturnRawValue()
    {
        var purpose = UserVerificationPurpose.PasswordReset;

        var result = purpose.ToString();

        Assert.Equal("password_reset", result);
    }

    [Fact]
    [Trait("UserVerificationPurpose", "Equality")]
    public void Equality_ShouldBeTrue_WhenPurposesHaveTheSameValue()
    {
        var left = UserVerificationPurpose.EmailVerify;
        var right = CreatePurpose("email_verify");

        Assert.Equal(left, right);
        Assert.True(left == right);
    }

    [Fact]
    [Trait("UserVerificationPurpose", "Equality")]
    public void Equality_ShouldBeFalse_WhenPurposesHaveDifferentValues()
    {
        var left = UserVerificationPurpose.EmailVerify;
        var right = UserVerificationPurpose.PasswordReset;

        Assert.NotEqual(left, right);
        Assert.True(left != right);
    }

    [Fact]
    [Trait("UserVerificationPurpose", "Constructor")]
    public void Constructor_ShouldTrimValue_WhenValueHasLeadingOrTrailingWhitespace()
    {
        var purpose = CreatePurpose("  password_reset  ");

        Assert.Equal("password_reset", purpose.Value);
    }

    [Fact]
    [Trait("UserVerificationPurpose", "Constructor")]
    public void Constructor_ShouldThrowException_WhenValueIsNull()
    {
        var exception = Assert.Throws<ArgumentException>(() => CreatePurpose(null!));

        Assert.Contains("cannot be null or whitespace", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [Trait("UserVerificationPurpose", "Constructor")]
    public void Constructor_ShouldThrowException_WhenValueIsEmptyOrWhitespace(string value)
    {
        var exception = Assert.Throws<ArgumentException>(() => CreatePurpose(value));

        Assert.Contains("cannot be null or whitespace", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static UserVerificationPurpose CreatePurpose(string value)
    {
        var constructor = typeof(UserVerificationPurpose).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            types: [typeof(string)],
            modifiers: null);

        Assert.NotNull(constructor);

        try
        {
            return (UserVerificationPurpose)constructor.Invoke([value]);
        }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            throw exception.InnerException;
        }
    }
}