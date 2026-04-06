# Garage Operations Management System

A web-based management platform for automotive repair garages built with **ASP.NET Core 8.0 MVC**. The system supports multiple roles — Admins, Employees, and Guests — each with a tailored experience for managing garages, vehicles, owners, and repair orders.

**Live Repository:** https://github.com/PlamenKuzev/GarageOperationsManagementSystem

---

## Table of Contents

- [Concept](#concept)
- [Features](#features)
- [Architecture](#architecture)
- [Entity Models](#entity-models)
- [Services](#services)
- [Areas & Controllers](#areas--controllers)
- [Validation](#validation)
- [Security](#security)
- [Seeding](#seeding)
- [Test Coverage](#test-coverage)
- [Setup Instructions](#setup-instructions)
- [Technology Stack](#technology-stack)

---

## Concept

The Garage Operations Management System allows garage businesses to manage their day-to-day operations. Administrators have full control over all data. Employees can manage repair orders, cars and owners within their assigned garage, with trusted employees having elevated edit/delete privileges. Registered customers (Guests) can be linked to an Owner record to view their full repair history.

Anyone — even unauthenticated users — can look up the status of a repair order using its issue code, and view all garage locations on an interactive map.

---

## Features

### Public
- **Repair status tracker** — look up any repair order by issue code
- **Garage map** — interactive Leaflet.js map showing all garage locations
- **User registration & login** — with full name, email and password

### Admin
- Full CRUD for Garages, Cars, Owners, Employees and Repair Orders
- Search and pagination across all major lists
- Interactive map picker for setting garage coordinates
- Link Owner records to Guest accounts so customers can view their repair history
- Load demo garage data (30 randomised garages)
- View demo issue codes on the repair tracker page

### Employee
- View and manage repair orders, cars and owners within their assigned garage
- **Trusted employees** — can edit and delete across all records; can link owners to accounts
- **Untrusted employees** — can create and view only; no edit or delete access
- Inline garage editor on the dashboard (trusted only)

### Guest (registered customer)
- **My Repairs** page — view all repair orders for their linked cars, grouped by vehicle

### Account management
- Update full name and phone number
- Change password
- Two-factor authentication via authenticator app (Google/Microsoft Authenticator)
- Dark mode toggle with localStorage persistence

---

## Architecture

The project follows a **layered MVC architecture**:

```
├── Areas/
│   ├── Admin/          — Admin-only controllers and views
│   ├── Employee/       — Employee controllers and views
│   ├── Identity/       — Scaffolded Identity pages (Login, Register, Manage)
│   └── Public/         — Unauthenticated public pages
├── Controllers/        — Shared controllers (Home, Garages MapView, Profile)
├── Data/               — EF Core DbContext and migrations
├── Helpers/            — PaginatedList<T>
├── Interfaces/         — Service contracts
├── Models/             — Entity models
├── Seed/               — Database and role seeders
├── Services/           — Service implementations
└── Views/              — Shared views, layouts, partials
```

### Design Decisions

- **Service layer** — all business logic is encapsulated in services injected via interfaces, keeping controllers thin
- **Areas** — the application is divided into four areas to enforce clear separation of concerns and access control
- **IsTrusted on Employee** — a boolean flag provides tiered access within the Employee role without needing a separate role
- **Owner ↔ ApplicationUser link** — a nullable FK on `Owner` allows Admins/trusted Employees to associate a Guest account with an owner record, granting them a personalised repair history view
- **PaginatedList\<T\>** — supports both `IQueryable<T>` (EF async) and `IEnumerable<T>` (in-memory) sources via two factory methods

---

## Entity Models

| Model | Key Fields |
|---|---|
| `ApplicationUser` | Extends `IdentityUser`; adds `FullName` |
| `Garage` | City, Address, Capacity, WorkSchedule, Latitude, Longitude |
| `Employee` | Name, Position, Salary, WorkingSince, IsTrusted, GarageId, ApplicationUserId |
| `Car` | Brand, Model, Year, Mileage, LicensePlate, OwnerId |
| `Owner` | FullName, Phone, Email, ApplicationUserId |
| `RepairOrder` | IssueCode, Description, ArrivalDate, CompletionDate, IsCompleted, RepairPrice, GarageId, CarId |

---

## Services

| Interface | Responsibility |
|---|---|
| `IGarageService` | CRUD for garages, bulk create, queryable access |
| `ICarService` | CRUD for cars |
| `IOwnerService` | CRUD for owners, lookup by user ID, cars with repair orders |
| `IEmployeeService` | CRUD for employees, lookup by user ID, trusted check |
| `IRepairOrderService` | CRUD for repair orders, lookup by issue code, complete order |

All services are registered as **scoped** dependencies in `Program.cs` and resolved via constructor injection.

---

## Areas & Controllers

### Admin Area — `[Authorize(Roles = "Admin")]`
| Controller | Actions |
|---|---|
| HomeController | Dashboard with welcome heading |
| GaragesController | Index (search/paginate), Create, Edit, Delete, Details, LoadDemoData |
| CarsController | Index, Create, Edit, Delete, Details |
| OwnersController | Index, Create, Edit, Delete, Details, LinkUser, UnlinkUser |
| EmployeesController | Index (search/paginate), Create, Edit, Delete |
| RepairOrdersController | Index (search/paginate), Create, Edit, Delete, Details, Complete |

### Employee Area — `[Authorize(Roles = "Admin,Employee")]`
| Controller | Actions |
|---|---|
| HomeController | Dashboard; `EditGarage` POST (trusted only) |
| CarsController | Index, Create, Edit (trusted), Delete (trusted), Details |
| OwnersController | Index, Create, Edit (trusted), Delete (trusted), Details, LinkUser/UnlinkUser (trusted) |
| RepairOrdersController | Index (search/paginate), Create, Edit, Delete, Details, Complete |

### Public Area — `[AllowAnonymous]`
| Controller | Actions |
|---|---|
| RepairStatusController | GET/POST index — 5-second delay on not-found results |

### Main Controllers
| Controller | Actions |
|---|---|
| HomeController | Index (redirects Admin→Admin dashboard, Employee→Employee dashboard) |
| GaragesController | MapView (public) |
| ProfileController | Index — shows repair history for linked owner |

---

## Validation

### Server-side
- All entity models use Data Annotations (`[Required]`, `[StringLength]`, `[EmailAddress]`, etc.)
- `ModelState.IsValid` checked on every POST action before any persistence
- Guards against invalid IDs (returns `NotFound()`)
- Business rules enforced in controllers (e.g. cannot delete a garage with active employees or repair orders)
- Security checks: untrusted employees are blocked from edit/delete actions server-side

### Client-side
- `_ValidationScriptsPartial` wired into all Create and Edit forms
- Unobtrusive jQuery validation active on all form inputs

---

## Security

| Concern | Implementation |
|---|---|
| CSRF | `[ValidateAntiForgeryToken]` on all 32 POST actions; token auto-injected in tag-helper forms |
| XSS | Razor auto-encodes all model output; no raw HTML rendered from user input |
| SQL Injection | All queries use EF Core parameterised LINQ — no raw SQL |
| Authentication | ASP.NET Core Identity with hashed passwords (PBKDF2) |
| Authorisation | Role-based `[Authorize]` on all areas; `IsTrusted` check enforced server-side |
| Brute force | 5-second server-side delay on failed login and invalid repair code lookup |
| Two-factor auth | TOTP authenticator app support (Google/Microsoft Authenticator) |
| Parameter tampering | Server re-fetches entities by ID from DB; never trusts client-supplied hidden fields for ownership |

---

## Seeding

The application seeds the following data on startup:

### Roles
`Admin`, `Employee`, `Guest`

### Demo Accounts
| Email | Password | Role | Notes |
|---|---|---|---|
| admin@garage.com | Admin123! | Admin | Full system access |
| employee@garage.com | Employee123! | Employee | Untrusted; linked to Demo Employee record |
| trustedEmployee@garage.com | TrustedEmployee123! | Employee | Trusted; full edit/delete access |
| owner@garage.com | Owner123! | Guest | Linked to Demo Owner; can view repair history |

### Demo Data (`DbSeeder`)
- 1 demo garage (Sofia)
- 1 demo owner linked to guest account
- 1 demo car
- 3 demo repair orders with issue codes `DEMO0001`, `DEMO0002`, `DEMO0003`

---

## Test Coverage

**Project:** `GarageOperationsManagementSystem.Tests`
**Framework:** xUnit with EF Core InMemory provider

| Test File | Tests | Service Covered |
|---|---|---|
| GarageServiceTests.cs | 11 | IGarageService |
| CarServiceTests.cs | 11 | ICarService |
| EmployeeServiceTests.cs | 16 | IEmployeeService |
| OwnerServiceTests.cs | 17 | IOwnerService |
| RepairOrderServiceTests.cs | 16 | IRepairOrderService |
| **Total** | **71** | All 5 services |

Coverage targets all CRUD operations, edge cases (not found, duplicates), and business logic methods. Exceeds the required **65% service layer coverage**.

---

## Setup Instructions

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express or higher)
- Visual Studio 2022 or JetBrains Rider

### Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/PlamenKuzev/GarageOperationsManagementSystem.git
   cd GarageOperationsManagementSystem
   ```

2. **Configure the connection string**
   Open `GarageOperationsManagementSystem/appsettings.json` and update `DefaultConnection` to match your SQL Server instance:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=GarageOperationsManagementSystem;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
   }
   ```

3. **Apply migrations**
   ```bash
   cd GarageOperationsManagementSystem
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```
   Or press **F5** in Visual Studio / Rider.

5. **The database is seeded automatically on first run.**
   Log in with any of the demo accounts listed in the [Seeding](#seeding) section.

### Running Tests
```bash
cd GarageOperationsManagementSystem.Tests
dotnet test
```

---

## AI Disclosure

1. The map was generated using Cursor to explore how AI-assisted development works in practice.
2. Claude was used selectively to evaluate code generation approaches and improve personal coding practices, primarily for repetitive or time-consuming tasks.

---

## Technology Stack

| Technology | Purpose |
|---|---|
| ASP.NET Core 8.0 MVC | Web framework |
| Entity Framework Core 8 | ORM and database access |
| Microsoft SQL Server | Relational database |
| ASP.NET Core Identity | Authentication and authorisation |
| Bootstrap 5 | Responsive UI framework |
| Bootstrap Icons 1.11 | Icon library |
| Leaflet.js 1.9.4 | Interactive maps |
| qrcode.js | 2FA QR code generation |
| xUnit | Unit testing framework |
| EF Core InMemory | In-memory database for tests |
