using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Tagging
{
    public class BatchedTagsChangedEventArgs : EventArgs
    {
        public ReadOnlyCollection<IMappingSpan> Spans { get; }

        public BatchedTagsChangedEventArgs(IList<IMappingSpan> spans)
        {
            if (spans == null)
                throw new ArgumentNullException(nameof(spans));
            Spans = new ReadOnlyCollection<IMappingSpan>(new List<IMappingSpan>(spans));
        }
    }
}