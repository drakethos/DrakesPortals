# Changelog

## 0.1.0

First version (not released yet).

- One-way portals on a linked pair: **Entry** and **Exit**, set with Shift+E (Two-way, One-way Entry, One-way Exit). Choosing one sets the linked portal to the opposite end.
- An exit portal refuses players. The hover text says which way a portal goes; the glow is tinted by direction (this device only, configurable).
- `ManageAccess` (AdminOnly or Everyone) and `Enabled`, synced from the server; the server checks who is asking and that the pair is linked.
- Menu built with the template's standalone UI module; no DrakeModsLibs needed.
- DrakesTestLab suite (11 tests) and `portals.*` test hooks (Debug builds).
