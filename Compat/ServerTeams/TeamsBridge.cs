using System;
using System.Reflection;

namespace DrakesPortals.Compat.ServerTeams;

/// <summary>
/// Soft link to DrakesServerTeams (same approach as LockSmith's TeamWardBridge): no assembly reference, so the mod
/// loads without Teams. Every call returns "no team" when Teams is missing or its API changed.
/// </summary>
internal static class TeamsBridge
{
    public const string PluginGuid = "com.drakemods.serverteams";
    private const string ApiType = "DrakesServerTeams.TeamsAPI";

    private static bool _looked;
    private static MethodInfo? _getTeamId;

    /// <summary>True when DrakesServerTeams is loaded and its TeamsAPI.GetTeamId(Player) was found.</summary>
    public static bool Available
    {
        get
        {
            Ensure();
            return _getTeamId != null;
        }
    }

    public static void Probe()
    {
        Ensure();
        DrakesPortals.Log?.LogInfo(_getTeamId != null
            ? "DrakesServerTeams found: team gates available."
            : "DrakesServerTeams not found: team gates disabled.");
    }

    /// <summary>Team id of the player, or null (no Teams, no team, or any failure).</summary>
    public static string? GetTeamId(Player? player)
    {
        if (player == null)
            return null;
        Ensure();
        try
        {
            return _getTeamId?.Invoke(null, new object[] { player }) as string;
        }
        catch
        {
            return null;
        }
    }

    private static void Ensure()
    {
        if (_looked)
            return;
        _looked = true;
        try
        {
            if (!BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(PluginGuid))
                return;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(ApiType, false);
                if (type == null)
                    continue;
                _getTeamId = type.GetMethod("GetTeamId", BindingFlags.Public | BindingFlags.Static, null,
                    new[] { typeof(Player) }, null);
                return;
            }
        }
        catch
        {
            _getTeamId = null;
        }
    }
}
