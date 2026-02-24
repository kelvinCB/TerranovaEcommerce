using Domain.Validations;
using Domain.ValueObjects;

namespace Domain.Entities;

/// <summary>
/// Represents a user in the system with properties such as name, contact information, and authentication details.
/// </summary>
public class User
{
    public Ulid Id { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public PhoneNumber? PhoneNumber { get; private set; }
    public DateOnly BirthDate { get; private set; }
    public char Gender { get; private set; } = char.MinValue;
    public PasswordHash PasswordHash { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Email EmailAddress { get; private set; } = null!;
    public bool IsDeleted { get; private set; }

    /// <summary>
    /// Initializes a new instance of the User class with the specified parameters.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="firstName">The first name of the user.</param>
    /// <param name="lastName">The last name of the user.</param>
    /// <param name="birthDate">The birth date of the user.</param>
    /// <param name="gender">The gender of the user.</param>
    /// <param name="passwordHash">The password hash for the user.</param>
    /// <param name="isActive">Whether the user is active.</param>
    /// <param name="createdAt">The timestamp when the user was created.</param>
    /// <param name="updatedAt">The timestamp when the user was last updated.</param>
    /// <param name="emailAddress">The email address of the user.</param>
    /// <param name="isDeleted">Whether the user is deleted.</param>
    /// <param name="phoneNumber">The phone number of the user.</param>
    private User(
      Ulid id,
      string firstName,
      string lastName,
      DateOnly birthDate,
      char gender,
      PasswordHash passwordHash,
      bool isActive,
      DateTimeOffset createdAt,
      DateTimeOffset updatedAt,
      Email emailAddress,
      bool isDeleted,
      PhoneNumber? phoneNumber = null
    )
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        PasswordHash = passwordHash;
        IsActive = isActive;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        EmailAddress = emailAddress;
        IsDeleted = isDeleted;
        PhoneNumber = phoneNumber;
        BirthDate = birthDate;
        Gender = gender;
    }

    /// <summary>
    /// Creates a new instance of the User class with the specified parameters and performs necessary validations.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="firstName">The first name of the user.</param>
    /// <param name="lastName">The last name of the user.</param>
    /// <param name="birthDate">The birth date of the user.</param>
    /// <param name="gender">The gender of the user.</param>
    /// <param name="passwordHash">The password hash for the user.</param>
    /// <param name="timestamp">The timestamp in UTC when the user is created.</param>
    /// <param name="emailAddress">The email address of the user.</param>
    /// <param name="phoneNumber">The phone number of the user.</param>
    /// <returns>The newly created User instance.</returns>
    public static User Create(
      Ulid id,
      string firstName,
      string lastName,
      DateOnly birthDate,
      char gender,
      PasswordHash passwordHash,
      DateTimeOffset timestamp,
      Email emailAddress,
      PhoneNumber? phoneNumber = default)
    {
        // Perform validations using the Guard class to ensure domain invariants are maintained
        Guard.EnsureUlidNotEmpty(id, nameof(id));
        Guard.EnsureStringNotNullOrWhiteSpace(firstName, nameof(firstName));
        Guard.EnsureStringNotNullOrWhiteSpace(lastName, nameof(lastName));
        Guard.EnsureDateOnlyNotFuture(birthDate, nameof(birthDate));
        Guard.EnsureCharInitializedAndNotWhiteSpace(gender, nameof(gender));
        Guard.EnsureNotNull(passwordHash, nameof(passwordHash));
        Guard.EnsureNotNull(emailAddress, nameof(emailAddress));
        Guard.EnsureUtc(timestamp, nameof(timestamp));

        return new User(
          id,
          firstName,
          lastName,
          birthDate,
          gender,
          passwordHash,
          isActive: true,
          createdAt: timestamp,
          updatedAt: timestamp,
          emailAddress,
          isDeleted: false,
          phoneNumber
        );
    }

    /// <summary>
    /// Updates the user's properties with the provided values.
    /// Only non-null and non-empty values will be updated.
    /// </summary>
    /// <param name="timestamp">The timestamp in UTC when the user was last updated.</param>
    /// <param name="firstName">The first name of the user.</param>
    /// <param name="lastName">The last name of the user.</param>
    /// <param name="gender">The gender of the user.</param>
    /// <exception cref="ArgumentException">Thrown when any provided parameter violates domain invariants.</exception>
    public void Update(
      DateTimeOffset timestamp,
      string? firstName = null,
      string? lastName = null,
      char? gender = null
    )
    {
        if (string.IsNullOrWhiteSpace(firstName) == false)
        {
            Guard.EnsureStringNotNullOrWhiteSpace(firstName, nameof(firstName));
            FirstName = firstName;
        }
        if (string.IsNullOrWhiteSpace(lastName) == false)
        {
            Guard.EnsureStringNotNullOrWhiteSpace(lastName, nameof(lastName));
            LastName = lastName;
        }
        if (gender.HasValue)
        {
            Guard.EnsureCharInitializedAndNotWhiteSpace(gender.Value, nameof(gender));
            Gender = gender.Value;
        }

        Guard.EnsureUtc(timestamp, nameof(timestamp));
        Guard.EnsureUtcNotBefore(timestamp, CreatedAt, nameof(timestamp));
        UpdatedAt = timestamp;
    }

    /// <summary>
    /// Sets the password hash for the user and updates the timestamp.
    /// </summary>
    /// <param name="passwordHash">The new password hash to set for the user.</param>
    /// <param name="timestamp">The timestamp in UTC when the password hash was set.</param>
    public void SetPasswordHash(PasswordHash passwordHash, DateTimeOffset timestamp)
    {
        Guard.EnsureNotNull(passwordHash, nameof(passwordHash));
        Guard.EnsureUtc(timestamp, nameof(timestamp));
        Guard.EnsureUtcNotBefore(timestamp, CreatedAt, nameof(timestamp));

        PasswordHash = passwordHash;
        UpdatedAt = timestamp;
    }

    /// <summary>
    /// Sets the active status of the user and updates the timestamp.
    /// </summary>
    /// <param name="isActive">The active status to set for the user.</param>
    /// <param name="timestamp">The timestamp in UTC when the active status was set.</param>
    public void SetIsActive(bool isActive, DateTimeOffset timestamp)
    {
        Guard.EnsureUtc(timestamp, nameof(timestamp));
        Guard.EnsureUtcNotBefore(timestamp, CreatedAt, nameof(timestamp));

        IsActive = isActive;
        UpdatedAt = timestamp;
    }

    /// <summary>
    /// Sets the deleted status of the user and updates the timestamp.
    /// </summary>
    /// <param name="isDeleted">The deleted status to set for the user.</param>
    /// <param name="timestamp">The timestamp in UTC when the deleted status was set.</param>
    public void SetIsDeleted(bool isDeleted, DateTimeOffset timestamp)
    {
        Guard.EnsureUtc(timestamp, nameof(timestamp));
        Guard.EnsureUtcNotBefore(timestamp, CreatedAt, nameof(timestamp));

        IsDeleted = isDeleted;
        UpdatedAt = timestamp;
    }

    /// <summary>
    /// Sets the email address of the user and updates the timestamp.
    /// </summary>
    /// <param name="emailAddress">The email address to set for the user.</param>
    /// <param name="timestamp">The timestamp in UTC when the email address was set.</param>
    public void SetEmailAddress(Email emailAddress, DateTimeOffset timestamp)
    {
        Guard.EnsureNotNull(emailAddress, nameof(emailAddress));
        Guard.EnsureUtc(timestamp, nameof(timestamp));
        Guard.EnsureUtcNotBefore(timestamp, CreatedAt, nameof(timestamp));

        EmailAddress = emailAddress;
        UpdatedAt = timestamp;
    }

    /// <summary>
    /// Sets the phone number of the user and updates the timestamp.
    /// </summary>
    /// <param name="phoneNumber">The phone number to set for the user.</param>
    /// <param name="timestamp">The timestamp in UTC when the phone number was set.</param>
    public void SetPhoneNumber(PhoneNumber? phoneNumber, DateTimeOffset timestamp)
    {
        Guard.EnsureUtc(timestamp, nameof(timestamp));
        Guard.EnsureUtcNotBefore(timestamp, CreatedAt, nameof(timestamp));
        PhoneNumber = phoneNumber;
        UpdatedAt = timestamp;
    }

    /// <summary>
    /// Sets the birth date of the user and updates the timestamp.
    /// </summary>
    /// <param name="birthDate">The birth date to set for the user.</param>
    /// <param name="timestamp">The timestamp in UTC when the birth date was set.</param>
    public void SetBirthDate(DateOnly birthDate, DateTimeOffset timestamp)
    {
        Guard.EnsureDateOnlyNotFuture(birthDate, nameof(birthDate));
        Guard.EnsureUtc(timestamp, nameof(timestamp));
        Guard.EnsureUtcNotBefore(timestamp, CreatedAt, nameof(timestamp));

        BirthDate = birthDate;
        UpdatedAt = timestamp;
    }

}
