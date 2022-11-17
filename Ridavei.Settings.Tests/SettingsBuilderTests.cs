using System;

using Ridavei.Settings.Exceptions;

using Ridavei.Settings.Tests.Managers;

using NUnit.Framework;
using Shouldly;

namespace Ridavei.Settings.Tests
{
    [TestFixture]
    public class SettingsBuilderTests
    {
        [Test]
        public void CreateBuilder__CreatesBuilder()
        {
            Should.NotThrow(() =>
            {
                using (var settings = SettingsBuilder.CreateBuilder())
                    settings.ShouldNotBeNull();
            });
        }

        [Test]
        public void SetManager_NullObject__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                using (var settings = SettingsBuilder.CreateBuilder())
                    settings.SetManager(null);
            });
        }

        [Test]
        public void GetSettings_DictionaryNameNull__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                using (var settings = SettingsBuilder.CreateBuilder())
                    settings.GetSettings(null);
            });
        }

        [Test]
        public void GetSettings_ManagerNotInitialized__RaisesException()
        {
            Should.Throw<ManagerNotExistsException>(() =>
            {
                using (var settings = SettingsBuilder.CreateBuilder())
                    settings.GetSettings("test");
            });
        }

        [Test]
        public void GetSettings_InMemoryManager__RetrieveSettings()
        {
            Should.NotThrow(() =>
            {
                using (var settings = SettingsBuilder.CreateBuilder())
                    settings
                        .SetManager(new MockManager())
                        .GetSettings("test")
                        .ShouldNotBeNull();
            });
        }

        [Test]
        public void EnableCache__NoException()
        {
            Should.NotThrow(() =>
            {
                using (var settings = SettingsBuilder.CreateBuilder())
                    settings.EnableCache();
            });
        }

        [Test]
        public void SetCacheTimeout_BelowMinimumValue__RaisesException()
        {
            Should.Throw<ArgumentException>(() =>
            {
                using (var settings = SettingsBuilder.CreateBuilder())
                    settings.SetCacheTimeout(-100);
            });
        }

        [Test]
        public void SetCacheTimeout__NoException()
        {
            Should.NotThrow(() =>
            {
                using (var settings = SettingsBuilder.CreateBuilder())
                    settings.SetCacheTimeout(15);
            });
        }
    }
}
