using Domain.ValueObjects;

namespace Application.Common.Abstractions.Services;

/// <summary>
/// Defines an interface for generating verification codes used in user verification processes such as password resets or email confirmations.
/// </summary>
public interface IVerificationCodeGenerator
{
    /// <summary>
    /// Generates a new verification code according to the defined rules and requirements (e.g., length, character set).
    /// </summary>
    /// <returns>Returns the generated verification code.</returns>
    Code Generate();
}