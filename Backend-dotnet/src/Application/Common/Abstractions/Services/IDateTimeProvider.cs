namespace Application.Common.Abstractions.Services;

/// <summary>
/// Provides access to the current date and time.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Gets the current date and time in UTC.
    /// </summary>
    DateTimeOffset Timestamp { get; }
}