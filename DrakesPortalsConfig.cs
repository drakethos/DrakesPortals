using BepInEx.Configuration;
using DrakesPortals.Core;
using Jotunn.Configs;
using UnityEngine;

namespace DrakesPortals;

/// <summary>
/// Synced settings use Jotunn's own sync (IsAdminOnly): the server's values reach every client and only admins can
/// change them in game. Display settings are per client and never synced.
/// </summary>
public static class DrakesPortalsConfig
{
    private const string SectionAdmin = "01 Admin";
    private const string SectionPortals = "02 Portals";
    private const string SectionDisplay = "03 Display";

    private static ConfigEntry<bool> _enabled = null!;
    private static ConfigEntry<ManageAccess> _manageAccess = null!;
    private static ConfigEntry<bool> _showTint = null!;
    private static ConfigEntry<Color> _entryColor = null!;
    private static ConfigEntry<Color> _exitColor = null!;

    /// <summary>Master switch (synced). Off = every portal behaves like vanilla, directions are kept but ignored.</summary>
    public static bool Enabled => _enabled.Value;

    /// <summary>Who may change a portal's direction with Shift+E (synced).</summary>
    public static ManageAccess ManageAccess => _manageAccess.Value;

    public static bool ShowTint => _showTint.Value;
    public static Color EntryColor => _entryColor.Value;
    public static Color ExitColor => _exitColor.Value;

    private static ConfigEntry<T> Bind<T>(ConfigFile config, string section, string key, T value, string description, bool synced)
    {
        var attributes = new ConfigurationManagerAttributes { IsAdminOnly = synced };
        return config.Bind(section, key, value, new ConfigDescription(description, null, attributes));
    }

    public static void Bind(ConfigFile config)
    {
        _enabled = Bind(config, SectionAdmin, "Enabled", true,
            "Master switch for one-way portals. Off = every portal works like vanilla.", synced: true);
        _manageAccess = Bind(config, SectionPortals, "ManageAccess", ManageAccess.AdminOnly,
            "Who may change a portal's direction with Shift+E: AdminOnly or Everyone. The server checks every request.", synced: true);
        _showTint = Bind(config, SectionDisplay, "ShowDirectionTint", true,
            "Tint the portal glow by direction: entry and exit get their own colour. This device only.", synced: false);
        _entryColor = Bind(config, SectionDisplay, "EntryColor", new Color(0.35f, 0.85f, 1f, 1f),
            "Glow colour of an Entry portal (r,g,b,a). This device only.", synced: false);
        _exitColor = Bind(config, SectionDisplay, "ExitColor", new Color(1f, 0.55f, 0.2f, 1f),
            "Glow colour of an Exit portal (r,g,b,a). This device only.", synced: false);
    }
}
