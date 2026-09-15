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
        private const string PluginVersion = "1.3.1";

        private Harmony _harmonyInstance;

        private static ConfigEntry<bool> _isModEnabled;
        private static ConfigEntry<bool> _playTrackedCreatureSound;
        private static ConfigEntry<bool> _isManualPinRemovalEnabled;
        private static ConfigEntry<bool> _isCreatureIconsEnabled;

        private static readonly Dictionary<string, TrackedCreatureSetting> TrackedCreatures = new Dictionary<string, TrackedCreatureSetting>();

        internal static bool IsModEnabled => _isModEnabled?.Value ?? false;
        internal static bool PlayTrackedCreatureSound => _playTrackedCreatureSound?.Value ?? false;
        internal static bool IsManualPinRemovalEnabled => _isManualPinRemovalEnabled?.Value ?? true;
        internal static bool IsCreatureIconsEnabled => _isCreatureIconsEnabled?.Value ?? true;

        internal static Plugin Instance;
        internal static ManualLogSource Log;
        internal static readonly FieldInfo CharacterMLevelField = AccessTools.Field(typeof(Character), "m_level");

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
            _isCreatureIconsEnabled = Config.Bind("Map Pins", "Enable Creature Icons", true, "Use creature icons for tracked creature map pins. When disabled, the default Valheim blue ping icon is used. Requires a game restart.");

            foreach (TrackedCreatureDefinition creature in TrackedCreatureDefinition.Creatures)
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

        internal static TrackedCreatureSetting GetTrackedCreature(string prefabName)
        {
            if (!TrackedCreatures.TryGetValue(prefabName, out TrackedCreatureSetting trackedCreature))
            {
                return null;
            }

            return trackedCreature.Enabled.Value ? trackedCreature : null;
        }

        internal sealed class TrackedCreatureSetting
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