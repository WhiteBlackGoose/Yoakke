﻿using Newtonsoft.Json;
using System.Collections.Generic;
using Yoakke.Lsp.Model.LanguageFeatures;

namespace Yoakke.Lsp.Model.Capabilities.Client.TextDocument
{
    public class CodeActionClientCapabilities
    {
        public class CodeActionLiteralSupportCapabilities
        {
            public class CodeActionKindCapabilities
            {
                /// <summary>
                /// The code action kind values the client supports. When this
                /// property exists the client also guarantees that it will
                /// handle values outside its set gracefully and falls back
                /// to a default value when unknown.
                /// </summary>
                [JsonProperty("valueSet")]
                public IReadOnlyList<CodeActionKind> ValueSet { get; set; }
            }

            /// <summary>
            /// The code action kind is supported with the following value
            /// set.
            /// </summary>
            [JsonProperty("codeActionKind")]
            public CodeActionKindCapabilities CodeActionKind { get; set; }
        }

        public class ResolveSupportCapabilities
        {
            /// <summary>
            /// The properties that a client can resolve lazily.
            /// </summary>
            [JsonProperty("properties")]
            public IReadOnlyList<string> Properties { get; set; }
        }

        /// <summary>
        /// Whether code action supports dynamic registration.
        /// </summary>
        [JsonProperty("dynamicRegistration", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DynamicRegistration { get; set; }
        /// <summary>
        /// The client supports code action literals as a valid
        /// response of the `textDocument/codeAction` request.
        /// </summary>
        [Since(3, 8, 0)]
        [JsonProperty("codeActionLiteralSupport", NullValueHandling = NullValueHandling.Ignore)]
        public CodeActionLiteralSupportCapabilities? CodeActionLiteralSupport { get; set; }
        /// <summary>
        /// Whether code action supports the `isPreferred` property.
        /// </summary>
        [Since(3, 15, 0)]
        [JsonProperty("isPreferredSupport", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsPreferredSupport { get; set; }
        /// <summary>
        /// Whether code action supports the `disabled` property.
        /// </summary>
        [Since(3, 16, 0)]
        [JsonProperty("disabledSupport", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DisabledSupport { get; set; }
        /// <summary>
        /// Whether code action supports the `data` property which is
        /// preserved between a `textDocument/codeAction` and a
        /// `codeAction/resolve` request.
        /// </summary>
        [Since(3, 16, 0)]
        [JsonProperty("dataSupport", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DataSupport { get; set; }
        /// <summary>
        /// Whether the client supports resolving additional code action
        /// properties via a separate `codeAction/resolve` request.
        /// </summary>
        [Since(3, 16, 0)]
        [JsonProperty("resolveSupport", NullValueHandling = NullValueHandling.Ignore)]
        public ResolveSupportCapabilities? ResolveSupport { get; set; }
        /// <summary>
        /// Whether th client honors the change annotations in
        /// text edits and resource operations returned via the
        /// `CodeAction#edit` property by for example presenting
        /// the workspace edit in the user interface and asking
        /// for confirmation.
        /// </summary>
        [Since(3, 16, 0)]
        [JsonProperty("honorsChangeAnnotations", NullValueHandling = NullValueHandling.Ignore)]
        public bool? HonorsChangeAnnotations { get; set; }
    }
}
