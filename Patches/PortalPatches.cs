namespace DrakesPortals.Patches;

// Placeholder. No Harmony patches yet on purpose: the gate trigger (vanilla TeleportWorld portal, a door, or an
// invisible zone) is the first design decision and its patch targets must be checked against the Valheim 1.0
// assemblies before anything is attributed here. See docs/design.md ("Open questions").
//
// When added, annotate patch classes with [HarmonyPatch]; DrakesPortals.Awake applies them through ModGuard.PatchAll,
// which isolates each class and names any that fail to apply.
internal static class PortalPatches
{
}
