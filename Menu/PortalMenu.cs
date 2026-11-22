using System.Collections.Generic;
using DrakesPortals.Core;
using DrakesPortals.Net;
using DrakesPortals.Ui;

namespace DrakesPortals.Menu;

/// <summary>The Shift+E menu on a portal: pick Two-way, Entry or Exit. Built with the shared wood menu.</summary>
public static class PortalMenu
{
    private static readonly DrakeWoodActionMenu Menu = new DrakeWoodActionMenu("drakesportals_menu");

    public const string TwoWayLabel = "Two-way (normal portal)";
    public const string EntryLabel = "One-way: this is the Entry";
    public const string ExitLabel = "One-way: this is the Exit";

    public static string Label(PortalDirection direction) => direction switch
    {
        PortalDirection.Entry => EntryLabel,
        PortalDirection.Exit => ExitLabel,
        _ => TwoWayLabel,
    };

    /// <summary>Whether a choice is offered: one-way needs a linked pair, and the current state is not offered again.</summary>
    public static bool IsOffered(TeleportWorld portal, PortalDirection choice)
    {
        if (choice == PortalState.Get(portal))
            return false;
        return choice == PortalDirection.TwoWay || portal.HaveTarget();
    }

    public static bool Open(TeleportWorld? portal)
    {
        if (!portal || !PortalAccess.LocalCanManage())
            return false;
        var p = portal!;
        var tag = p.GetText();
        var current = PortalState.Get(p);
        var subtitle = $"{(string.IsNullOrEmpty(tag) ? "(no tag)" : tag)}: {PortalState.Describe(current)}";
        if (!p.HaveTarget())
            subtitle += "\nNot linked yet. Give two portals the same tag first.";

        var actions = new List<DrakeMenuAction>();
        foreach (var choice in new[] { PortalDirection.TwoWay, PortalDirection.Entry, PortalDirection.Exit })
        {
            var captured = choice;
            actions.Add(new DrakeMenuAction(Label(choice), () => Pick(p, captured), IsOffered(p, choice)));
        }

        Menu.Open("Portal direction", subtitle, actions);
        return true;
    }

    /// <summary>What a click on a menu row does. Public so tests use the same path.</summary>
    public static void Pick(TeleportWorld? portal, PortalDirection choice)
    {
        Menu.Close();
        PortalNet.Request(portal, choice);
    }

    public static void Close() => Menu.Close();
}
