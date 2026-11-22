using BepInEx;
using BepInEx.Logging;
using DrakesPortals.Defense;
using DrakesPortals.Ui;
using HarmonyLib;
using Jotunn;
using Jotunn.Utils;

namespace DrakesPortals
{
    // GUID, ModName and Version are generated from mod.package.props (ModInfo.g.cs): edit them there.
    [BepInPlugin(GUID, ModName, Version)]
    [BepInDependency(Main.ModGuid)]
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
            try
            {
                DrakesPortalsConfig.Bind(Config);
                UiSettings.Bind(Config);
            }
            finally { Config.SaveOnConfigSet = saveOnSet; }
            Config.Save();

            // Patch classes are applied one by one, runtime errors are attributed to the mod that threw them,
            // and BepInEx/DrakeMods-compat-report.txt lists other mods patching the same methods.
            var guard = ModGuard.Create(ModName, GUID, Version, typeof(DrakesPortals).Assembly, _harmony, Logger);
            guard.ReportTo = "the mod's Hexium page (https://valheim.hexium.gg/mods/DrakeMods/DrakesPortals, Threads tab)";
            guard.PatchAll();
            guard.Arm();

            Logger.LogInfo($"{ModName} {Version} Awake.");
        }
    }
}
