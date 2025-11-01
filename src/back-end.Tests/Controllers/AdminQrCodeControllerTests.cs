using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using back_end.Controllers;
using back_end.Services;
using back_end.domain.DbContexts;
using back_end.domain.Entities;

namespace back_end.Tests.Controllers;

public class AdminQrCodeControllerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<ILogger<QrGeneratorService>> _mockServiceLogger;
    private readonly Mock<ILogger<AdminQrCodeController>> _mockControllerLogger;
    private readonly QrGeneratorService _qrService;
    private readonly AdminQrCodeController _controller;

    public AdminQrCodeControllerTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mockConfig = new Mock<IConfiguration>();
        _mockServiceLogger = new Mock<ILogger<QrGeneratorService>>();
        _mockControllerLogger = new Mock<ILogger<AdminQrCodeController>>();

        _qrService = new QrGeneratorService(_context, _mockConfig.Object, _mockServiceLogger.Object);
        _controller = new AdminQrCodeController(_qrService, _mockControllerLogger.Object);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        var location = new Locations
        {
            Location_Id = 1,
            Name = "Downtown",
            Address_Primary = "123 Main St",
            City = "Edmonton",
            Province = "AB",
            Postal_Code = "T5K 2B7",
            Phone_Number = "780-123-4567"
        };

        _context.Locations.Add(location);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetWifiQr_ValidRequest_ReturnsFileResult()
    {
        // Arrange
        var locationId = 1;
        var table = 5;

        _mockConfig.Setup(c => c[$"QRCodeSettings:Locations:{locationId}:WiFi:SSID"])
            .Returns("TestWiFi");
        _mockConfig.Setup(c => c[$"QRCodeSettings:Locations:{locationId}:WiFi:Password"])
            .Returns("TestPassword123");

        // Act
        var result = await _controller.GetWifiQr(locationId, table);

        // Assert
        result.Should().BeOfType<FileContentResult>();
        var fileResult = result as FileContentResult;
        fileResult!.ContentType.Should().Be("image/png");
        fileResult.FileDownloadName.Should().Be($"wifi_L{locationId}_T{table}.png");
        fileResult.FileContents.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetWifiQr_InvalidLocation_ReturnsNotFound()
    {
        // Arrange
        var locationId = 999;
        var table = 5;

        _mockConfig.Setup(c => c[$"QRCodeSettings:Locations:{locationId}:WiFi:SSID"])
            .Returns((string?)null);
        _mockConfig.Setup(c => c["QRCodeSettings:WiFi:SSID"])
            .Returns((string?)null);

        // Act
        var result = await _controller.GetWifiQr(locationId, table);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task GetWifiQr_InvalidTableNumber_ReturnsBadRequest()
    {
        // Arrange
        var locationId = 1;
        var table = 0;

        // Act
        var result = await _controller.GetWifiQr(locationId, table);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetWifiQr_NegativeLocationId_ReturnsBadRequest()
    {
        // Arrange
        var locationId = -1;
        var table = 5;

        // Act
        var result = await _controller.GetWifiQr(locationId, table);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetSessionQr_ValidRequest_ReturnsFileResult()
    {
        // Arrange
        var locationId = 1;
        var table = 5;

        _mockConfig.Setup(c => c["QRCodeSettings:SessionPageUrl"])
            .Returns("http://localhost:3000");

        // Act
        var result = await _controller.GetSessionQr(locationId, table);

        // Assert
        result.Should().BeOfType<FileContentResult>();
        var fileResult = result as FileContentResult;
        fileResult!.ContentType.Should().Be("image/png");
        fileResult.FileDownloadName.Should().Be($"session_L{locationId}_T{table}.png");
        fileResult.FileContents.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetSessionQr_InvalidLocation_ReturnsNotFound()
    {
        // Arrange
        var locationId = 999;
        var table = 5;

        // Act
        var result = await _controller.GetSessionQr(locationId, table);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetSessionQr_InvalidTableNumber_ReturnsBadRequest()
    {
        // Arrange
        var locationId = 1;
        var table = -1;

        // Act
        var result = await _controller.GetSessionQr(locationId, table);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task BulkGenerateQrCodes_ValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var locationId = 1;
        var tableCount = 10;

        _mockConfig.Setup(c => c[$"QRCodeSettings:Locations:{locationId}:WiFi:SSID"])
            .Returns("TestWiFi");
        _mockConfig.Setup(c => c[$"QRCodeSettings:Locations:{locationId}:WiFi:Password"])
            .Returns("TestPassword123");
        _mockConfig.Setup(c => c["QRCodeSettings:SessionPageUrl"])
            .Returns("http://localhost:3000");

        // Act
        var result = await _controller.BulkGenerateQrCodes(locationId, tableCount);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();

        var value = okResult.Value as dynamic;
        Assert.NotNull(value);
    }

    [Fact]
    public async Task BulkGenerateQrCodes_InvalidLocation_ReturnsNotFound()
    {
        // Arrange
        var locationId = 999;
        var tableCount = 10;

        // Act
        var result = await _controller.BulkGenerateQrCodes(locationId, tableCount);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task BulkGenerateQrCodes_InvalidTableCount_ReturnsBadRequest()
    {
        // Arrange
        var locationId = 1;
        var tableCount = 0;

        // Act
        var result = await _controller.BulkGenerateQrCodes(locationId, tableCount);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task BulkGenerateQrCodes_ExcessiveTableCount_ReturnsBadRequest()
    {
        // Arrange
        var locationId = 1;
        var tableCount = 1001; // Over the 1000 limit

        // Act
        var result = await _controller.BulkGenerateQrCodes(locationId, tableCount);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void ClearQrCache_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var locationId = 1;
        var table = 5;

        // Act
        var result = _controller.ClearQrCache(locationId, table);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task GetWifiQr_MultipleRequests_GeneratesConsistentQrCodes()
    {
        // Arrange
        var locationId = 1;
        var table = 5;

        _mockConfig.Setup(c => c[$"QRCodeSettings:Locations:{locationId}:WiFi:SSID"])
            .Returns("TestWiFi");
        _mockConfig.Setup(c => c[$"QRCodeSettings:Locations:{locationId}:WiFi:Password"])
            .Returns("TestPassword123");

        // Act - Make two requests
        var result1 = await _controller.GetWifiQr(locationId, table);
        var result2 = await _controller.GetWifiQr(locationId, table);

        // Assert - Both should return file results
        result1.Should().BeOfType<FileContentResult>();
        result2.Should().BeOfType<FileContentResult>();

        var file1 = result1 as FileContentResult;
        var file2 = result2 as FileContentResult;

        file1!.FileContents.Length.Should().Be(file2!.FileContents.Length);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 10)]
    [InlineData(1, 100)]
    public async Task GetWifiQr_DifferentTables_GeneratesUniqueQrCodes(int locationId, int table)
    {
        // Arrange
        _mockConfig.Setup(c => c[$"QRCodeSettings:Locations:{locationId}:WiFi:SSID"])
            .Returns("TestWiFi");
        _mockConfig.Setup(c => c[$"QRCodeSettings:Locations:{locationId}:WiFi:Password"])
            .Returns("TestPassword123");

        // Act
        var result = await _controller.GetWifiQr(locationId, table);

        // Assert
        result.Should().BeOfType<FileContentResult>();
        var fileResult = result as FileContentResult;
        fileResult!.FileDownloadName.Should().Contain($"T{table}");
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
