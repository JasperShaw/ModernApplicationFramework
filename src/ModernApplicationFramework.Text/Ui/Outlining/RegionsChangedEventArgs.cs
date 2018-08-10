using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Outlining
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