using System;

namespace ModernApplicationFramework.TextEditor.Text
{
    internal class TextSnapshotLine : ITextSnapshotLine
    {
        public ITextSnapshot Snapshot => Extent.Snapshot;
        public SnapshotSpan Extent { get; }

        public SnapshotSpan ExtentIncludingLineBreak => new SnapshotSpan(Extent.Start, LengthIncludingLineBreak);
        public int LineNumber { get; }

        public SnapshotPoint Start => Extent.Start;
        public int Length => Extent.Length;
        public int LengthIncludingLineBreak => Extent.Length + LineBreakLength;
        public SnapshotPoint End => Extent.End;

        public SnapshotPoint EndIncludingLineBreak
        {
            get
            {
                var snapshot = Extent.Snapshot;
                var local = Extent;
                var position = local.Span.End + LineBreakLength;
                return new SnapshotPoint(snapshot, position);
            }
        }
        public int LineBreakLength { get; }

        public TextSnapshotLine(ITextSnapshot snapshot, int lineNumber, Span extent, int lineBreakLength)
        {
            Extent = new SnapshotSpan(snapshot, extent);
            LineNumber = lineNumber;
            LineBreakLength = lineBreakLength;
        }

        public TextSnapshotLine(ITextSnapshot snapshot, Tuple<int, Span, int> lineSpan)
            : this(snapshot, lineSpan.Item1, lineSpan.Item2, lineSpan.Item3)
        {
        }


        public string GetText()
        {
            return Extent.GetText();
        }

        public string GetTextIncludingLineBreak()
        {
            return ExtentIncludingLineBreak.GetText();
        }

        public string GetLineBreakText()
        {
            var local = Extent;
            var snapshot = local.Snapshot;
            local = Extent;
            var span = new Span(local.Span.End, LineBreakLength);
            return snapshot.GetText(span);
        }
    }
}