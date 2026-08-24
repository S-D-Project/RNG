# Game Assets

## Scope

This document applies to Unity assets under `Assets/Game/`.

It defines project-wide safety rules for modifying assets managed by Unity.

More specific rules may exist in descendant `AGENTS.md` files.

---

## Unity Asset Integrity

* Preserve Unity asset identity when moving, renaming, or reorganizing files.
* Keep each Unity asset and its corresponding `.meta` file synchronized.
* When moving or renaming an asset outside the Unity Editor, move or rename its `.meta` file together with it.
* When intentionally deleting an asset, delete its corresponding `.meta` file as part of the same change.
* Never replace an existing `.meta` file with a newly generated one unless explicitly required.
* Avoid unnecessary `.meta` changes.

Unity GUID stability must be preserved unless changing asset identity is explicitly intended.

---

## Unity YAML Files

Unity-managed serialized files include, but are not limited to:

* Scenes
* Prefabs
* ScriptableObject assets
* Materials
* Animation-related assets

Do not directly modify Unity YAML serialization unless explicitly requested or clearly required by the task.

Prefer changes through Unity-supported workflows when the serialized structure cannot be modified safely.

Do not perform broad formatting, reserialization, or unrelated YAML changes.

---

## Asset Changes

* Keep asset changes limited to the requested scope.
* Avoid moving or renaming assets unless required.
* Check existing asset locations before creating duplicates.
* Preserve references between Scenes, Prefabs, ScriptableObjects, and other Unity assets.
* Do not reorganize asset directories as part of unrelated work.

---

## Script Changes

Runtime C# code under `Assets/Game/Scripts/` follows additional rules defined in:

```text
Assets/Game/Scripts/AGENTS.md
```

Always read that document before modifying runtime scripts.
