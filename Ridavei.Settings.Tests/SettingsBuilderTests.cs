using System;

using Ridavei.Settings.Exceptions;
using Ridavei.Settings.Internals;

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
                var builder = SettingsBuilder.CreateBuilder();
                builder.ShouldNotBeNull();
            });
        }

        [Test]
        public void SetManager_NullObject__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                var builder = SettingsBuilder.CreateBuilder();
                builder.SetManager(null);
            });
        }

        [Test]
        public void GetSettings_DictionaryNameNull__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                var builder = SettingsBuilder.CreateBuilder();
                builder.GetSettings(null);
            });
        }

        [Test]
        public void GetSettings_ManagerNotInitialized__RaisesException()
        {
            Should.Throw<ManagerNotExistsException>(() =>
            {
                var builder = SettingsBuilder.CreateBuilder();
                builder.GetSettings("test");
            });
        }

        [Test]
        public void GetSettings_MockManager__RetrieveSettings()
        {
            Should.NotThrow(() =>
            {
                var builder = SettingsBuilder
                    .CreateBuilder()
                    .SetManager(new MockManager(true, true));
                using (var settings = builder.GetSettings("test"))
                    settings.ShouldNotBeNull();
            });
        }

        [Test]
        public void GetOrCreateSettings_DictionaryNameNull__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                var builder = SettingsBuilder.CreateBuilder();
                builder.GetOrCreateSettings(null);
            });
        }

        [Test]
        public void GetOrCreateSettings_ManagerNotInitialized__RaisesException()
        {
            Should.Throw<ManagerNotExistsException>(() =>
            {
                var builder = SettingsBuilder.CreateBuilder();
                builder.GetOrCreateSettings("test");
            });
        }

        [Test]
        public void GetOrCreateSettings_MockManager__RetrieveSettings()
        {
            Should.NotThrow(() =>
            {
                var builder = SettingsBuilder
                    .CreateBuilder()
                    .SetManager(new MockManager(true, true));
                using (var settings = builder.GetOrCreateSettings("test"))
                    builder.ShouldNotBeNull();
            });
        }

        [Test]
        public void EnableCache__NoException()
        {
            Should.NotThrow(() =>
            {
                var builder = SettingsBuilder.CreateBuilder();
                builder.EnableCache();
            });
        }

        [Test]
        public void SetCacheTimeout_BelowMinimumValue__RaisesException()
        {
            Should.Throw<ArgumentException>(() =>
            {
                var builder = SettingsBuilder.CreateBuilder();
                builder.SetCacheTimeout(-100);
            });
        }

        [Test]
        public void SetCacheTimeout__NoException()
        {
            Should.NotThrow(() =>
            {
                var builder = SettingsBuilder.CreateBuilder();
                builder.SetCacheTimeout(Consts.MinCacheTimeout);
            });
        }
    }
}
