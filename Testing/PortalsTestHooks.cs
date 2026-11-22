using System;
using System.Collections.Generic;
using System.Linq;
using DrakesPortals.Core;
using DrakesPortals.Menu;
using DrakesPortals.Net;
using UnityEngine;

namespace DrakesPortals.Testing;

/// <summary>DrakesTestLab finds methods marked with an attribute named <c>TestHookAttribute</c>. No reference to DrakesTestLab.</summary>
[AttributeUsage(AttributeTargets.Method)]
internal sealed class TestHookAttribute : Attribute
{
    public TestHookAttribute(string name) => Name = name;
    public string Name { get; }
}

/// <summary>
/// Test hooks for the DrakesPortals suite. Actions go through the same calls the Shift+E menu and the Teleport patch make.
/// Target = a tagged portal (arg "target"), else the nearest one.
/// </summary>
internal static class PortalsTestHooks
{
    static Player P => Player.m_localPlayer ?? throw new InvalidOperationException("no local player");

    static TeleportWorld Portal(GameObject? target)
    {
        var portal = PortalState.FromObject(target);
        if (portal != null)
            return portal;
        portal = UnityEngine.Object.FindObjectsOfType<TeleportWorld>()
            .OrderBy(t => Vector3.Distance(t.transform.position, P.transform.position))
            .FirstOrDefault();
        return portal ?? throw new InvalidOperationException("no portal nearby");
    }

    static PortalDirection Parse(string direction) =>
        Enum.TryParse<PortalDirection>(direction, true, out var d) ? d : throw new ArgumentException($"unknown direction '{direction}'");

    /// <summary>Give the portal a tag, as typing in the vanilla prompt does. Two portals with the same tag link.</summary>
    [TestHook("portals.setTag")]
    static bool SetTag(string tag, GameObject? target = null)
    {
        Portal(target).SetText(tag);
        return true;
    }

    /// <summary>The portal's text (tag).</summary>
    [TestHook("portals.tag")]
    static string Tag(GameObject? target = null) => Portal(target).GetText();

    /// <summary>True when the game has linked this portal to another.</summary>
    [TestHook("portals.linked")]
    static bool Linked(GameObject? target = null) => Portal(target).HaveTarget();

    /// <summary>TwoWay, Entry or Exit.</summary>
    [TestHook("portals.direction")]
    static string Direction(GameObject? target = null) => PortalState.Get(Portal(target)).ToString();

    /// <summary>Ask the server for a direction, as the menu does. True when the request was sent; read the result with portals.direction.</summary>
    [TestHook("portals.setDirection")]
    static bool SetDirection(string direction, GameObject? target = null) =>
        PortalNet.Request(Portal(target), Parse(direction));

    /// <summary>What a click on the menu row does (closes the menu, then the same request).</summary>
    [TestHook("portals.pick")]
    static bool Pick(string direction, GameObject? target = null)
    {
        PortalMenu.Pick(Portal(target), Parse(direction));
        return true;
    }

    /// <summary>The Teleport patch's rule: would stepping into this portal be refused right now.</summary>
    [TestHook("portals.blocked")]
    static bool Blocked(GameObject? target = null) => PortalState.WouldBlock(Portal(target));

    /// <summary>Is this choice offered in the menu for this portal (linked, and not the current state).</summary>
    [TestHook("portals.offered")]
    static bool Offered(string direction, GameObject? target = null) =>
        PortalMenu.IsOffered(Portal(target), Parse(direction));

    /// <summary>The access rule with a given admin flag (the server uses the same function for the sender).</summary>
    [TestHook("portals.canManage")]
    static bool CanManage(bool admin) => PortalAccess.CanManage(admin);

    /// <summary>Can the local player open the menu (host / single player = admin).</summary>
    [TestHook("portals.canManageHere")]
    static bool CanManageHere() => PortalAccess.LocalCanManage();

    /// <summary>The portal's hover text, as shown on screen.</summary>
    [TestHook("portals.hover")]
    static string Hover(GameObject? target = null) => Portal(target).GetHoverText();

    /// <summary>Open the Shift+E menu on the portal. True when it opened.</summary>
    [TestHook("portals.openMenu")]
    static bool OpenMenu(GameObject? target = null) => PortalMenu.Open(Portal(target));

    /// <summary>The glow colour the portal uses right now, as "r,g,b" (0-255).</summary>
    [TestHook("portals.glow")]
    static string Glow(GameObject? target = null)
    {
        var c = Portal(target).m_colorTargetfound;
        return $"{Mathf.RoundToInt(c.r * 255)},{Mathf.RoundToInt(c.g * 255)},{Mathf.RoundToInt(c.b * 255)}";
    }
}
