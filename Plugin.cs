using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;

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

        internal static readonly FieldInfo CharacterMLevelField =
        typeof(Character).GetField(
            "m_level",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        internal static bool IsModEnabled => _isModEnabled?.Value ?? false;
        internal static bool PlayTrackedCreatureSound => _playTrackedCreatureSound?.Value ?? false;

        internal static Plugin Instance;
        internal static ManualLogSource Log;

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
            AddTrackedCreature("Wolf", "Track 2-star Wolves", "Wolf", "sfx_wolf_alerted", 3);
            AddTrackedCreature("Boar", "Track 2-star Boars", "Boar", "sfx_boar_idle", 3);
            AddTrackedCreature("Deer", "Track 2-star Deer", "Deer", "sfx_deer_idle", 3);
            AddTrackedCreature("Asksvin", "Track 2-star Asksvin", "Asksvin", "sfx_asksvin_idle", 3);
            AddTrackedCreature("FallenValkyrie", "Track Fallen Valkyrie", "FallenValkyrie", "sfx_valkyrie_flapwing");
        }

        private void AddTrackedCreature(string prefabName, string configName, string displayName, string soundEffectName, int rarityLevel = 1)
        {
            TrackedCreatures[prefabName] = new TrackedCreatureSetting(
                Config.Bind("Tracking", configName, true, $"Enable or disable tracking of {displayName}."),
                displayName,
                soundEffectName,
                rarityLevel);
        }

        internal static bool IsTrackedPrefab(string prefabName, int creatureLevel)
        {
            if (!IsModEnabled)
            {
                return false;
            }

            return TryGetTrackedCreature(prefabName, out TrackedCreatureSetting trackedCreature)
                && trackedCreature.Enabled != null
                && trackedCreature.Enabled.Value
                && creatureLevel >= trackedCreature.RarityLevel;
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

        internal static int GetCreatureLevel(Character character)
        {
            if (character == null || CharacterMLevelField == null)
            {
                return 1;
            }

            object value = CharacterMLevelField.GetValue(character);

            if (value is int level)
            {
                return level;
            }

            return 1;
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
            internal TrackedCreatureSetting(ConfigEntry<bool> enabled, string displayName, string soundEffectName, int rarityLevel)
            {
                Enabled = enabled;
                DisplayName = displayName;
                SoundEffectName = soundEffectName;
                RarityLevel = rarityLevel;
            }

            internal ConfigEntry<bool> Enabled { get; }
            internal string DisplayName { get; }
            internal string SoundEffectName { get; }
            internal int RarityLevel { get; }
        }
    }
}
