using Domain.ValueObjects;

namespace Application.Common.Verification;

/// <summary>
/// Represents a generated verification code, containing both the code value and its plain text representation.
/// </summary>
/// <param name="Code">The generated verification code.</param>
/// <param name="PlainTextCode">The plain text representation of the verification code.</param>
public sealed record GeneratedVerificationCode(Code Code, string PlainTextCode);