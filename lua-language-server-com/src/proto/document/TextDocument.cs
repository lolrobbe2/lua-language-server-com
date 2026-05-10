using System;
using System.Collections.Generic;
using System.Text;

namespace src.proto.document
{
    using DocumentUri = string;
    internal class TextDocument
    {
        /// <summary>
        /// The text document's URI.
        /// </summary>
        public DocumentUri uri { get; set; }
        /// <summary>
        /// The text document's language identifier.
        /// </summary>
        public string languageId { get; set; }
        /// <summary>
        /// The version number of this document (it will increase after each change, including undo/redo).
        /// </summary>
        public int version { get; set; }

        /// <summary>
        /// The content of the opened text document.
        /// </summary>
        public virtual string text { get; set; }
    }
}
