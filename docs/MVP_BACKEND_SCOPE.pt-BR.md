# TaskForge — Escopo do MVP do Backend

> **English:** [MVP_BACKEND_SCOPE.md](MVP_BACKEND_SCOPE.md)

Este documento define exatamente o que precisa ser concluído no MVP do backend antes de iniciar módulos futuros ou um frontend visual.

---

## 1. Objetivo do MVP

Oferecer uma **fundação técnica demonstrável** que permita a um usuário:

```text
criar conta → fazer login → receber JWT → criar projetos → criar tarefas nos projetos
→ alterar prioridade e status → concluir tarefas → acessar somente os próprios dados
```

O MVP não é o produto completo de longo prazo. Ele estabelece autenticação, propriedade dos dados e padrões de CRUD que os módulos futuros reutilizarão.

---

## 2. Módulos incluídos

```text
Identity
Projects
Tasks
```

---

## 3. Entidades

### 3.1 ApplicationUser

```text
ApplicationUser
├── Id
├── FullName
├── Email
├── PasswordHash
└── Projects (navegação, futuro)
```

**Nota sobre a implementação atual:** `ApplicationUser` está em `TaskForge.Infrastructure/Identity/`, estende `IdentityUser` e inclui `FullName`. Ainda não há navegação `Projects` nem `OwnerId` em `Project`.

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

**Nota sobre a implementação atual:** Apenas `Id`, `Name` e `Description` existem hoje. `OwnerId`, timestamps e coleção `Tasks` **não estão implementados**.

### 3.3 TaskItem

Utilizar o nome `TaskItem`, evitando conflito com `System.Threading.Tasks.Task`.

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

**Nota sobre a implementação atual:** **Não implementado** — sem entidade, migration ou API.

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

## 4. Regras de negócio

### 4.1 Projetos

| Regra | Requisito do MVP | Estado atual |
|-------|------------------|--------------|
| Todo projeto pertence a um usuário | Obrigatório | **Não implementado** — sem `OwnerId` |
| Nome é obrigatório | Obrigatório | **Implementado** (domínio + FluentValidation) |
| Nome possui limite de caracteres | Obrigatório | **Implementado** (máx. 100 via FluentValidation) |
| Descrição é opcional | Obrigatório | **Implementado** |
| Usuário vê apenas os próprios projetos | Obrigatório | **Não implementado** — `GetAllAsync` retorna todos |
| Usuário edita apenas os próprios projetos | Obrigatório | **Não implementado** |
| Usuário exclui apenas os próprios projetos | Obrigatório | **Não implementado** |
| Excluir projeto exclui suas tarefas | Obrigatório | **Ainda não aplicável** — sem tarefas |

### 4.2 Tarefas

| Regra | Requisito do MVP | Estado atual |
|-------|------------------|--------------|
| Toda tarefa pertence a um projeto | Obrigatório | **Não implementado** |
| Título é obrigatório | Obrigatório | **Não implementado** |
| Descrição é opcional | Obrigatório | **Não implementado** |
| Prioridade padrão é `Medium` | Obrigatório | **Não implementado** |
| Status inicial é `Pending` | Obrigatório | **Não implementado** |
| `Completed` preenche `CompletedAt` | Obrigatório | **Não implementado** |
| Reabrir limpa `CompletedAt` | Obrigatório | **Não implementado** |
| Usuário acessa tarefas apenas pelos próprios projetos | Obrigatório | **Não implementado** |

---

## 5. Endpoints

### 5.1 Autenticação

| Método | Rota | Estado atual |
|--------|------|--------------|
| POST | `/api/auth/register` | **Implementado** |
| POST | `/api/auth/login` | **Implementado** |

### 5.2 Projetos

| Método | Rota | Estado atual |
|--------|------|--------------|
| GET | `/api/projects` | **Implementado** (sem filtro por usuário) |
| POST | `/api/projects` | **Implementado** (sem atribuição de dono) |
| GET | `/api/projects/{id}` | **Implementado** |
| PUT | `/api/projects/{id}` | **Implementado** |
| DELETE | `/api/projects/{id}` | **Implementado** |

### 5.3 Tarefas

| Método | Rota | Estado atual |
|--------|------|--------------|
| GET | `/api/projects/{projectId}/tasks` | **Não implementado** |
| POST | `/api/projects/{projectId}/tasks` | **Não implementado** |
| GET | `/api/projects/{projectId}/tasks/{taskId}` | **Não implementado** |
| PUT | `/api/projects/{projectId}/tasks/{taskId}` | **Não implementado** |
| DELETE | `/api/projects/{projectId}/tasks/{taskId}` | **Não implementado** |

### 5.4 Filtros opcionais (após CRUD)

| Método | Rota | Estado atual |
|--------|------|--------------|
| GET | `/api/projects/{projectId}/tasks?status=InProgress` | **Planejado** |
| GET | `/api/projects/{projectId}/tasks?priority=High` | **Planejado** |

---

## 6. Critérios de aceite

O MVP está concluído quando este cenário funcionar de ponta a ponta:

```text
1.  Cadastrar usuário A
2.  Fazer login como usuário A
3.  Criar projeto "Candidatura SENAI"
4.  Criar tarefa "Preparar aula-teste"
5.  Definir prioridade High
6.  Alterar status para InProgress
7.  Listar tarefas do projeto
8.  Concluir tarefa
9.  Cadastrar usuário B
10. Fazer login como usuário B
11. Confirmar que usuário B não consegue acessar o projeto do usuário A
12. Confirmar que usuário B não consegue acessar tarefas do projeto do usuário A
```

**Estado atual:** Os passos 1–3 e listagem de projetos funcionam. Os passos 4–12 **ainda não são possíveis** (sem tarefas, sem isolamento por usuário).

