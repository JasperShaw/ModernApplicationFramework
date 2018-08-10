using System;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Tagging
{
    public interface ITagger<out T> where T : ITag
    {
        IEnumerable<ITagSpan<T>> GetTags(NormalizedSnapshotSpanCollection spans);

        event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}