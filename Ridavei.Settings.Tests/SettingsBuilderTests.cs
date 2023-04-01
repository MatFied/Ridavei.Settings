using System;

using Ridavei.Settings.Exceptions;
using Ridavei.Settings.Internals;

using Ridavei.Settings.Tests.Managers;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
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
        public void SetDistributedCache__NoException()
        {
            Should.NotThrow(() =>
            {
                var builder = SettingsBuilder.CreateBuilder();
                builder.SetDistributedCache(new MemoryDistributedCache(Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions())), Consts.DefaultCacheTimeout);
            });
        }
    }
}
