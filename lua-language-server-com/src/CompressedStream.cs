using src.proto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace src
{
    /// <summary>
    /// A seekable compressed stream.
    /// </summary>
    internal class CompressedStream : Stream
    {
        Int64 _frameSize = 1024;
        private List<CompressedBuffer> _frames = new List<CompressedBuffer>();
        private Dictionary<Int64,CacheBuffer> _cachedFrames = new Dictionary<Int64,CacheBuffer>();
            
        private Int64 _position = 0;
        private Int64 _length = 0;
        public override bool CanRead => throw new NotImplementedException();

        public override bool CanSeek => throw new NotImplementedException();

        public override bool CanWrite => throw new NotImplementedException();

        public override long Length { get => _length; }  

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
            int totalRead = 0;
            while (totalRead < count)
            {
                Int64 currentIndex = _position / _frameSize;
                Int64 frameOffset = _position % _frameSize;

                long bytesLeftInFrame = _frameSize - frameOffset;
                int bytesToRead = (int)Math.Min(bytesLeftInFrame, count - totalRead);
                ReadCached(buffer, offset + totalRead, bytesToRead);
                _position += bytesToRead;
                totalRead += bytesToRead;
            }
            return totalRead;
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
            long newPosition = 0;

            switch (origin)
            {
                case SeekOrigin.Begin:
                    newPosition = offset;
                    break;

                case SeekOrigin.Current:
                    newPosition = _position + offset;
                    break;

                case SeekOrigin.End:
                    newPosition = _length + offset;
                    break;
            }

            // Guard against seeking before the beginning of the stream
            if (newPosition < 0)
            {
                throw new IOException("An attempt was made to move the stream pointer before the beginning of the stream.");
            }

            _position = newPosition;
            return _position;
        }

        public override void SetLength(long value)
        {
            Int64 maxIndex = value / _frameSize;
            maxIndex += (value % _frameSize > 0) ? 1 : 0;
            /* clear the cache (ignore the bool result)*/
            for (int i = (int)maxIndex; i < _frames.Count; i++)
            {
                _cachedFrames.Remove(i);
            }
            /*clear range*/
            _frames.RemoveRange((int)maxIndex, (int)(_frames.Count - maxIndex));
            
        }

        private int WriteCached(byte[] buffer, int offset, int count){
            Int64 currentIndex = _position / _frameSize;
            Int64 frameOffset = _position % _frameSize;

            if (_cachedFrames.TryGetValue(currentIndex, out CacheBuffer cacheBuffer))
            {
                cacheBuffer.Write(buffer,(int)(offset + frameOffset), (int)(_frameSize - frameOffset));
                return (int)(_frameSize - frameOffset);
            } else {
                cacheBuffer = new CacheBuffer(200, _frameSize);
                _cachedFrames.Add(currentIndex, cacheBuffer);
                /* reattempt the cached write */
                return WriteCached(buffer, offset, count);
            }
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            int totalWrite = 0;
            while (totalWrite < count)
            {
                Int64 currentIndex = _position / _frameSize;
                Int64 frameOffset = _position % _frameSize;

                long bytesLeftInFrame = _frameSize - frameOffset;
                int bytesToRead = (int)Math.Min(bytesLeftInFrame, count - totalWrite);
                WriteCached(buffer, offset + totalWrite, bytesToRead);
                _position += bytesToRead;
                totalWrite += bytesToRead;
            }

        }
    }
}
