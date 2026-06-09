# TaskForge — Git Workflow

> **Português (Brasil):** [GIT_WORKFLOW.pt-BR.md](GIT_WORKFLOW.pt-BR.md)

This document defines how branches, commits, and merges are handled in this repository.

---

## 1. Commit messages

We use **[Conventional Commits](https://www.conventionalcommits.org/)** — a short `type` prefix, imperative description, and optional body for context.

### Format

```text
type: short description in imperative mood

Optional body explaining why or important context.
Multiple paragraphs are fine.
```

### Rules

| Rule | Example |
|------|---------|
| Use a valid **type** (see table below) | `feat: ...` |
| **Imperative mood** in the subject (as if completing: “this commit will…”) | `add task filters` ✓ — `added task filters` ✗ |
| **Lowercase** subject after the colon | `feat: add ownership` ✓ |
| **No period** at the end of the subject | `fix: handle 404 on delete` ✓ |
| Keep the subject **under ~72 characters** when possible | |
| Write the subject in **English** for a consistent `git log` | |
| One logical change per commit when practical | |
| Optional **scope** in parentheses for a module: `feat(tasks): ...` | |

### Types

| Type | When to use |
|------|-------------|
| `feat` | New behavior, endpoint, entity, or user-visible capability |
| `fix` | Bug fix |
| `docs` | Documentation only (README, `docs/`, comments in docs) |
| `refactor` | Code restructuring without changing external behavior |
| `test` | Adding or updating tests only |
| `chore` | Tooling, housekeeping, non-production config |
| `build` | Build system, Docker, `.csproj`, dependencies affecting build |
| `ci` | CI/CD pipelines and automation |

### Good examples (from this repo)

```text
feat: complete backend MVP with tasks, ownership, and user scoping

docs: add bilingual project documentation reflecting actual MVP state

fix: return 404 when project belongs to another user

test: add UpdateTaskHandler ownership scenarios
```

### Avoid

```text
create readme.md                    # missing type
Add project files.                  # capitalized, no type, period
feat: add command test              # too vague — what command? what test?
refactor: iniciated the migration   # typo, unclear scope
feat: WIP                           # avoid vague WIP on shared branches
```

### Body (optional)

Use the body when the **why** is not obvious from the subject:

```text
feat: add JsonStringEnumConverter for task status and priority

Swagger and API clients expect string enums (e.g. "InProgress").
Numeric enums caused 400 errors on PUT /tasks.
```

### Breaking changes

If a commit breaks existing clients or contracts, add a footer:

```text
feat: rename project route prefix to /api/v2/projects

BREAKING CHANGE: clients must use /api/v2/projects instead of /api/projects.
```

---

## 2. Branches

| Branch | Role |
|--------|------|
| `master` | Stable line; reflects what is considered “done” and merge-ready |
| `devel` | Integration branch; features land here before `master` |
| `aaaa-mm-dd-word` | Short-lived **feature branches** (date + one general word) |

### Feature branch naming

```text
yyyy-mm-dd-short-description
```

- **Date:** day work **started** (or the main delivery day), ISO format `yyyy-mm-dd`
- **Description:** one simple English word (or short hyphenated phrase) describing the theme
- Use **lowercase** and **hyphens**

Examples:

```text
2026-06-07-docs
2026-06-08-mvp
2026-06-15-tasks
```

---

## 3. Merge flow

Default flow for a feature:

```text
feature branch  →  devel  →  master
```

### Step by step

**1. Create the feature branch** (from `master`):

```powershell
git checkout master
git pull origin master
git checkout -b 2026-06-08-mvp
```

**2. Work and commit** on the feature branch (follow [commit conventions](#1-commit-messages) above).

**3. Merge into `devel`:**

```powershell
git checkout devel
git pull origin devel
git merge 2026-06-08-mvp
```

**4. Merge `devel` into `master`:**

```powershell
git checkout master
git pull origin master
git merge devel
```

**5. Push** when ready to publish:

```powershell
git push origin master devel 2026-06-08-mvp
```

Fast-forward merges are fine when `devel` and `master` have no divergent work.

### Real example (2026-06-08)

```text
2026-06-08-mvp  →  devel  →  master
commit d544be9: feat: complete backend MVP with tasks, ownership, and user scoping
```

---

## 4. What not to commit

| Path / file | Reason |
|-------------|--------|
| `.env` | Local secrets and environment-specific values |
| `.cursor/` | IDE/agent local configuration |
| Real JWT keys, passwords, connection strings | Security |

`.env.example` may be tracked as a **template** without real secrets.

---

## 5. Quick reference

```text
Branch:   yyyy-mm-dd-word
Commit:   type: imperative description
Flow:     feature → devel → master
```
