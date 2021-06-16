using ArangoDBNetStandard.CursorApi.Models;
using ArangoDBNetStandard.Serialization;
using ArangoDBNetStandard.TransactionApi.Models;
using ArangoDBNetStandardTest.Serialization.Models;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace ArangoDBNetStandardTest.Serialization
{
    public class JsonNetApiClientSerializationTest
    {

        [Fact]
        public void Serialize_ShouldSucceed()
        {
            var model = new TestModel()
            {
                NullPropertyToIgnore = null,
                PropertyToCamelCase = "myvalue",
                EnumToConvertToString = TestModel.Number.Two
            };

            var serialization = new JsonNetApiClientSerialization();

            // Perform serialize with camel case option
            byte[] jsonBytesWithCamelCase = serialization.Serialize(model, 
                new ApiClientSerializationOptions(true, true, true));
            string jsonStringWithCamelCase = Encoding.UTF8.GetString(jsonBytesWithCamelCase);

            // Perform serialize without camel case option
            byte[] jsonBytesWithoutCamelCase = serialization.Serialize(model,
                new ApiClientSerializationOptions(false, true, true));
            string jsonStringWithoutCamelCase = Encoding.UTF8.GetString(jsonBytesWithoutCamelCase);

            // standard property with and without camelCase
            Assert.Contains("propertyToCamelCase", jsonStringWithCamelCase);
            Assert.Contains("PropertyToCamelCase", jsonStringWithoutCamelCase);

            // Null property should be ignored in both cases
            // (ignore case is important to make sure we don't miss the string 
            // if it is using different casing than we checked for)
            Assert.DoesNotContain(
                "nullPropertyToIgnore",
                jsonStringWithCamelCase,
                System.StringComparison.OrdinalIgnoreCase);

            Assert.DoesNotContain("nullPropertyToIgnore", 
                jsonStringWithoutCamelCase,
                System.StringComparison.OrdinalIgnoreCase);

            // We expect enum conversion to string, as well as camelCase
            Assert.Contains("enumToConvertToString", jsonStringWithCamelCase);

            // We expect enum conversion to string, but not camelCase
            Assert.Contains("EnumToConvertToString", jsonStringWithoutCamelCase);
        }

        [Fact]
        public void Serialize_ShouldSucceed_WhenUsingDefaultOptions()
        {
            var model = new TestModel()
            {
                NullPropertyToIgnore = null,
                PropertyToCamelCase = "myvalue",
                EnumToConvertToString = TestModel.Number.Two
            };
            var serialization = new JsonNetApiClientSerialization();

            // Perform serialize with default options 
            // i.e. camelCase: false, ignoreNull: true, stringEnum: false
            byte[] jsonBytes = serialization.Serialize(model, null);
            string jsonString = Encoding.UTF8.GetString(jsonBytes);

            Assert.Contains("PropertyToCamelCase", jsonString);
            Assert.DoesNotContain(
                "NullPropetyToIgnore",
                jsonString,
                System.StringComparison.OrdinalIgnoreCase);
            Assert.Contains("EnumToConvertToString", jsonString);
        }

        [Fact]
        public void Serialize_ShouldNotCamelCaseBindVars_WhenSerializingPostCursorBody()
        {
            var body = new PostCursorBody
            {
                BindVars = new Dictionary<string, object>
                {
                    ["DontCamelCaseKey"] = new { DontCamelCaseMe = true }
                }
            };
            var serialization = new JsonNetApiClientSerialization();

            byte[] jsonBytes = serialization.Serialize(
                body,
                new ApiClientSerializationOptions(
                     useCamelCasePropertyNames: true,
                     ignoreNullValues: true,
                     useStringEnumConversion: false,
                     useSpecialDictionaryHandling: true));

            string jsonString = Encoding.UTF8.GetString(jsonBytes);

            Assert.Contains("DontCamelCaseMe", jsonString);
            Assert.Contains("DontCamelCaseKey", jsonString);
            Assert.DoesNotContain("dontCamelCaseMe", jsonString);
            Assert.DoesNotContain("dontCamelCaseKey", jsonString);
        }

        [Fact]
        public void Serialize_ShouldNotCamelCaseParams_WhenSerializingPostTransactionBody()
        {
            var body = new PostTransactionBody
            {
                Params = new Dictionary<string, object>
                {
                    ["DontCamelCaseKey"] = new { DontCamelCaseMe = true }
                }
            };

            var serialization = new JsonNetApiClientSerialization();

            byte[] jsonBytes = serialization.Serialize(
                body,
                new ApiClientSerializationOptions(
                     useCamelCasePropertyNames: true,
                     ignoreNullValues: true,
                     useStringEnumConversion: false,
                     useSpecialDictionaryHandling: true));

            string jsonString = Encoding.UTF8.GetString(jsonBytes);

            Assert.Contains("DontCamelCaseMe", jsonString);
            Assert.Contains("DontCamelCaseKey", jsonString);
            Assert.DoesNotContain("dontCamelCaseMe", jsonString);
            Assert.DoesNotContain("dontCamelCaseKey", jsonString);
        }


        [Fact]
        public void Serialize_ShouldNotIgnoreNull_WhenSerializingPostCursorBody()
        {
            var body = new PostCursorBody
            {
                BindVars = new Dictionary<string, object>
                {
                    ["DontCamelCaseKey"] = null
                }
            };
            var serialization = new JsonNetApiClientSerialization();

            byte[] jsonBytes = serialization.Serialize(body, 
                new ApiClientSerializationOptions(
                     useCamelCasePropertyNames: true,
                     ignoreNullValues: true,
                     useStringEnumConversion: false,
                     useSpecialDictionaryHandling: true));

            string jsonString = Encoding.UTF8.GetString(jsonBytes);

            Assert.Contains("DontCamelCaseKey", jsonString);
        }


        [Fact]
        public void Serialize_ShouldNotIgnoreNull_WhenSerializingPostTransactionBody()
        {
            var body = new PostTransactionBody
            {
                Params = new Dictionary<string, object>
                {
                    ["DontCamelCaseKey"] = null
                }
            };

            var serialization = new JsonNetApiClientSerialization();

            byte[] jsonBytes = serialization.Serialize(body,
                new ApiClientSerializationOptions(
                     useCamelCasePropertyNames: true,
                     ignoreNullValues: true,
                     useStringEnumConversion: false,
                     useSpecialDictionaryHandling: true));

            string jsonString = Encoding.UTF8.GetString(jsonBytes);

            Assert.Contains("DontCamelCaseKey", jsonString);
        }

        [Fact]
        public void DeserializeFromStream_ShouldSucceed()
        {
            // Deserializing should match both "camelCase" and "CamelCase"

            byte[] jsonBytes = Encoding.UTF8.GetBytes(
                "{\"propertyToCamelCase\":\"myvalue\",\"NullPropertyToIgnore\":\"something\"}");

            var stream = new MemoryStream(jsonBytes);

            var serialization = new JsonNetApiClientSerialization();

            TestModel model = serialization.DeserializeFromStream<TestModel>(stream);

            Assert.Equal("myvalue", model.PropertyToCamelCase);
            Assert.Equal("something", model.NullPropertyToIgnore);
        }
    }
}
