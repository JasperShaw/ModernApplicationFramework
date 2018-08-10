using System;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;

namespace ModernApplicationFramework.Text.Logic.Tagging
{
    public interface ITagAggregator<out T> : IDisposable where T : ITag
    {
        IEnumerable<IMappingTagSpan<T>> GetTags(SnapshotSpan span);

        IEnumerable<IMappingTagSpan<T>> GetTags(IMappingSpan span);

        IEnumerable<IMappingTagSpan<T>> GetTags(NormalizedSnapshotSpanCollection snapshotSpans);

        event EventHandler<TagsChangedEventArgs> TagsChanged;

        event EventHandler<BatchedTagsChangedEventArgs> BatchedTagsChanged;

        IBufferGraph BufferGraph { get; }
    }
}