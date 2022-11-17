using System;
using System.Collections.Generic;
using System.Runtime.Caching;

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
                using (var manager = new MockManager())
                {
                    manager.UseCache.ShouldBe(false);
                    manager.CacheTimeout.ShouldBe(0);
                }
            });
        }

        [Test]
        public void Init__SetValues()
        {
            Should.NotThrow(() =>
            {
                var useCache = true;
                var cacheTimeout = 50;
                using (var manager = new MockManager())
                {
                    manager.Init(useCache, cacheTimeout);
                    manager.UseCache.ShouldBe(useCache);
                    manager.CacheTimeout.ShouldBe(cacheTimeout);
                }
            });
        }

        [Test]
        public void Init_RunMultiple__SetValuesOnlyOnTheFirstCall()
        {
            Should.NotThrow(() =>
            {
                var useCache = true;
                var cacheTimeout = 50;
                using (var manager = new MockManager())
                {
                    manager.Init(useCache, cacheTimeout);

                    for (int i = 0; i < 10; i++)
                        manager.Init(i % 2 == 1, i);
                    manager.UseCache.ShouldBe(useCache);
                    manager.CacheTimeout.ShouldBe(cacheTimeout);
                }
            });
        }

        [Test]
        public void GetSettings_NullDictionaryName__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                using (var manager = new MockManager())
                    manager.GetSettings(null);
            });
        }

        [Test]
        public void GetSettings_EmptyDictionaryName__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                using (var manager = new MockManager())
                    manager.GetSettings("");
            });
        }

        [Test]
        public void GetSettings_WhitespaceDictionaryName__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                using (var manager = new MockManager())
                    manager.GetSettings("     ");
            });
        }

        [Test]
        public void GetSettings__GetObject()
        {
            Should.NotThrow(() =>
            {
                using (var manager = new MockManager())
                {
                    var settings = manager.GetSettings(DictionaryName);
                    settings.ShouldNotBeNull();
                }
            });
        }

        [Test]
        public void GetSettings_UseTheSameDictionaryName__GetDifferentObjects()
        {
            Should.NotThrow(() =>
            {
                using (var manager = new MockManager())
                {
                    var settings = manager.GetSettings(DictionaryName);
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
                using (var manager = new MockManager())
                {
                    manager.Init(true, 100);

                    var settings = manager.GetSettings(DictionaryName);
                    settings.ShouldNotBeNull();
                    var settingsToCompare = manager.GetSettings(DictionaryName);
                    settingsToCompare.ShouldNotBeNull();
                    settingsToCompare.ShouldBe(settings);
                }
            });
        }
    }
}
