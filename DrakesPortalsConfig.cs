using BepInEx.Configuration;
using BepInEx.Logging;
using DrakeModsLibs.Sync;

namespace DrakesPortals;

/// <summary>Synced gameplay config. Count must match <see cref="ExpectedSyncedEntryCount"/>.</summary>
public static class DrakesPortalsConfig
{
    /// <summary>Every <c>BindSynced</c> call below, including the admin lock. Bump when adding synced binds.</summary>
    public const int ExpectedSyncedEntryCount = 4;

    private const string SectionAdmin = "01 Admin";
    private const string SectionPortals = "02 Portals";

    private static readonly DrakeConfigSync Sync = DrakeConfigSync.Create(
        DrakesPortals.ModName,
        DrakesPortals.ModName,
        DrakesPortals.Version);

    private static ConfigEntry<bool> _lockSyncedConfig = null!;
    private static ConfigEntry<bool> _enabled = null!;
    private static ConfigEntry<string> _defaultResolver = null!;
    private static ConfigEntry<string> _fixedDestination = null!;

    public static bool LockSyncedConfig => _lockSyncedConfig.Value;

    /// <summary>Master switch. Off = every portal behaves like vanilla.</summary>
    public static bool Enabled => _enabled.Value;

    /// <summary>Name of the destination resolver new gates use (see <c>ResolverRegistry</c>).</summary>
    public static string DefaultResolver => _defaultResolver.Value;

    /// <summary>"x,y,z" world position used by the Fixed resolver.</summary>
    public static string FixedDestination => _fixedDestination.Value;

    public static void Bind(ConfigFile config, ManualLogSource log)
    {
        _lockSyncedConfig = Sync.BindSynced(
            config, SectionAdmin, "Admin", "LockSyncedConfig", true,
            "When true, only the host/admin can change synced DrakesPortals settings.");

        _enabled = Sync.BindSynced(
            config, SectionPortals, "Portals", "Enabled", true,
            "Master switch for one-way gates.");

        _defaultResolver = Sync.BindSynced(
            config, SectionPortals, "Portals", "DefaultResolver", "Fixed",
            "Which destination resolver gates use. Only 'Fixed' exists in 0.1.0.");

        _fixedDestination = Sync.BindSynced(
            config, SectionPortals, "Portals", "FixedDestination", "0,0,0",
            "World position 'x,y,z' the Fixed resolver sends players to.");

        Sync.AddLockingConfigEntry(_lockSyncedConfig);
        Sync.FinalizeBinding(log, ExpectedSyncedEntryCount, () => _lockSyncedConfig.Value);
    }
}
