#!/bin/bash
# Render Build Script for .NET Core

set -e  # Exit on any error

echo "==> Starting .NET Core build on Render..."

# Install .NET SDK
echo "==> Installing .NET 8.0 SDK..."
wget -q https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y apt-transport-https dotnet-sdk-8.0

# Verify .NET installation
echo "==> Verifying .NET installation..."
dotnet --version

# Navigate to presentation project
echo "==> Navigating to FinancialApp.Presentation..."
cd FinancialApp.Presentation

# Restore packages
echo "==> Restoring NuGet packages..."
dotnet restore --verbosity normal

# Build the application
echo "==> Building application..."
dotnet build --configuration Release --no-restore --verbosity normal

# Publish the application
echo "==> Publishing application..."
dotnet publish --configuration Release --output ./publish --no-build --verbosity normal

# Verify publish directory
echo "==> Verifying publish directory..."
ls -la ./publish/

echo "==> Build completed successfully!"