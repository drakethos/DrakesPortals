# DrakesPortals

One-way portals for Valheim servers. Link two portals with the same tag as usual, then press **Shift+E** on one: make it the
**Entry** and the other becomes the **Exit**. Players walk into the entry and arrive at the exit; the exit does not send them back.
Each portal says which way it goes (hover text and glow colour).

> [!NOTE]
> **Work in progress (0.1.0).** Written against the Valheim 1.0 assemblies and compile-checked, but not yet run in game. The DrakesTestLab suite
> in `Tests/` is written and not yet run either. Back up your world before trying it.

## Use

1. Build two portals and give them the same tag (vanilla).
2. Shift+E on one portal opens **Portal direction**: Two-way, One-way: Entry, One-way: Exit.
3. Choosing one makes the linked portal the opposite end. Two-way puts both back.

- **Who can change directions** (`ManageAccess`): `AdminOnly` (default) or `Everyone`. Synced from the server; the server checks every request.
- **Enabled**: master switch (synced). Off = every portal works like vanilla; directions are kept but ignored.
- **Display** (this device only): tint the glow by direction, and the two colours.
- Plain **E** is still the vanilla tag prompt.

## Settings

`BepInEx/config/com.drakemods.portals.cfg`. The server's `Enabled` and `ManageAccess` reach every client (Jotunn's config sync) and only
admins can change them in game. `UseToolkitUi` (client) switches the menu between the new look and the classic wood panel.

## Install

Needs BepInEx and Jotunn. No other DrakeMods are required. On a dedicated server install it on the server and on every client
(it is network-checked).

If DrakeModsLibs is also installed, DrakesPortals claims portal pieces so RenameIt's piece rename leaves Shift+E on portals alone.

## Limits

- The block on exit portals runs in the client's game, like every portal rule. It stops players using the mod, not a modified client.
- Directions need a linked pair (same tag). Unlinked portals can only be two-way.
- A portal changed while its partner is far away is written by the server to both, so both ends update together.

## Building

Built from the [DrakesModStub](https://github.com/drakethos/DrakesModStub) template with its Ui module. Self-contained: no DrakeModsLibs.
Machine paths in `environment.props` (copy `environment.props.example`, or `.linux`). Windows: Visual Studio / `msbuild DrakesPortals.csproj`.
Linux: `build/scripts/build-linux.sh`. Tag `vX.Y.Z` (matching `<Version>`) to release to Hexium.
Tests: see `Tests/README.md`.
