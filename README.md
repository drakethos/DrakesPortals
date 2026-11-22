# DrakesPortals

> [!NOTE]
> **Stub (0.1.0).** The project, build, packaging and config are in place. The gate itself is not: no portal does anything yet. See [docs/design.md](docs/design.md).

One-way gates for Valheim servers. A gate sends a player somewhere and never offers a way back through itself.
Part of DrakeMods, built on **DrakeModsLibs** (config sync, UI) with **ModGuard** (isolated patching and attributed errors).

## Idea

- Every gate is one way. Direction is not an option; the rules around it are.
- A gate asks a **destination resolver** where to send the player. 0.1.0 has one: **Fixed** (a configured position).
- Later resolvers: held item, team or global key, random, puzzle. The portal code does not change when one is added.
- Planned rules: who may enter (team via DrakesServerTeams, boss kills), use limits per player, admin-only gates.

## Install

Needs BepInEx, Jotunn and DrakeModsLibs. On a dedicated server, install it on the server and every client.
DrakesServerTeams is optional (team gates only).

## Settings

Synced from the server (`BepInEx/config/com.drakemods.portals.cfg`): `Enabled`, `DefaultResolver`, `FixedDestination`
(`x,y,z`), and the admin lock `LockSyncedConfig`.

## Building

Settings come from `mod.package.props` (name, version, GUID, dependencies). Machine paths come from `environment.props`
(gitignored); copy `environment.props.example` (Windows) or `environment.props.example.linux`.

**Windows:** open `DrakesPortals.sln` in Visual Studio, or `msbuild DrakesPortals.csproj`. A build deploys the DLL to the
Gale and r2modman profile named by `ProfileName`, and writes the Thunderstore folder and zip.

**Linux:** `build/scripts/build-linux.sh [Debug|Release]`. It restores NuGet, stages the game references from the
Pfhoenix package (no Valheim install needed), builds DrakeModsLibs at the pinned tag, builds the mod and writes
`dist/DrakeMods-DrakesPortals-<version>.zip`. Needs dotnet SDK 8, nuget, git, zip, python3 and jq.

**Releases:** push a tag `vX.Y.Z` that matches `<Version>` in `mod.package.props`; CI builds and publishes to Hexium.
