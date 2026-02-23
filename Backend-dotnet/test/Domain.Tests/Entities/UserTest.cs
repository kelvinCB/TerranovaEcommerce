using Domain.Entities;
using Domain.ValueObjects;
using Domain.Tests.Factories;

namespace Domain.Tests.Entities
{
    [Trait("Layer", "Domain")]
    public class UserTest
    {
        [Fact]
        [Trait("User", "Create")]
        public void Create_ShouldCreateUser_WhenParametersAreValid()
        {
            // Arrange
            var id = Ulid.NewUlid();
            var firstName = "David";
            var lastName = "Calcanio Hernandez";
            var birthDate = new DateOnly(2001, 1, 1);
            var gender = 'M';
            var passwordHash = PasswordHash.From(new String('a', 64));
            var timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var emailAddress = Email.Create("test@example.com");
            var phoneNumber = PhoneNumber.Create("+18298881212");

            // Act
            var user = User.Create(
                id,
                firstName,
                lastName,
                birthDate,
                gender,
                passwordHash,
                timestamp,
                emailAddress,
                phoneNumber
            );

            // Assert
            Assert.NotNull(user);
            Assert.Equal(id, user.Id);
            Assert.Equal(firstName, user.FirstName);
            Assert.Equal(lastName, user.LastName);
            Assert.Equal(birthDate, user.BirthDate);
            Assert.Equal(gender, user.Gender);
            Assert.Equal(passwordHash, user.PasswordHash);
            Assert.Equal(timestamp, user.CreatedAt);
            Assert.Equal(emailAddress, user.EmailAddress);
            Assert.Equal(phoneNumber, user.PhoneNumber);
        }

        [Fact]
        [Trait("User", "Create")]
        public void Create_ShouldThrowException_WhenIdIsEmpty()
        {
            // Arrange
            var id = Ulid.Empty; // Invalid empty ID
            var firstName = "David";
            var lastName = "Calcanio Hernandez";
            var birthDate = new DateOnly(2001, 1, 1);
            var gender = 'M';
            var passwordHash = PasswordHash.From(new String('a', 64));
            var timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var emailAddress = Email.Create("test@example.com");
            var phoneNumber = PhoneNumber.Create("+18298881212");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => User.Create(
                id,
                firstName,
                lastName,
                birthDate,
                gender,
                passwordHash,
                timestamp,
                emailAddress,
                phoneNumber
            ));

            Assert.Contains("is uninitialized", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait("User", "Create")]
        public void Create_ShouldThrowException_WhenStringParametersAreNull()
        {
            // Arrange
            var id = Ulid.NewUlid();
            string? firstName = default; // Null first name
            string? lastName = default; // Null last name
            var birthDate = new DateOnly(2001, 1, 1);
            var gender = 'M';
            var passwordHash = PasswordHash.From(new String('a', 64));
            var timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var emailAddress = Email.Create("test@example.com");
            var phoneNumber = PhoneNumber.Create("+18298881212");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => User.Create(
                id,
                firstName!, // Force non-nullable for testing
                lastName!, // Force non-nullable for testing
                birthDate,
                gender,
                passwordHash,
                timestamp,
                emailAddress,
                phoneNumber
            ));

            Assert.Contains("cannot be null or whitespace", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait("User", "Create")]
        public void Create_ShouldThrowException_WhenStringParametersAreWhitespace()
        {
            // Arrange
            var id = Ulid.NewUlid();
            string? firstName = " "; // Single whitespace character
            string? lastName = "   "; // Multiple whitespace characters
            var birthDate = new DateOnly(2001, 1, 1);
            var gender = 'M';
            var passwordHash = PasswordHash.From(new String('a', 64));
            var timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var emailAddress = Email.Create("test@example.com");
            var phoneNumber = PhoneNumber.Create("+18298881212");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => User.Create(
                id,
                firstName!, // Force non-nullable for testing
                lastName!, // Force non-nullable for testing
                birthDate,
                gender,
                passwordHash,
                timestamp,
                emailAddress,
                phoneNumber
            ));

