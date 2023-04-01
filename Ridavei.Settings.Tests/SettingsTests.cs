using System;
using System.Collections.Generic;

using Ridavei.Settings.Cache;
using Ridavei.Settings.Internals;

using Ridavei.Settings.Tests.Settings;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Shouldly;

namespace Ridavei.Settings.Tests
{
    [TestFixture]
    internal class SettingsTests
    {
        private const string DictionaryName = "TestDict";
        private const string KeyName = "TestKey";

        private CacheManager _cacheManager = new CacheManager(new MemoryDistributedCache(Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions())), Consts.DefaultCacheTimeout);

        [TearDown]
        public void TearDown()
        {
            _cacheManager.Remove(KeyGenerator.Generate(DictionaryName, KeyName));
            _cacheManager.Remove(KeyGenerator.GenerateForGetAllDictionary(DictionaryName));
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
                    settings.CacheManager.ShouldBeNull();
                }
            });
        }

        [Test]
        public void Init__SetValues()
        {
            Should.NotThrow(() =>
            {
                using (var settings = new MockSettings(DictionaryName))
                {
                    settings.Init(_cacheManager);

                    settings.DictionaryName.ShouldBe(DictionaryName);
                    settings.CacheManager.ShouldNotBeNull();
                }
            });
        }

        [Test]
        public void Init_RunMultiple__SetValuesOnlyOnTheFirstCall()
        {
            Should.NotThrow(() =>
            {
                using (var settings = new MockSettings(DictionaryName))
                {
                    settings.Init(_cacheManager);
                    for (int i = 0; i < 10; i++)
                        settings.Init(null);

                    settings.DictionaryName.ShouldBe(DictionaryName);
                    settings.CacheManager.ShouldNotBeNull();
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
        public void Set_Dictionary_NullDictionary__RaisesException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                using (var settings = CreateSettings())
                    settings.Set(null);
            });
        }

        [Test]
        public void Set_Dictionary_EmptyDictionary__NoException()
        {
            Should.NotThrow(() =>
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                using (var settings = CreateSettings())
                    settings.Set(dict);
            });
        }

        [Test]
        public void Set_Dictionary__NoException()
        {
            Should.NotThrow(() =>
            {
                Dictionary<string, string> dict = new Dictionary<string, string>
                {
                    { "1", "1" }
                };
                using (var settings = CreateSettings())
                    settings.Set(dict);
            });
        }

        [Test]
        public void Set_Dictionary_UseCache_EmptyDictionary__NoException()
        {
            Should.NotThrow(() =>
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                using (var settings = CreateSettings(true))
                    settings.Set(dict);
            });
        }

        [Test]
        public void Set_Dictionary_UseCache__NoException()
        {
            Should.NotThrow(() =>
            {
                Dictionary<string, string> dict = new Dictionary<string, string>
                {
                    { "1", "1" }
                };
                using (var settings = CreateSettings(true))
                    settings.Set(dict);
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
                _cacheManager.GetString(KeyGenerator.Generate(DictionaryName, KeyName)).ShouldBeNull();
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
                    for (int i = 0; i < 10; i++)
                    {
                        var cacheVal = settings.Get(KeyName);
                        cacheVal.ShouldNotBeNullOrEmpty();
                        val.ShouldBe(cacheVal);
                    }
                }
            });
        }

        [Test]
        public void Get_UseCache_ChangeVal__GetTheSameValue()
        {
            Should.NotThrow(() =>
            {
                using (var settings = CreateSettings(true))
                {
                    var val = settings.Get(KeyName);
                    val.ShouldNotBeNullOrEmpty();
                    for (int i = 0; i < 10; i++)
                    {
                        var cacheVal = settings.Get(KeyName);
                        cacheVal.ShouldNotBeNullOrEmpty();
                        val.ShouldBe(cacheVal);
                        cacheVal += i.ToString();
                        var cacheVal2 = settings.Get(KeyName);
                        cacheVal2.ShouldNotBeNullOrEmpty();
                        val.ShouldBe(cacheVal2);
                        val.ShouldNotBe(cacheVal);
                    }
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
                _cacheManager.GetString(KeyGenerator.Generate(DictionaryName, KeyName)).ShouldBeNull();
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
                _cacheManager.GetString(KeyGenerator.Generate(DictionaryName, KeyName)).ShouldBeNull();
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
                    for (int i = 0; i < 10; i++)
                    {
                        var cacheVal = settings.Get(KeyName, defaultValue);
                        cacheVal.ShouldNotBeNullOrEmpty();
                        val.ShouldBe(cacheVal);
                    }
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
                _cacheManager.GetString(KeyGenerator.GenerateForGetAllDictionary(DictionaryName)).ShouldBeNull();
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
                _cacheManager.GetString(KeyGenerator.GenerateForGetAllDictionary(DictionaryName)).ShouldBeNull();
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
                    try
                    {
                        val.ShouldNotBeNull();
                        val.Count.ShouldBe(10);
                    }
                    finally
                    {
                        foreach (var kvp in val)
                            _cacheManager.Remove(KeyGenerator.Generate(DictionaryName, kvp.Key));
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
                    try
                    {
                        val.ShouldNotBeNull();
                        val.Count.ShouldBe(10);
                        var valSecond = settings.GetAll();
                        valSecond.ShouldNotBeNull();
                        valSecond.Count.ShouldBe(10);
                        valSecond.ShouldBe(val);
                    }
                    finally
                    {
                        foreach (var kvp in val)
                            _cacheManager.Remove(KeyGenerator.Generate(DictionaryName, kvp.Key));
                    }
                }
            });
        }

        private MockSettings CreateSettings(bool useCache = false)
        {
            var res = new MockSettings(DictionaryName);
            res.Init(useCache ? _cacheManager : null);
            return res;
        }
    }
}
