using ArangoDBNetStandard.Serialization;
using ArangoDBNetStandard.TransactionApi.Models;
using System.Threading.Tasks;

namespace ArangoDBNetStandard.TransactionApi
{
    /// <summary>
    /// Defines a client to access the ArangoDB Transactions API.
    /// </summary>
    public interface ITransactionApiClient
    {
        /// <summary>
        /// POST a transaction to ArangoDB.
        /// </summary>
        /// <typeparam name="T">Type to use for deserializing the object returned by the transaction function.</typeparam>
        /// <param name="body">Object containing information to submit in the POST transaction request.</param>
        /// <param name="serializationOptions">Optional serialization options.</param>
        /// <returns>Response from ArangoDB after processing the request.</returns>
        Task<PostTransactionResponse<T>> PostTransactionAsync<T>(
            PostTransactionBody body,
            ApiClientSerializationOptions serializationOptions = null);
    }
}
