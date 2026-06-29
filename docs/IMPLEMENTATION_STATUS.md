# TaskForge — Implementation Status

> **Português (Brasil):** [IMPLEMENTATION_STATUS.pt-BR.md](IMPLEMENTATION_STATUS.pt-BR.md)

Progress is calculated only from **verifiable checklist items**. Partial items count as 0 in the main percentage. Formula: `completed / total × 100`, rounded to nearest integer.

**Last verified:** 2026-06-07 — GIT_WORKFLOW documentation added; MVP verified 2026-06-08 (`dotnet test` 28 passed, acceptance 12/12).

---

## Summary percentages

| Area | Completed | Total | Progress |
|------|-----------|-------|----------|
| **MVP — Identity** | 7 | 8 | **88%** |
| **MVP — Projects** | 11 | 12 | **92%** |
| **MVP — Tasks** | 13 | 14 | **93%** |
| **MVP — Infrastructure** | 11 | 11 | **100%** |
| **MVP overall** (Identity + Projects + Tasks + Infrastructure) | 42 | 45 | **93%** |
| **Documentation** | 12 | 12 | **100%** |
| **Tests** | 5 | 8 | **63%** |

---

## MVP — Identity

| Area | Feature | Status | Progress | Evidence | In current MVP | Notes |
|------|---------|--------|----------|----------|----------------|-------|
| Identity | Register endpoint | Implemented | 90% | `UsersController.Register` | Yes | Returns 201 + `UserDto` |
| Identity | Login endpoint | Implemented | 90% | `SessionsController.Create` | Yes | Returns 201 + `SessionDto` |
| Identity | Password hashing via Identity | Implemented | 88% | `UserManager.CreateAsync` | Yes | Digit + non-alphanumeric required |
| Identity | JWT generation | Implemented | 90% | `JwtTokenService` | Yes | Claims: sub, email |
| Identity | Current user endpoint | Implemented | 90% | `UsersController.GetMe` | Yes | Requires JWT |
| Identity | JWT validation | Implemented | 88% | `Program.cs` AddJwtBearer | Yes | Issuer, audience, lifetime |
| Identity | Current user in business logic | Implemented | 88% | `ICurrentUserService` in handlers | Yes | `ClaimTypes.NameIdentifier` |
| Identity | Swagger Bearer auth | Implemented | 88% | `Program.cs` AddSecurityDefinition | Yes | — |
| Identity | Sensitive config outside repo | Implemented | 88% | No `Jwt:Key` in appsettings | Yes | user-secrets / `.env` |

---

## MVP — Projects

| Area | Feature | Status | Progress | Evidence | In current MVP | Notes |
|------|---------|--------|----------|----------|----------------|-------|
| Projects | Project entity | Implemented | 92% | `Domain/Entities/Project.cs` | Yes | OwnerId, CreatedAt, UpdatedAt |
| Projects | Create project | Implemented | 92% | POST `/api/projects` | Yes | OwnerId from current user |
| Projects | List projects | Implemented | 92% | GET `/api/projects` | Yes | `GetAllForOwnerAsync` |
| Projects | Get project by id | Implemented | 92% | GET `/api/projects/{id}` | Yes | Owner-scoped lookup |
| Projects | Update project | Implemented | 92% | PUT `/api/projects/{id}` | Yes | — |
| Projects | Delete project | Implemented | 92% | DELETE `/api/projects/{id}` | Yes | — |
| Projects | OwnerId | Implemented | 92% | Entity + migration `AddOwnerIdAndTaskItems` | Yes | FK to AspNetUsers |
| Projects | List only own projects | Implemented | 92% | `GetAllForOwnerAsync` | Yes | — |
| Projects | Block other user's project | Implemented | 92% | Returns 404 for other owners | Yes | Verified in acceptance step 11 |
| Projects | Cascade delete tasks | Implemented | 92% | EF `OnDelete(DeleteBehavior.Cascade)` | Yes | TaskItems deleted with project |
| Projects | Validation | Implemented | 92% | FluentValidation + domain | Yes | Name max 100, desc max 500 |
| Projects | Unit tests (module) | Partially implemented | 92% | Create/Update/Delete/GetById handlers | Yes | Missing GetAll handler + validator tests |

---

## MVP — Tasks

