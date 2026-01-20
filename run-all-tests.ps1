# PowerShell script to run all automated tests
# Usage: .\run-all-tests.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Running All Automated Tests" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Track overall success
$overallSuccess = $true

# Function to print section headers
function Print-Section {
    param([string]$title)
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Yellow
    Write-Host "  $title" -ForegroundColor Yellow
    Write-Host "========================================" -ForegroundColor Yellow
    Write-Host ""
}

# Function to check last exit code
function Check-ExitCode {
    param([string]$testName)
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ $testName FAILED" -ForegroundColor Red
        $script:overallSuccess = $false
        return $false
    } else {
        Write-Host "✅ $testName PASSED" -ForegroundColor Green
        return $true
    }
}

# Back-End Unit Tests
Print-Section "Back-End Unit Tests"
dotnet test src/back-end.Tests/back-end.Tests.csproj --verbosity normal
Check-ExitCode "Back-End Unit Tests"

# Back-End Integration Tests
Print-Section "Back-End Integration Tests"
dotnet test src/back-end.IntegrationTests/back-end.IntegrationTests.csproj --verbosity normal
Check-ExitCode "Back-End Integration Tests"

# Front-End Unit Tests
Print-Section "Front-End Unit Tests"
Push-Location src/front-end
npm test -- --watchAll=false --passWithNoTests
$frontendTestResult = $LASTEXITCODE
Pop-Location
if ($frontendTestResult -ne 0) {
    Write-Host "❌ Front-End Unit Tests FAILED" -ForegroundColor Red
    $overallSuccess = $false
} else {
    Write-Host "✅ Front-End Unit Tests PASSED" -ForegroundColor Green
}

# Summary
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Test Execution Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

if ($overallSuccess) {
    Write-Host "🎉 All tests PASSED successfully!" -ForegroundColor Green
    exit 0
} else {
    Write-Host "⚠️  Some tests FAILED. Please review the output above." -ForegroundColor Red
    exit 1
}
