using System.Collections.Generic;
using System.Threading;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Tagging
{
    public interface IAccurateTagAggregator<out T> : ITagAggregator<T> where T : ITag
    {
        IEnumerable<IMappingTagSpan<T>> GetAllTags(SnapshotSpan span, CancellationToken cancel);

        IEnumerable<IMappingTagSpan<T>> GetAllTags(IMappingSpan span, CancellationToken cancel);

        IEnumerable<IMappingTagSpan<T>> GetAllTags(NormalizedSnapshotSpanCollection snapshotSpans,
            CancellationToken cancel);
    }
}