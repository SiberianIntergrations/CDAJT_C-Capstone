using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using back_end.Controllers;
using back_end.domain.DbContexts;
using back_end.domain.Entities;
using back_end.domain.enums;
using back_end.DTO.OrdersDTOs;
using System.Security.Claims;

namespace back_end.Tests.Controllers;

public class OrderControllerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ILogger<OrderController>> _mockLogger;
    private readonly OrderController _controller;

    public OrderControllerTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mockLogger = new Mock<ILogger<OrderController>>();
        _controller = new OrderController(_context, _mockLogger.Object);

        // Setup test data
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

        var session = new DiningSession
        {
            Session_Id = 1,
            Location_Id = 1,
            // Note: Status and Created_At properties may have changed in DiningSession entity
            // Status = SessionStatus.Active,
            // Created_At = DateTime.UtcNow
        };

        var bill = new Billing
        {
            Bill_Id = 1,
            Session_Id = 1,
            Bill_Name = "Test Bill",
            Senior_Count = 0,
            Adult_Count = 1,
            Child_Count = 0,
            Total_Count = 1,
            Status = BillStatus.Open,
            Created_At = DateTime.UtcNow
        };

        _context.Locations.Add(location);
        _context.DiningSessions.Add(session);
        _context.Bills.Add(bill);
        _context.SaveChanges();
    }

    private void SetupUserClaims(string userOid, string userName = "Test User")
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userOid),
            new Claim("name", userName),  // Use lowercase "name" claim as expected by ClaimsHelpers
            new Claim(ClaimTypes.Email, "test@example.com"),
            new Claim(ClaimTypes.Role, "Customer")
        };

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Fact]
    public async Task CreateOrder_ValidData_CreatesOrder()
    {
        // Arrange
        SetupUserClaims("test-oid-123", "Test User");

        var orderCreateDto = new OrderCreateDTO
        {
            Session_Id = 1,
            Bill_Id = 1
        };

        // Act
        var result = await _controller.CreateOrder(orderCreateDto);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var orderResponse = okResult!.Value as OrderResponseDTO;

        orderResponse.Should().NotBeNull();
        orderResponse!.Session_Id.Should().Be(1);
        orderResponse.Bill_Id.Should().Be(1);
        orderResponse.User_Oid.Should().Be("test-oid-123");
        orderResponse.User_Name.Should().Be("Test User");
        orderResponse.Status.Should().Be(OrderStatus.Pending);

        // Verify order was saved to database
        var savedOrder = await _context.SessionOrders.FirstOrDefaultAsync();
        savedOrder.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateOrder_InvalidSession_ReturnsNotFound()
    {
        // Arrange
        SetupUserClaims("test-oid-123");

        var orderCreateDto = new OrderCreateDTO
        {
            Session_Id = 999, // Non-existent session
            Bill_Id = 1
        };

        // Act
        var result = await _controller.CreateOrder(orderCreateDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreateOrder_InvalidBill_ReturnsNotFound()
    {
        // Arrange
        SetupUserClaims("test-oid-123");

        var orderCreateDto = new OrderCreateDTO
        {
            Session_Id = 1,
            Bill_Id = 999 // Non-existent bill
        };

        // Act
        var result = await _controller.CreateOrder(orderCreateDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreateOrder_ClosedBill_ReturnsNotFound()
    {
        // Arrange
        SetupUserClaims("test-oid-123");

        var closedBill = new Billing
        {
            Bill_Id = 2,
            Session_Id = 1,
            Bill_Name = "Closed Bill",
            Senior_Count = 0,
            Adult_Count = 2,
            Child_Count = 0,
            Total_Count = 2,
            Status = BillStatus.Paid, // Closed bill
            Created_At = DateTime.UtcNow
        };

        _context.Bills.Add(closedBill);
        await _context.SaveChangesAsync();

        var orderCreateDto = new OrderCreateDTO
        {
            Session_Id = 1,
            Bill_Id = 2
        };

        // Act
        var result = await _controller.CreateOrder(orderCreateDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreateOrder_SetsCorrectTimestamp()
    {
        // Arrange
        SetupUserClaims("test-oid-123");
        var beforeCreation = DateTime.UtcNow;

        var orderCreateDto = new OrderCreateDTO
        {
            Session_Id = 1,
            Bill_Id = 1
        };

        // Act
        await _controller.CreateOrder(orderCreateDto);

        // Assert
        var savedOrder = await _context.SessionOrders.FirstOrDefaultAsync();
        savedOrder.Should().NotBeNull();
        savedOrder!.Created_At.Should().BeAfter(beforeCreation);
        savedOrder.Created_At.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("test-oid-123", "User One")]
    [InlineData("test-oid-456", "User Two")]
    public async Task CreateOrder_DifferentUsers_CreatesOrdersWithCorrectUserOid(string userOid, string userName)
    {
        // Arrange
        SetupUserClaims(userOid, userName);

        var orderCreateDto = new OrderCreateDTO
        {
            Session_Id = 1,
            Bill_Id = 1
        };

        // Act
        var result = await _controller.CreateOrder(orderCreateDto);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var orderResponse = okResult!.Value as OrderResponseDTO;

        orderResponse.Should().NotBeNull();
        orderResponse!.User_Oid.Should().Be(userOid);
        orderResponse.User_Name.Should().Be(userName);
    }

    [Fact]
    public async Task CreateOrder_MultipleOrders_AllCreatedSuccessfully()
    {
        // Arrange
        SetupUserClaims("test-oid-123");

        // Act - Create 3 orders
        for (int i = 0; i < 3; i++)
        {
            var orderCreateDto = new OrderCreateDTO
            {
                Session_Id = 1,
                Bill_Id = 1
            };
            await _controller.CreateOrder(orderCreateDto);
        }

        // Assert
        var orders = await _context.SessionOrders.ToListAsync();
        orders.Should().HaveCount(3);
        orders.Should().AllSatisfy(o => o.Status.Should().Be(OrderStatus.Pending));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
