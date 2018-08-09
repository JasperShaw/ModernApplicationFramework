using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal class TrackingSpanComparer : IComparer<ITrackingSpan>
    {
        private ITextBuffer Buffer { get; }

        internal TrackingSpanComparer(ITextBuffer buffer)
        {
            Buffer = buffer;
        }

        public int Compare(ITrackingSpan trackingSpan1, ITrackingSpan trackingSpan2)
        {
            var currentSnapshot = Buffer.CurrentSnapshot;
            var span1 = trackingSpan1.GetSpan(currentSnapshot);
            var span2 = trackingSpan2.GetSpan(currentSnapshot);
            if (span1.Start != span2.Start)
                return span1.Start.CompareTo(span2.Start);
            return -span1.Length.CompareTo(span2.Length);
        }
    }
}