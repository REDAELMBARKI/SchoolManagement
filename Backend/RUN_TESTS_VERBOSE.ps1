# Run tests with verbose output to see detailed errors

Write-Host "Running tests with detailed output..." -ForegroundColor Cyan

dotnet test `
    --logger "console;verbosity=detailed" `
    --no-build `
    --no-restore `
    c:\SchoolManagement\Backend\SchoolManagement.Tests\SchoolManagement.Tests.csproj

Write-Host "`nTest run complete!" -ForegroundColor Green
