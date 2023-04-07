using System;
using System.Collections.Generic;
using System.Text.Json;

using Ridavei.Settings.Cache;
using Ridavei.Settings.Internals;

using Microsoft.Extensions.Caching.Distributed;
using NUnit.Framework;
using Shouldly;
using NSubstitute;
using System.Text;
using System.Linq;

namespace Ridavei.Settings.Tests.Cache
{
    [TestFixture]
    public class CacheManagerTests
    {
        private IDistributedCache _fakeCache;

        [SetUp]
        public void SetUp()
        {
            _fakeCache = Substitute.For<IDistributedCache>();
        }

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
                _ = new CacheManager(_fakeCache, -1);
            });
        }

        [Test]
        public void Constructor__NoException()
        {
            Should.NotThrow(() =>
            {
                var cacheManager = new CacheManager(_fakeCache, Consts.DefaultCacheTimeout);
                cacheManager.ShouldNotBeNull();
            });
        }

        [Test]
        public void AddString__ObjectExistsInCache()
        {
            Should.NotThrow(() =>
            {
                var cacheManager = new CacheManager(_fakeCache, Consts.DefaultCacheTimeout);
                var key = "AddString__ObjectExistsInCache";
                var expectedValue = "Test";
                var expectedBytes = Encoding.UTF8.GetBytes(expectedValue);
                _fakeCache.Get(key).Returns(expectedBytes);
                cacheManager.AddString(key, expectedValue);
                var val = cacheManager.GetString(key);
                val.ShouldNotBeNull();
                val.ShouldBe(expectedValue);
                _fakeCache.Received(1).GetString(Arg.Any<string>());
                _fakeCache.Received(1).Set(Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>());
            });
        }

        [Test]
        public void AddDictionary__ObjectExistsInCache()
        {
            Should.NotThrow(() =>
            {
                var cacheManager = new CacheManager(_fakeCache, Consts.DefaultCacheTimeout);
                var key = "AddDictionary__ObjectExistsInCache";
                Dictionary<string, string> expectedDict = new Dictionary<string, string>
                {
                    { "test", "test" }
                };
                var expectedValue = JsonSerializer.Serialize(expectedDict);
                var expectedBytes = Encoding.UTF8.GetBytes(expectedValue);
                _fakeCache.Get(key).Returns(expectedBytes);
                cacheManager.AddDictionary(key, expectedDict);
                var val = cacheManager.GetDictionary(key);
                val.ShouldNotBeNull();
                val.ShouldBe(expectedDict);
                _fakeCache.Received(1).GetString(Arg.Any<string>());
                _fakeCache.Received(1).Set(Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>());
            });
        }

        [Test]
        public void GetString_NonExistingValue__GetNull()
        {
            Should.NotThrow(() =>
            {
                var key = "GetString_NonExistingValue__GetNull";
                var cacheManager = new CacheManager(_fakeCache, Consts.DefaultCacheTimeout);
                _fakeCache.Get(key).Returns((byte[])null);
                cacheManager.GetString(key).ShouldBeNull();
                _fakeCache.Received(1).GetString(Arg.Any<string>());
            });
        }

        [Test]
        public void GetString_AddValueToCache__GetValue()
        {
            Should.NotThrow(() =>
            {
                var milliseconds = 1000;
                var cacheManager = new CacheManager(_fakeCache, milliseconds);
                var key = "GetString_AddValueToCache__GetValue";
                var expectedValue = "Test";
                var expectedBytes = Encoding.UTF8.GetBytes(expectedValue);
                _fakeCache.Get(key).Returns(expectedBytes);
                cacheManager.AddString(key, expectedValue);
                var val = cacheManager.GetString(key);
                val.ShouldNotBeNull();
                val.ShouldBe(expectedValue);
                _fakeCache.Received(1).GetString(Arg.Any<string>());
                _fakeCache.Received(1).Set(Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>());
            });
        }

        [Test]
        public void GetDictionary_NonExistingValue__GetNull()
        {
            Should.NotThrow(() =>
            {
                var key = "GetString_NonExistingValue__GetNull";
                var cacheManager = new CacheManager(_fakeCache, Consts.DefaultCacheTimeout);
                _fakeCache.Get(key).Returns((byte[])null);
                cacheManager.GetDictionary(key).ShouldBeNull();
                _fakeCache.Received(1).GetString(Arg.Any<string>());
            });
        }

        [Test]
        public void GetDictionary_AddValueToCache__GetValue()
        {
            Should.NotThrow(() =>
            {
                var milliseconds = 1000;
                var cacheManager = new CacheManager(_fakeCache, milliseconds);
                var key = "GetString_AddValueToCache__GetValue";
                Dictionary<string, string> expectedDict = new Dictionary<string, string>
                {
                    { "test", "test" }
                };
                var expectedValue = JsonSerializer.Serialize(expectedDict);
                var expectedBytes = Encoding.UTF8.GetBytes(expectedValue);
                _fakeCache.Get(key).Returns(expectedBytes);
                cacheManager.AddDictionary(key, expectedDict);
                var val = cacheManager.GetDictionary(key);
                val.ShouldNotBeNull();
                val.ShouldBe(expectedDict);
                _fakeCache.Received(1).GetString(Arg.Any<string>());
                _fakeCache.Received(1).Set(Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>());
            });
        }

        [Test]
        public void Remove__RemoveFromCache()
        {
            Should.NotThrow(() =>
            {
                var milliseconds = 1000;
                var cacheManager = new CacheManager(_fakeCache, milliseconds);
                var key = "Remove__RemoveFromCache";
                cacheManager.Remove(key);
                _fakeCache.Received(1).Remove(Arg.Any<string>());
            });
        }
    }
}
