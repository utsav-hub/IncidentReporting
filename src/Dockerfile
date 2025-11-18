# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["IncidentReporting.Api/IncidentReporting.Api.csproj", "IncidentReporting.Api/"]
COPY ["IncidentReporting.Application/IncidentReporting.Application.csproj", "IncidentReporting.Application/"]
COPY ["IncidentReporting.Infrastructure/IncidentReporting.Infrastructure.csproj", "IncidentReporting.Infrastructure/"]
COPY ["IncidentReporting.Domain/IncidentReporting.Domain.csproj", "IncidentReporting.Domain/"]

# Restore dependencies
RUN dotnet restore "IncidentReporting.Api/IncidentReporting.Api.csproj"

# Copy all source files
COPY . .

# Build the application
WORKDIR "/src/IncidentReporting.Api"
RUN dotnet build "IncidentReporting.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "IncidentReporting.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Install curl for healthchecks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create directory for database
RUN mkdir -p /app/Data

# Copy published application
COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "IncidentReporting.Api.dll"]

