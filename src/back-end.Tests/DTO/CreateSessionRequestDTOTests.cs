using Xunit;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using back_end.domain.DTOs;

namespace back_end.Tests.DTO;

public class CreateSessionRequestDTOTests
{
    [Fact]
    public void CreateSessionRequestDTO_WithValidData_IsValid()
    {
        // Arrange
        var dto = new CreateSessionRequestDTO
        {
            Menu_Id = 1,
            Location_Id = 1,
            Table_Id = 5,
            Request_By_Oid = "user-123",
            Request_By_Name = "John Doe"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

        // Assert
        isValid.Should().BeTrue();
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void CreateSessionRequestDTO_WithNullValues_IsValid()
    {
        // Arrange - All properties are nullable, so null should be valid
        var dto = new CreateSessionRequestDTO
        {
            Menu_Id = null,
            Location_Id = null,
            Table_Id = null,
            Request_By_Oid = null,
            Request_By_Name = null
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

        // Assert
        isValid.Should().BeTrue("All properties are nullable");
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void CreateSessionRequestDTO_SerializesAndDeserializesCorrectly()
    {
        // Arrange
        var dto = new CreateSessionRequestDTO
        {
            Menu_Id = 1,
            Location_Id = 2,
            Table_Id = 3,
            Request_By_Oid = "user-456",
            Request_By_Name = "Jane Smith"
        };

        // Act
        var json = JsonSerializer.Serialize(dto);
        var deserialized = JsonSerializer.Deserialize<CreateSessionRequestDTO>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Menu_Id.Should().Be(1);
        deserialized.Location_Id.Should().Be(2);
        deserialized.Table_Id.Should().Be(3);
        deserialized.Request_By_Oid.Should().Be("user-456");
        deserialized.Request_By_Name.Should().Be("Jane Smith");
    }

    [Fact]
    public void CreateSessionRequestDTO_WithOnlyRequiredFields_IsValid()
    {
        // Arrange - Assuming Menu_Id and Location_Id might be required in practice
        var dto = new CreateSessionRequestDTO
        {
            Menu_Id = 1,
            Location_Id = 1,
            Table_Id = null,
            Request_By_Oid = null,
            Request_By_Name = null
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

        // Assert
        isValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void CreateSessionRequestDTO_WithValidMenuId_IsAccepted(int menuId)
    {
        // Arrange
        var dto = new CreateSessionRequestDTO
        {
            Menu_Id = menuId,
            Location_Id = 1,
            Table_Id = 1
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

        // Assert
        isValid.Should().BeTrue();
        dto.Menu_Id.Should().Be(menuId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    public void CreateSessionRequestDTO_WithValidLocationId_IsAccepted(int locationId)
    {
        // Arrange
        var dto = new CreateSessionRequestDTO
        {
            Menu_Id = 1,
            Location_Id = locationId,
            Table_Id = 1
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

        // Assert
        isValid.Should().BeTrue();
        dto.Location_Id.Should().Be(locationId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    public void CreateSessionRequestDTO_WithValidTableId_IsAccepted(int tableId)
    {
        // Arrange
        var dto = new CreateSessionRequestDTO
        {
            Menu_Id = 1,
            Location_Id = 1,
            Table_Id = tableId
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

        // Assert
        isValid.Should().BeTrue();
        dto.Table_Id.Should().Be(tableId);
    }

    [Fact]
    public void CreateSessionRequestDTO_WithEmptyStrings_IsValid()
    {
        // Arrange
        var dto = new CreateSessionRequestDTO
        {
            Menu_Id = 1,
            Location_Id = 1,
            Table_Id = 1,
            Request_By_Oid = "",
            Request_By_Name = ""
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

        // Assert
        isValid.Should().BeTrue("Empty strings should be allowed");
    }

    [Fact]
    public void CreateSessionRequestDTO_WithLongStrings_IsValid()
    {
        // Arrange
        var longString = new string('x', 500);
        var dto = new CreateSessionRequestDTO
        {
            Menu_Id = 1,
            Location_Id = 1,
            Table_Id = 1,
            Request_By_Oid = longString,
            Request_By_Name = longString
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

        // Assert
        isValid.Should().BeTrue("Long strings should be allowed without explicit length validation");
    }

    [Fact]
    public void CreateSessionRequestDTO_WithWhitespaceStrings_IsValid()
    {
        // Arrange
        var dto = new CreateSessionRequestDTO
        {
            Menu_Id = 1,
            Location_Id = 1,
            Table_Id = 1,
            Request_By_Oid = "   ",
            Request_By_Name = "   "
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

        // Assert
        isValid.Should().BeTrue("Whitespace strings should be allowed");
    }

    [Fact]
    public void CreateSessionRequestDTO_DefaultValues_AreNull()
    {
        // Arrange & Act
        var dto = new CreateSessionRequestDTO();

        // Assert - All properties should default to null
        dto.Menu_Id.Should().BeNull();
        dto.Location_Id.Should().BeNull();
        dto.Table_Id.Should().BeNull();
        dto.Request_By_Oid.Should().BeNull();
        dto.Request_By_Name.Should().BeNull();
    }

    [Fact]
    public void CreateSessionRequestDTO_CanBeInstantiatedWithObjectInitializer()
    {
        // Act
        var dto = new CreateSessionRequestDTO
        {
            Menu_Id = 1,
            Location_Id = 2,
            Table_Id = 3,
            Request_By_Oid = "oid-123",
            Request_By_Name = "Test User"
        };

        // Assert
        dto.Should().NotBeNull();
        dto.Menu_Id.Should().Be(1);
        dto.Location_Id.Should().Be(2);
        dto.Table_Id.Should().Be(3);
        dto.Request_By_Oid.Should().Be("oid-123");
        dto.Request_By_Name.Should().Be("Test User");
    }
}
