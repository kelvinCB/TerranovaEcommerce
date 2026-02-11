using Domain.Validations;
using Domain.ValueObjects;

namespace Domain.Entities
{
  public class User
  {
    public Ulid Id { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public PhoneNumber? PhoneNumber { get; private set; }
    public DateTimeOffset BirthDate { get; private set; }
    public char Gender { get; private set; } = char.MinValue;
    public PasswordHash PasswordHash { get; private set; } = null!;
    public bool IsActive { get; private set; } = default!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Email EmailAddress { get; private set; } = null!;
    public bool IsDeleted { get; private set; } = default!;
    
    public User(
      Ulid id,
      string firstName,
      string lastName,
      char gender,
      PasswordHash passwordHash,
      bool isActive,
      DateTimeOffset createdAt,
      DateTimeOffset updatedAt,
      Email emailAddress,
      bool isDeleted,
      PhoneNumber? phoneNumber = null,
      DateTimeOffset? birthDate = null
    )
    {
        Guard.EnsureStringNotEmpty(firstName, nameof(FirstName));
        Guard.EnsureStringNotEmpty(lastName, nameof(LastName));

        Guard.EnsureCharNotEmpty(gender, nameof(Gender));


      Id = id;
      FirstName = firstName;
      LastName = lastName;
      PasswordHash = passwordHash;
      IsActive = isActive;
      CreatedAt = createdAt;
      UpdatedAt = updatedAt;
      EmailAddress = emailAddress;
      IsDeleted = isDeleted;
      PhoneNumber = phoneNumber ?? default;
      BirthDate = birthDate ?? default;
      Gender = gender;
    }

    public void UpdateUser(
      DateTimeOffset timestamp,
      string? firstName = null,
      string? lastName = null,
      PhoneNumber? phoneNumber = null,
      DateTimeOffset? birthDate = null,
      char? gender = null
    )
    {
      if (string.IsNullOrWhiteSpace(firstName) == false)
      {
        Guard.EnsureStringNotEmpty(firstName, nameof(FirstName));
        FirstName = firstName;
      }
      if (string.IsNullOrWhiteSpace(lastName) == false)
      {
        Guard.EnsureStringNotEmpty(lastName, nameof(LastName));
        LastName = lastName;
      }
      if (gender.HasValue)
      {
        Guard.EnsureCharNotEmpty(gender.Value, nameof(Gender));
        Gender = gender.Value;
      }
      if (phoneNumber != null) PhoneNumber = phoneNumber;

      if (birthDate.HasValue) BirthDate = birthDate.Value;

      Guard.EnsureUtc(timestamp);
      UpdatedAt = timestamp;
    }
  }
}