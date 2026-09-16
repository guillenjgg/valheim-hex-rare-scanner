using HarmonyLib;

namespace HexRareScanner.Patches
{
    [HarmonyPatch(typeof(Character), nameof(Character.OnDeath))]
    internal static class PatchCharacterOnDeath
    {
        private static void Prefix(Character __instance)
        {
            if (!Plugin.IsModEnabled || __instance == null)
            {
                return;
            }

            string prefabName = PrefabNameHelper.GetPrefabNameFromClone(__instance.gameObject.name);

            if (string.IsNullOrWhiteSpace(prefabName))
            {
                return;
            }

            Plugin.TrackedCreatureSetting trackedCreature = Plugin.GetTrackedCreature(prefabName);

            if (trackedCreature == null)
            {
                return;
            }

            int creatureLevel = (int)Plugin.CharacterMLevelField.GetValue(__instance);

            if (creatureLevel < trackedCreature.RarityLevel)
            {
                return;
            }

            PinManager.RemoveCreaturePin(__instance);
        }
    }
}