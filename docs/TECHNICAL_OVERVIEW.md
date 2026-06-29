# TaskForge — Technical Overview

> **Português (Brasil):** [TECHNICAL_OVERVIEW.pt-BR.md](TECHNICAL_OVERVIEW.pt-BR.md)

This document reflects the **actual state of the repository** as verified against source code. When documentation conflicts with code, code wins.

---

## 1. Solution and projects

**Solution file:** `TaskForge.sln`

| Project | Target | Responsibility |
|---------|--------|----------------|
| `TaskForge.Domain` | `net8.0` | Domain entities and invariants |
| `TaskForge.Application` | `net8.0` | CQRS handlers, DTOs, validators, repository interfaces |
| `TaskForge.Infrastructure` | `net8.0` | EF Core, Identity, repositories, migrations |
| `TaskForge.Api` | `net8.0` | HTTP API, controllers, composition root (`Program.cs`) |
| `TaskForge.Tests` | `net8.0` | Unit tests (references Domain + Application only) |

**Dependency direction:**

```text
Domain ← Application ← Infrastructure ← Api
                              ↑
                         Tests (Application, Domain)
```

---

## 2. Layer responsibilities

### Domain (`TaskForge.Domain`)

- Contains `Project` entity with constructor validation and `UpdateDetails`.
- No `TaskItem`, enums, or `OwnerId` on `Project`.
- No dependency on other layers.

### Application (`TaskForge.Application`)

- MediatR commands/queries for **Projects only** (5 handlers).
- `IProjectRepository` interface.
- FluentValidation validators for create/update project commands.
- `ValidationBehavior<,>` MediatR pipeline behavior.

### Infrastructure (`TaskForge.Infrastructure`)

- `TaskForgeDbContext` extending `IdentityDbContext<ApplicationUser>`.
- `ApplicationUser` with `FullName`.
- `ProjectRepository` implementing `IProjectRepository`.
- EF Core migrations for Identity schema and `Projects` table.

### API (`TaskForge.Api`)

- Controllers: `UsersController`, `SessionsController`, `ProjectsController`, `HealthController`.
- JWT and Identity configuration in `Program.cs`.
- Swagger, health checks, global exception handler.

---

## 3. Main dependencies

| Package | Project | Version (approx.) |
|---------|---------|-------------------|
| MediatR | Application | 12.1.1 |
| FluentValidation | Application | 11.3.0 |
| FluentValidation.AspNetCore | Api | (via Api csproj) |
| EF Core + Npgsql | Infrastructure, Api | 8.0.11 |
| ASP.NET Core Identity | Infrastructure, Api | 8.x |
| JWT Bearer | Api | 8.x |
| Swashbuckle (Swagger) | Api | — |
| AspNetCore.HealthChecks.NpgSql | Api | — |
| xUnit, Moq, FluentAssertions | Tests | — |

**Not present:** Serilog, Polly (beyond EF retry), MassTransit, Redis, OpenTelemetry.

---

## 4. Current architecture

Clean Architecture with CQRS for the Projects module:

```text
HTTP Request
  → Controller
  → MediatR
  → ValidationBehavior (FluentValidation)
  → Handler
  → IProjectRepository
  → ProjectRepository / TaskForgeDbContext
  → PostgreSQL
```

Auth bypasses MediatR — `UsersController` and `SessionsController` use `UserManager` / `SignInManager` directly.

---

## 5. Request flow example

**Create project (authenticated):**

1. Client sends `POST /api/projects` with `Authorization: Bearer {token}`.
2. JWT middleware validates token (issuer, audience, signature, lifetime).
3. `ProjectsController.Create` sends `CreateProjectCommand` via MediatR.
4. `CreateProjectCommandValidator` runs in `ValidationBehavior`.
5. `CreateProjectHandler` creates `Project` entity and calls `IProjectRepository.AddAsync`.
6. Controller fetches created project and returns `201 Created`.

**Gap:** Handler does not read `User.Identity` — projects are not associated with the authenticated user.

---

## 6. Authentication

| Component | Implementation |
|-----------|----------------|
| Identity user | `ApplicationUser` : `IdentityUser` + `FullName` |
| Password policy | Digit required, min length 6, unique email |
| JWT claims | `sub` (user Id), `email` |
| JWT config | `Jwt:Key` (required, not in `appsettings.json`), `Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpireMinutes` (default 60) |
| Protected routes | `[Authorize]` on `ProjectsController` |
| Public routes | Auth register/login, health endpoints |

**Not implemented:** refresh tokens, email confirmation, role-based policies, `ICurrentUserService`.

---

## 7. Persistence

### DbContext

`TaskForge.Infrastructure/Data/TaskForgeDbContext.cs`:

- `DbSet<Project> Projects`
- Inherits Identity tables from `IdentityDbContext<ApplicationUser>`
- No fluent configuration or `OnModelCreating` overrides

### Migrations

| Migration | Tables |
|-----------|--------|
| `20250426203017_InitialCreate` | `Projects` (Id, Name, Description) |
| `20250428024230_InitialIdentityAndProjects` | Full ASP.NET Identity schema |

### Repository

`ProjectRepository`:

- `GetAllAsync` — returns **all** projects (no user filter)
- `GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`

### Auto-migrate

When `ApplyMigrationsOnStartup=true` (Docker default), migrations run on startup.

---

## 8. Existing endpoints

