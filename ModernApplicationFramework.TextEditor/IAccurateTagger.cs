using System.Collections.Generic;
using System.Threading;

namespace ModernApplicationFramework.TextEditor
{
    public interface IAccurateTagger<out T> : ITagger<T> where T : ITag
    {
        IEnumerable<ITagSpan<T>> GetAllTags(NormalizedSnapshotSpanCollection spans, CancellationToken cancel);
    }
}