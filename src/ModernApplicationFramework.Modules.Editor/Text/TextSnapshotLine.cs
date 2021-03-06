﻿using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.Text
{
    internal class TextSnapshotLine : ITextSnapshotLine
    {
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

        public SnapshotSpan Extent { get; }

        public SnapshotSpan ExtentIncludingLineBreak => new SnapshotSpan(Extent.Start, LengthIncludingLineBreak);
        public int Length => Extent.Length;
        public int LengthIncludingLineBreak => Extent.Length + LineBreakLength;
        public int LineBreakLength { get; }
        public int LineNumber { get; }
        public ITextSnapshot Snapshot => Extent.Snapshot;

        public SnapshotPoint Start => Extent.Start;

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

        public TextSnapshotLine(ITextSnapshot snapshot, TextImageLine lineSpan)
            : this(snapshot, lineSpan.LineNumber, lineSpan.Extent, lineSpan.LineBreakLength)
        {
        }

        public string GetLineBreakText()
        {
            var local = Extent;
            var snapshot = local.Snapshot;
            local = Extent;
            var span = new Span(local.Span.End, LineBreakLength);
            return snapshot.GetText(span);
        }


        public string GetText()
        {
            return Extent.GetText();
        }

        public string GetTextIncludingLineBreak()
        {
            return ExtentIncludingLineBreak.GetText();
        }
    }
}