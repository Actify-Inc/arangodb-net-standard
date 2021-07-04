using System.Net.Http;
using ArangoDBNetStandard.AqlFunctionApi;
using ArangoDBNetStandard.AuthApi;
using ArangoDBNetStandard.CollectionApi;
using ArangoDBNetStandard.CursorApi;
using ArangoDBNetStandard.DatabaseApi;
using ArangoDBNetStandard.DocumentApi;
using ArangoDBNetStandard.GraphApi;
using ArangoDBNetStandard.Serialization;
using ArangoDBNetStandard.TransactionApi;
using ArangoDBNetStandard.Transport;
using ArangoDBNetStandard.Transport.Http;
using ArangoDBNetStandard.UserApi;

namespace ArangoDBNetStandard
{
    /// <summary>
    /// Wrapper class providing access to the complete set of ArangoDB REST resources.
    /// </summary>
    public class ArangoDBClient : IArangoDBClient
    {
        /// <summary>
        /// The transport client used to communicate with the ArangoDB host.
        /// </summary>
        protected IApiClientTransport _transport;

        /// <summary>
        /// AQL user functions management API.
        /// </summary>
        public AqlFunctionApiClient AqlFunction { get; private set; }

        /// <summary>
        /// Auth API
        /// </summary>
        public AuthApiClient Auth { get; private set; }

        /// <summary>
        /// Cursor API
        /// </summary>
        public CursorApiClient Cursor { get; private set; }

        /// <summary>
        /// Database API
        /// </summary>
        public DatabaseApiClient Database { get; private set; }

        /// <summary>
        /// Document API
        /// </summary>
        public DocumentApiClient Document { get; private set; }

        /// <summary>
        /// Collection API
        /// </summary>
        public CollectionApiClient Collection { get; private set; }

        /// <summary>
        /// Transaction API
        /// </summary>
        public TransactionApiClient Transaction { get; private set; }

        /// <summary>
        /// Graph API
        /// </summary>
        public GraphApiClient Graph { get; private set; }

        /// <summary>
        /// User management API.
        /// </summary>
        public UserApiClient User { get; private set; }

        /// <summary>
        /// Create an instance of <see cref="ArangoDBClient"/> from an existing
        /// <see cref="HttpClientConfig"/> instance, using the default JSON serialization.
        /// </summary>
        /// <param name="clientConfig"></param>
        public ArangoDBClient(HttpClientConfig clientConfig)
        {
            _transport = new HttpApiTransport(clientConfig, HttpContentType.Json);

            var serialization = new JsonNetApiClientSerialization();

            InitializeApis(_transport, serialization);
        }

        /// <summary>
        /// Create an instance of <see cref="ArangoDBClient"/>
        /// using the provided transport layer and the default JSON serialization.
        /// </summary>
        /// <param name="transport">The ArangoDB transport layer implementation.</param>
        public ArangoDBClient(IApiClientTransport transport)
        {
            _transport = transport;

            var serialization = new JsonNetApiClientSerialization();

            InitializeApis(_transport, serialization);
        }

        /// <summary>
        /// Create an instance of <see cref="ArangoDBClient"/>
        /// using the provided transport and serialization layers.
        /// </summary>
        /// <param name="transport">The ArangoDB transport layer implementation.</param>
        /// <param name="serialization">The serialization layer implementation.</param>
        public ArangoDBClient(IApiClientTransport transport, IApiClientSerialization serialization)
        {
            _transport = transport;

            InitializeApis(_transport, serialization);
        }

        /// <summary>
        /// Disposes the underlying transport instance.
        /// </summary>
        public void Dispose()
        {
            _transport.Dispose();
        }

        private void InitializeApis(
            IApiClientTransport transport,
            IApiClientSerialization serialization)
        {
            AqlFunction = new AqlFunctionApiClient(transport, serialization);
            Auth = new AuthApiClient(transport, serialization);
            Cursor = new CursorApiClient(transport, serialization);
            Database = new DatabaseApiClient(transport, serialization);
            Document = new DocumentApiClient(transport, serialization);
            Collection = new CollectionApiClient(transport, serialization);
            Transaction = new TransactionApiClient(transport, serialization);
            Graph = new GraphApiClient(transport, serialization);
            User = new UserApiClient(transport, serialization);
        }
    }
}
