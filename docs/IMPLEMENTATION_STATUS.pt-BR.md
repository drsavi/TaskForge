# TaskForge — Status de Implementação

> **English:** [IMPLEMENTATION_STATUS.md](IMPLEMENTATION_STATUS.md)

O progresso é calculado apenas a partir de **itens verificáveis em checklist**. Itens parciais contam como 0 no percentual principal. Fórmula: `concluídos / total × 100`, arredondado para o inteiro mais próximo.

**Última verificação:** 2026-06-07 — documentação GIT_WORKFLOW adicionada; MVP verificado em 2026-06-08 (`dotnet test` 28 aprovados, aceite 12/12).

---

## Percentuais resumo

| Área | Concluídos | Total | Progresso |
|------|------------|-------|-----------|
| **MVP — Identity** | 7 | 8 | **88%** |
| **MVP — Projects** | 11 | 12 | **92%** |
| **MVP — Tasks** | 13 | 14 | **93%** |
| **MVP — Infraestrutura** | 11 | 11 | **100%** |
| **MVP geral** (Identity + Projects + Tasks + Infraestrutura) | 42 | 45 | **93%** |
| **Documentação** | 12 | 12 | **100%** |
| **Testes** | 5 | 8 | **63%** |

---

## MVP — Identity

| Área | Funcionalidade | Status | Progresso | Evidência | No MVP atual | Observações |
|------|----------------|--------|-----------|-----------|--------------|-------------|
| Identity | Endpoint de registro | Implementado | 88% | `AuthController.Register` | Sim | Retorna 400 em erros do Identity |
| Identity | Endpoint de login | Implementado | 88% | `AuthController.Login` | Sim | Retorna JWT `{ token }` |
| Identity | Hash de senha via Identity | Implementado | 88% | `UserManager.CreateAsync` | Sim | Dígito + não alfanumérico obrigatórios |
| Identity | Geração de JWT | Implementado | 88% | `AuthController.GenerateJwt` | Sim | Claims: sub, email |
| Identity | Validação de JWT | Implementado | 88% | `Program.cs` AddJwtBearer | Sim | Issuer, audience, lifetime |
| Identity | Usuário atual na lógica de negócio | Implementado | 88% | `ICurrentUserService` nos handlers | Sim | `ClaimTypes.NameIdentifier` |
| Identity | Swagger Bearer auth | Implementado | 88% | `Program.cs` AddSecurityDefinition | Sim | — |
| Identity | Config sensível fora do repo | Implementado | 88% | Sem `Jwt:Key` em appsettings | Sim | user-secrets / `.env` |

---

## MVP — Projects

| Área | Funcionalidade | Status | Progresso | Evidência | No MVP atual | Observações |
|------|----------------|--------|-----------|-----------|--------------|-------------|
| Projects | Entidade Project | Implementado | 92% | `Domain/Entities/Project.cs` | Sim | OwnerId, CreatedAt, UpdatedAt |
| Projects | Criar projeto | Implementado | 92% | POST `/api/projects` | Sim | OwnerId do usuário atual |
| Projects | Listar projetos | Implementado | 92% | GET `/api/projects` | Sim | `GetAllForOwnerAsync` |
| Projects | Buscar por id | Implementado | 92% | GET `/api/projects/{id}` | Sim | Consulta por dono |
| Projects | Atualizar projeto | Implementado | 92% | PUT `/api/projects/{id}` | Sim | — |
| Projects | Excluir projeto | Implementado | 92% | DELETE `/api/projects/{id}` | Sim | — |
| Projects | OwnerId | Implementado | 92% | Entidade + migration `AddOwnerIdAndTaskItems` | Sim | FK para AspNetUsers |
| Projects | Listar apenas próprios | Implementado | 92% | `GetAllForOwnerAsync` | Sim | — |
| Projects | Bloquear projeto de outro usuário | Implementado | 92% | Retorna 404 para outros donos | Sim | Verificado no passo 11 do aceite |
| Projects | Exclusão em cascata de tarefas | Implementado | 92% | EF `OnDelete(DeleteBehavior.Cascade)` | Sim | TaskItems excluídas com o projeto |
| Projects | Validação | Implementado | 92% | FluentValidation + domínio | Sim | Nome máx. 100, desc máx. 500 |
| Projects | Testes unitários (módulo) | Parcialmente implementado | 92% | Handlers Create/Update/Delete/GetById | Sim | Faltam GetAll + validators |

