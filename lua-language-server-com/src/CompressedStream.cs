using src.proto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace src
{
    
    internal class CompressedStream : Stream
    {
        Int64 _frameSize = 1024;
        private List<CompressedBuffer> _frames = new List<CompressedBuffer>();
        private Dictionary<Int64,CacheBuffer> _cachedFrames = new Dictionary<Int64,CacheBuffer>();

        private Int64 _position = 0;
        public override bool CanRead => throw new NotImplementedException();

        public override bool CanSeek => throw new NotImplementedException();

        public override bool CanWrite => throw new NotImplementedException();

        public override long Length => throw new NotImplementedException();

        public override long Position { get => _position; set => _position = value; }

        public override void Flush()
        {
            foreach (KeyValuePair<Int64, CacheBuffer> frameCache in _cachedFrames)
            {
                if(frameCache.Value.Lenght == 0){
                    /* if lenght is 0 => cleared/invalidated buffer remove */
                    _cachedFrames.Remove(frameCache.Key);
                    continue;
                } else if(_frames.Count < frameCache.Key) {
                    /* range has not be added to the list => increase compressed range size */
                    _frames.AddRange(Enumerable.Repeat(default(CompressedBuffer), (int)(frameCache.Key - _frames.Count)));
                }
                
                CacheBuffer buffer = frameCache.Value;
                _frames[(int)frameCache.Key].Write(buffer.GetBytes(), 0, (int)buffer.Lenght);
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
           
        }
        private int ReadCached(byte[]buffer, int offset, int count) {
            Int64 currentIndex = _position / _frameSize;
            Int64 bufferOffset = _position % _frameSize;
            if (_cachedFrames.TryGetValue(currentIndex, out CacheBuffer cacheBuffer))
            {
                cacheBuffer.Read(buffer, offset, count);
                return (int)(_frameSize - bufferOffset);
            }
            else
            {
                CacheBuffer newCacheBuffer = new CacheBuffer(200, _frameSize);
                CompressedBuffer compressed = _frames[(int)currentIndex];
                byte[] tempBuffer = new byte[compressed.OriginalLength];

                compressed.Read(tempBuffer, 0, tempBuffer.Length);
                newCacheBuffer.Write(tempBuffer,0, (int)compressed.OriginalLength);
                _cachedFrames[currentIndex] = newCacheBuffer;
                /* reattempt to read */
                return ReadCached(buffer, offset, count);
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        
        public override void Write(byte[] buffer, int offset, int count)
        {
            Int64 frameIndex = _position / _frameSize;
            Int64 bufferIndex = _position % _frameSize;
            CacheBuffer cacheBuffer = _cachedFrames[frameIndex] ?? new CacheBuffer(200,_frameSize);
            if ((offset + count) > _frameSize)
            {
                cacheBuffer.Write(buffer, offset, (int)(_frameSize - offset));
                _position += _frameSize - offset;
                Write(buffer, 0, (int)(count - (_frameSize - offset)));
                _cachedFrames[frameIndex] = cacheBuffer;
            } else {
                cacheBuffer.Write(buffer, offset, (int)(_frameSize - offset));
                _position += _frameSize - offset;
                _cachedFrames[frameIndex] = cacheBuffer;
            }

        }
    }
}
