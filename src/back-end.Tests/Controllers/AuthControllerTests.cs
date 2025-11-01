using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using back_end.controllers;
using back_end.domain.DbContexts;
using back_end.domain.Entities;
using back_end.domain.enums;
using back_end.DTO.Auth;
using BCryptNet = BCrypt.Net.BCrypt;

namespace back_end.Tests.Controllers;

public class AuthControllerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mockConfig = new Mock<IConfiguration>();

        // Setup JWT configuration
        _mockConfig.Setup(c => c["Jwt:Key"]).Returns("ThisIsAVerySecureKeyForTestingPurposesOnly123456");
        _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        _mockConfig.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
        _mockConfig.Setup(c => c["Jwt:ExpiresInMinutes"]).Returns("60");

        _controller = new AuthController(_context, _mockConfig.Object);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsJwtToken()
    {
        // Arrange
        var password = "TestPassword123";
        var hashedPassword = BCryptNet.HashPassword(password);

        var user = new User
        {
            User_Id = 1,
            Email = "test@example.com",
            Password_hash = hashedPassword,
            First_name = "Test",
            Last_name = "User",
            Role = UserRole.Customer,
            Status = UserStatus.Active,
            Is_email_confirmed = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var loginRequest = new LoginDTO
        {
            UserEmail = "test@example.com",
            UserPassword = password
        };

        // Act
        var result = await _controller.Login(loginRequest);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();

        // Verify the response contains token properties
        var value = okResult.Value as dynamic;
        Assert.NotNull(value);
    }

    [Fact]
    public async Task Login_InvalidEmail_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginDTO
        {
            UserEmail = "nonexistent@example.com",
            UserPassword = "password"
        };

        // Act
        var result = await _controller.Login(loginRequest);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var password = "CorrectPassword123";
        var hashedPassword = BCryptNet.HashPassword(password);

        var user = new User
        {
            User_Id = 1,
            Email = "test@example.com",
            Password_hash = hashedPassword,
            First_name = "Test",
            Last_name = "User",
            Role = UserRole.Customer,
            Status = UserStatus.Active
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var loginRequest = new LoginDTO
        {
            UserEmail = "test@example.com",
            UserPassword = "WrongPassword"
        };

        // Act
        var result = await _controller.Login(loginRequest);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Login_InactiveUser_ReturnsBadRequest()
    {
        // Arrange
        var password = "TestPassword123";
        var hashedPassword = BCryptNet.HashPassword(password);

        var user = new User
        {
            User_Id = 1,
            Email = "inactive@example.com",
            Password_hash = hashedPassword,
            First_name = "Inactive",
            Last_name = "User",
            Role = UserRole.Customer,
            Status = UserStatus.Inactive
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var loginRequest = new LoginDTO
        {
            UserEmail = "inactive@example.com",
            UserPassword = password
        };

        // Act
        var result = await _controller.Login(loginRequest);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Login_UpdatesLastInteractionTimestamp()
    {
        // Arrange
        var password = "TestPassword123";
        var hashedPassword = BCryptNet.HashPassword(password);
        var initialTime = DateTime.UtcNow.AddDays(-1);

        var user = new User
        {
            User_Id = 1,
            Email = "test@example.com",
            Password_hash = hashedPassword,
            First_name = "Test",
            Last_name = "User",
            Role = UserRole.Customer,
            Status = UserStatus.Active,
            Last_Interaction_at = initialTime
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var loginRequest = new LoginDTO
        {
            UserEmail = "test@example.com",
            UserPassword = password
        };

        // Act
        await _controller.Login(loginRequest);

        // Assert
        var updatedUser = await _context.Users.FindAsync(1);
        updatedUser!.Last_Interaction_at.Should().BeAfter(initialTime);
    }

    [Fact]
    public async Task Register_ValidData_CreatesUser()
    {
        // Arrange
        var registerRequest = new RegisterDTO
        {
            UserEmail = "newuser@example.com",
            UserPassword = "Password123",
            FirstName = "New",
            LastName = "User"
        };

        // Act
        var result = await _controller.CreateUser(registerRequest);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var createdUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "newuser@example.com");
        createdUser.Should().NotBeNull();
        createdUser!.First_name.Should().Be("New");
        createdUser.Last_name.Should().Be("User");
        createdUser.Is_email_confirmed.Should().BeFalse();
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsConflict()
    {
        // Arrange
        var existingUser = new User
        {
            User_Id = 1,
            Email = "existing@example.com",
            Password_hash = BCryptNet.HashPassword("password"),
            First_name = "Existing",
            Last_name = "User",
            Role = UserRole.Customer,
            Status = UserStatus.Active
        };

        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var registerRequest = new RegisterDTO
        {
            UserEmail = "existing@example.com",
            UserPassword = "Password123",
            FirstName = "New",
            LastName = "User"
        };

        // Act
        var result = await _controller.CreateUser(registerRequest);

        // Assert
        result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task Register_ShortPassword_ReturnsBadRequest()
    {
        // Arrange
        var registerRequest = new RegisterDTO
        {
            UserEmail = "newuser@example.com",
            UserPassword = "12345", // Less than 6 characters
            FirstName = "New",
            LastName = "User"
        };

        // Act
        var result = await _controller.CreateUser(registerRequest);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Register_MissingFields_ReturnsBadRequest()
    {
        // Arrange
        var registerRequest = new RegisterDTO
        {
            UserEmail = "newuser@example.com",
            UserPassword = "Password123",
            FirstName = "", // Empty first name
            LastName = "User"
        };

        // Act
        var result = await _controller.CreateUser(registerRequest);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Register_NullRequest_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.CreateUser(null!);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Register_EmailCaseInsensitive_ConvertsToLowerCase()
    {
        // Arrange
        var registerRequest = new RegisterDTO
        {
            UserEmail = "NewUser@Example.COM",
            UserPassword = "Password123",
            FirstName = "New",
            LastName = "User"
        };

        // Act
        await _controller.CreateUser(registerRequest);

        // Assert
        var createdUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "newuser@example.com");
        createdUser.Should().NotBeNull();
    }

    [Fact]
    public async Task Register_PasswordIsHashed()
    {
        // Arrange
        var plainPassword = "Password123";
        var registerRequest = new RegisterDTO
        {
            UserEmail = "newuser@example.com",
            UserPassword = plainPassword,
            FirstName = "New",
            LastName = "User"
        };

        // Act
        await _controller.CreateUser(registerRequest);

        // Assert
        var createdUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "newuser@example.com");
        createdUser.Should().NotBeNull();
        createdUser!.Password_hash.Should().NotBe(plainPassword);

        // Verify BCrypt can verify the password
        BCryptNet.Verify(plainPassword, createdUser.Password_hash).Should().BeTrue();
    }

    [Theory]
    [InlineData("user@example.com  ", "  Password123", "  John", "  Doe")]
    [InlineData("  user@example.com", "Password123  ", "John  ", "Doe  ")]
    public async Task Register_TrimsWhitespace(string email, string password, string firstName, string lastName)
    {
        // Arrange
        var registerRequest = new RegisterDTO
        {
            UserEmail = email,
            UserPassword = password,
            FirstName = firstName,
            LastName = lastName
        };

        // Act
        var result = await _controller.CreateUser(registerRequest);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var createdUser = await _context.Users.FirstOrDefaultAsync();
        createdUser.Should().NotBeNull();
        createdUser!.Email.Should().NotContain(" ");
        createdUser.First_name.Should().Be(firstName.Trim());
        createdUser.Last_name.Should().Be(lastName.Trim());
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