---

## MVP — Tasks

| Área | Funcionalidade | Status | Progresso | Evidência | No MVP atual | Observações |
|------|----------------|--------|-----------|-----------|--------------|-------------|
| Tasks | Entidade TaskItem | Implementado | 93% | `Domain/Entities/TaskItem.cs` | Sim | — |
| Tasks | Enum TaskItemStatus | Implementado | 93% | `Domain/Enums/TaskItemStatus.cs` | Sim | Pending, InProgress, Completed |
| Tasks | Enum TaskItemPriority | Implementado | 93% | `Domain/Enums/TaskItemPriority.cs` | Sim | Low, Medium, High |
| Tasks | Criar tarefa | Implementado | 93% | POST `/api/projects/{id}/tasks` | Sim | — |
| Tasks | Listar por projeto | Implementado | 93% | GET `/api/projects/{id}/tasks` | Sim | Filtros opcionais `?status=` `?priority=` |
| Tasks | Buscar por id | Implementado | 93% | GET `.../tasks/{taskId}` | Sim | — |
| Tasks | Atualizar tarefa | Implementado | 93% | PUT `.../tasks/{taskId}` | Sim | — |
| Tasks | Alterar status | Implementado | 93% | `UpdateTaskHandler` + `ChangeStatus` | Sim | — |
| Tasks | Definir prioridade | Implementado | 93% | `UpdateTaskHandler` | Sim | — |
| Tasks | Excluir tarefa | Implementado | 93% | DELETE `.../tasks/{taskId}` | Sim | — |
| Tasks | Regra CompletedAt | Implementado | 93% | `TaskItem.ChangeStatus` | Sim | `TaskItemTests` |
| Tasks | Regra de reabertura | Implementado | 93% | Limpa `CompletedAt` ao reabrir | Sim | `UpdateTaskHandlerTests` |
| Tasks | Bloquear via projeto alheio | Implementado | 93% | `ProjectAuthorization` + 404 | Sim | Verificado no passo 12 do aceite |
| Tasks | Validação + testes unitários | Parcialmente implementado | 93% | Validators + 3 classes de teste | Sim | Faltam handlers Delete/Get/List |

---

## MVP — Infraestrutura

| Área | Funcionalidade | Status | Progresso | Evidência | No MVP atual | Observações |
|------|----------------|--------|-----------|-----------|--------------|-------------|
| Infra | Configuração PostgreSQL | Implementado | 100% | Npgsql em Program.cs | Sim | Retry on failure habilitado |
| Infra | EF Core | Implementado | 100% | csproj Infrastructure | Sim | v8.0.11 |
| Infra | DbContext | Implementado | 100% | `TaskForgeDbContext.cs` | Sim | Identity + Projects + TaskItems |
| Infra | Migrations | Implementado | 100% | 3 migrations | Sim | Inclui `AddOwnerIdAndTaskItems` |
| Infra | Credenciais locais consistentes | Implementado | 100% | appsettings + `.env.example` | Sim | postgres/postgres/taskforge_db |
| Infra | Docker Compose | Implementado | 100% | `docker-compose.yml` | Sim | postgres + api + portainer |
| Infra | Endpoint health live | Implementado | 100% | `HealthController` | Sim | GET /health/live |
| Infra | Endpoint health ready | Implementado | 100% | Health check NpgSql | Sim | GET /health/ready |
| Infra | Tratamento global de erros | Implementado | 100% | `Program.cs` UseExceptionHandler | Sim | Mapeamento 400/403/404/500 |
| Infra | Swagger | Implementado | 100% | AddSwaggerGen + UI | Sim | Esquema Bearer |
| Infra | Execução local documentada | Implementado | 100% | README + TECHNICAL_OVERVIEW | Sim | — |

---

## Módulos futuros

