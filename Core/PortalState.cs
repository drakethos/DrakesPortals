using UnityEngine;

namespace DrakesPortals.Core;

/// <summary>Reads a portal's direction and answers "can a player step into it".</summary>
public static class PortalState
{
    public static readonly int DirectionKey = "DrakesPortals_direction".GetStableHashCode();

    public static ZDO? Zdo(TeleportWorld? portal)
    {
        if (!portal)
            return null;
        var view = portal!.m_nview;
        return view != null && view.IsValid() ? view.GetZDO() : null;
    }

    public static PortalDirection Get(TeleportWorld? portal)
    {
        var zdo = Zdo(portal);
        if (zdo == null)
            return PortalDirection.TwoWay;
        var value = zdo.GetInt(DirectionKey, 0);
        return value == (int)PortalDirection.Entry || value == (int)PortalDirection.Exit
            ? (PortalDirection)value
            : PortalDirection.TwoWay;
    }

    /// <summary>The same check the Teleport patch makes: an exit portal refuses players while the mod is on.</summary>
    public static bool WouldBlock(TeleportWorld? portal) =>
        DrakesPortalsConfig.Enabled && Get(portal) == PortalDirection.Exit;

    /// <summary>The portal this one is linked to (same tag, found by the game), or null.</summary>
    public static ZDO? LinkedZdo(ZDO? zdo)
    {
        if (zdo == null || ZDOMan.instance == null)
            return null;
        var id = zdo.GetConnectionZDOID(ZDOExtraData.ConnectionType.Portal);
        return id.IsNone() ? null : ZDOMan.instance.GetZDO(id);
    }

    public static TeleportWorld? FromObject(GameObject? go)
    {
        if (!go)
            return null;
        return go!.GetComponentInChildren<TeleportWorld>() ?? go.GetComponentInParent<TeleportWorld>();
    }

    public static string Describe(PortalDirection direction) => direction switch
    {
        PortalDirection.Entry => "One-way: Entry (leads to the linked portal)",
        PortalDirection.Exit => "One-way: Exit (arrival only)",
        _ => "Two-way",
    };
}
