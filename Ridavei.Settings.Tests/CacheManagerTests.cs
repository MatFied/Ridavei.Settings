using System;
using System.Runtime.Caching;

using Ridavei.Settings.Tests.Managers;

using NUnit.Framework;
using Shouldly;
using System.Threading;

namespace Ridavei.Settings.Tests
{
    [TestFixture]
    public class CacheManagerTests
    {
        [Test]
        public void Add__ObjectExistsInCache()
        {
            Should.NotThrow(() =>
            {
                var seconds = 100;
                var key = "Add__ObjectExistsInCache";
                var expectedValue = "Test";
                var time = CacheManager.GetAbsoluteExpiration(seconds);
                try
                {
                    CacheManager.Add(key, expectedValue, time);
                    var val = MemoryCache.Default.Get(key);
                    val.ShouldNotBeNull();
                    val.ShouldBe(expectedValue);
                }
                finally
                {
                    MemoryCache.Default.Remove(key);
                }
            });
        }

        [Test]
        public void Add_ExpireTime__ObjectNotExistsInCache()
        {
            Should.NotThrow(() =>
            {
                var seconds = 2;
                var key = "Add_ExpireTime__ObjectNotExistsInCache";
                var expectedValue = "Test";
                var time = CacheManager.GetAbsoluteExpiration(seconds);
                CacheManager.Add(key, expectedValue, time);
                Thread.Sleep((seconds + 1) * 1000);
                var val = MemoryCache.Default.Get(key);
                val.ShouldBeNull();
            });
        }

        [Test]
        public void Get_NonExistingValue__GetNull()
        {
            Should.NotThrow(() =>
            {
                var seconds = 100;
                var key = "Get_NonExistingValue__GetNull";
                var expectedValue = "Test";
                var time = CacheManager.GetAbsoluteExpiration(seconds);
                try
                {
                    CacheManager.Add(key, expectedValue, time);
                    var val = CacheManager.Get(key);
                    val.ShouldNotBeNull();
                    val.ShouldBe(expectedValue);
                }
                finally
                {
                    MemoryCache.Default.Remove(key);
                }
            });
        }

        [Test]
        public void Get_AddValueToCache__GetValue()
        {
            Should.NotThrow(() =>
            {
                var key = "Get_AddValueToCache__GetValue";
                CacheManager.Get(key).ShouldBeNull();
            });
        }

        [Test]
        public void GetAbsoluteExpiration__GetValueFromFuture()
        {
            Should.NotThrow(() =>
            {
                var time = CacheManager.GetAbsoluteExpiration(100);
                Assert.True(DateTimeOffset.UtcNow < time);
            });
        }

        [Test]
        public void GenerateKeyName_EmptyDictionaryName__GetValue()
        {
            Should.NotThrow(() =>
            {
                CacheManager.GenerateKeyName(null, "Test").ShouldNotBeNullOrEmpty();
            });
        }

        [Test]
        public void GenerateKeyName_EmptyKey__GetValue()
        {
            Should.NotThrow(() =>
            {
                CacheManager.GenerateKeyName("Test", null).ShouldNotBeNullOrEmpty();
            });
        }

        [Test]
        public void GenerateKeyName__GetValue()
        {
            Should.NotThrow(() =>
            {
                CacheManager.GenerateKeyName("Test", "Test").ShouldNotBeNullOrEmpty();
            });
        }

        [Test]
        public void GenerateKeyNameForGetAllDictionary_EmptyDictionaryName__GetValue()
        {
            Should.NotThrow(() =>
            {
                CacheManager.GenerateKeyNameForGetAllDictionary(null).ShouldNotBeNullOrEmpty();
            });
        }

        [Test]
        public void GenerateKeyNameForGetAllDictionary__GetValue()
        {
            Should.NotThrow(() =>
            {
                CacheManager.GenerateKeyNameForGetAllDictionary("Test").ShouldNotBeNullOrEmpty();
            });
        }
    }
}
