# TaskForge

> **English version:** [README.en.md](README.en.md)

API Web .NET para gerenciamento de projetos, com autenticação JWT, Clean Architecture e CQRS via MediatR.

## Pré-requisitos

| Cenário | O que instalar |
|---------|----------------|
| **Rodar tudo com Docker (recomendado)** | [Docker Engine + Compose](#docker-no-windows-sem-desktop) |
| **Desenvolver código .NET localmente** | .NET 8 SDK + Docker (só para o banco) ou stack Docker completa |

---

## Deploy local com Docker (recomendado)

Sobe **PostgreSQL + API + Portainer** com um comando — como se fosse instalar no seu PC/servidor.

### 1. Instalar Docker

Veja [Docker no Windows (sem Desktop)](#docker-no-windows-sem-desktop) se ainda não tiver Docker Engine + Compose.

### 2. Configurar variáveis de ambiente

Na pasta raiz da solution (`TaskForge/`):

```powershell
copy .env.example .env
```

Edite o `.env` e defina uma chave JWT com **pelo menos 32 caracteres**:

```env
JWT_KEY=sua-chave-local-com-pelo-menos-32-caracteres
```

> O arquivo `.env` não é versionado. Nunca commite segredos reais.

### 3. Subir a stack completa

```powershell
cd TaskForge
docker compose up -d --build
```

Isso sobe:

| Serviço | Container | Acesso |
|---------|-----------|--------|
| API + Swagger | `taskforge-api` | **http://localhost:8080/swagger** |
| PostgreSQL | `taskforge-postgres` | `localhost:5432` |
| Portainer (UI) | `taskforge-portainer` | **http://localhost:9000** |

A API aplica as **migrations automaticamente** na inicialização (`ApplyMigrationsOnStartup`).

Verifique o status:

```powershell
docker compose ps
docker compose logs api
```

Parar tudo:

```powershell
docker compose down
```

Remover volumes (apaga o banco):

```powershell
docker compose down -v
```

### 4. Testar no Swagger

Abra **http://localhost:8080/swagger** e siga o [fluxo manual](#fluxo-manual-da-api-swagger).

### 5. Portainer

Abra **http://localhost:9000**, crie o usuário admin na primeira visita e selecione o ambiente **local**. Você verá os containers `taskforge-api`, `taskforge-postgres` e `taskforge-portainer`.

---

## Docker no Windows (sem Desktop)

O TaskForge **precisa de um engine Docker**. O **Portainer** é apenas a interface web — **não substitui** o Docker.

### Opção recomendada: Docker Engine no WSL2

1. **Habilite o WSL2** (PowerShell como administrador):

   ```powershell
   wsl --install
   ```

2. **Dentro do Ubuntu (WSL)**:

   ```bash
   sudo apt update
   sudo apt install -y docker.io docker-compose-v2
   sudo usermod -aG docker $USER
   ```

   Feche e reabra o terminal WSL. Verifique: `docker --version` e `docker compose version`.

3. Execute os comandos de [Deploy local com Docker](#deploy-local-com-docker-recomendado) a partir da pasta do projeto.

### Alternativa

[Docker Desktop](https://www.docker.com/products/docker-desktop/) — mais simples, porém mais pesado.

---

## Desenvolvimento .NET local (opcional)

Use este fluxo se quiser depurar código no Visual Studio / VS Code com `dotnet run`, mantendo o banco no Docker.

### 1. Ferramentas e JWT

```powershell
dotnet tool restore
dotnet user-secrets set "Jwt:Key" "TaskForge-Local-Dev-Secret-Key-32chars!" --project TaskForge.Api
```

### 2. Subir só a infraestrutura (PostgreSQL + Portainer)

```powershell
cd TaskForge
docker compose up -d postgres portainer
```

### 3. Migrations e API

```powershell
dotnet ef database update --project TaskForge.Infrastructure --startup-project TaskForge.Api
cd TaskForge.Api
dotnet run
```

Swagger local: `https://localhost:7294/swagger` ou `http://localhost:5062/swagger`

### 4. Testes

```powershell
dotnet test TaskForge.sln
```

---

## Fluxo manual da API (Swagger)

1. Abra o Swagger (`http://localhost:8080/swagger` no Docker, ou porta local do `dotnet run`).
2. **Registrar** — `POST /api/auth/register`:

   ```json
   {
     "fullName": "Usuário Demo",
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

   Copie o `token`.

4. **Autorizar** — clique em **Authorize** → `Bearer {token}`.
5. **Criar projeto** — `POST /api/projects`:

   ```json
   {
     "name": "Meu Primeiro Projeto",
     "description": "Demonstração do portfólio"
   }
   ```

6. **Listar** — `GET /api/projects`
7. **Buscar** — `GET /api/projects/{id}`
8. **Atualizar** — `PUT /api/projects/{id}`
9. **Excluir** — `DELETE /api/projects/{id}`

---

## Referência de configuração

| Chave | Docker (`.env`) | Local (`dotnet run`) |
|-------|-----------------|----------------------|
| JWT | `JWT_KEY` → `Jwt__Key` | User Secrets / `Jwt__Key` |
| Banco | automático via compose | `ConnectionStrings:Default` em `appsettings.json` |
| Swagger no container | `EnableSwagger=true` | `ASPNETCORE_ENVIRONMENT=Development` |
| Migrations automáticas | `ApplyMigrationsOnStartup=true` | `dotnet ef database update` manual |

Credenciais PostgreSQL padrão (somente dev local): `postgres` / `postgres` / `taskforge_db`.

---

## Health checks

- Liveness: `GET /health/live`
- Readiness (PostgreSQL): `GET /health/ready`

No Docker: `http://localhost:8080/health/ready`

---

## Arquitetura

```
TaskForge.Domain → TaskForge.Application → TaskForge.Infrastructure → TaskForge.Api
```

Arquivos de deploy: `Dockerfile`, `docker-compose.yml`, `.env.example` na raiz da solution.

---

## Tecnologias

- .NET 8, ASP.NET Core Web API, Docker
- Entity Framework Core 8 + PostgreSQL
- ASP.NET Core Identity + JWT Bearer
- MediatR, FluentValidation
- xUnit, Moq, FluentAssertions
