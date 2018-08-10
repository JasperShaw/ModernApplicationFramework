using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    public static class LineBreakManager
    {
        public static readonly ILineBreaks Empty = new ShortLineBreaksEditor(0);

        public static ILineBreaksEditor CreateLineBreakEditor(int maxLength, int initialCapacity = 0)
        {
            if (maxLength >= short.MaxValue)
                return new IntLineBreaksEditor(initialCapacity);
            return new ShortLineBreaksEditor(initialCapacity);
        }

        public static ILineBreaks CreateLineBreaks(string source)
        {
            ILineBreaksEditor lineBreaksEditor = null;
            var start = 0;
            while (start < source.Length)
            {
                var length = TextUtilities.LengthOfLineBreak(source, start, source.Length);
                if (length == 0)
                {
                    ++start;
                }
                else
                {
                    if (lineBreaksEditor == null)
                        lineBreaksEditor = CreateLineBreakEditor(source.Length);
                    lineBreaksEditor.Add(start, length);
                    start += length;
                }
            }
            return lineBreaksEditor ?? Empty;
        }

        private class ShortLineBreaksEditor : ILineBreaksEditor
        {
            private static readonly List<ushort> Empty = new List<ushort>(0);
            private List<ushort> _lineBreaks = Empty;

            public int Length => _lineBreaks.Count;

            public ShortLineBreaksEditor(int initialCapacity)
            {
                if (initialCapacity <= 0)
                    return;
                _lineBreaks = new List<ushort>(initialCapacity);
            }

            public int LenghtOfLineBreak(int index)
            {
                return (_lineBreaks[index] & 32768) == 0 ? 1 : 2;
            }

            public int StartOfLineBreak(int index)
            {
                return _lineBreaks[index] & short.MaxValue;
            }

            public int EndOfLineBreak(int index)
            {
                int lineBreak = _lineBreaks[index];
                return (lineBreak & short.MaxValue) + ((lineBreak & 32768) != 0 ? 2 : 1);
            }

            public void Add(int start, int length)
            {
                if (start < 0 || start > short.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(start));
                if (length < 1 || length > 2)
                    throw new ArgumentOutOfRangeException(nameof(length));
                if (_lineBreaks == Empty)
                    _lineBreaks = new List<ushort>();
                if (length == 1)
                {
                    _lineBreaks.Add((ushort)start);
                }
                else
                {
                    if (length != 2)
                        return;
                    _lineBreaks.Add((ushort)(start | 32768));
                }
            }
        }

        private class IntLineBreaksEditor : ILineBreaksEditor
        {
            private static readonly List<uint> Empty = new List<uint>(0);
            private List<uint> _lineBreaks = Empty;

            public IntLineBreaksEditor(int initialCapacity)
            {
                if (initialCapacity <= 0)
                    return;
                _lineBreaks = new List<uint>(initialCapacity);
            }

            public int Length => _lineBreaks.Count;

            public int LengthOfLineBreak(int index)
            {
                return ((int)_lineBreaks[index] & int.MinValue) == 0 ? 1 : 2;
            }

            public int StartOfLineBreak(int index)
            {
                return (int)_lineBreaks[index] & int.MaxValue;
            }

            public int EndOfLineBreak(int index)
            {
                uint lineBreak = _lineBreaks[index];
                return (int)((lineBreak & int.MaxValue) + (((int)lineBreak & int.MinValue) != 0 ? 2L : 1L));
            }

            public void Add(int start, int length)
            {
                if (start < 0 || start > int.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(start));
                if (length < 1 || length > 2)
                    throw new ArgumentOutOfRangeException(nameof(length));
                if (_lineBreaks == Empty)
                    _lineBreaks = new List<uint>();
                if (length == 1)
                {
                    _lineBreaks.Add((uint)start);
                }
                else
                {
                    if (length != 2)
                        return;
                    _lineBreaks.Add((uint)((ulong)start | 2147483648UL));
                }
            }
        }
    }
}