using HarmonyLib;
using HexRareScanner;

[HarmonyPatch(typeof(Character), nameof(Character.OnDeath))]
internal static class PatchCharacterOnDeath
{
    internal static void Prefix(Character __instance)
    {
        if(!Plugin.IsModEnabled || __instance == null || !__instance.gameObject.name.Contains("Serpent"))
        {
            return;
        }

        Plugin.Log.LogInfo($"Removing pin for {__instance.gameObject.name} at position {__instance.transform.position}");

        PinManager.RemoveCreaturePin(__instance);
    }
}