            Assert.Contains("cannot be null or whitespace", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait("User", "Create")]
        public void Create_ShouldThrowException_WhenBirthDateIsInTheFuture()
        {
            // Arrange
            var id = Ulid.NewUlid();
            var firstName = "David";
            var lastName = "Calcanio Hernandez";
            var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)); // Future date
            var gender = 'M';
            var passwordHash = PasswordHash.From(new String('a', 64));
            var timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var emailAddress = Email.Create("test@example.com");
            var phoneNumber = PhoneNumber.Create("+18298881212");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => User.Create(
                id,
                firstName,
                lastName,
                birthDate,
                gender,
                passwordHash,
                timestamp,
                emailAddress,
                phoneNumber
            ));

            Assert.Contains("cannot be a future date", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait("User", "Create")]
        public void Create_ShouldThrowException_WhenGenderIsNotInitialized()
        {
            // Arrange
            var id = Ulid.NewUlid();
            var firstName = "David";
            var lastName = "Calcanio Hernandez";
            var birthDate = new DateOnly(2001, 1, 1);
            char gender = default; // Uninitialized char
            var passwordHash = PasswordHash.From(new String('a', 64));
            var timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var emailAddress = Email.Create("test@example.com");
            var phoneNumber = PhoneNumber.Create("+18298881212");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => User.Create(
                id,
                firstName,
                lastName,
                birthDate,
                gender,
                passwordHash,
                timestamp,
                emailAddress,
                phoneNumber
            ));

            Assert.Contains("is uninitialized.", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait("User", "Create")]
        public void Create_ShouldThrowException_WhenGenderIsWhitespace()
        {
            // Arrange
            var id = Ulid.NewUlid();
            var firstName = "David";
            var lastName = "Calcanio Hernandez";
            var birthDate = new DateOnly(2001, 1, 1);
            char gender = ' '; // Whitespace character
            var passwordHash = PasswordHash.From(new String('a', 64));
            var timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var emailAddress = Email.Create("test@example.com");
            var phoneNumber = PhoneNumber.Create("+18298881212");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => User.Create(
                id,
                firstName,
                lastName,
                birthDate,
                gender,
                passwordHash,
                timestamp,
                emailAddress,
                phoneNumber
            ));

            Assert.Contains("cannot be whitespace.", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait("User", "Create")]
        public void Create_ShouldThrowException_WhenObjectParametersAreNull()
        {
            // Arrange
            var id = Ulid.NewUlid();
            var firstName = "David";
            var lastName = "Calcanio Hernandez";
            var birthDate = new DateOnly(2001, 1, 1);
            var gender = 'M';
            var passwordHash = default(PasswordHash); // Null password hash
            var timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var emailAddress = default(Email); // Null email address
            var phoneNumber = default(PhoneNumber); // Null phone number (optional)
            
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => User.Create(
                id,
                firstName,
                lastName,
                birthDate,
                gender,
                passwordHash!, // Force non-nullable for testing
                timestamp,
                emailAddress!, // Force non-nullable for testing
                phoneNumber
            ));

            Assert.Contains("cannot be null", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait("User", "Create")]
        public void Create_ShouldThrowException_WhenTimestampIsUninitialized()
        {
            // Arrange
            var id = Ulid.NewUlid();
            var firstName = "David";
            var lastName = "Calcanio Hernandez";
            var birthDate = new DateOnly(2001, 1, 1);
            var gender = 'M';
            var passwordHash = PasswordHash.From(new String('a', 64));
            var timestamp = default(DateTimeOffset); // Uninitialized timestamp
            var emailAddress = Email.Create("test@example.com");
            var phoneNumber = PhoneNumber.Create("+18298881212");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => User.Create(
                id,
                firstName,
                lastName,
                birthDate,
                gender,
                passwordHash,
                timestamp,
                emailAddress,
                phoneNumber
            ));

            Assert.Contains("is uninitialized.", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait("User", "Create")]
        public void Create_ShouldThrowException_WhenTimestampIsNotUtc()
        {
            // Arrange
            var id = Ulid.NewUlid();
            var firstName = "David";
            var lastName = "Calcanio Hernandez";
            var birthDate = new DateOnly(2001, 1, 1);
            var gender = 'M';
            var passwordHash = PasswordHash.From(new String('a', 64));
            var timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.FromHours(-4)); // Not UTC
            var emailAddress = Email.Create("test@example.com");
            var phoneNumber = PhoneNumber.Create("+18298881212");
            
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => User.Create(
                id,
                firstName,
                lastName,
                birthDate,
                gender,
                passwordHash,
                timestamp,
                emailAddress,
                phoneNumber
            ));

            Assert.Contains("must be in UTC", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait("User", "Update")]
        public void Update_ShouldUpdateUser_WhenParametersAreValid()
        {
            // Arrange
            var user = UserTestFactory.CreateUser();
            var updatedAt = user.UpdatedAt;
            var oldFirstName = user.FirstName;
            var oldLastName = user.LastName;
            var oldGender = user.Gender;

            // Act
            user.Update(
                timestamp: updatedAt.AddMinutes(1),
                firstName: "Patricia",
                lastName: "Hernandez Santana",
                gender: 'F'
            );

            // Assert
            Assert.NotEqual(updatedAt, user.UpdatedAt);
            Assert.NotEqual(oldFirstName, user.FirstName);
            Assert.NotEqual(oldLastName, user.LastName);
            Assert.NotEqual(oldGender, user.Gender);
        }

        [Fact]
        [Trait("User", "Update")]
        public void Update_ShouldUpdateUser_WhenStringParametersAreNull()
        {
            // Arrange
            var user = UserTestFactory.CreateUser();
            var updatedAt = user.UpdatedAt;
            var oldFirstName = user.FirstName;
            var oldLastName = user.LastName;

            // Act & Assert
            var exception = Record.Exception(() => user.Update(
                timestamp: updatedAt.AddMinutes(1),
                firstName: default(string),
                lastName: default(string)
            ));

            Assert.Null(exception);
            Assert.NotEqual(updatedAt, user.UpdatedAt);
            Assert.Equal(oldFirstName, user.FirstName); // First name should not be updated to null
            Assert.Equal(oldLastName, user.LastName); // Last name should not be updated to null
        }

        [Fact]
        [Trait("User", "Update")]
        public void Update_ShouldUpdateUser_WhenStringParametersAreWhitespace()
        {
            // Arrange
            var user = UserTestFactory.CreateUser();
            var updatedAt = user.UpdatedAt;
            var oldFirstName = user.FirstName;
            var oldLastName = user.LastName;

            // Act & Assert
            var exception = Record.Exception(() => user.Update(
                timestamp: updatedAt.AddMinutes(1),
                firstName: " ", // Whitespace character
                lastName: "   " // Multiple whitespace characters
            ));

            Assert.Null(exception);
            Assert.NotEqual(updatedAt, user.UpdatedAt);
            Assert.Equal(oldFirstName, user.FirstName); // First name should not be updated to null
            Assert.Equal(oldLastName, user.LastName); // Last name should not be updated to null
        }

        [Fact]
        [Trait("User", "Update")]
        public void Update_ShouldThrowException_WhenGenderIsUninitialized()
        {
            // Arrange
            var user = UserTestFactory.CreateUser();
            var updatedAt = user.UpdatedAt;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => user.Update(
                timestamp: updatedAt.AddMinutes(1),
                gender: default(char)
            ));

            Assert.Contains("is uninitialized", exception.Message, StringComparison.Ordinal);
        }

        [Fact]
        [Trait("User", "Update")]
        public void Update_ShouldThrowException_WhenGenderIsWhitespace()
        {
            // Arrange
            var user = UserTestFactory.CreateUser();
            var updatedAt = user.UpdatedAt;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => user.Update(
                timestamp: updatedAt.AddMinutes(1),
                gender: ' ' // Whitespace character
            ));

            Assert.Contains("cannot be whitespace", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait("User", "Update")]
        public void Update_ShouldThrowException_WhenTimestampIsUninitialized()
        {
            // Arrange
            var user = UserTestFactory.CreateUser();
            var timestamp = default(DateTimeOffset); // Uninitialized timestamp

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => user.Update(
                timestamp: timestamp,
                firstName: "Kelvin",
                lastName: "Hernandez Santana"
            ));

            Assert.Contains("is uninitialized.", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait("User", "Update")]
        public void Update_ShouldThrowException_WhenTimestampIsNotUtc()
        {
            // Arrange
            var user = UserTestFactory.CreateUser();
            var timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.FromHours(-4)); // Not UTC

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => user.Update(
                timestamp: timestamp,
                firstName: "Kelvin",
                lastName: "Hernandez Santana"
            ));

            Assert.Contains("must be in UTC", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait("User", "Update")]
        public void Update_ShouldThrowException_WhenTimestampIsBeforeCreatedAt()
        {
            // Arrange
            var user = UserTestFactory.CreateUser();
            var timestamp = user.CreatedAt.AddMinutes(-1); // Before CreatedAt

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => user.Update(
                timestamp: timestamp
            ));

            Assert.Contains("cannot be before", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait("User", "SetPasswordHash")]
        public void SetPasswordHash_ShouldSetPasswordHash_WhenParametersAreValid()
        {
            // Arrange
            var user = UserTestFactory.CreateUser();
            var oldPasswordHash = user.PasswordHash;
            var newPasswordHash = PasswordHash.From(new String('b', 64));
            var timestamp = user.UpdatedAt.AddMinutes(1);

            // Act and Assert
            var exception = Record.Exception(() => user.SetPasswordHash(newPasswordHash, timestamp));

            Assert.Null(exception);
            Assert.NotEqual(oldPasswordHash, user.PasswordHash);
            Assert.Equal(newPasswordHash, user.PasswordHash);
        }

        [Fact]
        [Trait("User", "SetPasswordHash")]
        public void SetPasswordHash_ShouldThrowException_WhenPasswordHashIsNull()
        {
            // Arrange
            var user = UserTestFactory.CreateUser();
            var oldPasswordHash = user.PasswordHash;
            PasswordHash? newNullPasswordHash = null;
            var timestamp = user.UpdatedAt.AddMinutes(1);

            // Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => user.SetPasswordHash(newNullPasswordHash!, timestamp)); // Force non-nullable for testing

            Assert.Contains("cannot be null", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait("User", "SetPasswordHash")]
        public void SetPasswordHash_ShouldThrowException_WhenTimestampIsUninitialized()
        {
            // Arrange
            var user = UserTestFactory.CreateUser();
            var newPasswordHash = PasswordHash.From(new String('b', 64));
            var timestamp = default(DateTimeOffset);

            // Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => user.SetPasswordHash(newPasswordHash, timestamp));

            Assert.Contains("is uninitialized", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait("User", "SetPasswordHash")]
        public void SetPasswordHash_ShouldThrowException_WhenTimestampIsNotUtc()
        {
            // Arrange
            var user = UserTestFactory.CreateUser();
            var newPasswordHash = PasswordHash.From(new String('b', 64));
            var timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.FromHours(4)); // Its not utc

            // Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => user.SetPasswordHash(newPasswordHash, timestamp));

            Assert.Contains("Must be in UTC", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait("User", "SetPasswordHash")]
        public void SetPasswordHash_ShouldThrowException_WhenTimestampIsBeforeCreateAt()
        {
            var user = UserTestFactory.CreateUser();
            var newPasswordHash = PasswordHash.From(new String('b', 64));
            var timestamp = user.CreatedAt.AddMinutes(-1);

            // Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => user.SetPasswordHash(newPasswordHash, timestamp));

            Assert.Contains("cannot be before", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}