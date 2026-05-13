using src.buffer;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
namespace tests.src
{
    public class ComrpessedBufferTests
    {
        [Fact]
        public void CompressedBufferWriteSize(){
            CompressedBuffer buffer = new CompressedBuffer();
            byte[] data = new byte[1024];
            for(int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)i;
            }
            buffer.Write(data, 0, data.Length);
            Assert.Equal(buffer.OriginalLength, data.Length);
            Assert.NotEqual(buffer.Length, data.Length);
            Assert.NotEmpty(buffer.GetBytes());
        }

        [Fact]
        public void CompressedBufferWriteFrameSmall()
        {
            CompressedBuffer buffer = new CompressedBuffer();
            byte[] data = new byte[10];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)i;
            }
            buffer.Write(data, 0, data.Length);
            Assert.Equal(buffer.OriginalLength, data.Length);
            Assert.NotEqual(buffer.Length, data.Length);
            Assert.NotEmpty(buffer.GetBytes());
        }

        [Fact]
        public void CompressedBufferRead(){
            CompressedBuffer buffer = new CompressedBuffer();
            byte[] data = new byte[1024];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)i;
            }
            buffer.Write(data, 0, data.Length);
            byte[] readData = new byte[1024];
            buffer.Read (readData, 0, readData.Length);
            Assert.Equal(readData, data);
        }
    }
}
