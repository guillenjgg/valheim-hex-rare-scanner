using System.Collections.Generic;
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
        private static ConfigEntry<bool> _playTrackedCreatureSound;
        private static readonly Dictionary<string, TrackedCreatureSetting> TrackedCreatures = new Dictionary<string, TrackedCreatureSetting>();
        
        internal static bool IsModEnabled => _isModEnabled?.Value ?? false;
        internal static bool PlayTrackedCreatureSound => _playTrackedCreatureSound?.Value ?? false;
        
        internal static ManualLogSource Log;

        private void Awake()
        {
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
            Log = null;
        }

        private void InitializeConfig()
        {
            TrackedCreatures.Clear();

            _isModEnabled = Config.Bind("General", "IsModEnabled", true, "Enable or disable the mod.");
            _playTrackedCreatureSound = Config.Bind("General", "PlayTrackedCreatureSound", true, "Enable or disable the tracked creature spawn sound.");

            AddTrackedCreature("Serpent", "Track Sea Serpents", "Sea Serpent", "sfx_serpent_taunt");
            AddTrackedCreature("BonemawSerpent", "Track Bonemaw Serpents", "Bonemaw Serpent", "sfx_bonemaw_serpent_alert");
            AddTrackedCreature("Troll", "Track Trolls", "Troll", "sfx_troll_idle");
            AddTrackedCreature("Bjorn", "Track Black Forest Bears", "Black Forest Bear", "sfx_bear_bite_attack");
            AddTrackedCreature("Unbjorn", "Track Vile Bears", "Vile Bear", "sfx_bear_bite_attack");
            AddTrackedCreature("Abomination", "Track Abominations", "Abomination", "sfx_abomination_arise_end");
            AddTrackedCreature("StoneGolem", "Track Stone Golems", "Stone Golem", "sfx_stonegolem_idle");
            AddTrackedCreature("Morgen", "Track Morgens", "Morgen", "sfx_morgen_idle");
            AddTrackedCreature("Wolf", "Track 2-star Wolves", "Wolf", "sfx_wolf_idle");
            AddTrackedCreature("Boars", "Track 2-star Boars", "Boar", "sfx_boar_idle");
        }

        private void AddTrackedCreature(string prefabName, string configName, string displayName, string soundEffectName)
        {
            TrackedCreatures[prefabName] = new TrackedCreatureSetting(
                Config.Bind("Tracking", configName, true, $"Enable or disable tracking of {displayName}."),
                displayName,
                soundEffectName);
        }

        internal static bool IsTrackedPrefab(string prefabName)
        {
            if (!IsModEnabled)
            {
                return false;
            }

            return TryGetTrackedCreature(prefabName, out TrackedCreatureSetting trackedCreature)
                && trackedCreature.Enabled != null
                && trackedCreature.Enabled.Value;
        }

        internal static string GetDisplayName(string prefabName)
        {
            return TryGetTrackedCreature(prefabName, out TrackedCreatureSetting trackedCreature)
                ? trackedCreature.DisplayName
                : prefabName;
        }

        internal static string GetSoundEffectName(string prefabName)
        {
            return TryGetTrackedCreature(prefabName, out TrackedCreatureSetting trackedCreature)
                ? trackedCreature.SoundEffectName
                : null;
        }

        private static bool TryGetTrackedCreature(string prefabName, out TrackedCreatureSetting trackedCreature)
        {
            trackedCreature = null;

            if (string.IsNullOrEmpty(prefabName))
            {
                return false;
            }

            return TrackedCreatures.TryGetValue(prefabName, out trackedCreature) && trackedCreature != null;
        }

        private sealed class TrackedCreatureSetting
        {
            internal TrackedCreatureSetting(ConfigEntry<bool> enabled, string displayName, string soundEffectName)
            {
                Enabled = enabled;
                DisplayName = displayName;
                SoundEffectName = soundEffectName;
            }

            internal ConfigEntry<bool> Enabled { get; }
            internal string DisplayName { get; }
            internal string SoundEffectName { get; }
        }
    }
}
