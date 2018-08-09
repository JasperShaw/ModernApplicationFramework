using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal class HiddenRegionsExpandedOrCollapsedEventArgs : EventArgs
    {
        internal IEnumerable<ITrackingSpan> TrackingSpans { get; }

        internal bool Collapsed { get; }

        internal bool SubjectBufferSpans { get; }

        internal bool Undoable { get; }

        internal HiddenRegionsExpandedOrCollapsedEventArgs(IEnumerable<ITrackingSpan> trackingSpans, bool collapsed, bool subjectBufferSpans, bool undoable)
        {
            TrackingSpans = trackingSpans;
            Collapsed = collapsed;
            SubjectBufferSpans = subjectBufferSpans;
            Undoable = undoable;
        }
    }
}