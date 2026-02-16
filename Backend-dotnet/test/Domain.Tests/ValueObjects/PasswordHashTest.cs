using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects
{
    public class PasswordHashTest
    {
        [Fact]
        public void PasswordHash_ShouldCreatePasswordHash_WhenValueIsValid()
        {
            // Arrange
            var validPasswordHash = "a".PadLeft(65, 'a'); // Create a string of 65 characters

            // Act
            var passwordHash = PasswordHash.From(validPasswordHash);

            // Assert
            Assert.Equal(validPasswordHash, passwordHash.Value);
        }

        [Fact]
        public void PasswordHash_ShouldThrowException_WhenValueIsNull()
        {
            // Arrange
            string? nullPasswordHash = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => PasswordHash.From(nullPasswordHash!)); // Force non-nullable string
            Assert.Contains("is required", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void PasswordHash_ShouldThrowException_WhenValueIsNotLongerThan64Characters()
        {
            // Arrange
            var shortPasswordHash = "a";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => PasswordHash.From(shortPasswordHash));
            Assert.Contains("must be longer than 64 characters", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void PasswordHash_ShouldThrowException_WhenValueHasWhitespace()
        {
            // Arrange
            var whitespacePasswordHash = "a a".PadLeft(65, 'a'); // Create a string of 65 characters with whitespace

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => PasswordHash.From(whitespacePasswordHash));
            Assert.Contains("cannot contain whitespace", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void PasswordHash_ToString_ShouldReturnMaskedValue()
        {
            // Arrange
            var validPasswordHash = "a".PadLeft(65, 'a'); // Create a string of 65 characters
            var passwordHash = PasswordHash.From(validPasswordHash);

            // Act
            var result = passwordHash.ToString();

            // Assert
            Assert.Equal("PasswordHash(***)", result);
        }
    }
}