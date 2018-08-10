using System;

namespace ModernApplicationFramework.Text.Data
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