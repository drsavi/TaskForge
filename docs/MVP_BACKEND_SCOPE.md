# TaskForge — MVP Backend Scope

> **Português (Brasil):** [MVP_BACKEND_SCOPE.pt-BR.md](MVP_BACKEND_SCOPE.pt-BR.md)

This document defines exactly what must be completed in the backend MVP before starting future modules or a visual frontend.

---

## 1. MVP objective

Provide a **demonstrable technical foundation** that allows a user to:

```text
create account → login → receive JWT → create projects → create tasks within projects
→ change priority and status → complete tasks → access only their own data
```

The MVP is not the full long-term product. It establishes authentication, ownership, and core CRUD patterns that future modules will reuse.

---

## 2. Modules included

```text
Identity
Projects
Tasks
```

---

## 3. Entities

### 3.1 ApplicationUser

```text
ApplicationUser
├── Id
├── FullName
├── Email
├── PasswordHash
└── Projects (navigation, future)
```

**Current implementation note:** `ApplicationUser` lives in `TaskForge.Infrastructure/Identity/`, extends `IdentityUser`, and includes `FullName`. There is no `Projects` navigation property yet, and `Project` has no `OwnerId`.

### 3.2 Project

```text
Project
├── Id: Guid
├── OwnerId: string
├── Name: string
├── Description: string?
├── CreatedAt: DateTimeOffset
├── UpdatedAt: DateTimeOffset?
└── Tasks: ICollection<TaskItem>
```

**Current implementation note:** Only `Id`, `Name`, and `Description` exist today. `OwnerId`, timestamps, and `Tasks` collection are **not implemented**.

### 3.3 TaskItem

Use the name `TaskItem` to avoid conflict with `System.Threading.Tasks.Task`.

```text
TaskItem
├── Id: Guid
├── ProjectId: Guid
├── Title: string
├── Description: string?
├── Status: TaskItemStatus
├── Priority: TaskItemPriority
├── DueDate: DateTimeOffset?
├── CreatedAt: DateTimeOffset
├── UpdatedAt: DateTimeOffset?
└── CompletedAt: DateTimeOffset?
```

**Current implementation note:** **Not implemented** — no entity, migration, or API.

### 3.4 TaskItemStatus

```text
Pending
InProgress
Completed
```

### 3.5 TaskItemPriority

```text
Low
Medium
High
```

---

## 4. Business rules

### 4.1 Projects

| Rule | MVP requirement | Current state |
|------|-----------------|---------------|
| Every project belongs to a user | Required | **Not implemented** — no `OwnerId` |
| Name is required | Required | **Implemented** (domain + FluentValidation) |
| Name has character limit | Required | **Implemented** (max 100 via FluentValidation) |
| Description is optional | Required | **Implemented** |
| User sees only own projects | Required | **Not implemented** — `GetAllAsync` returns all projects |
| User edits only own projects | Required | **Not implemented** |
| User deletes only own projects | Required | **Not implemented** |
| Deleting a project deletes its tasks | Required | **Not applicable yet** — no tasks |

### 4.2 Tasks

| Rule | MVP requirement | Current state |
|------|-----------------|---------------|
| Every task belongs to a project | Required | **Not implemented** |
| Title is required | Required | **Not implemented** |
| Description is optional | Required | **Not implemented** |
| Default priority is `Medium` | Required | **Not implemented** |
| Initial status is `Pending` | Required | **Not implemented** |
| `Completed` sets `CompletedAt` | Required | **Not implemented** |
| Reopening clears `CompletedAt` | Required | **Not implemented** |
| User accesses tasks only through own projects | Required | **Not implemented** |

---

## 5. Endpoints

### 5.1 Authentication

| Method | Route | Current state |
|--------|-------|---------------|
| POST | `/api/auth/register` | **Implemented** |
| POST | `/api/auth/login` | **Implemented** |

### 5.2 Projects

| Method | Route | Current state |
|--------|-------|---------------|
| GET | `/api/projects` | **Implemented** (no user filter) |
| POST | `/api/projects` | **Implemented** (no owner assignment) |
| GET | `/api/projects/{id}` | **Implemented** |
| PUT | `/api/projects/{id}` | **Implemented** |
| DELETE | `/api/projects/{id}` | **Implemented** |

### 5.3 Tasks

| Method | Route | Current state |
|--------|-------|---------------|
| GET | `/api/projects/{projectId}/tasks` | **Not implemented** |
| POST | `/api/projects/{projectId}/tasks` | **Not implemented** |
| GET | `/api/projects/{projectId}/tasks/{taskId}` | **Not implemented** |
| PUT | `/api/projects/{projectId}/tasks/{taskId}` | **Not implemented** |
| DELETE | `/api/projects/{projectId}/tasks/{taskId}` | **Not implemented** |

### 5.4 Optional filters (after CRUD)

| Method | Route | Current state |
|--------|-------|---------------|
| GET | `/api/projects/{projectId}/tasks?status=InProgress` | **Planned** |
| GET | `/api/projects/{projectId}/tasks?priority=High` | **Planned** |

---

## 6. Acceptance criteria

The MVP is complete when this scenario works end to end:

```text
1.  Register user A
2.  Login as user A
3.  Create project "Candidatura SENAI"
4.  Create task "Preparar aula-teste"
5.  Set priority High
6.  Change status to InProgress
7.  List project tasks
8.  Complete the task
9.  Register user B
10. Login as user B
11. Confirm user B cannot access user A's project
12. Confirm user B cannot access user A's project tasks
```

