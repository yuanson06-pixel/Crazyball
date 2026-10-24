# Crazyball Course Kit Architecture

Crazyball is a component-based Unity course template. The course uses two scenes:

- `Crazyball_Main`: persistent upgrades and starting a run.
- `Crazyball_Course`: the single scene that students progressively assemble.

The original asset folders are reference sources. New runtime code and adapted assets live under `Assets/Crazyball` so the folder can be exported as one unitypackage.

## Currency

- **Coin / RunCoin**: earned during a run and spent at checkpoints. It resets when a new run starts.
- **Core / UpgradeCore**: awarded once for the first clear of each checkpoint. It is saved and spent in the Main scene.

## Progression

The course game contains five authored stages. Clearing a checkpoint grants its repeatable Coin reward and, on first clear only, its Core reward. Failure starts a new run at Stage 1.

## Upgrade split

- In-game upgrades present up to three random eligible choices. The player may buy one or skip.
- Persistent upgrades include starting shield, starting Coin bonus, Coin Magnet unlock, and Time Slow unlock.

## Authoring rules

- Do not edit Unity scene YAML directly.
- Do not edit existing `.meta` files.
- Configure scenes, prefabs, and serialized references in the Unity Editor.
- Use only the New Input System for runtime and UI input.
- Runtime scripts must remain complete and compilable; lessons focus on assembling components and data.

## Complete scene EditorLayout

`CrazyballCompleteEditorLayout.Apply()` assembles the complete reference scene with Unity Editor APIs. It creates scene objects, prefabs, TMP UI, Input Actions, upgrade definitions, and serialized references without directly editing scene YAML or existing meta files.
