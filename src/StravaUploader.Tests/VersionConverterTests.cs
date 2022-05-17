using NUnit.Framework;
using System;
using System.Text.Json;

namespace StravaUploader.Tests
{
    public class VersionConverterTests
    {
        [TestCase]
        public void ConvertsVersion_BothWays()
        {
            var version = new Version("1.0.0.0");
            
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Converters =
                {
                    new VersionConverter()
                }
            };

            var json = JsonSerializer.Serialize(version);
            var convertedJson = JsonSerializer.Deserialize<Version>(json, options);

            Assert.AreEqual(convertedJson, version);
        }
    }
}