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

        if (!Plugin.IsTrackedPrefab(prefabName))
        {
            return;
        }

        Plugin.Log.LogInfo($"Removing pin for {__instance.gameObject.name} at position {__instance.transform.position}");

        PinManager.RemoveCreaturePin(__instance);
    }
}