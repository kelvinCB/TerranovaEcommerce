using Domain.Entities;
using Domain.ValueObjects;
using Domain.Tests.Factories;

namespace Domain.Tests.Entities;

[Trait("Layer", "Domain")]
public class UserVerificationTest
{
    [Fact]
    [Trait("UserVerification", "Create")]
    public void Create_ShouldCreateUserVerification_WhenParametersAreValid()
    {
        // Arrange
        var id = Ulid.NewUlid();
        var userId = Ulid.NewUlid();
        var purpose = "email_verify";
        var verificationCode = Code.From("123456"); // Minimum code length is 6
        var expiresAt = new DateTimeOffset(2026, 1, 1, 0, 5, 0, TimeSpan.Zero); // 5 minutes after creation time
        var createdAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Act
        var userVerification = UserVerification.Create(id, userId, purpose, verificationCode, expiresAt, createdAt);

        // Assert
        Assert.NotNull(userVerification);
        Assert.Equal(id, userVerification.Id);
        Assert.Equal(userId, userVerification.UserId);
        Assert.Equal(purpose, userVerification.Purpose);
        Assert.Equal(verificationCode, userVerification.VerificationCode);
        Assert.Equal(expiresAt, userVerification.ExpiresAt);
        Assert.Equal(createdAt, userVerification.CreatedAt);
        Assert.True(userVerification.ExpiresAt > userVerification.CreatedAt);
    }

    [Fact]
    [Trait("UserVerification", "Create")]
    public void Create_ShouldThrowException_WhenIdIsEmpty()
    {
        // Arrange
        var id = Ulid.Empty; // Uninitialized
        var userId = Ulid.NewUlid();
        var purpose = "email_verify";
        var verificationCode = Code.From("123456"); // minimum code length is 6
        var expiresAt = new DateTimeOffset(2026, 1, 1, 0, 5, 0, TimeSpan.Zero); // 5 minutes after creation time
        var createdAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => UserVerification.Create(
                id, 
                userId, 
                purpose, 
                verificationCode, 
                expiresAt, 
                createdAt
            )
        );

        Assert.Contains("Is uninitialized", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("UserVerification", "Create")]
    public void Create_ShouldThrowException_WhenUserIdIsEmpty()
    {
        // Arrange
        var id = Ulid.NewUlid();
        var userId = Ulid.Empty; // Uninitialized
        var purpose = "email_verify";
        var verificationCode = Code.From("123456"); // minimum code length is 6
        var expiresAt = new DateTimeOffset(2026, 1, 1, 0, 5, 0, TimeSpan.Zero); // 5 minutes after creation time
        var createdAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => UserVerification.Create(
                id, 
                userId, 
                purpose, 
                verificationCode, 
                expiresAt, 
                createdAt
            )
        );

        Assert.Contains("Is uninitialized", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("UserVerification", "Create")]
    public void Create_ShouldThrowException_WhenPurposeIsNull()
    {
        // Arrange
        var id = Ulid.NewUlid();
        var userId = Ulid.NewUlid();
        var purpose = default(string); // Incorrect value
        var verificationCode = Code.From("123456"); // minimum code length is 6
        var expiresAt = new DateTimeOffset(2026, 1, 1, 0, 5, 0, TimeSpan.Zero); // 5 minutes after creation time
        var createdAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => UserVerification.Create(
                id, 
                userId, 
                purpose!, // Force non-null value for testing 
                verificationCode, 
                expiresAt, 
                createdAt
            )
        );

        Assert.Contains("Cannot be null", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("UserVerification", "Create")]
    public void Create_ShouldThrowException_WhenPurposeIsWhiteSpace()
    {
        // Arrange
        var id = Ulid.NewUlid();
        var userId = Ulid.NewUlid();
        var purpose = "   "; // Incorrect value
        var verificationCode = Code.From("123456"); // minimum code length is 6
        var expiresAt = new DateTimeOffset(2026, 1, 1, 0, 5, 0, TimeSpan.Zero); // 5 minutes after creation time
        var createdAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => UserVerification.Create(
                id, 
                userId, 
                purpose, 
                verificationCode, 
                expiresAt, 
                createdAt
            )
        );

        Assert.Contains("Cannot be null or whitespace", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("UserVerification", "Create")]
    public void Create_ShouldThrowException_WhenVerificationCodeIsNull()
    {
        // Arrange
        var id = Ulid.NewUlid();
        var userId = Ulid.NewUlid();
        var purpose = "email_verify";
        var verificationCode = default(Code); // Incorrect value
        var expiresAt = new DateTimeOffset(2026, 1, 1, 0, 5, 0, TimeSpan.Zero); // 5 minutes after creation time
        var createdAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => UserVerification.Create(
                id, 
                userId, 
                purpose, 
                verificationCode!, // Force non-null value for testing 
                expiresAt, 
                createdAt
            )
        );

        Assert.Contains("Cannot be null", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("UserVerification", "Create")]
    public void Create_ShouldThrowException_WhenExpiresAtIsUninitialized()
    {
        // Arrange
        var id = Ulid.NewUlid();
        var userId = Ulid.NewUlid();
        var purpose = "email_verify";
        var verificationCode = Code.From("123456"); // minimum code length is 6
        var expiresAt = default(DateTimeOffset); // Incorrect value
        var createdAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => UserVerification.Create(
                id, 
                userId, 
                purpose, 
                verificationCode, 
                expiresAt, 
                createdAt
            )
        );

