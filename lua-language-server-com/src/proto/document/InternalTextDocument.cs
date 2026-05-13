using K4os.Compression.LZ4.Streams;
using src.buffer;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#nullable enable
namespace src.proto.document

{
    using DocumentUri = string;

    internal class Chunk
    {
        private string[] Lines { get; set; }
        public Chunk(string chunk) {
            Lines = chunk.Split('\n');
        }
        public Chunk(string[] newLines) {
            Lines = newLines;
        }
        public Chunk(List<string> newLines)
        {
            Lines = newLines.ToArray();
        }
        public Chunk GetPosition(Position position){
            string[] temp = Lines.Skip((int)position.line -1).ToArray();
            if (position.character > temp[0].Length)
                position.character = (UInt32)temp[0].Length;
            temp[0] = String.Join("",temp[0].Skip((int)position.character));
            return new Chunk(temp);
        }
        public Chunk Replace(ProtoRange range, string[] newLines){
            List<string> updated = new List<string>();

            for (Int32 i = 0; i < range.start.line; i++)
            {
                updated.Add(Lines[i]);
            }

            String prefix = Lines[(Int32)range.start.line]
                .Substring(0, (Int32)range.start.character);

            String suffix = Lines[(Int32)range.end.line]
                .Substring((Int32)range.end.character);

            if (newLines.Length == 0) {
                updated.Add(prefix + suffix);
            } else if (newLines.Length == 1){
                updated.Add(prefix + newLines[0] + suffix);
            } else{
                updated.Add(prefix + newLines[0]);

                for (Int32 i = 1; i < newLines.Length - 1; i++) {
                    updated.Add(newLines[i]);
                }

                updated.Add(newLines[newLines.Length - 1] + suffix);
            }

            for (Int32 i = (Int32)range.end.line + 1; i < Lines.Length; i++) {
                updated.Add(Lines[i]);
            }

            return new Chunk(updated);
        }
        public string Text => String.Join('\n',Lines);
    }

    /// <summary>
    /// this is the internal used representation of a LSP text document, this is compressed to optemise memory usage and speed up processing
    /// </summary>
    internal class InternalTextDocument
    {
        private List<FastBuffer> lineBuffer = new List<FastBuffer>();
        /// <summary>
        /// lines per fastbuffer
        /// </summary>
        private UInt32 bufferChunkSize = 20;
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

        /// <summary>
        /// this function does the initial text ingress.
        /// </summary>
        /// <param name="text"></param>
        private void Compress(string text) {
            string[] chunks = text
             .Split('\n')
             .Chunk((int)bufferChunkSize)
             .Select(static lines => string.Join('\n', lines))
             .ToArray();

            FastBuffer[] temp = new FastBuffer[chunks.Length];

            Parallel.For(0, chunks.Length, index =>
            {
                string region = chunks[index];
                byte[] bytes = Encoding.UTF8.GetBytes(region);

                FastBuffer fast = new FastBuffer();
                fast.Write(bytes, 0, bytes.Length);
                fast.ForceCompress();
                temp[index] = fast;
            });

            lineBuffer = [.. temp];
        }
        public string GetLineBuffer(int index){
            return Encoding.UTF8.GetString(lineBuffer[index].GetBytes());

        }
        public string GetRange(ProtoRange range) {
            UInt32 startIndex = range.start.line / bufferChunkSize;
            UInt32 endIndex = range.end.line / bufferChunkSize;

            List<Chunk> chunks = new List<Chunk>();

            for (UInt32 i = startIndex; i < endIndex; i++)
            {
                chunks.Add(new Chunk(GetLineBuffer((int)i)));
            }
            chunks[0] = chunks[0].GetPosition(range.start);
            chunks[chunks.Count - 1] = chunks[chunks.Count - 1].GetPosition(range.end);
            return String.Join('\n',chunks.Select((chunk)=> chunk.Text));
        }
        public void UpdateRange(ProtoRange range, string text)
        {
            UInt32 startIndex = range.start.line / bufferChunkSize;
            UInt32 startLineOffset = range.start.line % bufferChunkSize;

            UInt32 endIndex = range.end.line / bufferChunkSize;
            UInt32 endLineOffset = range.end.line % bufferChunkSize;

            List<Chunk> chunks = new List<Chunk>();

            for (UInt32 i = startIndex; i < endIndex; i++)
            {
                chunks.Add(new Chunk(GetLineBuffer((int)i)));
            }
            
        }
        internal string GetText()
        {
            List<string> chunks = new List<string>();
            for (int i = 0; i < lineBuffer.Count; i++)
            {
                chunks.Add(GetLineBuffer(i));
            }
            return String.Join("\n", chunks);
        }
    }
}
