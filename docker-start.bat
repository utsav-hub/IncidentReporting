@echo off
REM Docker Start Script for Incident Reporting System (Windows)

echo 🚀 Starting Incident Reporting System with Docker...

REM Check if Docker is running
docker info >nul 2>&1
if errorlevel 1 (
    echo ❌ Docker is not running. Please start Docker Desktop and try again.
    exit /b 1
)

REM Build and start services
echo 📦 Building and starting services...
docker-compose up -d --build

REM Wait for services to be ready
echo ⏳ Waiting for services to start...
timeout /t 10 /nobreak >nul

REM Check service status
echo 📊 Service Status:
docker-compose ps

echo.
echo ✅ Services started!
echo.
echo 🌐 Access the application:
echo    Frontend: http://localhost:3000
echo    Backend API: http://localhost:8080/api
echo    Swagger UI: http://localhost:8080/swagger
echo.
echo    Note: Access Swagger at /swagger (not /swagger/index.html)
echo.
echo 📝 View logs: docker-compose logs -f
echo 🛑 Stop services: docker-compose down

pause

