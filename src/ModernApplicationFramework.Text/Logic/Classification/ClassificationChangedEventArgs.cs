using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Classification
{
    public class ClassificationChangedEventArgs : EventArgs
    {
        public SnapshotSpan ChangeSpan { get; }

        public ClassificationChangedEventArgs(SnapshotSpan changeSpan)
        {
            ChangeSpan = changeSpan;
        }
    }
}