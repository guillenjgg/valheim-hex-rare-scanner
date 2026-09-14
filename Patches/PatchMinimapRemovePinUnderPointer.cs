using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace HexRareScanner.Patches
{
    [HarmonyPatch(typeof(Minimap), nameof(Minimap.RemovePinUnderPointer))]
    internal static class PatchMinimapRemovePinUnderPointer
    {
        private static readonly MethodInfo ScreenToWorldPointMethod = AccessTools.Method(typeof(Minimap), "ScreenToWorldPoint", new[] { typeof(Vector3) });
        private static readonly MethodInfo GetClosestPinMethod = AccessTools.Method(typeof(Minimap), "GetClosestPin", new[] { typeof(Vector3), typeof(float), typeof(bool) });
        private static readonly MethodInfo HidePinTextInputMethod = AccessTools.Method(typeof(Minimap), "HidePinTextInput", new[] { typeof(bool) });
        private static readonly PropertyInfo PinInteractRadiusProperty = AccessTools.Property(typeof(Minimap), "PinInteractRadius");
        private static readonly FieldInfo NamePinField = AccessTools.Field(typeof(Minimap), "m_namePin");

        private static bool Prefix(Minimap __instance)
        {
            if (!Plugin.IsModEnabled || !Plugin.IsManualPinRemovalEnabled || __instance == null)
            {
                return true;
            }

            if (ScreenToWorldPointMethod == null || GetClosestPinMethod == null || PinInteractRadiusProperty == null)
            {
                return true;
            }

            Vector3 position = (Vector3)ScreenToWorldPointMethod.Invoke(__instance, new object[] { ZInput.pointerPosition });
            float pinInteractRadius = (float)PinInteractRadiusProperty.GetValue(__instance);

            Minimap.PinData creaturePin = PinManager.GetClosestCreaturePin(position, pinInteractRadius);

            if (creaturePin == null)
            {
                return true;
            }

            Minimap.PinData vanillaPin = GetClosestPinMethod.Invoke(__instance, new object[] { position, pinInteractRadius, true }) as Minimap.PinData;

            if (vanillaPin != null)
            {
                float creaturePinDistance = Utils.DistanceXZ(position, creaturePin.m_pos);
                float vanillaPinDistance = Utils.DistanceXZ(position, vanillaPin.m_pos);

                if (vanillaPinDistance < creaturePinDistance)
                {
                    return true;
                }
            }

            HidePinTextInputMethod?.Invoke(__instance, new object[] { false });

            PinManager.RemoveCreaturePin(creaturePin);
            NamePinField?.SetValue(__instance, null);

            return false;
        }
    }
}