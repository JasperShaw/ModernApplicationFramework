using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ModernApplicationFramework.Text.Data.Projection
{
    public class ProjectionSourceBuffersChangedEventArgs : ProjectionSourceSpansChangedEventArgs
    {
        private readonly IList<ITextBuffer> _addedBuffers;
        private readonly IList<ITextBuffer> _removedBuffers;

        public ProjectionSourceBuffersChangedEventArgs(ITextSnapshot beforeSnapshot, ITextSnapshot afterSnapshot, IList<ITrackingSpan> insertedSpans, IList<ITrackingSpan> deletedSpans, int spanPosition, IList<ITextBuffer> addedBuffers, IList<ITextBuffer> removedBuffers, EditOptions options, object editTag)
            : base(beforeSnapshot, afterSnapshot, insertedSpans, deletedSpans, spanPosition, options, editTag)
        {
            _addedBuffers = addedBuffers ?? throw new ArgumentNullException(nameof(addedBuffers));
            _removedBuffers = removedBuffers ?? throw new ArgumentNullException(nameof(removedBuffers));
        }

        public ReadOnlyCollection<ITextBuffer> AddedBuffers => new ReadOnlyCollection<ITextBuffer>(_addedBuffers);

        public ReadOnlyCollection<ITextBuffer> RemovedBuffers => new ReadOnlyCollection<ITextBuffer>(_removedBuffers);
    }
}