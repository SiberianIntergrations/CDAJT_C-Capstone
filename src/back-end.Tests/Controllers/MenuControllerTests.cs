using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using back_end.Controllers;
using back_end.domain.DbContexts;
using back_end.domain.Entities;

namespace back_end.Tests.Controllers;

public class MenuControllerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly MenuController _controller;
    private readonly Mock<ILogger<MenuController>> _mockLogger;

    public MenuControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mockLogger = new Mock<ILogger<MenuController>>();
        _controller = new MenuController(_context, _mockLogger.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var location = new Locations
        {
            Location_Id = 1,
            Name = "Test Location",
            Address_Primary = "123 Test St",
            City = "Test City",
            Province = "TC",
            Postal_Code = "T1T 1T1",
            Phone_Number = "123-456-7890"
        };

        var menu = new Menu
        {
            Menu_id = 1,
            Name = "Main Menu",
            Description = "Main menu for testing",
            Start_time = new TimeOnly(8, 0),
            End_time = new TimeOnly(22, 0),
            Is_active = true
        };

        var category = new Category
        {
            Category_id = 1,
            Category_name = "Appetizers",
            Description = "Starters and appetizers"
        };

        _context.Locations.Add(location);
        _context.Menus.Add(menu);
        _context.Categories.Add(category);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllMenu_ReturnsAllMenus()
    {
        // Act
        var result = await _controller.GetAllMenu();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var menus = okResult!.Value as List<Menu>;

        menus.Should().NotBeNull();
        menus!.Should().HaveCount(1);
        menus[0].Name.Should().Be("Main Menu");
    }

    [Fact]
    public async Task GetMenuByID_ValidId_ReturnsMenu()
    {
        // Act
        var result = await _controller.GetMenuByID(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var menu = okResult!.Value as Menu;

        menu.Should().NotBeNull();
        menu!.Menu_id.Should().Be(1);
        menu.Name.Should().Be("Main Menu");
    }

    [Fact]
    public async Task GetMenuByID_InvalidId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.GetMenuByID(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetAllMenu_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        _context.Menus.RemoveRange(_context.Menus);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetAllMenu();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var menus = okResult!.Value as List<Menu>;

        menus.Should().NotBeNull();
        menus!.Should().BeEmpty();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
