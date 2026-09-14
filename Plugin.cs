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
        private const string PluginVersion = "1.3.0";

        private Harmony _harmonyInstance;

        private static ConfigEntry<bool> _isModEnabled;
        private static ConfigEntry<bool> _playTrackedCreatureSound;
        private static ConfigEntry<bool> _isManualPinRemovalEnabled;

        private static readonly Dictionary<string, TrackedCreatureSetting> TrackedCreatures = new Dictionary<string, TrackedCreatureSetting>();

        internal static readonly FieldInfo CharacterMLevelField = AccessTools.Field(typeof(Character), "m_level");

        internal static bool IsModEnabled => _isModEnabled?.Value ?? false;
        internal static bool PlayTrackedCreatureSound => _playTrackedCreatureSound?.Value ?? false;
        internal static bool IsManualPinRemovalEnabled => _isManualPinRemovalEnabled?.Value ?? true;

        internal static Plugin Instance;
        internal static ManualLogSource Log;

        private void Awake()
        {
            Instance = this;
            Log = Logger;

            InitializeConfig();

            Assembly assembly = Assembly.GetExecutingAssembly();
            _harmonyInstance = new Harmony(PluginGuid);
            _harmonyInstance.PatchAll(assembly);

            Log.LogInfo($"{PluginName} v{PluginVersion} loaded.");
        }

        private void OnDestroy()
        {
            Log?.LogInfo($"{PluginName} v{PluginVersion} unloaded.");

            _harmonyInstance?.UnpatchSelf();
            _harmonyInstance = null;
            Instance = null;
            Log = null;
        }

        private void InitializeConfig()
        {
            TrackedCreatures.Clear();

            _isModEnabled = Config.Bind("General", "IsModEnabled", true, "Enable or disable the mod.");
            _playTrackedCreatureSound = Config.Bind("Sounds", "PlayTrackedCreatureSound", true, "Enable or disable the tracked creature spawn sound.");
            _isManualPinRemovalEnabled = Config.Bind("Map Pins", "Enable Manual Pin Removal", true, "Allow tracked creature pins to be removed by right-clicking them on the map.");

            foreach (var creature in TrackedCreatureDefinition.Creatures)
            {
                AddTrackedCreature(creature.ConfigSection, creature.PrefabName, creature.ConfigName, creature.DisplayName, creature.SoundEffectName, creature.MinimumLevel);
            }
        }

        private void AddTrackedCreature(string configSection, string prefabName, string configName, string displayName, string soundEffectName, int rarityLevel = 1)
        {
            TrackedCreatures[prefabName] = new TrackedCreatureSetting(
                Config.Bind(configSection, configName, true, $"Enable or disable tracking of {displayName}."),
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