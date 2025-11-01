using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.Controllers;
using back_end.domain.DbContexts;
using back_end.domain.Entities;

namespace back_end.Tests.Controllers;

public class MenuControllerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly MenuController _controller;

    public MenuControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _controller = new MenuController(_context);

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
            Menu_Id = 1,
            Menu_Name = "Main Menu",
            Description = "Main menu for testing"
        };

        var category = new Category
        {
            Category_Id = 1,
            Category_Name = "Appetizers",
            Display_Order = 1
        };

        _context.Locations.Add(location);
        _context.Menus.Add(menu);
        _context.Categories.Add(category);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllMenus_ReturnsAllMenus()
    {
        // Act
        var result = await _controller.GetAllMenus();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var menus = okResult!.Value as List<Menu>;

        menus.Should().NotBeNull();
        menus!.Should().HaveCount(1);
        menus[0].Menu_Name.Should().Be("Main Menu");
    }

    [Fact]
    public async Task GetMenuById_ValidId_ReturnsMenu()
    {
        // Act
        var result = await _controller.GetMenuById(1);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var menu = okResult!.Value as Menu;

        menu.Should().NotBeNull();
        menu!.Menu_Id.Should().Be(1);
        menu.Menu_Name.Should().Be("Main Menu");
    }

    [Fact]
    public async Task GetMenuById_InvalidId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.GetMenuById(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetAllMenus_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        _context.Menus.RemoveRange(_context.Menus);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetAllMenus();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
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
