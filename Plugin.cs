using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace HexRareScanner
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "com.hex.rarescanner";
        private const string PluginName = "HexRareScanner";
        private const string PluginVersion = "1.0.0";
        
        private Harmony _harmonyInstance;

        private static ConfigEntry<bool> _isModEnabled;
        private static ConfigEntry<bool> _trackSeaSerpents;
        private static ConfigEntry<bool> _trackTrolls;
        private static ConfigEntry<bool> _trackBlackForestBears;
        private static ConfigEntry<bool> _trackVileBears;
        
        internal static bool IsModEnabled => _isModEnabled?.Value ?? false;
        internal static bool TrackSeaSerpents => _trackSeaSerpents?.Value ?? false;
        internal static bool TrackTrolls => _trackTrolls?.Value ?? false;
        internal static bool TrackBlackForestBears => _trackBlackForestBears?.Value ?? false;
        internal static bool TrackVileBears => _trackVileBears?.Value ?? false;
        
        internal static ManualLogSource Log;
        internal static Plugin Instance;

        private void Awake()
        {
            Instance = this;
            Log = Logger;

            InitializeConfig();

            _harmonyInstance = new Harmony(PluginGuid);
            _harmonyInstance.PatchAll();

            Log.LogInfo($"{PluginName} v{PluginVersion} loaded.");
        }

        private void OnDestroy()
        {
            Log.LogInfo($"{PluginName} v{PluginVersion} unloaded.");

            _harmonyInstance?.UnpatchSelf();
            _harmonyInstance = null;
            Instance = null;
            Log = null;
        }

        private void InitializeConfig()
        {
            _isModEnabled = Config.Bind("General", "IsModEnabled", true, "Enable or disable the mod.");
            _trackSeaSerpents = Config.Bind("Tracking", "Track SeaSerpents", true, "Enable or disable tracking of Sea Serpents.");
            _trackTrolls = Config.Bind("Tracking", "Track Trolls", true, "Enable or disable tracking of Trolls.");
            _trackBlackForestBears = Config.Bind("Tracking", "Track Black Forest Bears", true, "Enable or disable tracking of Black Forest Bears.");
            _trackVileBears = Config.Bind("Tracking", "Track Vile Bears", true, "Enable or disable tracking of Vile Bears.");
        }

        internal static bool IsTrackedPrefab(string prefabName)
        {
            if (!IsModEnabled)
            {
                return false;
            }

            Log.LogInfo($"Prefab name: {prefabName}");

            switch (prefabName)
            {
                case "Serpent":
                    return TrackSeaSerpents;
                case "Troll":
                    return TrackTrolls;
                case "Bjorn":
                    return TrackBlackForestBears;
                case "Unbjorn":
                    return TrackVileBears;
                default:
                    return false;
            }
        }

        internal static string GetDisplayName(string prefabName)
        {
            switch(prefabName)
            {
                case "Serpent":
                    return "Sea Serpent";
                case "Troll":
                    return "Troll";
                case "Bjorn":
                    return "Black Forest Bear";
                case "Unbjorn":
                    return "Vile Bear";
                default:
                    return prefabName;
            }
        }
    }
}
