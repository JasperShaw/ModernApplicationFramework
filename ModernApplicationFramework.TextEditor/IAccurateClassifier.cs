using System.Collections.Generic;
using System.Threading;

namespace ModernApplicationFramework.TextEditor
{
    public interface IAccurateClassifier : IClassifier
    {
        IList<ClassificationSpan> GetAllClassificationSpans(SnapshotSpan span, CancellationToken cancel);
    }
}