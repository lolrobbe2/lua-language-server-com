using K4os.Compression.LZ4;
using K4os.Compression.LZ4.Streams;
using System;
using System.Collections.Generic;
using System.Text;

namespace src.buffer
{
    internal class CompressedBuffer
    {
        private byte[] _buffer;
        public Int64 Length => _buffer.LongLength;
        public Int64 OriginalLength = 0;
        public Int64 Offset;
        public void Write(byte[] buffer, int offset, int count)
        {
            int maxRequiredSize = LZ4Codec.MaximumOutputSize(count);
            byte[] destination = new byte[maxRequiredSize];
            int size = LZ4Frame.Encode(buffer.AsSpan(offset), destination.AsSpan());
            _buffer = new byte[size];
            Buffer.BlockCopy(destination,0,_buffer, 0,size);
            OriginalLength = count;
        }

        public void Read(byte[] buffer, int offset,int count){
            Span<byte> temp = buffer.AsSpan(offset);
            var reader = LZ4Frame.Decode(_buffer);
            reader.ReadManyBytes(temp);          
        }
        public byte[] GetBytes(){
            return _buffer;
        }
    }
}
