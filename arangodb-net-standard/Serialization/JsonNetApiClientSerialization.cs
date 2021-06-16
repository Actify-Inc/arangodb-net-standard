using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Text;

namespace ArangoDBNetStandard.Serialization
{
    /// <summary>
    /// Implements a <see cref="IApiClientSerialization"/> that uses Json.NET.
    /// </summary>
    public class JsonNetApiClientSerialization : ApiClientSerialization
    {
        /// <summary>
        /// Deserializes the JSON structure contained by the specified stream
        /// into an instance of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="stream">The stream containing the JSON structure to deserialize.</param>
        /// <returns></returns>
        public override T DeserializeFromStream<T>(Stream stream)
        {
            if (stream == null || stream.CanRead == false)
            {
                return default(T);
            }

            using (var sr = new StreamReader(stream))
            using (var jtr = new JsonTextReader(sr))
            {
                var js = new JsonSerializer();

                T result = js.Deserialize<T>(jtr);

                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="serializationOptions">The serialization options. When the value is null the
        /// the serialization options should be provided by the serializer, otherwise the given options should be used.</param>
        /// <returns></returns>

        public override byte[] Serialize<T>(T item, ApiClientSerializationOptions serializationOptions)
        {
            // When no options passed use the default.
            if (serializationOptions == null)
            {
                serializationOptions = DefaultOptions;
            }

            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = serializationOptions.IgnoreNullValues ?
                    NullValueHandling.Ignore :
                    NullValueHandling.Include
            };

            if (serializationOptions.UseStringEnumConversion)
            {
                var stringEnumConverter = new StringEnumConverter();
                jsonSettings.Converters.Add(stringEnumConverter);
            }

            if (serializationOptions.UseCamelCasePropertyNames)
            {
                jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }

            if (serializationOptions.UseSpecialDictionaryHandling)
            {
                var dictionaryValueConverter = new DictionaryValueConverter(serializationOptions.UseStringEnumConversion);
                jsonSettings.Converters.Add(dictionaryValueConverter);
            }

            string json = JsonConvert.SerializeObject(item, jsonSettings);

            return Encoding.UTF8.GetBytes(json);
        }
    }
}
