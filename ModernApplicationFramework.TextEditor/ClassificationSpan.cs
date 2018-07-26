using System;

namespace ModernApplicationFramework.TextEditor
{
    public class ClassificationSpan
    {
        public ClassificationSpan(SnapshotSpan span, IClassificationType classification)
        {
            Span = span;
            ClassificationType = classification ?? throw new ArgumentNullException(nameof(classification));
        }

        public IClassificationType ClassificationType { get; }

        public SnapshotSpan Span { get; }
    }
}