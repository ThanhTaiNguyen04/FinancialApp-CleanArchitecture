#!/bin/bash
# Render Deploy Script for .NET Core with SQL Server

echo "Starting .NET Core deployment on Render..."

# Install .NET SDK if not available
if ! command -v dotnet &> /dev/null; then
    echo "Installing .NET SDK..."
    wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    dpkg -i packages-microsoft-prod.deb
    apt-get update
    apt-get install -y apt-transport-https dotnet-sdk-8.0
fi

# Navigate to project directory
cd FinancialApp.Presentation

# Restore packages
echo "Restoring .NET packages..."
dotnet restore

# Build the application
echo "Building application..."
dotnet build --configuration Release --no-restore

# Publish the application
echo "Publishing application..."
dotnet publish --configuration Release --output ./publish --no-build

echo "Deployment completed successfully!"