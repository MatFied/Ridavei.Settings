using System.Collections.Generic;

using Ridavei.Settings.Base;

namespace Ridavei.Settings.Tests.Settings
{
    internal class MockSettings : ASettings
    {
        public bool ReturnValue = true;

        public MockSettings(string dictionaryName, bool useCache, int cacheTimeout) : base(dictionaryName, useCache, cacheTimeout) { }

        protected override IReadOnlyDictionary<string, string> GetAllValues()
        {
            var res = new Dictionary<string, string>();
            if (ReturnValue)
                for (int i = 0; i < 10; i++)
                    res.Add(i.ToString(), i.ToString());
            return res;
        }

        protected override bool TryGetValue(string key, out string value)
        {
            value = ReturnValue ? "Mock" : null;
            return ReturnValue;
        }

        protected override void SetValue(string key, string value) { }
    }
}
