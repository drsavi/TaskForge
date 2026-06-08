# TaskForge

> **This file has been superseded.** The canonical English README is **[README.md](README.md)**.
>
> **Português (Brasil):** [README.pt-BR.md](README.pt-BR.md)

---

> Legacy deployment documentation below. For product vision, MVP scope, and current status, use the links above and the [docs/](docs/) folder.

Portfolio-grade .NET Web API for project management with JWT authentication, Clean Architecture, and CQRS via MediatR.

## Prerequisites

| Scenario | What to install |
|----------|-----------------|
| **Run everything with Docker (recommended)** | [Docker Engine + Compose](#docker-on-windows-without-desktop) |
| **Develop .NET code locally** | .NET 8 SDK + Docker (database only) or full Docker stack |

---

## Local deploy with Docker (recommended)

Starts **PostgreSQL + API + Portainer** with one command — like installing on your PC/server.

### 1. Install Docker

See [Docker on Windows (without Desktop)](#docker-on-windows-without-desktop) if Docker is not installed yet.

### 2. Configure environment variables

From the solution root (`TaskForge/`):

```powershell
copy .env.example .env
```

Edit `.env` and set a JWT key with **at least 32 characters**:

```env
JWT_KEY=your-local-key-with-at-least-32-characters
```

> The `.env` file is not tracked by git. Never commit real secrets.

### 3. Start the full stack

```powershell
cd TaskForge
docker compose up -d --build
```

This starts:

| Service | Container | Access |
|---------|-----------|--------|
| API + Swagger | `taskforge-api` | **http://localhost:8080/swagger** |
| PostgreSQL | `taskforge-postgres` | `localhost:5432` |
| Portainer (UI) | `taskforge-portainer` | **http://localhost:9000** |

The API applies **migrations automatically** on startup (`ApplyMigrationsOnStartup`).

Check status:

```powershell
docker compose ps
docker compose logs api
```

Stop everything:

```powershell
docker compose down
```

Remove volumes (deletes database data):

```powershell
docker compose down -v
```

### 4. Test via Swagger

Open **http://localhost:8080/swagger** and follow the [manual API flow](#manual-api-flow-swagger).

### 5. Portainer

Open **http://localhost:9000**, create the admin user on first visit, and select the **local** environment. You will see `taskforge-api`, `taskforge-postgres`, and `taskforge-portainer`.

---

## Docker on Windows (without Desktop)

TaskForge **requires a Docker engine**. **Portainer** is only a web UI — it **does not replace** Docker.

### Recommended: Docker Engine on WSL2

1. **Enable WSL2** (PowerShell as administrator):

   ```powershell
   wsl --install
   ```

2. **Inside Ubuntu (WSL)**:

   ```bash
   sudo apt update
   sudo apt install -y docker.io docker-compose-v2
   sudo usermod -aG docker $USER
   ```

   Close and reopen the WSL terminal. Verify: `docker --version` and `docker compose version`.

3. Run the [Local deploy with Docker](#local-deploy-with-docker-recommended) steps from the project folder.

### Alternative

[Docker Desktop](https://www.docker.com/products/docker-desktop/) — easier setup, heavier footprint.

---

## Local .NET development (optional)

Use this flow to debug with `dotnet run`, keeping the database in Docker.

### 1. Tools and JWT

```powershell
dotnet tool restore
dotnet user-secrets set "Jwt:Key" "TaskForge-Local-Dev-Secret-Key-32chars!" --project TaskForge.Api
```

### 2. Start infrastructure only (PostgreSQL + Portainer)

```powershell
cd TaskForge
docker compose up -d postgres portainer
```

### 3. Migrations and API

```powershell
dotnet ef database update --project TaskForge.Infrastructure --startup-project TaskForge.Api
cd TaskForge.Api
dotnet run
```

Local Swagger: `https://localhost:7294/swagger` or `http://localhost:5062/swagger`

### 4. Tests

```powershell
dotnet test TaskForge.sln
```

---

## Manual API flow (Swagger)

1. Open Swagger (`http://localhost:8080/swagger` in Docker, or local `dotnet run` port).
2. **Register** — `POST /api/auth/register`:

   ```json
   {
     "fullName": "Demo User",
     "email": "demo@taskforge.local",
     "password": "Demo123"
   }
   ```

3. **Login** — `POST /api/auth/login`:

   ```json
   {
     "email": "demo@taskforge.local",
     "password": "Demo123"
   }
   ```

   Copy the `token`.

4. **Authorize** — click **Authorize** → `Bearer {token}`.
5. **Create project** — `POST /api/projects`:

   ```json
   {
     "name": "My First Project",
     "description": "Portfolio demo"
   }
   ```

6. **List** — `GET /api/projects`
7. **Get by id** — `GET /api/projects/{id}`
8. **Update** — `PUT /api/projects/{id}`
9. **Delete** — `DELETE /api/projects/{id}`

---

## Configuration reference

| Key | Docker (`.env`) | Local (`dotnet run`) |
|-----|-----------------|----------------------|
| JWT | `JWT_KEY` → `Jwt__Key` | User Secrets / `Jwt__Key` |
| Database | automatic via compose | `ConnectionStrings:Default` in `appsettings.json` |
| Swagger in container | `EnableSwagger=true` | `ASPNETCORE_ENVIRONMENT=Development` |
| Auto migrations | `ApplyMigrationsOnStartup=true` | manual `dotnet ef database update` |

Default PostgreSQL credentials (local dev only): `postgres` / `postgres` / `taskforge_db`.

---

## Health checks

Public endpoints (no JWT), documented in Swagger under **Health**:

| Endpoint | Purpose | Healthy HTTP | Failure HTTP |
|----------|---------|--------------|--------------|
| `GET /health/live` | **Liveness** — is the API process running? | 200 | — |
| `GET /health/ready` | **Readiness** — API + PostgreSQL ready for traffic? | 200 | 503 |

Example response (`GET /health/ready` healthy):

```json
{
  "endpoint": "ready",
  "purpose": "Readiness",
  "description": "Confirma que a API e suas dependências críticas estão prontas para atender requisições.",
  "status": "Healthy",
  "summary": "A aplicação está pronta. Todas as dependências verificadas estão saudáveis.",
  "dependencies": [
    {
      "name": "postgresql",
      "purpose": "Verifica conexão e consulta básica ao PostgreSQL.",
      "status": "Healthy",
      "error": null
    }
  ],
  "checkedAtUtc": "2026-06-03T12:00:00+00:00"
}
```

In Docker: `http://localhost:8080/health/ready`

---

## Architecture

```
TaskForge.Domain → TaskForge.Application → TaskForge.Infrastructure → TaskForge.Api
```

Deploy files: `Dockerfile`, `docker-compose.yml`, `.env.example` at the solution root.

---

## Technologies

- .NET 8, ASP.NET Core Web API, Docker
- Entity Framework Core 8 + PostgreSQL
- ASP.NET Core Identity + JWT Bearer
- MediatR, FluentValidation
- xUnit, Moq, FluentAssertions
