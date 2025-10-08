# Dockerfile for .NET 8.0 deployment on Render

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["FinancialApp.sln", "./"]
COPY ["FinancialApp.Presentation/FinancialApp.Presentation.csproj", "FinancialApp.Presentation/"]
COPY ["FinancialApp.Application/FinancialApp.Application.csproj", "FinancialApp.Application/"]
COPY ["FinancialApp.Infrastructure/FinancialApp.Infrastructure.csproj", "FinancialApp.Infrastructure/"]
COPY ["FinancialApp.Domain/FinancialApp.Domain.csproj", "FinancialApp.Domain/"]

# Restore packages
RUN dotnet restore "FinancialApp.Presentation/FinancialApp.Presentation.csproj"

# Copy source code
COPY . .

# Build and publish
WORKDIR "/src/FinancialApp.Presentation"
RUN dotnet build "FinancialApp.Presentation.csproj" -c Release -o /app/build
RUN dotnet publish "FinancialApp.Presentation.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Create entrypoint script
RUN echo '#!/bin/bash\nexec dotnet FinancialApp.Presentation.dll --urls=http://0.0.0.0:$PORT' > entrypoint.sh
RUN chmod +x entrypoint.sh

ENTRYPOINT ["./entrypoint.sh"]