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

            if (creatureLevel < trackedCreature.RarityLevel || PinManager.HasCreaturePin(__instance))
            {
                return;
            }

            string displayName = trackedCreature.DisplayName;

            if (creatureLevel > 1)
            {
                displayName = $"{creatureLevel - 1}-star {displayName}";
            }

            Vector3 spawnPoint = __instance.transform.position;

            if (Plugin.PlayTrackedCreatureSound)
            {
                Plugin.Instance.StartCoroutine(PlayTrackedCreatureSound(trackedCreature.SoundEffectName, spawnPoint));
            }

            Player.m_localPlayer?.Message(MessageHud.MessageType.Center, $"A {displayName} spawned!");

            PinManager.AddCreaturePin(__instance, spawnPoint, displayName);
        }

        private static IEnumerator PlayTrackedCreatureSound(string soundEffectName, Vector3 position)
        {
            yield return null;

            if (string.IsNullOrEmpty(soundEffectName))
            {
                yield break;
            }

            if (ZNetScene.instance == null)
            {
                yield break;
            }

            GameObject sfxPrefab = ZNetScene.instance.GetPrefab(soundEffectName);

            if (sfxPrefab == null)
            {
                yield break;
            }

            Object.Instantiate(sfxPrefab, position, Quaternion.identity);
        }
    }
}