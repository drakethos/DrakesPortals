using System;
using BepInEx;
using BepInEx.Logging;
using DrakeModsLibs;
using DrakesPortals.Compat.ServerTeams;
using DrakesPortals.Core;
using DrakesPortals.Defense;
using HarmonyLib;
using Jotunn;
using Jotunn.Utils;

namespace DrakesPortals
{
    [BepInPlugin(GUID, ModName, Version)]
    [BepInDependency(Main.ModGuid)]
    [BepInDependency(CustomizeLibsPlugin.GUID)]
    // Soft: only used to gate portals by team when DrakesServerTeams is installed.
    [BepInDependency(TeamsBridge.PluginGuid, BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    public partial class DrakesPortals : BaseUnityPlugin
    {
        public static DrakesPortals Instance { get; private set; } = null!;

        public static ManualLogSource? Log { get; private set; }

        private readonly Harmony _harmony = new Harmony(GUID);

        private void Awake()
        {
            Instance = this;
            Log = Logger;

            // BepInEx rewrites the whole .cfg on every Bind by default: batch them into one save.
            var saveOnSet = Config.SaveOnConfigSet;
            Config.SaveOnConfigSet = false;
            try { DrakesPortalsConfig.Bind(Config, Logger); }
            finally { Config.SaveOnConfigSet = saveOnSet; }
            Config.Save();

            ResolverRegistry.RegisterBuiltIns();

            // Defensive host: patch classes are applied one by one, runtime errors are attributed to the mod that
            // threw them, and BepInEx/DrakeMods-compat-report.txt lists which other mods touch the same methods.
            var guard = ModGuard.Create(ModName, GUID, Version, typeof(DrakesPortals).Assembly, _harmony, Logger);
            guard.ReportTo = "the mod's Hexium page (https://valheim.hexium.gg/mods/DrakeMods/DrakesPortals, Threads tab)";
            guard.PatchAll();
            guard.Try("soft-dependency: DrakesServerTeams", TeamsBridge.Probe);
            guard.Arm();

            Logger.LogInfo($"{ModName} {Version} Awake.");
        }
    }
}
