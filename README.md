# task-flow

A small, production-like backend built with .NET 9 for learning and demonstrating backend engineering skills.

This repository contains an API for managing projects, tasks and comments with real-world concerns implemented (auth, background jobs, auditing, pagination, etc.). It is maintained as a personal portfolio project — useful for interviews or technical review.

Tech stack
- .NET 9 (C# 13)
- PostgreSQL (EF Core)
- MediatR-style request handlers, Hangfire (background jobs), SignalR
- Optional: Redis, RabbitMQ, OpenTelemetry (ideas in README)

Key features
- CRUD for Projects, Tasks and Comments
- Authentication/Authorization with refresh tokens
- Pagination and filtering using `DataFilter`
- Background jobs, audit logging and email sending
- SignalR notifications for real-time updates

Architecture overview
- Clean separation: `Domain`, `Application`, `Persistance`, and API projects
- Uses CQRS-style handlers for commands/queries (MediatR pattern)
- EF Core for data access and migrations

Quick start
1. Copy `.env.example` (or `.env`) and update connection strings and secrets.
2. Run migrations from the Persistance project:
   - `dotnet ef database update --project Persistance --startup-project task-flow-api`
3. Run the API:
   - `dotnet run --project task-flow-api`

Useful commands
- Build: `dotnet build`
- Run tests: `dotnet test` (if present)

Next steps and ideas
- Add Redis caching and cache invalidation, full-text search (Postgres/Elastic), OpenTelemetry, transactional outbox for reliable messaging, and load-testing with seeded data.
- Add Redis caching and cache invalidation, full-text search (Postgres/Elastic), OpenTelemetry, transactional outbox for reliable messaging, and load-testing with seeded data.

