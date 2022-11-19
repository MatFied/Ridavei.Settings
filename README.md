# Ridavei.Settings

### Latest release
[![NuGet Badge Ridavei.Settings](https://buildstats.info/nuget/Ridavei.Settings)](https://www.nuget.org/packages/Ridavei.Settings)

## What is Settings?

Ridavei.Settings is a cross-platform library created to ease getting and setting values in settings manager.\
The settings class implement the [IDisposable](https://learn.microsoft.com/pl-pl/dotnet/api/system.idisposable) interface to dispose objects that are created by the extensions.

## Examples in using Settings

### Get settings and then change and retrieve its values.
```csharp
using Ridavei.Settings;
using Ridavei.Settings.Base;

namespace TestProgram
{
    class Program
    {
        public static void Main(string[] args)
        {
            SettingsBuilder settingsBuilder = SettingsBuilder
                .CreateBuilder()
                .SetManager(YOUR_MANAGER_CLASS);
            using (ASettings settings = settingsBuilder.GetSettings("DictionaryName")
                /*you can use the GetOrCreateSettings method if you are not sure if the settings dictionary exists*/)
            {
                //You can use settings.Get("ExampleKey", "DefaultValue") if you want to retrieve the default value if the key doesn't exists.
                string value = settings.Get("ExampleKey");
                settings.Set("AnotherKey", "NewValue");
            }
        }
    }
}
```
### Get all keys with their values from settings.
```csharp
using System.Collections.Generic;

using Ridavei.Settings;
using Ridavei.Settings.Base;

namespace TestProgram
{
    class Program
    {
        public static void Main(string[] args)
        {
            SettingsBuilder settingsBuilder = SettingsBuilder
                .CreateBuilder()
                .SetManager(YOUR_MANAGER_CLASS);
            using (ASettings settings = settingsBuilder.GetSettings("DictionaryName")
                /*you can use the GetOrCreateSettings method if you are not sure if the settings dictionary exists*/)
            {
                //Returns the IReadOnlyDictionary to prevent from value changing.
                IReadOnlyDictionary<string, string> dict = settings.GetAll();
            }
        }
    }
}
```
### Changing a set of keys.
```csharp
using System.Collections.Generic;

using Ridavei.Settings;
using Ridavei.Settings.Base;

namespace TestProgram
{
    class Program
    {
        public static void Main(string[] args)
        {
            Dictionary<string, string> newValues = new Dictionary<string, string>();
            newValues.Add("NewKey1", "NewValue1");
            
            SettingsBuilder settingsBuilder = SettingsBuilder
                .CreateBuilder()
                .SetManager(YOUR_MANAGER_CLASS);
            using (ASettings settings = settingsBuilder.GetSettings("DictionaryName")
                /*you can use the GetOrCreateSettings method if you are not sure if the settings dictionary exists*/)
            {
                settings.Set(newValues);
            }
        }
    }
}
```

## Caching

For caching it uses [MemoryCache](https://learn.microsoft.com/pl-pl/dotnet/api/system.runtime.caching.memorycache).\
To use the cache for storing the settings values you can use the `EnableCache` method.
```csharp
builder.EnableCache();
```
You can also change the timeout for the cache (default is 15 minutes) by using `SetCacheTimeout` method.
```csharp
builder.SetCacheTimeout(VALUE_IN_MILLISECONDS);
```

## Example of creating extensions
```csharp
using System.Collections.Generic;

using Ridavei.Settings;
using Ridavei.Settings.Base;

namespace TestProgram
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SettingsBuilder settingsBuilder = SettingsBuilder
                .CreateBuilder()
                .UseExampleManager();
            using (ASettings settings = settingsBuilder.GetSettings("ExampleDictionary"))
            {
                //Operations on the settings
            }
        }
    }

    public static class Ext
    {
        public static SettingsBuilder UseExampleManager(this SettingsBuilder builder)
        {
            return builder.SetManager(new ExampleManager());
        }
    }

    public class ExampleManager : AManager
    {
        protected override ASettings CreateSettingsObject(string dictionaryName)
        {
            return new ExampleSettings(dictionaryName);
        }

        protected override bool TryGetSettingsObject(string dictionaryName, out ASettings settings)
        {
            settings = new ExampleSettings(dictionaryName);
            return true;
        }
    }
    
    public class ExampleSettings : ASettings
    {
        public ExampleSettings(string dictionaryName) : base(dictionaryName) { }

        protected override IReadOnlyDictionary<string, string> GetAllValues()
        {
            return new Dictionary<string, string>();
        }

        protected override bool TryGetValue(string key, out string value)
        {
            value = "Example";
            return true;
        }

        protected override void SetValue(string key, string value) { }
    }
}
```
