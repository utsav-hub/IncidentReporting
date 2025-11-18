# Incident Reporting System

A full-stack web application for managing and tracking incidents, built with **Clean Architecture**, **Domain-Driven Design (DDD)**, and modern web technologies.

## 📋 Table of Contents

1. [Project Overview](#project-overview)
2. [Architecture & Design Patterns](#architecture--design-patterns)
3. [Technology Stack](#technology-stack)
4. [High-Level System Design](#high-level-system-design)
5. [Backend Architecture](#backend-architecture)
6. [Frontend Architecture](#frontend-architecture)
7. [Key Features](#key-features)
8. [API Documentation](#api-documentation)
9. [Database Schema](#database-schema)
10. [Authentication Flow](#authentication-flow)
11. [Incident Management Flow](#incident-management-flow)
12. [Notification System](#notification-system)
13. [Domain Events & State Machine](#domain-events--state-machine)
14. [Docker Setup](#docker-setup)
15. [Development Guide](#development-guide)

---

## 🎯 Project Overview

The Incident Reporting System is a comprehensive solution for organizations to track, manage, and resolve incidents. It provides:

- **User Authentication & Authorization** - Secure JWT-based authentication
- **Incident Management** - Create, update, delete, and track incidents through their lifecycle
- **Category Management** - Organize incidents by categories (Hardware, Software, Network, Security, etc.)
- **Status Workflow** - State machine-based workflow (Open → InProgress → Closed)
- **Real-time Notifications** - Mock notification system for user alerts
- **Audit Trail** - Automatic history tracking for incident state changes
- **RESTful API** - Well-structured API with Swagger documentation

---

## 🏗️ Architecture & Design Patterns

### Clean Architecture (Layered Architecture)

The application follows **Clean Architecture** principles with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer                    │
│              (API Controllers, Middleware)               │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│                 Application Layer                        │
│    (Use Cases, Handlers, DTOs, Validators, MediatR)    │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│                   Domain Layer                          │
│    (Entities, Domain Events, Business Logic, State)    │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│              Infrastructure Layer                        │
│    (Data Access, Repositories, External Services)       │
└─────────────────────────────────────────────────────────┘
```

### Design Patterns Used

1. **CQRS (Command Query Responsibility Segregation)**
   - Commands: `CreateIncidentCommand`, `UpdateIncidentCommand`, `DeleteIncidentCommand`
   - Queries: `GetAllIncidentsQuery`, `GetIncidentByIdQuery`

2. **Mediator Pattern (MediatR)**
   - Decouples controllers from business logic
   - Handlers process commands/queries independently

3. **Repository Pattern**
   - Abstracts data access logic
   - `IIncidentRepository`, `IIncidentHistoryRepository`

4. **Domain Events**
   - `IncidentClosedEvent` - Raised when incident is closed
   - Handled by `CreateIncidentHistoryHandler`, `SendIncidentClosedEmailHandler`

5. **State Machine Pattern**
   - Incident status transitions managed by Stateless library
   - Enforces valid state transitions

6. **Dependency Injection**
   - All dependencies injected via constructor
   - Configured in `DependencyInjection.cs`

---

## 💻 Technology Stack

### Backend
- **.NET 9.0** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core** - ORM for database operations
- **SQLite** - Lightweight database (can be swapped for SQL Server/PostgreSQL)
- **MediatR** - CQRS and mediator pattern implementation
- **FluentValidation** - Input validation
- **JWT Bearer Authentication** - Secure token-based auth
- **Swashbuckle (Swagger)** - API documentation
- **Stateless** - State machine library

### Frontend
- **React 19** - UI library
- **TypeScript** - Type-safe JavaScript
- **Vite** - Fast build tool and dev server
- **React Router** - Client-side routing
- **Axios** - HTTP client
- **Tailwind CSS** - Utility-first CSS framework
- **Context API** - State management

### DevOps
- **Docker** - Containerization
- **Docker Compose** - Multi-container orchestration
- **Nginx** - Web server for frontend (production)

---

## 🎨 High-Level System Design

```
┌─────────────────────────────────────────────────────────────────┐
│                         Client Browser                          │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │         React SPA (Port 3000)                            │  │
│  │  - Authentication UI                                      │  │
│  │  - Incident Management UI                                 │  │
│  │  - Notification Bell                                     │  │
│  └──────────────────┬───────────────────────────────────────┘  │
└──────────────────────┼─────────────────────────────────────────┘
                       │ HTTP/REST
                       │ JWT Token
                       ▼
┌─────────────────────────────────────────────────────────────────┐
│                    ASP.NET Core API (Port 8080)                  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  Controllers Layer                                        │  │
│  │  - AuthController                                         │  │
│  │  - IncidentsController                                   │  │
│  │  - CategoriesController                                  │  │
│  │  - NotificationsController                                │  │
│  └──────────────────┬───────────────────────────────────────┘  │
│                     │                                           │
│  ┌──────────────────▼───────────────────────────────────────┐  │
│  │  Application Layer (MediatR)                              │  │
│  │  - Command Handlers                                       │  │
│  │  - Query Handlers                                         │  │
│  │  - Notification Handlers                                  │  │
│  │  - Validators                                            │  │
│  └──────────────────┬───────────────────────────────────────┘  │
│                     │                                           │
│  ┌──────────────────▼───────────────────────────────────────┐  │
│  │  Domain Layer                                             │  │
│  │  - Entities (Incident, User, Category)                   │  │
│  │  - Domain Events                                         │  │
│  │  - State Machine                                         │  │
│  └──────────────────┬───────────────────────────────────────┘  │
│                     │                                           │
│  ┌──────────────────▼───────────────────────────────────────┐  │
│  │  Infrastructure Layer                                     │  │
│  │  - Repositories                                           │  │
│  │  - DbContext (EF Core)                                    │  │
│  │  - Services (Auth, Notifications)                         │  │
│  └──────────────────┬───────────────────────────────────────┘  │
└──────────────────────┼─────────────────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────────────────┐
│                    SQLite Database                              │
│  - Users                                                        │
│  - Incidents                                                    │
│  - Categories                                                   │
│  - IncidentHistory                                              │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🔧 Backend Architecture

### Layer Breakdown

#### 1. **API Layer** (`IncidentReporting.Api`)
- **Purpose**: Entry point for HTTP requests
- **Components**:
  - Controllers (thin, delegate to MediatR)
  - Middleware (exception handling, CORS)
  - Program.cs (startup configuration)

**Key Files:**
- `Controllers/IncidentsController.cs` - Incident CRUD operations
- `Controllers/AuthController.cs` - Authentication endpoints
- `Middleware/ExceptionMiddleware.cs` - Global error handling

#### 2. **Application Layer** (`IncidentReporting.Application`)
- **Purpose**: Business logic and use cases
- **Components**:
  - Commands/Queries (CQRS)
  - Handlers (business logic)
  - DTOs (data transfer objects)
  - Validators (FluentValidation)
  - Interfaces (abstractions)

**Key Files:**
- `Handlers/CreateIncidentHandler.cs` - Creates incident + notification
- `Handlers/UpdateIncidentHandler.cs` - Updates incident with state machine
- `Requests/CreateIncidentCommand.cs` - Command definition
- `Validators/CreateIncidentCommandValidator.cs` - Input validation

#### 3. **Domain Layer** (`IncidentReporting.Domain`)
- **Purpose**: Core business entities and rules
- **Components**:
  - Entities (Incident, User, Category)
  - Domain Events
  - Value Objects
  - Business Logic

**Key Files:**
- `Entities/Incident.cs` - Core entity with state machine
- `DomainEvents/IncidentClosedEvent.cs` - Domain event
- `Common/EntityBase.cs` - Base class with domain events

#### 4. **Infrastructure Layer** (`IncidentReporting.Infrastructure`)
- **Purpose**: External concerns (database, services)
- **Components**:
  - Repositories (data access)
  - DbContext (EF Core)
  - Services (Auth, Notifications)
  - Dependency Injection setup

**Key Files:**
- `Repositories/IncidentRepository.cs` - Data access
- `Data/AppDbContext.cs` - EF Core context + domain event dispatch
- `Services/AuthService.cs` - JWT token generation
- `Services/MockNotificationService.cs` - In-memory notifications

---

## 🎨 Frontend Architecture

### Component Structure

```
src/
├── api/                    # API service layer
│   ├── axiosClient.ts      # Axios configuration
│   ├── authService.ts      # Authentication API calls
│   ├── incidentService.ts  # Incident API calls
│   └── notificationService.ts # Notification API calls
│
├── components/             # Reusable components
│   ├── Layout/             # Layout components
│   │   ├── Layout.tsx     # Main layout wrapper
│   │   ├── Navbar.tsx     # Top navigation bar
│   │   └── Sidebar.tsx    # Side navigation
│   ├── NotificationBell.tsx # Notification dropdown
│   └── ProtectedRoute.tsx # Route protection
│
├── contexts/               # React Context providers
│   ├── AuthContext.tsx     # Authentication state
│   └── NotificationContext.tsx # Notification state
│
├── pages/                  # Page components
│   ├── Login.tsx          # Login page
│   ├── Register.tsx       # Registration page
│   ├── Dashboard.tsx     # Dashboard
│   ├── IncidentList.tsx  # List all incidents
│   ├── IncidentCreate.tsx # Create incident form
│   └── IncidentEdit.tsx  # Edit incident form
│
└── App.tsx                 # Root component with routing
```

### State Management Flow

1. **Authentication State** (`AuthContext`)
   - Stores JWT token and user info
   - Persists to localStorage
   - Provides login/logout functions

2. **Notification State** (`NotificationContext`)
   - Fetches notifications every 30 seconds
   - Manages unread count
   - Provides mark-as-read functions

3. **Component State**
   - Local state for forms and UI
   - Fetches data via service layer
   - Updates UI reactively

---

## ✨ Key Features

### 1. **Authentication & Authorization**
- JWT-based authentication
- Secure password hashing (BCrypt)
- Token stored in localStorage
- Protected routes
- Auto-logout on token expiry

### 2. **Incident Management**
- Create incidents with title, description, category
- Update incident status (Open → InProgress → Closed)
- State machine enforces valid transitions
- User can only access their own incidents
- Automatic timestamp tracking

### 3. **Category Management**
- Predefined categories (Hardware, Software, Network, Security, Other)
- Auto-seeded on first run
- Category filtering in UI

### 4. **Status Workflow**
- **Open**: Initial state, can move to InProgress or Closed
- **InProgress**: Work in progress, can move to Closed or reopen
- **Closed**: Final state with resolution, can be reopened

### 5. **Notifications**
- Real-time notification polling (30s interval)
- Notification on incident creation
- Unread count badge
- Mark as read functionality
- Mock implementation (in-memory)

### 6. **Audit Trail**
- Automatic history creation on incident closure
- Tracks action, description, timestamp
- Stored in `IncidentHistory` table

### 7. **Domain Events**
- `IncidentClosedEvent` raised when incident closes
- Handled by multiple handlers:
  - Creates history record
  - Sends email notification (mock)

---

## 📡 API Documentation

### Base URL
```
http://localhost:8080/api
```

### Authentication Endpoints

#### Register
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "SecurePass123!"
}

Response:
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "id": 1,
    "username": "john_doe",
    "email": "john@example.com"
  }
}
```

### Incident Endpoints

All incident endpoints require `Authorization: Bearer <token>` header.

#### Get All Incidents
```http
GET /api/incidents
Authorization: Bearer <token>
```

#### Get Incident by ID
```http
GET /api/incidents/{id}
Authorization: Bearer <token>
```

#### Create Incident
```http
POST /api/incidents
Authorization: Bearer <token>
Content-Type: application/json

{
  "title": "Server Down",
  "description": "Production server is not responding",
  "categoryId": 2
}
```

#### Update Incident
```http
PUT /api/incidents/{id}
Authorization: Bearer <token>
Content-Type: application/json

{
  "description": "Updated description",
  "status": 1,  // 0=Open, 1=InProgress, 2=Closed
  "resolution": "Server restarted successfully"
}
```

#### Delete Incident
```http
DELETE /api/incidents/{id}
Authorization: Bearer <token>
```

### Category Endpoints

#### Get All Categories
```http
GET /api/categories
Authorization: Bearer <token>
```

### Notification Endpoints

#### Get All Notifications
```http
GET /api/notifications
Authorization: Bearer <token>
```

#### Get Unread Count
```http
GET /api/notifications/unread-count
Authorization: Bearer <token>
```

#### Mark as Read
```http
POST /api/notifications/{id}/mark-read
Authorization: Bearer <token>
```

#### Mark All as Read
```http
POST /api/notifications/mark-all-read
Authorization: Bearer <token>
```

#### Seed Mock Notifications
```http
POST /api/notifications/seed-mock
Authorization: Bearer <token>
```

---

## 🗄️ Database Schema

### Users Table
```sql
Users
├── Id (PK, int)
├── Username (string, required, max 100)
├── Email (string, required, max 200, unique)
├── PasswordHash (string, required)
├── FirstName (string, nullable, max 100)
├── LastName (string, nullable, max 100)
├── IsActive (bool, default true)
├── CreatedAt (datetime)
└── UpdatedAt (datetime, nullable)
```

### Categories Table
```sql
Categories
├── Id (PK, int)
├── Name (string, required, max 100)
├── Description (string, nullable)
└── IsActive (bool, default true)
```

### Incidents Table
```sql
Incidents
├── Id (PK, int)
├── Title (string, required)
├── Description (string, nullable)
├── CategoryId (FK, int, nullable) → Categories.Id
├── UserId (FK, int, required) → Users.Id
├── Status (enum: 0=Open, 1=InProgress, 2=Closed)
├── Resolution (string, nullable)
├── CreatedAt (datetime)
├── UpdatedAt (datetime, nullable)
└── RowVersion (byte[], concurrency token)
```

### IncidentHistory Table
```sql
IncidentHistory
├── Id (PK, int)
├── IncidentId (FK, int, required) → Incidents.Id
├── Action (string, required)
├── Description (string, nullable)
└── CreatedAt (datetime)
```

---

## 🔐 Authentication Flow

### Registration Flow

```
1. User submits registration form
   ↓
2. Frontend: POST /api/auth/register
   ↓
3. Backend: AuthController.Register()
   ↓
4. AuthService.Register()
   - Validates email uniqueness
   - Hashes password (BCrypt)
   - Creates User entity
   - Saves to database
   ↓
5. Returns success response
   ↓
6. Frontend: Redirects to login
```

### Login Flow

```
1. User submits login form
   ↓
2. Frontend: POST /api/auth/login
   ↓
3. Backend: AuthController.Login()
   ↓
4. AuthService.Login()
   - Validates credentials
   - Verifies password hash
   - Generates JWT token
     - Claims: UserId, Username, Email
     - Expiration: 24 hours
   ↓
5. Returns token + user info
   ↓
6. Frontend: AuthContext.login()
   - Stores token in localStorage
   - Stores user in localStorage
   - Updates context state
   ↓
7. Redirects to Dashboard
```

### Protected Route Flow

```
1. User navigates to protected route
   ↓
2. ProtectedRoute component checks:
   - Is token in localStorage?
   - Is token valid?
   ↓
3. If valid:
   - Renders protected component
   - Adds token to axios interceptor
   ↓
4. If invalid:
   - Redirects to /login
   - Clears localStorage
```

### API Request Flow (with JWT)

```
1. Frontend makes API call
   ↓
2. Axios interceptor adds:
   Authorization: Bearer <token>
   ↓
3. Backend: JWT Middleware validates token
   - Verifies signature
   - Checks expiration
   - Extracts claims
   ↓
4. Controller extracts UserId from claims
   ↓
5. Handler processes request with UserId
```

---

## 📝 Incident Management Flow

### Create Incident Flow

```
1. User fills create incident form
   ↓
2. Frontend: POST /api/incidents
   Body: { title, description, categoryId }
   ↓
3. Backend: IncidentsController.Create()
   - Extracts UserId from JWT
   - Creates CreateIncidentCommand
   ↓
4. MediatR sends command to CreateIncidentHandler
   ↓
5. Handler:
   - Creates Incident entity (Status = Open)
   - Saves to database
   - Creates notification via INotificationService
   - Returns IncidentResponseDto
   ↓
6. Frontend: Receives response
   - Shows success message
   - Redirects to incident list
   - Notification appears in bell
```

### Update Incident Flow

```
1. User edits incident form
   Changes: status, description, resolution
   ↓
2. Frontend: PUT /api/incidents/{id}
   Body: { status, description, resolution }
   ↓
3. Backend: IncidentsController.Update()
   - Extracts UserId from JWT
   - Creates UpdateIncidentCommand
   ↓
4. MediatR sends command to UpdateIncidentHandler
   ↓
5. Handler:
   - Fetches incident (with user ownership check)
   - Updates status via state machine:
     * Status.Open → Reopen()
     * Status.InProgress → StartProgress()
     * Status.Closed → Close(resolution)
   - Updates description
   - Saves to database
   ↓
6. If status = Closed:
   - Incident.Close() raises IncidentClosedEvent
   ↓
7. Domain Event Handlers:
   - CreateIncidentHistoryHandler:
     * Creates history record
   - SendIncidentClosedEmailHandler:
     * Sends email (mock)
   ↓
8. Frontend: Receives updated incident
   - Updates UI
```

### State Machine Flow

```
Initial State: Open
├── StartProgress() → InProgress
└── Close(resolution) → Closed

State: InProgress
├── Close(resolution) → Closed
└── Reopen() → Open

State: Closed
└── Reopen() → Open
```

**Invalid Transitions:**
- Open → Open (no-op)
- Closed → InProgress (not allowed)
- Direct status assignment (bypassed state machine)

---

## 🔔 Notification System

### Architecture

```
MockNotificationService (In-Memory)
├── ConcurrentDictionary<int, List<NotificationDto>>
└── Thread-safe operations with locks
```

### Notification Creation Flow

```
1. Incident created
   ↓
2. CreateIncidentHandler calls:
   _notificationService.CreateNotificationAsync()
   ↓
3. MockNotificationService:
   - Generates unique ID
   - Creates NotificationDto
   - Stores in user's notification list
   ↓
4. Frontend polls every 30 seconds
   ↓
5. NotificationContext.refreshNotifications()
   - GET /api/notifications
   - GET /api/notifications/unread-count
   ↓
6. Updates UI:
   - Badge count
   - Notification dropdown
```

### Notification Types

- **Info**: General information
- **Success**: Successful operations (e.g., incident created)
- **Warning**: Warnings
- **Error**: Error notifications

### Future Enhancements

- Real-time WebSocket notifications
- Email notifications
- Push notifications
- Database persistence
- Notification preferences

---

## 🎯 Domain Events & State Machine

### Domain Events Pattern

**Purpose**: Decouple side effects from core business logic

**Flow:**
```
1. Domain entity raises event
   incident.AddDomainEvent(new IncidentClosedEvent(...))
   ↓
2. Entity Framework saves changes
   ↓
3. AppDbContext.DispatchDomainEventsAsync()
   - Collects all domain events
   - Publishes via MediatR
   ↓
4. Handlers process events:
   - CreateIncidentHistoryHandler
   - SendIncidentClosedEmailHandler
```

**Benefits:**
- Single Responsibility Principle
- Open/Closed Principle
- Easy to add new handlers
- Testable in isolation

### State Machine Implementation

**Library**: Stateless

**Configuration:**
```csharp
_stateMachine.Configure(IncidentStatus.Open)
    .Permit(IncidentTrigger.StartProgress, IncidentStatus.InProgress)
    .Permit(IncidentTrigger.Close, IncidentStatus.Closed);
```

**Benefits:**
- Enforces valid transitions
- Prevents invalid state changes
- Clear workflow definition
- Self-documenting code

---

## 🐳 Docker Setup

### Quick Start

```bash
# Build and start all services
docker-compose up -d --build

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

### Services

1. **Backend** (Port 8080)
   - .NET 9.0 API
   - SQLite database (persistent volume)
   - Swagger enabled

2. **Frontend** (Port 3000)
   - React app (Nginx)
   - Built with Vite
   - Serves static files

### Access Points

- Frontend: http://localhost:3000
- Backend API: http://localhost:8080/api
- Swagger: http://localhost:8080/swagger

See [DOCKER_README.md](./DOCKER_README.md) for detailed instructions.

---

## 🛠️ Development Guide

### Prerequisites

- .NET 9.0 SDK
- Node.js 20+
- SQLite (or SQL Server/PostgreSQL)
- Docker (optional)

### Backend Setup

```bash
cd src/IncidentReporting.Api

# Restore packages
dotnet restore

# Run migrations
dotnet ef database update

# Run application
dotnet run
```

### Frontend Setup

```bash
cd webui/incident-reporting-ui

# Install dependencies
npm install

# Start dev server
npm run dev
```

### Project Structure

```
src/
├── IncidentReporting.Api/          # API layer
│   ├── Controllers/               # API endpoints
│   ├── Middleware/                # Exception handling
│   └── Program.cs                 # Startup
│
├── IncidentReporting.Application/ # Application layer
│   ├── Handlers/                  # CQRS handlers
│   ├── Requests/                  # Commands/Queries
│   ├── DTOs/                      # Data transfer objects
│   ├── Validators/                # FluentValidation
│   └── Interfaces/                # Abstractions
│
├── IncidentReporting.Domain/      # Domain layer
│   ├── Entities/                  # Domain entities
│   ├── DomainEvents/              # Domain events
│   └── Common/                    # Base classes
│
└── IncidentReporting.Infrastructure/ # Infrastructure
    ├── Data/                      # DbContext
    ├── Repositories/              # Data access
    └── Services/                  # External services
```

### Testing

```bash
cd tests/IncidentReporting.UnitTests
dotnet test
```

---



### Scalability Considerations

- **Database**: Can swap SQLite for SQL Server/PostgreSQL
- **Caching**: Can add Redis for notifications
- **Message Queue**: Can add RabbitMQ/Azure Service Bus for events
- **Microservices**: Each layer can be split into separate services
- **Load Balancing**: Stateless API supports horizontal scaling

### Security Features

- Password hashing (BCrypt)
- JWT token expiration
- CORS configuration
- Input validation (FluentValidation)
- SQL injection protection (EF Core parameterized queries)
- Concurrency control (RowVersion)

---

## 🚀 Future Enhancements

- [ ] Real-time notifications (SignalR/WebSocket)
- [ ] Email notifications (SMTP integration)
- [ ] File attachments for incidents
- [ ] Role-based access control (RBAC)
- [ ] Incident assignment to users
- [ ] Comments/Notes on incidents
- [ ] Advanced filtering and search
- [ ] Export to PDF/Excel
- [ ] Dashboard with analytics
- [ ] Multi-tenancy support

---

## 📝 License

This project is for demonstration purposes.

---
