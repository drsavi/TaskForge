# TaskForge — Status de Implementação

> **English:** [IMPLEMENTATION_STATUS.md](IMPLEMENTATION_STATUS.md)

O progresso é calculado apenas a partir de **itens verificáveis em checklist**. Itens parciais contam como 0 no percentual principal. Fórmula: `concluídos / total × 100`, arredondado para o inteiro mais próximo.

**Última verificação:** 2026-06-07 — revisão de código + `dotnet test` (13 aprovados).

---

## Percentuais resumo

| Área | Concluídos | Total | Progresso |
|------|------------|-------|-----------|
| **MVP — Identity** | 6 | 8 | **75%** |
| **MVP — Projects** | 7 | 12 | **58%** |
| **MVP — Tasks** | 0 | 14 | **0%** |
| **MVP — Infraestrutura** | 11 | 11 | **100%** |
| **MVP geral** (Identity + Projects + Tasks + Infraestrutura) | 24 | 45 | **53%** |
| **Documentação** | 10 | 10 | **100%** |
| **Testes** | 3 | 8 | **38%** |

---

## MVP — Identity

| Área | Funcionalidade | Status | Progresso | Evidência | No MVP atual | Observações |
|------|----------------|--------|-----------|-----------|--------------|-------------|
| Identity | Endpoint de registro | Implementado | 75% | `AuthController.Register` | Sim | Retorna 400 em erros do Identity |
| Identity | Endpoint de login | Implementado | 75% | `AuthController.Login` | Sim | Retorna JWT `{ token }` |
| Identity | Hash de senha via Identity | Implementado | 75% | `UserManager.CreateAsync` | Sim | Dígito obrigatório, mín. 6 chars |
| Identity | Geração de JWT | Implementado | 75% | `AuthController.GenerateJwt` | Sim | Claims: sub, email |
| Identity | Validação de JWT | Implementado | 75% | `Program.cs` AddJwtBearer | Sim | Issuer, audience, lifetime |
| Identity | Usuário atual na lógica de negócio | Parcialmente implementado | 75% | Apenas middleware JWT | Sim | Handlers não leem Id do usuário |
| Identity | Swagger Bearer auth | Implementado | 75% | `Program.cs` AddSecurityDefinition | Sim | — |
| Identity | Config sensível fora do repo | Implementado | 75% | Sem `Jwt:Key` em appsettings | Sim | user-secrets / `.env` |

---

## MVP — Projects

| Área | Funcionalidade | Status | Progresso | Evidência | No MVP atual | Observações |
|------|----------------|--------|-----------|-----------|--------------|-------------|
| Projects | Entidade Project | Implementado | 58% | `Domain/Entities/Project.cs` | Sim | Sem OwnerId, timestamps |
| Projects | Criar projeto | Implementado | 58% | POST `/api/projects` | Sim | — |
| Projects | Listar projetos | Implementado | 58% | GET `/api/projects` | Sim | Sem filtro por usuário |
| Projects | Buscar por id | Implementado | 58% | GET `/api/projects/{id}` | Sim | — |
| Projects | Atualizar projeto | Implementado | 58% | PUT `/api/projects/{id}` | Sim | — |
| Projects | Excluir projeto | Implementado | 58% | DELETE `/api/projects/{id}` | Sim | — |
| Projects | OwnerId | Planejado para o MVP atual | 58% | Ausente na entidade/migration | Sim | Obrigatório para aceite |
| Projects | Listar apenas próprios | Planejado para o MVP atual | 58% | `GetAllAsync` retorna todos | Sim | — |
| Projects | Bloquear projeto de outro usuário | Planejado para o MVP atual | 58% | Sem verificação de ownership | Sim | — |
| Projects | Exclusão em cascata de tarefas | Planejado para o MVP atual | 58% | N/A — sem tarefas | Sim | Bloqueado até Tasks existir |
| Projects | Validação | Implementado | 58% | FluentValidation + domínio | Sim | Nome máx. 100, desc máx. 500 |
| Projects | Testes unitários (módulo) | Parcialmente implementado | 58% | Apenas CreateProjectHandler | Sim | Update/delete/get sem testes |

---

## MVP — Tasks

