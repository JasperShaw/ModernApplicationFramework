using System;

namespace ModernApplicationFramework.TextEditor
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