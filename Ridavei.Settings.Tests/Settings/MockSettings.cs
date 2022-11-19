using System.Collections.Generic;

using Ridavei.Settings.Base;

namespace Ridavei.Settings.Tests.Settings
{
    public class MockSettings : ASettings
    {
        private static string _get = "Mock";
        private static Dictionary<string, string> _getAllEmptyDict = new Dictionary<string, string>();
        private static Dictionary<string, string> _getAllDict = new Dictionary<string, string>
        {
            { "0", "0" },
            { "1", "1" },
            { "2", "2" },
            { "3", "3" },
            { "4", "4" },
            { "5", "5" },
            { "6", "6" },
            { "7", "7" },
            { "8", "8" },
            { "9", "9" }
        };

        public bool ReturnValue = true;

        public MockSettings(string dictionaryName) : base(dictionaryName) { }

        protected override IReadOnlyDictionary<string, string> GetAllValues()
        {
            return ReturnValue ? _getAllDict : _getAllEmptyDict;
        }

        protected override bool TryGetValue(string key, out string value)
        {
            value = ReturnValue ? _get : null;
            return ReturnValue;
        }

        protected override void SetValue(string key, string value) { }
    }
}