| Area | Feature | Status | Progress | Evidence | In current MVP | Notes |
|------|---------|--------|----------|----------|----------------|-------|
| Tasks | TaskItem entity | Implemented | 93% | `Domain/Entities/TaskItem.cs` | Yes | — |
| Tasks | TaskItemStatus enum | Implemented | 93% | `Domain/Enums/TaskItemStatus.cs` | Yes | Pending, InProgress, Completed |
| Tasks | TaskItemPriority enum | Implemented | 93% | `Domain/Enums/TaskItemPriority.cs` | Yes | Low, Medium, High |
| Tasks | Create task | Implemented | 93% | POST `/api/projects/{id}/tasks` | Yes | — |
| Tasks | List tasks by project | Implemented | 93% | GET `/api/projects/{id}/tasks` | Yes | Optional `?status=` `?priority=` |
| Tasks | Get task by id | Implemented | 93% | GET `.../tasks/{taskId}` | Yes | — |
| Tasks | Update task | Implemented | 93% | PUT `.../tasks/{taskId}` | Yes | — |
| Tasks | Change status | Implemented | 93% | `UpdateTaskHandler` + `ChangeStatus` | Yes | — |
| Tasks | Set priority | Implemented | 93% | `UpdateTaskHandler` | Yes | — |
| Tasks | Delete task | Implemented | 93% | DELETE `.../tasks/{taskId}` | Yes | — |
| Tasks | CompletedAt rule | Implemented | 93% | `TaskItem.ChangeStatus` | Yes | `TaskItemTests` |
| Tasks | Reopen rule | Implemented | 93% | Clears `CompletedAt` on reopen | Yes | `UpdateTaskHandlerTests` |
| Tasks | Block via other user's project | Implemented | 93% | `ProjectAuthorization` + 404 | Yes | Verified in acceptance step 12 |
| Tasks | Validation + unit tests | Partially implemented | 93% | Validators + 3 test classes | Yes | Missing Delete/Get/List handler tests |

---

## MVP — Infrastructure

| Area | Feature | Status | Progress | Evidence | In current MVP | Notes |
|------|---------|--------|----------|----------|----------------|-------|
| Infra | PostgreSQL configuration | Implemented | 100% | Npgsql in Program.cs | Yes | Retry on failure enabled |
| Infra | EF Core | Implemented | 100% | Infrastructure csproj | Yes | v8.0.11 |
| Infra | DbContext | Implemented | 100% | `TaskForgeDbContext.cs` | Yes | Identity + Projects + TaskItems |
| Infra | Migrations | Implemented | 100% | 3 migrations | Yes | Includes `AddOwnerIdAndTaskItems` |
| Infra | Consistent local credentials | Implemented | 100% | appsettings + `.env.example` | Yes | postgres/postgres/taskforge_db |
| Infra | Docker Compose | Implemented | 100% | `docker-compose.yml` | Yes | postgres + api + portainer |
| Infra | Health live endpoint | Implemented | 100% | `HealthController` | Yes | GET /health/live |
| Infra | Health ready endpoint | Implemented | 100% | NpgSql health check | Yes | GET /health/ready |
| Infra | Global error handling | Implemented | 100% | `Program.cs` UseExceptionHandler | Yes | 400/403/404/500 mapping |
| Infra | Swagger | Implemented | 100% | AddSwaggerGen + UI | Yes | Bearer scheme |
| Infra | Local execution documented | Implemented | 100% | README + TECHNICAL_OVERVIEW | Yes | — |

---

## Future modules

| Area | Feature | Status | Progress | Evidence | In current MVP | Notes |
|------|---------|--------|----------|----------|----------------|-------|
| Inbox | Module | Planned for future version | 0% | No code | No | First roadmap item after MVP |
| Habits | Module | Planned for future version | 0% | No code | No | — |
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
| Docs | GIT_WORKFLOW EN/PT | Implemented | 100% | `docs/GIT_WORKFLOW*.md` | Yes | Branches, commits, merge flow |
| Docs | IMPLEMENTATION_STATUS EN/PT | Implemented | 100% | This file | Yes | Updated 2026-06-07 |

---

## Tests

| Area | Feature | Status | Progress | Evidence | In current MVP | Notes |
|------|---------|--------|----------|----------|----------------|-------|
| Tests | Solution builds | Implemented | 63% | `dotnet build` succeeds | Yes | — |
| Tests | Existing tests pass | Implemented | 63% | 28/28 passed | Yes | 2026-06-08 |
| Tests | Domain unit tests | Implemented | 63% | `ProjectTests` + `TaskItemTests` | Yes | 9 test methods |
| Tests | MVP unit tests (full coverage) | Partially implemented | 63% | 7 handler test classes | Yes | Missing auth, some task handlers |
| Tests | Integration tests | Planned for current MVP | 63% | None | Yes | — |
| Tests | Auth tests | Planned for current MVP | 63% | None | Yes | — |
| Tests | Ownership authorization tests | Implemented | 63% | Project + task handler tests | Yes | Handler-level, not API integration |
| Tests | End-to-end manual validation | Implemented | 63% | 12/12 acceptance steps | Yes | 2026-06-08 via HTTP |

**Tests progress: 5 / 8 = 63%**

---

## Acceptance criteria status

| Step | Description | Status |
|------|-------------|--------|
| 1–2 | Register and login user A | **Pass** |
| 3 | Create project "Candidatura SENAI" | **Pass** |
| 4 | Create task "Preparar aula-teste" | **Pass** |
| 5 | Set priority High | **Pass** |
| 6 | Change status to InProgress | **Pass** |
| 7 | List project tasks | **Pass** |
| 8 | Complete task (`CompletedAt` set) | **Pass** |
| 9–10 | Register and login user B | **Pass** |
| 11 | User B cannot access user A's project | **Pass** (404) |
| 12 | User B cannot access user A's tasks | **Pass** (404) |

**MVP acceptance scenario: complete.**
