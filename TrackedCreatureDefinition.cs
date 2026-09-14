using System.Collections.Generic;

namespace HexRareScanner
{
    internal sealed class TrackedCreatureDefinition
    {
        internal static class Biomes
        {
            internal const string Ocean = "Tracked Creatures - Ocean";
            internal const string Meadows = "Tracked Creatures - Meadows";
            internal const string BlackForest = "Tracked Creatures - Black Forest";
            internal const string Swamp = "Tracked Creatures - Swamp";
            internal const string Mountains = "Tracked Creatures - Mountains";
            internal const string Plains = "Tracked Creatures - Plains";
            internal const string Mistlands = "Tracked Creatures - Mistlands";
            internal const string Ashlands = "Tracked Creatures - Ashlands";
            internal const string DeepNorth = "Tracked Creatures - Deep North";
        }

        internal static readonly List<TrackedCreatureDefinition> Creatures = new List<TrackedCreatureDefinition>
        {
            new TrackedCreatureDefinition(Biomes.Ocean, "Serpent", "Track Sea Serpents", "Sea Serpent", "sfx_serpent_taunt"),
            new TrackedCreatureDefinition(Biomes.Ocean, "BonemawSerpent", "Track Bonemaw Serpents", "Bonemaw Serpent", "sfx_bonemaw_serpent_alert"),

            new TrackedCreatureDefinition(Biomes.Meadows, "Deer", "Track 2-star Deer", "Deer", "sfx_deer_idle", null, 3),
            new TrackedCreatureDefinition(Biomes.Meadows, "Boar", "Track 2-star Boars", "Boar", "sfx_boar_idle", null, 3),

            new TrackedCreatureDefinition(Biomes.BlackForest, "Troll", "Track Trolls", "Troll", "sfx_troll_idle", "TrophyForestTroll"),
            new TrackedCreatureDefinition(Biomes.BlackForest, "Bjorn", "Track Black Forest Bears", "Black Forest Bear", "sfx_bear_bite_attack", "TrophyBjorn"),

            new TrackedCreatureDefinition(Biomes.Swamp, "Abomination", "Track Abominations", "Abomination", "sfx_abomination_arise_end"),

            new TrackedCreatureDefinition(Biomes.Mountains, "StoneGolem", "Track Stone Golems", "Stone Golem", "sfx_stonegolem_idle"),
            new TrackedCreatureDefinition(Biomes.Mountains, "Wolf", "Track 2-star Wolves", "Wolf", "sfx_wolf_alerted", null, 3),

            new TrackedCreatureDefinition(Biomes.Plains, "Unbjorn", "Track Vile Bears", "Vile Bear", "sfx_bear_bite_attack"),

            new TrackedCreatureDefinition(Biomes.Ashlands, "Asksvin", "Track 2-star Asksvin", "Asksvin", "sfx_asksvin_idle", null, 3),
            new TrackedCreatureDefinition(Biomes.Ashlands, "FallenValkyrie", "Track Fallen Valkyrie", "FallenValkyrie", "sfx_fallenvalkyrie_alert"),
            new TrackedCreatureDefinition(Biomes.Ashlands, "Morgen", "Track Morgens", "Morgen", "sfx_morgen_idle"),

            new TrackedCreatureDefinition(Biomes.DeepNorth, "Writhan", "Track Writhan", "Writhan", "sfx_writhan_verse_attack"),
            new TrackedCreatureDefinition(Biomes.DeepNorth, "Barka", "Track Barkas", "Barka", "Enemy_Barka_Footstep"),
            new TrackedCreatureDefinition(Biomes.DeepNorth, "ShadowPerson", "Track Shadow Persons", "Shadow Person", null),
            new TrackedCreatureDefinition(Biomes.DeepNorth, "Skeleton_DeepNorth", "Track Deep North Skeletons", "Deep North Skeleton", null),
            new TrackedCreatureDefinition(Biomes.DeepNorth, "Greydwarf_Frozen", "Track 2-star Frozen Greydwarfs", "Frozen Greydwarf", null),
            new TrackedCreatureDefinition(Biomes.DeepNorth, "TrollFrost", "Track Frost Trolls", "Frost Troll", "sfx_gameltroll_melee_attack"),
            new TrackedCreatureDefinition(Biomes.DeepNorth, "Moose", "Track Moose", "Moose", "sfx_moose_alert"),
            new TrackedCreatureDefinition(Biomes.DeepNorth, "FallenWarrior", "Track Fallen Warriors", "Fallen Warrior", "sfx_fallenwarrior_attack"),
            new TrackedCreatureDefinition(Biomes.DeepNorth, "ElakingMole", "Track Eyeless One", "Eyeless One", "ElakingMole_AttackClaw"),
            new TrackedCreatureDefinition(Biomes.DeepNorth, "Elaking", "Track Elaking", "Elaking", "ElakingMole_AttackClaw")
        };

        internal string ConfigSection { get; }
        internal string PrefabName { get; }
        internal string ConfigName { get; }
        internal string DisplayName { get; }
        internal string SoundEffectName { get; }
        internal string TrophyPrefabName { get; }
        internal int MinimumLevel { get; }

        internal TrackedCreatureDefinition(string configSection, string prefabName, string configName, string displayName, string soundEffectName, string trophyPrefabName = null, int minimumLevel = 1)
        {
            ConfigSection = configSection;
            PrefabName = prefabName;
            ConfigName = configName;
            DisplayName = displayName;
            SoundEffectName = soundEffectName;
            TrophyPrefabName = trophyPrefabName;
            MinimumLevel = minimumLevel;
        }
    }
}