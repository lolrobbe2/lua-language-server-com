using src.buffer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace tests.src
{
    public class CacheBufferTest
    {
        [Fact]
        void CacheBufferWriteRead(){
            CacheBuffer buffer = new CacheBuffer(200);
            byte[] data = new byte[1024];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)i;
            }
            buffer.Write(data, 0, data.Length);
            byte[] readData = new byte[1024];
            buffer.Read(readData, 0, readData.Length);
            Assert.Equal(readData, data);
        }
        [Fact]
        async Task CacheBufferAutoInvalidated()
        {
            /*The cachebuffer should clear itself automatically! */
            CacheBuffer buffer = new CacheBuffer(200);
            byte[] data = new byte[1024];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)i;
            }
            buffer.Write(data, 0, data.Length);
            byte[] readData = new byte[1024];
            buffer.Read(readData, 0, readData.Length);
            Assert.Equal(readData, data);
            await Task.Delay(300, TestContext.Current.CancellationToken);
            //NOTE this might fail during debugging (expected, async clearing)!
            Assert.Empty(buffer.GetBytes());
        }
    }
}
