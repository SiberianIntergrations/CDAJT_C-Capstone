using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using back_end.domain.DbContexts;

namespace back_end.IntegrationTests;

public class QrCodeIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public QrCodeIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                // Add DbContext using in-memory database for testing
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Build the service provider and ensure database is created
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetWifiQr_NoAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var locationId = 1;
        var table = 5;

        // Act
        var response = await _client.GetAsync($"/api/admin/qr/wifi?locationId={locationId}&table={table}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetSessionQr_NoAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var locationId = 1;
        var table = 5;

        // Act
        var response = await _client.GetAsync($"/api/admin/qr/session?locationId={locationId}&table={table}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task BulkGenerate_NoAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var locationId = 1;

        // Act - Updated to match new API (no tableCount parameter)
        var response = await _client.PostAsync($"/api/admin/qr/bulk?locationId={locationId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Clear_NoAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var locationId = 1;
        var table = 5;

        // Act
        var response = await _client.DeleteAsync($"/api/admin/qr/clear?locationId={locationId}&table={table}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // Note: To test authenticated endpoints, you would need to:
    // 1. Create a test user in the database
    // 2. Generate a valid JWT token
    // 3. Add the token to the request headers
    // Example:
    // _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", testToken);
}
