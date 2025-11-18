# Docker Setup for Incident Reporting System

This guide explains how to run the Incident Reporting System using Docker and Docker Compose.

## Prerequisites

- Docker Desktop (or Docker Engine + Docker Compose)
- At least 4GB of available RAM
- Ports 3000 and 8080 available (or modify in docker-compose.yml)

## Quick Start

### Production Mode

To run the application in production mode:

```bash
docker-compose up -d
```

This will:
- Build and start the backend API on port 8080
- Build and start the frontend on port 3000
- Create a Docker volume for persistent database storage

### Development Mode

To run with development settings:

```bash
docker-compose -f docker-compose.dev.yml up -d
```

This uses:
- Backend on port 5268
- Frontend on port 5173
- Development environment settings

## Accessing the Application

- **Frontend**: http://localhost:3000 (production) or http://localhost:5173 (development)
- **Backend API**: http://localhost:8080/api (production) or http://localhost:5268/api (development)
- **Swagger UI**: http://localhost:8080/swagger (production) or http://localhost:5268/swagger (development)

**Note**: Swagger is enabled in Docker by default. Access it at `/swagger` (not `/swagger/index.html`).

## Useful Commands

### View Logs

```bash
# All services
docker-compose logs -f

# Backend only
docker-compose logs -f backend

# Frontend only
docker-compose logs -f frontend
```

### Stop Services

```bash
docker-compose down
```

### Stop and Remove Volumes (⚠️ This will delete the database)

```bash
docker-compose down -v
```

### Rebuild After Code Changes

```bash
# Rebuild and restart
docker-compose up -d --build

# Rebuild specific service
docker-compose up -d --build backend
docker-compose up -d --build frontend
```

### Check Service Status

```bash
docker-compose ps
```

## Environment Variables

### Backend

The backend uses the following environment variables (set in docker-compose.yml):

- `ASPNETCORE_ENVIRONMENT`: Production or Development
- `ASPNETCORE_URLS`: Server binding URL
- `ConnectionStrings__DefaultConnection`: SQLite database path

### Frontend

The frontend uses:

- `VITE_API_URL`: Backend API URL (set during build)

## Database Persistence

The SQLite database is stored in a Docker volume named `backend-data`. This ensures data persists even when containers are stopped.

To backup the database:

```bash
docker run --rm -v incident-reporting-system_backend-data:/data -v $(pwd):/backup alpine tar czf /backup/db-backup.tar.gz -C /data .
```

To restore:

```bash
docker run --rm -v incident-reporting-system_backend-data:/data -v $(pwd):/backup alpine tar xzf /backup/db-backup.tar.gz -C /data
```

## Troubleshooting

### Port Already in Use

If ports 3000 or 8080 are already in use, modify the port mappings in `docker-compose.yml`:

```yaml
ports:
  - "3001:80"  # Change 3000 to 3001
```

### Database Issues

If you encounter database errors:

1. Stop containers: `docker-compose down`
2. Remove volume: `docker-compose down -v`
3. Restart: `docker-compose up -d`

### Frontend Can't Connect to Backend

Ensure the `VITE_API_URL` in the frontend build matches the backend service name in docker-compose. In Docker, services communicate using service names (e.g., `backend:8080`), not `localhost`.

### Rebuild After Dependency Changes

If you add new NuGet packages or npm packages:

```bash
docker-compose build --no-cache
docker-compose up -d
```

## Development Workflow

For active development, you may prefer to run services locally:

- **Backend**: Use Visual Studio or `dotnet run` in `src/IncidentReporting.Api`
- **Frontend**: Use `npm run dev` in `webui/incident-reporting-ui`

Then use Docker only for testing the containerized setup.

## Production Deployment

For production deployment:

1. Update `docker-compose.yml` with production environment variables
2. Use a reverse proxy (nginx/traefik) in front of the services
3. Set up SSL/TLS certificates
4. Configure proper logging and monitoring
5. Use environment-specific secrets management

## Architecture

```
┌─────────────────┐
│   Frontend      │
│   (Nginx)       │
│   Port: 3000    │
└────────┬────────┘
         │
         │ HTTP
         │
┌────────▼────────┐
│   Backend       │
│   (.NET API)    │
│   Port: 8080    │
└────────┬────────┘
         │
         │ SQLite
         │
┌────────▼────────┐
│   Database      │
│   (Volume)      │
└─────────────────┘
```

Both services run on the same Docker network (`incident-reporting-network`) allowing them to communicate using service names.