---

## 7. Requisitos técnicos

| Requisito | Estado atual |
|-----------|--------------|
| .NET 8, ASP.NET Core Web API | **Implementado** |
| Clean Architecture (Domain, Application, Infrastructure, Api) | **Implementado** |
| PostgreSQL + EF Core | **Implementado** |
| ASP.NET Core Identity + JWT | **Implementado** |
| MediatR para commands/queries | **Implementado** (apenas Projects) |
| Pipeline FluentValidation | **Implementado** (comandos de Project) |
| Swagger com Bearer auth | **Implementado** |
| Health checks (`/health/live`, `/health/ready`) | **Implementado** |
| Tratamento global de erros | **Implementado** |
| Docker Compose (PostgreSQL + API) | **Implementado** |
| Migrations | **Implementado** (Identity + Projects) |
| Segredo JWT fora de arquivos versionados | **Implementado** (user-secrets / `.env`) |
| Logging estruturado (Serilog) | **Não implementado** — usa logging padrão do ASP.NET |
| Pipeline CI/CD | **Não implementado** |
| Refresh tokens | **Fora do escopo do MVP** |

---

## 8. Checklist de implementação

O progresso usa apenas itens verificáveis. Itens parciais contam como 0.

### 8.1 Identity (8 itens)

| # | Item | Feito |
|---|------|-------|
| 1 | Endpoint de registro | ✓ |
| 2 | Endpoint de login | ✓ |
| 3 | Hash de senha via Identity | ✓ |
| 4 | Geração de JWT | ✓ |
| 5 | Validação de JWT | ✓ |
| 6 | Identificação do usuário atual na lógica de negócio | ✗ |
| 7 | Swagger Bearer authentication | ✓ |
| 8 | Configuração sensível fora de arquivos versionados | ✓ |

**Progresso: 6 / 8 = 75%**

### 8.2 Projects (12 itens)

| # | Item | Feito |
|---|------|-------|
| 1 | Entidade Project | ✓ |
| 2 | Criar projeto | ✓ |
| 3 | Listar projetos | ✓ |
| 4 | Buscar projeto por id | ✓ |
| 5 | Atualizar projeto | ✓ |
| 6 | Excluir projeto | ✓ |
| 7 | OwnerId | ✗ |
| 8 | Listar apenas projetos do usuário atual | ✗ |
| 9 | Bloquear acesso a projeto de outro usuário | ✗ |
| 10 | Exclusão em cascata de tarefas | ✗ |
| 11 | Validação | ✓ |
| 12 | Testes unitários (cobertura do módulo) | ✗ |

**Progresso: 7 / 12 = 58%**

### 8.3 Tasks (14 itens)

| # | Item | Feito |
|---|------|-------|
| 1 | Entidade TaskItem | ✗ |
| 2 | Enum TaskItemStatus | ✗ |
| 3 | Enum TaskItemPriority | ✗ |
| 4 | Criar tarefa | ✗ |
| 5 | Listar tarefas por projeto | ✗ |
| 6 | Buscar tarefa por id | ✗ |
| 7 | Atualizar tarefa | ✗ |
| 8 | Alterar status | ✗ |
| 9 | Definir prioridade | ✗ |
| 10 | Excluir tarefa | ✗ |
| 11 | Regra CompletedAt | ✗ |
| 12 | Regra de reabertura | ✗ |
| 13 | Bloquear acesso via projeto de outro usuário | ✗ |
| 14 | Validação + testes unitários | ✗ |

**Progresso: 0 / 14 = 0%**

### 8.4 Persistência e infraestrutura (11 itens)

| # | Item | Feito |
|---|------|-------|
| 1 | Configuração PostgreSQL | ✓ |
| 2 | EF Core | ✓ |
| 3 | DbContext | ✓ |
| 4 | Migrations | ✓ |
| 5 | Credenciais locais consistentes | ✓ |
| 6 | Docker Compose para PostgreSQL | ✓ |
| 7 | Endpoint health live | ✓ |
| 8 | Endpoint health ready | ✓ |
| 9 | Tratamento global de erros | ✓ |
| 10 | Swagger | ✓ |
| 11 | Execução local documentada | ✓ |

**Progresso: 11 / 11 = 100%**

### 8.5 MVP geral (módulos centrais + infraestrutura)

```text
(6 + 7 + 0 + 11) / (8 + 12 + 14 + 11) = 24 / 45 = 53%
```

---

## 9. Fora do escopo (MVP atual)

Estes itens pertencem **apenas ao roadmap futuro**:

- Frontend
- Inbox
- Hábitos
- Rotinas
- Tracker
- Listas livres
- Calendário
- Notas
- Dashboard
- Insights
- Assistência por IA
- Colaboração entre usuários, convites, comentários, anexos
- Notificações
- Gamificação
- Microsserviços
- Refresh token
- Roles complexas
- CI/CD completo
- Analytics avançado

---

## 10. Divergências com documentação anterior

| Afirmação anterior | Estado real (código é fonte da verdade) |
|--------------------|----------------------------------------|
| `ESTADO_DA_ESTRUTURA.md` lista Serilog como implementado | Apenas logging padrão do ASP.NET |
| `ESTADO_DA_ESTRUTURA.md` marca testes unitários como pendentes | 13 testes unitários existem e passam |
| `ESTADO_DA_ESTRUTURA.md` coloca `docker-compose.yml` em `TaskForge.Api/` | Arquivo está na raiz da solution |
| `ESTADO_DA_ESTRUTURA.md` marca containerização como pendente | Stack Docker Compose já existe |
| READMEs sugerem API de gerenciamento de projetos | Correto para projetos/auth; tarefas nunca foram implementadas |
