#!/bin/bash
# Render Build Script for .NET Core (No sudo required)

set -e  # Exit on any error

echo "==> Starting .NET Core build on Render..."

# Install .NET SDK using install script (no sudo needed)
echo "==> Installing .NET 8.0 SDK..."
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 8.0 --install-dir ~/.dotnet

# Add .NET to PATH
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$PATH:$HOME/.dotnet"

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