using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace HexRareScanner
{
    internal static class PinManager
    {
        private static readonly Dictionary<ZDOID, Minimap.PinData> PinsByZdoid = new Dictionary<ZDOID, Minimap.PinData>();

        private static readonly FieldInfo CharacterNViewField = AccessTools.Field(typeof(Character), "m_nview");
        private static readonly MethodInfo RemovePinMethod = AccessTools.Method(typeof(Minimap), "RemovePin", new[] { typeof(Minimap.PinData) });

        internal static void AddCreaturePin(Character character, Vector3 position, string pinName)
        {
            if (Minimap.instance == null || character == null)
            {
                return;
            }

            ZDOID zdoid = GetZdoId(character);
            string characterName = character.gameObject.name;

            if (zdoid == ZDOID.None)
            {
                Plugin.Log.LogDebug($"Could not get ZDOID for character {characterName}. Pin will not be added.");
                return;
            }

            if (PinsByZdoid.ContainsKey(zdoid))
            {
                return;
            }

            Minimap.PinData pin = Minimap.instance.AddPin(position, Minimap.PinType.Ping, pinName, false, false);

            if (pin == null)
            {
                return;
            }

            string prefabName = PrefabNameHelper.GetPrefabNameFromClone(character.gameObject.name);
            TrackedCreatureDefinition definition = GetCreatureDefinition(prefabName);

            if (definition != null)
            {
                Sprite creatureIcon = GetCreatureIcon(definition.TrophyPrefabName);

                if (creatureIcon != null)
                {
                    SetPinIcon(pin, creatureIcon);
                }
            }

            PinsByZdoid[zdoid] = pin;
        }

        internal static void RemoveCreaturePin(Character character)
        {
            if (Minimap.instance == null)
            {
                return;
            }

            ZDOID zdoid = GetZdoId(character);
            string characterName = character != null ? character.gameObject.name : "null";

            if (zdoid == ZDOID.None)
            {
                Plugin.Log.LogDebug($"Could not get ZDOID for character {characterName}. Pin will not be removed.");
                return;
            }

            if (PinsByZdoid.TryGetValue(zdoid, out Minimap.PinData pin))
            {
                RemovePinMethod?.Invoke(Minimap.instance, new object[] { pin });
                PinsByZdoid.Remove(zdoid);

                return;
            }

            Plugin.Log.LogDebug($"No pin found for {characterName}. ZDOID: {zdoid}");
        }

        internal static Minimap.PinData GetClosestCreaturePin(Vector3 position, float radius)
        {
            Minimap.PinData closestPin = null;
            float closestDistance = float.MaxValue;

            foreach (Minimap.PinData pin in PinsByZdoid.Values)
            {
                if (pin == null)
                {
                    continue;
                }

                if (pin.m_uiElement == null || !pin.m_uiElement.gameObject.activeInHierarchy)
                {
                    continue;
                }

                float distance = Utils.DistanceXZ(position, pin.m_pos);

                if (distance >= radius || distance >= closestDistance)
                {
                    continue;
                }

                closestPin = pin;
                closestDistance = distance;
            }

            return closestPin;
        }

        internal static bool RemoveCreaturePin(Minimap.PinData pin)
        {
            if (pin == null || Minimap.instance == null)
            {
                return false;
            }

            ZDOID zdoidToRemove = ZDOID.None;

            foreach (KeyValuePair<ZDOID, Minimap.PinData> entry in PinsByZdoid)
            {
                if (entry.Value == pin)
                {
                    zdoidToRemove = entry.Key;
                    break;
                }
            }

            if (zdoidToRemove == ZDOID.None)
            {
                return false;
            }

            RemovePinMethod?.Invoke(Minimap.instance, new object[] { pin });
            PinsByZdoid.Remove(zdoidToRemove);

            return true;
        }

        internal static ZDOID GetZdoId(Character character)
        {
            if (character == null || CharacterNViewField == null)
            {
                return ZDOID.None;
            }

            ZNetView nview = CharacterNViewField.GetValue(character) as ZNetView;

            if (nview == null || nview.GetZDO() == null)
            {
                return ZDOID.None;
            }

            return nview.GetZDO().m_uid;
        }

        internal static bool HasCreaturePin(Character character)
        {
            ZDOID zdoid = GetZdoId(character);

            if (zdoid == ZDOID.None)
            {
                return false;
            }

            return PinsByZdoid.ContainsKey(zdoid);
        }

        internal static void Clear()
        {
            PinsByZdoid.Clear();
        }

        private static TrackedCreatureDefinition GetCreatureDefinition(string prefabName)
        {
            foreach (TrackedCreatureDefinition creature in TrackedCreatureDefinition.Creatures)
            {
                if (creature.PrefabName == prefabName)
                {
                    return creature;
                }
            }

            return null;
        }

        private static Sprite GetCreatureIcon(string trophyPrefabName)
        {
            if (string.IsNullOrEmpty(trophyPrefabName) || ObjectDB.instance == null)
            {
                return null;
            }

            GameObject trophyPrefab = ObjectDB.instance.GetItemPrefab(trophyPrefabName);

            if (trophyPrefab == null)
            {
                Plugin.Log.LogDebug($"Could not find trophy prefab {trophyPrefabName}.");
                return null;
            }

            ItemDrop itemDrop = trophyPrefab.GetComponent<ItemDrop>();

            if (itemDrop == null)
            {
                Plugin.Log.LogDebug($"Trophy prefab {trophyPrefabName} does not contain an ItemDrop component.");
                return null;
            }

            return itemDrop.m_itemData.GetIcon();
        }

        private static void SetPinIcon(Minimap.PinData pin, Sprite icon)
        {
            if (pin == null || icon == null)
            {
                return;
            }

            pin.m_icon = icon;

            if (pin.m_iconElement != null)
            {
                pin.m_iconElement.sprite = icon;
            }
        }
    }
}