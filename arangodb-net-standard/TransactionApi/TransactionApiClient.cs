using ArangoDBNetStandard.Serialization;
using ArangoDBNetStandard.TransactionApi.Models;
using ArangoDBNetStandard.Transport;
using System.Threading.Tasks;

namespace ArangoDBNetStandard.TransactionApi
{
    /// <summary>
    /// Provides access to ArangoDB transaction API.
    /// </summary>
    public class TransactionApiClient : ApiClientBase, ITransactionApiClient
    {
        /// <summary>
        /// The transport client used to communicate with the ArangoDB host.
        /// </summary>
        protected IApiClientTransport _client;

        /// <summary>
        /// The root path of the API.
        /// </summary>
        protected readonly string _transactionApiPath = "_api/transaction";

        /// <summary>
        /// Create an instance of <see cref="TransactionApiClient"/>
        /// using the provided transport layer and the default JSON serialization.
        /// </summary>
        /// <param name="client"></param>
        public TransactionApiClient(IApiClientTransport client)
            : base(new JsonNetApiClientSerialization())
        {
            _client = client;
        }

        /// <summary>
        /// Create an instance of <see cref="TransactionApiClient"/>
        /// using the provided transport and serialization layers.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="serializer"></param>
        public TransactionApiClient(IApiClientTransport client, IApiClientSerialization serializer)
            : base(serializer)
        {
            _client = client;
        }

        /// <summary>
        /// POST a js-transaction to ArangoDB.
        /// </summary>
        /// <remarks>
        /// https://www.arangodb.com/docs/stable/http/transaction-js-transaction.html
        /// </remarks>
        /// <typeparam name="T">Type to use for deserializing the object returned by the transaction function.</typeparam>
        /// <param name="body">Object containing information to submit in the POST transaction request.</param>
        /// <param name="serializationOptions">Optional serialization options.</param>
        /// <returns>Response from ArangoDB after processing the request.</returns>
        public virtual async Task<PostTransactionResponse<T>> PostTransactionAsync<T>(
            PostTransactionBody body,
            ApiClientSerializationOptions serializationOptions = null)
        {
            // Set default serialization options if not passed in
            serializationOptions = serializationOptions ??
                new ApiClientSerializationOptions(
                     useCamelCasePropertyNames: true,
                     ignoreNullValues: true,
                     useStringEnumConversion: true,
                     useSpecialDictionaryHandling: true);

            var content = GetContent(body, serializationOptions);
            using (var response = await _client.PostAsync(_transactionApiPath, content))
            {
                var stream = await response.Content.ReadAsStreamAsync();
                if (response.IsSuccessStatusCode)
                {
                    return DeserializeJsonFromStream<PostTransactionResponse<T>>(stream);
                }
                var error = DeserializeJsonFromStream<ApiErrorResponse>(stream);
                throw new ApiErrorException(error);
            }
        }
    }
}
