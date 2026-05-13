using System;
using System.Collections.Generic;
using System.Text;

namespace src.proto
{
    /// <summary>
    /// Position in a text document expressed as zero-based line and zero-based character offset.
    /// 
    /// A position is between two characters like an ‘insert’ cursor in an editor. Special values like for example -1 to denote the end of a line are not supported.
    /// </summary>
    internal class Position {
        /// <summary>
        /// Line position in a document (zero-based).
        /// </summary>
        public UInt32 line {  get; set; }
        /// <summary>
        /// Character offset on a line in a document (zero-based). The meaning of this offset is determined by the negotiated `PositionEncodingKind`.
        /// 
        /// If the character value is greater than the line length it defaults back to the line length.
        /// </summary>
        public UInt32 character { get; set; }
    }
    internal class ProtoRange
    {
        /// <summary>
        /// The range's start position.
        /// </summary>
        public Position start { get; set; }
        /// <summary>
        /// The range's end position.
        /// </summary>
        public Position end { get; set; }
    }
}
