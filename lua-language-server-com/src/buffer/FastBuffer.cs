using K4os.Compression.LZ4.Streams;
using System;
using System.Collections.Generic;
using System.Text;

namespace src.buffer
{
    internal class FastBuffer
    {
        CacheBuffer cache { get; set; }
        CompressedBuffer compressed { get; set; }

        public FastBuffer(double clearInterval = 200){
            this.cache = new CacheBuffer(clearInterval);
            this.compressed = new CompressedBuffer();
            this.cache.OnClear += Flush;
        }
        public void Write(byte[] buffer, int offset, int count)
        {
            cache.Write(buffer, offset, count);
        }
        public void Read(byte[] buffer, int offset, int count)
        {
           if(cache.IsCleared()){
                byte[] temp = new byte[compressed.OriginalLength];
                compressed.Read(temp,0, temp.Length);
                cache.Write(temp,0,temp.Length);
           }
           cache.Read(buffer, offset, count);
        }
        private void Flush(byte[] buffer, Int64 lenght) {
            compressed.Write(buffer, 0, (int)lenght);
        }
        public void ForceCompress(){
            cache.Clear();
        }
        public byte[] GetBytes() {
            if (cache.IsCleared())
            {
                byte[] temp = new byte[compressed.OriginalLength];
                compressed.Read(temp, 0, temp.Length);
                cache.Write(temp, 0, temp.Length);
            }
            return cache.GetBytes();
        }
    }
}
