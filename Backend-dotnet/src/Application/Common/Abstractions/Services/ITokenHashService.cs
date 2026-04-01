namespace Application.Common.Abstractions.Services;

public interface ITokenHashService
{
    string HashToken(string token);
}