using System;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Tagging
{
    public interface ITagger<out T> where T : ITag
    {
        event EventHandler<SnapshotSpanEventArgs> TagsChanged;
        IEnumerable<ITagSpan<T>> GetTags(NormalizedSnapshotSpanCollection spans);
    }
}