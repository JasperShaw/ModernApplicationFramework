using System;

namespace ModernApplicationFramework.Text.Data
{
    public struct SnapshotPoint : IComparable<SnapshotPoint>
    {
        public int Position { get; }

        public ITextSnapshot Snapshot { get; }


        public SnapshotPoint(ITextSnapshot snapshot, int position)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));
            if (position < 0 || position > snapshot.Length)
                throw new ArgumentOutOfRangeException(nameof(position));
            Snapshot = snapshot;
            Position = position;
        }

        public static SnapshotPoint operator +(SnapshotPoint point, int offset)
        {
            return point.Add(offset);
        }

        public static bool operator ==(SnapshotPoint left, SnapshotPoint right)
        {
            if (left.Snapshot == right.Snapshot)
                return left.Position == right.Position;
            return false;
        }

        public static bool operator >(SnapshotPoint left, SnapshotPoint right)
        {
            return left.CompareTo(right) > 0;
        }

        public static implicit operator int(SnapshotPoint snapshotPoint)
        {
            return snapshotPoint.Position;
        }

        public static bool operator !=(SnapshotPoint left, SnapshotPoint right)
        {
            return !(left == right);
        }

        public static bool operator <(SnapshotPoint left, SnapshotPoint right)
        {
            return left.CompareTo(right) < 0;
        }

        public static SnapshotPoint operator -(SnapshotPoint point, int offset)
        {
            return point.Add(-offset);
        }

        public static int operator -(SnapshotPoint start, SnapshotPoint other)
        {
            if (start.Snapshot != other.Snapshot)
                throw new ArgumentException();
            return start.Position - other.Position;
        }

        public SnapshotPoint Add(int offset)
        {
            return new SnapshotPoint(Snapshot, Position + offset);
        }


        public int CompareTo(SnapshotPoint other)
        {
            if (Snapshot != other.Snapshot)
                throw new ArgumentException();
            return Position.CompareTo(other.Position);
        }

        public int Difference(SnapshotPoint other)
        {
            return other - this;
        }

        public override bool Equals(object obj)
        {
            if (obj is SnapshotPoint point)
                return point == this;
            return false;
        }

        public char GetChar()
        {
            return Snapshot[Position];
        }

        public ITextSnapshotLine GetContainingLine()
        {
            return Snapshot.GetLineFromPosition(Position);
        }

        public override int GetHashCode()
        {
            if (Snapshot == null)
                return 0;
            return Position.GetHashCode() ^ Snapshot.GetHashCode();
        }

        public SnapshotPoint Subtract(int offset)
        {
            return Add(-offset);
        }

        public SnapshotPoint TranslateTo(ITextSnapshot targetSnapshot, PointTrackingMode trackingMode)
        {
            if (targetSnapshot == Snapshot)
                return this;
            if (targetSnapshot == null)
                throw new ArgumentNullException(nameof(targetSnapshot));
            if (targetSnapshot.TextBuffer != Snapshot.TextBuffer)
                throw new ArgumentException();
            var position = targetSnapshot.Version.VersionNumber > Snapshot.Version.VersionNumber
                ? Tracking.TrackPositionForwardInTime(trackingMode, Position, Snapshot.Version, targetSnapshot.Version)
                : Tracking.TrackPositionBackwardInTime(trackingMode, Position, Snapshot.Version,
                    targetSnapshot.Version);
            return new SnapshotPoint(targetSnapshot, position);
        }
    }
}