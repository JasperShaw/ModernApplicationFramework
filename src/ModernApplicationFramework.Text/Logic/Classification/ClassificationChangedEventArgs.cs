using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Classification
{
    public class ClassificationChangedEventArgs : EventArgs
    {
        public ClassificationChangedEventArgs(SnapshotSpan changeSpan)
        {
            ChangeSpan = changeSpan;
        }

        public SnapshotSpan ChangeSpan { get; }
    }
}