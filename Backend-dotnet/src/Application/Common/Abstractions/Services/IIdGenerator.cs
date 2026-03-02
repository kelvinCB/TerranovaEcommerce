namespace Application.Common.Abstractions.Services;

/// <summary>
/// Represents a service for generating unique identifiers.
/// </summary>
public interface IIdGenerator
{
  /// <summary>
  /// Generates a new ULID.
  /// </summary>
  /// <returns>Returns a new ULID.</returns>
  public Ulid NewUlid();
}