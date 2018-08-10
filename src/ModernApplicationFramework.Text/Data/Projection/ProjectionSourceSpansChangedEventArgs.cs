using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ModernApplicationFramework.Text.Data.Projection
{
    public class ProjectionSourceSpansChangedEventArgs : TextContentChangedEventArgs
    {
        public ProjectionSourceSpansChangedEventArgs(ITextSnapshot beforeSnapshot, ITextSnapshot afterSnapshot, IList<ITrackingSpan> insertedSpans, IList<ITrackingSpan> deletedSpans, int spanPosition, EditOptions options, object editTag)
            : base(beforeSnapshot, afterSnapshot, options, editTag)
        {
            if (insertedSpans == null)
                throw new ArgumentNullException(nameof(insertedSpans));
            if (deletedSpans == null)
                throw new ArgumentNullException(nameof(deletedSpans));
            InsertedSpans = new ReadOnlyCollection<ITrackingSpan>(insertedSpans);
            DeletedSpans = new ReadOnlyCollection<ITrackingSpan>(deletedSpans);
            SpanPosition = spanPosition;
        }

        public int SpanPosition { get; }

        public ReadOnlyCollection<ITrackingSpan> InsertedSpans { get; }

        public ReadOnlyCollection<ITrackingSpan> DeletedSpans { get; }

        public new IProjectionSnapshot Before => (IProjectionSnapshot)base.Before;

        public new IProjectionSnapshot After => (IProjectionSnapshot)base.After;
    }
}