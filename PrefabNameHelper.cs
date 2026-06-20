namespace HexRareScanner
{
    internal static class PrefabNameHelper
    {
        internal static string GetPrefabNameFromClone(string objectName)
        {
            const string CloneSuffix = "(Clone)";

            if (string.IsNullOrEmpty(objectName))
            {
                return string.Empty;
            }

            if (objectName.EndsWith(CloneSuffix))
            {
                return objectName.Substring(0, objectName.Length - CloneSuffix.Length);
            }

            return objectName;
        }
    }
}
