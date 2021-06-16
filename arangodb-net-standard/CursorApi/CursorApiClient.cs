using ArangoDBNetStandard.CursorApi.Models;
using ArangoDBNetStandard.Serialization;
using ArangoDBNetStandard.Transport;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ArangoDBNetStandard.CursorApi
{
    /// <summary>
    /// ArangoDB Cursor API.
    /// </summary>
    public class CursorApiClient : ApiClientBase, ICursorApiClient
    {
        /// <summary>
        /// The root path of the API.
        /// </summary>
        protected readonly string _cursorApiPath = "_api/cursor";

        /// <summary>
        /// The transport client used to communicate with the ArangoDB host.
        /// </summary>
        protected IApiClientTransport _client;

        /// <summary>
        /// Creates an instance of <see cref="CursorApiClient"/>
        /// using the provided transport layer and the default JSON serialization.
        /// </summary>
        /// <param name="client"></param>
        public CursorApiClient(IApiClientTransport client)
            : base(new JsonNetApiClientSerialization())
        {
            _client = client;
        }

        /// <summary>
        /// Creates an instance of <see cref="CursorApiClient"/>
        /// using the provided transport and serialization layers.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="serializer"></param>
        public CursorApiClient(IApiClientTransport client, IApiClientSerialization serializer)
            : base(serializer)
        {
            _client = client;
        }

        /// <summary>
        /// Execute an AQL query, creating a cursor which can be used to page query results.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="bindVars"></param>
        /// <param name="options"></param>
        /// <param name="count"></param>
        /// <param name="batchSize"></param>
        /// <param name="cache"></param>
        /// <param name="memoryLimit"></param>
        /// <param name="ttl"></param>
        /// <param name="serializationOptions">Optional serialization options.</param>
        /// <returns></returns>
        public virtual async Task<CursorResponse<T>> PostCursorAsync<T>(
                string query,
                Dictionary<string, object> bindVars = null,
                PostCursorOptions options = null,
                bool? count = null,
                long? batchSize = null,
                bool? cache = null,
                long? memoryLimit = null,
                int? ttl = null,
                ApiClientSerializationOptions serializationOptions = null)
        {
            return await PostCursorAsync<T>(new PostCursorBody
            {
                Query = query,
                BindVars = bindVars,
                Options = options,
                Count = count,
                BatchSize = batchSize,
                Cache = cache,
                MemoryLimit = memoryLimit,
                Ttl = ttl,
            }, serializationOptions);
        }

        /// <summary>
        /// Execute an AQL query, creating a cursor which can be used to page query results.
        /// </summary>
        /// <param name="postCursorBody">Object encapsulating options and parameters of the query.</param>
        /// <param name="serializationOptions">Optional serialization options.</param>
        /// <returns></returns>
        public virtual async Task<CursorResponse<T>> PostCursorAsync<T>(
            PostCursorBody postCursorBody,
            ApiClientSerializationOptions serializationOptions = null)
        {
            // set default serialization options for this operation
            serializationOptions = serializationOptions ??
                new ApiClientSerializationOptions(
                     useCamelCasePropertyNames: true, 
                     ignoreNullValues: true,
                     useStringEnumConversion: true,
                     useSpecialDictionaryHandling: true);

            var content = GetContent(postCursorBody, serializationOptions);
            using (var response = await _client.PostAsync(_cursorApiPath, content))
            {
                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    return DeserializeJsonFromStream<CursorResponse<T>>(stream);
                }
                throw await GetApiErrorException(response);
            }
        }

        /// <summary>
        /// Deletes an existing cursor and frees the resources associated with it.
        /// DELETE /_api/cursor/{cursor-identifier}
        /// </summary>
        /// <param name="cursorId">The id of the cursor to delete.</param>
        /// <returns></returns>
        public virtual async Task<DeleteCursorResponse> DeleteCursorAsync(string cursorId)
        {
            using (var response = await _client.DeleteAsync(_cursorApiPath + "/" + WebUtility.UrlEncode(cursorId)))
            {
                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    return DeserializeJsonFromStream<DeleteCursorResponse>(stream);
                }
                throw await GetApiErrorException(response);
            }
        }

        /// <summary>
        /// Advances an existing query cursor and gets the next set of results.
        /// </summary>
        /// <typeparam name="T">Result type to deserialize to</typeparam>
        /// <param name="cursorId">ID of the existing query cursor.</param>
        /// <returns></returns>
        public virtual async Task<PutCursorResponse<T>> PutCursorAsync<T>(string cursorId)
        {
            string uri = _cursorApiPath + "/" + WebUtility.UrlEncode(cursorId);
            using (var response = await _client.PutAsync(uri, new byte[0]))
            {
                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    return DeserializeJsonFromStream<PutCursorResponse<T>>(stream);
                }
                throw await GetApiErrorException(response);
            }
        }
    }
}
