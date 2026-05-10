using K4os.Compression.LZ4.Streams;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
#nullable enable
namespace src.proto.document
{
    using DocumentUri = string;
    /// <summary>
    /// this is the internal used representation of a LSP text document, this is compressed to optemise memory usage and speed up processing
    /// </summary>
    internal class InternalTextDocument
    {
        private readonly MemoryStream _masterStream = new();
        private readonly List<(long Start, int Length)> _lineMap = new();

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

        public InternalTextDocument(TextDocument document) 
        {
            this.uri = document.uri;
            this.version = document.version;
            this.languageId = document.languageId;
            Compress(document.text);
        }

        private void Compress(string text) {
            string[] lines = text.Split("\n");
            if (lines.Length > 1)
            {
                foreach (string line in lines)
                {
                    CompressLine(line);
                }
            } 
        }
        private void CompressLine(string line) {
            int lineCount = _lineMap.Count;
            long startOffset = _masterStream.Position;
            //TODO add encoding type.
            byte[] lineBytes = Encoding.UTF8.GetBytes(line);
            //_compressorStream!.Write(lineBytes, 0, lineBytes.Length);
            //_compressorStream.Flush();

            long endOffset = _masterStream.Position;
            int compressedLength = (int)(endOffset - startOffset);
            _lineMap.Add((startOffset, compressedLength));
        }
        public string GetLine(int index)
        {
            if(index < 0 || index >= _lineMap.Count)
            {
                throw new IndexOutOfRangeException("line index was out of range");
            }
            var (startOffset, length) = _lineMap[index];
            byte[] compressedBuffer = ArrayPool<byte>.Shared.Rent(length);
            string? result;
            try
            {
                _masterStream.Position = startOffset;
                //_decomporessorStream.ReadExactly(compressedBuffer,(int)0, length);
                result = Encoding.UTF8.GetString(compressedBuffer);
            } finally {
                ArrayPool<byte>.Shared.Return(compressedBuffer);
            }
            return result;
        }
        public string GetText() {
            string text = "";
            for (int i = 0; i < _lineMap.Count; i++)
            {
                text += GetLine(i) + "\n";
            }
            return text;
        }
    }
}
