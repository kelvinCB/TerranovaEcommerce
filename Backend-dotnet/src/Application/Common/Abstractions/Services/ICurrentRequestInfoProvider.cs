namespace Application.Common.Abstractions.Services;

/// <summary>
/// Represents a service for providing information about the current request.
/// </summary>
public interface ICurrentRequestInfoProvider
{
    string UserAgent { get; }
    string IpAddress { get; }
}