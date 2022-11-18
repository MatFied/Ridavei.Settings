using System;

using Ridavei.Settings.Base;

using Ridavei.Settings.Tests.Settings;

namespace Ridavei.Settings.Tests.Managers
{
    public class MockManager : AManager
    {
        private readonly bool _createSettings;
        private readonly bool _getSettings;

        public MockManager(bool createSettings, bool getSettings) : base()
        {
            _createSettings = createSettings;
            _getSettings = getSettings;
        }

        protected override ASettings CreateSettingsObject(string dictionaryName)
        {
            return _createSettings ? new MockSettings(dictionaryName) : throw new Exception("Error");
        }

        protected override bool TryGetSettingsObject(string dictionaryName, out ASettings settings)
        {
            settings = _getSettings ? new MockSettings(dictionaryName) : null;
            return _getSettings;
        }
    }
}
