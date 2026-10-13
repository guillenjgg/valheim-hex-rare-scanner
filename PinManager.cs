using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace HexRareScanner
{
    internal static class PinManager
    {
        private static readonly Dictionary<ZDOID, CreaturePin> PinsByZdoid = new Dictionary<ZDOID, CreaturePin>();
        private static readonly FieldInfo CharacterNViewField = AccessTools.Field(typeof(Character), "m_nview");
        private static readonly MethodInfo RemovePinMethod = AccessTools.Method(typeof(Minimap), "RemovePin", new[] { typeof(Minimap.PinData) });
        private static readonly FieldInfo MPinUpdateRequired = AccessTools.Field(typeof(Minimap), "m_pinUpdateRequired");
        private static readonly Color UIPinBorderColor = new Color(1f, 0.75f, 0.15f, 1f);

        private const string WraithUIBorder = "hex_rare_scanner_wrait_border";
        private const float BorderPadding = -16f;

        internal static void AddCreaturePin(Character character, Vector3 position, string pinName)
        {
            if (Minimap.instance == null || character == null)
            {
                return;
            }

            ZDOID zdoid = GetZdoId(character);

            if (zdoid == ZDOID.None)
            {
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
                if (Plugin.IsCreatureIconsEnabled)
                {
                    Sprite creatureIcon = GetCreatureIcon(definition.TrophyPrefabName);

                    if (creatureIcon != null)
                    {
                        SetPinIcon(pin, creatureIcon);
                    }
                }
            }

            CreaturePin creaturePin = new CreaturePin(pin, pinName, prefabName);
            PinsByZdoid[zdoid] = creaturePin;

            UpdateLabel(creaturePin, Plugin.IsLabelsEnabled);
        }

        internal static void RemoveCreaturePin(Character character)
        {
            if (Minimap.instance == null)
            {
                return;
            }

            ZDOID zdoid = GetZdoId(character);

            if (zdoid == ZDOID.None)
            {
                return;
            }

            if (!PinsByZdoid.TryGetValue(zdoid, out CreaturePin creaturePin))
            {
                return;
            }

            RemovePinMethod?.Invoke(Minimap.instance, new object[] { creaturePin.Pin });
            PinsByZdoid.Remove(zdoid);
        }

        internal static Minimap.PinData GetClosestCreaturePin(Vector3 position, float radius)
        {
            Minimap.PinData closestPin = null;
            float closestDistance = float.MaxValue;

            foreach (CreaturePin creaturePin in PinsByZdoid.Values)
            {
                Minimap.PinData pin = creaturePin.Pin;

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

            foreach (KeyValuePair<ZDOID, CreaturePin> entry in PinsByZdoid)
            {
                if (entry.Value.Pin == pin)
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

        internal static void UpdateLabels()
        {
            foreach (CreaturePin creaturePin in PinsByZdoid.Values)
            {
                UpdateLabel(creaturePin, Plugin.IsLabelsEnabled);
            }

            SetPinUpdateRequired();
        }

        internal static void UpdateCreaturePinVisuals()
        {
            foreach (var creaturePin in PinsByZdoid.Values)
            {
                var pin = creaturePin.Pin;

                if (pin == null || pin.m_uiElement == null)
                {
                    continue;
                }

                if (creaturePin.PrefabName != "Wraith" || creaturePin.PrefabName != "Abomination")
                {
                    continue;
                }

                if (pin.m_uiElement.Find(WraithUIBorder) != null)
                {
                    continue;
                }

                var iconSize = pin.m_uiElement.rect.width;

                AddIconBorder(pin.m_uiElement, iconSize);
            }
        }

        private static void UpdateLabel(CreaturePin creaturePin, bool labelsEnabled)
        {
            if (creaturePin?.Pin == null)
            {
                return;
            }

            Minimap.PinData pin = creaturePin.Pin;
            pin.m_name = labelsEnabled ? creaturePin.Label : string.Empty;

            if (pin.m_NamePinData?.PinNameText != null)
            {
                pin.m_NamePinData.PinNameText.text = Localization.instance.Localize(pin.m_name);
            }
        }

        private static void SetPinUpdateRequired()
        {
            if (Minimap.instance == null)
            {
                return;
            }

            MPinUpdateRequired.SetValue(Minimap.instance, true);
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
                return null;
            }

            ItemDrop itemDrop = trophyPrefab.GetComponent<ItemDrop>();

            if (itemDrop == null || itemDrop.m_itemData == null)
            {
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

        private sealed class CreaturePin
        {
            internal CreaturePin(Minimap.PinData pin, string label, string prefabName = null)
            {
                Pin = pin;
                Label = label;
                PrefabName = prefabName;
            }

            internal Minimap.PinData Pin { get; }
            internal string Label { get; }
            internal string PrefabName { get; }
        }

        private static void AddIconBorder(RectTransform pinUiElement, float iconSize)
        {
            Transform existingBorder = pinUiElement.Find(WraithUIBorder);

            if (existingBorder != null)
            {
                return;
            }

            var borderObject = new GameObject(WraithUIBorder);
            borderObject.transform.SetParent(pinUiElement, false);
            borderObject.transform.SetAsFirstSibling();

            var borderRect = borderObject.AddComponent<RectTransform>();
            borderRect.anchorMin = new Vector2(0.5f, 0.5f);
            borderRect.anchorMax = new Vector2(0.5f, 0.5f);
            borderRect.pivot = new Vector2(0.5f, 0.5f);
            borderRect.anchoredPosition = Vector2.zero;

            float borderSize = iconSize + BorderPadding;
            borderRect.sizeDelta = new Vector2(borderSize, borderSize);

            AddBorderEdge(borderRect, "Top", new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -2f), new Vector2(0f, 2f));
            AddBorderEdge(borderRect, "Bottom", new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 2f));
            AddBorderEdge(borderRect, "Left", new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(2f, 0f));
            AddBorderEdge(borderRect, "Right", new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(-2f, 0f), new Vector2(2f, 0f));
        }

        private static void AddBorderEdge(RectTransform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            var edgeObject = new GameObject(name);
            edgeObject.transform.SetParent(parent, false);

            var edgeRect = edgeObject.AddComponent<RectTransform>();
            edgeRect.anchorMin = anchorMin;
            edgeRect.anchorMax = anchorMax;
            edgeRect.pivot = new Vector2(0.5f, 0.5f);
            edgeRect.anchoredPosition = anchoredPosition;
            edgeRect.sizeDelta = sizeDelta;

            var edgeImage = edgeObject.AddComponent<Image>();
            edgeImage.color = UIPinBorderColor;
            edgeImage.raycastTarget = false;
        }
    }
}