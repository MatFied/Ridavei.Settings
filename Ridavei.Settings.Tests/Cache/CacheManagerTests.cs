using System.Threading;

using Ridavei.Settings.Cache;

using Ridavei.Settings.Tests.Settings;

using NUnit.Framework;
using Shouldly;

namespace Ridavei.Settings.Tests.Cache
{
    [TestFixture]
    public class CacheManagerTests
    {
        [Test]
        public void Add__ObjectExistsInCache()
        {
            Should.NotThrow(() =>
            {
                var milliseconds = 1000;
                var key = "Add__ObjectExistsInCache";
                var expectedValue = "Test";
                var time = EvictPolicyGenerator.GetAbsoluteExpirationTime(milliseconds);
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
                var milliseconds = 20;
                var key = "Add_ExpireTime__ObjectNotExistsInCache";
                var expectedValue = "Test";
                var time = EvictPolicyGenerator.GetAbsoluteExpirationTime(milliseconds);
                CacheManager.Add(key, expectedValue, time);
                Thread.Sleep(milliseconds + 10);
                var val = CacheManager.Get(key);
                val.ShouldBeNull();
            });
        }

        [Test]
        public void Get_NonExistingValue__GetNull()
        {
            Should.NotThrow(() =>
            {
                var key = "Get_NonExistingValue__GetNull";
                CacheManager.Get(key).ShouldBeNull();
            });
        }

        [Test]
        public void Get_AddValueToCache__GetValue()
        {
            Should.NotThrow(() =>
            {
                var milliseconds = 1000;
                var key = "Get_AddValueToCache__GetValue";
                var expectedValue = "Test";
                var time = EvictPolicyGenerator.GetAbsoluteExpirationTime(milliseconds);
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
        public void Get_AddValueToCache_MultipleGet__GetValue()
        {
            Should.NotThrow(() =>
            {
                var milliseconds = 10000;
                var key = "Get_AddValueToCache_MultipleGet__GetValue";
                var expectedValue = "Test";
                var time = EvictPolicyGenerator.GetAbsoluteExpirationTime(milliseconds);
                try
                {
                    CacheManager.Add(key, expectedValue, time);
                    for (int i = 0; i < 10; i++)
                    {
                        var val = CacheManager.Get(key);
                        val.ShouldNotBeNull();
                        val.ShouldBe(expectedValue);
                    }
                }
                finally
                {
                    CacheManager.Remove(key);
                }
            });
        }

        [Test]
        public void Remove__RemoveFromCache()
        {
            Should.NotThrow(() =>
            {
                var milliseconds = 1000;
                var key = "Get_AddValueToCache__GetValue";
                var expectedValue = "Test";
                var time = EvictPolicyGenerator.GetAbsoluteExpirationTime(milliseconds);
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
                CacheManager.Get(key).ShouldBeNull();
            });
        }

        [Test]
        public void Remove_DisposableObject__RemoveFromCache()
        {
            Should.NotThrow(() =>
            {
                var milliseconds = 1000;
                var key = "Get_AddValueToCache__GetValue";
                var expectedValue = new MockSettings("Test");
                var time = EvictPolicyGenerator.GetAbsoluteExpirationTime(milliseconds);
                try
                {
                    CacheManager.Add(key, expectedValue, time);
                    var val = CacheManager.Get(key);
                    val.ShouldNotBeNull();
                }
                finally
                {
                    CacheManager.Remove(key);
                }
                CacheManager.Get(key).ShouldBeNull();
            });
        }
    }
}
