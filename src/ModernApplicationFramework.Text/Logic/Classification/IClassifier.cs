using System;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Classification
{
    public interface IClassifier
    {
        IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span);

        event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
    }
}