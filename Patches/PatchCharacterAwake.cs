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
            if (!Plugin.IsModEnabled || __instance == null)
            {
                return;
            }

            string prefabName = PrefabNameHelper.GetPrefabNameFromClone(__instance.gameObject.name);

            if(string.IsNullOrWhiteSpace(prefabName))
            {
                return;
            }

            Plugin.TrackedCreatureSetting trackedCreature = Plugin.GetTrackedCreature(prefabName);

            if (trackedCreature == null)
            {
                return;
            }

            Plugin.Instance.StartCoroutine(DelayedScan(__instance, prefabName, trackedCreature));
        }

        // Delay two frames so m_level is initialized.
        private static IEnumerator DelayedScan(Character character, string prefabName, Plugin.TrackedCreatureSetting trackedCreature)
        {
            yield return null;
            yield return null;

            if (!Plugin.IsModEnabled || character == null)
            {
                yield break;
            }

            int creatureLevel = (int)Plugin.CharacterMLevelField.GetValue(character);

            if (creatureLevel < trackedCreature.RarityLevel || PinManager.HasCreaturePin(character))
            {
                yield break;
            }

            string displayName = trackedCreature.DisplayName;

            if (creatureLevel > 1)
            {
                displayName = $"{creatureLevel - 1}-star {displayName}";
            }

            Vector3 spawnPoint = character.transform.position;

            if (Plugin.PlayTrackedCreatureSound)
            {
#if DEBUG
                if (string.IsNullOrEmpty(trackedCreature.SoundEffectName))
                {
                    Plugin.Log.LogDebug($"No sound effect configured for tracked creature '{displayName}' (prefab: '{prefabName}').");
                }
#endif

                PlayTrackedCreatureSound(trackedCreature.SoundEffectName, spawnPoint);
            }

            Player.m_localPlayer?.Message(MessageHud.MessageType.Center, $"A {displayName} spawned!");
            PinManager.AddCreaturePin(character, spawnPoint, displayName);
        }

        private static void PlayTrackedCreatureSound(string soundEffectName, Vector3 position)
        {
            if (string.IsNullOrEmpty(soundEffectName))
            {
                return;
            }

            if (ZNetScene.instance == null)
            {
                Plugin.Log.LogDebug("ZNetScene.instance is null. Could not play tracked creature sound.");
                return;
            }

            GameObject sfxPrefab = ZNetScene.instance.GetPrefab(soundEffectName);

            if (sfxPrefab == null)
            {
                Plugin.Log.LogDebug($"Could not find sound effect prefab: {soundEffectName}");
                return;
            }

            Object.Instantiate(sfxPrefab, position, Quaternion.identity);
        }
    }
}