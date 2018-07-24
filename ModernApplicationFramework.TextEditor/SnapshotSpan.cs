using System;

namespace ModernApplicationFramework.TextEditor
{
    public struct SnapshotSpan
    {
        public SnapshotSpan(ITextSnapshot snapshot, Span span)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));
            if (span.End > snapshot.Length)
                throw new ArgumentOutOfRangeException(nameof(span));
            Start = new SnapshotPoint(snapshot, span.Start);
            Length = span.Length;
        }

        public SnapshotSpan(ITextSnapshot snapshot, int start, int length)
        {
            this = new SnapshotSpan(snapshot, new Span(start, length));
        }

        public SnapshotSpan(SnapshotPoint start, SnapshotPoint end)
        {
            if (start.Snapshot == null || end.Snapshot == null)
                throw new ArgumentException();
            if (start.Snapshot != end.Snapshot)
                throw new ArgumentException();
            if (end.Position < start.Position)
                throw new ArgumentOutOfRangeException(nameof(end));
            Start = start;
            Length = end - start;
        }

        public SnapshotSpan(SnapshotPoint start, int length)
        {
            if (length < 0 || start.Position + length > start.Snapshot.Length)
                throw new ArgumentOutOfRangeException(nameof(length));
            Start = start;
            Length = length;
        }

        public static implicit operator Span(SnapshotSpan snapshotSpan)
        {
            return snapshotSpan.Span;
        }

        public ITextSnapshot Snapshot => Start.Snapshot;

        public string GetText()
        {
            return Snapshot.GetText(Span);
        }

        public SnapshotSpan TranslateTo(ITextSnapshot targetSnapshot, SpanTrackingMode spanTrackingMode)
        {
            if (targetSnapshot == Snapshot)
                return this;
            if (targetSnapshot == null)
                throw new ArgumentNullException(nameof(targetSnapshot));
            if (targetSnapshot.TextBuffer != Start.Snapshot.TextBuffer)
                throw new ArgumentException();
            var span = targetSnapshot.Version.VersionNumber > Snapshot.Version.VersionNumber
                ? Tracking.TrackSpanForwardInTime(spanTrackingMode, Span, Snapshot.Version, targetSnapshot.Version)
                : Tracking.TrackSpanBackwardInTime(spanTrackingMode, Span, Snapshot.Version, targetSnapshot.Version);
            return new SnapshotSpan(targetSnapshot, span);
        }

        public Span Span => new Span(Start, Length);

        public SnapshotPoint Start { get; }

        public SnapshotPoint End => Start + Length;

        public int Length { get; }

        public bool IsEmpty => Length == 0;

        public bool Contains(int position)
        {
            return Span.Contains(position);
        }

        public bool Contains(SnapshotPoint point)
        {
            EnsureSnapshot(point.Snapshot);
            return Span.Contains(point.Position);
        }

        public bool Contains(Span simpleSpan)
        {
            return Span.Contains(simpleSpan);
        }

        public bool Contains(SnapshotSpan snapshotSpan)
        {
            EnsureSnapshot(snapshotSpan.Snapshot);
            return Span.Contains(snapshotSpan.Span);
        }

        public bool OverlapsWith(Span simpleSpan)
        {
            return Span.OverlapsWith(simpleSpan);
        }

        public bool OverlapsWith(SnapshotSpan snapshotSpan)
        {
            EnsureSnapshot(snapshotSpan.Snapshot);
            return Span.OverlapsWith(snapshotSpan.Span);
        }

        public SnapshotSpan? Overlap(Span simpleSpan)
        {
            Span? nullable = Span.Overlap(simpleSpan);
            if (nullable.HasValue)
                return new SnapshotSpan(Snapshot, nullable.Value);
            return new SnapshotSpan?();
        }

        public SnapshotSpan? Overlap(SnapshotSpan snapshotSpan)
        {
            EnsureSnapshot(snapshotSpan.Snapshot);
            return Overlap(snapshotSpan.Span);
        }

        public bool IntersectsWith(Span simpleSpan)
        {
            return Span.IntersectsWith(simpleSpan);
        }

        public bool IntersectsWith(SnapshotSpan snapshotSpan)
        {
            EnsureSnapshot(snapshotSpan.Snapshot);
            return Span.IntersectsWith(snapshotSpan.Span);
        }

        public SnapshotSpan? Intersection(Span simpleSpan)
        {
            Span? nullable = Span.Intersection(simpleSpan);
            if (nullable.HasValue)
                return new SnapshotSpan(Snapshot, nullable.Value);
            return new SnapshotSpan?();
        }

        public SnapshotSpan? Intersection(SnapshotSpan snapshotSpan)
        {
            EnsureSnapshot(snapshotSpan.Snapshot);
            return Intersection(snapshotSpan.Span);
        }

        public override int GetHashCode()
        {
            if (Snapshot == null)
                return 0;
            return Span.GetHashCode() ^ Snapshot.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is SnapshotSpan span)
                return span == this;
            return false;
        }

        public static bool operator ==(SnapshotSpan left, SnapshotSpan right)
        {
            if (left.Snapshot == right.Snapshot)
                return left.Span == right.Span;
            return false;
        }

        public static bool operator !=(SnapshotSpan left, SnapshotSpan right)
        {
            return !(left == right);
        }

        private void EnsureSnapshot(ITextSnapshot requestedSnapshot)
        {
            if (Snapshot != requestedSnapshot)
                throw new ArgumentException();
        }
    }
}