| Área | Funcionalidade | Status | Progresso | Evidência | No MVP atual | Observações |
|------|----------------|--------|-----------|-----------|--------------|-------------|
| Tasks | Entidade TaskItem | Planejado para o MVP atual | 0% | Sem arquivo no código | Sim | — |
| Tasks | Enum TaskItemStatus | Planejado para o MVP atual | 0% | Não encontrado | Sim | Pending, InProgress, Completed |
| Tasks | Enum TaskItemPriority | Planejado para o MVP atual | 0% | Não encontrado | Sim | Low, Medium, High |
| Tasks | Criar tarefa | Planejado para o MVP atual | 0% | Sem controller | Sim | — |
| Tasks | Listar por projeto | Planejado para o MVP atual | 0% | Sem endpoint | Sim | — |
| Tasks | Buscar por id | Planejado para o MVP atual | 0% | Sem endpoint | Sim | — |
| Tasks | Atualizar tarefa | Planejado para o MVP atual | 0% | Sem endpoint | Sim | — |
| Tasks | Alterar status | Planejado para o MVP atual | 0% | Sem endpoint | Sim | — |
| Tasks | Definir prioridade | Planejado para o MVP atual | 0% | Sem endpoint | Sim | — |
| Tasks | Excluir tarefa | Planejado para o MVP atual | 0% | Sem endpoint | Sim | — |
| Tasks | Regra CompletedAt | Planejado para o MVP atual | 0% | Não encontrado | Sim | — |
| Tasks | Regra de reabertura | Planejado para o MVP atual | 0% | Não encontrado | Sim | — |
| Tasks | Bloquear via projeto alheio | Planejado para o MVP atual | 0% | Não encontrado | Sim | Depende de ownership |
| Tasks | Validação + testes unitários | Planejado para o MVP atual | 0% | Não encontrado | Sim | — |

---

## MVP — Infraestrutura

| Área | Funcionalidade | Status | Progresso | Evidência | No MVP atual | Observações |
|------|----------------|--------|-----------|-----------|--------------|-------------|
| Infra | Configuração PostgreSQL | Implementado | 100% | Npgsql em Program.cs | Sim | Retry on failure habilitado |
| Infra | EF Core | Implementado | 100% | csproj Infrastructure | Sim | v8.0.11 |
| Infra | DbContext | Implementado | 100% | `TaskForgeDbContext.cs` | Sim | Identity + Projects |
| Infra | Migrations | Implementado | 100% | 2 migrations | Sim | — |
| Infra | Credenciais locais consistentes | Implementado | 100% | appsettings + .env.example | Sim | postgres/postgres/taskforge_db |
| Infra | Docker Compose | Implementado | 100% | `docker-compose.yml` | Sim | postgres + api + portainer |
| Infra | Endpoint health live | Implementado | 100% | `HealthController` | Sim | GET /health/live |
| Infra | Endpoint health ready | Implementado | 100% | Health check NpgSql | Sim | GET /health/ready |
| Infra | Tratamento global de erros | Implementado | 100% | `Program.cs` UseExceptionHandler | Sim | Mapeamento 400/404/500 |
| Infra | Swagger | Implementado | 100% | AddSwaggerGen + UI | Sim | Esquema Bearer |
| Infra | Execução local documentada | Implementado | 100% | README + TECHNICAL_OVERVIEW | Sim | — |

---

## Módulos futuros

| Área | Funcionalidade | Status | Progresso | Evidência | No MVP atual | Observações |
|------|----------------|--------|-----------|-----------|--------------|-------------|
| Inbox | Módulo | Planejado para versão futura | 0% | Sem código | Não | Após MVP + Habits no roadmap |
| Habits | Módulo | Planejado para versão futura | 0% | Sem código | Não | Primeiro módulo pós-MVP |
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
| Docs | IMPLEMENTATION_STATUS EN/PT | Implementado | 100% | Este arquivo | Sim | — |

---

## Testes

| Área | Funcionalidade | Status | Progresso | Evidência | No MVP atual | Observações |
|------|----------------|--------|-----------|-----------|--------------|-------------|
| Testes | Solution compila | Implementado | 38% | `dotnet test` com sucesso | Sim | — |
| Testes | Testes existentes passam | Implementado | 38% | 13/13 aprovados | Sim | 2026-06-07 |
| Testes | Testes unitários de domínio | Implementado | 38% | `ProjectTests.cs` | Sim | 4 métodos de teste |
| Testes | Testes unitários do MVP (cobertura completa) | Parcialmente implementado | 38% | Apenas CreateProjectHandler | Sim | Faltam tarefas, ownership |
| Testes | Testes de integração | Planejado para o MVP atual | 38% | Nenhum | Sim | Cenário de aceite precisa deles |
| Testes | Testes de auth | Planejado para o MVP atual | 38% | Nenhum | Sim | — |
| Testes | Testes de autorização por ownership | Planejado para o MVP atual | 38% | Nenhum | Sim | — |
| Testes | Validação manual ponta a ponta | Planejado para o MVP atual | 38% | Não registrada | Sim | Bloqueada por ausência de tarefas |

**Progresso de testes: 3 / 8 = 38%**

---

## Status dos critérios de aceite

| Passo | Descrição | Status |
|-------|-----------|--------|
| 1–2 | Cadastrar e logar usuário A | **OK** |
| 3 | Criar projeto | **OK** |
| 4–8 | CRUD e conclusão de tarefa | **Falha** — sem API de tarefas |
| 9–10 | Cadastrar e logar usuário B | **OK** |
| 11–12 | Usuário B não acessa dados do usuário A | **Falha** — sem ownership |

**Cenário de aceite do MVP: não concluído.**
