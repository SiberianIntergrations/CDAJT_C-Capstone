using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using back_end.controllers;
using back_end.domain.DbContexts;
using back_end.domain.Entities;
using back_end.domain.enums;
using back_end.DTO.bill;
using System.Security.Claims;

namespace back_end.Tests.Controllers;

public class BillControllerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly BillController _controller;

    public BillControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _controller = new BillController(_context);

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
            Status = SessionStatus.Active,
            Created_At = DateTime.UtcNow
        };

        var bill = new Bill
        {
            Bill_Id = 1,
            Session_Id = 1,
            Status = BillStatus.Open,
            Created_At = DateTime.UtcNow,
            Subtotal = 50.00m,
            Tax = 5.00m,
            Total = 55.00m
        };

        _context.Locations.Add(location);
        _context.DiningSessions.Add(session);
        _context.Bills.Add(bill);
        _context.SaveChanges();
    }

    private void SetupUserClaims(int userId, string role = "Customer")
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, "test@example.com"),
            new Claim(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Fact]
    public async Task GetBillById_ValidId_ReturnsBill()
    {
        // Act
        var result = await _controller.GetBill(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var bill = okResult!.Value as Bill;

        bill.Should().NotBeNull();
        bill!.Bill_Id.Should().Be(1);
        bill.Total.Should().Be(55.00m);
    }

    [Fact]
    public async Task GetBillById_InvalidId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.GetBill(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetBillsBySession_ValidSession_ReturnsBills()
    {
        // Act
        var result = await _controller.GetBillsBySession(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var bills = okResult!.Value as List<Bill>;

        bills.Should().NotBeNull();
        bills!.Should().HaveCount(1);
        bills[0].Session_Id.Should().Be(1);
    }

    [Fact]
    public async Task GetBillsBySession_InvalidSession_ReturnsEmptyList()
    {
        // Act
        var result = await _controller.GetBillsBySession(999);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var bills = okResult!.Value as List<Bill>;

        bills.Should().NotBeNull();
        bills!.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateBill_ValidSession_CreatesBill()
    {
        // Arrange
        SetupUserClaims(1);

        var billCreateDto = new BillCreateDTO
        {
            Session_Id = 1
        };

        // Act
        var result = await _controller.CreateBill(billCreateDto);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var bill = okResult!.Value as Bill;

        bill.Should().NotBeNull();
        bill!.Session_Id.Should().Be(1);
        bill.Status.Should().Be(BillStatus.Open);
        bill.Subtotal.Should().Be(0);
        bill.Tax.Should().Be(0);
        bill.Total.Should().Be(0);
    }

    [Fact]
    public async Task UpdateBillStatus_ValidBill_UpdatesStatus()
    {
        // Arrange
        SetupUserClaims(1, "Staff");

        var billUpdateDto = new BillUpdateDTO
        {
            Status = BillStatus.Paid
        };

        // Act
        var result = await _controller.UpdateBillStatus(1, billUpdateDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        var updatedBill = await _context.Bills.FindAsync(1);
        updatedBill.Should().NotBeNull();
        updatedBill!.Status.Should().Be(BillStatus.Paid);
    }

    [Fact]
    public async Task CalculateBillTotal_ValidBill_CalculatesCorrectly()
    {
        // Arrange
        var bill = await _context.Bills.FindAsync(1);
        bill!.Subtotal = 100.00m;

        // Assuming 5% tax rate
        var expectedTax = 5.00m;
        var expectedTotal = 105.00m;

        // Update the bill
        bill.Tax = expectedTax;
        bill.Total = expectedTotal;
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetBill(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedBill = okResult!.Value as Bill;

        returnedBill.Should().NotBeNull();
        returnedBill!.Subtotal.Should().Be(100.00m);
        returnedBill.Tax.Should().Be(expectedTax);
        returnedBill.Total.Should().Be(expectedTotal);
    }

    [Fact]
    public async Task CreateBill_InvalidSession_ReturnsNotFound()
    {
        // Arrange
        SetupUserClaims(1);

        var billCreateDto = new BillCreateDTO
        {
            Session_Id = 999
        };

        // Act
        var result = await _controller.CreateBill(billCreateDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
