# CoreKit

CoreKit is a modular C# utility library designed to streamline Unity and .NET development. It provides essential components such as dependency injection, UI management, and utility helpers to accelerate your project setup and maintain clean architecture.

# Table of Contents
- [App Module](#corekit--app-module)
- [Log](#corekit---log)
- [Scenes Module](#corekit---scenes-module)
- [StateMachine](#corekit---statemachine)
- [Saving](#corekit---saving)

# App Module

## Purpose
`BaseApp` centralizes common application lifecycle concerns:
- Singleton instance accessible via `BaseApp.Instance`.
- Persistent root across scenes using `DontDestroyOnLoad`.
- Deterministic initialization flow and guarded per-frame updates.
- Virtual hooks for focus, pause, back button and destruction handling.

## Key behaviors
- On `Awake()` the first `BaseApp` instance becomes the singleton, `DontDestroyOnLoad` is applied and initialization starts. Additional instances are destroyed.
- `InternalAppCreate()` calls `OnAppCreate()`, sets the internal initialized flag, then calls `OnAppStart()`.
- `Update()`, `OnApplicationFocus()` and `OnApplicationPause()` forward events only after initialization.
- `OnDestroy()` calls `OnAppDestroy()` for the active singleton instance.

## Typical usage
1. Create a concrete subclass of `BaseApp` in your project and override `OnAppCreate()` and `OnAppDestroy()` at minimum.
2. Put the concrete `BaseApp` on a GameObject in an initialization scene (or create it at runtime). The object will persist across scenes.
3. Use `OnAppStart()` for starting services or flows that should run after initialization.
4. Use `OnAppUpdate()` for centralized per\-frame logic or forward updates to `IAppUpdateListener` implementations.

# Log

This document describes the lightweight logging subsystem included in the project. The log system provides a small, pluggable logging façade and several concrete logger implementations intended for use throughout the app.

## Purpose
- Provide a single, project-wide entry point for logging.
- Allow multiple logger implementations (console, file, scene components) to be used interchangeably.
- Keep logging usage simple and consistent (use the `Logger` façade).

## Design overview
- `ILogger` defines the required logging contract. Concrete loggers implement that contract.
- `Logger` is a static façade that application code calls directly. It delegates to one or more `ILogger` instances.
- Concrete implementations handle output specifics:
   - `UnityLogger` -> `Debug.Log`, `Debug.LogWarning`, `Debug.LogError`.
   - `FileLogger` -> appends to a file in persistent data path (rotates or truncates depending on implementation).
   - `ComponentLogger` -> forwards to MonoBehaviour components for in-scene display or remote forwarding.

## Usage
- To log simple messages:
   - `Logger.Log("Initialization complete");` (used throughout the project; see `BaseApp` usage).
- To log errors or warnings, check the logging façade for provided severity helpers (e.g. `LogError` / `LogWarning`) or include severity text in the message if only a single method exists.
- Concrete loggers are typically registered during application initialization. Check `Logger.cs` for the actual registration API (common patterns: `AddLogger`, `SetLogger`, or automatic registration via scene components).

## Extending
- Implement `ILogger` to create a custom logger (network sink, telemetry, remote syslog, etc.).
- Register your implementation with the façade during startup so it receives messages from calls to `Logger`.

## Best practices
- Use the `Logger` façade everywhere instead of calling Unity API directly; this keeps output consistent and allows adding/removing outputs without changing call sites.
- Keep log messages small and include context (component name, subsystem, event).
- Avoid logging excessive data in tight loops; prefer throttling or conditional logging for high-frequency sections.
- Use file logging for post-mortem diagnostics and `UnityLogger` for editor/runtime console visibility.

## Troubleshooting
- If logs do not appear in the Unity Console, verify that `UnityLogger` is active or that the façade is configured to forward to it.
- If file logs are missing, verify write permissions and the path used by `FileLogger` (usually `Application.persistentDataPath`).

## Example
- Typical call from application code:
   - `Logger.Log("[App] OnAppCreate");`


# Scenes Module

## Purpose
Provide a small, robust abstraction for asynchronous scene management in Unity. The implementation uses `Cysharp.Threading.Tasks` (`UniTask`) and wraps `UnityEngine.SceneManagement` to offer simple loading, unloading and load-progress callbacks.

## Key features
\- Async scene loading with progress reporting and controlled activation.  
\- Additive scene tracking for correct unload behavior.  
\- Safe unload with exception handling and logging.

## Main API
\- `UniTask LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, bool setActive = true, Action<float>? onProgress = null)`  
Loads a scene asynchronously. Reports progress via `onProgress` (0..1). When `mode` is `Additive`, the loader tracks the scene name for later unloads.

\- `UniTask UnloadSceneAsync(string sceneName)`  
Unloads a previously loaded scene (tracked or loaded in the scene manager).

\- `bool IsSceneLoaded(string sceneName)`  
Returns whether the scene is currently loaded or tracked.

## Usage
Register or construct an `ISceneLoader` implementation (for example via DI). Example call:

`await sceneLoader.LoadSceneAsync("Level1", LoadSceneMode.Additive, setActive: true, onProgress: p => Debug.Log($"Load: {p:P0}"));`

Use `onProgress` to update UI and call `UnloadSceneAsync` when the scene is no longer needed.

## Notes
\- Progress is normalized (operation progress divided by 0.9) to map Unity's load progress range to 0..1.  
\- Activation is deferred until the operation reaches 0.9 and `setActive` is true.  
\- Errors are logged and surfaced by the internal logger; progress callback receives `0f` on failure.

# StateMachine

## Overview
The StateMachine module provides a small, dependency\-\-free finite state machine for structuring application/game flow into explicit states. It defines a minimal lifecycle contract and a coordinator that performs deterministic transitions.

## Core concepts

### `IState`
A state is a unit of behavior with a small lifecycle:

- `Prepare(object payload = null)`  
  Called before entering the state. Use it to read/capture transition parameters and pre\-\-configure the state.

- `Enter(object payload = null)`  
  Activates the state (subscribe to events, show UI, start logic).

- `Exit()`  
  Deactivates the state (unsubscribe from events, hide UI, stop logic, cleanup).

`payload` is optional and can be any object needed to parameterize a transition (e.g., level id, configuration, navigation data).

### `StateMachine`
`StateMachine` is responsible for:
- Storing the current active state.
- Performing transitions deterministically (exit current state \-> prepare new state \-> enter new state).
- Passing optional payloads through the transition.

See `StateMachine.cs` for the exact transition method names and the enforced order.

### `BaseState`
`BaseState` provides no\-\-op/default implementations and commonly shared plumbing. Use it to keep concrete states focused on behavior rather than lifecycle wiring.

## Usage

### 1\) Implement a state
Create a concrete state by implementing `IState` (or inheriting `BaseState`).

Typical responsibilities:
- `Prepare`: cache payload and validate required inputs.
- `Enter`: set up runtime behavior.
- `Exit`: undo everything created/subscribed in `Enter`.

### 2\) Create and drive the state machine
- Instantiate the concrete state machine (or `StateMachine`) during bootstrap.
- Transition to the initial state (e.g., boot/loading/menu).
- Ensure transitions call `Exit()` on the previous state.

### 3\) Pass transition data via payload
Use the optional `payload` argument for one\-\-off parameters used only for the upcoming transition (avoid storing transient data globally).

## Example (reference)
The `Example/` folder demonstrates a minimal application flow:
- `BootState` performs startup logic and moves to `MainMenuState`.
- `ExampleContext` stores shared data that should persist across states.
- `ExampleApplication` shows how a Unity component can create and drive the state machine.

## Best practices
- Keep each state responsible for a single feature/phase (boot, loading, menu, gameplay).
- Treat `Exit()` as mandatory cleanup (unsubscribe events, detach callbacks, stop work started in `Enter()`).
- Use a typed context object (e.g., `ExampleContext`) for shared long\-\-lived data.
- Use `payload` only for transition\-\-specific inputs.

## Troubleshooting
- **State does not run**: verify an initial state is set and that the transition method actually calls `Prepare()` and `Enter()`.
- **Lingering behavior/events**: ensure `Exit()` undoes all subscriptions and external hooks created in `Enter()`.
- **Wrong data on transitions**: validate what is passed in `payload` and where it is captured (prefer capturing in `Prepare()`).

# Saving

## Overview
The Saving module provides a small set of interfaces and services for persisting game/application data using a key\-\-value storage abstraction, JSON serialization, and optional migration support. It is designed to keep save logic decoupled from Unity\-specific storage details while still supporting common backends (file, PlayerPrefs).

## Core concepts

### Storage vs Serializer
- `ISaveStorage` defines \*where\* bytes/text are stored (file system, PlayerPrefs, etc.).
- `ISaveSerializer` defines \*how\* save data is converted to/from a persisted representation (JSON via `NewtonsoftJsonSerializer`).

This split allows swapping either side without rewriting save logic.

### Save data providers
`ISaveDataProvider` (and `ISaveDataProviderWithDirty`) represent game systems that:
- contribute their portion of data into the save model, and
- restore themselves from loaded save data.

Using providers keeps save logic modular (inventory, progression, settings, etc. can be independent).

### Dirty tracking
To avoid rewriting saves every time, providers can expose dirty state via `ISaveDataProviderWithDirty`, optionally aggregated through `ISaveDirtyTracker`. `SaveService` can then decide when a write is needed.

### Migrations
Saved data formats evolve. `ISaveMigration` and `SaveMigrationService` allow upgrading older persisted data to the current version (e.g., renaming fields, splitting one field into many, defaulting missing values) without breaking existing players.

## Typical flow
1\) Create a storage backend:
- `FileJsonStorage` for a JSON file in a persistent location.
- `PlayerPrefsStorage` for small settings\-like data.

2\) Choose a serializer:
- `NewtonsoftJsonSerializer` for JSON serialization.

3\) Register providers:
- Each subsystem implements `ISaveDataProvider` (optionally dirty\-aware).

4\) Load on startup:
- Read persisted data, migrate if required, then distribute loaded data to providers.

5\) Save when needed:
- Collect provider data into the save model and persist it via storage + serializer.

## Best practices
- Keep providers small and focused (one feature/system per provider).
- Use `ISaveDataProviderWithDirty` for high\-frequency systems to reduce IO.
- Treat migrations as deterministic, versioned transforms; never depend on runtime\-only state.
- Prefer file storage for larger saves; keep PlayerPrefs for lightweight settings.

## Troubleshooting
- **Data not restoring**: verify the provider is registered and is applied during load (check `SaveService.cs` flow).
- **Save file never updates**: confirm dirty tracking is enabled/used and that providers mark themselves dirty when data changes.
- **Migration issues**: ensure migrations are ordered correctly and that the stored version is updated after applying them.
- **Serialization failures**: validate the save data model is compatible with the configured serializer (`NewtonsoftJsonSerializer.cs`).

