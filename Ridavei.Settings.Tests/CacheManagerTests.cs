using System;
using System.Runtime.Caching;

using Ridavei.Settings.Cache;

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
                var time = EvictPolicyGenerator.GetAbsoluteExpirationTime(seconds);
                try
                {
                    CacheManager.Add(key, expectedValue, time);
                    var val = CacheManager.Get(key);
                    val.ShouldNotBeNull();
                    val.ShouldBe(expectedValue);
                }
                finally
                {
                    CacheManager.Remove(key);
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
                var time = EvictPolicyGenerator.GetAbsoluteExpirationTime(seconds);
                CacheManager.Add(key, expectedValue, time);
                Thread.Sleep((seconds + 1) * 1000);
                var val = CacheManager.Get(key);
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
                var time = EvictPolicyGenerator.GetAbsoluteExpirationTime(seconds);
                try
                {
                    CacheManager.Add(key, expectedValue, time);
                    var val = CacheManager.Get(key);
                    val.ShouldNotBeNull();
                    val.ShouldBe(expectedValue);
                }
                finally
                {
                    CacheManager.Remove(key);
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
                var time = EvictPolicyGenerator.GetAbsoluteExpirationTime(100);
                Assert.True(DateTimeOffset.UtcNow < time);
            });
        }
    }
}
