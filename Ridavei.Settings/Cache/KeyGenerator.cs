using System.Collections.Generic;

namespace Ridavei.Settings.Cache
{
    internal static class KeyGenerator
    {
        private readonly static string StartKey = "Dict_";
        private readonly static string DoubleUnderlineKey = "__";
        private readonly static string GetAllDictionaryKey = "_GetAllDictionary";

        /// <summary>
        /// Generates the key name of the key used to cache object.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <param name="key">Settings key</param>
        /// <returns></returns>
        public static string Generate(string dictionaryName, string key)
        {
            if (string.IsNullOrWhiteSpace(dictionaryName) || string.IsNullOrWhiteSpace(key))
                return string.Empty;
            if (!KeyPool.DictKeys.TryGetValue(dictionaryName, out var dict))
            {
                dict = new Dictionary<string, string>();
                KeyPool.DictKeys.Add(dictionaryName, dict);
            }

            if (dict.TryGetValue(dictionaryName, out var genKey))
                return genKey;
            var res = string.Concat(GenerateForDictionary(dictionaryName), DoubleUnderlineKey, key);
            dict.Add(dictionaryName, res);
            return res;
        }

        /// <summary>
        /// Generates the key name for the GetAll method used to cache settings keys.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <returns></returns>
        public static string GenerateForGetAllDictionary(string dictionaryName)
        {
            if (string.IsNullOrWhiteSpace(dictionaryName))
                return string.Empty;
            if (KeyPool.DictKeysForAllGet.TryGetValue(dictionaryName, out var key))
                return key;
            var res = string.Concat(GenerateForDictionary(dictionaryName), GetAllDictionaryKey);
            KeyPool.DictKeysForAllGet.Add(dictionaryName, res);
            return res;
        }

        /// <summary>
        /// Generates the key name for the dictionary.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <returns></returns>
        public static string GenerateForDictionary(string dictionaryName)
        {
            if (string.IsNullOrWhiteSpace(dictionaryName))
                return string.Empty;
            if (KeyPool.DictVals.TryGetValue(dictionaryName, out var key))
                return key;
            var res = string.Concat(StartKey, dictionaryName);
            KeyPool.DictVals.Add(dictionaryName, res);
            return res;
        }
    }
}
