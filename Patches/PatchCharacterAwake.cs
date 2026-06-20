using HarmonyLib;
using UnityEngine;

namespace HexRareScanner.Patches
{
    [HarmonyPatch(typeof(Character), nameof(Character.Awake))]
    internal static class PatchCharacterAwake
    {
        private static void Postfix(Character __instance)
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

            if (PinManager.HasCreaturePin(__instance))
            {
                return;
            }

            string displayName = Plugin.GetDisplayName(prefabName);
            Vector3 spawnPoint = __instance.transform.position;

            Plugin.Log.LogInfo($"A {displayName} spawned at {spawnPoint}");

            if (Plugin.PlayTrackedCreatureSound)
            {
                string soundEffectName = Plugin.GetSoundEffectName(prefabName);
                PlayTrackedCreatureSound(soundEffectName, spawnPoint);
            }

            Player.m_localPlayer?.Message(MessageHud.MessageType.Center, $"A {displayName} is spawning!");

            PinManager.AddCreaturePin(__instance, spawnPoint, displayName);
        }

        private static void PlayTrackedCreatureSound(string soundEffectName, Vector3 position)
        {
            if (string.IsNullOrEmpty(soundEffectName))
            {
                return;
            }

            if (ZNetScene.instance == null)
            {
                Plugin.Log.LogWarning("ZNetScene.instance is null. Could not play tracked creature sound.");
                return;
            }

            GameObject sfxPrefab = ZNetScene.instance.GetPrefab(soundEffectName);

            if (sfxPrefab == null)
            {
                Plugin.Log.LogWarning($"Could not find sound effect prefab: {soundEffectName}");
                return;
            }

            Object.Instantiate(sfxPrefab, position, Quaternion.identity);
        }
    }
}