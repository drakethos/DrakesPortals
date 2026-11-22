# DrakesPortals design

## Core rule
A linked portal pair can be one way. One end is the **Entry** (players step in and arrive at the other), the other is the **Exit**
(arrival only; stepping into it is refused). Direction is stored on each portal's ZDO (`DrakesPortals_direction`: 0 two-way, 1 entry,
2 exit), so every client sees it.

## Parts
- `Core/`: direction enum, the access rule (`PortalAccess.CanManage(isAdmin)`), reading state (`PortalState`).
- `Net/PortalNet`: a routed RPC to the server. The server checks the sender (ManageAccess, admin list), that the ZDO is a portal,
  that one-way has a linked partner, then writes both ends (the partner gets the opposite direction).
- `Patches/`: `TeleportWorld.Teleport` (refuse on an exit), `Interact` (Shift+E opens the menu), `GetHoverText` (says which way),
  `Update` (glow tint), `Game.Start` (registers the RPC and the soft Libs claim).
- `Menu/PortalMenu`: the Shift+E menu on the template's standalone UI module.
- Config: Jotunn's sync (`IsAdminOnly`) for `Enabled` and `ManageAccess`; display options are per client.

## Not done (next)
- Per-portal "who may enter" (team via DrakesServerTeams, boss kills), use limits per player, admin-only gates.
- Destination resolvers (fixed position, item, condition, puzzle) so a portal needs no partner; see the earlier design notes in the changelog history.
- Secret door portals.

## Open questions
- Portal rules run in the client (as in vanilla); a modified client can ignore the exit block.
- The glow tint sets `TeleportWorld.m_colorTargetfound`; whether every part of the effect follows it needs a look in game.
- Linking is done by the game some seconds after tags match; the menu only offers one-way once `HaveTarget()` is true.
