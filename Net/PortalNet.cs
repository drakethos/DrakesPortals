using System;
using DrakesPortals.Core;
using UnityEngine;

namespace DrakesPortals.Net;

/// <summary>
/// Changing a direction is a request to the server. The server checks who is asking (admin or not, per the synced
/// ManageAccess setting), that the target really is a portal, and that it is linked, then writes both ends.
/// </summary>
public static class PortalNet
{
    public const string Rpc = "DrakesPortals_SetDirection";
    private static bool _registered;

    public static void Register()
    {
        if (_registered || ZRoutedRpc.instance == null)
            return;
        ZRoutedRpc.instance.Register<ZDOID, int>(Rpc, OnSetDirection);
        _registered = true;
    }

    /// <summary>Called from a client (or the host). Returns false when nothing could be sent.</summary>
    public static bool Request(TeleportWorld? portal, PortalDirection direction)
    {
        var zdo = PortalState.Zdo(portal);
        if (zdo == null || ZRoutedRpc.instance == null)
            return false;
        ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), Rpc, zdo.m_uid, (int)direction);
        return true;
    }

    private static void OnSetDirection(long sender, ZDOID id, int raw)
    {
        if (ZNet.instance == null || !ZNet.instance.IsServer())
            return;
        try
        {
            if (raw < 0 || raw > (int)PortalDirection.Exit)
                return;
            if (!SenderMayManage(sender))
            {
                DrakesPortals.Log?.LogInfo($"Direction change from {sender} refused (not allowed by ManageAccess).");
                return;
            }

            var zdo = ZDOMan.instance?.GetZDO(id);
            if (zdo == null || !IsPortal(zdo))
                return;

            var direction = (PortalDirection)raw;
            var other = PortalState.LinkedZdo(zdo);
            if (direction != PortalDirection.TwoWay && other == null)
                return; // one-way needs a linked pair

            Write(zdo, direction);
            if (other != null)
                Write(other, Opposite(direction));
        }
        catch (Exception ex)
        {
            DrakesPortals.Log?.LogError($"SetDirection failed: {ex}");
        }
    }

    public static PortalDirection Opposite(PortalDirection d) => d switch
    {
        PortalDirection.Entry => PortalDirection.Exit,
        PortalDirection.Exit => PortalDirection.Entry,
        _ => PortalDirection.TwoWay,
    };

    private static bool SenderMayManage(long sender)
    {
        var rpc = ZRoutedRpc.instance;
        // The server's own player (single player, or the host of a listen server) counts as admin.
        if (rpc != null && sender == rpc.GetServerPeerID())
            return PortalAccess.CanManage(true);
        var peer = ZNet.instance.GetPeer(sender);
        var admin = peer?.m_socket != null && ZNet.instance.IsAdmin(peer.m_socket.GetHostName());
        return PortalAccess.CanManage(admin);
    }

    private static bool IsPortal(ZDO zdo)
    {
        var prefab = ZNetScene.instance != null ? ZNetScene.instance.GetPrefab(zdo.GetPrefab()) : null;
        return prefab != null && prefab.GetComponent<TeleportWorld>() != null;
    }

    private static void Write(ZDO zdo, PortalDirection direction)
    {
        zdo.SetOwner(ZDOMan.GetSessionID());
        zdo.Set(PortalState.DirectionKey, (int)direction);
    }
}
