@echo off
echo ========================================
echo    FINANCIAL APP BACKEND - LOCAL TEST
echo ========================================
echo.
echo Starting .NET backend on localhost:50255...
echo Database: Local SQL Server
echo.

cd /d "D:\EXE201\FinancialApp-CleanArchitecture\FinancialApp.Presentation"

echo Restoring packages...
dotnet restore

echo.
echo Building project...
dotnet build

echo.
echo Starting server...
echo Backend will be available at: http://localhost:50255
echo Swagger UI: http://localhost:50255/swagger
echo.
echo Press Ctrl+C to stop the server
echo.

dotnet run --urls "http://localhost:50255"