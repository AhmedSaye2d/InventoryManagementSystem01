# 📦 Enterprise Inventory & Stock Management System

<p align="center">

### A Scalable Inventory & Warehouse Management RESTful API

Built with **C# · ASP.NET Core 8 · Clean Architecture · CQRS · MediatR · SQL Server · Redis · Docker**

</p>

---

## 📑 Table of Contents

- [Project Overview](#-project-overview)
- [Business Problem](#-business-problem)
- [Key Features](#-key-features)
- [Technology Stack](#-technology-stack)
- [System Architecture](#-system-architecture)
- [Project Structure](#-project-structure)
- [Core Business Logic](#-core-business-logic)
- [Database Design](#-database-design)
- [API Documentation](#-api-documentation)
- [CQRS and MediatR](#-cqrs-and-mediatr)
- [Redis Caching](#-redis-caching)
- [Authentication and Authorization](#-authentication-and-authorization)
- [Repository Pattern and Unit of Work](#-repository-pattern-and-unit-of-work)
- [Docker and Containerization](#-docker-and-containerization)
- [CI/CD Pipeline](#-cicd-pipeline)
- [Testing](#-testing)
- [Getting Started](#-getting-started)
- [Configuration](#-configuration)
- [Future Improvements](#-future-improvements)
- [License](#-license)

---

# 📌 Project Overview

The **Enterprise Inventory & Stock Management System** is a backend RESTful API developed using **C# and ASP.NET Core 8**.

The system is designed to manage inventory operations, warehouse locations, product catalogs, customer orders, stock movements, user accounts, and operational reports.

It provides a centralized solution for managing warehouse inventory while maintaining accurate stock quantities, tracking every inventory movement, and controlling access to system operations.

The application follows **Clean Architecture** principles and implements **CQRS (Command Query Responsibility Segregation)** using MediatR to separate read and write operations.

It integrates Microsoft SQL Server for persistent data storage and Redis for distributed caching to improve application performance.

### Project Highlights

- **39 RESTful API Endpoints** across 7 Controllers.
- Clean Architecture with separated application responsibilities.
- CQRS implementation using MediatR.
- Repository Pattern and Unit of Work.
- Entity Framework Core with SQL Server 2022.
- Redis Distributed Caching.
- JWT Authentication.
- Role-Based and Claim-Based Authorization.
- Inventory Movement Tracking and Audit Logging.
- Automated Stock Deduction during Order Creation.
- Docker and Docker Compose.
- GitHub Actions CI/CD.
- Automated Unit Testing using NUnit and Moq.

---

# 🎯 Business Problem

Traditional inventory management systems can face several operational challenges:

### 1. Stock Inconsistency

Uncontrolled inventory updates can lead to inaccurate stock quantities.

**Solution:** Implement dedicated Stock-In and Stock-Out operations with inventory validation.

### 2. Negative Inventory

Stock deductions may exceed the available quantity, resulting in incorrect inventory balances.

**Solution:** Validate available stock before executing stock-out operations.

### 3. Lack of Inventory Tracking

Without proper tracking, it becomes difficult to identify who performed an inventory operation and when it happened.

**Solution:** Maintain an Inventory Movement Ledger that records stock changes and related user information.

### 4. Warehouse Management Complexity

Managing products across multiple warehouse locations requires organized location tracking.

**Solution:** Introduce warehouse Location management and associate products with their storage locations.

### 5. Performance Issues

Frequently accessing product information can generate unnecessary database queries.

**Solution:** Integrate Redis using the Cache-Aside Pattern.

### 6. Security Risks

Different users should have different levels of access depending on their responsibilities.

**Solution:** Implement JWT Authentication with Role-Based and Claim-Based Authorization Policies.

---

# 🚀 Key Features

## 1. Product Management

Manage the product catalog through dedicated API endpoints.

Features include:

- Create new products.
- Retrieve all products.
- Retrieve product details by ID.
- Update existing products.
- Delete products.
- Retrieve products with their associated category and location.
- Validate duplicate product names.
- Prevent duplicate Serial Numbers using a Unique Database Index.
- Redis caching for product listing.
- Automatic cache invalidation after product modifications.

## 2. Category Management

Organize products into categories.

Features include:

- Create categories.
- Retrieve all categories.
- Retrieve a category by ID.
- Retrieve categories with their associated products.
- Update category information.
- Delete categories.
- Validate duplicate category information.

## 3. Warehouse Location Management

Manage physical warehouse locations and storage shelves.

Features include:

- Create warehouse locations.
- Retrieve all locations.
- Retrieve location details by ID.
- Update location information.
- Delete locations.
- Associate products with warehouse locations.

## 4. Inventory Management

The Inventory module handles the main stock operations.

### Stock In

Increases the available quantity of a product.

Workflow:

1. Receive the product and quantity information.
2. Validate the requested operation.
3. Increase the available stock quantity.
4. Create an Inventory Movement record.
5. Record the positive stock movement.
6. Invalidate the related Redis cache entries.

### Stock Out

Deducts a quantity from available inventory.

Workflow:

1. Receive the product and requested quantity.
2. Check the available stock quantity.
3. Reject the operation if the requested quantity exceeds the available balance.
4. Decrease the stock quantity.
5. Create an Inventory Movement record.
6. Record the negative stock movement.
7. Invalidate the related Redis cache entries.

**Important:** The system prevents stock deductions that would result in negative inventory.

## 5. Inventory Movement Tracking

The Inventory Movement module maintains a historical record of stock changes.

Each movement can contain:

- Product ID.
- Location ID.
- User ID.
- Movement Type.
- Quantity.
- Quantity After Operation.
- Created Date.
- Optional Reference ID.

Supported operations:

- Retrieve all inventory movements.
- Filter movements by Product.
- Filter movements by Location.
- Filter movements by Movement Type.
- Filter movements by User.
- Filter movements by Date Range.
- Retrieve movement details by ID.
- Retrieve product movement history.
- Pagination and filtering.
- Stock-In Reports.
- Stock-Out Reports.
- Adjustment Reports.

This provides traceability for inventory operations and helps monitor stock changes.

## 6. Order Management

Manage customer orders and their inventory-related operations.

Features include:

- Create new orders.
- Retrieve all orders.
- Retrieve orders by user ID.
- Update order status.
- Update delivery dates.
- Cancel orders.
- Retrieve order counts grouped by status.
- Generate monthly order/revenue reports.
- Retrieve the top-ordered product.

### Order Status Lifecycle

The documented order lifecycle includes:

`Pending → Processing → Delivering → Delivered`

Orders can also be cancelled through the cancellation endpoint.

### Automatic Stock Deduction

When an order is created:

1. The system receives the order information.
2. Validates the requested order data.
3. Calculates the order total.
4. Checks the available product quantity.
5. Deducts the requested quantity from inventory.
6. Saves the order information.

## 7. Reporting and Analytics

The system provides reporting capabilities using LINQ queries and database aggregations.

Available reports include:

- Monthly revenue breakdown.
- Top-ordered products.
- Order counts grouped by status.
- Inventory Stock-In reports.
- Inventory Stock-Out reports.
- Inventory Adjustment reports.
- Product movement history.

## 8. User Management and Activity Tracking

The Account module provides authentication and user-management functionality.

Features include:

- User login.
- JWT Token generation.
- Create users with role restrictions.
- Retrieve system users.
- Initial Identity Setup.
- Update user roles and claims.
- Retrieve user activities.
- Record user activities using authenticated JWT identity claims.

---

# 💻 Technology Stack

| Component | Technology | Version | Purpose |
|---|---|---|---|
| Programming Language | C# | - | Main development language |
| Framework | .NET | 8.0 | Application framework |
| API Framework | ASP.NET Core Web API | 8.0 | RESTful API development |
| ORM | Entity Framework Core | 8.0.28 | Database access |
| Database | Microsoft SQL Server | 2022 | Relational database |
| CQRS / Mediator | MediatR | 12.2.0 | Request handling |
| Validation | FluentValidation | 12.1.1 | Request validation |
| Distributed Cache | StackExchange.Redis | 10.0.12 | Redis integration |
| Authentication | JWT Bearer | - | Token-based authentication |
| Identity | ASP.NET Core Identity | 8.0.28 | User management |
| Object Mapping | Mapster | 10.0.11 | Object mapping |
| Unit Testing | NUnit | 4.6.1 | Automated testing |
| Mocking | Moq | - | Dependency mocking |
| Test Database | EF Core InMemory | - | Testing database operations |
| Containerization | Docker | - | Application containerization |
| Orchestration | Docker Compose | - | Multi-container management |
| CI/CD | GitHub Actions | - | Automated workflows |
| Container Registry | GHCR | - | Docker image publishing |

---

# 🏗️ System Architecture

The project follows **Clean Architecture** principles to achieve separation of concerns, maintainability, and testability.

The application is organized into four main layers.

## 1. API Layer (Presentation Layer)

Responsible for handling incoming HTTP requests and returning HTTP responses.

Responsibilities:

- API Controllers.
- HTTP Request Handling.
- Response Management.
- Dependency Injection Configuration.
- Swagger Configuration.
- Middleware Registration.
- Application Startup Configuration.

## 2. Application Layer

Contains the application's use cases and business operation orchestration.

Responsibilities:

- CQRS Commands and Queries.
- MediatR Handlers.
- Request DTOs.
- Response DTOs.
- Application Interfaces.
- Request Validation.
- Application Services.

## 3. Domain Layer

Contains the core business entities and domain concepts.

Main entities include:

- Product.
- Category.
- Location.
- Order.
- InventoryMovement.
- Activity Tracking Entity.

This layer represents the core business model of the application.

## 4. Infrastructure Layer

Responsible for implementing external technical concerns and persistence operations.

Responsibilities:

- Entity Framework Core.
- AppDbContext.
- Fluent API Configurations.
- Repository Implementations.
- Unit of Work Implementation.
- Redis Cache Implementation.
- MediatR Pipeline Behaviors.
- Middleware Implementations.

## Request Execution Flow

A typical request follows this execution path:

```text
HTTP Request
     |
     v
Request Logging Middleware
     |
     v
API Controller
     |
     v
MediatR
     |
     v
Validation Behavior
     |
     v
Command / Query Handler
     |
     v
Repository / Unit of Work
     |
     +-------------------+
     |                   |
     v                   v
 SQL Server            Redis
     |                   |
     +---------+---------+
               |
               v
         API Response
```

The exact path depends on the operation. Cached queries may be served directly from Redis without accessing SQL Server.

---

# 📁 Project Structure

```text
Inventory Management System/
│
├── API/                                  # Presentation Layer
│   ├── Controllers/
│   │   ├── AccountController.cs
│   │   ├── CategoryController.cs
│   │   ├── InventoryController.cs
│   │   ├── InventoryMovementController.cs
│   │   ├── LocationController.cs
│   │   ├── OrderController.cs
│   │   └── ProductController.cs
│   │
│   ├── Program.cs
│   └── appsettings.json
│
├── Application/                          # Application Layer
│   ├── DTO/
│   │   ├── Request/
│   │   └── Response/
│   │
│   ├── Interfaces/
│   └── Services/
│       ├── Inventory/
│       ├── Order/
│       └── Product/
│
├── Domain/                               # Domain Layer
│   └── Entities/
│
├── Infrastructure/                      # Infrastructure Layer
│   ├── Behaviors/
│   ├── Caching/
│   ├── DataAccess/
│   │   └── Configuration/
│   ├── Middleware/
│   ├── Repositories/
│   └── UnitOfWork/
│
├── Testing/
│   ├── InventoryTest/
│   └── ProductTest/
│
├── .github/
│   └── workflows/
│       ├── ci.yml
│       └── cd.yml
│
├── docker-compose.yml
└── Dockerfile
```

### Folder Responsibilities

| Folder | Responsibility |
|---|---|
| API/Controllers | Handle incoming HTTP requests |
| Application/DTO | Request and Response objects |
| Application/Interfaces | Application abstractions |
| Application/Services | Commands, Queries, and Handlers |
| Domain/Entities | Core business entities |
| Infrastructure/DataAccess | Database context and configurations |
| Infrastructure/Caching | Redis implementation |
| Infrastructure/Repositories | Repository implementations |
| Infrastructure/UnitOfWork | Persistence coordination |
| Infrastructure/Behaviors | MediatR pipeline behaviors |
| Infrastructure/Middleware | Request logging and exception handling |
| Testing | Automated test projects |
| .github/workflows | CI/CD pipelines |

---

# 🔄 Core Business Logic

## Inventory Stock-In Workflow

```text
Stock-In Request
      |
      v
Validate Request
      |
      v
Load Product Information
      |
      v
Increase Stock Quantity
      |
      v
Create Inventory Movement
      |
      v
Save Database Changes
      |
      v
Invalidate Redis Cache
      |
      v
Return Response
```

## Inventory Stock-Out Workflow

```text
Stock-Out Request
      |
      v
Validate Request
      |
      v
Load Product Information
      |
      v
Check Available Quantity
      |
      v
Is Stock Sufficient?
    /       \
  No         Yes
  |           |
  v           v
Reject     Decrease Stock
              |
              v
      Create Inventory Movement
              |
              v
        Save Changes
              |
              v
       Invalidate Cache
              |
              v
        Return Response
```

## Order Creation Workflow

```text
Create Order Request
        |
        v
Validate Order Data
        |
        v
Calculate Order Total
        |
        v
Check Available Stock
        |
        v
Deduct Product Quantity
        |
        v
Save Order Information
        |
        v
Return Created Order
```

---

# 🗄️ Database Design

The application uses Microsoft SQL Server 2022 as its relational database and Entity Framework Core for data access.

## Main Entities

| Entity | Responsibility |
|---|---|
| Product | Stores product information |
| Category | Organizes products into categories |
| Location | Represents warehouse locations |
| Order | Stores order information |
| InventoryMovement | Records inventory changes |
| Activity Tracker | Tracks user operations |

## Database Constraints and Configurations

### 1. Delete Behavior Restriction

The entity relationships use:

```csharp
OnDelete(DeleteBehavior.Restrict)
```

This prevents unintended cascading deletes and helps protect related inventory and movement records.

### 2. Unique Serial Number

The `SerialNumber` field in the Products table has a Unique Index.

This prevents duplicate product serial numbers at the database level.

### 3. Inventory Movement Indexing

The InventoryMovements table has indexes on five important fields:

- ProductId
- LocationId
- CreatedAt
- MovementType
- UserId

These indexes support filtering and reporting operations.

### 4. Financial Precision

Financial fields use:

```sql
decimal(18,2)
```

This provides fixed decimal precision for product prices and monetary totals.

> **Note:** The exact entity cardinalities and complete foreign-key structure should be represented in an ERD generated from the actual EF Core configurations.

---

# 🔌 API Documentation

The application exposes **39 RESTful API endpoints** organized across 7 Controllers.

## Authorization Policies

| Policy | Purpose |
|---|---|
| AllowAnonymous | Endpoint does not require authentication |
| Authorize | Requires an authenticated user |
| AdminPolicy | Administrative access |
| ManagerPolicy | Manager-level access |
| ReadPolicy | Read permission |
| CreatePolicy | Create permission |
| UpdatePolicy | Update permission |
| DeletePolicy | Delete permission |

## 1. AccountController

Base Route: `/api/account`

| # | HTTP | Endpoint | Action | Authorization | Description |
|---|---|---|---|---|---|
| 1 | POST | `/api/account/login` | Login | AllowAnonymous | Authenticate user and generate JWT |
| 2 | POST | `/api/account/create` | CreateUser | AdminPolicy | Create a user (Manager or User only) |
| 3 | GET | `/api/account/users` | GetUsers | AdminPolicy | Retrieve all system users |
| 4 | POST | `/api/account/identity/setup` | Setup | AllowAnonymous | Initialize identity roles and initial Admin |
| 5 | PUT | `/api/account/update-claims` | UpdateUserClaims | AdminPolicy | Update user roles and claims |
| 6 | GET | `/api/account/activities` | GetActivities | ManagerPolicy | Retrieve activity and audit records |
| 7 | POST | `/api/account/activity` | SaveActivity | Authorize | Record authenticated user activity |

## 2. CategoryController

Base Route: `/api/category`

| # | HTTP | Endpoint | Action | Authorization | Description |
|---|---|---|---|---|---|
| 8 | GET | `/api/category` | GetAllCategories | ReadPolicy | Retrieve all categories |
| 9 | GET | `/api/category/{id:guid}` | GetCategoryById | ReadPolicy | Retrieve category by ID |
| 10 | GET | `/api/category/with-products` | GetAllCategoriesWithProducts | ReadPolicy | Retrieve categories with products |
| 11 | POST | `/api/category` | CreateCategory | CreatePolicy | Create a category |
| 12 | PUT | `/api/category` | UpdateCategory | UpdatePolicy | Update category information |
| 13 | DELETE | `/api/category/{id:guid}` | DeleteCategory | DeletePolicy | Delete a category |

## 3. InventoryController

Base Route: `/api/inventory`

| # | HTTP | Endpoint | Action | Authorization | Description |
|---|---|---|---|---|---|
| 14 | POST | `/api/inventory/stock-in` | StockIn | CreatePolicy | Increase stock and record movement |
| 15 | POST | `/api/inventory/stock-out` | StockOut | CreatePolicy | Deduct stock after quantity validation |

## 4. InventoryMovementController

Base Route: `/api/inventorymovement`

| # | HTTP | Endpoint | Action | Authorization | Description |
|---|---|---|---|---|---|
| 16 | GET | `/api/inventorymovement` | GetInventoryMovements | ReadPolicy | Retrieve filtered and paginated movements |
| 17 | GET | `/api/inventorymovement/{id:guid}` | GetById | ReadPolicy | Retrieve movement details |
| 18 | GET | `/api/inventorymovement/product/{productId:guid}` | GetProductHistory | ReadPolicy | Retrieve product movement history |
| 19 | GET | `/api/inventorymovement/reports/stock-in` | GetStockInReport | ReadPolicy | Stock-In report |
| 20 | GET | `/api/inventorymovement/reports/stock-out` | GetStockOutReport | ReadPolicy | Stock-Out report |
| 21 | GET | `/api/inventorymovement/reports/adjustments` | GetAdjustmentReport | ReadPolicy | Adjustment report |

Supported filters:

- Product.
- Location.
- Movement Type.
- User.
- Date Range.

Pagination is also supported.

## 5. LocationController

Base Route: `/api/location`

| # | HTTP | Endpoint | Action | Authorization | Description |
|---|---|---|---|---|---|
| 22 | GET | `/api/location` | GetAllLocations | ReadPolicy | Retrieve warehouse locations |
| 23 | GET | `/api/location/{id:guid}` | GetLocationById | ReadPolicy | Retrieve location by ID |
| 24 | POST | `/api/location` | CreateLocation | CreatePolicy | Create a location |
| 25 | PUT | `/api/location` | UpdateLocation | UpdatePolicy | Update location information |
| 26 | DELETE | `/api/location/{id:guid}` | DeleteLocation | DeletePolicy | Delete a location |

## 6. OrderController

Base Route: `/api/order`

| # | HTTP | Endpoint | Action | Authorization | Description |
|---|---|---|---|---|---|
| 27 | GET | `/api/order` | GetAllOrders | ManagerPolicy | Retrieve all orders with related details |
| 28 | GET | `/api/order/user/{userId}` | GetOrdersByUserId | ReadPolicy | Retrieve orders by user ID |
| 29 | GET | `/api/order/count` | GetOrderCount | ReadPolicy | Retrieve order counts by status |
| 30 | GET | `/api/order/monthly` | GetProductOrderByMonth | ManagerPolicy | Monthly order/revenue aggregation |
| 31 | GET | `/api/order/top-ordered` | GetTopOrderedProduct | ReadPolicy | Retrieve top-ordered product |
| 32 | POST | `/api/order` | CreateOrder | ReadPolicy | Create order and deduct stock |
| 33 | PUT | `/api/order` | UpdateOrder | ManagerPolicy | Update order status and delivery date |
| 34 | PUT | `/api/order/cancel/{orderId:guid}` | CancelOrder | ReadPolicy | Cancel an order |

## 7. ProductController

Base Route: `/api/product`

| # | HTTP | Endpoint | Action | Authorization | Description |
|---|---|---|---|---|---|
| 35 | GET | `/api/product` | GetAllProducts | ReadPolicy | Retrieve all products with Redis caching |
| 36 | GET | `/api/product/{id:guid}` | GetProductById | ReadPolicy | Retrieve product with category and location |
| 37 | POST | `/api/product` | CreateProduct | CreatePolicy | Create product and validate duplicates |
| 38 | PUT | `/api/product/{id:guid}` | UpdateProduct | UpdatePolicy | Update product and invalidate cache |
| 39 | DELETE | `/api/product/{id:guid}` | DeleteProduct | DeletePolicy | Delete product and invalidate cache |

### API Summary

| Controller | Endpoints |
|---|---:|
| AccountController | 7 |
| CategoryController | 6 |
| InventoryController | 2 |
| InventoryMovementController | 6 |
| LocationController | 5 |
| OrderController | 8 |
| ProductController | 5 |
| **Total** | **39** |

---

# 🔄 CQRS and MediatR

The application uses CQRS to separate operations that modify application state from operations that retrieve information.

## Commands

Commands represent operations that change application data.

Examples:

- CreateProductCommand.
- UpdateProductCommand.
- StockInCommand.
- StockOutCommand.
- CreateOrderCommand.
- CancelOrderCommand.

## Queries

Queries represent operations that retrieve data without modifying application state.

Examples:

- GetAllProductsQuery.
- GetProductByIdQuery.
- GetInventoryMovementsQuery.
- GetOrderCountQuery.
- GetMonthlyRevenueQuery.
- GetTopOrderedProductQuery.

## MediatR Pipeline

The application uses MediatR to dispatch requests to their corresponding handlers.

The documented request flow includes:

1. API Controller receives a request.
2. Controller sends the request through MediatR.
3. Validation Behavior processes the request.
4. The corresponding Handler executes the use case.
5. Repositories and Unit of Work handle persistence operations.
6. The result is returned to the API.

### Benefits

- Separation of read and write operations.
- Organized application use cases.
- Reduced controller responsibilities.
- Easier unit testing.
- Improved maintainability.

---

# ⚡ Redis Caching

The application integrates Redis as a distributed caching solution.

It follows the **Cache-Aside Pattern** to improve product retrieval performance.

## Cache-Aside Workflow

```text
Client Request
      |
      v
Check Redis Cache
      |
      v
Is Data Available?
    /       \
  Yes        No
  |           |
  v           v
Return      Query SQL Server
Cached          |
Data            v
             Store in Redis
                 |
                 v
             Return Data
```

## Cache Invalidation

Whenever product or inventory data changes, related cache entries are removed.

Example:

```csharp
await _cacheService.RemoveAsync("products:all");

await _cacheService.RemoveAsync($"product:{product.Id}");
```

### Cache Keys

| Cache Key | Description |
|---|---|
| `products:all` | Cached product listing |
| `product:{productId}` | Cached product details |

### Cache Expiration

The product-list endpoint uses a Redis cache duration of **10 minutes**.

### Serialization Configuration

The application uses:

```csharp
ReferenceHandler.IgnoreCycles
```

to prevent cyclic-reference serialization errors.

---

# 🔐 Authentication and Authorization

The system uses JWT Bearer Authentication and role/claim-based authorization.

## JWT Authentication

The login endpoint authenticates users and generates JWT tokens.

The documented token claims include:

- User ID.
- User Name.
- Email.
- Role.

Authenticated requests send the token using the HTTP Authorization header.

Example:

```http
Authorization: Bearer <your-jwt-token>
```

## Role-Based Authorization

The application supports three documented roles:

| Role | Description |
|---|---|
| Admin | Administrative access |
| Manager | Management-level permissions |
| User | Standard user access |

## Claim-Based Authorization

The application uses granular permission claims:

- create
- update
- delete
- read
- MangerUser

## Authorization Policies

The application defines policies such as:

- AdminPolicy.
- ManagerPolicy.
- CreatePolicy.
- UpdatePolicy.
- DeletePolicy.
- ReadPolicy.

## Privilege Escalation Protection

The regular user-creation endpoint restricts the allowed roles.

Example:

```csharp
private static readonly HashSet<string> AllowedCreateRoles =
    new(StringComparer.OrdinalIgnoreCase)
{
    Policy.RoleClaim.Manager,
    Policy.RoleClaim.User
};
```

This prevents assigning the Admin role through the regular user-creation endpoint.

## Initial Identity Setup

The endpoint:

`POST /api/account/identity/setup`

is designed to initialize the system roles and initial administrator.

It is documented as idempotent, meaning repeated execution should not recreate existing setup data unnecessarily.

---

# 🔄 Repository Pattern and Unit of Work

The application uses the Repository Pattern to abstract data access operations.

## Repository Pattern

Repositories separate database access implementation from application use cases.

Benefits:

- Centralized data access logic.
- Reduced direct database dependencies in handlers.
- Improved testability.
- Better separation of concerns.

## Unit of Work

The Unit of Work coordinates persistence operations.

It is especially relevant to inventory operations where related changes need to be saved together, such as:

- Updating product stock.
- Creating an inventory movement record.

The exact transaction implementation is determined by the Unit of Work and handler code.

---

# 🐳 Docker and Containerization

The project supports containerized development using Docker and Docker Compose.

The environment contains three main services:

| Service | Technology | Purpose |
|---|---|---|
| API | ASP.NET Core 8 | RESTful API |
| SQL Server | SQL Server 2022 | Database |
| Redis | Redis 7 Alpine | Distributed caching |

## Dockerfile

The Dockerfile uses a three-stage Multi-stage Build.

### 1. Restore Stage

Copies project files and restores NuGet dependencies.

This structure improves Docker Layer Caching.

### 2. Build Stage

Builds and publishes the application.

### 3. Runtime Stage

Uses the lightweight ASP.NET Core 8 runtime image.

The runtime container includes:

- Non-root user (`appuser`).
- Non-root group (`appgroup`).
- Health check configured against `/health`.

## Docker Compose

The `docker-compose.yml` file orchestrates all application services.

Features include:

- SQL Server 2022 container.
- Redis 7 container.
- API container.
- Shared `inventory-network`.
- SQL Server health check using `sqlcmd`.
- Redis health check using `redis-cli ping`.
- API startup dependency on healthy SQL Server and Redis services.

### Start Containers

```bash
docker compose up -d
```

### Check Container Status

```bash
docker compose ps
```

### View Logs

```bash
docker compose logs -f
```

### Stop Containers

```bash
docker compose down
```

---

# 🚢 CI/CD Pipeline

The project uses GitHub Actions to automate build, testing, security scanning, and Docker image publishing.

The workflows are located in:

```text
.github/workflows/
├── ci.yml
└── cd.yml
```

## 1. Continuous Integration (CI)

The CI workflow runs automatically on Push and Pull Request events.

It contains three main jobs.

### Build Job

Responsibilities:

- Restore dependencies.
- Build the solution in Release configuration.
- Verify code formatting.
- Run `dotnet format --verify-no-changes`.

### Test Job

Responsibilities:

- Start SQL Server 2022 as a GitHub Actions service container.
- Start Redis 7 as a GitHub Actions service container.
- Wait for service health checks.
- Execute automated NUnit tests.
- Generate code coverage using XPlat Code Coverage.
- Upload test results.
- Upload coverage reports.

Generated artifacts include:

- `test-results.trx`
- `coverage.cobertura.xml`

Artifacts are configured to be retained for 30 days.

### Security Scan Job

The security scan checks project dependencies for vulnerable and deprecated packages.

Commands:

```bash
dotnet list package --vulnerable
dotnet list package --deprecated
```

## 2. Continuous Delivery (CD)

The CD workflow includes:

### Build and Test

Builds and tests the application before Docker image publishing.

### Docker Build and Publish

The workflow:

1. Builds the Docker image.
2. Authenticates with GitHub Container Registry.
3. Uses GitHub's `GITHUB_TOKEN`.
4. Publishes the Docker image to GHCR.
5. Applies version and branch-related image tags.

Required package permission:

```yaml
permissions:
  contents: read
  packages: write
```

### Staging and Production Deployment

The workflow contains staging and production deployment jobs.

The documented conditions include:

- Main branch pushes for staging.
- Published releases for production.

**Deployment Status:**

The SSH deployment commands using `appleboy/ssh-action` are currently commented out.

Therefore:

- CI build and test automation is configured.
- Docker image publishing is automated.
- Staging and production deployment workflows are prepared.
- Actual remote SSH deployment requires enabling the deployment steps and configuring the required GitHub Secrets.

Documented staging secrets include:

- `STAGING_HOST`
- `STAGING_SSH_KEY`

### CI/CD Summary

Automated CI/CD workflows for building, testing with containerized dependencies, scanning project dependencies, and publishing Docker images to GitHub Container Registry, with staging and production deployment workflows prepared for activation.

---

# 🧪 Testing

The project includes automated tests using:

- NUnit.
- Moq.
- Entity Framework Core InMemory.

## Tested Scenarios

| Test Scenario | Expected Behavior |
|---|---|
| Stock-In | Increases product quantity |
| Stock-In Movement | Creates a positive movement |
| Stock-Out | Decreases product quantity |
| Stock-Out Movement | Creates a negative movement |
| Insufficient Stock | Rejects the operation |
| Failed Stock-Out | Does not alter inventory state |
| Pagination | Validates paginated movement results |
| Dynamic Sorting | Validates sorting behavior |
| Product Validation | Rejects invalid product data |
| Duplicate Product | Prevents duplicate product names |

### Run Tests

```bash
dotnet test
```

Run tests using Release configuration:

```bash
dotnet test --configuration Release
```

---

# 🚀 Getting Started

## Prerequisites

Before running the application, make sure you have:

- .NET 8 SDK.
- Docker Desktop.
- Git.
- Visual Studio or Visual Studio Code.

For manual execution without Docker, you also need:

- SQL Server.
- Redis.

## 1. Clone the Repository

```bash
git clone https://github.com/your-username/inventory-management-system.git

cd inventory-management-system
```

Replace the repository URL with your actual GitHub repository.

## 2. Run with Docker Compose

Start the API, SQL Server, and Redis services:

```bash
docker compose up -d
```

## 3. Access Swagger UI

Open:

```text
http://localhost:8080/swagger
```

The actual URL depends on the configured Docker port mapping.

## 4. Manual Configuration

If you want to run the API without Docker, configure the connection strings in:

`API/appsettings.Development.json`

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=InventoryDb;Trusted_Connection=True;TrustServerCertificate=True;",
    "RedisCacheUrl": "localhost:6379"
  }
}
```

Use the appropriate authentication settings for your SQL Server environment.

## 5. Apply Database Migrations

Run:

```bash
dotnet ef database update --project Infrastructure --startup-project API
```

Make sure EF Core tools are installed and the connection string is configured.

## 6. Run the API

```bash
dotnet run --project API
```

Use the listening URL printed in the console to access the application.

---

# ⚙️ Configuration

The application requires configuration for external services.

## SQL Server

Configure the `DefaultConnection` connection string.

## Redis

Configure:

`RedisCacheUrl`

Example:

```text
localhost:6379
```

## JWT Settings

Configure the JWT values required by the application:

- Signing Key.
- Issuer.
- Audience.
- Token Expiration.

Use secure secrets and avoid committing credentials or JWT signing keys to source control.

---

# 📈 Future Improvements

The following are potential enhancements for future versions and are not claimed as currently implemented.

## Inventory Improvements

- Low-stock alerts.
- Inventory forecasting.
- Barcode and QR Code integration.
- Stock transfer between warehouse locations.
- Inventory adjustment approval workflows.

## Performance and Reliability

- Advanced concurrency handling for simultaneous stock operations.
- Additional integration testing with SQL Server and Redis.
- More advanced cache monitoring.
- Background processing for long-running tasks.

## Security Improvements

- More granular permission management.
- Additional security auditing.
- Improved deployment secret management.

## DevOps Improvements

- Activate automated staging deployment.
- Activate automated production deployment.
- Add deployment rollback procedures.
- Add application monitoring and centralized logging.

## API Improvements

- Standardized pagination and filtering.
- API versioning.
- Consistent error response contracts.
- Expanded integration testing.

---

# 🔒 Security Notes

For production deployments:

- Keep JWT signing keys secure.
- Never commit database credentials.
- Use HTTPS.
- Review authorization policies for every endpoint.
- Protect the anonymous identity setup endpoint.
- Keep dependencies updated.
- Review Docker image vulnerabilities.
- Protect GitHub Actions secrets.
- Review concurrent inventory operations.

---

# 📄 License

No license information was supplied for this project.

Add an appropriate `LICENSE` file before publishing the repository publicly.

---

# 👨‍💻 Author

**Your Name**

- GitHub: [Your GitHub Profile](https://github.com/your-username)
- LinkedIn: [Your LinkedIn Profile](https://www.linkedin.com/in/your-profile/)

Replace these placeholders with your actual profile links.

---

<p align="center">
  <strong>Enterprise Inventory & Stock Management System</strong>
  <br/>
  Built with ASP.NET Core 8 and modern backend engineering practices.
</p>
