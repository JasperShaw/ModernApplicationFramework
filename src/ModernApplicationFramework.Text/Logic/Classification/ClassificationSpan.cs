using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Classification
{
    public class ClassificationSpan
    {
        public IClassificationType ClassificationType { get; }

        public SnapshotSpan Span { get; }

        public ClassificationSpan(SnapshotSpan span, IClassificationType classification)
        {
            Span = span;
            ClassificationType = classification ?? throw new ArgumentNullException(nameof(classification));
        }
    }
}