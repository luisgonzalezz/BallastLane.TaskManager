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

GenAI was used as a pair-programming assistant to:

- Interpret the technical exercise requirements.
- Propose the Clean Architecture solution structure.
- Generate incremental implementation plans.
- Draft tests and implementation slices.
- Review warnings, documentation, and setup instructions.

All AI-generated suggestions were validated through manual review, automated tests, and alignment with the assignment constraints: no Entity Framework, no Dapper, no MediatR, manual ADO.NET data access, and clean separation between layers.
