# TaskForge — Visão Técnica

> **English:** [TECHNICAL_OVERVIEW.md](TECHNICAL_OVERVIEW.md)

Este documento reflete o **estado real do repositório**, verificado contra o código-fonte. Quando a documentação conflita com o código, o código prevalece.

---

## 1. Solution e projetos

**Arquivo da solution:** `TaskForge.sln`

| Projeto | Target | Responsabilidade |
|---------|--------|------------------|
| `TaskForge.Domain` | `net8.0` | Entidades de domínio e invariantes |
| `TaskForge.Application` | `net8.0` | Handlers CQRS, DTOs, validators, interfaces de repositório |
| `TaskForge.Infrastructure` | `net8.0` | EF Core, Identity, repositórios, migrations |
| `TaskForge.Api` | `net8.0` | API HTTP, controllers, composition root (`Program.cs`) |
| `TaskForge.Tests` | `net8.0` | Testes unitários (referencia Domain + Application apenas) |

**Direção de dependências:**

```text
Domain ← Application ← Infrastructure ← Api
                              ↑
                         Tests (Application, Domain)
```

---

## 2. Responsabilidades por camada

### Domain (`TaskForge.Domain`)

- Contém entidade `Project` com validação no construtor e `UpdateDetails`.
- Sem `TaskItem`, enums ou `OwnerId` em `Project`.
- Sem dependência de outras camadas.

### Application (`TaskForge.Application`)

- Commands/queries MediatR **apenas para Projects** (5 handlers).
- Interface `IProjectRepository`.
- Validators FluentValidation para create/update de projeto.
- Pipeline behavior `ValidationBehavior<,>`.

### Infrastructure (`TaskForge.Infrastructure`)

- `TaskForgeDbContext` estendendo `IdentityDbContext<ApplicationUser>`.
- `ApplicationUser` com `FullName`.
- `ProjectRepository` implementando `IProjectRepository`.
- Migrations EF Core para schema Identity e tabela `Projects`.

### API (`TaskForge.Api`)

- Controllers: `AuthController`, `ProjectsController`, `HealthController`.
- Configuração JWT e Identity em `Program.cs`.
- Swagger, health checks, tratamento global de exceções.

---

## 3. Dependências principais

| Pacote | Projeto | Versão (aprox.) |
|--------|---------|-----------------|
| MediatR | Application | 12.1.1 |
| FluentValidation | Application | 11.3.0 |
| FluentValidation.AspNetCore | Api | (via csproj da Api) |
| EF Core + Npgsql | Infrastructure, Api | 8.0.11 |
| ASP.NET Core Identity | Infrastructure, Api | 8.x |
| JWT Bearer | Api | 8.x |
| Swashbuckle (Swagger) | Api | — |
| AspNetCore.HealthChecks.NpgSql | Api | — |
| xUnit, Moq, FluentAssertions | Tests | — |

**Não presentes:** Serilog, Polly (além do retry do EF), MassTransit, Redis, OpenTelemetry.

---

## 4. Arquitetura atual

Clean Architecture com CQRS para o módulo Projects:

```text
Requisição HTTP
  → Controller
  → MediatR
  → ValidationBehavior (FluentValidation)
  → Handler
  → IProjectRepository
  → ProjectRepository / TaskForgeDbContext
  → PostgreSQL
```

Auth não usa MediatR — `AuthController` usa `UserManager` / `SignInManager` diretamente.

---

## 5. Exemplo de fluxo de requisição

**Criar projeto (autenticado):**

1. Cliente envia `POST /api/projects` com `Authorization: Bearer {token}`.
2. Middleware JWT valida token (issuer, audience, assinatura, lifetime).
3. `ProjectsController.Create` envia `CreateProjectCommand` via MediatR.
4. `CreateProjectCommandValidator` executa em `ValidationBehavior`.
5. `CreateProjectHandler` cria entidade `Project` e chama `IProjectRepository.AddAsync`.
6. Controller busca o projeto criado e retorna `201 Created`.

