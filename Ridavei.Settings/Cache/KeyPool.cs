using System.Collections.Generic;

namespace Ridavei.Settings.Cache
{
    internal class KeyPool
    {
        public readonly static Dictionary<string, string> DictVals = new Dictionary<string, string>();
        public readonly static Dictionary<string, string> DictKeysForAllGet = new Dictionary<string, string>();
        public readonly static Dictionary<string, Dictionary<string, string>> DictKeys = new Dictionary<string, Dictionary<string, string>>();
    }
}
