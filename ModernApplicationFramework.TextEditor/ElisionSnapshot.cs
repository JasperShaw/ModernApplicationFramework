using System;
using System.Collections.ObjectModel;
using ModernApplicationFramework.TextEditor.Text;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    internal class ElisionSnapshot : BaseProjectionSnapshot, IElisionSnapshot
    {
        private readonly ElisionBuffer _elisionBuffer;
        private readonly ElisionMap _content;
        private readonly bool _fillInMappingMode;

        public ElisionSnapshot(ElisionBuffer elisionBuffer, ITextSnapshot sourceSnapshot, ITextVersion version, StringRebuilder builder, ElisionMap content, bool fillInMappingMode)
            : base(version, builder)
        {
            _elisionBuffer = elisionBuffer;
            SourceSnapshot = sourceSnapshot;
            SourceSnapshots = new ReadOnlyCollection<ITextSnapshot>(new FrugalList<ITextSnapshot>
            {
                sourceSnapshot
            });
            TotalLength = content.Length;
            _content = content;
            TotalLineCount = content.LineCount;
            _fillInMappingMode = fillInMappingMode;
            if (TotalLength != version.Length)
                throw new InvalidOperationException();
        }

        public override IProjectionBufferBase TextBuffer => _elisionBuffer;

        IElisionBuffer IElisionSnapshot.TextBuffer => _elisionBuffer;

        protected override ITextBuffer TextBufferHelper => _elisionBuffer;

        public override int SpanCount => _content.SpanCount;

        public override ReadOnlyCollection<ITextSnapshot> SourceSnapshots { get; }

        public ITextSnapshot SourceSnapshot { get; }

        public SnapshotPoint MapFromSourceSnapshotToNearest(SnapshotPoint point)
        {
            return _content.MapFromSourceSnapshotToNearest(this, point.Position);
        }

        public override ITextSnapshot GetMatchingSnapshot(ITextBuffer textBuffer)
        {
            if (textBuffer == null)
                throw new ArgumentNullException(nameof(textBuffer));
            if (SourceSnapshot.TextBuffer != textBuffer)
                return null;
            return SourceSnapshot;
        }

        public override ITextSnapshot GetMatchingSnapshotInClosure(ITextBuffer textBuffer)
        {
            if (textBuffer == null)
                throw new ArgumentNullException(nameof(textBuffer));
            if (SourceSnapshot.TextBuffer == textBuffer)
                return SourceSnapshot;
            return (SourceSnapshot as IProjectionSnapshot)?.GetMatchingSnapshotInClosure(textBuffer);
        }

        public override ITextSnapshot GetMatchingSnapshotInClosure(Predicate<ITextBuffer> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));
            if (match(SourceSnapshot.TextBuffer))
                return SourceSnapshot;
            return (SourceSnapshot as IProjectionSnapshot)?.GetMatchingSnapshotInClosure(match);
        }

        public override ReadOnlyCollection<SnapshotSpan> GetSourceSpans(int startSpanIndex, int count)
        {
            if (startSpanIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startSpanIndex));
            if (count < 0 || startSpanIndex + count > SpanCount)
                throw new ArgumentOutOfRangeException(nameof(count));
            return new ReadOnlyCollection<SnapshotSpan>(_content.GetSourceSpans(SourceSnapshot, startSpanIndex, count));
        }

        public override ReadOnlyCollection<SnapshotSpan> GetSourceSpans()
        {
            return GetSourceSpans(0, _content.SpanCount);
        }

        public override SnapshotPoint MapToSourceSnapshot(int position)
        {
            if (position < 0 || position > TotalLength)
                throw new ArgumentOutOfRangeException(nameof(position));
            var sourceSnapshots = _content.MapInsertionPointToSourceSnapshots(this, position);
            if (sourceSnapshots.Count == 1)
                return sourceSnapshots[0];
            if (_elisionBuffer.Resolver == null)
                return sourceSnapshots[sourceSnapshots.Count - 1];
            return sourceSnapshots[_elisionBuffer.Resolver.GetTypicalInsertionPosition(new SnapshotPoint(this, position), new ReadOnlyCollection<SnapshotPoint>(sourceSnapshots))];
        }

        public override SnapshotPoint MapToSourceSnapshot(int position, PositionAffinity affinity)
        {
            if (position < 0 || position > TotalLength)
                throw new ArgumentOutOfRangeException(nameof(position));
            switch (affinity)
            {
                case PositionAffinity.Predecessor:
                case PositionAffinity.Successor:
                    return _content.MapToSourceSnapshot(SourceSnapshot, position, affinity);
                default:
                    throw new ArgumentOutOfRangeException(nameof(affinity));
            }
        }

        public override SnapshotPoint? MapFromSourceSnapshot(SnapshotPoint point, PositionAffinity affinity)
        {
            if (point.Snapshot != SourceSnapshot)
                throw new ArgumentException("The point does not belong to a source snapshot of the projection snapshot");
            switch (affinity)
            {
                case PositionAffinity.Predecessor:
                case PositionAffinity.Successor:
                    return _content.MapFromSourceSnapshot(this, point.Position);
                default:
                    throw new ArgumentOutOfRangeException(nameof(affinity));
            }
        }

        private ReadOnlyCollection<SnapshotSpan> MapToSourceSnapshots(Span span, bool fillIn)
        {
            if (span.End > TotalLength)
                throw new ArgumentOutOfRangeException(nameof(span));
            var result = new FrugalList<SnapshotSpan>();
            if (fillIn)
                _content.MapToSourceSnapshotsInFillInMode(SourceSnapshot, span, result);
            else
                _content.MapToSourceSnapshots(this, span, result);
            return new ReadOnlyCollection<SnapshotSpan>(result);
        }

        public override ReadOnlyCollection<SnapshotSpan> MapToSourceSnapshots(Span span)
        {
            return MapToSourceSnapshots(span, _fillInMappingMode);
        }

        public override ReadOnlyCollection<SnapshotSpan> MapToSourceSnapshotsForRead(Span span)
        {
            return MapToSourceSnapshots(span, false);
        }

        public override ReadOnlyCollection<Span> MapFromSourceSnapshot(SnapshotSpan span)
        {
            if (span.Snapshot != SourceSnapshot)
                throw new ArgumentException("The span does not belong to a source snapshot of the projection snapshot");
            var result = new FrugalList<Span>();
            _content.MapFromSourceSnapshot(span, result);
            return new ReadOnlyCollection<Span>(result);
        }

        internal override ReadOnlyCollection<SnapshotPoint> MapInsertionPointToSourceSnapshots(int position, ITextBuffer excludedBuffer)
        {
            return new ReadOnlyCollection<SnapshotPoint>(_content.MapInsertionPointToSourceSnapshots(this, position));
        }

        internal override ReadOnlyCollection<SnapshotSpan> MapReplacementSpanToSourceSnapshots(Span replacementSpan, ITextBuffer excludedBuffer)
        {
            return MapToSourceSnapshots(replacementSpan, false);
        }
    }
}