**Lacuna:** Handler não lê `User.Identity` — projetos não são associados ao usuário autenticado.

---

## 6. Autenticação

| Componente | Implementação |
|------------|---------------|
| Usuário Identity | `ApplicationUser` : `IdentityUser` + `FullName` |
| Política de senha | Dígito obrigatório, mínimo 6 caracteres, e-mail único |
| Claims JWT | `sub` (Id do usuário), `email` |
| Config JWT | `Jwt:Key` (obrigatório, fora de `appsettings.json`), `Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpireMinutes` (padrão 60) |
| Rotas protegidas | `[Authorize]` em `ProjectsController` |
| Rotas públicas | Register/login, health |

**Não implementado:** refresh tokens, confirmação de e-mail, policies por role, `ICurrentUserService`.

---

## 7. Persistência

### DbContext

`TaskForge.Infrastructure/Data/TaskForgeDbContext.cs`:

- `DbSet<Project> Projects`
- Herda tabelas Identity de `IdentityDbContext<ApplicationUser>`
- Sem configuração fluent ou overrides em `OnModelCreating`

### Migrations

| Migration | Tabelas |
|-----------|---------|
| `20250426203017_InitialCreate` | `Projects` (Id, Name, Description) |
| `20250428024230_InitialIdentityAndProjects` | Schema completo ASP.NET Identity |

### Repositório

`ProjectRepository`:

- `GetAllAsync` — retorna **todos** os projetos (sem filtro por usuário)
- `GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`

### Auto-migrate

Quando `ApplyMigrationsOnStartup=true` (padrão Docker), migrations rodam na inicialização.

---

## 8. Endpoints existentes

### Autenticação (`AuthController`)

| Método | Rota | Auth |
|--------|------|------|
| POST | `/api/auth/register` | Anônimo |
| POST | `/api/auth/login` | Anônimo |

### Projetos (`ProjectsController`)

| Método | Rota | Auth |
|--------|------|------|
| GET | `/api/projects` | JWT obrigatório |
| POST | `/api/projects` | JWT obrigatório |
| GET | `/api/projects/{id:guid}` | JWT obrigatório |
| PUT | `/api/projects/{id:guid}` | JWT obrigatório |
| DELETE | `/api/projects/{id:guid}` | JWT obrigatório |

### Health (`HealthController`)

| Método | Rota | Auth |
|--------|------|------|
| GET | `/health/live` | Anônimo |
| GET | `/health/ready` | Anônimo (verifica PostgreSQL) |

### Não implementado

Todos os endpoints de tarefas em `/api/projects/{projectId}/tasks`.

---

## 9. Validações

| Área | Mecanismo |
|------|-----------|
| Nome do projeto | Construtor de domínio + `UpdateDetails`; FluentValidation máx. 100 caracteres |
| Descrição do projeto | FluentValidation máx. 500 caracteres |
| DTOs de auth | Apenas regras de senha do Identity (sem validators FluentValidation) |
| Model binding | `InvalidModelStateResponseFactory` customizado → 400 ValidationProblemDetails |

Handler global mapeia `ValidationException` → 400, `KeyNotFoundException` → 404, demais → 500.

---

## 10. Testes

**Framework:** xUnit + Moq + FluentAssertions

| Classe de teste | Cobertura |
|-----------------|-----------|
| `DomainTests/ProjectTests.cs` | Validação de domínio (4 facts/theories) |
| `Application/Projects/Commands/CreateProjectHandlerTests.cs` | Handler de criação (5 testes) |

**Última execução verificada:** 13 testes aprovados, 0 falhas.

**Não coberto:** handlers update/delete/get, validators, auth, repositório, integração de API, ownership, tarefas.

---

## 11. Health checks

Registrados via `AddHealthChecks().AddNpgSql(...)` com tags `db`, `ready`.

