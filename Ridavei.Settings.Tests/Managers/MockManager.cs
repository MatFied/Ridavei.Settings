using Ridavei.Settings.Base;

using Ridavei.Settings.Tests.Settings;

namespace Ridavei.Settings.Tests.Managers
{
    internal class MockManager : AManager
    {
        public MockManager() : base() { }

        protected override ASettings GetSettingsObject(string dictionaryName)
        {
            return new MockSettings(dictionaryName, UseCache, CacheTimeout);
        }
    }
}
