# Project Documentation

## Scope

This directory contains project documentation used to guide development, implementation, and technical decisions.

```text
docs/
├─ Architecture/
├─ Conventions/
├─ Plans/
└─ Decisions/
```

Each documentation area contains its own `AGENTS.md`.

Read the closest applicable `AGENTS.md` before creating or modifying documentation.

---

## Documentation Areas

### Architecture

```text
Docs/Architecture/
```

Describes the established structure of the project, including responsibilities, boundaries, dependencies, and major architectural concepts.

Follow:

```text
Docs/Architecture/AGENTS.md
```

### Conventions

```text
Docs/Conventions/
```

Defines repeatable project-wide development rules such as coding, Unity, naming, Git, and implementation conventions.

Follow:

```text
Docs/Conventions/AGENTS.md
```

### Plans

```text
Docs/Plans/
```

Contains approved execution plans for substantial implementation or refactoring work.

Follow:

```text
Docs/Plans/AGENTS.md
```

### Decisions

```text
Docs/Decisions/
```

Records approved architectural or technical decisions with long-term project impact.

Follow:

```text
Docs/Decisions/AGENTS.md
```

---

## Documentation Placement

Place information in the directory that matches its responsibility.

* Current project structure or boundaries → `Architecture/`
* Repeatable development rules → `Conventions/`
* How a specific substantial task will be executed → `Plans/`
* Why a long-lived architectural or technical choice was made → `Decisions/`

Do not duplicate the same rule or decision across multiple documentation areas.

When classification is unclear, prefer the document type that represents the information's long-term responsibility rather than its immediate use.
