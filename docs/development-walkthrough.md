# Development Walkthrough

This document summarizes the development process for the BallastLane.TaskManager technical interview project. It is intended as presentation support and as a record of the main engineering decisions.

## 1. Requirements Review

The exercise asks for a full-stack task management application using:

- .NET 8 and ASP.NET Core Web API
- Angular frontend
- SQL Server
- Clean Architecture
- JWT authentication
- Manual ADO.NET data access
- No Entity Framework, no Dapper, and no MediatR
- Unit and integration tests
- README documentation, including GenAI usage

The selected use case is a task management system where users register, log in, and manage their own tasks.

## 2. Initial Solution Scaffold

The solution was created with separate projects for each architectural layer:

- `BallastLane.TaskManager.Domain`
- `BallastLane.TaskManager.Application`
- `BallastLane.TaskManager.Infrastructure`
- `BallastLane.TaskManager.Api`
- `BallastLane.TaskManager.Web`

The test projects mirror the backend layers:

- `BallastLane.TaskManager.Domain.Tests`
- `BallastLane.TaskManager.Application.Tests`
- `BallastLane.TaskManager.Infrastructure.Tests`
- `BallastLane.TaskManager.Api.IntegrationTests`

The dependency direction follows Clean Architecture:

`Api -> Application -> Domain`

`Infrastructure -> Application + Domain`

The Domain project does not depend on any other project.

## 3. Domain Layer First

Development started with the Domain layer because it contains the core business concepts and rules.

Implemented domain objects:

- `User`
- `TaskItem`
- `TaskItemStatus`
- `DomainValidationException`

Key business rules covered by tests:

- A user requires a valid email.
- A user requires a password hash.
- A task requires a title.
- A task must belong to a user.
- A task requires a due date.
- A task starts with `Pending` status.
- A task can move to `InProgress`.
- A task can move to `Completed`.
- Null task descriptions are normalized to an empty string.

The Domain tests were written first, then executed to confirm failure, and finally the minimum domain implementation was added.

## 4. Application Layer

The Application layer defines use cases and contracts without depending on SQL Server, JWT libraries, or ASP.NET Core controllers.

Implemented abstractions:

- `IUserRepository`
- `ITaskItemRepository`
- `IPasswordHasher`
- `IJwtTokenGenerator`

Implemented DTOs:

- `RegisterUserRequest`
- `LoginRequest`
- `AuthResponse`
- `CreateTaskItemRequest`
- `TaskItemResponse`

Implemented services:

- `AuthService`
- `TaskItemService`

Application behavior covered by tests:

- Registering a new user hashes the password, stores the user, and returns a token.
- Registering with an existing email is rejected.
- Registering with a blank password is rejected.
- Logging in with valid credentials returns a token.
- Logging in with invalid credentials is rejected.
- Creating a task stores it for the authenticated user.
- Listing tasks returns only the requested user's tasks.

The Application tests use in-memory fakes so the layer stays independent from Infrastructure.

## 5. TDD Evidence

The development process followed red-green cycles:

1. Write tests describing the expected behavior.
2. Run the tests and confirm they fail because the feature does not exist yet.
3. Implement the smallest amount of code needed to pass.
4. Run the tests again and confirm they pass.
5. Clean up names, contracts, and warnings while keeping tests green.

Example commands used during development:

```powershell
dotnet test tests/BallastLane.TaskManager.Domain.Tests/BallastLane.TaskManager.Domain.Tests.csproj
dotnet test tests/BallastLane.TaskManager.Application.Tests/BallastLane.TaskManager.Application.Tests.csproj
dotnet build BallastLane.TaskManager.sln
dotnet test BallastLane.TaskManager.sln --no-build
```

## 6. GenAI Usage

GenAI was used as a pair-programming assistant to:

- Interpret the interview requirements.
- Propose a Clean Architecture solution structure.
- Generate initial test ideas.
- Scaffold code incrementally.
- Review warnings and adjust contracts.
- Produce documentation for the presentation.

