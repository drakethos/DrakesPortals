# Portal mod landscape (2026-11-23)

How this was checked: package lists of Thunderstore (14,893) and Hexium (2,658); 694 + 207 name/description matches for portal, teleport, waypoint, gate and similar;
then the full READMEs of the 146 Hexium packages that match (119 are really about portals) through Hexium's readme API. Keyword scans, so a feature a README does not
mention is invisible, and Thunderstore-only mods were judged on their one-line description only. Treat "none found" as "none found in these READMEs".

## Already covered (do not rebuild)

| Idea | Who already does it |
|---|---|
| One-way links, entry / exit | Rare Magic Portal Plus (One-Way, One-Way Password Locked), Portaler ("one-way links", Swap), PortalFlare (one-way routes, receive-only destinations), XPortalNetworks, TortalPortal (corpse-run gate) |
| Who may use a portal | PortalRules (Personal / Public / Invite / Clan / Tagged, Shift+Interact cycles the mode, server checks), Rare Magic Portal Plus (Allowed Users, Password, Admin-Only) |
| Destination without a partner portal | Rare Magic Portal Plus (Coordinates, Random Teleport), TortalPortal Lite (phantom anchors), PortalRules (Admin portals, location portals) |
| Boss / progression gates | PortalRules (GlobalKey gates), TortalPortal Lite (Sealed Gate on a global key), BossGatedPortals, ServersideQoL_PortalProgression, ZenPortal |
| Costs, cooldowns, caps | PortalRules (coin fares, cooldowns, limits), Tollbound (tolls per biome), TortalPortal Lite (caps) |
| Destination pickers, maps, favorites, previews | Portaler, TortalPortal, PortalRules, TheGreatestPortal, PortalPreview, Immersive_Portals, many more |
| Dungeon / instance style trips | Rift, Beyond, Fires Alternate Dimensions |

## Not found (candidates for something new)

- Portals that open because of **world state**: a pressure plate or lever held, an item on a pedestal, a rune order, a timed window (night, moon, weather).
  PressurePlate and LatchLink exist for doors and gates, Sanctums has puzzles, balrond_runerails has signals; no portal mod connects to any of them.
- **Per-character use limits for travel** ("once ever", "3 times"). Only a corpse-run gate (once per death), cooldowns and a once-per-character reward in Rift turned up.
- **Saying why a gate is shut**: what a gate is still waiting for, shown to the player. Not seen in the READMEs read (not proof it is absent).
- **Server Teams aware gates** (DrakesServerTeams): PortalRules does Clan / Guild through other mods, nobody uses server-wide teams.

DrakesPortals 0.1.0 (linked Entry / Exit, Shift+E, server-checked) is, in function, in the first table. What it has that others may not: no dependency on a
shared library, a server-checked request, and an automated test suite. That is a base, not a reason to publish.
