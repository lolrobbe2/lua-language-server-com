using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
#nullable enable
namespace src.buffer
{
    internal class CacheBuffer
    {
        byte[]? _buffer;
        private Timer? _timer;
        public Int64 Lenght => _buffer?.LongLength ?? 0;
        private readonly object _lock = new object();
        private double _interval;

        public event Action<byte[], Int64>? OnClear;
        public CacheBuffer(double intervalMs){
            _interval = intervalMs;
        }
        public byte[] GetBytes(){
            return _buffer ?? [];
        }
        public void Write(byte[] buffer, int offset, int length){
            lock (_lock)
            {
                if(_buffer is null) {
                    _buffer = new byte[length];
                }
                Buffer.BlockCopy(buffer, offset, _buffer!, 0, length);
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
                StartTimer();
            }
        }
        public void StartTimer(){
            _timer = new Timer(_interval);
            _timer.Elapsed += ClearBuffer;
            _timer.AutoReset = false;
            _timer.Start();
        }
        public bool IsCleared(){
            return _buffer == null;
        }
        private void ClearBuffer(object? sender, ElapsedEventArgs e)
        {
            Clear();
        }
        public void Clear() {
            if (_buffer is null)
                return;
            lock (_lock)
            {
                OnClear?.Invoke(_buffer!, Lenght);
                _buffer = null;
            }
        }

    }
}
