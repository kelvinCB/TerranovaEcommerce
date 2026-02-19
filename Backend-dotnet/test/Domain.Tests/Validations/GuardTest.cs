using Domain.Validations;

namespace Domain.Tests.Validations
{
    public class GuardTest
    {
        [Fact]
        public void EnsureUtc_ShouldThrowArgumentException_WhenValueIsUninitialized()
        {
            // Arrange
            DateTimeOffset value = default;

            // Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => Guard.EnsureUtc(value, nameof(value)));
            Assert.Contains("uninitialized", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void EnsureUtc_ShouldThrowArgumentException_WhenValueIsNotUtc()
        {
            // Arrange
            DateTimeOffset value = DateTimeOffset.Now;

            // Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => Guard.EnsureUtc(value, nameof(value)));
            Assert.Contains("must be in UTC", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void EnsureUtc_ShouldNotThrowException_WhenValueIsValid()
        {
            // Arrange
            DateTimeOffset value = DateTimeOffset.UtcNow;

            // Act and Assert
            var exception = Record.Exception(() => Guard.EnsureUtc(value, nameof(value)));
            Assert.Null(exception);
        }

        [Fact]
        public void EnsureUtcNotBefore_ShouldNotThrowException_WhenValueIsValid()
        {
            // Arrange
            var now = DateTimeOffset.UtcNow;
            DateTimeOffset value = now;
            DateTimeOffset reference = now.AddDays(-1); // Reference is in the past

            // Act and Assert
            var exception = Record.Exception(() => Guard.EnsureUtcNotBefore(value, reference, nameof(value)));
            Assert.Null(exception);
        }

        [Fact]
        public void EnsureUtcNotBefore_ShouldThrowException_WhenValueIsNotUtc()
        {
            // Arrange
            DateTimeOffset value = DateTimeOffset.Now; // Not in UTC
            DateTimeOffset reference = DateTimeOffset.UtcNow;

            // Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => Guard.EnsureUtcNotBefore(value, reference, nameof(value)));
            Assert.Contains("must be in UTC", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void EnsureUtcNotBefore_ShouldThrowException_WhenValueIsBeforeReference()
        {
            // Arrange
            var now = DateTimeOffset.UtcNow;
            DateTimeOffset value = now;
            DateTimeOffset reference = now.AddDays(1); // Reference is in the future

            // Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => Guard.EnsureUtcNotBefore(value, reference, nameof(value)));
            Assert.Contains("cannot be before", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void EnsureDateOnlyNotFuture_ShouldNotThrowException_WhenValueIsValid()
        {
            // Arrange
            DateOnly value = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

            // Act and Assert
            var exception = Record.Exception(() => Guard.EnsureDateOnlyNotFuture(value, nameof(value)));
            Assert.Null(exception);
        }

        [Fact]
        public void EnsureDateOnlyNotFuture_ShouldThrowException_WhenValueIsFuture()
        {
            // Arrange
            DateOnly value = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            // Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => Guard.EnsureDateOnlyNotFuture(value, nameof(value)));
            Assert.Contains("cannot be a future date", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void EnsureStringNotNullOrWhiteSpace_ShouldThrowException_WhenValueIsNull()
        {
            // Arrange
            string? value = default;

            // Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => Guard.EnsureStringNotNullOrWhiteSpace(value!, nameof(value))); // Force non-nullable for testing
            Assert.Contains("cannot be null or whitespace.", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void EnsureStringNotNullOrWhiteSpace_ShouldThrowException_WhenValueHasOnlyWhitespace()
        {
            // Arrange
            string value = " ";

            // Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => Guard.EnsureStringNotNullOrWhiteSpace(value, nameof(value)));
            Assert.Contains("cannot be null or whitespace.", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void EnsureStringNotNullOrWhiteSpace_ShouldNotThrowException_WhenValueIsValid()
        {
            // Arrange
            string value = "Test";

            // Act and Assert
            var exception = Record.Exception(() => Guard.EnsureStringNotNullOrWhiteSpace(value, nameof(value)));
            Assert.Null(exception);
        }

        [Fact]
        public void EnsureCharInitializedAndNotWhiteSpace_ShouldThrowException_WhenValueIsUninitialized()
        {
            // Arrange
            char value = default;

            // Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => Guard.EnsureCharInitializedAndNotWhiteSpace(value, nameof(value)));
            Assert.Contains("uninitialized", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void EnsureCharInitializedAndNotWhiteSpace_ShouldThrowException_WhenValueHasOnlyWhitespace()
        {
            // Arrange
            char value = ' ';

            // Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => Guard.EnsureCharInitializedAndNotWhiteSpace(value, nameof(value)));
            Assert.Contains("cannot be whitespace", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void EnsureCharInitializedAndNotWhiteSpace_ShouldNotThrowException_WhenValueIsValid()
        {
            // Arrange
            char value = 'A';

            // Act and Assert
            var exception = Record.Exception(() => Guard.EnsureCharInitializedAndNotWhiteSpace(value, nameof(value)));
            Assert.Null(exception);
        }
    }
}