using System;

using Ridavei.Settings.Exceptions;
using Ridavei.Settings.Tests.Managers;

using NUnit.Framework;
using Shouldly;

namespace Ridavei.Settings.Tests
{
    [TestFixture]
    internal class ManagerTests
    {
        private const string DictionaryName = "TestDict";

        [Test]
        public void Constructor__Created()
        {
            Should.NotThrow(() =>
            {
                var manager = new MockManager(true, true);
                manager.UseCache.ShouldBe(false);
                manager.CacheTimeout.ShouldBe(0);
            });
        }

        [Test]
        public void Init__SetValues()
        {
            Should.NotThrow(() =>
            {
                var useCache = true;
                var cacheTimeout = 50;
                var manager = new MockManager(true, true);
                manager.Init(useCache, cacheTimeout);
                manager.UseCache.ShouldBe(useCache);
                manager.CacheTimeout.ShouldBe(cacheTimeout);
            });
        }

        [Test]
        public void Init_RunMultiple__SetValuesOnlyOnTheFirstCall()
        {
            Should.NotThrow(() =>
            {
                var useCache = true;
                var cacheTimeout = 50;
                var manager = new MockManager(true, true);
                manager.Init(useCache, cacheTimeout);
                
                for (int i = 0; i < 10; i++)
                    manager.Init(i % 2 == 1, i);
                manager.UseCache.ShouldBe(useCache);
                manager.CacheTimeout.ShouldBe(cacheTimeout);
            });
        }

        [Test]
        public void GetSettings_NullDictionaryName__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                var manager = new MockManager(true, true);
                manager.GetSettings(null);
            });
        }

        [Test]
        public void GetSettings_EmptyDictionaryName__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                var manager = new MockManager(true, true);
                manager.GetSettings("");
            });
        }

        [Test]
        public void GetSettings_WhitespaceDictionaryName__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                var manager = new MockManager(true, true);
                manager.GetSettings("     ");
            });
        }

        [Test]
        public void GetSettings_ExistingSettings__GetObject()
        {
            Should.NotThrow(() =>
            {
                var manager = new MockManager(true, true);
                using (var settings = manager.GetSettings(DictionaryName))
                    settings.ShouldNotBeNull();
            });
        }

        [Test]
        public void GetSettings_NonExistingSettings__RaisesException()
        {
            Should.Throw<DictionaryNotFoundException>(() =>
            {
                var manager = new MockManager(true, false);
                var settings = manager.GetSettings(DictionaryName);
            });
        }

        [Test]
        public void GetSettings_UseTheSameDictionaryName__GetDifferentObjects()
        {
            Should.NotThrow(() =>
            {
                var manager = new MockManager(true, true);
                using (var settings = manager.GetSettings(DictionaryName))
                {
                    settings.ShouldNotBeNull();
                    var settingsToCompare = manager.GetSettings(DictionaryName);
                    settingsToCompare.ShouldNotBeNull();
                    settingsToCompare.ShouldNotBe(settings);
                }
            });
        }

        [Test]
        public void GetSettings_UseCache_UseTheSameDictionaryName__GetTheSameObject()
        {
            Should.NotThrow(() =>
            {
                var manager = new MockManager(true, true);
                manager.Init(true, 100);

                using (var settings = manager.GetSettings(DictionaryName))
                {
                    settings.ShouldNotBeNull();
                    var settingsToCompare = manager.GetSettings(DictionaryName);
                    settingsToCompare.ShouldNotBeNull();
                    settingsToCompare.ShouldBe(settings);
                }
            });
        }

        [Test]
        public void GetOrCreateSettings_NullDictionaryName__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                var manager = new MockManager(true, true);
                manager.GetOrCreateSettings(null);
            });
        }

        [Test]
        public void GetOrCreateSettings_EmptyDictionaryName__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                var manager = new MockManager(true, true);
                manager.GetOrCreateSettings("");
            });
        }

        [Test]
        public void GetOrCreateSettings_WhitespaceDictionaryName__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                var manager = new MockManager(true, true);
                manager.GetOrCreateSettings("     ");
            });
        }

        [Test]
        public void GetOrCreateSettings_ExistingSettings__GetObject()
        {
            Should.NotThrow(() =>
            {
                var manager = new MockManager(true, true);
                using (var settings = manager.GetOrCreateSettings(DictionaryName))
                    settings.ShouldNotBeNull();
            });
        }

        [Test]
        public void GetOrCreateSettings_CannotCreateSettings__RaisesException()
        {
            Should.Throw<Exception>(() =>
            {
                var manager = new MockManager(false, false);
                var settings = manager.GetOrCreateSettings(DictionaryName);
            });
        }

        [Test]
        public void GetOrCreateSettings_UseTheSameDictionaryName__GetDifferentObjects()
        {
            Should.NotThrow(() =>
            {
                var manager = new MockManager(true, false);
                using (var settings = manager.GetOrCreateSettings(DictionaryName))
                {
                    settings.ShouldNotBeNull();
                    var settingsToCompare = manager.GetOrCreateSettings(DictionaryName);
                    settingsToCompare.ShouldNotBeNull();
                    settingsToCompare.ShouldNotBe(settings);
                }
            });
        }

        [Test]
        public void GetOrCreateSettings_UseCache_UseTheSameDictionaryName__GetTheSameObject()
        {
            Should.NotThrow(() =>
            {
                var manager = new MockManager(true, false);
                manager.Init(true, 100);

                using (var settings = manager.GetOrCreateSettings(DictionaryName))
                {
                    settings.ShouldNotBeNull();
                    var settingsToCompare = manager.GetOrCreateSettings(DictionaryName);
                    settingsToCompare.ShouldNotBeNull();
                    settingsToCompare.ShouldBe(settings);
                }
            });
        }
    }
}
