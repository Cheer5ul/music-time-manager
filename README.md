<div align="center">

# 🎵 Music Time Manager

**A workflow tool for small music production teams — not a generic task manager.**

Built to organize weekly production work: tasks, subtasks, multiple assignees, and a calendar that reflects reality without a single background job faking it.

[![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)](#)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Neon-4169E1?logo=postgresql&logoColor=white)](#)
[![EF Core](https://img.shields.io/badge/EF%20Core-Clean%20Architecture-black)](#)
[![React](https://img.shields.io/badge/React-TypeScript-61DAFB?logo=react&logoColor=black)](#)
[![Status](https://img.shields.io/badge/status-backend%20in%20progress-orange)](#)

</div>

---

## What this is

A private task manager for a 2–10 person music production team. Task is the primary entity (not Project — that's a deliberate v1 scope decision, not an oversight). Tasks can be split into subtasks, and both support multiple assignees via proper many-to-many relations.

The interesting part isn't the CRUD — it's the handful of decisions that came out of actually thinking through the domain before writing code:

- **"Overdue" is never stored, always computed.** No stored status, no background sweep job silently going stale between ticks — `isOverdue = dueDate < now && status != Done`, evaluated at request time.
- **Statistics aren't stored either.** Completed/missed counts are aggregate queries against existing tables — zero counter columns, zero desync risk.
- **Recreating a missed task doesn't mutate history.** A new `Task` row is created with a self-referencing FK (`RecreatedFromTaskId`) back to the original — the original stays in the archive untouched.
- **A task can exist with zero assignees** — briefly, by design. Creation and assignment are separate calls (`TaskAssignee` needs a real `TaskId` to reference), but the invariant "can't be emptied once it has one" is still enforced where it matters.

Every non-obvious decision like this is written down — see [`/docs`](./docs).

---

## Stack

| Layer | Tech |
|---|---|
| **Backend** | ASP.NET Core Web API · EF Core · PostgreSQL (Neon, Docker locally) |
| **Auth** | JWT in an `HttpOnly` cookie — not the `Authorization` header |
| **Architecture** | Clean Architecture: `API` → `Application` → `Core` → `Infrastructure` / `Persistence` |
| **Frontend** *(next up)* | React · TypeScript · Vite · Tailwind · Framer Motion · dnd-kit · TanStack Query |
| **Hosting** | Render (API + frontend) · Neon (Postgres) |
| **Background jobs** | In-process `IHostedService`, no external broker — not needed at this scale |

---

## Architecture

```
backend/
├── music-time-manager.API             → controllers, DTOs, HTTP concerns
├── music-time-manager.Application     → services, orchestration
├── music-time-manager.Core            → domain models, invariants, business rules
├── music-time-manager.Infrastructure  → JWT, password hashing, external concerns
└── music-time-manager.Persistence     → EF Core, entities, repositories, migrations
```

Dependencies point inward — `Core` knows nothing about EF Core, HTTP, or persistence. Domain entities (`Task`, `Subtask`) are deliberately separate from their EF Core counterparts (`TaskEntity`, `SubtaskEntity`): navigation properties and ORM concerns stay in `Persistence`, `Core` only knows IDs and business rules.

Validation is layered on purpose, not by accident:

1. **DTO** — Data Annotations, shape/format checks before the controller body runs
2. **Application** — business/DB-dependent checks (Result pattern, no exceptions for expected failures)
3. **Domain** — invariants that must hold no matter who calls the entity (e.g. a task's title length, or "can't remove the last assignee")

---

## API design

- REST, `/api/v1`, resource-based (`/tasks`, `/subtasks`, `/users`, `/auth`)
- Filtering via query parameters, not one endpoint per filter combination
- `RFC 7807 ProblemDetails` for every error response, including uncaught exceptions (global `IExceptionHandler`, no leaked stack traces or headers in production)
- Full endpoint list, DTOs, and status codes: [`docs/03-api-design.md`](./docs/03-api-design.md)

---

## Documentation

This project is documented *before* being built, not after — each design decision (and the reasoning, and the ones that got reversed) lives in `/docs`:

| Doc | Covers |
|---|---|
| [`01-requirements.md`](./docs/01-requirements.md) | Functional requirements, user flows, business rules |
| [`02-database-design.md`](./docs/02-database-design.md) | ER diagram, schema, and *why* each design choice was made |
| [`03-api-design.md`](./docs/03-api-design.md) | REST contract, DTOs, conventions, status codes |

English versions available as `*.en.md` alongside each file.

---

## Status

- ✅ Requirements & database design finalized
- ✅ Backend: Clean Architecture skeleton, auth (JWT + cookie), Task/Subtask CRUD, assignee management
- 🚧 Backend: recreate flow, notifications, remaining endpoints
- ⏳ Frontend: not started yet

---

## Running locally

```bash
# Postgres via Docker
docker compose up -d

# Backend
cd backend/music-time-manager.API
dotnet ef database update -p ../music-time-manager.Persistence -s .
dotnet run
```

Copy `appsettings.Development.json.example` → `appsettings.Development.json` and fill in your local connection string and JWT secret — these are gitignored and never committed.

---

<div align="center">

Built solo, backend written by hand on purpose — this repo is also a learning log for going deeper on ASP.NET Core.

</div>
