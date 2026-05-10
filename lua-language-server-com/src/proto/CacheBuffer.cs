using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
#nullable enable
namespace src.proto
{
    internal class CacheBuffer
    {
        byte[]? _buffer;
        private Timer? _timer;
        public Int64 Lenght => _buffer?.LongLength ?? 0;
        public Int64 Offset;
        private Int64 _frameSize;
        private readonly object _lock = new object();
        private double _interval;
        public CacheBuffer(double intervalMs, Int64 frameSize){
            _interval = intervalMs;
        }
        public byte[] GetBytes(){
            return _buffer ?? [];
        }
        public void Write(byte[] buffer, int offset, int length){
            lock (_lock)
            {
                if(_buffer is null) {
                    _buffer = new byte[_frameSize];
                }
                Buffer.BlockCopy(buffer, offset, _buffer!, 0, length);
                if (_timer is not null)
                    StartTimer();
            }
        }

        public void Read(byte[] buffer, int offset, int length)
        {
            lock (_lock)
            {
                if (_buffer is null)
                    return;
                Buffer.BlockCopy(_buffer!, 0, buffer, 0, length);
                if(_timer is not null) 
                    StartTimer();
            }
        }
        public void StartTimer(){
            _timer = new Timer(_interval);
            _timer.Elapsed += ClearBuffer;
            _timer.AutoReset = false;
            _timer.Start();
        }
        private void ClearBuffer(object? sender, ElapsedEventArgs e)
        {
            lock (_lock)
            {
                if (_buffer is not null)
                    Array.Clear(_buffer, 0, _buffer.Length);
            }
        }
    }
}
