using System;
using System.Collections.Generic;
using ModernApplicationFramework.Modules.Editor.Text;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Differencing;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Projection
{
    internal abstract class BaseProjectionBuffer : BaseBuffer, IProjectionBufferBase
    {
        protected internal readonly IProjectionEditResolver Resolver;
        protected bool EditApplicationInProgress;

        protected List<TextContentChangedEventArgs> PendingContentChangedEventArgs =
            new List<TextContentChangedEventArgs>();

        public new abstract IProjectionSnapshot CurrentSnapshot { get; }

        public abstract IList<ITextBuffer> SourceBuffers { get; }

        protected abstract BaseProjectionSnapshot CurrentBaseSnapshot { get; }

        protected BaseProjectionBuffer(IProjectionEditResolver resolver, IContentType contentType,
            ITextDifferencingService textDifferencingService, GuardedOperations guardedOperations)
            : base(contentType, 0, textDifferencingService, guardedOperations)
        {
            Resolver = resolver;
        }

        public bool AreBaseBuffersReadOnly(Span span, bool isEdit)
        {
            if (span.Length == 0)
                return AreBaseBuffersReadOnly(span.Start, isEdit);
            foreach (var sourceSnapshot in (IEnumerable<SnapshotSpan>) CurrentBaseSnapshot.MapToSourceSnapshots(span))
                if (((BaseBuffer) sourceSnapshot.Snapshot.TextBuffer).IsReadOnlyImplementation(sourceSnapshot, isEdit))
                    return true;
            return false;
        }

        public abstract override ITextEdit
            CreateEdit(EditOptions options, int? reiteratedVersionNumber, object editTag);

        public new IProjectionSnapshot Delete(Span deleteSpan)
        {
            return (IProjectionSnapshot) base.Delete(deleteSpan);
        }

        public new IProjectionSnapshot Insert(int position, string text)
        {
            return (IProjectionSnapshot) base.Insert(position, text);
        }

        public abstract ITextEventRaiser PropagateSourceChanges(EditOptions options, object editTag);

        public new IProjectionSnapshot Replace(Span replaceSpan, string replaceWith)
        {
            return (IProjectionSnapshot) base.Replace(replaceSpan, replaceWith);
        }

        internal void OnSourceBufferContentTypeChanged(object sender, ContentTypeChangedEventArgs e)
        {
            PendingContentChangedEventArgs.Add(new TextContentChangedEventArgs(e.Before, e.After, EditOptions.None,
                e.EditTag));
            Group.ScheduleIndependentEdit(this);
        }

        internal virtual void OnSourceBufferReadOnlyRegionsChanged(object sender, SnapshotSpanEventArgs e)
        {
            var normalizedSpanCollection =
                new NormalizedSpanCollection(CurrentBaseSnapshot.MapFromSourceSnapshot(e.Span));
            if (normalizedSpanCollection.Count <= 0)
                return;
            var currentSnapshot = CurrentSnapshotProtected;
            var span1 = normalizedSpanCollection[0];
            var start = span1.Start;
            span1 = normalizedSpanCollection[normalizedSpanCollection.Count - 1];
            var end = span1.End;
            var span2 = Span.FromBounds(start, end);
            var raiser =
                (ITextEventRaiser) new ReadOnlyRegionsChangedEventRaiser(new SnapshotSpan(currentSnapshot, span2));
            Group.BeginEdit();
            Group.EnqueueEvents(raiser, this);
            Group.FinishEdit();
        }

        internal abstract void OnSourceTextChanged(object sender, TextContentChangedEventArgs e);

        protected internal abstract override ISubordinateTextEdit CreateSubordinateEdit(EditOptions options,
            int? reiteratedVersionNumber, object editTag);

        protected internal override NormalizedSpanCollection GetReadOnlyExtentsImplementation(Span span)
        {
            var frugalList = new FrugalList<Span>(base.GetReadOnlyExtentsImplementation(span));
            foreach (var snapshotSpan in
                (IEnumerable<SnapshotSpan>) CurrentBaseSnapshot.MapToSourceSnapshotsForRead(span))
            {
                var nullable = snapshotSpan.Span == span ? snapshotSpan : snapshotSpan.Overlap(span);
                if (nullable.HasValue)
                    foreach (var readOnlyExtent in ((BaseBuffer) snapshotSpan.Snapshot.TextBuffer).GetReadOnlyExtents(
                        nullable.Value))
                        frugalList.AddRange(
                            CurrentBaseSnapshot.MapFromSourceSnapshot(new SnapshotSpan(snapshotSpan.Snapshot,
                                readOnlyExtent)));
            }

            return new NormalizedSpanCollection(frugalList);
        }

        protected internal override bool IsReadOnlyImplementation(int position, bool isEdit)
        {
            if (CurrentBaseSnapshot.SpanCount == 0)
                throw new InvalidOperationException();
            if (base.IsReadOnlyImplementation(position, isEdit))
                return true;
            return AreBaseBuffersReadOnly(position, isEdit);
        }

        protected internal override bool IsReadOnlyImplementation(Span span, bool isEdit)
        {
            if (CurrentSnapshot.SpanCount == 0)
                throw new InvalidOperationException();
            if (base.IsReadOnlyImplementation(span, isEdit))
                return true;
            return AreBaseBuffersReadOnly(span, isEdit);
        }

        protected abstract override BaseSnapshot TakeSnapshot();

        private bool AreBaseBuffersReadOnly(int position, bool isEdit)
        {
            foreach (var sourceSnapshot in CurrentBaseSnapshot.MapInsertionPointToSourceSnapshots(position, null))
                if (!((BaseBuffer) sourceSnapshot.Snapshot.TextBuffer).IsReadOnlyImplementation(sourceSnapshot.Position,
                    isEdit))
                    return false;
            return true;
        }
    }
}