        Assert.Contains("Is uninitialized", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("UserVerification", "Create")]
    public void Create_ShouldThrowException_WhenExpiresAtIsNotUtc()
    {
        // Arrange
        var id = Ulid.NewUlid();
        var userId = Ulid.NewUlid();
        var purpose = "email_verify";
        var verificationCode = Code.From("123456"); // minimum code length is 6
        var expiresAt = new DateTimeOffset(2026, 1, 1, 0, 5, 0, TimeSpan.FromHours(-4)); // It's not UTC
        var createdAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => UserVerification.Create(
                id, 
                userId, 
                purpose, 
                verificationCode, 
                expiresAt, 
                createdAt
            )
        );

        Assert.Contains("Must be in UTC", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("UserVerification", "Create")]
    public void Create_ShouldThrowException_WhenCreatedAtIsUninitialized()
    {
        // Arrange
        var id = Ulid.NewUlid();
        var userId = Ulid.NewUlid();
        var purpose = "email_verify";
        var verificationCode = Code.From("123456"); // minimum code length is 6
        var expiresAt = new DateTimeOffset(2026, 1, 1, 0, 5, 0, TimeSpan.Zero); // 5 minutes after creation time
        var createdAt = default(DateTimeOffset);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => UserVerification.Create(
                id, 
                userId, 
                purpose, 
                verificationCode, 
                expiresAt, 
                createdAt
            )
        );

        Assert.Contains("Is uninitialized", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("UserVerification", "Create")]
    public void Create_ShouldThrowException_WhenCreatedAtIsNotUtc()
    {
        // Arrange
        var id = Ulid.NewUlid();
        var userId = Ulid.NewUlid();
        var purpose = "email_verify";
        var verificationCode = Code.From("123456"); // minimum code length is 6
        var expiresAt = new DateTimeOffset(2026, 1, 1, 0, 5, 0, TimeSpan.Zero); // 5 minutes after creation time
        var createdAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.FromHours(-4)); // It's not UTC

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => UserVerification.Create(
                id, 
                userId, 
                purpose, 
                verificationCode, 
                expiresAt, 
                createdAt
            )
        );

        Assert.Contains("Must be in UTC", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("UserVerification", "Create")]
    public void Create_ShouldThrowException_WhenExpiresAtIsBeforeCreatedAt()
    {
        // Arrange
        var id = Ulid.NewUlid();
        var userId = Ulid.NewUlid();
        var purpose = "email_verify";
        var verificationCode = Code.From("123456"); // minimum code length is 6
        var expiresAt = new DateTimeOffset(2026, 1, 1, 0, 5, 0, TimeSpan.Zero); // 5 minutes after creation time
        var createdAt = new DateTimeOffset(2026, 1, 1, 0, 10, 0, TimeSpan.Zero); // 10 minutes after creation time

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => UserVerification.Create(
                id, 
                userId, 
                purpose, 
                verificationCode, 
                expiresAt, 
                createdAt
            )
        );

        Assert.Contains("Cannot be before", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("UserVerification", "Consume")]
    public void Consume_ShouldConsumeUserVerification_WhenParametersAreValid()
    {
        // Arrange
        var userVerification = UserVerificationTestFactory.CreateUserVerification();
        var timestamp = userVerification.ExpiresAt.AddMinutes(-1); // 1 minute before expiration

        // Act
        userVerification.Consume(timestamp);

        // Assert
        Assert.Equal(timestamp, userVerification.ConsumedAt);
        Assert.True(timestamp < userVerification.ExpiresAt);
        Assert.True(timestamp > userVerification.CreatedAt);
    }

    [Fact]
    [Trait("UserVerification", "Consume")]
    public void Consume_ShouldThrowException_WhenVerificationCodeHasAlreadyBeenConsumed()
    {
        // Arrange
        var userVerification = UserVerificationTestFactory.CreateUserVerification();
        var timestamp = userVerification.ExpiresAt.AddMinutes(-1); // 1 minute before expiration
        userVerification.Consume(timestamp);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => userVerification.Consume(timestamp));

        Assert.Contains("The verification code has already been consumed", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("UserVerification", "Consume")]
    public void Consume_ShouldThrowException_WhenTimestampIsUninitialized()
    {
        // Arrange
        var userVerification = UserVerificationTestFactory.CreateUserVerification();
        var timestamp = default(DateTimeOffset); // Uninitialized timestamp

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => userVerification.Consume(timestamp));

        Assert.Contains("Is uninitialized", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("UserVerification", "Consume")]
    public void Consume_ShouldThrowException_WhenTimestampIsNotUtc()
    {
        // Arrange
        var userVerification = UserVerificationTestFactory.CreateUserVerification();
        var timestamp = new DateTimeOffset(2026, 1, 1, 0, 5, 0, TimeSpan.FromHours(-4)); // It's not UTC

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => userVerification.Consume(timestamp));

        Assert.Contains("Must be in UTC", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("UserVerification", "Consume")]
    public void Consume_ShouldThrowException_WhenTimestampIsBeforeCreatedAt()
    {
        // Arrange
        var userVerification = UserVerificationTestFactory.CreateUserVerification();
        var timestamp = userVerification.CreatedAt.AddMinutes(-1); // 1 minute before creation time

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => userVerification.Consume(timestamp));

        Assert.Contains("Cannot be before", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("UserVerification", "Consume")]
    public void Consume_ShouldThrowException_WhenVerificationCodeHasExpired()
    {
        // Arrange
        var userVerification = UserVerificationTestFactory.CreateUserVerification();
        var timestamp = userVerification.ExpiresAt.AddMinutes(1); // 1 minute after expiration time

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => userVerification.Consume(timestamp));

        Assert.Contains("The verification code has expired", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

}