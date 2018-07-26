using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ModernApplicationFramework.TextEditor
{
    public class BatchedTagsChangedEventArgs : EventArgs
    {
        public BatchedTagsChangedEventArgs(IList<IMappingSpan> spans)
        {
            if (spans == null)
                throw new ArgumentNullException(nameof(spans));
            Spans = new ReadOnlyCollection<IMappingSpan>(new List<IMappingSpan>(spans));
        }

        public ReadOnlyCollection<IMappingSpan> Spans { get; }
    }
}