using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Utilities
{
    public class LineBuffer
    {
        public static int BufferSize = 1024;
        private readonly ITextSnapshotLine _line;
        private string _contents;
        private Span _extent;

        public LineBuffer(ITextSnapshotLine line)
        {
            _line = line;
            if (line.LengthIncludingLineBreak <= BufferSize)
            {
                _contents = line.GetTextIncludingLineBreak();
                _extent = new Span(0, line.LengthIncludingLineBreak);
            }
            else
            {
                _extent = new Span(0, 0);
            }
        }

        public char this[int index]
        {
            get
            {
                if (_extent.Contains(index))
                    return _contents[index - _extent.Start];
                var start = Math.Max(0, index - BufferSize / 2);
                var end = Math.Min(_line.LengthIncludingLineBreak, start + BufferSize);
                _extent = Span.FromBounds(start, end);
                _contents = _line.Snapshot.GetText(_line.Start + start, _extent.Length);
                return _contents[index - _extent.Start];
            }
        }
    }
}