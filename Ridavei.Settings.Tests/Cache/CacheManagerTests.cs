using System;
using System.Collections.Generic;
using System.Threading;

using Ridavei.Settings.Cache;
using Ridavei.Settings.Internals;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Shouldly;

namespace Ridavei.Settings.Tests.Cache
{
    [TestFixture]
    public class CacheManagerTests
    {
        [Test]
        public void Constructor_NullDistributedCache__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                _ = new CacheManager(null, Consts.DefaultCacheTimeout);
            });
        }

        [Test]
        public void Constructor_TimeoutLowerThenMinimal__RaisesException()
        {
            Should.Throw<ArgumentException>(() =>
            {
                _ = new CacheManager(new MemoryDistributedCache(Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions())), -1);
            });
        }

        [Test]
        public void Constructor__NoException()
        {
            Should.NotThrow(() =>
            {
                var cacheManager = new CacheManager(new MemoryDistributedCache(Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions())), Consts.DefaultCacheTimeout);
                cacheManager.ShouldNotBeNull();
            });
        }

        [Test]
        public void AddString__ObjectExistsInCache()
        {
            Should.NotThrow(() =>
            {
                var cacheManager = new CacheManager(new MemoryDistributedCache(Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions())), Consts.DefaultCacheTimeout);
                var key = "AddString__ObjectExistsInCache";
                var expectedValue = "Test";
                try
                {
                    cacheManager.AddString(key, expectedValue);
                    var val = cacheManager.GetString(key);
                    val.ShouldNotBeNull();
                    val.ShouldBe(expectedValue);
                }
                finally
                {
                    cacheManager.Remove(key);
                }
            });
        }

        [Test]
        public void AddString_ExpireTime__ObjectNotExistsInCache()
        {
            Should.NotThrow(() =>
            {
                var milliseconds = Consts.MinCacheTimeout;
                var cacheManager = new CacheManager(new MemoryDistributedCache(Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions())), milliseconds);
                var key = "AddString_ExpireTime__ObjectNotExistsInCache";
                var expectedValue = "Test";
                cacheManager.AddString(key, expectedValue);
                Thread.Sleep(milliseconds + 10);
                var val = cacheManager.GetString(key);
                val.ShouldBeNull();
            });
        }

        [Test]
        public void AddDictionary__ObjectExistsInCache()
        {
            Should.NotThrow(() =>
            {
                var cacheManager = new CacheManager(new MemoryDistributedCache(Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions())), Consts.DefaultCacheTimeout);
                var key = "AddDictionary__ObjectExistsInCache";
                Dictionary<string, string> expectedDict = new Dictionary<string, string>
                {
                    { "test", "test" }
                };
                try
                {
                    cacheManager.AddDictionary(key, expectedDict);
                    var val = cacheManager.GetDictionary(key);
                    val.ShouldNotBeNull();
                    val.ShouldBe(expectedDict);
                }
                finally
                {
                    cacheManager.Remove(key);
                }
            });
        }

        [Test]
        public void AddDictionary_ExpireTime__ObjectNotExistsInCache()
        {
            Should.NotThrow(() =>
            {
                var milliseconds = Consts.MinCacheTimeout;
                var cacheManager = new CacheManager(new MemoryDistributedCache(Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions())), milliseconds);
                var key = "AddDictionary_ExpireTime__ObjectNotExistsInCache";
                Dictionary<string, string> expectedDict = new Dictionary<string, string>
                {
                    { "test", "test" }
                };
                cacheManager.AddDictionary(key, expectedDict);
                Thread.Sleep(milliseconds + 10);
                var val = cacheManager.GetString(key);
                val.ShouldBeNull();
            });
        }

        [Test]
        public void GetString_NonExistingValue__GetNull()
        {
            Should.NotThrow(() =>
            {
                var key = "GetString_NonExistingValue__GetNull";
                var cacheManager = new CacheManager(new MemoryDistributedCache(Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions())), Consts.DefaultCacheTimeout);
                cacheManager.GetString(key).ShouldBeNull();
            });
        }

        [Test]
        public void GetString_AddValueToCache__GetValue()
        {
            Should.NotThrow(() =>
            {
                var milliseconds = 1000;
                var cacheManager = new CacheManager(new MemoryDistributedCache(Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions())), milliseconds);
                var key = "GetString_AddValueToCache__GetValue";
                var expectedValue = "Test";
                try
                {
                    cacheManager.AddString(key, expectedValue);
                    var val = cacheManager.GetString(key);
                    val.ShouldNotBeNull();
                    val.ShouldBe(expectedValue);
                }
                finally
                {
                    cacheManager.Remove(key);
                }
            });
        }

        [Test]
        public void GetString_AddValueToCache_MultipleGet__GetValue()
        {
            Should.NotThrow(() =>
            {
                var milliseconds = 10000;
                var cacheManager = new CacheManager(new MemoryDistributedCache(Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions())), milliseconds);
                var key = "GetString_AddValueToCache_MultipleGet__GetValue";
                var expectedValue = "Test";
                try
                {
                    cacheManager.AddString(key, expectedValue);
                    for (int i = 0; i < 10; i++)
                    {
                        var val = cacheManager.GetString(key);
                        val.ShouldNotBeNull();
                        val.ShouldBe(expectedValue);
                    }
                }
                finally
                {
                    cacheManager.Remove(key);
                }
            });
        }

        [Test]
        public void GetDictionary_NonExistingValue__GetNull()
        {
            Should.NotThrow(() =>
            {
                var key = "GetString_NonExistingValue__GetNull";
                var cacheManager = new CacheManager(new MemoryDistributedCache(Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions())), Consts.DefaultCacheTimeout);
                cacheManager.GetDictionary(key).ShouldBeNull();
            });
        }

        [Test]
        public void GetDictionary_AddValueToCache__GetValue()
        {
            Should.NotThrow(() =>
            {
                var milliseconds = 1000;
                var cacheManager = new CacheManager(new MemoryDistributedCache(Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions())), milliseconds);
                var key = "GetString_AddValueToCache__GetValue";
                Dictionary<string, string> expectedDict = new Dictionary<string, string>
                {
                    { "test", "test" }
                };
                try
                {
                    cacheManager.AddDictionary(key, expectedDict);
                    var val = cacheManager.GetDictionary(key);
                    val.ShouldNotBeNull();
                    val.ShouldBe(expectedDict);
                }
                finally
                {
                    cacheManager.Remove(key);
                }
            });
        }

        [Test]
        public void GetDictionary_AddValueToCache_MultipleGet__GetValue()
        {
            Should.NotThrow(() =>
            {
                var milliseconds = 10000;
                var cacheManager = new CacheManager(new MemoryDistributedCache(Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions())), milliseconds);
                var key = "GetString_AddValueToCache_MultipleGet__GetValue";
                Dictionary<string, string> expectedDict = new Dictionary<string, string>
                {
                    { "test", "test" }
                };
                try
                {
                    cacheManager.AddDictionary(key, expectedDict);
                    for (int i = 0; i < 10; i++)
                    {
                        var val = cacheManager.GetDictionary(key);
                        val.ShouldNotBeNull();
                        val.ShouldBe(expectedDict);
                    }
                }
                finally
                {
                    cacheManager.Remove(key);
                }
            });
        }

        [Test]
        public void Remove__RemoveFromCache()
        {
            Should.NotThrow(() =>
            {
                var milliseconds = 1000;
                var cacheManager = new CacheManager(new MemoryDistributedCache(Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions())), milliseconds);
                var key = "Remove__RemoveFromCache";
                var expectedValue = "Test";
                try
                {
                    cacheManager.AddString(key, expectedValue);
                    var val = cacheManager.GetString(key);
                    val.ShouldNotBeNull();
                    val.ShouldBe(expectedValue);
                }
                finally
                {
                    cacheManager.Remove(key);
                }
                cacheManager.GetString(key).ShouldBeNull();
            });
        }
    }
}
