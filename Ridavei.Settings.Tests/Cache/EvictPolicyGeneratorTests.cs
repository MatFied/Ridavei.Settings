using System;

using Ridavei.Settings.Cache;

using NUnit.Framework;
using Shouldly;

namespace Ridavei.Settings.Tests.Cache
{
    [TestFixture]
    public class EvictPolicyGeneratorTests
    {
        [Test]
        public void GetAbsoluteExpirationTime_TimeoutBelowZero__GetCurrentTime()
        {
            Should.NotThrow(() =>
            {
                var time = EvictPolicyGenerator.GetAbsoluteExpirationTime(-100);
                time.ShouldBeInRange(DateTimeOffset.UtcNow.AddMilliseconds(-1), DateTimeOffset.UtcNow);
            });
        }

        [Test]
        public void GetAbsoluteExpirationTime__GetValueFromFuture()
        {
            Should.NotThrow(() =>
            {
                var time = EvictPolicyGenerator.GetAbsoluteExpirationTime(100);
                Assert.True(DateTimeOffset.UtcNow < time);
            });
        }

        [Test]
        public void CreatePolicyItem__GetObject()
        {
            Should.NotThrow(() =>
            {
                var time = EvictPolicyGenerator.GetAbsoluteExpirationTime(100);
                var cacheItemPolicy = EvictPolicyGenerator.CreatePolicyItem(time);
                cacheItemPolicy.ShouldNotBeNull();
                cacheItemPolicy.AbsoluteExpiration.ShouldBe(time);
                cacheItemPolicy.RemovedCallback.ShouldNotBeNull();
            });
        }
    }
}
