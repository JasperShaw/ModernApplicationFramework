﻿using System;
using System.Text;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal class FallbackDetector : DecoderFallback
    {
        private readonly DecoderFallback _decoderFallback;

        public override int MaxCharCount => _decoderFallback.MaxCharCount;

        internal bool FallbackOccurred { get; private set; }

        public FallbackDetector(DecoderFallback decoderFallback)
        {
            _decoderFallback = decoderFallback;
        }

        public override DecoderFallbackBuffer CreateFallbackBuffer()
        {
            var fallbackBufferDetector = new FallbackBufferDetector(_decoderFallback.CreateFallbackBuffer());
            fallbackBufferDetector.FallbackOccurred += (s, e) => FallbackOccurred = true;
            return fallbackBufferDetector;
        }

        private class FallbackBufferDetector : DecoderFallbackBuffer
        {
            private readonly DecoderFallbackBuffer _inner;

            internal event EventHandler FallbackOccurred;

            public override int Remaining => _inner.Remaining;

            internal FallbackBufferDetector(DecoderFallbackBuffer inner)
            {
                _inner = inner;
            }

            public override bool Fallback(byte[] bytesUnknown, int index)
            {
                FallbackOccurred?.Invoke(this, EventArgs.Empty);
                return _inner.Fallback(bytesUnknown, index);
            }

            public override char GetNextChar()
            {
                return _inner.GetNextChar();
            }

            public override bool MovePrevious()
            {
                return _inner.MovePrevious();
            }
        }
    }
}