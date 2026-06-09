# TaskForge

> **English:** [README.md](README.md)

> **Status:** Este projeto está em desenvolvimento ativo. O MVP atual do backend ainda não está completo.

O TaskForge é uma plataforma pessoal de organização modular criada para centralizar projetos, tarefas, hábitos, rotinas, listas, leituras, estudos e planejamento cotidiano com o mínimo possível de atrito.

Inspirado em ferramentas como Notion, aplicativos de hábitos, calendários e quadros Kanban, o projeto busca oferecer uma alternativa mais estruturada e rápida de manter. Seu objetivo é ajudar o usuário a capturar demandas, priorizar atividades, visualizar progresso e, futuramente, compreender padrões pessoais a partir do histórico registrado.

O backend está sendo desenvolvido como um **monólito modular** com ASP.NET Core e princípios de Clean Architecture. O MVP atual estabelece a base da plataforma por meio de autenticação, gerenciamento de projetos, tarefas e autorização por usuário.

---

## Problema

Manter um sistema pessoal de produtividade costuma exigir manutenção manual excessiva: duplicar informações entre listas, calendários e projetos; decidir metadados antes de capturar uma ideia; e perder visibilidade do que realmente importa hoje.

O TaskForge busca reduzir essa carga mental oferecendo flexibilidade suficiente para acompanhar a vida real, mas estrutura suficiente para não exigir reconfiguração constante.

---

## Visão do produto

Com o tempo, o TaskForge deve funcionar como um **sistema operacional pessoal para organização cotidiana**, ajudando o usuário a compreender rapidamente:

- o que precisa ser feito hoje;
- o que está atrasado ou é prioridade;
- o que cabe no tempo e na energia disponíveis;
- quais projetos estão parados;
- quais hábitos e rotinas estão sendo mantidos;
- quais conteúdos de aprendizado estão em andamento;
- quais padrões emergem do histórico pessoal.

---

## Princípios

| Princípio | Resumo |
|-----------|--------|
| **Baixa fricção** | Capturar uma tarefa ou hábito deve levar segundos, não um formulário. |
| **Capturar primeiro, organizar depois** | Apenas o título é obrigatório na captura rápida; metadados podem esperar. |
| **Cadastrar uma única vez** | Uma tarefa com prazo deve aparecer no projeto, na lista, no calendário e em “Hoje” sem duplicação. |
| **Progresso visual satisfatório** | Listas e estados de conclusão devem ser gratificantes e legíveis. |
| **Dias de baixa energia** | Visualizações futuras destacarão o essencial, vitórias rápidas e rotinas mínimas. |
| **Histórico preservado** | Timestamps e dados históricos são mantidos desde o início para análises futuras. |

---

## Arquitetura (em uma frase)

Uma aplicação ASP.NET Core única, organizada em módulos de domínio (Identity, Projects, Tasks e áreas futuras), com baixo acoplamento entre responsabilidades — não microsserviços.

---

## Estado atual

| Área | Status |
|------|--------|
| **Identity** | Registro, login, JWT e Swagger Bearer implementados. Isolamento de dados por usuário **ainda não** conectado à lógica de negócio. |
| **Projects** | CRUD completo via MediatR, com FluentValidation. Faltam `OwnerId`, filtro por usuário e autorização por propriedade. |
| **Tasks** | **Não iniciado** — sem entidade `TaskItem`, enums ou endpoints de tarefas. |
| **Infraestrutura** | PostgreSQL, EF Core, migrations, Docker Compose, health checks, Swagger e tratamento global de erros implementados. |
| **Testes** | 13 testes unitários passam (domínio `Project` + `CreateProjectHandler`). Sem testes de integração ou de ownership. |

Consulte [Status de Implementação](docs/IMPLEMENTATION_STATUS.pt-BR.md) para progresso item a item e percentuais calculados.

---

## Escopo do MVP atual (backend)

O primeiro MVP é uma **fundação técnica demonstrável**, não o produto completo de longo prazo.

