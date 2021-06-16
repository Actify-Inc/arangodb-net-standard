namespace ArangoDBNetStandard.Serialization
{
    /// <summary>
    /// The API client serialization options class.
    /// </summary>
    public class ApiClientSerializationOptions
    {
        /// <summary>
        /// Use camel case if true, otherwise depends on
        /// the action will be implemented in the serializer.
        /// </summary>
        public bool UseCamelCasePropertyNames { get; set; }
        
        /// <summary>
        /// True to ignore null values, otherwise false.
        /// </summary>
        public bool IgnoreNullValues { get; set; }

        /// <summary>
        /// True to serialize enums to string values, 
        /// false to serialize enums to integer values (default).
        /// </summary>
        public bool UseStringEnumConversion { get; set; }

        /// <summary>
        /// When enabled will never use camel case property names, 
        /// will always include null values, for dictionary types only. 
        /// This applies only to types assignable to <see cref="System.Collections.IDictionary" />.
        /// </summary>
        public bool UseSpecialDictionaryHandling { get; set; }

        /// <summary>
        /// Create serialization options.
        /// </summary>
        /// <param name="useCamelCasePropertyNames">
        /// Whether property names should be serialized using camelCase.
        /// </param>
        /// <param name="ignoreNullValues">
        /// Whether null values should be ignored - i.e. not defined at all in the serialized string.
        /// </param>
        /// <param name="useStringEnumConversion">
        /// Whether to serialize enum values to a string value instead of an integer.
        /// </param>
        /// <param name="useSpecialDictionaryHandling">
        /// Whether to use the special dictionary handling,
        /// which overrides useCamelCasePropertyNames and ignoreNullValues for dictionary types only.
        /// </param>
        public ApiClientSerializationOptions(
            bool useCamelCasePropertyNames,
            bool ignoreNullValues,
            bool useStringEnumConversion = false,
            bool useSpecialDictionaryHandling = false)
        {
            UseCamelCasePropertyNames = useCamelCasePropertyNames;
            IgnoreNullValues = ignoreNullValues;
            UseStringEnumConversion = useStringEnumConversion;
            UseSpecialDictionaryHandling = useSpecialDictionaryHandling;
        }
    }
}
