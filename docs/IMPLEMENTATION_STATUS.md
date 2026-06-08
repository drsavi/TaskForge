# TaskForge — Implementation Status

> **Português (Brasil):** [IMPLEMENTATION_STATUS.pt-BR.md](IMPLEMENTATION_STATUS.pt-BR.md)

Progress is calculated only from **verifiable checklist items**. Partial items count as 0 in the main percentage. Formula: `completed / total × 100`, rounded to nearest integer.

**Last verified:** 2026-06-07 — code review + `dotnet test` (13 passed).

---

## Summary percentages

| Area | Completed | Total | Progress |
|------|-----------|-------|----------|
| **MVP — Identity** | 6 | 8 | **75%** |
| **MVP — Projects** | 7 | 12 | **58%** |
| **MVP — Tasks** | 0 | 14 | **0%** |
| **MVP — Infrastructure** | 11 | 11 | **100%** |
| **MVP overall** (Identity + Projects + Tasks + Infrastructure) | 24 | 45 | **53%** |
| **Documentation** | 10 | 10 | **100%** |
| **Tests** | 3 | 8 | **38%** |

---

## MVP — Identity

| Area | Feature | Status | Progress | Evidence | In current MVP | Notes |
|------|---------|--------|----------|----------|----------------|-------|
| Identity | Register endpoint | Implemented | 75% | `AuthController.Register` | Yes | Returns 400 on Identity errors |
| Identity | Login endpoint | Implemented | 75% | `AuthController.Login` | Yes | Returns JWT `{ token }` |
| Identity | Password hashing via Identity | Implemented | 75% | `UserManager.CreateAsync` | Yes | Digit required, min 6 chars |
| Identity | JWT generation | Implemented | 75% | `AuthController.GenerateJwt` | Yes | Claims: sub, email |
| Identity | JWT validation | Implemented | 75% | `Program.cs` AddJwtBearer | Yes | Issuer, audience, lifetime |
| Identity | Current user in business logic | Partially implemented | 75% | JWT middleware only | Yes | Handlers do not read user Id |
| Identity | Swagger Bearer auth | Implemented | 75% | `Program.cs` AddSecurityDefinition | Yes | — |
| Identity | Sensitive config outside repo | Implemented | 75% | No `Jwt:Key` in appsettings | Yes | user-secrets / `.env` |

---

## MVP — Projects

| Area | Feature | Status | Progress | Evidence | In current MVP | Notes |
|------|---------|--------|----------|----------|----------------|-------|
| Projects | Project entity | Implemented | 58% | `Domain/Entities/Project.cs` | Yes | Missing OwnerId, timestamps |
| Projects | Create project | Implemented | 58% | POST `/api/projects` | Yes | — |
| Projects | List projects | Implemented | 58% | GET `/api/projects` | Yes | No user filter |
| Projects | Get project by id | Implemented | 58% | GET `/api/projects/{id}` | Yes | — |
| Projects | Update project | Implemented | 58% | PUT `/api/projects/{id}` | Yes | — |
| Projects | Delete project | Implemented | 58% | DELETE `/api/projects/{id}` | Yes | — |
| Projects | OwnerId | Planned for current MVP | 58% | Not in entity or migration | Yes | Required for acceptance |
| Projects | List only own projects | Planned for current MVP | 58% | `GetAllAsync` returns all | Yes | — |
| Projects | Block other user's project | Planned for current MVP | 58% | No ownership check | Yes | — |
| Projects | Cascade delete tasks | Planned for current MVP | 58% | N/A — no tasks | Yes | Blocked until Tasks exist |
| Projects | Validation | Implemented | 58% | FluentValidation + domain | Yes | Name max 100, desc max 500 |
| Projects | Unit tests (module) | Partially implemented | 58% | Only CreateProjectHandler | Yes | Update/delete/get untested |

---

## MVP — Tasks

| Area | Feature | Status | Progress | Evidence | In current MVP | Notes |
|------|---------|--------|----------|----------|----------------|-------|
| Tasks | TaskItem entity | Planned for current MVP | 0% | No file in codebase | Yes | — |
| Tasks | TaskItemStatus enum | Planned for current MVP | 0% | Not found | Yes | Pending, InProgress, Completed |
| Tasks | TaskItemPriority enum | Planned for current MVP | 0% | Not found | Yes | Low, Medium, High |
| Tasks | Create task | Planned for current MVP | 0% | No controller | Yes | — |
| Tasks | List tasks by project | Planned for current MVP | 0% | No endpoint | Yes | — |
| Tasks | Get task by id | Planned for current MVP | 0% | No endpoint | Yes | — |
| Tasks | Update task | Planned for current MVP | 0% | No endpoint | Yes | — |
| Tasks | Change status | Planned for current MVP | 0% | No endpoint | Yes | — |
| Tasks | Set priority | Planned for current MVP | 0% | No endpoint | Yes | — |
| Tasks | Delete task | Planned for current MVP | 0% | No endpoint | Yes | — |
| Tasks | CompletedAt rule | Planned for current MVP | 0% | Not found | Yes | — |
| Tasks | Reopen rule | Planned for current MVP | 0% | Not found | Yes | — |
| Tasks | Block via other user's project | Planned for current MVP | 0% | Not found | Yes | Depends on ownership |
| Tasks | Validation + unit tests | Planned for current MVP | 0% | Not found | Yes | — |

