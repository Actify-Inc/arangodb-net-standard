﻿using System;
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
    /// <see cref="HttpApiTransport.Dispose"/> or using a <see cref="using"/> block.
    /// </remarks>
    public class HttpApiTransport : IApiClientTransport
    {
        private HttpClient _client;

        /// <summary>
        /// Create <see cref="HttpApiTransport"/> from an existing <see cref="HttpClient"/> instance.
        /// </summary>
        /// <param name="hostUri"></param>
        /// <param name="dbName"></param>
        /// <param name="username"></param>
        /// <param name="passwd"></param>
        public HttpApiTransport(HttpClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Create <see cref="HttpApiTransport"/> using basic auth.
        /// </summary>
        /// <param name="hostUri"></param>
        /// <param name="dbName"></param>
        /// <param name="username"></param>
        /// <param name="passwd"></param>
        public HttpApiTransport(
            Uri hostUri,
            string dbName,
            string username,
            string passwd)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(hostUri.AbsoluteUri + "/_db/" + dbName + "/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    Encoding.ASCII.GetBytes($"{username}:{passwd}")));

            _client = client;
        }

        /// <summary>
        /// Sends a DELETE request using <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public async Task<IApiClientResponse> DeleteAsync(string requestUri)
        {
            var response = await _client.DeleteAsync(requestUri);
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
            var response = await _client.SendAsync(request);
            return new HttpApiClientResponse(response);
        }

        /// <summary>
        /// Sends a POST request using <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<IApiClientResponse> PostAsync(string requestUri, byte[] content)
        {
            var httpContent = new ByteArrayContent(content);
            var response = await _client.PostAsync(requestUri, httpContent);
            return new HttpApiClientResponse(response);
        }

        /// <summary>
        /// Sends a PUT request using <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<IApiClientResponse> PutAsync(string requestUri, byte[] content)
        {
            var httpContent = new ByteArrayContent(content);
            var response = await _client.PutAsync(requestUri, httpContent);
            return new HttpApiClientResponse(response);
        }

        /// <summary>
        /// Sends a GET request using <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public async Task<IApiClientResponse> GetAsync(string requestUri)
        {
            var response = await _client.GetAsync(requestUri);
            return new HttpApiClientResponse(response);
        }

        /// <summary>
        /// Sends a PATCH request using <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<IApiClientResponse> PatchAsync(string requestUri, byte[] content)
        {
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = new ByteArrayContent(content)
            };
            var response = await _client.SendAsync(request);
            return new HttpApiClientResponse(response);
        }

        /// <summary>
        /// Send a HEAD request using <see cref="HttpClient"/>
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public async Task<IApiClientResponse> HeadAsync(string requestUri, WebHeaderCollection webHeaderCollection = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Head, requestUri);

            if (webHeaderCollection != null)
            {
                foreach (var key in webHeaderCollection.AllKeys)
                {
                    request.Headers.Add(key, webHeaderCollection[key]);
                }
            }
            var response = await _client.SendAsync(request);
            return new HttpApiClientResponse(response);
        }

        /// <summary>
        /// Disposes the underlying <see cref="HttpClient"/> instance.
        /// </summary>
        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
