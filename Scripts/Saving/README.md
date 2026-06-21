# CoreKit — Saving

Modular JSON save framework for PC games with multi-slot support, dirty tracking, migrations, and cloud-sync hooks (Steam-ready).

## Overview

The Saving module keeps game persistence decoupled from storage backends and serialization. Game systems contribute data through `ISaveDataProvider`; `SaveService` builds a versioned JSON envelope, writes atomically to disk, and optionally uploads via `ICloudSaveSync`.

**Local-first:** during a session the disk is the source of truth. Cloud sync runs after a successful local flush.

## Architecture

```
ISaveDataProvider(s)  →  SaveService  →  ISaveSerializer (JSON)
                              ↓
                         ISaveStorage (FileJsonStorage)
                              ↓
                         ICloudSaveSync (optional, stub by default)
```

| Type | Role |
|------|------|
| `ISaveService` | Public API: save/load/delete slots, metadata, cloud resolve |
| `SaveService` | Orchestrates providers, merge-save, migrations, events |
| `ISaveStorage` | Where bytes are stored (`FileJsonStorage`) |
| `ISaveSerializer` | How objects become JSON (`NewtonsoftJsonSerializer`) |
| `ISaveDataProvider` | Module captures/restores its DTO by `Key` |
| `ISaveDataProviderWithDirty` | Skips unchanged modules on partial saves |
| `SaveMigrationService` | Upgrades old envelope versions on load |
| `ICloudSaveSync` | Upload/download/conflict hooks (`NullCloudSaveSync` = no-op) |
| `IKeyValueStorage` / `PlayerPrefsStorage` | Lightweight settings only — **not** game saves |

## Save file format

Envelope written as `{key}.json`:

```json
{
  "Version": "1.0.0",
  "SavedAtUtc": 1718880000,
  "Metadata": {
    "LocationName": "Mine",
    "PlayTimeSeconds": 1234.5
  },
  "Data": {
    "inventory": "{\"Slots\":[...]}",
    "level": "{\"Corruption\":42,...}"
  },
  "Timestamps": {
    "inventory": 1718880000,
    "level": 1718880000
  }
}
```

Each `Data` value is a nested JSON string for one provider module.

## Multi-slot layout

`SaveSlotId`: `Slot0` … `Slot4` (manual), `AutoSave`.

Storage keys → files (under caller-provided directory):

```
{persistentDataPath}/Saves/{profileId}/
  slot_0.json
  slot_1.json
  ...
  slot_4.json
  autosave.json
```

Profile folder (`local`, later `steam_{SteamId}`) isolates saves per user/account.

`FileJsonStorage` uses atomic writes: `.tmp` → replace → previous `.bak` for recovery.

## Quick start

```csharp
string dir = Path.Combine(Application.persistentDataPath, "Saves", "local");
FileJsonStorage storage = new FileJsonStorage(dir);
SaveMigrationService migrations = new SaveMigrationService(Array.Empty<ISaveMigration>());
SaveService saveService = new SaveService(
    storage,
    new NewtonsoftJsonSerializer(),
    providers,
    migrations,
    new NullCloudSaveSync(),
    currentSaveVersion: "1.0.0");

await saveService.ResolveCloudOnStartupAsync();
await saveService.LoadAsync(SaveSlotId.Slot0);
await saveService.SaveAsync(SaveSlotId.Slot0, saveAllModules: true);
```

Register providers before save/load. Manual slots should use `saveAllModules: true`. Autosave can use dirty tracking (`saveAllModules: false`).

## Dirty tracking

Providers implementing `ISaveDataProviderWithDirty` expose `IsDirty`. On partial save, only dirty modules are re-captured.

**Merge behavior:** `SaveAsync` loads the existing envelope first, updates only dirty (or all, when forced) modules, and preserves other modules' data, `Version`, and timestamps. Clean modules are never dropped.

Call `MarkDirty()` when module state changes. `SaveService` calls `ClearDirty()` after a successful capture.

## Migrations

Implement `ISaveMigration` with `FromVersion`, `ToVersion`, and `Migrate(GameSaveData)`. Register in `SaveMigrationService`; applied automatically on load.

Use exact version strings. Bump `currentSaveVersion` passed to `SaveService` when the schema changes.

## Cloud sync (future / Steam)

`ICloudSaveSync`:

- `UploadAsync(SaveSlotId)` — after local save succeeds
- `DownloadAsync(SaveSlotId)`
- `ResolveOnStartupAsync()` — compare local vs cloud timestamps before load
- `IsAvailable` — `false` in `NullCloudSaveSync`

**Default conflict policy:** newer `SavedAtUtc` wins; tie → local.

**Steam checklist (not implemented in CoreKit):**

1. Add Steam SDK in a separate assembly (e.g. `GreedyGold/Platform/Steam/`).
2. Implement `SteamCloudSaveSync : ICloudSaveSync` using Remote Storage API.
3. Enable Steam Cloud in partner portal; match cloud file names to local (`slot_0.json`, etc.).
4. Use per-SteamID profile path under `Saves/`.
5. Handle sync callbacks; avoid writing local files during active cloud download.

## Greedy Gold integration

Registered in `GameBootstrap.RegisterSaveSystem()`:

- Path: `Saves/local/` (see `GameSaveConstants`)
- Providers: `InventorySaveProvider`, `LevelSaveProvider`, `PlayerProgressSaveProvider`, `SaveMetadataProvider`
- `SaveCoordinator` — autosave debounce (30s), quit/pause/level-state triggers
- UI: `SaveLoadWindow` + `SaveLoadPresenter` (`WindowType.SaveLoad`)

**Unity Editor wiring:**

1. Add `SaveCoordinator` to bootstrap scene (or let bootstrap auto-add).
2. Create Save/Load UI prefab with `SaveLoadWindow`; wire 5 manual slot button groups + autosave row.
3. Add window to `UIManager._windows`.
4. Open via `UIManager.OpenWindow(WindowType.SaveLoad)`.

## Troubleshooting

| Issue | Check |
|-------|--------|
| Data missing after autosave | Partial save requires merge — ensure CoreKit `SaveService` merge logic is used |
| Module not restored | Provider registered before load; `Key` matches saved data |
| Corrupt save | `FileJsonStorage` falls back to `.bak`; check logs |
| Slot always empty | `saveAllModules: true` for first manual save |
| Cloud not syncing | `ICloudSaveSync.IsAvailable`; `NullCloudSaveSync` is expected until Steam is added |
| Migration loop | `FromVersion` / `ToVersion` chain must be contiguous |

## Best practices

- One feature per provider; keep DTOs small and serializable.
- Use dirty tracking for high-frequency state; force full save for manual slots.
- Never depend on runtime-only state in migrations.
- Use `PlayerPrefsStorage` / `IKeyValueStorage` for settings, not run saves.
- Avoid saving during combat unless debounced autosave is acceptable.

## Tests

Edit Mode tests live in `Assets/CursedVein/CoreKit/Tests/EditMode/Saving/` (assembly `YeKostenko.CoreKit.Tests`).

Run in Unity: **Window → General → Test Runner → EditMode → Run All**.

Async tests use `[UnityTest]` + `UniTask.ToCoroutine` because `FileJsonStorage` schedules IO on the thread pool.

Coverage includes:

- `SaveSlotKey` — slot key mapping
- `FileJsonStorage` — atomic write, backup fallback, timestamps
- `SaveMigrationService` — version chain
- `SaveService` — save/load/delete, dirty merge-save, metadata, cloud upload hook
- `NullCloudSaveSync` — no-op cloud stub
