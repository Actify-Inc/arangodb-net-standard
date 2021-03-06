﻿using System.Net;

namespace ArangoDBNetStandard.GraphApi.Models
{
    public class PostVertexResponse<T>
    {
        /// <summary>
        /// The complete newly written vertex document. 
        /// Includes all written attributes in the request 
        /// body and all internal attributes generated by ArangoDB. 
        /// Will only be present if 
        /// <see cref="PostVertexQuery.ReturnNew"/> is true.
        /// </summary>
        public  T New { get; set; }

        public PostVertexResult Vertex { get; set; }

        public HttpStatusCode Code { get; set; }

        public bool Error { get; set; }
    }
}
