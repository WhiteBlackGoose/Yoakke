﻿using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.TextSynchronization
{
    public class DidSaveTextDocumentParams
    {
        /// <summary>
        /// The document that was saved.
        /// </summary>
        [JsonProperty("textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }
        /// <summary>
        /// Optional the content when saved. Depends on the includeText value
        /// when the save notification was requested.
        /// </summary>
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string? Text { get; set; }
    }
}
