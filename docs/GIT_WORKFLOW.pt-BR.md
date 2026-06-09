# TaskForge — Fluxo Git

> **English:** [GIT_WORKFLOW.md](GIT_WORKFLOW.md)

Este documento define como branches, commits e merges são tratados neste repositório.

---

## 1. Mensagens de commit

Usamos **[Conventional Commits](https://www.conventionalcommits.org/)** — um prefixo `type`, descrição no imperativo e corpo opcional com contexto.

### Formato

```text
type: descrição curta no imperativo

Corpo opcional explicando o porquê ou contexto importante.
Vários parágrafos são aceitáveis.
```

### Regras

| Regra | Exemplo |
|-------|---------|
| Usar um **type** válido (tabela abaixo) | `feat: ...` |
| **Imperativo** no assunto (como se completasse: “este commit vai…”) | `add task filters` ✓ — `added task filters` ✗ |
| **Minúsculas** no assunto após os dois pontos | `feat: add ownership` ✓ |
| **Sem ponto** no final do assunto | `fix: handle 404 on delete` ✓ |
| Manter o assunto **em até ~72 caracteres** quando possível | |
| Escrever o assunto em **inglês** para um `git log` consistente | |
| Um change lógico por commit, quando fizer sentido | |
| **Scope** opcional entre parênteses: `feat(tasks): ...` | |

### Types

| Type | Quando usar |
|------|-------------|
| `feat` | Novo comportamento, endpoint, entidade ou capacidade visível |
| `fix` | Correção de bug |
| `docs` | Apenas documentação (README, `docs/`, comentários em docs) |
| `refactor` | Reestruturação sem mudar comportamento externo |
| `test` | Adicionar ou atualizar testes |
| `chore` | Ferramentas, manutenção, config fora do código de produção |
| `build` | Build, Docker, `.csproj`, dependências de compilação |
| `ci` | Pipelines e automação de CI/CD |

### Bons exemplos (deste repositório)

```text
feat: complete backend MVP with tasks, ownership, and user scoping

docs: add bilingual project documentation reflecting actual MVP state

fix: return 404 when project belongs to another user

test: add UpdateTaskHandler ownership scenarios
```

### Evitar

```text
create readme.md                    # sem type
Add project files.                  # sem type, maiúscula, ponto final
feat: add command test              # vago — qual command? qual test?
refactor: iniciated the migration   # typo, escopo pouco claro
feat: WIP                           # evitar WIP vago em branches compartilhadas
```

### Corpo (opcional)

Use o corpo quando o **porquê** não ficar claro só pelo assunto:

```text
feat: add JsonStringEnumConverter for task status and priority

Swagger e clientes da API esperam enums como string (ex.: "InProgress").
Enums numéricos causavam 400 no PUT /tasks.
```

### Breaking changes

Se o commit quebrar clientes ou contratos existentes, adicione um rodapé:

```text
feat: rename project route prefix to /api/v2/projects

BREAKING CHANGE: clients must use /api/v2/projects instead of /api/projects.
```

---

## 2. Branches

| Branch | Papel |
|--------|-------|
| `master` | Linha estável; reflete o que está “pronto” e mergeável |
| `devel` | Branch de integração; features entram aqui antes do `master` |
| `aaaa-mm-dd-palavra` | **Feature branches** de curta duração (data + palavra geral) |

### Nome das feature branches

```text
aaaa-mm-dd-descricao-curta
```

- **Data:** dia em que o trabalho **começou** (ou o dia principal da entrega), formato ISO `aaaa-mm-dd`
- **Descrição:** uma palavra simples em inglês (ou frase curta com hífens) descrevendo o tema
- Use **minúsculas** e **hífens**

Exemplos:

```text
2026-06-07-docs
2026-06-08-mvp
2026-06-15-tasks
```

---

## 3. Fluxo de merge

Fluxo padrão para uma feature:

```text
feature branch  →  devel  →  master
```

### Passo a passo

**1. Criar a feature branch** (a partir de `master`):

```powershell
git checkout master
git pull origin master
git checkout -b 2026-06-08-mvp
```

**2. Trabalhar e commitar** na feature branch (seguir as [convenções de commit](#1-mensagens-de-commit) acima).

**3. Merge em `devel`:**

```powershell
git checkout devel
git pull origin devel
git merge 2026-06-08-mvp
```

**4. Merge de `devel` em `master`:**

```powershell
git checkout master
git pull origin master
git merge devel
```

**5. Push** quando quiser publicar:

```powershell
git push origin master devel 2026-06-08-mvp
```

Fast-forward é aceitável quando `devel` e `master` não tiverem trabalho divergente.

### Exemplo real (2026-06-08)

```text
2026-06-08-mvp  →  devel  →  master
commit d544be9: feat: complete backend MVP with tasks, ownership, and user scoping
```

---

## 4. O que não commitar

| Caminho / arquivo | Motivo |
|-------------------|--------|
| `.env` | Segredos e valores locais de ambiente |
| `.cursor/` | Configuração local de IDE/agent |
| JWT keys, senhas e connection strings reais | Segurança |

`.env.example` pode ficar versionado como **template**, sem segredos reais.

---

## 5. Referência rápida

```text
Branch:   aaaa-mm-dd-palavra
Commit:   type: descrição no imperativo (inglês)
Fluxo:    feature → devel → master
```
