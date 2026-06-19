using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace HexRareScanner.Patches
{
    [HarmonyPatch(typeof(SpawnSystem), nameof(SpawnSystem.Spawn))]
    internal static class PatchSpawnSystemSpawn
    {
        private static void Prefix(SpawnSystem.SpawnData critter, out HashSet<ZDOID> __state)
        {
            if (!Plugin.IsModEnabled)
            {
                __state = null;
                return;
            }

            __state = new HashSet<ZDOID>();

            if (critter?.m_prefab == null)
            {
                return;
            }

            string prefabName = critter.m_prefab.name;

            if (!Plugin.IsTrackedPrefab(prefabName))
            {
                return;
            }

            foreach (Character character in Character.GetAllCharacters())
            {
                ZDOID zdoid = PinManager.GetZdoId(character);

                if (zdoid != ZDOID.None && IsCharacterMatchingPrefab(character, prefabName))
                {
                    __state.Add(zdoid);
                }
            }
        }

        private static void Postfix(SpawnSystem.SpawnData critter, Vector3 spawnPoint, HashSet<ZDOID> __state)
        {
            if (!Plugin.IsModEnabled || critter?.m_prefab == null)
            {
                return;
            }

            string prefabName = critter.m_prefab.name;

            if (!Plugin.IsTrackedPrefab(prefabName))
            {
                return;
            }

            string displayName = Plugin.GetDisplayName(prefabName);
            Character spawnedCharacter = null;

            foreach (Character character in Character.GetAllCharacters())
            {
                ZDOID zdoid = PinManager.GetZdoId(character);

                if (zdoid == ZDOID.None)
                {
                    continue;
                }

                if (!IsCharacterMatchingPrefab(character, prefabName))
                {
                    continue;
                }

                if (!__state.Contains(zdoid))
                {
                    spawnedCharacter = character;
                    break;
                }
            }

            if (spawnedCharacter == null)
            {
                Plugin.Log.LogWarning($"{displayName} spawned, but spawned Character was not found.");
                return;
            }

            Plugin.Log.LogInfo($"A {displayName} spawned at {spawnPoint}");

            PlayTrackedCreatureSound(prefabName, spawnPoint);

            Player.m_localPlayer?.Message(MessageHud.MessageType.Center, $"A {displayName} is spawning!");

            PinManager.AddCreaturePin(spawnedCharacter, spawnPoint, displayName);
        }

        private static void PlayTrackedCreatureSound(string prefabName, Vector3 position)
        {
            if (prefabName != "Serpent")
            {
                return;
            }

            if (ZNetScene.instance == null)
            {
                Plugin.Log.LogWarning("ZNetScene.instance is null. Could not play serpent sound.");
                return;
            }

            GameObject sfxPrefab = ZNetScene.instance.GetPrefab("sfx_serpent_taunt");

            if (sfxPrefab == null)
            {
                Plugin.Log.LogWarning("Could not find prefab: sfx_serpent_taunt");
                return;
            }

            Object.Instantiate(sfxPrefab, position, Quaternion.identity);
        }

        private static bool IsCharacterMatchingPrefab(Character character, string prefabName)
        {
            if (character == null || string.IsNullOrEmpty(prefabName))
            {
                return false;
            }

            return character.gameObject.name == $"{prefabName}(Clone)";
        }
    }
}