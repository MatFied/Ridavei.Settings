using System;

using Ridavei.Settings.Cache;
using Ridavei.Settings.Exceptions;
using Ridavei.Settings.Internals;

using Ridavei.Settings.Tests.Managers;

using Microsoft.Extensions.Caching.Distributed;
using NUnit.Framework;
using Shouldly;
using NSubstitute;

namespace Ridavei.Settings.Tests
{
    [TestFixture]
    internal class ManagerTests
    {
        private const string DictionaryName = "TestDict";

        private IDistributedCache _fakeCache;

        [SetUp]
        public void SetUp()
        {
            _fakeCache = Substitute.For<IDistributedCache>();
        }

        [Test]
        public void Constructor__Created()
        {
            Should.NotThrow(() =>
            {
                var manager = new MockManager(true, true);
                manager.ShouldNotBeNull();
            });
        }

        [Test]
        public void Init__SetValues()
        {
            Should.NotThrow(() =>
            {
                var cacheManager = new CacheManager(_fakeCache, Consts.DefaultCacheTimeout);
                var manager = new MockManager(true, true);
                manager.Init(cacheManager);
                manager.CacheManager.ShouldNotBeNull();
            });
        }

        [Test]
        public void Init_RunMultiple__SetValuesOnlyOnTheFirstCall()
        {
            Should.NotThrow(() =>
            {
                var cacheManager = new CacheManager(_fakeCache, Consts.DefaultCacheTimeout);
                var manager = new MockManager(true, true);
                manager.Init(cacheManager);

                for (int i = 0; i < 10; i++)
                    manager.Init(null);
                manager.CacheManager.ShouldNotBeNull();
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
        public void GetSettings_UseTheSameDictionaryName__GetTheSameObject()
        {
            Should.NotThrow(() =>
            {
                var manager = new MockManager(true, true);
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
        public void GetOrCreateSettings_UseTheSameDictionaryName__GetTheSameObject()
        {
            Should.NotThrow(() =>
            {
                var manager = new MockManager(true, false);
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
