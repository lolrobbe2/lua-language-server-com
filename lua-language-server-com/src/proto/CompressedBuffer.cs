using K4os.Compression.LZ4.Streams;
using System;
using System.Collections.Generic;
using System.Text;

namespace src.proto
{
    internal class CompressedBuffer
    {
        private byte[] _buffer;
        public Int64 Lenght => _buffer.LongLength;
        public Int64 OriginalLength = 0;
        public Int64 Offset;
        public void Write(byte[] buffer, int offset, int count)
        {
            Span<byte> destenation = new Span<byte>();
            LZ4Frame.Encode(buffer.AsSpan(offset), destenation);
            _buffer = destenation.ToArray();
            OriginalLength = count;
        }

        public void Read(byte[] buffer, int offset,int count){
            Span<byte> temp = buffer.AsSpan(offset);
            var reader = LZ4Frame.Decode(_buffer);
            reader.ReadManyBytes(temp);          
        }
    }
}
