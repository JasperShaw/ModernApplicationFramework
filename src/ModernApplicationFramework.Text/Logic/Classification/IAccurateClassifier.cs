using System.Collections.Generic;
using System.Threading;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Classification
{
    public interface IAccurateClassifier : IClassifier
    {
        IList<ClassificationSpan> GetAllClassificationSpans(SnapshotSpan span, CancellationToken cancel);
    }
}