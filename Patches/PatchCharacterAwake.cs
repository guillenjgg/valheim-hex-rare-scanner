using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace HexRareScanner.Patches
{
    [HarmonyPatch(typeof(Character), nameof(Character.Awake))]
    internal static class PatchCharacterAwake
    {
        private static void Postfix(Character __instance)
        {
            Plugin.Instance.StartCoroutine(DelayedScan(__instance));
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


        // We need to delay the scan, so m_level is initialized
        private static IEnumerator DelayedScan(Character character)
        {
            yield return null;
            yield return null;

            if (!Plugin.IsModEnabled || character == null)
            {
                yield break;
            }

            string prefabName = PrefabNameHelper.GetPrefabNameFromClone(character.gameObject.name);
            int creatureLevel = Plugin.GetCreatureLevel(character);

            if (!Plugin.IsTrackedPrefab(prefabName, creatureLevel))
            {
                yield break;
            }

            if (PinManager.HasCreaturePin(character))
            {
                yield break;
            }

            string displayName = Plugin.GetDisplayName(prefabName);

            if (creatureLevel > 1)
            {
                displayName = $"{creatureLevel - 1}-star {displayName}";
            }

            Vector3 spawnPoint = character.transform.position;

            if (Plugin.PlayTrackedCreatureSound)
            {
                string soundEffectName = Plugin.GetSoundEffectName(prefabName);
                PlayTrackedCreatureSound(soundEffectName, spawnPoint);
            }

            Player.m_localPlayer?.Message(MessageHud.MessageType.Center, $"A {displayName} spawned!");

            PinManager.AddCreaturePin(character, spawnPoint, displayName);
        }
    }
}