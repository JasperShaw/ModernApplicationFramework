using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ModernApplicationFramework.Text.Data.Projection
{
    public class ProjectionSourceSpansChangedEventArgs : TextContentChangedEventArgs
    {
        public new IProjectionSnapshot After => (IProjectionSnapshot) base.After;

        public new IProjectionSnapshot Before => (IProjectionSnapshot) base.Before;

        public ReadOnlyCollection<ITrackingSpan> DeletedSpans { get; }

        public ReadOnlyCollection<ITrackingSpan> InsertedSpans { get; }

        public int SpanPosition { get; }

        public ProjectionSourceSpansChangedEventArgs(ITextSnapshot beforeSnapshot, ITextSnapshot afterSnapshot,
            IList<ITrackingSpan> insertedSpans, IList<ITrackingSpan> deletedSpans, int spanPosition,
            EditOptions options, object editTag)
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
    }
}