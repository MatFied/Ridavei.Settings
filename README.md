# Ridavei.Settings

## What is Settings?

Ridavei.Settings is a cross-platform library created to ease getting and setting values in settings manager.\
For caching it uses [MemoryCache](https://learn.microsoft.com/pl-pl/dotnet/api/system.runtime.caching.memorycache).\
The settings class implement the [IDisposable](https://learn.microsoft.com/pl-pl/dotnet/api/system.idisposable) interface to dispose objects that are created by the extensions.

## Examples in using Settings

### Get settings and then change and retrieve its values.
```csharp
using Ridavei.Settings;
using Ridavei.Settings.Interface;

namespace TestProgram
{
    class Program
    {
        public static void Main(string[] args)
        {
            SettingsBuilder settingsBuilder = SettingsBuilder
                .CreateBuilder()
                .SetManager(YOUR_MANAGER_CLASS);
            using (ASettings settings = settingsBuilder.GetSettings("DictionaryName"))
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
using Ridavei.Settings.Interface;

namespace TestProgram
{
    class Program
    {
        public static void Main(string[] args)
        {
            SettingsBuilder settingsBuilder = SettingsBuilder
                .CreateBuilder()
                .SetManager(YOUR_MANAGER_CLASS);
            using (ASettings settings = settingsBuilder.GetSettings("DictionaryName"))
            {
                //Returns the IReadOnlyDictionary to prevent from value changing.
                IReadOnlyDictionary<string, string> dict = settings.GetAll();
            }
        }
    }
}
```
```
### To use the cache for storing the settings values you can use use the 'EnableCache' method.
```csharp
builder.EnableCache();
```
