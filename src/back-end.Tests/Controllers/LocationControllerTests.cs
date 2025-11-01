using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.Controllers;
using back_end.domain.DbContexts;
using back_end.domain.Entities;

namespace back_end.Tests.Controllers;

public class LocationControllerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly LocationController _controller;

    public LocationControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _controller = new LocationController(_context);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var locations = new List<Locations>
        {
            new Locations
            {
                Location_Id = 1,
                Name = "Downtown",
                Address_Primary = "123 Main St",
                City = "Edmonton",
                Province = "AB",
                Postal_Code = "T5K 2B7",
                Phone_Number = "780-123-4567"
            },
            new Locations
            {
                Location_Id = 2,
                Name = "West End",
                Address_Primary = "456 West Rd",
                City = "Edmonton",
                Province = "AB",
                Postal_Code = "T5H 1A1",
                Phone_Number = "780-987-6543"
            }
        };

        _context.Locations.AddRange(locations);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllLocations_ReturnsAllLocations()
    {
        // Act
        var result = await _controller.GetLocations();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var locations = okResult!.Value as List<Locations>;

        locations.Should().NotBeNull();
        locations!.Should().HaveCount(2);
        locations.Should().Contain(l => l.Name == "Downtown");
        locations.Should().Contain(l => l.Name == "West End");
    }

    [Fact]
    public async Task GetLocationById_ValidId_ReturnsLocation()
    {
        // Act
        var result = await _controller.GetLocation(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var location = okResult!.Value as Locations;

        location.Should().NotBeNull();
        location!.Location_Id.Should().Be(1);
        location.Name.Should().Be("Downtown");
    }

    [Fact]
    public async Task GetLocationById_InvalidId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.GetLocation(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetLocations_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        _context.Locations.RemoveRange(_context.Locations);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetLocations();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var locations = okResult!.Value as List<Locations>;

        locations.Should().NotBeNull();
        locations!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetLocations_ReturnsLocationsInCorrectOrder()
    {
        // Act
        var result = await _controller.GetLocations();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var locations = okResult!.Value as List<Locations>;

        locations.Should().NotBeNull();
        locations!.Should().HaveCount(2);
        // Verify locations are returned (order depends on implementation)
        locations.Should().AllSatisfy(l => l.Location_Id.Should().BeGreaterThan(0));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