Expostos por `HealthController` customizado (não `MapHealthChecks`):

- `/health/live` — sempre 200 se o processo está rodando
- `/health/ready` — 200 se PostgreSQL saudável, 503 caso contrário

`HEALTHCHECK` do Docker consulta `/health/live`.

---

## 12. Configuração

### `appsettings.json`

- `ConnectionStrings:Default` — padrões localhost PostgreSQL
- `Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpireMinutes`
- **Sem `Jwt:Key`** (deve ser fornecido externamente)

### Ambiente / secrets

| Chave | Fonte |
|-------|-------|
| `Jwt:Key` | User Secrets (local) ou `JWT_KEY` no `.env` (Docker) |
| `ConnectionStrings:Default` | `appsettings.json` ou `ConnectionStrings__Default` |
| `ApplyMigrationsOnStartup` | Docker compose define `true` |
| `EnableSwagger` | Docker compose define `true` |

### `.env.example`

Documenta `JWT_KEY`, credenciais PostgreSQL, configurações JWT opcionais, `API_PORT`.

---

## 13. Comandos de execução

### Docker (stack completa)

```powershell
cd TaskForge
copy .env.example .env
docker compose up -d --build
```

### Desenvolvimento local

```powershell
dotnet tool restore
dotnet user-secrets set "Jwt:Key" "<segredo-32-chars>" --project TaskForge.Api
docker compose up -d postgres
dotnet ef database update --project TaskForge.Infrastructure --startup-project TaskForge.Api
dotnet run --project TaskForge.Api
```

### Testes

```powershell
dotnet test TaskForge.sln
```

### Ferramentas EF

`dotnet-tools.json` fixa `dotnet-ef` 8.0.11.

---

## 14. Docker Compose

**Localização:** raiz da solution (`TaskForge/docker-compose.yml`) — não em `TaskForge.Api/`.

| Serviço | Função |
|---------|--------|
| `postgres` | PostgreSQL 15 Alpine, porta 5432, volume `pgdata` |
| `api` | Build via `Dockerfile`, porta `${API_PORT:-8080}` |
| `portainer` | UI de gerenciamento de containers (opcional) |

---

## 15. Problemas conhecidos e lacunas

| Problema | Severidade |
|----------|------------|
| Sem `OwnerId` em `Project` — todos os usuários autenticados veem todos os projetos | **Alta** — bloqueia aceite do MVP |
| Sem entidade `TaskItem` nem API de tarefas | **Alta** — bloqueia aceite do MVP |
| `Project` sem `CreatedAt`/`UpdatedAt` conforme spec do MVP | Média |
| `ApplicationUser` em Infrastructure, não em Domain | Baixa — escolha arquitetural |
| Sem testes de integração | Média |
| `ESTADO_DA_ESTRUTURA.md` desatualizado (Serilog, testes, caminho docker) | Apenas documentação |

---

## 16. Divergências: implementação vs escopo planejado do MVP

| Planejado | Real |
|-----------|------|
| `Project.OwnerId` | Ausente |
| Consultas de projeto por usuário | Retorna todos os projetos |
| `TaskItem` + enums + CRUD | Não iniciado |
| Regras `CompletedAt` / reabertura | Não iniciado |
| Exclusão em cascata de tarefas ao excluir projeto | N/A até existirem tarefas |
| Filtros de query (`?status=`, `?priority=`) | Não iniciado |
| Logging estruturado Serilog | Não implementado |

---

## 17. O que NÃO deve ser afirmado

Os itens abaixo **não estão implementados** e não devem ser documentados como presentes:

- Logging estruturado / Serilog
- Pipelines CI/CD
- Observabilidade avançada (métricas, tracing)
- Padrões de resiliência além do retry do EF
- Refresh tokens ou 2FA
- Controle de acesso baseado em roles
- Gerenciamento de tarefas
- Isolamento multi-tenant de dados
