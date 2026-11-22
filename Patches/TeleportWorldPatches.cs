using System.Runtime.CompilerServices;
using DrakesPortals.Core;
using DrakesPortals.Menu;
using HarmonyLib;
using UnityEngine;

namespace DrakesPortals.Patches;

/// <summary>An exit portal refuses players. Everything else about the portal stays vanilla.</summary>
[HarmonyPatch(typeof(TeleportWorld), nameof(TeleportWorld.Teleport))]
internal static class TeleportPatch
{
    [HarmonyPrefix]
    private static bool Prefix(TeleportWorld __instance)
    {
        if (!PortalState.WouldBlock(__instance))
            return true;
        MessageHud.instance?.ShowMessage(MessageHud.MessageType.Center, "This portal is exit only");
        return false;
    }
}

/// <summary>Shift+E opens the direction menu (when this player may change directions). Plain E stays the vanilla tag prompt.</summary>
[HarmonyPatch(typeof(TeleportWorld), nameof(TeleportWorld.Interact))]
internal static class InteractPatch
{
    [HarmonyPrefix]
    private static bool Prefix(TeleportWorld __instance, bool hold, bool alt, ref bool __result)
    {
        if (!alt || hold || !PortalAccess.LocalCanManage())
            return true;
        __result = PortalMenu.Open(__instance);
        return !__result;
    }
}

/// <summary>Says which way the portal goes, and hints the menu key.</summary>
[HarmonyPatch(typeof(TeleportWorld), nameof(TeleportWorld.GetHoverText))]
internal static class HoverPatch
{
    [HarmonyPostfix]
    private static void Postfix(TeleportWorld __instance, ref string __result)
    {
        if (!DrakesPortalsConfig.Enabled)
            return;
        switch (PortalState.Get(__instance))
        {
            case PortalDirection.Entry:
                __result += "\n<color=#5ad7ff>One-way: step in here, you arrive at the linked portal</color>";
                break;
            case PortalDirection.Exit:
                __result += "\n<color=#ff8c3a>Exit only: you can arrive here, not leave through it</color>";
                break;
        }

        if (PortalAccess.LocalCanManage())
            __result += "\n[<color=yellow><b>Shift+E</b></color>] Portal direction";
    }
}

/// <summary>Tints the glow by direction. Only runs the colour change when the direction (or the setting) changed.</summary>
[HarmonyPatch(typeof(TeleportWorld), nameof(TeleportWorld.Update))]
internal static class TintPatch
{
    private sealed class Applied
    {
        public PortalDirection Direction;
        public bool Tinted;
        public Color Original;
    }

    private static readonly ConditionalWeakTable<TeleportWorld, Applied> State = new ConditionalWeakTable<TeleportWorld, Applied>();

    [HarmonyPostfix]
    private static void Postfix(TeleportWorld __instance)
    {
        var want = DrakesPortalsConfig.Enabled && DrakesPortalsConfig.ShowTint ? PortalState.Get(__instance) : PortalDirection.TwoWay;
        var applied = State.GetValue(__instance, p => new Applied { Original = p.m_colorTargetfound });
        if (applied.Direction == want)
            return;
        applied.Direction = want;
        __instance.m_colorTargetfound = want switch
        {
            PortalDirection.Entry => DrakesPortalsConfig.EntryColor,
            PortalDirection.Exit => DrakesPortalsConfig.ExitColor,
            _ => applied.Original,
        };
    }
}
