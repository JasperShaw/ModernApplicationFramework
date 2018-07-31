using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    internal class CollapsibleSorter : IComparer<ICollapsible>
    {
        private ITextBuffer SourceBuffer { get; }

        internal CollapsibleSorter(ITextBuffer sourceBuffer)
        {
            SourceBuffer = sourceBuffer;
        }

        public int Compare(ICollapsible x, ICollapsible y)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));
            if (y == null)
                throw new ArgumentNullException(nameof(y));
            ITextSnapshot currentSnapshot = SourceBuffer.CurrentSnapshot;
            SnapshotSpan span1 = x.Extent.GetSpan(currentSnapshot);
            SnapshotSpan span2 = y.Extent.GetSpan(currentSnapshot);
            if (span1.Start != span2.Start)
                return span1.Start.CompareTo(span2.Start);
            return -span1.Length.CompareTo(span2.Length);
        }
    }
}