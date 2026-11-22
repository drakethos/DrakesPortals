namespace DrakesPortals.Core;

/// <summary>Who may change a portal's direction (Shift+E menu). Server setting; the server checks it again on every request.</summary>
public enum ManageAccess
{
    AdminOnly = 0,
    Everyone = 1,
}

public static class PortalAccess
{
    /// <summary>The rule, with the admin flag passed in so the server (which knows the sender) and the menu (which knows the local player) use the same code.</summary>
    public static bool CanManage(bool isAdmin) =>
        DrakesPortalsConfig.Enabled && (DrakesPortalsConfig.ManageAccess == ManageAccess.Everyone || isAdmin);

    /// <summary>Local player: single player and the host count as admin.</summary>
    public static bool LocalCanManage() =>
        CanManage(ZNet.instance != null && ZNet.instance.LocalPlayerIsAdminOrHost());
}
