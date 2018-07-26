using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    public interface IClassifier
    {
        IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span);

        event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
    }
}