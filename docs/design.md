# DrakesPortals design

## Core rule
Every gate is one way. A gate has an entry and sends the player to a destination. It never offers a way back through
itself. Everything else (who, how often, where to) is a rule around that.

## Resolvers
`IDestinationResolver.Resolve(ResolveContext)` returns a position and rotation, or a refusal message shown to the player.
`ResolverRegistry` maps a name to a resolver; other mods can register theirs at startup.

| Resolver | Status | Behaviour |
|---|---|---|
| Fixed | in 0.1.0 | Configured `x,y,z` |
| Item | idea | Held item picks the destination |
| Condition | idea | Team, global key, time of day, use count |
| Random / weighted | idea | Pick from a list |
| Puzzle | idea | World state (switch, pedestal item) |
| Chain | idea | Try one, fall back to the next |

Build the second resolver before fixing the interface; it will show what `ResolveContext` is missing.

## Possible triggers (not decided)
- A vanilla portal piece (entry), with no exit piece needed.
- A door (a hidden or admin-assigned "secret" gate).
- An invisible trigger zone.

## Rules to add after the first gate works
- Who may enter: team (DrakesServerTeams, soft), boss kills (global keys), admin only.
- Per-player use limits (once ever, N times, per day), stored on the server.
- Point of no return: optionally reset bed/respawn to the landing area.

## Open questions (check before coding)
- Valheim portals are largely client-driven. Can the server enforce a gate, or must it be on every client with the server
  as authority for per-player data? Verify against the Valheim 1.0 assemblies.
- Which patch target stops a player using the entry a second time without breaking vanilla portals?
- Dungeon destinations depend on chunk loading; does a named location load in time?
- Whether ModGuard should move into DrakeModsLibs. Today it only exists as a vendored copy in DrakesRenameIt (Libs 1.0.6 has none), so this repo carries its own copy in `Defense/ModGuard.cs` under `DrakesPortals.Defense`.
