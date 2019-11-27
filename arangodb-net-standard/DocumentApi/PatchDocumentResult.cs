﻿namespace ArangoDBNetStandard.DocumentApi
{
    public class PatchDocumentResult<U>
    {
        public U New { get; set; }

        public U Old { get; set; }

        public string _key { get; set; }

        public string _rev { get; set; }

        public string _oldRev { get; set; }

        public string _id { get; set; }
    }
}
