# Automated Testing Implementation Guide

This document provides instructions for running the automated tests that have been implemented based on the [TESTING_STRATEGY.md](TESTING_STRATEGY.md).

## Table of Contents
- [Overview](#overview)
- [Back-End Testing](#back-end-testing)
- [Front-End Testing](#front-end-testing)
- [End-to-End Testing](#end-to-end-testing)
- [CI/CD Pipeline](#cicd-pipeline)
- [Test Coverage](#test-coverage)

## Overview

The automated testing suite implements a comprehensive testing pyramid:
- **Unit Tests (70%)**: Back-end services, controllers, and front-end components
- **Integration Tests (20%)**: API endpoints with database interactions
- **E2E Tests (10%)**: Critical user flows and workflows

## Back-End Testing

### Prerequisites
- .NET 9.0 SDK
- Windows OS (for QR code generation with System.Drawing)

### Project Structure
```
src/
├── back-end/                    # Main application
├── back-end.Tests/              # Unit tests
│   ├── Services/
│   │   └── QrGeneratorServiceTests.cs
│   └── Controllers/
│       └── AdminQrCodeControllerTests.cs
└── back-end.IntegrationTests/   # Integration tests
    └── QrCodeIntegrationTests.cs
```

### Running Back-End Tests

#### Run All Unit Tests
```bash
cd src/back-end.Tests
dotnet test
```

#### Run All Integration Tests
```bash
cd src/back-end.IntegrationTests
dotnet test
```

#### Run All Back-End Tests
```bash
dotnet test src/back-end.Tests/back-end.Tests.csproj
dotnet test src/back-end.IntegrationTests/back-end.IntegrationTests.csproj
```

#### Generate Coverage Report
```bash
cd src/back-end.Tests
dotnet test --collect:"XPlat Code Coverage"
```

### Implemented Tests

#### QrGeneratorService Tests
- ✅ GetLocationWifiCredentials_ValidLocation_ReturnsCredentials
- ✅ GetLocationWifiCredentials_InvalidLocation_ReturnsNull
- ✅ GetLocationWifiCredentials_NoLocationSpecific_ReturnsFallback
- ✅ CreateWifiQr_ValidCredentials_ReturnsQrCodeBitmap
- ✅ CreateSessionQr_ValidUrl_ReturnsQrCodeBitmap
- ✅ GetSessionUrl_ValidParameters_ReturnsCorrectUrl
- ✅ RenderLabeledQr_ValidBitmap_AddsLabel

#### AdminQrCodeController Tests
- ✅ GetWifiQr_ValidRequest_ReturnsFileResult
- ✅ GetWifiQr_InvalidLocation_ReturnsNotFound
- ✅ GetWifiQr_InvalidTableNumber_ReturnsBadRequest
- ✅ GetSessionQr_ValidRequest_ReturnsFileResult
- ✅ GetSessionQr_InvalidLocation_ReturnsNotFound
- ✅ BulkGenerateQrCodes_ValidRequest_ReturnsSuccessResponse
- ✅ BulkGenerateQrCodes_ExcessiveTableCount_ReturnsBadRequest
- ✅ ClearQrCache_ValidRequest_ReturnsSuccess

#### Integration Tests
- ✅ GetWifiQr_NoAuthentication_ReturnsUnauthorized
- ✅ GetSessionQr_NoAuthentication_ReturnsUnauthorized
- ✅ BulkGenerateQrCodes_NoAuthentication_ReturnsUnauthorized

## Front-End Testing

### Prerequisites
- Node.js 18+
- npm or yarn

### Project Structure
```
src/front-end/
├── src/
│   └── components/
│       └── admin/
│           ├── QRCodeManagement.jsx
│           └── __tests__/
│               └── QRCodeManagement.test.jsx
├── e2e/
│   └── qr-code-generation.spec.js
├── jest.config.js
├── jest.setup.js
└── playwright.config.js
```

### Running Front-End Tests

#### Install Dependencies
```bash
cd src/front-end
npm install
```

#### Run Unit Tests
```bash
npm test
```

#### Run Tests in Watch Mode
```bash
npm run test:watch
```

#### Generate Coverage Report
```bash
npm run test:coverage
```

### Implemented Component Tests

#### QRCodeManagement Component Tests
- ✅ Renders QR Code Management title
- ✅ Fetches and displays locations on mount
- ✅ Shows warning when downloading without table number
- ✅ Downloads WiFi QR code when valid input provided
- ✅ Opens preview dialog when preview button clicked
- ✅ Bulk generation sends correct API request
- ✅ Displays error message on API failure
- ✅ Handles location selection change
- ✅ Shows loading state during API calls

## End-to-End Testing

### Prerequisites
- Playwright installed
- Both back-end and front-end servers running

### Running E2E Tests

#### Install Playwright Browsers
```bash
cd src/front-end
npx playwright install --with-deps
```

#### Run E2E Tests (Headless)
```bash
npm run test:e2e
```

#### Run E2E Tests (Headed - see the browser)
```bash
npm run test:e2e:headed
```

#### Run E2E Tests with UI Mode
```bash
npm run test:e2e:ui
```

### E2E Test Scenarios

#### QR Code Generation Flow
- ✅ Display login page
- ✅ Admin navigation to QR codes page
- ✅ QR code page has required form elements
- ✅ Handle invalid location gracefully
- ✅ Preview dialog opens and closes correctly

#### Customer Ordering Flow
- ✅ Customer can access start-session page
- ✅ Session parameters are preserved in URL

#### Mobile Testing
- ✅ QR code page is responsive on mobile

## CI/CD Pipeline

### GitHub Actions Workflow

The automated testing pipeline is configured in [.github/workflows/automated-tests.yml](.github/workflows/automated-tests.yml)

#### Pipeline Jobs

1. **backend-tests**
   - Restores dependencies
   - Builds the application
   - Runs unit tests
   - Runs integration tests
   - Generates coverage reports
   - Uploads results to Codecov

2. **frontend-tests**
   - Installs dependencies
   - Runs linter
   - Runs unit tests with coverage
   - Uploads results to Codecov

3. **e2e-tests** (runs on main/develop branches only)
   - Starts back-end server
   - Builds and starts front-end
   - Installs Playwright browsers
   - Runs E2E tests
   - Uploads test reports

4. **test-summary**
   - Downloads all test results
   - Generates summary report

### Triggering the Pipeline

The pipeline runs automatically on:
- Push to `main`, `develop`, or `David--QR-code` branches
- Pull requests to `main` or `develop` branches

### Viewing Test Results

1. Navigate to the **Actions** tab in GitHub
2. Select the latest workflow run
3. Review the job summaries
4. Download artifacts for detailed reports

## Test Coverage

### Current Coverage Goals

#### Back-End
- **Unit Tests**: 80%+ code coverage
- **Service Layer**: 90%+ coverage (critical for QR generation)
- **Controllers**: 80%+ coverage
- **Critical Paths**: 100% coverage
  - QR code generation
  - Authentication
  - Authorization

#### Front-End
- **Components**: 70%+ coverage
- **Critical Components**: 90%+ coverage
  - QRCodeManagement
  - MenuItemManagement
  - BillManagement
- **Hooks/Utilities**: 85%+ coverage

### Viewing Coverage Reports

#### Back-End Coverage
```bash
cd src/back-end.Tests
dotnet test --collect:"XPlat Code Coverage"
# Coverage report will be in TestResults/*/coverage.cobertura.xml
```

#### Front-End Coverage
```bash
cd src/front-end
npm run test:coverage
# Open coverage/lcov-report/index.html in a browser
```

## Running All Tests Locally

### Option 1: Run Tests Separately

```bash
# Back-end unit tests
dotnet test src/back-end.Tests/back-end.Tests.csproj

# Back-end integration tests
dotnet test src/back-end.IntegrationTests/back-end.IntegrationTests.csproj

# Front-end tests
cd src/front-end
npm test

# E2E tests (requires servers running)
npm run test:e2e
```

### Option 2: Run All Tests Script

Create a script file `run-all-tests.ps1` (PowerShell):

```powershell
Write-Host "Running Back-End Unit Tests..." -ForegroundColor Green
dotnet test src/back-end.Tests/back-end.Tests.csproj

Write-Host "Running Back-End Integration Tests..." -ForegroundColor Green
dotnet test src/back-end.IntegrationTests/back-end.IntegrationTests.csproj

Write-Host "Running Front-End Tests..." -ForegroundColor Green
cd src/front-end
npm test -- --watchAll=false

Write-Host "All tests completed!" -ForegroundColor Green
```

## Troubleshooting

### Common Issues

#### Issue: Tests fail due to missing dependencies
**Solution**:
```bash
# Back-end
dotnet restore src/back-end/back-end.csproj
dotnet restore src/back-end.Tests/back-end.Tests.csproj

# Front-end
cd src/front-end
npm install
```

#### Issue: Integration tests fail with database errors
**Solution**: Integration tests use in-memory database. Ensure EF Core InMemory package is installed.

#### Issue: E2E tests timeout
**Solution**: Increase timeout in `playwright.config.js` or ensure both servers are running.

#### Issue: QR code tests fail on non-Windows OS
**Solution**: QrGeneratorService uses System.Drawing which is Windows-only. Run tests on Windows or update code to use SkiaSharp (cross-platform).

## Next Steps

### Additional Tests to Implement

Based on the TESTING_STRATEGY.md, consider adding:

1. **AuthController Tests**
   - Login with valid credentials
   - Login with invalid credentials
   - Token refresh functionality

2. **OrderController Tests**
   - Create order flow
   - Update order status
   - Order validation

3. **BillController Tests**
   - Bill creation
   - Payment processing
   - Split bill functionality

4. **Performance Tests**
   - Load testing with k6 or JMeter
   - Stress testing for concurrent users

5. **Security Tests**
   - SQL injection prevention
   - XSS protection
   - Authentication/Authorization flows

## Contributing

When adding new tests:

1. Follow the existing test structure
2. Use descriptive test names (Given_When_Then pattern)
3. Keep tests isolated and independent
4. Mock external dependencies
5. Aim for high coverage of critical paths
6. Update this README with new test documentation

## Resources

- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [React Testing Library](https://testing-library.com/react)
- [Jest Documentation](https://jestjs.io/)
- [Playwright Documentation](https://playwright.dev/)
- [Testing Strategy](TESTING_STRATEGY.md)
