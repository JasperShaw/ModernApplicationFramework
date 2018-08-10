using System.Collections.Generic;
using System.Threading;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Tagging
{
    public interface IAccurateTagger<out T> : ITagger<T> where T : ITag
    {
        IEnumerable<ITagSpan<T>> GetAllTags(NormalizedSnapshotSpanCollection spans, CancellationToken cancel);
    }
}