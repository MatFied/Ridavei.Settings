using Ridavei.Settings.Cache;

using NUnit.Framework;
using Shouldly;

namespace Ridavei.Settings.Tests.Cache
{
    [TestFixture]
    public class KeyGeneratorTests
    {
        [TestCase(null, null, "Dict___")]
        [TestCase("TestDict", null, "Dict_TestDict__")]
        [TestCase(null, "TestKey", "Dict___TestKey")]
        [TestCase("TestDict", "TestKey", "Dict_TestDict__TestKey")]
        public void Generate__ReturnsString(string dictionaryName, string key, string expectedValue)
        {
            Should.NotThrow(() =>
            {
                KeyGenerator.Generate(dictionaryName, key).ShouldBe(expectedValue);
            });
        }

        [TestCase(null, "Dict__GetAllDictionary")]
        [TestCase("TestDict", "Dict_TestDict_GetAllDictionary")]
        public void GenerateForGetAllDictionary__ReturnsString(string dictionaryName, string expectedValue)
        {
            Should.NotThrow(() =>
            {
                KeyGenerator.GenerateForGetAllDictionary(dictionaryName).ShouldBe(expectedValue);
            });
        }

        [TestCase(null, "Dict_")]
        [TestCase("TestDict", "Dict_TestDict")]
        public void GenerateForDictionary__ReturnsString(string dictionaryName, string expectedValue)
        {
            Should.NotThrow(() =>
            {
                KeyGenerator.GenerateForDictionary(dictionaryName).ShouldBe(expectedValue);
            });
        }
    }
}
