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
                SettingsBuilder.CreateBuilder().ShouldNotBeNull();
            });
        }

        [Test]
        public void SetManager_NullObject__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                SettingsBuilder
                    .CreateBuilder()
                    .SetManager(null);
            });
        }

        [Test]
        public void GetSettings_DictionaryNameNull__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                SettingsBuilder
                    .CreateBuilder()
                    .GetSettings(null);
            });
        }

        [Test]
        public void GetSettings_ManagerNotInitialized__RaisesException()
        {
            Should.Throw<ManagerNotExistsException>(() =>
            {
                SettingsBuilder
                    .CreateBuilder()
                    .GetSettings("test");
            });
        }

        [Test]
        public void GetSettings_InMemoryManager__RetrieveSettings()
        {
            Should.NotThrow(() =>
            {
                SettingsBuilder
                    .CreateBuilder()
                    .SetManager(new MockManager())
                    .GetSettings("test")
                    .ShouldNotBeNull();
            });
        }

        [Test]
        public void SetCache__NoException()
        {
            Should.NotThrow(() =>
            {
                SettingsBuilder
                    .CreateBuilder()
                    .SetCache(true);
            });
        }

        [Test]
        public void SetCacheTimeout__NoException()
        {
            Should.NotThrow(() =>
            {
                SettingsBuilder
                    .CreateBuilder()
                    .SetCacheTimeout(15);
            });
        }
    }
}
