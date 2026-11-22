using DrakesPortals.Integration;
using DrakesPortals.Net;
using HarmonyLib;

namespace DrakesPortals.Patches;

/// <summary>Registers the routed RPC and the soft Libs claim once the game (and so every plugin) has started.</summary>
[HarmonyPatch(typeof(Game), nameof(Game.Start))]
internal static class GameStartPatch
{
    [HarmonyPostfix]
    private static void Postfix()
    {
        PortalNet.Register();
        LibsClaims.TryRegister();
    }
}
