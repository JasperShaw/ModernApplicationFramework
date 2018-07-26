using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITagger<out T> where T : ITag
    {
        IEnumerable<ITagSpan<T>> GetTags(NormalizedSnapshotSpanCollection spans);

        event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}