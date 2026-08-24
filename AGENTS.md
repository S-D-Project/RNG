# RNG Diff

## Project Purpose

RNG Diff is a short-session action roguelike built with Unity.

The core experience is to let players focus on movement and positioning while attacks are performed automatically, then build a unique weapon setup through limited random trait choices.

Each run should provide a clear sense of rapid growth within roughly 5–10 minutes, with weapon behavior changing noticeably as the build develops.

The project is intentionally scoped for two developers to complete and release on Steam within a limited development period.

---

## Project Structure

Primary project areas:

```text
Assets/Game/
├─ AGENTS.md
└─ Scripts/
   ├─ AGENTS.md
   ├─ Core/
   ├─ Data/
   ├─ Gameflow/
   ├─ Gameplay/
   └─ Presentation/

Docs/
├─ AGENTS.md
├─ Architecture/
├─ Conventions/
├─ Plans/
└─ Decisions/
```

Use the closest applicable `AGENTS.md` for detailed working rules.

---

## AGENTS Resolution

Before making changes:

1. Read this root `AGENTS.md`.
2. Read each applicable `AGENTS.md` on the path to the target.
3. Follow the most specific applicable rule.
4. Review referenced project documentation when required.

Priority between `AGENTS.md` files:

```text
Closest AGENTS.md
↓
Parent AGENTS.md
↓
Root AGENTS.md
```

More specific rules override broader rules unless a broader rule explicitly prohibits the change.

`AGENTS Resolution` applies only to conflicts between `AGENTS.md` files.

---

## Global Working Principles

* Make small and focused changes.
* Avoid unrelated refactoring.
* Preserve established architecture and conventions.
* Do not invent requirements.
* Do not expand the requested scope without explicit approval.
* Keep changes aligned with the current project and release scope.

---

## Documentation

Project documentation is managed under `docs/`.

Follow `docs/AGENTS.md` when creating, modifying, or using project documentation.

Runtime code is primarily managed under `Assets/Game/Scripts/`.

Follow `Assets/Game/AGENTS.md` and `Assets/Game/Scripts/AGENTS.md` when modifying game assets or runtime code.

---

## Source of Truth

When project information conflicts, use the following priority:

1. Explicit user instructions
2. Applicable `AGENTS.md`
3. Approved Decisions
4. Approved Plans
5. Architecture and convention documents
6. Existing implementation

If a conflict materially affects the requested work and cannot be resolved from this order, ask for clarification before proceeding.

---

## Navigation Rule

When the correct location or responsibility owner is unclear:

1. Inspect the existing project structure.
2. Read the applicable `AGENTS.md` files.
3. Review relevant architecture and convention documents.
4. Search for similar existing implementations.
5. Prefer the option most consistent with the established project structure.

Do not introduce a new architectural responsibility solely because the correct ownership is initially unclear.
