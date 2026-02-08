## VContainer LoopSystem Adapter

This sample shows how to initialize `LoopSystem` via VContainer and execute loops in the appropriate phases.

### How to use

1. Install VContainer in your project.
2. Import the Sample via Package Manager.
3. Create a GameObject in the scene and add the `LoopSystemInstaller` component.

### What happens

- `Loops.Initiate()` is called when the container starts.
- In `Tick()`, `Timer` runs first, then `Update`.
- In `FixedTick()`, `FixedUpdate` runs.
- In `LateTick()`, `LateUpdate` runs.
- In `Dispose()`, `Loops.Dispose()` is called.
