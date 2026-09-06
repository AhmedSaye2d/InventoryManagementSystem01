# Inventory Management System

A backend RESTful API for managing products, categories, locations, orders, and user accounts built with ASP.NET Core 8.0 using Clean Architecture and CQRS patterns.

## Tech Stack

| Category | Technology |
|---|---|
| Runtime | .NET 8.0 |
| Web Framework | ASP.NET Core Web API |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Auth | ASP.NET Core Identity + JWT Bearer |
| Mediator | MediatR |
| Object Mapping | Mapster |
| Validation | FluentValidation |
| API Docs | Swashbuckle (Swagger) |
| Testing | NUnit + Moq |

## Features

- **Product Management** - Full CRUD with category & location relations
- **Category Management** - Full CRUD with product association
- **Location Management** - Full CRUD
- **Order Management** - Create, update, cancel orders with stock validation & reporting
- **Authentication & Authorization** - JWT-based with role & claim policies (Admin, Manager, User)
- **Activity Tracking** - Logs user operations
- **Request Logging** - Middleware for HTTP request/response logging
- **Exception Handling** - Global middleware for structured error responses
- **Input Validation** - FluentValidation pipeline via MediatR

## Project Structure

```
├── Domain/                  # Entities & domain models
├── Application/             # Business logic, DTOs, interfaces, CQRS commands & queries
├── Infrastructure/          # EF Core, repositories, middleware, DI registration
├── API/                     # Web API host, controllers, Swagger
├── Testing/                 # Unit tests
└── Host/                    # Alternate host (unused)
```

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB, Express, or full instance)

### Setup

1. **Clone the repository**

   ```bash
   git clone https://github.com/your-username/Inventory-Management-System.git
   ```

2. **Configure the database connection string**

   The connection string is not committed. Set it via User Secrets:

   ```bash
   cd "API"
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.;Database=InventoryDb;Trusted_Connection=True;TrustServerCertificate=True"
   ```

   Or add it to `API/appsettings.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.;Database=InventoryDb;Trusted_Connection=True;TrustServerCertificate=True"
     }
   }
   ```

3. **Restore packages**

   ```bash
   dotnet restore
   ```

4. **Apply database migrations**

   ```bash
   dotnet ef database update --project Infrastructure --startup-project API
   ```

5. **Run the API**

   ```bash
   dotnet run --project API
   ```

6. **Access Swagger UI**

   Navigate to `https://localhost:{port}/swagger`

### Seed Identity Data

After running the API, call the setup endpoint to create roles and the default admin user:

```
POST /api/account/identity/setup
```

**Default Admin Credentials:**

| Field | Value |
|---|---|
| Email | `admin@system.com` |
| Password | `Admin@123456` |

### Authentication

1. Login via `POST /api/account/login` to get a JWT token
2. Use the token in Swagger via the **Authorize** button, or in requests as:
   ```
   Authorization: Bearer <your-token>
   ```

## API Endpoints

### Account

| Method | Endpoint | Description | Auth |
|---|---|---|---|
| POST | `/api/account/login` | Login & get JWT token | Public |
| POST | `/api/account/identity/setup` | Seed roles & admin user | Public |
| POST | `/api/account/create-user` | Create a new user | Admin/Manager |
| PUT | `/api/account/update-user-claim` | Update user claims/role | Admin/Manager |
| GET | `/api/account/get-users` | Get all users with claims | Admin/Manager |

### Products

| Method | Endpoint | Description | Auth |
|---|---|---|---|
| POST | `/api/product` | Create product | Admin/Manager |
| GET | `/api/product` | Get all products | Authenticated |
| GET | `/api/product/{id}` | Get product by ID | Authenticated |
| PUT | `/api/product` | Update product | Admin/Manager |
| DELETE | `/api/product/{id}` | Delete product | Admin |

### Categories

| Method | Endpoint | Description | Auth |
|---|---|---|---|
| POST | `/api/category` | Create category | Admin/Manager |
| GET | `/api/category` | Get all categories | Authenticated |
| GET | `/api/category/{id}` | Get category by ID | Authenticated |
| GET | `/api/category/with-products/{id}` | Get category with products | Authenticated |
| PUT | `/api/category` | Update category | Admin/Manager |
| DELETE | `/api/category/{id}` | Delete category | Admin |

### Locations

| Method | Endpoint | Description | Auth |
|---|---|---|---|
| POST | `/api/location` | Create location | Admin/Manager |
| GET | `/api/location` | Get all locations | Authenticated |
| GET | `/api/location/{id}` | Get location by ID | Authenticated |
| PUT | `/api/location` | Update location | Admin/Manager |
| DELETE | `/api/location/{id}` | Delete location | Admin |

### Orders

| Method | Endpoint | Description | Auth |
|---|---|---|---|
| POST | `/api/order` | Create order (stock deducted) | Authenticated |
| PUT | `/api/order` | Update order status | Admin/Manager |
| PUT | `/api/order/cancel` | Cancel order | Authenticated |
| GET | `/api/order` | Get all orders | Admin/Manager |
| GET | `/api/order/user/{userId}` | Get orders by user | Authenticated |
| GET | `/api/order/count` | Get order counts by status | Authenticated |
| GET | `/api/order/monthly-totals` | Get monthly order totals | Admin/Manager |
| GET | `/api/order/top-product` | Get most ordered product | Admin/Manager |

### Activity Tracker

| Method | Endpoint | Description | Auth |
|---|---|---|---|
| GET | `/api/activity` | Get all activity logs | Admin/Manager |

## Roles & Permissions

| Role | Permissions |
|---|---|
| **Admin** | Full access: CRUD all resources, manage users, view activity logs |
| **Manager** | CRUD products/categories/locations, manage orders, create users, view activity logs |
| **User** | View products/categories/locations, place orders, cancel own orders |

## Run Tests

```bash
dotnet test --project Testing
```

## License

This project is open source. See the repository for license details.
