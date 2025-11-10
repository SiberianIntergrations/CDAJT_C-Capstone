using Xunit;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using back_end.DTO.ServiceRequestDTOs;
using back_end.domain.enums;

namespace back_end.Tests.DTO;

public class ServiceRequestDTOsTests
{
    [Fact]
    public void ServiceRequestCreateDTO_WithValidData_SerializesCorrectly()
    {
        // Arrange
        var dto = new ServiceRequestCreateDTO
        {
            Notes = "Need more napkins",
            Request_By_Oid = "user-123",
            Request_By_Name = "John Doe"
        };

        // Act
        var json = JsonSerializer.Serialize(dto);
        var deserialized = JsonSerializer.Deserialize<ServiceRequestCreateDTO>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Notes.Should().Be("Need more napkins");
        deserialized.Request_By_Oid.Should().Be("user-123");
        deserialized.Request_By_Name.Should().Be("John Doe");
    }

    [Fact]
    public void ServiceRequestCreateDTO_WithNullValues_IsValid()
    {
        // Arrange
        var dto = new ServiceRequestCreateDTO
        {
            Notes = null,
            Request_By_Oid = null,
            Request_By_Name = null
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

        // Assert
        isValid.Should().BeTrue("DTO should allow null values for optional fields");
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void ServiceRequestCreateDTO_JsonPropertyNames_MatchExpectedFormat()
    {
        // Arrange
        var dto = new ServiceRequestCreateDTO
        {
            Notes = "Test",
            Request_By_Oid = "123",
            Request_By_Name = "Test User"
        };

        // Act
        var json = JsonSerializer.Serialize(dto);

        // Assert
        json.Should().Contain("\"notes\"");
        json.Should().Contain("\"request_by_oid\"");
        json.Should().Contain("\"request_by_name\"");
        json.Should().NotContain("Notes"); // Should use snake_case
    }

    [Fact]
    public void ServiceRequestResponseDTO_WithValidData_SerializesCorrectly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var dto = new ServiceRequestResponseDTO
        {
            Request_Id = 1,
            Session_Id = 5,
            Table_Id = 3,
            Request_By_Oid = "user-123",
            Request_By_Name = "John Doe",
            Claimed_By_Oid = "staff-456",
            Claimed_By_Name = "Jane Smith",
            Notes = "Need water",
            Status = ServiceRequestStatus.Pending,
            Created_At = now,
            Claimed_At = now.AddMinutes(2),
            Completed_At = null,
            Table_Number = 5
        };

        // Act
        var json = JsonSerializer.Serialize(dto);
        var deserialized = JsonSerializer.Deserialize<ServiceRequestResponseDTO>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Request_Id.Should().Be(1);
        deserialized.Session_Id.Should().Be(5);
        deserialized.Table_Id.Should().Be(3);
        deserialized.Request_By_Oid.Should().Be("user-123");
        deserialized.Request_By_Name.Should().Be("John Doe");
        deserialized.Status.Should().Be(ServiceRequestStatus.Pending);
        deserialized.Table_Number.Should().Be(5);
    }

    [Fact]
    public void ServiceRequestResponseDTO_DefaultValues_AreSetCorrectly()
    {
        // Arrange & Act
        var dto = new ServiceRequestResponseDTO();

        // Assert
        dto.Request_By_Oid.Should().Be(string.Empty);
        dto.Request_By_Name.Should().Be(string.Empty);
        dto.Claimed_By_Oid.Should().BeNull();
        dto.Claimed_By_Name.Should().BeNull();
        dto.Notes.Should().BeNull();
        dto.Claimed_At.Should().BeNull();
        dto.Completed_At.Should().BeNull();
    }

    [Fact]
    public void ServiceRequestResponseDTO_JsonPropertyNames_MatchExpectedFormat()
    {
        // Arrange
        var dto = new ServiceRequestResponseDTO
        {
            Request_Id = 1,
            Session_Id = 2,
            Table_Id = 3,
            Request_By_Oid = "123",
            Request_By_Name = "Test",
            Status = ServiceRequestStatus.Pending,
            Created_At = DateTime.UtcNow,
            Table_Number = 5
        };

        // Act
        var json = JsonSerializer.Serialize(dto);

        // Assert
        json.Should().Contain("\"request_id\"");
        json.Should().Contain("\"session_id\"");
        json.Should().Contain("\"table_id\"");
        json.Should().Contain("\"request_by_oid\"");
        json.Should().Contain("\"request_by_name\"");
        json.Should().Contain("\"status\"");
        json.Should().Contain("\"created_at\"");
        json.Should().Contain("\"table_number\"");
    }

    [Theory]
    [InlineData(ServiceRequestStatus.Pending)]
    [InlineData(ServiceRequestStatus.Claimed)]
    [InlineData(ServiceRequestStatus.Completed)]
    [InlineData(ServiceRequestStatus.Cancelled)]
    public void ServiceRequestResponseDTO_SupportsAllServiceRequestStatuses(ServiceRequestStatus status)
    {
        // Arrange
        var dto = new ServiceRequestResponseDTO
        {
            Request_Id = 1,
            Session_Id = 1,
            Table_Id = 1,
            Status = status,
            Created_At = DateTime.UtcNow,
            Table_Number = 1
        };

        // Act
        var json = JsonSerializer.Serialize(dto);
        var deserialized = JsonSerializer.Deserialize<ServiceRequestResponseDTO>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Status.Should().Be(status);
    }

    [Fact]
    public void ServiceRequestResponseDTO_WithNullOptionalFields_SerializesCorrectly()
    {
        // Arrange
        var dto = new ServiceRequestResponseDTO
        {
            Request_Id = 1,
            Session_Id = 1,
            Table_Id = 1,
            Request_By_Oid = "user-123",
            Request_By_Name = "Test User",
            Claimed_By_Oid = null,
            Claimed_By_Name = null,
            Notes = null,
            Status = ServiceRequestStatus.Pending,
            Created_At = DateTime.UtcNow,
            Claimed_At = null,
            Completed_At = null,
            Table_Number = 1
        };

        // Act
        var json = JsonSerializer.Serialize(dto);
        var deserialized = JsonSerializer.Deserialize<ServiceRequestResponseDTO>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Claimed_By_Oid.Should().BeNull();
        deserialized.Claimed_By_Name.Should().BeNull();
        deserialized.Notes.Should().BeNull();
        deserialized.Claimed_At.Should().BeNull();
        deserialized.Completed_At.Should().BeNull();
    }

    [Fact]
    public void ServiceRequestCreateDTO_WithEmptyStrings_IsValid()
    {
        // Arrange
        var dto = new ServiceRequestCreateDTO
        {
            Notes = "",
            Request_By_Oid = "",
            Request_By_Name = ""
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

        // Assert
        isValid.Should().BeTrue("DTO should allow empty strings");
    }

    [Fact]
    public void ServiceRequestCreateDTO_WithLongNotes_IsValid()
    {
        // Arrange
        var longNotes = new string('x', 1000);
        var dto = new ServiceRequestCreateDTO
        {
            Notes = longNotes,
            Request_By_Oid = "user-123",
            Request_By_Name = "Test User"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

        // Assert
        isValid.Should().BeTrue("DTO should allow long notes");
        dto.Notes.Length.Should().Be(1000);
    }

    [Fact]
    public void ServiceRequestResponseDTO_TimestampOrdering_MakesLogicalSense()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var dto = new ServiceRequestResponseDTO
        {
            Request_Id = 1,
            Session_Id = 1,
            Table_Id = 1,
            Request_By_Oid = "user-123",
            Request_By_Name = "Test User",
            Status = ServiceRequestStatus.Completed,
            Created_At = now,
            Claimed_At = now.AddMinutes(5),
            Completed_At = now.AddMinutes(10),
            Table_Number = 1
        };

        // Assert - Timestamps should be in logical order
        dto.Created_At.Should().BeBefore(dto.Claimed_At!.Value);
        dto.Claimed_At!.Value.Should().BeBefore(dto.Completed_At!.Value);
    }
}
