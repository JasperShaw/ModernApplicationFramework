using System;
using System.IO;

namespace ModernApplicationFramework.Modules.Editor.Text
{
    internal class CharStream : Stream
    {
        private readonly char[] _data;
        private readonly int _length;
        private byte? _pendingByte;
        private int _position;

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => _length;

        public override long Position
        {
            get => _position;
            set => throw new NotSupportedException();
        }

        public CharStream(char[] data, int length)
        {
            _data = data;
            _length = 2 * length;
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var num1 = Math.Min(count, _length - _position);
            if (num1 > 0)
            {
                var num2 = num1;
                byte hi;
                byte lo;
                if (_position % 2 == 1)
                {
                    Split(_data[_position / 2], out hi, out lo);
                    buffer[offset++] = hi;
                    ++_position;
                    --num2;
                }

                for (var index = 0; index < num2 / 2; ++index)
                {
                    Split(_data[_position / 2], out hi, out lo);
                    buffer[offset++] = hi;
                    buffer[offset++] = lo;
                    _position += 2;
                }

                if (num2 % 2 == 1)
                {
                    Split(_data[_position / 2], out hi, out lo);
                    buffer[offset++] = lo;
                    ++_position;
                }
            }

            return num1;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (count == 0)
                return;
            if (_pendingByte.HasValue)
            {
                _data[_position / 2] = Make(_pendingByte.Value, buffer[offset]);
                ++_position;
                ++offset;
                --count;
            }

            for (var index = 0; index < count / 2; ++index)
            {
                _data[_position / 2] = Make(buffer[offset], buffer[offset + 1]);
                _position += 2;
                offset += 2;
            }

            if (count % 2 == 0)
            {
                _pendingByte = new byte?();
            }
            else
            {
                _pendingByte = buffer[offset];
                ++_position;
            }
        }

        private char Make(byte hi, byte lo)
        {
            return (char) (((uint) hi << 8) | lo);
        }

        private void Split(char c, out byte hi, out byte lo)
        {
            hi = (byte) ((uint) c >> 8);
            lo = (byte) (c & (uint) byte.MaxValue);
        }
    }
}