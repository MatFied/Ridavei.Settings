using System;
using System.Collections.Generic;

using Ridavei.Settings.Cache;

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
            CacheManager.Remove(KeyGenerator.Generate(DictionaryName, KeyName));
            CacheManager.Remove(KeyGenerator.GenerateForGetAllDictionary(DictionaryName));
        }

        [Test]
        public void Constructor_NullDictionaryName__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                new MockSettings(null);
            });
        }

        [Test]
        public void Constructor_EmptyDictionaryName__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                new MockSettings("");
            });
        }

        [Test]
        public void Constructor_WhitespaceDictionaryName__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                new MockSettings("     ");
            });
        }

        [Test]
        public void Constructor__Created()
        {
            Should.NotThrow(() =>
            {
                using (var settings = new MockSettings(DictionaryName))
                {
                    settings.DictionaryName.ShouldBe(DictionaryName);
                    settings.UseCache.ShouldBe(false);
                    settings.CacheTimeout.ShouldBe(0);
                }
            });
        }

        [Test]
        public void Init__SetValues()
        {
            Should.NotThrow(() =>
            {
                bool useCache = true;
                int cacheTimeout = -10;

                using (var settings = new MockSettings(DictionaryName))
                {
                    settings.Init(useCache, cacheTimeout);

                    settings.DictionaryName.ShouldBe(DictionaryName);
                    settings.UseCache.ShouldBe(useCache);
                    settings.CacheTimeout.ShouldBe(cacheTimeout);
                }
            });
        }

        [Test]
        public void Init_RunMultiple__SetValuesOnlyOnTheFirstCall()
        {
            Should.NotThrow(() =>
            {
                bool useCache = true;
                int cacheTimeout = -10;

                using (var settings = new MockSettings(DictionaryName))
                {
                    settings.Init(useCache, cacheTimeout);
                    for (int i = 0; i < 10; i++)
                        settings.Init(i % 2 == 1, i);

                    settings.DictionaryName.ShouldBe(DictionaryName);
                    settings.UseCache.ShouldBe(useCache);
                    settings.CacheTimeout.ShouldBe(cacheTimeout);
                }
            });
        }

        [Test]
        public void Set_NullKey__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                using (var settings = CreateSettings())
                    settings.Set(null, "Test");
            });
        }

        [Test]
        public void Set_EmptyKey__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                using (var settings = CreateSettings())
                    settings.Set("", "Test");
            });
        }

        [Test]
        public void Set_WhitespaceKey__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                using (var settings = CreateSettings())
                    settings.Set("            ", "Test");
            });
        }

        [Test]
        public void Set_NullValue__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                using (var settings = CreateSettings())
                    settings.Set(KeyName, null);
            });
        }

        [Test]
        public void Set_EmptyValue__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                using (var settings = CreateSettings())
                    settings.Set(KeyName, "");
            });
        }

        [Test]
        public void Set_WhitespaceValue__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                using (var settings = CreateSettings())
                    settings.Set(KeyName, "            ");
            });
        }

        [Test]
        public void Set__NoException()
        {
            Should.NotThrow(() =>
            {
                using (var settings = CreateSettings())
                    settings.Set(KeyName, "Test");
            });
        }

        [Test]
        public void Set_UseCache__NoException()
        {
            Should.NotThrow(() =>
            {
                using (var settings = CreateSettings(true))
                    settings.Set(KeyName, "Test");
            });
        }

        [Test]
        public void Get_NullKey__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                using (var settings = CreateSettings())
                    settings.Get(null);
            });
        }

        [Test]
        public void Get_EmptyKey__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                using (var settings = CreateSettings())
                    settings.Get("");
            });
        }

        [Test]
        public void Get_WhitespaceKey__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                using (var settings = CreateSettings())
                    settings.Get("            ");
            });
        }

        [Test]
        public void Get_NonExistingKey__RaisesException()
        {
            Should.Throw<KeyNotFoundException>(() =>
            {
                using (var settings = CreateSettings())
                {
                    settings.ReturnValue = false;
                    settings.Get(KeyName);
                }
            });
        }

        [Test]
        public void Get_NonExistingKey_UseCache__RaisesException()
        {
            Should.Throw<KeyNotFoundException>(() =>
            {
                using (var settings = CreateSettings(true))
                {
                    settings.ReturnValue = false;
                    settings.Get(KeyName);
                }
            });
        }

        [Test]
        public void Get__GetValue()
        {
            Should.NotThrow(() =>
            {
                using (var settings = CreateSettings())
                    settings.Get(KeyName).ShouldNotBeNullOrEmpty();
                CacheManager.Get(KeyGenerator.Generate(DictionaryName, KeyName)).ShouldBeNull();
            });
        }

        [Test]
        public void Get_UseCache__GetValue()
        {
            Should.NotThrow(() =>
            {
                using (var settings = CreateSettings(true))
                {
                    var val = settings.Get(KeyName);
                    val.ShouldNotBeNullOrEmpty();
                    var cacheVal = CacheManager.Get(KeyGenerator.Generate(DictionaryName, KeyName)) as string;
                    cacheVal.ShouldNotBeNullOrEmpty();
                    val.ShouldBe(cacheVal);
                }
            });
        }

        [Test]
        public void Get_Default_NullKey__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                using (var settings = CreateSettings())
                    settings.Get(null, null);
            });
        }

        [Test]
        public void Get_Default_EmptyKey__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                using (var settings = CreateSettings())
                    settings.Get("", null);
            });
        }

        [Test]
        public void Get_Default_WhitespaceKey__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                using (var settings = CreateSettings())
                    settings.Get("            ", null);
            });
        }

        [Test]
        public void Get_Default_NonExistingKey__ReturnDefault()
        {
            Should.NotThrow(() =>
            {
                using (var settings = CreateSettings())
                {
                    settings.ReturnValue = false;
                    settings.Get(KeyName, "def").ShouldNotBeNullOrEmpty();
                }
            });
        }

        [Test]
        public void Get_Default_NonExistingKey_UseCache__ReturnDefault()
        {
            Should.NotThrow(() =>
            {
                using (var settings = CreateSettings(true))
                {
                    var defaultValue = "def";
                    settings.ReturnValue = false;
                    var val = settings.Get(KeyName, defaultValue);
                    val.ShouldNotBeNullOrEmpty();
                    val.ShouldBe(defaultValue);
                }
                CacheManager.Get(KeyGenerator.Generate(DictionaryName, KeyName)).ShouldBeNull();
            });
        }

        [Test]
        public void Get_Default__GetValue()
        {
            Should.NotThrow(() =>
            {
                using (var settings = CreateSettings())
                {
                    var defaultValue = "def";
                    var val = settings.Get(KeyName, defaultValue);
                    val.ShouldNotBeNullOrEmpty();
                    val.ShouldNotBe(defaultValue);
                }
                CacheManager.Get(KeyGenerator.Generate(DictionaryName, KeyName)).ShouldBeNull();
            });
        }

        [Test]
        public void Get_Default_UseCache__GetValue()
        {
            Should.NotThrow(() =>
            {
                using (var settings = CreateSettings(true))
                {
                    var defaultValue = "def";
                    var val = settings.Get(KeyName, defaultValue);
                    val.ShouldNotBeNullOrEmpty();
                    val.ShouldNotBe(defaultValue);
                    var cacheVal = CacheManager.Get(KeyGenerator.Generate(DictionaryName, KeyName)) as string;
                    cacheVal.ShouldNotBeNullOrEmpty();
                    val.ShouldBe(cacheVal);
                }
            });
        }

        [Test]
        public void GetAll_Empty__GetEmptyDictionary()
        {
            Should.NotThrow(() =>
            {
                using (var settings = CreateSettings())
                {
                    settings.ReturnValue = false;
                    var val = settings.GetAll();
                    val.ShouldNotBeNull();
                    val.Count.ShouldBe(0);
                }
                CacheManager.Get(KeyGenerator.GenerateForGetAllDictionary(DictionaryName)).ShouldBeNull();
            });
        }

        [Test]
        public void GetAll_UseCache_Empty__GetEmptyDictionary()
        {
            Should.NotThrow(() =>
            {
                using (var settings = CreateSettings(true))
                {
                    settings.ReturnValue = false;
                    var val = settings.GetAll();
                    val.ShouldNotBeNull();
                    val.Count.ShouldBe(0);
                    var keys = CacheManager.Get(KeyGenerator.GenerateForGetAllDictionary(DictionaryName)) as List<string>;
                    keys.ShouldNotBeNull();
                    keys.Count.ShouldBe(0);
                }
            });
        }

        [Test]
        public void GetAll__GetDictionary()
        {
            Should.NotThrow(() =>
            {
                using (var settings = CreateSettings())
                {
                    var val = settings.GetAll();
                    val.ShouldNotBeNull();
                    val.Count.ShouldBe(10);
                }
                CacheManager.Get(KeyGenerator.GenerateForGetAllDictionary(DictionaryName)).ShouldBeNull();
            });
        }

        [Test]
        public void GetAll_UseCache__GetDictionary()
        {
            Should.NotThrow(() =>
            {
                using (var settings = CreateSettings(true))
                {
                    var val = settings.GetAll();
                    val.ShouldNotBeNull();
                    val.Count.ShouldBe(10);
                    List<string> keys = null;
                    try
                    {
                        keys = CacheManager.Get(KeyGenerator.GenerateForGetAllDictionary(DictionaryName)) as List<string>;
                        keys.ShouldNotBeNull();
                        keys.Count.ShouldBe(10);
                    }
                    finally
                    {
                        for (int i = 0; i < keys.Count; i++)
                            CacheManager.Remove(KeyGenerator.Generate(DictionaryName, i.ToString()));
                    }
                }
            });
        }

        [Test]
        public void GetAll_UseCache_CallAgain__GetDictionary()
        {
            Should.NotThrow(() =>
            {
                using (var settings = CreateSettings(true))
                {
                    var val = settings.GetAll();
                    val.ShouldNotBeNull();
                    val.Count.ShouldBe(10);
                    var valSecond = settings.GetAll();
                    valSecond.ShouldNotBeNull();
                    valSecond.Count.ShouldBe(10);
                    valSecond.ShouldBe(val);
                    List<string> keys = null;
                    try
                    {
                        keys = CacheManager.Get(KeyGenerator.GenerateForGetAllDictionary(DictionaryName)) as List<string>;
                        keys.ShouldNotBeNull();
                        keys.Count.ShouldBe(10);
                    }
                    finally
                    {
                        for (int i = 0; i < keys.Count; i++)
                            CacheManager.Remove(KeyGenerator.Generate(DictionaryName, i.ToString()));
                    }
                }
            });
        }

        private MockSettings CreateSettings(bool useCache = false)
        {
            var res = new MockSettings(DictionaryName);
            res.Init(useCache, 100);
            return res;
        }
    }
}
