# BallastLane.TaskManager

Technical interview project for a .NET 8 task management application.

## Stack

- .NET 8
- ASP.NET Core Web API
- Angular
- SQL Server LocalDB
- Clean Architecture
- JWT authentication
- Manual ADO.NET data access with `Microsoft.Data.SqlClient`
- xUnit unit and integration tests

## Architecture

The backend is split into Domain, Application, Infrastructure, and API projects. Domain contains business concepts, Application defines use cases and abstractions, Infrastructure implements SQL Server access and security services, and API exposes HTTP endpoints.

The frontend lives in `src/BallastLane.TaskManager.Web` and consumes the API through Angular services, route guards, and an HTTP interceptor for JWT tokens.

## User Story

As a registered user, I want to securely sign in and manage my personal tasks, so that I can track pending, in-progress, and completed work from a web application.

Acceptance criteria:

- A user can register and log in.
- Authenticated users can create, read, update, and delete only their own tasks.
- Each task has a title, description, status, and due date.
- Anonymous users cannot access task endpoints or the task workspace.

## Current Status

The project currently includes:

- User registration and login with JWT authentication.
- Authenticated task CRUD API endpoints.
- Manual ADO.NET repositories for users and tasks.
- Angular login, registration, route guard, JWT interceptor, and task CRUD screen.
- SQL Server LocalDB schema, seed, and reset scripts.
- Unit and integration tests across Domain, Application, Infrastructure, API, and Angular.

## Local Database

The project is configured for SQL Server LocalDB during development:

```text
Server=(localdb)\MSSQLLocalDB;Database=BallastLaneTaskManager;Trusted_Connection=True;TrustServerCertificate=True;
```

Run the database scripts from the `database` folder using SQLCMD mode:

```powershell
cd database
sqlcmd -S "(localdb)\MSSQLLocalDB" -i reset.sql
```

The reset script recreates the `BallastLaneTaskManager` schema and loads demo data.

## Demo Credentials

After running `database/reset.sql`, use:

```text
Email: demo@ballastlane.com
Password: Password123!
```

You can also create a new account from the Angular registration page.

## Run The Application

From the repository root, start the API:

```powershell
dotnet run --project src/BallastLane.TaskManager.Api --launch-profile https
```

The API runs at:

```text
https://localhost:7126
```

Swagger is available at:

```text
https://localhost:7126/swagger
```

In another terminal, start Angular:

```powershell
cd src/BallastLane.TaskManager.Web
npm install
npm start
```

The frontend runs at:

```text
http://localhost:4200
```

The Angular app is configured to call:

```text
https://localhost:7126/api
```

## Tests

Run backend tests from the repository root:

```powershell
dotnet test BallastLane.TaskManager.sln
```

Run frontend tests:

```powershell
cd src/BallastLane.TaskManager.Web
npm test
```

Build the Angular app:

```powershell
npm run build
```

## Presentation Notes

The development sequence is documented in `docs/development-walkthrough.md`.

## GenAI Usage

GenAI was used as a pair-programming assistant during the exercise. The tool was used to reason through requirements, propose an incremental Clean Architecture implementation plan, draft test-first implementation slices, review warnings, and prepare documentation.

### Prompt Used

The main prompt used for the API scaffold was:

```text
Build a .NET 8 ASP.NET Core Web API for a task management system using Clean Architecture.
The system must support user registration, login with JWT, and authenticated CRUD operations for tasks.
Each task must include title, description, status, due date, and user ownership.
Use SQL Server with manual ADO.NET repositories.
Do not use Entity Framework, Dapper, MediatR, or other data-access abstractions.
Write unit and integration tests and keep the architecture separated into Domain, Application, Infrastructure, and API layers.
```

### Representative Output Sample

The AI-assisted scaffold proposed the following kind of structure, which was then reviewed and implemented incrementally:

```text
Domain
  User
  TaskItem
  TaskItemStatus

Application
  AuthService
  TaskItemService
  IUserRepository
  ITaskItemRepository
  IPasswordHasher
  IJwtTokenGenerator

Infrastructure
  UserRepository
  TaskItemRepository
  PasswordHasher
  JwtTokenGenerator

API
  AuthController
  TasksController
```

### Validation And Corrections

AI-generated suggestions were not accepted blindly. They were validated and corrected through:

- Manual review of Clean Architecture dependency direction.
- Test-driven development cycles for domain, application, infrastructure, API, and Angular behavior.
- `dotnet test BallastLane.TaskManager.sln` for backend validation.
- `npm test` and `npm run build` for frontend validation.
- Manual end-to-end testing with LocalDB, the API, and Angular.
- Package and code searches to confirm no Entity Framework, Dapper, or MediatR references were introduced.

Notable corrections and improvements:

- Replaced any ORM-style persistence ideas with manual `SqlConnection`, `SqlCommand`, and `SqlDataReader` mapping.
- Added user-scoped task queries so authenticated users can only access their own tasks.
- Added domain and application validation for required fields, invalid credentials, ownership, and not-found cases.
- Added JWT route protection in both the API and Angular.
- Added seeded demo credentials with a real PBKDF2 password hash.

Edge cases considered include duplicate registration emails, invalid login credentials, blank task titles, missing task records, user ownership boundaries, unauthorized API requests, and CORS behavior for the Angular client.
