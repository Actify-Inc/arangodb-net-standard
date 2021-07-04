using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDBNetStandard.Transport.Http
{
    /// <summary>
    /// HTTP implementation for <see cref="IApiClientTransport"/>.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="HttpClient"/> under the hood. Note it is recommended to maintain a single
    /// instance of <see cref="HttpClient"/> per server host, for the lifetime of your application, without calling
    /// <see cref="HttpApiTransport.Dispose"/> or using a <c>using</c> block.
    /// </remarks>
    public class HttpApiTransport : IApiClientTransport
    {
        private readonly HttpClientConfig _clientConfig;

        /// <summary>
        /// Create <see cref="HttpApiTransport"/> from an existing <see cref="HttpClientConfig"/> instance.
        /// </summary>
        /// <param name="clientConfig">Existing HTTP client configuration instance.</param>
        /// <param name="contentType">Content type to use in requests.
        /// Used to set Content-Type and Accept HTTP headers.</param>
        public HttpApiTransport(HttpClientConfig clientConfig, HttpContentType contentType)
        {
            _clientConfig = clientConfig;
            _clientConfig.UseContentType(contentType);
        }

        /// <summary>
        /// Get an instance of <see cref="HttpApiTransport"/> that uses no authentication.
        /// </summary>
        /// <param name="hostUri"></param>
        /// <param name="dbName"></param>
        /// <param name="contentType">Content type to use in requests.
        /// Used to set Content-Type and Accept HTTP headers.
        /// Uses JSON content type by default.</param>
        /// <returns></returns>
        public static HttpApiTransport UsingNoAuth(
            Uri hostUri,
            string dbName,
            HttpContentType contentType = HttpContentType.Json)
        {
            var client = new HttpClientConfig();
            client.BaseAddress = new Uri(hostUri.AbsoluteUri + "_db/" + dbName + "/");

            var transport = new HttpApiTransport(client, contentType);
            return transport;
        }

        /// <summary>
        /// Get an instance of <see cref="HttpApiTransport"/> that uses basic auth.
        /// </summary>
        /// <param name="hostUri"></param>
        /// <param name="dbName"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="contentType">Content type to use in requests.
        /// Used to set Content-Type and Accept HTTP headers.
        /// Uses JSON content type by default.</param>
        /// <returns></returns>
        public static HttpApiTransport UsingBasicAuth(
            Uri hostUri,
            string dbName,
            string username,
            string password,
            HttpContentType contentType = HttpContentType.Json)
        {
            var client = new HttpClientConfig();
            client.BaseAddress = new Uri(hostUri.AbsoluteUri + "_db/" + dbName + "/");

            var transport = new HttpApiTransport(client, contentType);
            transport.SetBasicAuth(username, password);

            return transport;
        }

        /// <summary>
        /// Get an instance of <see cref="HttpApiTransport"/> that uses
        /// JWT-Token authentication.
        /// </summary>
        /// <param name="hostUri"></param>
        /// <param name="dbName"></param>
        /// <param name="jwtToken"></param>
        /// <param name="contentType">Content type to use in requests.
        /// Used to set Content-Type and Accept HTTP headers.
        /// Uses JSON content type by default.</param>
        /// <returns></returns>
        public static HttpApiTransport UsingJwtAuth(
            Uri hostUri,
            string dbName,
            string jwtToken,
            HttpContentType contentType = HttpContentType.Json)
        {
            var client = new HttpClientConfig();
            client.BaseAddress = new Uri(hostUri.AbsoluteUri + "_db/" + dbName + "/");

            var transport = new HttpApiTransport(client, contentType);
            transport.SetJwtToken(jwtToken);

            return transport;
        }

        /// <summary>
        /// Make this <see cref="HttpApiTransport"/> instance use JSON content type
        /// for Content-Type and Accept HTTP headers.
        /// </summary>
        public void UseJsonContentType()
        {
            _clientConfig.UseContentType(HttpContentType.Json);
        }

        /// <summary>
        /// Make this <see cref="HttpApiTransport"/> instance use VPack content type
        /// for Content-Type and Accept HTTP headers.
        /// </summary>
        public void UseVPackContentType()
        {
            _clientConfig.UseContentType(HttpContentType.VPack);
        }

        /// <summary>
        /// When using Basic auth, call this method to set the username and password
        /// used in requests to ArangoDB.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void SetBasicAuth(string username, string password)
        {
            _clientConfig.SetBasicAuth(username, password);
        }

        /// <summary>
        /// When using JWT authentication, call this method to refresh the JWT token
        /// used in requests to ArangoDB.
        /// </summary>
        /// <param name="jwt"></param>
        public void SetJwtToken(string jwt)
        {
            _clientConfig.SetJwtToken(jwt);
        }

        /// <summary>
        /// Sends a DELETE request using <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public async Task<IApiClientResponse> DeleteAsync(string requestUri)
        {
            var response = await _clientConfig.Build().DeleteAsync(requestUri);
            return new HttpApiClientResponse(response);
        }

        /// <summary>
        /// Send a DELETE request with body content using <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<IApiClientResponse> DeleteAsync(string requestUri, byte[] content)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, requestUri)
            {
                Content = new ByteArrayContent(content)
            };
            request.Content.Headers.ContentType = _clientConfig.ContentType;
            var response = await _clientConfig.Build().SendAsync(request);
            return new HttpApiClientResponse(response);
        }

        /// <summary>
        /// Sends a POST request using <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="content">The content of the request, must not be null.</param>
        /// <returns></returns>
        public async Task<IApiClientResponse> PostAsync(string requestUri, byte[] content)
        {
            var httpContent = new ByteArrayContent(content);
            httpContent.Headers.ContentType = _clientConfig.ContentType;
            var response = await _clientConfig.Build().PostAsync(requestUri, httpContent);
            return new HttpApiClientResponse(response);
        }

        /// <summary>
        /// Sends a PUT request using <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="content">The content of the request, must not be null.</param>
        /// <returns></returns>
        public async Task<IApiClientResponse> PutAsync(string requestUri, byte[] content)
        {
            var httpContent = new ByteArrayContent(content);
            httpContent.Headers.ContentType = _clientConfig.ContentType;
            var response = await _clientConfig.Build().PutAsync(requestUri, httpContent);
            return new HttpApiClientResponse(response);
        }

        /// <summary>
        /// Sends a GET request using <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="requestUri">The content of the request, must not be null.</param>
        /// <returns></returns>
        public async Task<IApiClientResponse> GetAsync(string requestUri)
        {
            var response = await _clientConfig.Build().GetAsync(requestUri);
            return new HttpApiClientResponse(response);
        }

        /// <summary>
        /// Sends a PATCH request using <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="content">The content of the request, must not be null.</param>
        /// <returns></returns>
        public async Task<IApiClientResponse> PatchAsync(string requestUri, byte[] content)
        {
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = new ByteArrayContent(content)
            };
            request.Content.Headers.ContentType = _clientConfig.ContentType;
            var response = await _clientConfig.Build().SendAsync(request);
            return new HttpApiClientResponse(response);
        }

        /// <summary>
        /// Send a HEAD request using <see cref="HttpClient"/>
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="webHeaderCollection"></param>
        /// <returns></returns>
        public async Task<IApiClientResponse> HeadAsync(
            string requestUri,
            WebHeaderCollection webHeaderCollection = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Head, requestUri);

            if (webHeaderCollection != null)
            {
                foreach (var key in webHeaderCollection.AllKeys)
                {
                    request.Headers.Add(key, webHeaderCollection[key]);
                }
            }
            var response = await _clientConfig.Build().SendAsync(request);
            return new HttpApiClientResponse(response);
        }

        /// <summary>
        /// The HttpClient in use is now shared, no disposing of it is needed.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
