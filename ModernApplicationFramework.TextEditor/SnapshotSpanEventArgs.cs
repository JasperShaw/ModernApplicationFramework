using System;

namespace ModernApplicationFramework.TextEditor
{
    public class SnapshotSpanEventArgs : EventArgs
    {
        public SnapshotSpan Span { get; }

        public SnapshotSpanEventArgs(SnapshotSpan span)
        {
            Span = span;
        }
    }
}