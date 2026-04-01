namespace Application.Common.Exceptions;

public sealed class RefreshTokenNotFoundException : ApplicationExceptionBase
{
    public RefreshTokenNotFoundException() : base($"Refresh token was not found.") { }
}