using System;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Classification
{
    public interface IClassifier
    {
        event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
        IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span);
    }
}