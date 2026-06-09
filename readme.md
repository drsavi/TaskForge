# TaskForge

> **Português (Brasil):** [README.pt-BR.md](README.pt-BR.md)

> **Status:** This project is under active development. The current backend MVP is not yet complete.

TaskForge is a modular personal organization platform designed to centralize projects, tasks, habits, routines, lists, learning activities, and daily planning with minimal friction.

Inspired by tools such as Notion, habit trackers, calendars, and Kanban boards, the project aims to provide a more structured and easier-to-maintain alternative. Its goal is to help users capture demands, prioritize activities, visualize progress and, in the future, understand personal patterns based on their historical data.

The backend is being developed as a **modular monolith** using ASP.NET Core and Clean Architecture principles. The current MVP establishes the foundation of the platform through authentication, project management, tasks, and user-based authorization.

---

## Problem

Maintaining a personal productivity system often requires too much manual upkeep: duplicating information across lists, calendars, and projects; deciding metadata before capturing an idea; and losing visibility into what actually matters today.

TaskForge seeks to reduce that cognitive load by offering enough flexibility to track real life, with enough structure to avoid constant reconfiguration.

---

## Product vision

Over time, TaskForge should work as a **personal operating system for everyday organization**, helping users quickly understand:

- what needs to be done today;
- what is overdue or high priority;
- what fits available time and energy;
- which projects are stalled;
- which habits and routines are being maintained;
- which learning content is in progress;
- which patterns emerge from personal history.

---

## Principles

| Principle | Summary |
|-----------|---------|
| **Low friction** | Capturing a task or habit should take seconds, not a form. |
| **Capture first, organize later** | Only the title is required for quick capture; metadata can wait. |
| **Register once** | A dated task should appear in its project, task list, calendar, and Today view without duplication. |
| **Satisfying visual progress** | Checklists and completion states should feel rewarding and readable. |
| **Low-energy days** | Future views will surface essentials, quick wins, and minimum routines. |
| **Preserved history** | Timestamps and historical data are kept from the start to enable future insights. |

---

## Architecture (one sentence)

A single deployable ASP.NET Core application organized into domain modules (Identity, Projects, Tasks, and future areas) with low coupling between responsibilities — not microservices.

---

## Current state

| Area | Status |
|------|--------|
| **Identity** | Register, login, JWT, and Swagger Bearer auth are implemented. User-scoped data isolation is **not** wired into business logic yet. |
| **Projects** | Full CRUD via MediatR, with FluentValidation. Missing `OwnerId`, per-user filtering, and ownership authorization. |
| **Tasks** | **Not started** — no `TaskItem` entity, enums, or task endpoints. |
| **Infrastructure** | PostgreSQL, EF Core, migrations, Docker Compose, health checks, Swagger, and global error handling are in place. |
| **Tests** | 13 unit tests pass (domain `Project` + `CreateProjectHandler`). No integration or ownership tests. |

See [Implementation Status](docs/IMPLEMENTATION_STATUS.md) for item-level progress and calculated percentages.

---

## Current MVP scope (backend)

The first MVP is a **demonstrable technical foundation**, not the full long-term product.

**Included modules:** Identity, Projects, Tasks.

**Target flow:**

```text
create account → login → receive JWT → create projects → create tasks in projects
→ change priority and status → complete tasks → access only own data
```

**Implemented today:** authentication and project CRUD. Tasks and per-user authorization remain outstanding.

Details: [MVP Backend Scope](docs/MVP_BACKEND_SCOPE.md).

---

## Planned modules (future)

| Module | MVP | Purpose |
|--------|-----|---------|
| Identity | Yes | Authentication and data isolation |
| Projects | Yes | Containers for larger efforts |
| Tasks | Yes | Actionable items with status and priority |
| Inbox | No | Quick capture without immediate categorization |
| Habits | No | Recurring actions with low-friction logging |
| Routines | No | Reusable checklists to start or finish sequences |
| Tracker | No | Books, courses, series, and long-running interests |
| Lists | No | Lightweight free-form lists |
| Calendar | No | Unified temporal view |
| Notes | No | Markdown notes linked to resources |
| Dashboard | No | Consolidated “Today” execution view |
| Insights | No | Personal pattern analysis from history |
| AI Assistance | No | Optional, transparent decision support |

Roadmap and rationale: [Product Roadmap](docs/PRODUCT_ROADMAP.md).

---

## Confirmed stack

- .NET 8, ASP.NET Core Web API
- Entity Framework Core 8 + PostgreSQL
- ASP.NET Core Identity + JWT Bearer
- MediatR, FluentValidation
- Docker, Docker Compose
- xUnit, Moq, FluentAssertions

---

## Quick start

**Docker (recommended):**

```powershell
cd TaskForge
copy .env.example .env
# Edit .env — set JWT_KEY (at least 32 characters)
docker compose up -d --build
```

Open **http://localhost:8080/swagger**.

**Local .NET development:**

```powershell
dotnet tool restore
dotnet user-secrets set "Jwt:Key" "TaskForge-Local-Dev-Secret-Key-32chars!" --project TaskForge.Api
docker compose up -d postgres
dotnet ef database update --project TaskForge.Infrastructure --startup-project TaskForge.Api
dotnet run --project TaskForge.Api
```

Full setup, configuration keys, health checks, and API reference: [Technical Overview](docs/TECHNICAL_OVERVIEW.md).

---

## Documentation

| Document | Description |
|----------|-------------|
| [Product Roadmap](docs/PRODUCT_ROADMAP.md) | Long-term vision, modules, and evolution order |
| [MVP Backend Scope](docs/MVP_BACKEND_SCOPE.md) | Exact MVP requirements and acceptance criteria |
| [Technical Overview](docs/TECHNICAL_OVERVIEW.md) | Repository structure, endpoints, and configuration |
| [Implementation Status](docs/IMPLEMENTATION_STATUS.md) | Verifiable progress tables and percentages |
| [Git Workflow](docs/GIT_WORKFLOW.md) | Branch naming, commit conventions, and merge flow |

---

## Out of scope for now

Frontend, Inbox, habits, routines, Tracker, lists, calendar, notes, dashboard, insights, AI, multi-user collaboration, notifications, gamification, microservices, refresh tokens, complex roles, and full CI/CD are **roadmap items only** — not part of the current MVP.

---

## License

See repository license information if applicable.
