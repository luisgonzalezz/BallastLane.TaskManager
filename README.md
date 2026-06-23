# BallastLane.TaskManager

Technical interview project for a .NET 8 task management application.

## Stack

- .NET 8
- ASP.NET Core Web API
- Angular
- SQL Server
- Clean Architecture
- JWT authentication
- Manual ADO.NET data access with `Microsoft.Data.SqlClient`
- xUnit unit and integration tests

## Architecture

The backend is split into Domain, Application, Infrastructure, and API projects. Domain contains business concepts, Application defines use cases and abstractions, Infrastructure implements SQL Server access and security services, and API exposes HTTP endpoints.

The frontend lives in `src/BallastLane.TaskManager.Web` and will consume the API through Angular services, route guards, and an HTTP interceptor for JWT tokens.

## Current Status

This repository currently contains the Clean Architecture scaffold, Domain layer, Application layer, Infrastructure foundations, SQL Server LocalDB scripts, and manual ADO.NET repositories. API endpoints and Angular screens will be added incrementally.

## Local Database

The project is configured for SQL Server LocalDB during development:

```text
Server=(localdb)\MSSQLLocalDB;Database=BallastLaneTaskManager;Trusted_Connection=True;TrustServerCertificate=True;
```

Run the database scripts from the `database` folder using SQLCMD mode:

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -i reset.sql
```

## Presentation Notes

The development sequence is documented in `docs/development-walkthrough.md`.

## GenAI Usage

GenAI will be used as a development assistant to scaffold and review implementation ideas. AI-generated suggestions will be validated through manual review, tests, and alignment with the assignment constraints: no Entity Framework, no Dapper, no MediatR, and clean separation between layers.
