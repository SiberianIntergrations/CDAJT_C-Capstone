# Automated Testing Strategy - Sushi Toshi Restaurant Management System

## Table of Contents
1. [Overview](#overview)
2. [Back-End Testing (C# .NET)](#back-end-testing-c-net)
3. [Front-End Testing (Next.js/React)](#front-end-testing-nextjsreact)
4. [QR Code System Specific Tests](#qr-code-system-specific-tests)
5. [Integration Testing](#integration-testing)
6. [End-to-End Testing](#end-to-end-testing)
7. [Test Coverage Goals](#test-coverage-goals)
8. [CI/CD Integration](#cicd-integration)
9. [Implementation Priority](#implementation-priority)

---

## Overview

A comprehensive testing strategy for a restaurant management system with multiple user roles (Customer, Staff, Admin) and critical features like ordering, billing, and QR code generation.

### Testing Pyramid Strategy:

```
        /\
       /  \      E2E Tests (10%)
      /    \     - Critical user flows
     /------\
    /        \   Integration Tests (20%)
   /          \  - API + Database
  /------------\
 /              \ Unit Tests (70%)
/________________\- Business logic, services
```

---

## Back-End Testing (C# .NET)

### 1. **Unit Tests**

#### Testing Framework:
- **xUnit** (recommended for .NET)
- **NUnit** (alternative)
- **Moq** for mocking dependencies
- **FluentAssertions** for readable assertions

#### What to Test:

##### A. **Service Layer Tests**
Test all business logic in services:

```csharp
// Example: QrGeneratorService Unit Tests
public class QrGeneratorServiceTests
{
    private readonly Mock<ApplicationDbContext> _mockContext;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<ILogger<QrGeneratorService>> _mockLogger;
    private readonly QrGeneratorService _service;

    [Fact]
    public async Task GetLocationWifiCredentials_ValidLocation_ReturnsCredentials()
    {
        // Arrange
        var locationId = 1;
        var expectedSsid = "TestWiFi";
        var expectedPassword = "TestPassword";

        // Setup mock data
        var mockLocation = new Location { Location_Id = 1, Name = "Test Location" };
        _mockContext.Setup(c => c.Locations.FindAsync(locationId))
            .ReturnsAsync(mockLocation);

        _mockConfig.Setup(c => c[$"QRCodeSettings:Locations:{locationId}:WiFi:SSID"])
            .Returns(expectedSsid);
        _mockConfig.Setup(c => c[$"QRCodeSettings:Locations:{locationId}:WiFi:Password"])
            .Returns(expectedPassword);

        // Act
        var result = await _service.GetLocationWifiCredentials(locationId);

        // Assert
        result.Should().NotBeNull();
        result.Value.ssid.Should().Be(expectedSsid);
        result.Value.password.Should().Be(expectedPassword);
    }

    [Fact]
    public async Task GetLocationWifiCredentials_InvalidLocation_ReturnsNull()
    {
        // Arrange
        var locationId = 999;
        _mockContext.Setup(c => c.Locations.FindAsync(locationId))
            .ReturnsAsync((Location)null);

        // Act
        var result = await _service.GetLocationWifiCredentials(locationId);

        // Assert
        result.Should().BeNull();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("not found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public void CreateWifiQr_ValidCredentials_ReturnsQrCodeBitmap()
    {
        // Arrange
        var ssid = "TestSSID";
        var password = "TestPassword";

        // Act
        using var result = _service.CreateWifiQr(ssid, password, false);

        // Assert
        result.Should().NotBeNull();
        result.Width.Should().BeGreaterThan(0);
        result.Height.Should().BeGreaterThan(0);
    }

    [Fact]
    public void GetAuthUrl_ValidParameters_ReturnsCorrectUrl()
    {
        // Arrange
        var locationId = 1;
        var tableNumber = 5;
        var baseUrl = "http://localhost:3000";
        _mockConfig.Setup(c => c["QRCodeSettings:AuthPageUrl"])
            .Returns(baseUrl + "/login");

        // Act
        var result = _service.GetAuthUrl(locationId, tableNumber);

        // Assert
        result.Should().Be($"{baseUrl}/login?location={locationId}&table={tableNumber}");
    }
}
```

##### B. **Controller Tests**
Test API endpoints and routing:

```csharp
public class AdminQrCodeControllerTests
{
    private readonly Mock<QrGeneratorService> _mockQrService;
    private readonly Mock<ILogger<AdminQrCodeController>> _mockLogger;
    private readonly AdminQrCodeController _controller;

    [Fact]
    public async Task GetWifiQr_ValidRequest_ReturnsFileResult()
    {
        // Arrange
        var locationId = 1;
        var table = 5;
        var mockCredentials = ("TestSSID", "TestPassword");

        _mockQrService.Setup(s => s.GetLocationWifiCredentials(locationId))
            .ReturnsAsync(mockCredentials);

        // Act
        var result = await _controller.GetWifiQr(locationId, table);

        // Assert
        result.Should().BeOfType<FileContentResult>();
        var fileResult = result as FileContentResult;
        fileResult.ContentType.Should().Be("image/png");
        fileResult.FileDownloadName.Should().Be($"wifi_L{locationId}_T{table}.png");
    }

    [Fact]
    public async Task GetWifiQr_InvalidLocation_ReturnsNotFound()
    {
        // Arrange
        var locationId = 999;
        var table = 5;

        _mockQrService.Setup(s => s.GetLocationWifiCredentials(locationId))
            .ReturnsAsync((ValueTuple<string, string>?)null);

        // Act
        var result = await _controller.GetWifiQr(locationId, table);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
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
    public async Task BulkGenerateQrCodes_ValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var locationId = 1;
        var tableCount = 10;

        var mockLocation = new Location { Location_Id = 1, Name = "Test" };
        _mockQrService._context.Locations.FindAsync(locationId)
            .Returns(Task.FromResult(mockLocation));

        // Act
        var result = await _controller.BulkGenerateQrCodes(locationId, tableCount);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().NotBeNull();
    }
}
```

##### C. **Domain/Entity Tests**
Test entity validation and business rules:

```csharp
public class LocationTests
{
    [Fact]
    public void Location_ValidData_CreatesSuccessfully()
    {
        // Arrange & Act
        var location = new Location
        {
            Location_Id = 1,
            Name = "Downtown",
            Address_Primary = "123 Main St",
            City = "Edmonton",
            Province = "AB",
            Postal_Code = "T5K 2B7",
            Phone_Number = "780-123-4567"
        };

        // Assert
        location.Name.Should().Be("Downtown");
        location.City.Should().Be("Edmonton");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Location_InvalidName_ThrowsValidationException(string invalidName)
    {
        // Arrange & Act & Assert
        var act = () => new Location { Name = invalidName };
        act.Should().Throw<ValidationException>();
    }
}
```

##### D. **Authentication & Authorization Tests**
```csharp
public class AuthControllerTests
{
    [Fact]
    public async Task Login_ValidCredentials_ReturnsJwtToken()
    {
        // Test JWT generation
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Test failed login
    }

    [Fact]
    public async Task RefreshToken_ValidToken_ReturnsNewToken()
    {
        // Test token refresh
    }
}
```

##### E. **Database Seeder Tests**
```csharp
public class DatabaseSeederTests
{
    [Fact]
    public async Task SeedDatabase_EmptyDatabase_SeedsSuccessfully()
    {
        // Test seeding logic
    }

    [Fact]
    public async Task SeedDatabase_AlreadySeeded_DoesNotDuplicate()
    {
        // Test idempotency
    }
}
```

---

### 2. **Integration Tests**

Test interactions between components, including database operations.

#### Setup:
- **In-memory database** for fast tests
- **TestContainers** for MySQL/MariaDB tests
- **WebApplicationFactory** for API testing

```csharp
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
                // Replace with in-memory database
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetWifiQr_EndToEnd_ReturnsQrCodeImage()
    {
        // Arrange
        var locationId = 1;
        var table = 5;

        // Add auth token
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", GenerateTestToken("Admin"));

        // Act
        var response = await _client.GetAsync($"/api/admin/qr/wifi?locationId={locationId}&table={table}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType.MediaType.Should().Be("image/png");

        var content = await response.Content.ReadAsByteArrayAsync();
        content.Length.Should().BeGreaterThan(0);
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
    public async Task GetWifiQr_CustomerRole_ReturnsForbidden()
    {
        // Arrange
        var locationId = 1;
        var table = 5;

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", GenerateTestToken("Customer"));

        // Act
        var response = await _client.GetAsync($"/api/admin/qr/wifi?locationId={locationId}&table={table}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
```

---

### 3. **Repository Pattern Tests**

If using repository pattern:

```csharp
public class LocationRepositoryTests
{
    [Fact]
    public async Task GetLocationById_ExistingLocation_ReturnsLocation()
    {
        // Test data access layer
    }

    [Fact]
    public async Task CreateLocation_ValidLocation_SavesSuccessfully()
    {
        // Test create operation
    }
}
```

---

## Front-End Testing (Next.js/React)

### 1. **Unit Tests**

#### Testing Framework:
- **Jest** for test runner
- **React Testing Library** for component testing
- **MSW (Mock Service Worker)** for API mocking

#### What to Test:

##### A. **Component Tests**

```javascript
// QRCodeManagement.test.jsx
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import '@testing-library/jest-dom';
import QRCodeManagement from '../QRCodeManagement';
import { api } from '@/config/api';

jest.mock('@/config/api');

describe('QRCodeManagement Component', () => {
  beforeEach(() => {
    // Mock API responses
    api.get.mockImplementation((url) => {
      if (url === '/location') {
        return Promise.resolve({
          data: [
            { location_Id: 1, name: 'Downtown', city: 'Edmonton' },
            { location_Id: 2, name: 'West End', city: 'Edmonton' }
          ]
        });
      }
      return Promise.reject(new Error('Not found'));
    });
  });

  afterEach(() => {
    jest.clearAllMocks();
  });

  test('renders QR Code Management title', () => {
    render(<QRCodeManagement />);
    expect(screen.getByText(/QR Code Management/i)).toBeInTheDocument();
  });

  test('fetches and displays locations on mount', async () => {
    render(<QRCodeManagement />);

    await waitFor(() => {
      expect(screen.getByText(/Downtown/i)).toBeInTheDocument();
      expect(screen.getByText(/West End/i)).toBeInTheDocument();
    });
  });

  test('shows warning when downloading without table number', async () => {
    render(<QRCodeManagement />);

    const downloadButton = screen.getAllByText(/Download/i)[0];
    fireEvent.click(downloadButton);

    await waitFor(() => {
      expect(screen.getByText(/Please select a location and enter a table number/i))
        .toBeInTheDocument();
    });
  });

  test('downloads WiFi QR code when valid input provided', async () => {
    // Mock blob creation
    global.URL.createObjectURL = jest.fn(() => 'blob:test');
    global.URL.revokeObjectURL = jest.fn();

    api.get.mockResolvedValueOnce({
      data: new Blob(['test'], { type: 'image/png' })
    });

    render(<QRCodeManagement />);

    // Select location
    const locationSelect = screen.getByLabelText(/Location/i);
    fireEvent.change(locationSelect, { target: { value: '1' } });

    // Enter table number
    const tableInput = screen.getByLabelText(/Table Number/i);
    fireEvent.change(tableInput, { target: { value: '5' } });

    // Click download
    const downloadButton = screen.getAllByText(/Download/i)[0];
    fireEvent.click(downloadButton);

    await waitFor(() => {
      expect(api.get).toHaveBeenCalledWith('/admin/qr/wifi', {
        params: { locationId: 1, table: '5' },
        responseType: 'blob'
      });
    });
  });

  test('opens preview dialog when preview button clicked', async () => {
    api.get.mockResolvedValueOnce({
      data: new Blob(['test'], { type: 'image/png' })
    });

    render(<QRCodeManagement />);

    // Setup
    const locationSelect = screen.getByLabelText(/Location/i);
    fireEvent.change(locationSelect, { target: { value: '1' } });

    const tableInput = screen.getByLabelText(/Table Number/i);
    fireEvent.change(tableInput, { target: { value: '5' } });

    // Click preview
    const previewButton = screen.getAllByText(/Preview/i)[0];
    fireEvent.click(previewButton);

    await waitFor(() => {
      expect(screen.getByRole('dialog')).toBeInTheDocument();
    });
  });

  test('bulk generation sends correct API request', async () => {
    api.post.mockResolvedValueOnce({
      data: { filesGenerated: 20, success: true }
    });

    render(<QRCodeManagement />);

    // Enter bulk count
    const bulkInput = screen.getByLabelText(/Number of Tables/i);
    fireEvent.change(bulkInput, { target: { value: '10' } });

    // Click bulk generate
    const bulkButton = screen.getByText(/Generate Bulk QR Codes/i);
    fireEvent.click(bulkButton);

    await waitFor(() => {
      expect(api.post).toHaveBeenCalledWith('/admin/qr/bulk', null, {
        params: { locationId: expect.any(Number), tableCount: 10 }
      });
    });
  });

  test('displays error message on API failure', async () => {
    api.get.mockRejectedValueOnce({
      response: { data: { message: 'Server error' } }
    });

    render(<QRCodeManagement />);

    const locationSelect = screen.getByLabelText(/Location/i);
    fireEvent.change(locationSelect, { target: { value: '1' } });

    const tableInput = screen.getByLabelText(/Table Number/i);
    fireEvent.change(tableInput, { target: { value: '5' } });

    const downloadButton = screen.getAllByText(/Download/i)[0];
    fireEvent.click(downloadButton);

    await waitFor(() => {
      expect(screen.getByText(/Failed to download/i)).toBeInTheDocument();
    });
  });
});
```

##### B. **Hook Tests**

```javascript
// useAuth.test.js
import { renderHook, act } from '@testing-library/react';
import useAuth from '../useAuth';

describe('useAuth Hook', () => {
  test('returns authentication state', () => {
    const { result } = renderHook(() => useAuth());

    expect(result.current.isAuthenticated).toBeDefined();
    expect(result.current.user).toBeDefined();
  });

  test('login updates authentication state', async () => {
    const { result } = renderHook(() => useAuth());

    await act(async () => {
      await result.current.login('test@example.com', 'password');
    });

    expect(result.current.isAuthenticated).toBe(true);
  });
});
```

##### C. **Utility Function Tests**

```javascript
// token.test.js
import { getAccessToken, setAuthTokens, clearAuthTokens } from '../token';

describe('Token Utilities', () => {
  beforeEach(() => {
    localStorage.clear();
  });

  test('setAuthTokens stores tokens in localStorage', () => {
    const accessToken = 'test_access_token';
    const refreshToken = 'test_refresh_token';

    setAuthTokens(accessToken, refreshToken);

    expect(localStorage.getItem('accessToken')).toBe(accessToken);
    expect(localStorage.getItem('refreshToken')).toBe(refreshToken);
  });

  test('getAccessToken retrieves token from localStorage', () => {
    localStorage.setItem('accessToken', 'test_token');

    const token = getAccessToken();

    expect(token).toBe('test_token');
  });

  test('clearAuthTokens removes all tokens', () => {
    localStorage.setItem('accessToken', 'test');
    localStorage.setItem('refreshToken', 'test');

    clearAuthTokens();

    expect(localStorage.getItem('accessToken')).toBeNull();
    expect(localStorage.getItem('refreshToken')).toBeNull();
  });
});
```

---

### 2. **Integration Tests (Front-End)**

Test component interactions with API:

```javascript
// QRCodeManagement.integration.test.jsx
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { setupServer } from 'msw/node';
import { rest } from 'msw';
import QRCodeManagement from '../QRCodeManagement';

const server = setupServer(
  rest.get('http://localhost:5264/api/location', (req, res, ctx) => {
    return res(ctx.json([
      { location_Id: 1, name: 'Downtown', city: 'Edmonton' }
    ]));
  }),

  rest.get('http://localhost:5264/api/admin/qr/wifi', (req, res, ctx) => {
    const locationId = req.url.searchParams.get('locationId');
    const table = req.url.searchParams.get('table');

    return res(
      ctx.set('Content-Type', 'image/png'),
      ctx.body(new ArrayBuffer(100))
    );
  })
);

beforeAll(() => server.listen());
afterEach(() => server.resetHandlers());
afterAll(() => server.close());

describe('QRCodeManagement Integration Tests', () => {
  test('full workflow: select location, enter table, download QR', async () => {
    render(<QRCodeManagement />);

    // Wait for locations to load
    await waitFor(() => {
      expect(screen.getByText(/Downtown/i)).toBeInTheDocument();
    });

    // Select location
    const locationSelect = screen.getByLabelText(/Location/i);
    fireEvent.change(locationSelect, { target: { value: '1' } });

    // Enter table number
    const tableInput = screen.getByLabelText(/Table Number/i);
    fireEvent.change(tableInput, { target: { value: '5' } });

    // Download
    const downloadButton = screen.getAllByText(/Download/i)[0];
    fireEvent.click(downloadButton);

    // Verify success message
    await waitFor(() => {
      expect(screen.getByText(/downloaded successfully/i)).toBeInTheDocument();
    });
  });
});
```

---

## QR Code System Specific Tests

### Critical Test Scenarios:

#### 1. **QR Code Generation Tests**
```csharp
[Fact]
public void GenerateWifiQr_ValidSsidAndPassword_CreatesValidWifiString()
{
    // Test WiFi QR format: WIFI:T:WPA;S:ssid;P:password;H:false;;
}

[Fact]
public void GenerateAuthQr_ValidUrl_CreatesQrWithCorrectUrl()
{
    // Test URL QR format
}

[Fact]
public void RenderLabeledQr_AddLabel_IncludesLocationAndTable()
{
    // Test label rendering
}
```

#### 2. **Location-Based WiFi Tests**
```csharp
[Theory]
[InlineData(1, "Location1-WiFi", "password1")]
[InlineData(2, "Location2-WiFi", "password2")]
public async Task GetLocationWifiCredentials_DifferentLocations_ReturnsCorrectCredentials(
    int locationId, string expectedSsid, string expectedPassword)
{
    // Test location-specific WiFi credentials
}

[Fact]
public async Task GetLocationWifiCredentials_NoLocationSpecific_ReturnsFallback()
{
    // Test fallback to default WiFi
}
```

#### 3. **Caching Tests**
```csharp
[Fact]
public void QrCodeExists_CachedFile_ReturnsTrue()
{
    // Test cache hit
}

[Fact]
public void QrCodeExists_NotCached_ReturnsFalse()
{
    // Test cache miss
}

[Fact]
public void DeleteQrCode_ExistingCache_RemovesFile()
{
    // Test cache clearing
}
```

#### 4. **Authorization Tests**
```csharp
[Theory]
[InlineData("Admin", HttpStatusCode.OK)]
[InlineData("Staff", HttpStatusCode.OK)]
[InlineData("Customer", HttpStatusCode.Forbidden)]
public async Task GetWifiQr_DifferentRoles_ReturnsExpectedStatusCode(
    string role, HttpStatusCode expectedStatus)
{
    // Test role-based access
}
```

---

## Integration Testing

### Database Integration Tests:

```csharp
public class DatabaseIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public DatabaseIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task CreateOrder_WithMenuItems_SavesCorrectly()
    {
        // Test full order creation flow
    }

    [Fact]
    public async Task UpdateBill_WithPayment_UpdatesStatus()
    {
        // Test bill payment flow
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
```

---

## End-to-End Testing

### Framework:
- **Playwright** (recommended) or **Cypress**

### Critical User Flows:

```javascript
// e2e/qr-code-generation.spec.js
import { test, expect } from '@playwright/test';

test.describe('QR Code Generation E2E', () => {
  test.beforeEach(async ({ page }) => {
    // Login as admin
    await page.goto('http://localhost:3000/login');
    await page.fill('[name="email"]', 'admin@sushitoshi.com');
    await page.fill('[name="password"]', 'admin123');
    await page.click('button[type="submit"]');
    await page.waitForURL('**/dashboard');
  });

  test('admin can generate and download WiFi QR code', async ({ page }) => {
    // Navigate to QR codes page
    await page.click('button[aria-label="menu"]');
    await page.click('text=QR Codes');
    await expect(page).toHaveURL('**/admin/qr-codes');

    // Select location
    await page.click('[data-testid="location-select"]');
    await page.click('text=Downtown');

    // Enter table number
    await page.fill('[data-testid="table-input"]', '5');

    // Download WiFi QR code
    const downloadPromise = page.waitForEvent('download');
    await page.click('text=Download >> nth=0');
    const download = await downloadPromise;

    // Verify download
    expect(download.suggestedFilename()).toMatch(/wifi_L\d+_T5\.png/);
  });

  test('staff can bulk generate QR codes', async ({ page }) => {
    // Navigate to QR codes page
    await page.goto('http://localhost:3000/admin/qr-codes');

    // Select location
    await page.selectOption('[data-testid="location-select"]', '1');

    // Enter table count
    await page.fill('[data-testid="bulk-count-input"]', '10');

    // Generate bulk
    await page.click('text=Generate Bulk QR Codes');

    // Verify success message
    await expect(page.locator('text=Successfully generated')).toBeVisible();
  });

  test('customer cannot access QR code page', async ({ page }) => {
    // Logout
    await page.click('[data-testid="logout-button"]');

    // Login as customer
    await page.fill('[name="email"]', 'customer@test.com');
    await page.fill('[name="password"]', 'customer123');
    await page.click('button[type="submit"]');

    // Try to access QR codes page directly
    await page.goto('http://localhost:3000/admin/qr-codes');

    // Should be redirected
    await expect(page).toHaveURL('**/dashboard');
  });
});

// e2e/customer-ordering-flow.spec.js
test.describe('Customer Ordering Flow E2E', () => {
  test('customer can scan QR, login, and place order', async ({ page }) => {
    // Simulate QR code scan by navigating to auth URL
    await page.goto('http://localhost:3000/login?location=1&table=5');

    // Login/Register
    await page.fill('[name="email"]', 'customer@test.com');
    await page.fill('[name="password"]', 'password123');
    await page.click('button[type="submit"]');

    // Verify table and location are set
    await expect(page.locator('text=Table 5')).toBeVisible();
    await expect(page.locator('text=Downtown')).toBeVisible();

    // Browse menu
    await page.click('text=Menu');

    // Add item to cart
    await page.click('[data-testid="menu-item-1"]');
    await page.click('text=Add to Order');

    // View cart and checkout
    await page.click('[data-testid="cart-button"]');
    await page.click('text=Place Order');

    // Verify order confirmation
    await expect(page.locator('text=Order placed successfully')).toBeVisible();
  });
});
```

---

## Test Coverage Goals

### Back-End Coverage Targets:
- **Unit Tests**: 80%+ code coverage
- **Service Layer**: 90%+ coverage
- **Controllers**: 80%+ coverage
- **Critical Paths**: 100% coverage
  - Authentication
  - Payment processing
  - Order creation
  - QR code generation

### Front-End Coverage Targets:
- **Components**: 70%+ coverage
- **Critical Components**: 90%+ coverage
  - QRCodeManagement
  - MenuItemManagement
  - BillManagement
  - OrderPlacement
- **Hooks/Utilities**: 85%+ coverage

---

## CI/CD Integration

### GitHub Actions Workflow Example:

```yaml
# .github/workflows/test.yml
name: Automated Tests

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]

jobs:
  backend-tests:
    runs-on: windows-latest

    steps:
    - uses: actions/checkout@v3

    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'

    - name: Restore dependencies
      run: dotnet restore src/back-end/back-end.csproj

    - name: Build
      run: dotnet build src/back-end/back-end.csproj --no-restore

    - name: Run Unit Tests
      run: dotnet test src/back-end.Tests/back-end.Tests.csproj --no-build --verbosity normal

    - name: Run Integration Tests
      run: dotnet test src/back-end.IntegrationTests/back-end.IntegrationTests.csproj

    - name: Generate Coverage Report
      run: dotnet test --collect:"XPlat Code Coverage"

    - name: Upload Coverage
      uses: codecov/codecov-action@v3
      with:
        files: ./coverage/coverage.cobertura.xml

  frontend-tests:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3

    - name: Setup Node.js
      uses: actions/setup-node@v3
      with:
        node-version: '18'

    - name: Install dependencies
      run: npm ci
      working-directory: src/front-end

    - name: Run Unit Tests
      run: npm test -- --coverage
      working-directory: src/front-end

    - name: Run Linter
      run: npm run lint
      working-directory: src/front-end

    - name: Upload Coverage
      uses: codecov/codecov-action@v3
      with:
        files: ./src/front-end/coverage/lcov.info

  e2e-tests:
    runs-on: ubuntu-latest
    needs: [backend-tests, frontend-tests]

    steps:
    - uses: actions/checkout@v3

    - name: Start Backend
      run: |
        cd src/back-end
        dotnet run &
        sleep 10

    - name: Start Frontend
      run: |
        cd src/front-end
        npm ci
        npm run build
        npm start &
        sleep 10

    - name: Run E2E Tests
      run: npx playwright test
      working-directory: src/front-end

    - name: Upload Test Results
      if: always()
      uses: actions/upload-artifact@v3
      with:
        name: playwright-report
        path: src/front-end/playwright-report
```

---

## Implementation Priority

### Phase 1: Essential Tests (Week 1-2)
1. ✅ Back-end unit tests for critical services
   - QrGeneratorService
   - AuthController
   - OrderController
2. ✅ Front-end unit tests for key components
   - QRCodeManagement
   - MenuItemManagement
3. ✅ Basic integration tests for API endpoints

### Phase 2: Expanded Coverage (Week 3-4)
1. ✅ Additional service layer tests
2. ✅ Repository/Data access tests
3. ✅ Front-end hook and utility tests
4. ✅ API integration tests with test database

### Phase 3: E2E & Performance (Week 5-6)
1. ✅ Critical user flow E2E tests
2. ✅ Performance tests for high-load scenarios
3. ✅ Security testing (authentication, authorization)
4. ✅ Load testing for concurrent orders

### Phase 4: CI/CD & Monitoring (Week 7-8)
1. ✅ GitHub Actions integration
2. ✅ Automated test runs on PR
3. ✅ Code coverage reporting
4. ✅ Test result visualization

---

## Additional Testing Considerations

### Performance Testing:
- **Load Testing**: k6 or JMeter
- **Stress Testing**: Simulate 100+ concurrent users
- **Database Performance**: Query optimization tests

### Security Testing:
- **Authentication**: Token expiration, refresh flow
- **Authorization**: Role-based access control
- **SQL Injection**: Parameterized query verification
- **XSS Protection**: Input sanitization tests

### Accessibility Testing:
- **Front-end**: jest-axe for a11y violations
- **Screen Reader**: Manual testing with NVDA/JAWS

### Mobile Testing:
- **Responsive Design**: Cypress viewport tests
- **Touch Interactions**: Mobile-specific gestures

---

## Test Data Management

### Test Fixtures:
```csharp
// TestDataFactory.cs
public static class TestDataFactory
{
    public static Location CreateTestLocation(int id = 1)
    {
        return new Location
        {
            Location_Id = id,
            Name = $"Test Location {id}",
            City = "Test City",
            // ...
        };
    }

    public static MenuItem CreateTestMenuItem(int id = 1)
    {
        return new MenuItem
        {
            Item_Id = id,
            Name = $"Test Item {id}",
            Price = 10.99m,
            // ...
        };
    }
}
```

---

## Monitoring & Reporting

### Test Metrics to Track:
- Test execution time
- Code coverage percentage
- Test pass/fail rate
- Flaky test identification
- Performance benchmarks

### Tools:
- **SonarQube**: Code quality and coverage
- **Codecov**: Coverage visualization
- **Allure**: Test reporting
- **TestRail**: Test case management

---

## Summary

This comprehensive testing strategy ensures:
- ✅ High code quality through unit tests
- ✅ Reliable integrations through integration tests
- ✅ Smooth user experience through E2E tests
- ✅ Early bug detection through CI/CD
- ✅ Maintainable codebase with good coverage

**Start with Phase 1 (critical tests) and gradually expand to full coverage!**