**Módulos incluídos:** Identity, Projects, Tasks.

**Fluxo alvo:**

```text
criar conta → login → receber JWT → criar projetos → criar tarefas nos projetos
→ alterar prioridade e status → concluir tarefas → acessar somente os próprios dados
```

**Implementado hoje:** autenticação e CRUD de projetos. Tarefas e autorização por usuário permanecem pendentes.

Detalhes: [Escopo do MVP do Backend](docs/MVP_BACKEND_SCOPE.pt-BR.md).

---

## Módulos planejados (futuro)

| Módulo | MVP | Objetivo |
|--------|-----|----------|
| Identity | Sim | Autenticação e isolamento de dados |
| Projects | Sim | Contêineres para esforços maiores |
| Tasks | Sim | Itens acionáveis com status e prioridade |
| Inbox | Não | Captura rápida sem categorização imediata |
| Habits | Não | Ações recorrentes com registro simples |
| Routines | Não | Checklists reutilizáveis para iniciar sequências |
| Tracker | Não | Livros, cursos, séries e interesses de longo prazo |
| Lists | Não | Listas livres e leves |
| Calendar | Não | Visão temporal unificada |
| Notes | Não | Anotações Markdown vinculadas a recursos |
| Dashboard | Não | Visão consolidada de execução do dia |
| Insights | Não | Análise de padrões pessoais a partir do histórico |
| AI Assistance | Não | Apoio opcional e transparente às decisões |

Roadmap e justificativa: [Visão do Produto e Roadmap](docs/PRODUCT_ROADMAP.pt-BR.md).

---

## Stack confirmada

- .NET 8, ASP.NET Core Web API
- Entity Framework Core 8 + PostgreSQL
- ASP.NET Core Identity + JWT Bearer
- MediatR, FluentValidation
- Docker, Docker Compose
- xUnit, Moq, FluentAssertions

---

## Início rápido

**Docker (recomendado):**

```powershell
cd TaskForge
copy .env.example .env
# Edite .env — defina JWT_KEY (mínimo 32 caracteres)
docker compose up -d --build
```

Abra **http://localhost:8080/swagger**.

**Desenvolvimento .NET local:**

```powershell
dotnet tool restore
dotnet user-secrets set "Jwt:Key" "TaskForge-Local-Dev-Secret-Key-32chars!" --project TaskForge.Api
docker compose up -d postgres
dotnet ef database update --project TaskForge.Infrastructure --startup-project TaskForge.Api
dotnet run --project TaskForge.Api
```

Configuração completa, chaves, health checks e referência da API: [Visão Técnica](docs/TECHNICAL_OVERVIEW.pt-BR.md).

---

## Documentação

| Documento | Descrição |
|-----------|-----------|
| [Visão do Produto e Roadmap](docs/PRODUCT_ROADMAP.pt-BR.md) | Visão de longo prazo, módulos e ordem de evolução |
| [Escopo do MVP do Backend](docs/MVP_BACKEND_SCOPE.pt-BR.md) | Requisitos exatos do MVP e critérios de aceite |
| [Visão Técnica](docs/TECHNICAL_OVERVIEW.pt-BR.md) | Estrutura do repositório, endpoints e configuração |
| [Status de Implementação](docs/IMPLEMENTATION_STATUS.pt-BR.md) | Tabelas de progresso verificável e percentuais |
| [Fluxo Git](docs/GIT_WORKFLOW.pt-BR.md) | Nome de branches, convenção de commits e fluxo de merge |

---

## Fora do escopo por enquanto

Frontend, Inbox, hábitos, rotinas, Tracker, listas, calendário, notas, dashboard, insights, IA, colaboração entre usuários, notificações, gamificação, microsserviços, refresh token, roles complexas e CI/CD completo são **itens de roadmap** — não fazem parte do MVP atual.

---

## Licença

Consulte as informações de licença do repositório, se aplicável.
