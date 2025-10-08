#!/bin/bash
# Render Start Script for .NET Core

set -e  # Exit on any error

echo "==> Starting .NET Core application..."

# Add .NET to PATH
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$PATH:$HOME/.dotnet"

# Verify .NET runtime
echo "==> Verifying .NET runtime..."
dotnet --version || echo "Warning: .NET runtime not found in PATH"

# Navigate to published application
echo "==> Navigating to published application..."
cd FinancialApp.Presentation/publish

# List files to verify
echo "==> Application files:"
ls -la

# Verify DLL exists
if [ ! -f "FinancialApp.Presentation.dll" ]; then
    echo "ERROR: FinancialApp.Presentation.dll not found!"
    exit 1
fi

# Start the application
echo "==> Starting application on port $PORT..."
exec dotnet FinancialApp.Presentation.dll --urls=http://0.0.0.0:$PORT