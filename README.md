# School Management System

A modern, enterprise-grade School Management System designed with **Clean Architecture**, **Domain-Driven Design (DDD)**, and **Command Query Responsibility Segregation (CQRS)** using **.NET 10** on the backend and a **React + TypeScript + Vite** Single Page Application (SPA) on the frontend.

---

## 🏛️ Architectural Print & Design Patterns

The backend is built following rigorous software engineering design patterns to ensure maximum testability, maintainability, low coupling, and scalability.

```
                  ┌─────────────────────────────────────────┐
                  │          Presentation (API)             │
                  │   ASP.NET Core Web API Controllers      │
                  └────────────┬───────────────┬────────────┘
                               │               │
                               ▼               ▼
                  ┌─────────────────────────────────────────┐
                  │             Application                 │
                  │   Commands, DTOs, Mappers, Validators   │
                  └────────────┬───────────────┬────────────┘
                               │               │
                               ▼               ▼
                  ┌─────────────────────────────────────────┐
                  │               Domain                    │
                  │   Entities, Value Objects, Events       │
                  └─────────────────────────────────────────┘
                               ▲               ▲
                               │               │
                  ┌────────────┴───────────────┴────────────┐
                  │             Infrastructure              │
                  │      EF Core, Queries, Repositories     │
                  └─────────────────────────────────────────┘
```

### 1. Clean Architecture (Onion Architecture)
The codebase is structured into loosely coupled projects with dependency directions pointing inwards towards the core Domain:

*   **`SchoolManagement.Domain` (Core / Center)**: Contains the enterprise business logic, core entities, domain events, validation invariants, exceptions, base definitions, and repository contracts. It has **zero dependencies** on external frameworks, databases, or libraries.
*   **`SchoolManagement.Application` (Inner Layer)**: Contains the orchestration of the system's business rules and use cases. It defines the application services, DTOs, mappers (AutoMapper), request validators (FluentValidation), and command objects.
*   **`SchoolManagement.Infrastructure` (Outer Layer)**: Outlines the physical data layers. It contains Entity Framework Core implementation, DbContext, Migrations, EF configuration files, Concrete Repositories, and optimized Query Services.
*   **`SchoolManagement.CrossCutting.Identity` (Outer Layer)**: Handles identity management, JWT generation, and token validations, kept decoupled from the business infrastructure.
*   **`SchoolManagement.Api` (Presentation)**: The entry point exposing RESTful API endpoints via ASP.NET Core controllers. Regulates security, logging (Serilog), pipeline middlewares, and Swagger/Scalar documentation.

---

### 2. Domain-Driven Design (DDD)
The domain layer encapsulates real-world concepts utilizing key DDD building blocks:

*   **Rich Domain Entities**: Entities guard their state through encapsulation. Properties are defined with `private set` accessors. State modification is only allowed through explicit, semantic domain methods (e.g., `UpdatePhone(string phone)`, `Register(...)`) containing business validations.
    *   *Example (`Student.cs`)*: Enforces that a student registrant must have either an `IntakeId` allocated or be set as a direct registration, but cannot satisfy both:
        ```csharp
        if (!intakeId.HasValue && !isDirectRegistration)
            throw new DomainException("Either IntakeId must be provided or IsDirectRegistration must be true.");
        ```
*   **Value Objects**: Implements immutable types defined solely by their property values.
    *   *Example (`Email`)*: Wraps string email properties to guarantee validity and normalize behavior across the system.
*   **Aggregate Roots**: Establishes consistency boundaries around sets of associated entities (e.g. `EnrollmentAggregate/Enrollment`).
*   **Domain Events**: Promotes loose coupling. When significant changes occur in the domain, entities emit domain events (e.g., `StudentCreatedDomainEvent`), which are dispatched using **MediatR** to trigger asynchronous event handlers (e.g., `SendWelcomeEmaiHandler` and `UpdateIntakeStatusHandler`).

---

### 3. Command Query Responsibility Segregation (CQRS)
To optimize performance and code clarity, the application segregates **State Mutation (Writes)** from **Data Retrieval (Reads)**:

| Attribute | Write Path (Commands) | Read Path (Queries) |
| :--- | :--- | :--- |
| **Goal** | Enforce domain constraints and mutate state | Project raw data to UI/DTOs as fast as possible |
| **Abstractions** | `IStudentService`, `StudentCommand` | `IStudentQueryService` |
| **Impl.** | `StudentRepository` (via EF Core with Change Tracking) | `StudentQueryService` (direct DTO projections) |
| **Operations** | `CreateAsync`, `UpdateAsync`, `DeleteAsync` | `GetAllResponsesAsync`, `GetResponseByIdAsync` |

