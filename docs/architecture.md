# Architecture Notes

## Backend

- `BallastLane.TaskManager.Domain`: entities, enums, and domain exceptions.
- `BallastLane.TaskManager.Application`: DTOs, service contracts, use cases, and validation rules.
- `BallastLane.TaskManager.Infrastructure`: manual ADO.NET repositories, SQL connection factory, password hashing, JWT token service, and dependency injection.
- `BallastLane.TaskManager.Api`: controllers, request/response contracts, middleware, authentication configuration, and API composition root.

## Frontend

- `BallastLane.TaskManager.Web`: Angular application with routes for authentication and task CRUD workflows.

## Dependency Rule

Dependencies point inward:

`Api -> Application -> Domain`

`Infrastructure -> Application + Domain`

Domain does not depend on any other project.