---

## MVP — Infrastructure

| Area | Feature | Status | Progress | Evidence | In current MVP | Notes |
|------|---------|--------|----------|----------|----------------|-------|
| Infra | PostgreSQL configuration | Implemented | 100% | Npgsql in Program.cs | Yes | Retry on failure enabled |
| Infra | EF Core | Implemented | 100% | Infrastructure csproj | Yes | v8.0.11 |
| Infra | DbContext | Implemented | 100% | `TaskForgeDbContext.cs` | Yes | Identity + Projects |
| Infra | Migrations | Implemented | 100% | 2 migrations | Yes | — |
| Infra | Consistent local credentials | Implemented | 100% | appsettings + .env.example | Yes | postgres/postgres/taskforge_db |
| Infra | Docker Compose | Implemented | 100% | `docker-compose.yml` | Yes | postgres + api + portainer |
| Infra | Health live endpoint | Implemented | 100% | `HealthController` | Yes | GET /health/live |
| Infra | Health ready endpoint | Implemented | 100% | NpgSql health check | Yes | GET /health/ready |
| Infra | Global error handling | Implemented | 100% | `Program.cs` UseExceptionHandler | Yes | 400/404/500 mapping |
| Infra | Swagger | Implemented | 100% | AddSwaggerGen + UI | Yes | Bearer scheme |
| Infra | Local execution documented | Implemented | 100% | README + TECHNICAL_OVERVIEW | Yes | — |

---

## Future modules

| Area | Feature | Status | Progress | Evidence | In current MVP | Notes |
|------|---------|--------|----------|----------|----------------|-------|
| Inbox | Module | Planned for future version | 0% | No code | No | After MVP + Habits roadmap |
| Habits | Module | Planned for future version | 0% | No code | No | First post-MVP module |
| Routines | Module | Planned for future version | 0% | No code | No | — |
| Tracker | Module | Planned for future version | 0% | No code | No | — |
| Lists | Module | Planned for future version | 0% | No code | No | — |
| Calendar | Module | Planned for future version | 0% | No code | No | — |
| Notes | Module | Planned for future version | 0% | No code | No | — |
| Dashboard | Module | Planned for future version | 0% | No code | No | — |
| Insights | Module | Planned for future version | 0% | No code | No | — |
| AI Assistance | Module | Planned for future version | 0% | No code | No | — |

---

## Documentation

| Area | Feature | Status | Progress | Evidence | In current MVP | Notes |
|------|---------|--------|----------|----------|----------------|-------|
| Docs | README.md (English) | Implemented | 100% | `README.md` | Yes | Product-focused |
| Docs | README.pt-BR.md | Implemented | 100% | `README.pt-BR.md` | Yes | — |
| Docs | TECHNICAL_OVERVIEW EN/PT | Implemented | 100% | `docs/TECHNICAL_OVERVIEW*.md` | Yes | — |
| Docs | MVP_BACKEND_SCOPE EN/PT | Implemented | 100% | `docs/MVP_BACKEND_SCOPE*.md` | Yes | — |
| Docs | PRODUCT_ROADMAP EN/PT | Implemented | 100% | `docs/PRODUCT_ROADMAP*.md` | Yes | — |
| Docs | IMPLEMENTATION_STATUS EN/PT | Implemented | 100% | This file | Yes | — |

---

## Tests

| Area | Feature | Status | Progress | Evidence | In current MVP | Notes |
|------|---------|--------|----------|----------|----------------|-------|
| Tests | Solution builds | Implemented | 38% | `dotnet test` succeeds | Yes | — |
| Tests | Existing tests pass | Implemented | 38% | 13/13 passed | Yes | 2026-06-07 |
| Tests | Domain unit tests | Implemented | 38% | `ProjectTests.cs` | Yes | 4 test methods |
| Tests | MVP unit tests (full coverage) | Partially implemented | 38% | CreateProjectHandler only | Yes | Missing tasks, ownership |
| Tests | Integration tests | Planned for current MVP | 38% | None | Yes | Acceptance scenario needs them |
| Tests | Auth tests | Planned for current MVP | 38% | None | Yes | — |
| Tests | Ownership authorization tests | Planned for current MVP | 38% | None | Yes | — |
| Tests | End-to-end manual validation | Planned for current MVP | 38% | Not recorded | Yes | Blocked by missing tasks |

**Tests progress: 3 / 8 = 38%**

---

## Acceptance criteria status

| Step | Description | Status |
|------|-------------|--------|
| 1–2 | Register and login user A | **Pass** |
| 3 | Create project | **Pass** |
| 4–8 | Task CRUD and completion | **Fail** — no task API |
| 9–10 | Register and login user B | **Pass** |
| 11–12 | User B cannot access user A data | **Fail** — no ownership |

**MVP acceptance scenario: not complete.**
