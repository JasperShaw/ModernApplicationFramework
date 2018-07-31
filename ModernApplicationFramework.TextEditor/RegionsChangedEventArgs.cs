using System;

namespace ModernApplicationFramework.TextEditor
{
    public class RegionsChangedEventArgs : EventArgs
    {
        public SnapshotSpan AffectedSpan { get; }

        public RegionsChangedEventArgs(SnapshotSpan affectedSpan)
        {
            AffectedSpan = affectedSpan;
        }
    }
}