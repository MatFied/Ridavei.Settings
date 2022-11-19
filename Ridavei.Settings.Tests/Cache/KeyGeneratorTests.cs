using Ridavei.Settings.Cache;

using NUnit.Framework;
using Shouldly;

namespace Ridavei.Settings.Tests.Cache
{
    [TestFixture]
    public class KeyGeneratorTests
    {
        [TestCase(null, null, "")]
        [TestCase(null, "", "")]
        [TestCase(null, "        ", "")]
        [TestCase("", null, "")]
        [TestCase("", "", "")]
        [TestCase("", "       ", "")]
        [TestCase("         ", null, "")]
        [TestCase("         ", "", "")]
        [TestCase("         ", "       ", "")]
        [TestCase("TestDict", null, "")]
        [TestCase("TestDict", "", "")]
        [TestCase("TestDict", "          ", "")]
        [TestCase(null, "TestKey", "")]
        [TestCase("", "TestKey", "")]
        [TestCase("        ", "TestKey", "")]
        [TestCase("TestDict", "TestKey", "Dict_TestDict__TestKey")]
        public void Generate__ReturnsString(string dictionaryName, string key, string expectedValue)
        {
            Should.NotThrow(() =>
            {
                KeyGenerator.Generate(dictionaryName, key).ShouldBe(expectedValue);
            });
        }

        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase("      ", "")]
        [TestCase("TestDict", "Dict_TestDict_GetAllDictionary")]
        public void GenerateForGetAllDictionary__ReturnsString(string dictionaryName, string expectedValue)
        {
            Should.NotThrow(() =>
            {
                KeyGenerator.GenerateForGetAllDictionary(dictionaryName).ShouldBe(expectedValue);
            });
        }

        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase("      ", "")]
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
