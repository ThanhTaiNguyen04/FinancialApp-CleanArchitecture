#!/bin/bash
# Render Start Script for .NET Core

echo "Starting .NET Core application..."

cd FinancialApp.Presentation/publish

# Start the application
exec dotnet FinancialApp.Presentation.dll --urls=http://0.0.0.0:$PORT