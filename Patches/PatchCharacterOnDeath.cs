using HarmonyLib;
using HexRareScanner;

[HarmonyPatch(typeof(Character), nameof(Character.OnDeath))]
internal static class PatchCharacterOnDeath
{
    internal static void Prefix(Character __instance)
    {
        if (!Plugin.IsModEnabled || __instance == null)
        {
            return;
        }

        string prefabName = PrefabNameHelper.GetPrefabNameFromClone(__instance.gameObject.name);
        int creatureLevel = Plugin.GetCreatureLevel(__instance);

        if (!Plugin.IsTrackedPrefab(prefabName, creatureLevel))
        {
            return;
        }

        PinManager.RemoveCreaturePin(__instance);
    }
}