| Área | Funcionalidade | Status | Progresso | Evidência | No MVP atual | Observações |
|------|----------------|--------|-----------|-----------|--------------|-------------|
| Inbox | Módulo | Planejado para versão futura | 0% | Sem código | Não | Primeiro item do roadmap pós-MVP |
| Habits | Módulo | Planejado para versão futura | 0% | Sem código | Não | — |
| Routines | Módulo | Planejado para versão futura | 0% | Sem código | Não | — |
| Tracker | Módulo | Planejado para versão futura | 0% | Sem código | Não | — |
| Lists | Módulo | Planejado para versão futura | 0% | Sem código | Não | — |
| Calendar | Módulo | Planejado para versão futura | 0% | Sem código | Não | — |
| Notes | Módulo | Planejado para versão futura | 0% | Sem código | Não | — |
| Dashboard | Módulo | Planejado para versão futura | 0% | Sem código | Não | — |
| Insights | Módulo | Planejado para versão futura | 0% | Sem código | Não | — |
| AI Assistance | Módulo | Planejado para versão futura | 0% | Sem código | Não | — |

---

## Documentação

| Área | Funcionalidade | Status | Progresso | Evidência | No MVP atual | Observações |
|------|----------------|--------|-----------|-----------|--------------|-------------|
| Docs | README.md (inglês) | Implementado | 100% | `README.md` | Sim | Foco em produto |
| Docs | README.pt-BR.md | Implementado | 100% | `README.pt-BR.md` | Sim | — |
| Docs | TECHNICAL_OVERVIEW EN/PT | Implementado | 100% | `docs/TECHNICAL_OVERVIEW*.md` | Sim | — |
| Docs | MVP_BACKEND_SCOPE EN/PT | Implementado | 100% | `docs/MVP_BACKEND_SCOPE*.md` | Sim | — |
| Docs | PRODUCT_ROADMAP EN/PT | Implementado | 100% | `docs/PRODUCT_ROADMAP*.md` | Sim | — |
| Docs | GIT_WORKFLOW EN/PT | Implementado | 100% | `docs/GIT_WORKFLOW*.md` | Sim | Branches, commits, fluxo de merge |
| Docs | IMPLEMENTATION_STATUS EN/PT | Implementado | 100% | Este arquivo | Sim | Atualizado em 2026-06-07 |

---

## Testes

| Área | Funcionalidade | Status | Progresso | Evidência | No MVP atual | Observações |
|------|----------------|--------|-----------|-----------|--------------|-------------|
| Testes | Solution compila | Implementado | 63% | `dotnet build` com sucesso | Sim | — |
| Testes | Testes existentes passam | Implementado | 63% | 28/28 aprovados | Sim | 2026-06-08 |
| Testes | Testes unitários de domínio | Implementado | 63% | `ProjectTests` + `TaskItemTests` | Sim | 9 métodos de teste |
| Testes | Testes unitários do MVP (cobertura completa) | Parcialmente implementado | 63% | 7 classes de teste de handlers | Sim | Faltam auth e alguns handlers de tasks |
| Testes | Testes de integração | Planejado para o MVP atual | 63% | Nenhum | Sim | — |
| Testes | Testes de auth | Planejado para o MVP atual | 63% | Nenhum | Sim | — |
| Testes | Testes de autorização por ownership | Implementado | 63% | Testes de handlers Project + Task | Sim | Nível handler, não integração de API |
| Testes | Validação manual ponta a ponta | Implementado | 63% | 12/12 passos do aceite | Sim | 2026-06-08 via HTTP |

**Progresso de testes: 5 / 8 = 63%**

---

## Status dos critérios de aceite

| Passo | Descrição | Status |
|-------|-----------|--------|
| 1–2 | Cadastrar e logar usuário A | **OK** |
| 3 | Criar projeto "Candidatura SENAI" | **OK** |
| 4 | Criar tarefa "Preparar aula-teste" | **OK** |
| 5 | Definir prioridade High | **OK** |
| 6 | Alterar status para InProgress | **OK** |
| 7 | Listar tarefas do projeto | **OK** |
| 8 | Concluir tarefa (`CompletedAt` preenchido) | **OK** |
| 9–10 | Cadastrar e logar usuário B | **OK** |
| 11 | Usuário B não acessa projeto do usuário A | **OK** (404) |
| 12 | Usuário B não acessa tarefas do usuário A | **OK** (404) |

**Cenário de aceite do MVP: concluído.**