### Authentication (`UsersController`, `SessionsController`)

| Method | Route | Auth |
|--------|-------|------|
| POST | `/api/users` | Anonymous |
| POST | `/api/sessions` | Anonymous |
| GET | `/api/users/me` | JWT required |

### Projects (`ProjectsController`)

| Method | Route | Auth |
|--------|-------|------|
| GET | `/api/projects` | JWT required |
| POST | `/api/projects` | JWT required |
| GET | `/api/projects/{id:guid}` | JWT required |
| PUT | `/api/projects/{id:guid}` | JWT required |
| DELETE | `/api/projects/{id:guid}` | JWT required |

### Health (`HealthController`)

| Method | Route | Auth |
|--------|-------|------|
| GET | `/health/live` | Anonymous |
| GET | `/health/ready` | Anonymous (checks PostgreSQL) |

### Not implemented

All task endpoints under `/api/projects/{projectId}/tasks`.

---

## 9. Validation

| Area | Mechanism |
|------|-----------|
| Project name | Domain constructor + `UpdateDetails`; FluentValidation max 100 chars |
| Project description | FluentValidation max 500 chars |
| Auth DTOs | Identity password rules only (no FluentValidation validators) |
| Model binding | Custom `InvalidModelStateResponseFactory` → 400 ValidationProblemDetails |

Global handler maps `ValidationException` → 400, `KeyNotFoundException` → 404, others → 500.

---

## 10. Tests

**Framework:** xUnit + Moq + FluentAssertions

| Test class | Coverage |
|------------|----------|
| `DomainTests/ProjectTests.cs` | Domain validation (4 facts/theories) |
| `Application/Projects/Commands/CreateProjectHandlerTests.cs` | Create handler (5 tests) |

**Last verified run:** 13 tests passed, 0 failed.

**Not covered:** update/delete/get handlers, validators, auth, repository, API integration, ownership, tasks.

---

## 11. Health checks

Registered via `AddHealthChecks().AddNpgSql(...)` with tags `db`, `ready`.

Exposed through custom `HealthController` (not `MapHealthChecks`):

- `/health/live` — always 200 if process is running
- `/health/ready` — 200 if PostgreSQL healthy, 503 otherwise

Docker `HEALTHCHECK` curls `/health/live`.

---

## 12. Configuration

### `appsettings.json`

- `ConnectionStrings:Default` — PostgreSQL localhost defaults
- `Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpireMinutes`
- **No `Jwt:Key`** (must be supplied externally)

### Environment / secrets

| Key | Source |
|-----|--------|
| `Jwt:Key` | User Secrets (local) or `JWT_KEY` in `.env` (Docker) |
| `ConnectionStrings:Default` | `appsettings.json` or `ConnectionStrings__Default` |
| `ApplyMigrationsOnStartup` | Docker compose sets `true` |
| `EnableSwagger` | Docker compose sets `true` |

### `.env.example`

Documents `JWT_KEY`, PostgreSQL credentials, optional JWT settings, `API_PORT`.

---

## 13. Run commands

### Docker (full stack)

```powershell
cd TaskForge
copy .env.example .env
docker compose up -d --build
```

### Local development

```powershell
dotnet tool restore
dotnet user-secrets set "Jwt:Key" "<secret-32-chars>" --project TaskForge.Api
docker compose up -d postgres
dotnet ef database update --project TaskForge.Infrastructure --startup-project TaskForge.Api
dotnet run --project TaskForge.Api
```

### Tests

```powershell
dotnet test TaskForge.sln
```

### EF tools

`dotnet-tools.json` pins `dotnet-ef` 8.0.11.

---

## 14. Docker Compose

**File location:** solution root (`TaskForge/docker-compose.yml`) — not under `TaskForge.Api/`.

| Service | Purpose |
|---------|---------|
| `postgres` | PostgreSQL 15 Alpine, port 5432, volume `pgdata` |
| `api` | Built from `Dockerfile`, port `${API_PORT:-8080}` |
| `portainer` | Container management UI (optional) |

---

## 15. Known issues and gaps

| Issue | Severity |
|-------|----------|
| No `OwnerId` on `Project` — all authenticated users see all projects | **High** — blocks MVP acceptance |
| No `TaskItem` entity or task API | **High** — blocks MVP acceptance |
| `Project` missing `CreatedAt`/`UpdatedAt` per MVP spec | Medium |
| `ApplicationUser` in Infrastructure, not Domain | Low — architectural choice |
| No integration tests | Medium |
| `ESTADO_DA_ESTRUTURA.md` outdated (Serilog, tests, docker path) | Documentation only |

---

## 16. Divergences: implementation vs planned MVP scope

| Planned | Actual |
|---------|--------|
| `Project.OwnerId` | Missing |
| User-scoped project queries | Returns all projects |
| `TaskItem` + enums + CRUD | Not started |
| `CompletedAt` / reopen rules | Not started |
| Cascade delete tasks on project delete | N/A until tasks exist |
| Task query filters (`?status=`, `?priority=`) | Not started |
| Serilog structured logging | Not implemented |

---

## 17. What is NOT claimed

The following are **not** implemented and should not be documented as present:

- Structured logging / Serilog
- CI/CD pipelines
- Advanced observability (metrics, tracing)
- Resilience patterns beyond EF retry
- Refresh tokens or 2FA
- Role-based access control
- Task management
- Multi-tenant data isolation