AI suggestions were validated through:

- Manual review of architecture boundaries.
- Running automated tests.
- Checking compiler warnings.
- Ensuring no prohibited libraries were introduced.
- Keeping SQL/data access planned for manual ADO.NET.

## 7. Infrastructure Foundations

The Infrastructure layer began with cross-cutting services and database scripts:

- `SqlConnectionFactory` reads `ConnectionStrings:DefaultConnection` and creates `SqlConnection` instances.
- `PasswordHasher` implements PBKDF2 password hashing with a random salt.
- `JwtTokenGenerator` creates signed JWTs with user id and email claims.
- `DependencyInjection` registers Infrastructure services behind Application abstractions.
- `database/schema.sql` creates the SQL Server database, `Users`, and `Tasks`.
- `database/seed.sql` adds demo records for presentation.
- `database/reset.sql` runs schema and seed scripts together through SQLCMD mode.

Infrastructure tests validate hashing, password verification, JWT claims, and connection string usage without requiring a live SQL Server connection yet.

## 8. Manual ADO.NET Repositories

The next Infrastructure slice added real SQL Server LocalDB integration:

- `User.FromPersistence(...)` and `TaskItem.FromPersistence(...)` rehydrate stored records without using creation factories meant for new records.
- `UserRepository` implements `IUserRepository` using `SqlConnection`, `SqlCommand`, and `SqlDataReader`.
- `TaskItemRepository` implements `ITaskItemRepository` with manual SQL commands and manual mapping.
- Repository tests create a temporary LocalDB database, create schema objects, run repository operations, and drop the database after the test fixture completes.
- Date parameters are sent as `SqlDbType.DateTime2` to match the schema and avoid SQL Server `datetime` rounding.

This keeps the project compliant with the assignment restrictions: no Entity Framework, no Dapper, and no MediatR.

## 9. Task CRUD Completion

The task use case was expanded from create/list to full CRUD:

- Domain now exposes `TaskItem.UpdateDetails(...)` so task changes continue to go through domain validation.
- Application now exposes task get-by-id, update, and delete use cases.
- `ApplicationNotFoundException` represents user-scoped missing records.
- `ITaskItemRepository` now supports get-by-id, update, and delete operations.
- `TaskItemRepository` implements the full CRUD flow with manual ADO.NET and parameterized SQL.
- LocalDB integration tests verify create, read, update, delete, and user scoping.

The service layer enforces ownership by always querying tasks with both task id and user id.

## 10. API Authentication Slice

The first API slice exposed authentication-related HTTP endpoints:

- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/auth/public`
- `GET /api/auth/private`

`Program.cs` now registers Application services, Infrastructure services, CORS for Angular, JWT bearer authentication, and authorization middleware.

The private endpoint uses `[Authorize]`, and integration tests verify:

- public endpoints can be reached without a token.
- private endpoints return `401 Unauthorized` without a token.
- private endpoints accept a valid JWT.
- register and login return authentication responses.

For this slice, the API integration tests replace `IAuthService` with a fake service while still generating and validating real JWTs through the same signing configuration. This keeps the tests focused on routing, controller behavior, and authentication middleware.

## 11. Task API Slice

The task CRUD use case is now exposed over HTTP:

- `GET /api/tasks`
- `GET /api/tasks/{id}`
- `POST /api/tasks`
- `PUT /api/tasks/{id}`
- `DELETE /api/tasks/{id}`

All task endpoints require JWT authentication. The controller reads the authenticated user id from the token claim and never accepts `UserId` from the request body.

Integration tests verify:

- task endpoints return `401 Unauthorized` without a token.
- authenticated users can list tasks.
- authenticated users can read one task.
- authenticated users can create, update, and delete tasks.

This completes the backend API surface required for task CRUD and authentication.

## 12. Next Planned Steps

The remaining implementation will continue in this order:

1. Connect API integration tests to real LocalDB services for end-to-end coverage.
2. Angular authentication flow and task CRUD screens.
3. Final README setup instructions and demo credentials.
