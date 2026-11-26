<div align="center">

# 📦 Inventory Management System

**A production-ready REST API for real-time warehouse & stock operations**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com)
[![EF Core](https://img.shields.io/badge/EF%20Core-8.0-purple)](https://learn.microsoft.com/ef/core)
[![Redis](https://img.shields.io/badge/Redis-7.0-DC382D?logo=redis)](https://redis.io)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![Docker](https://img.shields.io/badge/Docker-ready-2496ED?logo=docker)](https://docker.com)
[![CI](https://img.shields.io/badge/CI-GitHub%20Actions-2088FF?logo=githubactions)](https://github.com/features/actions)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

</div>

---

## 📌 Table of Contents

- [About the Project](#-about-the-project)
- [Problem Statement](#-problem-statement)
- [Architecture Overview](#-architecture-overview)
- [Tech Stack & Versions](#-tech-stack--versions)
- [Prerequisites](#-prerequisites)
- [Installation & Setup](#-installation--setup)
- [Environment Variables](#️-environment-variables)
- [Running the Application](#-running-the-application)
- [API Endpoints](#-api-endpoints)
- [Running Tests](#-running-tests)
- [Docker & CI/CD](#-docker--cicd)
- [Project Structure](#-project-structure)

---

## 🧩 About the Project

The **Inventory Management System** is a backend REST API built with **ASP.NET Core 8** following **Clean Architecture** and **CQRS** patterns. It enables businesses to manage their warehouse inventory — products, stock movements, orders, locations, and user activity — in real time.

The system is designed with **scalability**, **security**, and **maintainability** as first-class concerns, using industry-standard patterns such as CQRS via MediatR, Cache-Aside with Redis, Role-Based Access Control (RBAC) via ASP.NET Core Identity, and pipeline validation with FluentValidation.

---

## 🚨 Problem Statement

Warehouses and retail operations commonly suffer from:

| Problem | Impact |
|--------|--------|
| **No real-time stock visibility** | Overselling, stockouts, lost revenue |
| **Manual inventory tracking** | Human error, slow auditing |
| **No role separation** | Security risks, unauthorized operations |
| **Poor performance at scale** | Slow queries on large product catalogs |
| **No movement audit trail** | Inability to trace discrepancies |

**This system solves all of the above** by providing:
- ✅ Real-time stock tracking with negative-stock prevention
- ✅ Full audit trail for every inventory movement
- ✅ Role-based access control (Admin / Manager / User)
- ✅ Redis caching for high-performance read operations
- ✅ Containerized, CI/CD-ready deployment

---

## 🏛 Architecture Overview

The system follows **Clean / Onion Architecture** with strict dependency inversion:

```
┌─────────────────────────────────────────────────┐
│                     API Layer                   │
│         Controllers · Swagger · Middleware       │
├─────────────────────────────────────────────────┤
│                Application Layer                │
│    CQRS Commands & Queries · MediatR Handlers   │
│         FluentValidation · DTOs · Mapster        │
├─────────────────────────────────────────────────┤
│                  Domain Layer                   │
│         Entities · Interfaces · Enums           │
├─────────────────────────────────────────────────┤
│               Infrastructure Layer              │
│  EF Core · SQL Server · Redis · Identity · JWT  │
└─────────────────────────────────────────────────┘
```

**Data Flow (CQRS + Cache-Aside):**

```
Client → Controller → MediatR → Validator → Handler
                                               │
                                   ┌───────────┴────────────┐
                              Cache HIT                 Cache MISS
                           (Redis → return)       (SQL Server → cache → return)
                                               │
                              Write Commands → SQL Server → Cache Invalidation
```

---

## 🛠 Tech Stack & Versions

### Core Framework
| Technology | Version | Purpose |
|-----------|---------|---------|
| **ASP.NET Core** | 8.0 | Web API framework |
| **C#** | 12.0 | Primary language |
| **.NET SDK** | 8.0 | Runtime & build |

### Application Layer
| Package | Version | Purpose |
|---------|---------|---------|
| **MediatR** | 12.2.0 | CQRS mediator pattern |
| **FluentValidation** | 12.1.1 | Pipeline request validation |
| **Mapster** | 10.0.11 | High-performance object mapping |

### Infrastructure
| Package | Version | Purpose |
|---------|---------|---------|
| **EF Core** | 8.0.28 | ORM & migrations |
| **EF Core SqlServer** | 8.0.27 | SQL Server driver |
| **ASP.NET Core Identity** | 8.0.28 | User/role management |
| **JWT Bearer** | 8.0.0 | Token-based authentication |
| **StackExchange.Redis** | 10.0.12 | Distributed caching |
| **Swashbuckle (Swagger)** | 6.6.2 | API documentation |

### Testing
| Package | Version | Purpose |
|---------|---------|---------|
| **NUnit** | 4.6.1 | Test framework |
| **NUnit3TestAdapter** | 6.3.0 | Test runner |
| **Moq** | 4.20.72 | Mocking framework |
| **EF Core InMemory** | 8.0.28 | In-memory DB for tests |
| **Microsoft.NET.Test.Sdk** | 17.14.1 | Test SDK |

---

## ✅ Prerequisites

Before running the project, make sure the following are installed:

| Tool | Version | Download |
|------|---------|----------|
| **.NET SDK** | 8.0+ | [dotnet.microsoft.com](https://dotnet.microsoft.com/download) |
| **SQL Server** | 2019+ (or Express) | [microsoft.com/sql-server](https://www.microsoft.com/sql-server) |
| **Redis** | 7.0+ | [redis.io](https://redis.io/download) or via Docker |
| **Docker** *(optional)* | 24.0+ | [docker.com](https://docker.com) |
| **Git** | any | [git-scm.com](https://git-scm.com) |

---

## 🚀 Installation & Setup

### Option A — Local Setup (Manual)

**1. Clone the repository**
```bash
git clone https://github.com/AhmedSaye2d/InventoryManagementSystem01.git
cd "InventoryManagementSystem01"
```

**2. Restore dependencies**
```bash
dotnet restore "Inventory Management System.sln"
```

**3. Configure environment variables**

Copy the example settings and fill in your values (see [Environment Variables](#️-environment-variables) section):
```bash
# Edit appsettings.json in the API project with your DB & Redis connection strings
```

**4. Apply database migrations**
```bash
dotnet ef database update --project Infrastructure --startup-project API
```

**5. Build the solution**
```bash
dotnet build "Inventory Management System.sln" --configuration Release
```

**6. Run the API**
```bash
dotnet run --project API
```

The API will start at `https://localhost:5001` and Swagger UI at `https://localhost:5001/swagger`.

---

### Option B — Docker Compose (Recommended)

Runs the full stack (API + SQL Server + Redis) with a single command:

```bash
# Clone the repo
git clone https://github.com/AhmedSaye2d/InventoryManagementSystem01.git
cd "InventoryManagementSystem01"

# Start all services
docker compose up -d

# Check running containers
docker compose ps
```

| Service | URL |
|---------|-----|
| **API** | `http://localhost:8080` |
| **Swagger UI** | `http://localhost:8080/swagger` |
| **SQL Server** | `localhost:1433` |
| **Redis** | `localhost:6379` |

To stop all services:
```bash
docker compose down
```

---

## ⚙️ Environment Variables

Configure the following in `API/appsettings.json` or via environment variables in production:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SQL_SERVER;Database=InventoryMS;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True",
    "RedisCacheUrl": "YOUR_REDIS_HOST:6379"
  },
  "Jwt": {
    "Key": "YOUR_SUPER_SECRET_KEY_MIN_32_CHARS",
    "Issuer": "InventoryManagementApi",
    "Audience": "InventoryManagementApp"
  },
  "SeedAdmin": {
    "Email": "admin@yourdomain.com",
    "Password": "StrongPassword@123",
    "Name": "System Admin"
  },
  "SeedData": {
    "Enabled": true
  }
}
```

> [!IMPORTANT]
> **Never commit real credentials to source control.** Use environment variables, Docker secrets, or Azure Key Vault in production.

| Variable | Required | Description |
|----------|----------|-------------|
| `ConnectionStrings__DefaultConnection` | ✅ Yes | SQL Server connection string |
| `ConnectionStrings__RedisCacheUrl` | ✅ Yes | Redis host and port |
| `Jwt__Key` | ✅ Yes | Secret key for JWT signing (min 32 chars) |
| `Jwt__Issuer` | ✅ Yes | JWT token issuer name |
| `Jwt__Audience` | ✅ Yes | JWT token audience name |
| `SeedAdmin__Email` | Optional | Admin account email for seeding |
| `SeedAdmin__Password` | Optional | Admin account password for seeding |
| `SeedData__Enabled` | Optional | Whether to seed demo data on startup |

---

## 🗺 API Endpoints

All endpoints require a **Bearer JWT token** in the `Authorization` header except for `/api/Account/login` and `/api/Account/register`.

### 🔐 Authentication — `/api/Account`
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `POST` | `/api/Account/register` | ❌ | Register a new user |
| `POST` | `/api/Account/login` | ❌ | Login and get JWT token |
| `GET` | `/api/Account/users` | Admin | List all users |
| `GET` | `/api/Account/roles` | Admin | List all roles |
| `POST` | `/api/Account/assign-role` | Admin | Assign a role to a user |
| `DELETE` | `/api/Account/delete-user/{id}` | Admin | Delete a user |

### 📦 Products — `/api/Product`
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `GET` | `/api/Product` | Any | Get all products (cached) |
| `GET` | `/api/Product/{id}` | Any | Get product by ID (cached) |
| `POST` | `/api/Product` | Manager+ | Create a new product |
| `PUT` | `/api/Product/{id}` | Manager+ | Update a product |
| `DELETE` | `/api/Product/{id}` | Admin | Delete a product |

### 🗂 Categories — `/api/Category`
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `GET` | `/api/Category` | Any | Get all categories (cached) |
| `GET` | `/api/Category/{id}` | Any | Get category by ID |
| `POST` | `/api/Category` | Manager+ | Create a new category |
| `PUT` | `/api/Category/{id}` | Manager+ | Update a category |
| `DELETE` | `/api/Category/{id}` | Admin | Delete a category |

### 📍 Locations — `/api/Location`
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `GET` | `/api/Location` | Any | Get all locations (cached) |
| `GET` | `/api/Location/{id}` | Any | Get location by ID |
| `POST` | `/api/Location` | Manager+ | Create a new location |
| `PUT` | `/api/Location/{id}` | Manager+ | Update a location |
| `DELETE` | `/api/Location/{id}` | Admin | Delete a location |

### 📊 Inventory — `/api/Inventory`
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `GET` | `/api/Inventory` | Any | Get full inventory snapshot |
| `GET` | `/api/Inventory/{productId}` | Any | Get inventory for a product |
| `POST` | `/api/Inventory/stock-in` | Manager+ | Record stock-in movement |
| `POST` | `/api/Inventory/stock-out` | Manager+ | Record stock-out (validates no negative stock) |

### 🔄 Inventory Movements — `/api/InventoryMovement`
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `GET` | `/api/InventoryMovement` | Manager+ | Get all movements |
| `GET` | `/api/InventoryMovement/{id}` | Manager+ | Get a single movement |
| `GET` | `/api/InventoryMovement/product/{productId}` | Manager+ | Movements by product |
| `GET` | `/api/InventoryMovement/location/{locationId}` | Manager+ | Movements by location |

### 🛒 Orders — `/api/Order`
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `GET` | `/api/Order` | Manager+ | Get all orders |
| `GET` | `/api/Order/{id}` | Any | Get order by ID |
| `POST` | `/api/Order` | Any | Place a new order |
| `PUT` | `/api/Order/{id}/status` | Manager+ | Update order status |
| `DELETE` | `/api/Order/{id}` | Admin | Cancel/delete an order |

> [!TIP]
> After running the app, visit **`/swagger`** for the full interactive API documentation with request/response examples.

---

## 🧪 Running Tests

The project includes unit tests covering core business logic (inventory movements, stock operations, and product management).

**Run all tests:**
```bash
dotnet test "Inventory Management System.sln" --verbosity normal
```

**Run with code coverage:**
```bash
dotnet test "Inventory Management System.sln" \
  --collect:"XPlat Code Coverage" \
  --results-directory ./TestResults \
  --verbosity normal
```

**Run a specific test file:**
```bash
dotnet test Testing/Testing.csproj --filter "FullyQualifiedName~StockInOut"
```

### Test Coverage Areas

| Test File | What It Tests |
|-----------|--------------|
| `StockInOut_Test.cs` | Stock-in / stock-out business rules, negative stock prevention |
| `InventoryMovement_Test.cs` | Movement recording, audit trail validation |
| `Product_Test.cs` | Product creation, validation, uniqueness constraints |

**Testing Stack:**
- **NUnit 4.6.1** — test framework
- **Moq 4.20.72** — repository mocking
- **EF Core InMemory** — fast in-memory database for isolation

---

## 🐳 Docker & CI/CD

### Docker

The project uses a **multi-stage Dockerfile** for minimal production image size:

| Stage | Base Image | Purpose |
|-------|-----------|---------|
| `restore` | `mcr.microsoft.com/dotnet/sdk:8.0` | Restore NuGet packages |
| `build` | `restore` | Build & publish |
| `runtime` | `mcr.microsoft.com/dotnet/aspnet:8.0` | Lean production image |

**Build manually:**
```bash
docker build -t inventory-management-system .
docker run -p 8080:8080 inventory-management-system
```

---

### CI Pipeline (GitHub Actions)

Triggers on **every push and pull request**:

```
Push / PR → Restore → Build → Unit Tests → Security Scan
```

| Job | Description |
|-----|-------------|
| 🔨 **Build** | Restore packages, compile in Release mode, check formatting |
| 🧪 **Test** | Run NUnit tests with SQL Server + Redis service containers |
| 🔒 **Security Scan** | Check for vulnerable and deprecated NuGet packages |

---

### CD Pipeline (GitHub Actions)

Triggers on **push to `main`** or **GitHub Release**:

```
Push to main → Build & Test → Docker Build → Push to GHCR → Deploy to Staging
GitHub Release → Build & Test → Docker Build → Push to GHCR → Deploy to Production
```

Docker images are published to **GitHub Container Registry (GHCR)**:
```
ghcr.io/ahmedsaye2d/inventory-management-system:latest
ghcr.io/ahmedsaye2d/inventory-management-system:<version>
```

---

## 📁 Project Structure

```
Inventory Management System/
│
├── API/                          # Presentation layer
│   ├── Controllers/              # 7 REST controllers
│   ├── appsettings.json          # App configuration
│   └── Program.cs                # App entry point & DI setup
│
├── Application/                  # Business logic layer
│   ├── Features/
│   │   ├── Commands/             # 14 write operations (CQRS)
│   │   └── Queries/              # 15 read operations (CQRS)
│   ├── Interfaces/               # Repository & service abstractions
│   └── Validators/               # FluentValidation validators
│
├── Domain/                       # Core domain layer
│   └── Entities/                 # 9 domain entities
│
├── Infrastructure/               # Data access layer
│   ├── Caching/                  # Redis cache service
│   ├── Data/                     # EF Core DbContext & migrations
│   ├── Reposito/                 # 6 repository implementations
│   └── Services/                 # JWT, Identity, seed data
│
├── Testing/                      # Unit test project
│   ├── StockInOut_Test.cs
│   ├── InventoryMovement_Test.cs
│   └── Product_Test.cs
│
├── .github/
│   └── workflows/
│       ├── ci.yml                # CI pipeline
│       └── cd.yml                # CD pipeline
│
├── docker-compose.yml            # Full-stack local environment
├── Dockerfile                    # Multi-stage production build
└── Inventory Management System.sln
```

---

## 🔐 Security Design

| Feature | Implementation |
|---------|---------------|
| **Authentication** | JWT Bearer tokens |
| **Authorization** | ASP.NET Core Identity + Role-based policies |
| **Privilege Escalation Prevention** | Whitelist-based role assignment (`AllowedCreateRoles`) |
| **Input Validation** | FluentValidation pipeline on every command |
| **Non-root Container** | Docker runs as `appuser` (not root) |
| **Secret Management** | Environment variables / Docker secrets (never hard-coded) |

---

## 📄 License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

---

<div align="center">

**Built with ❤️ using ASP.NET Core 8 · Clean Architecture · CQRS**

[⬆ Back to Top](#-inventory-management-system)

</div>
