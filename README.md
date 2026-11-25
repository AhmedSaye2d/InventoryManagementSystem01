# 📦 Inventory Management System

A backend Inventory Management System built with **.NET 8** following **Clean Architecture**, **CQRS**, **MediatR**, and modern backend development practices.

> 🚧 **Project Status: Under Development**
>
> The system is actively being developed, and new features and improvements are being added continuously.

---

## 🚀 Overview

The Inventory Management System is designed to manage products, stock operations, inventory movements, and notifications while maintaining a clean and scalable architecture.

The main goal of the project is to build a realistic backend system that follows practices commonly used in professional .NET applications.

---

## 🛠️ Technologies & Architecture

* **.NET 8**
* **ASP.NET Core Web API**
* **C#**
* **Entity Framework Core**
* **SQL Server**
* **Clean Architecture**
* **CQRS**
* **MediatR**
* **Repository Pattern**
* **Unit of Work Pattern**
* **SOLID Principles**
* **FluentValidation**
* **JWT Authentication**
* **Role-Based Authorization**
* **ASP.NET Core Identity**
* **Serilog**
* **Swagger / OpenAPI**
* **Unit Testing**

---

## 🏗️ Architecture

The project follows **Clean Architecture** with separation of concerns between the main layers:

```text
InventoryManagementSystem
│
├── API
│   ├── Controllers
│   ├── Middleware
│   └── Configuration
│
├── Application
│   ├── Commands
│   ├── DTOs
│   ├── Validators
│   └── Services
│
├── Domain
│   ├── Entities
│   ├── Enums
│   └── Interfaces
│
├── Infrastructure
│   ├── DataAccess
│   ├── Repositories
│   ├── UnitOfWork
│   ├── Services
│   └── Handlers
│
└── Testing
```

### Request Flow

```text
HTTP Request
     ↓
Controller
     ↓
MediatR Command
     ↓
Validation
     ↓
Command Handler
     ↓
Repository / Unit of Work
     ↓
Entity Framework Core
     ↓
SQL Server
```

---

## 📋 Current Features

### Product Management

* Create Product
* Get Products
* Get Product by ID
* Update Product
* Delete Product
* Product Categories
* Product Locations

### 📦 Inventory Operations

#### Stock-In

Add stock to a product and record the inventory movement.

#### Stock-Out

Remove stock from a product while validating available quantity.

#### Stock Adjustment

Increase or decrease stock manually while preventing negative inventory quantities.

### 📊 Inventory Movements

Every stock operation is recorded in `InventoryMovement`.

The movement keeps information such as:

* Product
* Movement Type
* Quantity
* Quantity After
* Location
* Reference
* Date

This provides an inventory history/audit trail for stock changes.

### 🔔 Low Stock Alerts

The system monitors product stock levels using `LowStockThreshold`.

When:

```text
Product.Quantity <= Product.LowStockThreshold
```

a low-stock notification can be generated for users with:

* `Admin`
* `Manager`

The system also prevents duplicate unresolved low-stock notifications.

### 🔐 Authentication & Authorization

The API uses:

* JWT Authentication
* ASP.NET Core Identity
* Role-Based Authorization

Different operations can be restricted based on the user's role.

### 🔔 Notifications

Users can retrieve their notifications through the notification API.

Notifications currently include low-stock alerts.

---

## 🧪 Testing

The project includes automated tests for important business scenarios such as:

* Product operations
* Stock-In
* Stock-Out
* Stock Adjustment
* Insufficient stock
* Product not found
* Low-stock notifications
* Notification duplicate prevention
* Authorization scenarios

---

## 🔄 Features in Progress

The project is still under active development.

Planned/improving features include:

* [ ] Stock Transfer
* [ ] Supplier Management
* [ ] Purchase Orders
* [ ] Inventory Reports
* [ ] Advanced Audit Logging
* [ ] Notification Read / Unread
* [ ] Additional validation and business rules
* [ ] Additional automated tests
* [ ] Performance improvements

> The feature list may change as the project evolves.

---

## 🎯 Project Goals

This project is being developed to practice and demonstrate:

* Building scalable **.NET Web APIs**
* Applying **Clean Architecture**
* Implementing **CQRS and MediatR**
* Applying **Repository and Unit of Work**
* Designing real-world inventory business logic
* Implementing authentication and authorization
* Writing maintainable and testable code
* Following SOLID principles and separation of concerns

---

## 📌 API Documentation

The API can be explored through **Swagger / OpenAPI** when running the application locally.

Main API areas currently include:

```text
/api/Product
/api/Inventory
/api/Notification
/api/Auth
```

---

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone <repository-url>
```

### 2. Configure the database

Update the connection string in:

```text
appsettings.json
```

### 3. Apply migrations

```bash
dotnet ef database update
```

### 4. Run the application

```bash
dotnet run
```

### 5. Open Swagger

After running the API, open the Swagger UI to explore the available endpoints.

---

## 👨‍💻 Development Status

This project is **actively under development**.

New inventory features, business rules, tests, and architectural improvements are being added incrementally.

The README will be updated as the system evolves.

---

## 📄 License

This project is currently developed for learning, portfolio, and demonstration purposes.
