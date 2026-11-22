# DrakesPortals automated tests

DrakesTestLab suite for DrakesPortals. It runs unattended; results go to `<profile>\BepInEx\config\DrakesTestLab\results\latest.html`.

- `suites/DrakesPortals/` (generated, do not edit the JSON): `python gen_portals_suite.py`. `Tests/` is never packaged.
- `../Testing/PortalsTestHooks.cs`: the `portals.*` hooks. Debug builds only. They use the mod's real code paths (the same request
  the menu sends, the same rule the Teleport patch checks); no reference to DrakesTestLab.

Run: in game F9 > Tests, or with DrakesTestLab's `Run-TestLabSuites.ps1 -Suites DrakesPortals`.

## Covered (11 tests)

New portals are two-way; one-way needs a linked pair (the server refuses it otherwise); Entry on A makes B the exit and the other
way round; Two-way resets both; the hover text says which way; Shift+E opens the menu and a click sets the direction; switched off
means vanilla; ManageAccess (admin only / everyone); the direction survives a relog; the glow colour.

## Not covered

- A real second client (non-admin) trying to change a direction: the server-side admin check is exercised only through the host.
  Needs a TestLab `peerJoin` test with a routed RPC, or a manual two-player test.
- Actually walking into a portal (the Teleport patch is checked through its rule, `portals.blocked`, and not through a real teleport).
- Written from the TestLab authoring guide; not run yet. Step details (spawn / wait / link timing, the click label) may need a first-run fix.
