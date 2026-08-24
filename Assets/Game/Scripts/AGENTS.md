# Runtime Scripts

## Scope

This directory contains the primary runtime C# code for the game.

Major responsibilities are divided into:

```text
Application/
Core/
Data/
Gameflow/
Gameplay/
Presentation/
```

Each domain may define additional rules in its own `AGENTS.md`.

---

## Before Making Changes

Before modifying runtime code:

1. Read the closest domain-specific `AGENTS.md`.
2. Review relevant architecture and convention documents.
3. Search for existing code that already owns the requested responsibility.
4. Inspect nearby implementations for established patterns.

Do not create a new abstraction before checking whether the responsibility already exists.

---

## Working Rules

* Make the smallest change necessary to satisfy the requested task.
* Preserve existing layer boundaries and dependency direction.
* Avoid unrelated refactoring.
* Prefer extending an existing responsibility owner over creating a parallel implementation.
* Avoid introducing new managers, services, utilities, global access points, or abstractions without a clear responsibility.
* Do not move responsibilities between layers without explicit architectural justification.
* Follow existing naming, structure, and implementation conventions.
* Do not invent behavior that is not supported by the request or project documentation.

---

## Ambiguity

Resolve minor implementation details using:

1. Applicable architecture documents
2. Applicable conventions
3. Approved Decisions and Plans
4. Existing nearby implementation patterns

Ask for clarification only when ambiguity materially affects:

* observable game behavior
* architecture
* layer boundaries
* public APIs
* shared data contracts

Do not block localized implementation work over minor details that can be resolved consistently from existing project guidance.

---

## Structural Changes

Do not perform broad structural changes unless explicitly requested or approved.

Examples include:

* moving responsibilities between major layers
* large directory reorganizations
* introducing new global systems
* broad namespace changes
* package or assembly restructuring
* large-scale renaming

Keep architectural changes separate from unrelated implementation work.

---

## Domain Rules

Domain-specific implementation rules belong in the closest applicable `AGENTS.md`.

```text
Application/AGENTS.md
Core/AGENTS.md
Data/AGENTS.md
Gameflow/AGENTS.md
Gameplay/AGENTS.md
Presentation/AGENTS.md
```

Do not duplicate domain-specific rules in this document unless they apply to all runtime code.
