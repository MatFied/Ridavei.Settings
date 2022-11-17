using Ridavei.Settings.Base;

using Ridavei.Settings.Tests.Settings;

namespace Ridavei.Settings.Tests.Managers
{
    internal class MockManager : AManager
    {
        public MockManager() : base() { }

        protected override ASettings CreateSettingsObject(string dictionaryName)
        {
            throw new System.NotImplementedException();
        }

        protected override bool TryGetSettingsObject(string dictionaryName, out ASettings settings)
        {
            settings = new MockSettings(dictionaryName);
            return true;
        }
    }
}
