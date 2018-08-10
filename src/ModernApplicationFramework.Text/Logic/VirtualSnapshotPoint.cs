using System;
using System.Globalization;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic
{
    public struct VirtualSnapshotPoint : IComparable<VirtualSnapshotPoint>
    {
        public VirtualSnapshotPoint(SnapshotPoint position)
        {
            Position = position;
            VirtualSpaces = 0;
        }

        public VirtualSnapshotPoint(ITextSnapshot snapshot, int position)
        {
            Position = new SnapshotPoint(snapshot, position);
            VirtualSpaces = 0;
        }

        public VirtualSnapshotPoint(SnapshotPoint position, int virtualSpaces)
        {
            if (virtualSpaces < 0)
                throw new ArgumentOutOfRangeException(nameof(virtualSpaces));
            if (virtualSpaces != 0 && position.GetContainingLine().End != position)
                virtualSpaces = 0;
            Position = position;
            VirtualSpaces = virtualSpaces;
        }

        public VirtualSnapshotPoint(ITextSnapshotLine line, int offset)
        {
            if (line == null)
                throw new ArgumentNullException(nameof(line));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (offset <= line.Length)
            {
                Position = line.Start + offset;
                VirtualSpaces = 0;
            }
            else
            {
                Position = line.End;
                VirtualSpaces = offset - line.Length;
            }
        }

        public SnapshotPoint Position { get; }

        public int VirtualSpaces { get; }

        public bool IsInVirtualSpace => VirtualSpaces > 0;

        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ VirtualSpaces.GetHashCode();
        }

        public VirtualSnapshotPoint TranslateTo(ITextSnapshot snapshot)
        {
            return TranslateTo(snapshot, PointTrackingMode.Positive);
        }

        public VirtualSnapshotPoint TranslateTo(ITextSnapshot snapshot, PointTrackingMode trackingMode)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));
            int versionNumber1 = snapshot.Version.VersionNumber;
            SnapshotPoint position1 = Position;
            int versionNumber2 = position1.Snapshot.Version.VersionNumber;
            if (versionNumber1 < versionNumber2)
                throw new ArgumentException("VirtualSnapshotPoints can only be translated to later snapshots", nameof(snapshot));
            ITextSnapshot textSnapshot = snapshot;
            position1 = Position;
            ITextSnapshot snapshot1 = position1.Snapshot;
            if (textSnapshot == snapshot1)
                return this;
            if (IsInVirtualSpace)
            {
                position1 = Position;
                SnapshotPoint position2 = position1.TranslateTo(snapshot, PointTrackingMode.Positive);
                int virtualSpaces = VirtualSpaces == 0 || !(position2.GetContainingLine().End == position2) || CharacterDeleted(Position, snapshot) ? 0 : VirtualSpaces;
                return new VirtualSnapshotPoint(position2, virtualSpaces);
            }
            position1 = Position;
            return new VirtualSnapshotPoint(position1.TranslateTo(snapshot, trackingMode));
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0}+{1}", Position, VirtualSpaces);
        }

        public override bool Equals(object obj)
        {
            if (obj is VirtualSnapshotPoint point)
                return point == this;
            return false;
        }

        public static bool operator ==(VirtualSnapshotPoint left, VirtualSnapshotPoint right)
        {
            if (left.Position == right.Position)
                return left.VirtualSpaces == right.VirtualSpaces;
            return false;
        }

        public static bool operator !=(VirtualSnapshotPoint left, VirtualSnapshotPoint right)
        {
            return !(left == right);
        }

        public static bool operator >(VirtualSnapshotPoint left, VirtualSnapshotPoint right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(VirtualSnapshotPoint left, VirtualSnapshotPoint right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator <(VirtualSnapshotPoint left, VirtualSnapshotPoint right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(VirtualSnapshotPoint left, VirtualSnapshotPoint right)
        {
            return left.CompareTo(right) <= 0;
        }

        public int CompareTo(VirtualSnapshotPoint other)
        {
            int num = Position.CompareTo(other.Position);
            if (num != 0)
                return num;
            return VirtualSpaces - other.VirtualSpaces;
        }

        private static bool CharacterDeleted(SnapshotPoint position, ITextSnapshot snapshot)
        {
            int position1 = position.Position;
            for (ITextVersion textVersion = position.Snapshot.Version; textVersion.VersionNumber != snapshot.Version.VersionNumber; textVersion = textVersion.Next)
            {
                foreach (ITextChange change in textVersion.Changes)
                {
                    if (change.NewPosition <= position1)
                    {
                        if (change.NewPosition + change.OldLength > position1)
                            return true;
                        position1 += change.Delta;
                    }
                    else
                        break;
                }
            }
            return false;
        }
    }
}