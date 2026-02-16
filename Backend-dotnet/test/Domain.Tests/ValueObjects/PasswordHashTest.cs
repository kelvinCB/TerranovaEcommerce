using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects
{
    public class PasswordHashTest
    {
        [Fact]
        public void PasswordHash_ShouldCreatePasswordHash_WhenValueIsValid()
        {
            // Arrange
            const int validLength = 65;
            var validPasswordHash = "a".PadLeft(validLength, 'a'); // Create a string of 65 characters

            // Act
            var passwordHash = PasswordHash.From(validPasswordHash);

            // Assert
            Assert.Equal(validPasswordHash, passwordHash.Value);
        }

        [Fact]
        public void PasswordHash_ShouldThrowException_WhenValueIsNull()
        {
            // Arrange
            string? passwordHash = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => PasswordHash.From(passwordHash!)); // Force non-nullable string
            Assert.Contains("is required", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void PasswordHash_ShouldThrowException_WhenValueIsNotLongerThan64Characters()
        {
            // Arrange
            var shortPasswordHash = "a";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => PasswordHash.From(shortPasswordHash));
            Assert.Contains("must be longer", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void PasswordHash_ShouldThrowException_WhenValueHasWhitespace()
        {
            // Arrange
            const int validLength = 65;
            var whitespacePasswordHash = "a a".PadLeft(validLength, 'a'); // Create a string of 65 characters with whitespace

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => PasswordHash.From(whitespacePasswordHash));
            Assert.Contains("cannot contain whitespace", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void PasswordHash_ToString_ShouldReturnMaskedValue()
        {
            // Arrange
            const int validLength = 65;
            var validPasswordHash = "a".PadLeft(validLength, 'a'); // Create a string of 65 characters
            var passwordHash = PasswordHash.From(validPasswordHash);

            // Act
            var result = passwordHash.ToString();

            // Assert
            Assert.Equal("PasswordHash(***)", result);
        }
    }
}