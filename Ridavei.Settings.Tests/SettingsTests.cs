using System;
using System.Collections.Generic;
using System.Runtime.Caching;

using Ridavei.Settings.Tests.Settings;

using NUnit.Framework;
using Shouldly;

namespace Ridavei.Settings.Tests
{
    [TestFixture]
    internal class SettingsTests
    {
        private const string DictionaryName = "TestDict";
        private const string KeyName = "TestKey";

        [TearDown]
        public void TearDown()
        {
            MemoryCache.Default.Remove(CacheManager.GenerateKeyName(DictionaryName, KeyName));
            MemoryCache.Default.Remove(CacheManager.GenerateKeyNameForGetAllDictionary(DictionaryName));
        }

        [Test]
        public void Constructor_NullDictionaryName__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                new MockSettings(null, false, 0);
            });
        }

        [Test]
        public void Constructor_EmptyDictionaryName__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                new MockSettings("", false, 0);
            });
        }

        [Test]
        public void Constructor_WhitespaceDictionaryName__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                new MockSettings("     ", false, 0);
            });
        }

        [Test]
        public void Constructor__Created()
        {
            Should.NotThrow(() =>
            {
                bool useCache = true;
                int cacheTimeout = 10;

                var settings = new MockSettings(DictionaryName, useCache, cacheTimeout);
                settings.DictionaryName.ShouldBe(DictionaryName);
                settings.UseCache.ShouldBe(useCache);
                settings.CacheTimeout.ShouldBe(cacheTimeout);
            });
        }

        [Test]
        public void Constructor_CacheTimeoutBelowZero__CacheTimeoutGraterThenZero()
        {
            Should.NotThrow(() =>
            {
                bool useCache = true;
                int cacheTimeout = -10;

                var settings = new MockSettings(DictionaryName, useCache, cacheTimeout);
                settings.DictionaryName.ShouldBe(DictionaryName);
                settings.UseCache.ShouldBe(useCache);
                settings.CacheTimeout.ShouldNotBe(cacheTimeout);
                settings.CacheTimeout.ShouldBeGreaterThan(0);
            });
        }

        [Test]
        public void Set_NullKey__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                CreateSettings().Set(null, "Test");
            });
        }

        [Test]
        public void Set_EmptyKey__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                CreateSettings().Set("", "Test");
            });
        }

        [Test]
        public void Set_WhitespaceKey__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                CreateSettings().Set("            ", "Test");
            });
        }

        [Test]
        public void Set_NullValue__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                CreateSettings().Set(KeyName, null);
            });
        }

        [Test]
        public void Set_EmptyValue__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                CreateSettings().Set(KeyName, "");
            });
        }

        [Test]
        public void Set_WhitespaceValue__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                CreateSettings().Set(KeyName, "            ");
            });
        }

        [Test]
        public void Set__NoException()
        {
            Should.NotThrow(() =>
            {
                CreateSettings().Set(KeyName, "Test");
            });
        }

        [Test]
        public void Set_UseCache__NoException()
        {
            Should.NotThrow(() =>
            {
                CreateSettings(true).Set(KeyName, "Test");
            });
        }

        [Test]
        public void Get_NullKey__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                CreateSettings().Get(null);
            });
        }

        [Test]
        public void Get_EmptyKey__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                CreateSettings().Get("");
            });
        }

        [Test]
        public void Get_WhitespaceKey__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                CreateSettings().Get("            ");
            });
        }

        [Test]
        public void Get_NonExistingKey__RaisesException()
        {
            Should.Throw<KeyNotFoundException>(() =>
            {
                var settings = CreateSettings();
                settings.ReturnValue = false;
                settings.Get(KeyName);
            });
        }

        [Test]
        public void Get_NonExistingKey_UseCache__RaisesException()
        {
            Should.Throw<KeyNotFoundException>(() =>
            {
                var settings = CreateSettings(true);
                settings.ReturnValue = false;
                settings.Get(KeyName);
            });
        }

        [Test]
        public void Get__GetValue()
        {
            Should.NotThrow(() =>
            {
                var settings = CreateSettings();
                settings.Get(KeyName).ShouldNotBeNullOrEmpty();
                CacheManager.Get(CacheManager.GenerateKeyName(DictionaryName, KeyName)).ShouldBeNull();
            });
        }

        [Test]
        public void Get_UseCache__GetValue()
        {
            Should.NotThrow(() =>
            {
                var settings = CreateSettings(true);
                var val = settings.Get(KeyName);
                val.ShouldNotBeNullOrEmpty();
                var cacheVal = CacheManager.Get(CacheManager.GenerateKeyName(DictionaryName, KeyName)) as string;
                cacheVal.ShouldNotBeNullOrEmpty();
                val.ShouldBe(cacheVal);
            });
        }

        [Test]
        public void Get_Default_NullKey__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                CreateSettings().Get(null, null);
            });
        }

        [Test]
        public void Get_Default_EmptyKey__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                CreateSettings().Get("", null);
            });
        }

        [Test]
        public void Get_Default_WhitespaceKey__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                CreateSettings().Get("            ", null);
            });
        }

        [Test]
        public void Get_Default_NonExistingKey__ReturnDefault()
        {
            Should.NotThrow(() =>
            {
                var settings = CreateSettings();
                settings.ReturnValue = false;
                settings.Get(KeyName, "def").ShouldNotBeNullOrEmpty();
            });
        }

        [Test]
        public void Get_Default_NonExistingKey_UseCache__ReturnDefault()
        {
            Should.NotThrow(() =>
            {
                var defaultValue = "def";
                var settings = CreateSettings(true);
                settings.ReturnValue = false;
                var val = settings.Get(KeyName, defaultValue);
                val.ShouldNotBeNullOrEmpty();
                val.ShouldBe(defaultValue);
                CacheManager.Get(CacheManager.GenerateKeyName(DictionaryName, KeyName)).ShouldBeNull();
            });
        }

        [Test]
        public void Get_Default__GetValue()
        {
            Should.NotThrow(() =>
            {
                var defaultValue = "def";
                var settings = CreateSettings();
                var val = settings.Get(KeyName, defaultValue);
                val.ShouldNotBeNullOrEmpty();
                val.ShouldNotBe(defaultValue);
                CacheManager.Get(CacheManager.GenerateKeyName(DictionaryName, KeyName)).ShouldBeNull();
            });
        }

        [Test]
        public void Get_Default_UseCache__GetValue()
        {
            Should.NotThrow(() =>
            {
                var defaultValue = "def";
                var settings = CreateSettings(true);
                var val = settings.Get(KeyName, defaultValue);
                val.ShouldNotBeNullOrEmpty();
                val.ShouldNotBe(defaultValue);
                var cacheVal = CacheManager.Get(CacheManager.GenerateKeyName(DictionaryName, KeyName)) as string;
                cacheVal.ShouldNotBeNullOrEmpty();
                val.ShouldBe(cacheVal);
            });
        }

        [Test]
        public void GetAll_Empty__GetEmptyDictionary()
        {
            Should.NotThrow(() =>
            {
                var settings = CreateSettings();
                settings.ReturnValue = false;
                var val = settings.GetAll();
                val.ShouldNotBeNull();
                val.Count.ShouldBe(0);
                CacheManager.Get(CacheManager.GenerateKeyNameForGetAllDictionary(DictionaryName)).ShouldBeNull();
            });
        }

        [Test]
        public void GetAll_UseCache_Empty__GetEmptyDictionary()
        {
            Should.NotThrow(() =>
            {
                var settings = CreateSettings(true);
                settings.ReturnValue = false;
                var val = settings.GetAll();
                val.ShouldNotBeNull();
                val.Count.ShouldBe(0);
                var keys = CacheManager.Get(CacheManager.GenerateKeyNameForGetAllDictionary(DictionaryName)) as List<string>;
                keys.ShouldNotBeNull();
                keys.Count.ShouldBe(0);
            });
        }

        [Test]
        public void GetAll__GetDictionary()
        {
            Should.NotThrow(() =>
            {
                var settings = CreateSettings();
                var val = settings.GetAll();
                val.ShouldNotBeNull();
                val.Count.ShouldBe(10);
                CacheManager.Get(CacheManager.GenerateKeyNameForGetAllDictionary(DictionaryName)).ShouldBeNull();
            });
        }

        [Test]
        public void GetAll_UseCache__GetDictionary()
        {
            Should.NotThrow(() =>
            {
                var settings = CreateSettings(true);
                var val = settings.GetAll();
                val.ShouldNotBeNull();
                val.Count.ShouldBe(10);
                var keys = CacheManager.Get(CacheManager.GenerateKeyNameForGetAllDictionary(DictionaryName)) as List<string>;
                keys.ShouldNotBeNull();
                keys.Count.ShouldBe(10);
                for (int i = 0; i < keys.Count; i++)
                    MemoryCache.Default.Remove(CacheManager.GenerateKeyName(DictionaryName, i.ToString()));
            });
        }

        [Test]
        public void GetAll_UseCache_CallAgain__GetDictionary()
        {
            Should.NotThrow(() =>
            {
                var settings = CreateSettings(true);
                var val = settings.GetAll();
                val.ShouldNotBeNull();
                val.Count.ShouldBe(10);
                var valSecond = settings.GetAll();
                valSecond.ShouldNotBeNull();
                valSecond.Count.ShouldBe(10);
                valSecond.ShouldBe(val);
                var keys = CacheManager.Get(CacheManager.GenerateKeyNameForGetAllDictionary(DictionaryName)) as List<string>;
                keys.ShouldNotBeNull();
                keys.Count.ShouldBe(10);
                for (int i = 0; i < keys.Count; i++)
                    MemoryCache.Default.Remove(CacheManager.GenerateKeyName(DictionaryName, i.ToString()));
            });
        }

        private MockSettings CreateSettings(bool useCache = false)
        {
            return new MockSettings(DictionaryName, useCache, 0);
        }
    }
}
