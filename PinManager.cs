using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HexRareScanner
{
    internal static class PinManager
    {
        private static readonly Dictionary<ZDOID, Minimap.PinData> PinsByZdoid = new Dictionary<ZDOID, Minimap.PinData>();

        private static readonly FieldInfo CharacterNViewField =
        typeof(Character).GetField(
            "m_nview",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        private static readonly MethodInfo RemovePinMethod = typeof(Minimap).GetMethod(
            "RemovePin",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public,
            null,
            new[] { typeof(Minimap.PinData) },
            null
        );

        internal static void AddCreaturePin(Character character, Vector3 position, string pinName)
        {
            var zdoid = GetZdoId(character);

            if(zdoid == ZDOID.None)
            {
                Plugin.Log.LogWarning($"Could not get ZDOID for character {character.gameObject.name}. Pin will not be added.");
                return;
            }

            Minimap.PinData pin = Minimap.instance?.AddPin(
                position,
                Minimap.PinType.Ping,
                pinName,
                true,
                false);

            if(pin == null)
            {
                return;
            }

            PinsByZdoid[zdoid] = pin;
            Plugin.Log.LogInfo($"Added pin for {character.gameObject.name}. ZDOID: {zdoid}");
        }

        internal static void RemoveCreaturePin(Character character)
        {
            ZDOID zdoid = GetZdoId(character);
            string characterName = character != null ? character.gameObject.name : "null";

            if (zdoid == ZDOID.None)
            {
                Plugin.Log.LogWarning($"Could not get ZDOID for character {character.gameObject.name}. Pin will not be removed.");
                return;
            }

            if(PinsByZdoid.TryGetValue(zdoid, out Minimap.PinData pin))
            {
                RemovePinMethod?.Invoke(Minimap.instance, new object[] { pin });
                PinsByZdoid.Remove(zdoid);
                Plugin.Log.LogInfo($"Removed pin for {characterName}. ZDOID: {zdoid}");
                return;
            }

            Plugin.Log.LogWarning($"No pin found for {characterName}. ZDOID: {zdoid}");
        }

        internal static ZDOID GetZdoId(Character character)
        {
            if (character == null || CharacterNViewField == null)
            {
                return ZDOID.None;
            }

            var nview = CharacterNViewField.GetValue(character) as ZNetView;

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
    }
}