**Current state:** Steps 1–3 and project listing work. Steps 4–12 are **not yet possible** (no tasks, no ownership isolation).

---

## 7. Technical requirements

| Requirement | Current state |
|-------------|---------------|
| .NET 8, ASP.NET Core Web API | **Implemented** |
| Clean Architecture (Domain, Application, Infrastructure, Api) | **Implemented** |
| PostgreSQL + EF Core | **Implemented** |
| ASP.NET Core Identity + JWT | **Implemented** |
| MediatR for commands/queries | **Implemented** (Projects only) |
| FluentValidation pipeline | **Implemented** (Project commands) |
| Swagger with Bearer auth | **Implemented** |
| Health checks (`/health/live`, `/health/ready`) | **Implemented** |
| Global error handling | **Implemented** |
| Docker Compose (PostgreSQL + API) | **Implemented** |
| Migrations | **Implemented** (Identity + Projects) |
| JWT secret outside versioned config | **Implemented** (user-secrets / `.env`) |
| Structured logging (Serilog) | **Not implemented** — uses built-in ASP.NET logging |
| CI/CD pipeline | **Not implemented** |
| Refresh tokens | **Out of MVP scope** |

---

## 8. Implementation checklist

Progress uses verifiable items only. Partial items count as 0.

### 8.1 Identity (8 items)

| # | Item | Done |
|---|------|------|
| 1 | Register endpoint | ✓ |
| 2 | Login endpoint | ✓ |
| 3 | Password hashing via Identity | ✓ |
| 4 | JWT generation | ✓ |
| 5 | JWT validation | ✓ |
| 6 | Current user identification in business logic | ✗ |
| 7 | Swagger Bearer authentication | ✓ |
| 8 | Sensitive configuration outside versioned files | ✓ |

**Progress: 6 / 8 = 75%**

### 8.2 Projects (12 items)

| # | Item | Done |
|---|------|------|
| 1 | Project entity | ✓ |
| 2 | Create project | ✓ |
| 3 | List projects | ✓ |
| 4 | Get project by id | ✓ |
| 5 | Update project | ✓ |
| 6 | Delete project | ✓ |
| 7 | OwnerId | ✗ |
| 8 | List only current user's projects | ✗ |
| 9 | Block access to another user's project | ✗ |
| 10 | Cascade delete tasks | ✗ |
| 11 | Validation | ✓ |
| 12 | Unit tests (module coverage) | ✗ |

**Progress: 7 / 12 = 58%**

### 8.3 Tasks (14 items)

| # | Item | Done |
|---|------|------|
| 1 | TaskItem entity | ✗ |
| 2 | TaskItemStatus enum | ✗ |
| 3 | TaskItemPriority enum | ✗ |
| 4 | Create task | ✗ |
| 5 | List tasks by project | ✗ |
| 6 | Get task by id | ✗ |
| 7 | Update task | ✗ |
| 8 | Change status | ✗ |
| 9 | Set priority | ✗ |
| 10 | Delete task | ✗ |
| 11 | CompletedAt rule | ✗ |
| 12 | Reopen rule | ✗ |
| 13 | Block access through another user's project | ✗ |
| 14 | Validation + unit tests | ✗ |

**Progress: 0 / 14 = 0%**

### 8.4 Persistence and infrastructure (11 items)

| # | Item | Done |
|---|------|------|
| 1 | PostgreSQL configuration | ✓ |
| 2 | EF Core | ✓ |
| 3 | DbContext | ✓ |
| 4 | Migrations | ✓ |
| 5 | Consistent local credentials | ✓ |
| 6 | Docker Compose for PostgreSQL | ✓ |
| 7 | Health live endpoint | ✓ |
| 8 | Health ready endpoint | ✓ |
| 9 | Global error handling | ✓ |
| 10 | Swagger | ✓ |
| 11 | Local execution documented | ✓ |

**Progress: 11 / 11 = 100%**

### 8.5 MVP overall (core modules + infrastructure)

```text
(6 + 7 + 0 + 11) / (8 + 12 + 14 + 11) = 24 / 45 = 53%
```

---

## 9. Out of scope (current MVP)

These items belong to the **future roadmap only**:

- Frontend
- Inbox
- Habits
- Routines
- Tracker
- Free-form lists
- Calendar
- Notes
- Dashboard
- Insights
- AI assistance
- Multi-user collaboration, invites, comments, attachments
- Notifications
- Gamification
- Microservices
- Refresh token
- Complex roles
- Full CI/CD
- Advanced analytics

---

## 10. Divergences from previous documentation

| Previous claim | Actual state (code is source of truth) |
|----------------|----------------------------------------|
| `ESTADO_DA_ESTRUTURA.md` lists Serilog as implemented | Built-in ASP.NET logging only |
| `ESTADO_DA_ESTRUTURA.md` marks unit tests as pending | 13 unit tests exist and pass |
| `ESTADO_DA_ESTRUTURA.md` places `docker-compose.yml` under `TaskForge.Api/` | File is at solution root |
| `ESTADO_DA_ESTRUTURA.md` marks containerization as pending | Docker Compose stack exists |
| README files imply a project-management API | Accurate for projects/auth; tasks were never implemented |
