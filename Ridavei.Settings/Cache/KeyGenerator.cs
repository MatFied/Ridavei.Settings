namespace Ridavei.Settings.Cache
{
    internal static class KeyGenerator
    {
        /// <summary>
        /// Generates the key name of the key used to cache object.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <param name="key">Settings key</param>
        /// <returns></returns>
        public static string Generate(string dictionaryName, string key)
        {
            return string.Concat(GenerateForDictionary(dictionaryName), "__", key);
        }

        /// <summary>
        /// Generates the key name for the GetAll method used to cache settings keys.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <returns></returns>
        public static string GenerateForGetAllDictionary(string dictionaryName)
        {
            return string.Concat(GenerateForDictionary(dictionaryName), "_GetAllDictionary");
        }

        /// <summary>
        /// Generates the key name for the dictionary.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <returns></returns>
        public static string GenerateForDictionary(string dictionaryName)
        {
            return string.Concat("Dict_", dictionaryName);
        }
    }
}
