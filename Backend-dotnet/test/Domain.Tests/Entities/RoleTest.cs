using Domain.Entities;
using Domain.Tests.Factories;

namespace Domain.Tests.Entities
{
    [Trait("Layer", "Domain")]
    public class RoleTest
    {
        [Fact]
        [Trait("Role", "Create")]
        public void Create_ShouldCreateRole_WhenParametersAreValid()
        {
            // Arrange
            var id = Ulid.NewUlid();
            var name = "Admin";
            var description = "Administrator role";
            var timestamp = new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero);

            // Act
            var role = Role.Create(id, name, description, timestamp);

            // Assert
            Assert.Equal(id, role.Id);
            Assert.Equal(name, role.Name);
            Assert.Equal(description, role.Description);
            Assert.Equal(timestamp, role.CreatedAt);
        }

        [Fact]
        [Trait("Role", "Create")]
        public void Create_ShouldThrowException_WhenIdIsEmpty()
        {
            // Arrange
            var id = Ulid.Empty; // Invalid ID
            var name = "Admin";
            var description = "Administrator role";
            var timestamp = new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Role.Create(id, name, description, timestamp));

            Assert.Contains("Is uninitialized", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait("Role", "Create")]
        public void Create_ShouldThrowException_WhenNameIsEmpty()
        {
            // Arrange
            var id = Ulid.NewUlid();
            var name = string.Empty; // Invalid name
            var description = "Administrator role";
            var timestamp = new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Role.Create(id, name, description, timestamp));

            Assert.Contains("Cannot be null or whitespace.", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait("Role", "Create")]
        public void Create_ShouldThrowException_WhenNameIsWhitespace()
        {
            // Arrange
            var id = Ulid.NewUlid();
            var name = "   "; // Whitespace name
            var description = "Administrator role";
            var timestamp = new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Role.Create(id, name, description, timestamp));

            Assert.Contains("Cannot be null or whitespace.", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait("Role", "Create")]
        public void Create_ShouldThrowException_WhenTimestampIsNotUtc()
        {
            // Arrange
            var id = Ulid.NewUlid();
            var name = "Admin";
            var description = "Administrator role";
            var timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.FromHours(-4)); // Ist not UTC

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Role.Create(id, name, description, timestamp));

            Assert.Contains("Must be in UTC", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait("Role", "SetDescription")]
        public void SetDescription_ShouldSetDescription_WhenDescriptionHasValue()
        {
            // Arrange
            var role = RoleTestFactory.CreateRole();
            var oldDescription = role.Description; // Administrator role by default in RoleTestFactory
            var newDescription = "Technical role";
            
            // Act
            var exception = Record.Exception(() => role.SetDescription(newDescription));
            
            // Assert
            Assert.Null(exception);
            Assert.NotEqual(oldDescription, role.Description);
        }

        [Fact]
        [Trait("Role", "SetDescription")]
        public void SetDescription_ShouldSetDescription_WhenDescriptionIsNull()
        {
            // Arrange
            var role = RoleTestFactory.CreateRole();
            var oldDescription = role.Description;
            var newDescription = default(string);
            
            // Act & Assert
            var exception = Record.Exception(() => role.SetDescription(newDescription));

            Assert.Null(exception);
            Assert.NotEqual(oldDescription, role.Description);
        }

        [Fact]
        [Trait("Role", "SetDescription")]
        public void SetDescription_ShouldSetDescription_WhenDescriptionIsWhitespace()
        {
            // Arrange
            var role = RoleTestFactory.CreateRole();
            var oldDescription = role.Description;
            var newDescription = "   ";
            
            // Act & Assert
            var exception = Record.Exception(() => role.SetDescription(newDescription));

            Assert.Null(exception);
            Assert.NotEqual(oldDescription, role.Description);
        }

        [Fact]
        [Trait("Role", "SetName")]
        public void SetName_ShouldSetName_WhenNameHasValue()
        {
            // Arrange
            var role = RoleTestFactory.CreateRole();
            var oldName = role.Name; // Admin by default in RoleTestFactory
            var newName = "Manager";
            
            // Act
            var exception = Record.Exception(() => role.SetName(newName));
            
            // Assert
            Assert.Null(exception);
            Assert.NotEqual(oldName, role.Name);
        }

        [Fact]
        [Trait("Role", "SetName")]
        public void SetName_ShouldThrowException_WhenNameIsNull()
        {
            // Arrange
            var role = RoleTestFactory.CreateRole();
            var newName = default(string);
            
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => role.SetName(newName!)); // force non-nullable for testing

            Assert.Contains("cannot be null or whitespace.", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait("Role", "SetName")]
        public void SetName_ShouldThrowException_WhenNameIsWhitespace()
        {
            // Arrange
            var role = RoleTestFactory.CreateRole();
            var newName = "   ";
            
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => role.SetName(newName!)); // force non-nullable for testing

            Assert.Contains("cannot be null or whitespace.", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        
    }
}