using HarmonyLib;

namespace HexRareScanner.Patches
{
    [HarmonyPatch(typeof(Minimap), "UpdatePins")]
    internal static class MinimapUpdatePinsPatch
    {
        [HarmonyPostfix]
        private static void Postfix()
        {
            PinManager.UpdateCreaturePinVisuals();
        }
    }
}