*   **Write Path**: Handled by **Application Services** utilizing repositories. Entities are retrieved, mutated via encapsulated logic, and committed using the Repository and Unit of Work patterns.
*   **Read Path**: Circumvents the repository layer directly. **Query Services** query the database context without tracking (`AsNoTracking()`), projecting directly onto responses (e.g. `StudentResponseDto`), maximizing database read performance and bypassing rich domain models.

---

### 4. Advanced Modern Patterns Applied
*   **Automatic Dependency Injection (Scrutor Assembly Scanning)**: Rather than manually registering dozens of repository and service interfaces, the system scans assemblies automatically based on namespace signatures and maps matching interfaces dynamically:
    ```csharp
    builder.Services.Scan(scan => scan
        .FromAssemblyOf<Program>()
        .AddClasses(c => c.InNamespaces("SchoolManagement.Infrastructure.Repositories", "SchoolManagement.Application.Services", ...))
        .AsMatchingInterface()
        .WithScopedLifetime());
    ```
*   **Declarative Input Validation (FluentValidation)**: Pre-validates incoming requests before they trigger application logic via pipelines, separating request schema checks from domain rule checks.
*   **Guid-Based Unified Foreign Keys**: DTOs utilize `Guid` identifiers globally to unify ID formats, preventing runtime conversion errors and supporting horizontal scalability.

---

## 📂 Project Organization & Backend Tour

```
SchoolManagement/
├── Backend/
│   ├── SchoolManagement.Domain/                 # Pure Enterprise Business Rules
│   │   ├── DomainEvents/                        # Decoupled Domain notification triggers
│   │   ├── Entities/                            # Rich encapsulated Domain models & Aggregates
│   │   ├── ValueObjects/                        # Immutable structural primitives (e.g., Email)
│   │   └── Interfaces/                          # Abstract Repository & Query Service contracts
│   │
│   ├── SchoolManagement.Application/            # Orchestration layer
│   │   ├── Dtos/                                # Commands and Request/Response DTO contracts
│   │   ├── Services/                            # CQRS Command write services
│   │   ├── Mappers/                             # AutoMapper projection logic
│   │   └── Validators/                          # FluentValidation endpoint guard checks
│   │
│   ├── SchoolManagement.Infrastructure/         # Persistence layer
│   │   ├── Data/                                # DbContext, Migrations, Configuration blueprints
│   │   ├── Repositories/                        # Concrete Write-based Repositories
│   │   └── Queries/                             # Optimized Read-based Query Services
│   │
│   ├── SchoolManagement.CrossCutting.Identity/  # Security / JWT & Identity separation
│   │
│   └── SchoolManagement.Api/                    # Presentation Entry Point controllers & routing
│
└── Frontend/                                    # React SPA built on Vite, TS, and Tailwind CSS
```

---

## 💻 Frontend Architecture

The frontend is a reactive, client-side application engineered for clean visuals, speed, and premium user experience:

*   **Vite & TypeScript**: Standardized building environment ensuring quick compile times and static type checks.
*   **Tailwind CSS**: Utility-first CSS framework styled with modern dark components, micro-animations, color themes, and custom layout dashboards.
*   **Zod Schema Validation**: Operates validation logic directly on client forms (e.g., `studentSchema` in `formValidationSchemas.ts`) to intercept formatting issues before executing HTTP requests.
*   **Rich Dashboard Layout**: Structured dashboard panels for Admins, Teachers, Students, and Parents, mapping views dynamically using a component-based layout design.

---

## 🛠️ Getting Started

### Prerequisites
*   [.NET 10 SDK](https://dotnet.microsoft.com/download)
*   [Node.js (v18+) & npm](https://nodejs.org/)
*   SQL Server LocalDB / Express

### Run Backend
1. Navigate to the backend directory:
   ```bash
   cd Backend/SchoolManagement.Api
   ```
2. Configure your connection string inside `appsettings.json`.
3. Run the migrations and start the application:
   ```bash
   dotnet run
   ```
   *The database migrates and seeds mock data automatically on launch.*
4. Access API documentation at `http://localhost:<port>/swagger` (or using Scalar dashboard configuration).

### Run Frontend
1. Navigate to the frontend directory:
   ```bash
   cd Frontend
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Run the development server:
   ```bash
   npm run dev
   ```
4. Open the displayed local address inside your browser.
