using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.Controllers;
using back_end.controllers; // For TagController
using back_end.domain.DbContexts;
using back_end.domain.Entities;

namespace back_end.Tests.Controllers;

public class CategoryControllerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly CategoryController _controller;

    public CategoryControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _controller = new CategoryController(_context);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var categories = new List<Category>
        {
            new Category
            {
                Category_Id = 1,
                Category_Name = "Appetizers",
                Display_Order = 1
            },
            new Category
            {
                Category_Id = 2,
                Category_Name = "Main Courses",
                Display_Order = 2
            },
            new Category
            {
                Category_Id = 3,
                Category_Name = "Desserts",
                Display_Order = 3
            }
        };

        _context.Categories.AddRange(categories);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllCategories_ReturnsAllCategories()
    {
        // Act
        var result = await _controller.GetCategories();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var categories = okResult!.Value as List<Category>;

        categories.Should().NotBeNull();
        categories!.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetCategoryById_ValidId_ReturnsCategory()
    {
        // Act
        var result = await _controller.GetCategoryById(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var category = okResult!.Value as Category;

        category.Should().NotBeNull();
        category!.Category_Id.Should().Be(1);
        category.Category_Name.Should().Be("Appetizers");
    }

    [Fact]
    public async Task GetCategoryById_InvalidId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.GetCategoryById(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetCategories_OrderedByDisplayOrder()
    {
        // Act
        var result = await _controller.GetCategories();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var categories = okResult!.Value as List<Category>;

        categories.Should().NotBeNull();
        categories!.Should().BeInAscendingOrder(c => c.Display_Order);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

public class TagControllerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly TagController _controller;

    public TagControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _controller = new TagController(_context);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var tags = new List<Tag>
        {
            new Tag
            {
                Tag_Id = 1,
                Tag_Name = "Vegetarian",
                Tag_Color = "#00FF00"
            },
            new Tag
            {
                Tag_Id = 2,
                Tag_Name = "Spicy",
                Tag_Color = "#FF0000"
            },
            new Tag
            {
                Tag_Id = 3,
                Tag_Name = "Gluten-Free",
                Tag_Color = "#0000FF"
            }
        };

        _context.Tags.AddRange(tags);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllTags_ReturnsAllTags()
    {
        // Act
        var result = await _controller.GetTags();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var tags = okResult!.Value as List<Tag>;

        tags.Should().NotBeNull();
        tags!.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetTagById_ValidId_ReturnsTag()
    {
        // Act
        var result = await _controller.GetTagById(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var tag = okResult!.Value as Tag;

        tag.Should().NotBeNull();
        tag!.Tag_Id.Should().Be(1);
        tag.Tag_Name.Should().Be("Vegetarian");
    }

    [Fact]
    public async Task GetTagById_InvalidId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.GetTagById(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetTags_AllTagsHaveColors()
    {
        // Act
        var result = await _controller.GetTags();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var tags = okResult!.Value as List<Tag>;

        tags.Should().NotBeNull();
        tags!.Should().AllSatisfy(t => t.Tag_Color.Should().NotBeNullOrEmpty());
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
