# School Management System

A comprehensive School Management System designed to streamline administrative tasks, manage student and teacher data, handle enrollments, schedules, and monitor academic performance.

## 💡 Project Idea

The School Management System aims to provide a centralized platform for educational institutions to manage their day-to-day operations. It facilitates the management of various entities including Students, Teachers, Parents, Classes, Subjects, and Exams. Key features include:
- **User Management:** Roles for Admin, Teacher, Student, and Parent.
- **Academic Management:** Handling Intakes, Enrollments, Groups, and Subjects.
- **Scheduling:** Managing lessons, exams, and event calendars.
- **Performance Tracking:** Monitoring attendance and academic results.
- **Communication:** Basic messaging and announcement system.

## 🛠️ Tech Stack

### Backend
- **Framework:** ASP.NET Core 10 (Web API)
- **Language:** C#
- **Database:** SQL Server (Development uses SQLite)
- **ORM:** Entity Framework Core
- **Validation:** [FluentValidation](https://fluentvalidation.net/)
- **Mapping:** [AutoMapper](https://automapper.org/)
- **Dependency Injection:** Built-in DI with [Scrutor](https://github.com/khellang/Scrutor) for assembly scanning
- **Logging:** [Serilog](https://serilog.net/)
- **API Documentation:** OpenAPI / Scalar

### Frontend
- **Framework:** React 18
- **Build Tool:** [Vite](https://vitejs.dev/)
- **Language:** TypeScript
- **Styling:** [Tailwind CSS](https://tailwindcss.com/)
- **Form Management:** [React Hook Form](https://react-hook-form.com/) with [Zod](https://zod.dev/)
- **Routing:** [React Router DOM](https://reactrouter.com/)
- **Charts:** [Recharts](https://recharts.org/)
- **Calendar:** [React Big Calendar](https://jquense.github.io/react-big-calendar/)
- **HTTP Client:** [Axios](https://axios-http.com/)

## 🏗️ Architecture & Patterns

### Backend Architecture
The backend follows a **Layered Architecture** with a focus on separation of concerns:
- **Controllers:** Handle HTTP requests and define API endpoints.
- **Services:** Contain business logic and orchestrate operations between repositories and other services.
- **Repositories:** Abstract data access logic using the **Repository Pattern**.
- **Models/Entities:** Represent the domain data and database schema.
- **DTOs (Data Transfer Objects):** Used to decouple the API contract from the internal domain models.

**Key Patterns:**
- **Dependency Injection:** Extensively used for decoupling and testability.
- **Factory Pattern:** Used for creating mock data and complex entities during seeding.
- **Seeding:** Automated database initialization with [Bogus](https://github.com/bchavez/Bogus).

### Frontend Architecture
The frontend is built with a **Component-Based Architecture**:
- **Layouts:** High-level components that define the structure of pages (e.g., `DashboardLayout`).
- **Containers vs. Presenters:** Separation between components that handle logic/data fetching and those that focus on rendering.
- **Custom Hooks:** Used to encapsulate and reuse logic across different components.
- **Modular Forms:** Reusable form components with centralized validation schemas.

## 🚀 Getting Started

### Prerequisites
- .NET 10 SDK
- Node.js (v18+)
- SQL Server (or use the provided SQLite configuration)

### Installation
1. **Clone the repository:**
   ```bash
   git clone <repository-url>
   ```
2. **Backend Setup:**
   ```bash
   cd Backend
   dotnet restore
   dotnet run
   ```
3. **Frontend Setup:**
   ```bash
   cd Frontend
   npm install
   npm run dev
   ```
