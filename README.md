<div align="center">

# 🎵 Music Time Manager

A task manager for a small music production team — organizing weekly work through tasks, subtasks, and multiple assignees, with a calendar that reflects reality instead of a background job pretending to.

[![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)](#)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Neon-4169E1?logo=postgresql&logoColor=white)](#)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker&logoColor=white)](#)
[![React](https://img.shields.io/badge/React-TypeScript-61DAFB?logo=react&logoColor=black)](#)
[![TanStack Query](https://img.shields.io/badge/TanStack%20Query-FF4154?logo=reactquery&logoColor=white)](#)
[![Tailwind](https://img.shields.io/badge/Tailwind-06B6D4?logo=tailwindcss&logoColor=white)](#)

**[Live demo →](https://music-time-manager-1.onrender.com/)**

</div>

> The live demo is currently Russian-only — English is on the roadmap (see below).

---

## What this is

A private task manager for a 2–10 person team. `Task` is the primary entity — not `Project`, which is a deliberate scope decision, not an oversight. Tasks can be split into subtasks, and both support multiple assignees through proper many-to-many relations, not a single "assignee" field bolted on as an afterthought.

The interesting part isn't the CRUD — it's a handful of decisions that came out of actually thinking through the domain before writing code, then writing down *why*, not just *what*:

- **"Overdue" is never stored, always computed.** No stored status, no background sweep job silently going stale between ticks — `isOverdue = dueDate < now && status != Done`, evaluated fresh on every read.
- **Statistics aren't stored either.** Completed/missed counts are aggregate queries against existing tables — zero counter columns, zero desync risk.
- **Recreating a missed task doesn't rewrite history.** A new `Task` row is created with a self-referencing FK (`RecreatedFromTaskId`) pointing back to the original — the original stays in the archive exactly as it was.
- **A task can exist with zero assignees** — briefly, by design. Creation and assignment are separate calls (`TaskAssignee` needs a real `TaskId` to reference before it can exist), but the invariant "can't be emptied once it has one" is still enforced where it matters.
- **Errors carry a stable machine-readable code**, not just an English sentence — the frontend maps `Task.DoesNotExist` / `User.UsernameAlreadyUsed` / etc. to localized messages, so adding a language later means adding a dictionary, not touching backend or frontend logic.

Every non-obvious decision like this — including the ones that got reversed mid-project after more thought — is written down in `/docs`, not left to be reverse-engineered from the code later.

---

## Screenshots

<!-- Add images to docs/screenshots/ and reference them here, e.g.: -->
<!-- ![Login](docs/screenshots/login.png) -->
<!-- ![Task list](docs/screenshots/tasks.png) -->

*(Coming soon — UI is currently Russian-only, screenshots will follow once the English version ships.)*

---

## Features

- **Auth** — registration, login, logout via `HttpOnly` cookie (never exposed to client-side JS), session restore on page load
- **Tasks & Subtasks** — create, edit, delete, filter (by status, assignee, due date, overdue, has-assignees), status transitions with a clear action per state ("Start working" / "Mark done")
- **Multiple assignees** per task and subtask, assigned separately from creation, full-replace semantics on update
- **Recreate flow** — one click turns a missed task into a fresh one with a new due date, without losing the original as a historical record
- **Team view** — per-member workload and completed/missed stats, computed on the fly
- **Rate limiting** with a proper distinction between "you're not logged in" (401) and "slow down" (429) — the second one never logs you out
- **Light/dark theme**, respecting system preference by default with a manual override
- **Localized error messages** on the frontend, mapped from stable backend error codes
- **Responsive, mobile-first UI** — no horizontal scroll, no desktop-only assumptions baked into the layout

---

## Stack

| Layer | Tech |
|---|---|
| **Backend** | ASP.NET Core Web API · EF Core · PostgreSQL |
| **Auth** | JWT in an `HttpOnly` cookie — not the `Authorization` header |
| **Architecture** | Clean Architecture: `API` → `Application` → `Core` → `Infrastructure` / `Persistence` |
| **Frontend** | React · TypeScript · Vite · Tailwind CSS · Framer Motion · TanStack Query · Axios · React Router |
| **Local dev** | Docker Compose (Postgres container) |
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

frontend/
└── src/                                → React app (Vite), TanStack Query hooks per resource,
                                           Axios client with cookie-based auth and rate-limit handling
```

Dependencies point inward — `Core` knows nothing about EF Core, HTTP, or persistence. Domain entities (`Task`, `Subtask`) are deliberately separate from their EF Core counterparts (`TaskEntity`, `SubtaskEntity`): navigation properties and ORM concerns stay in `Persistence`, `Core` only knows IDs and business rules.

Validation is layered on purpose, not by accident:

1. **DTO** — Data Annotations, shape/format checks before the controller body runs
2. **Application** — business/DB-dependent checks (Result pattern, no exceptions for expected failures)
3. **Domain** — invariants that must hold no matter who calls the entity (e.g. a task's title length, or "can't remove the last assignee")

---

## API design

- REST, resource-based (`/tasks`, `/subtasks`, `/users`, `/auth`)
- Filtering via query parameters, not one endpoint per filter combination
- `RFC 7807 ProblemDetails` for every error response, including uncaught exceptions (global `IExceptionHandler`, no leaked stack traces or request headers in production)
- Stable, dot-namespaced error codes (`Task.DoesNotExist`, `User.UsernameAlreadyUsed`, ...) returned alongside the human-readable message, meant to be consumed by clients rather than parsed from English text
- Full endpoint list, DTOs, and status codes: [`03-api-design.md`](docs/en/03-api-design.md)

---

## Documentation

This project is documented *before* being built, not after — each design decision (and the reasoning, including the ones that got reversed) lives in `/docs`:

| Doc | Covers |
|---|---|
| [`01-requirements.md`](docs/en/01-requirements.md) | Functional requirements, user flows, business rules |
| [`02-database-design.md`](docs/en/02-database-design.md) | ER diagram, schema, and *why* each design choice was made |
| [`03-api-design.md`](docs/en/03-api-design.md) | REST contract, DTOs, conventions, status codes |

Documentation in Russian language is available [here](docs/ru).

---

## Roadmap

- [ ] English UI (the app is Russian-only right now)
- [ ] Telegram bot notifications for due/overdue tasks
- [ ] Structured logging with Serilog
- [ ] Automated tests (currently untested — small enough surface area that it hasn't bitten yet, but it will)

---

## Running locally

```bash
# Postgres via Docker
docker compose up -d

# Backend
cd backend/music-time-manager.API
dotnet ef database update -p ../music-time-manager.Persistence -s .
dotnet run

# Frontend
cd frontend
npm install
npm run dev
```

Copy `appsettings.Development.json.example` → `appsettings.Development.json` and fill in your local connection string and JWT secret — these are gitignored and never committed.

---

<div align="center">

Backend written by hand, solo, on purpose — this repo doubles as a learning log for going deeper on ASP.NET Core and Clean Architecture rather than shipping the fastest possible way.

</div>
