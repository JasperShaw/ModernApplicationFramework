using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;

namespace ModernApplicationFramework.Text.Ui.Text
{
    public struct Selection : IEquatable<Selection>
    {
        public static readonly Selection Invalid;

        public Selection(VirtualSnapshotPoint insertionPoint, PositionAffinity insertionPointAffinity = PositionAffinity.Successor)
        {
            this = new Selection(insertionPoint, insertionPoint, insertionPoint, insertionPointAffinity);
        }

        public Selection(SnapshotPoint insertionPoint, PositionAffinity insertionPointAffinity = PositionAffinity.Successor)
        {
            this = new Selection(new VirtualSnapshotPoint(insertionPoint), new VirtualSnapshotPoint(insertionPoint), new VirtualSnapshotPoint(insertionPoint), insertionPointAffinity);
        }

        public Selection(VirtualSnapshotSpan extent, bool isReversed = false)
        {
            if (isReversed)
            {
                AnchorPoint = extent.End;
                ActivePoint = InsertionPoint = extent.Start;
                InsertionPointAffinity = PositionAffinity.Successor;
            }
            else
            {
                AnchorPoint = extent.Start;
                ActivePoint = InsertionPoint = extent.End;
                InsertionPointAffinity = PositionAffinity.Predecessor;
            }
        }

        public Selection(SnapshotSpan extent, bool isReversed = false)
        {
            this = new Selection(new VirtualSnapshotSpan(extent), isReversed);
        }

        public Selection(VirtualSnapshotPoint anchorPoint, VirtualSnapshotPoint activePoint)
        {
            this = new Selection(activePoint, anchorPoint, activePoint, anchorPoint < activePoint ? PositionAffinity.Predecessor : PositionAffinity.Successor);
        }

        public Selection(SnapshotPoint anchorPoint, SnapshotPoint activePoint)
        {
            this = new Selection(new VirtualSnapshotPoint(anchorPoint), new VirtualSnapshotPoint(activePoint));
        }

        public Selection(VirtualSnapshotPoint insertionPoint, VirtualSnapshotPoint anchorPoint, VirtualSnapshotPoint activePoint, PositionAffinity insertionPointAffinity = PositionAffinity.Successor)
        {
            var position1 = insertionPoint.Position;
            var snapshot1 = position1.Snapshot;
            position1 = anchorPoint.Position;
            var snapshot2 = position1.Snapshot;
            if (snapshot1 == snapshot2)
            {
                var position2 = insertionPoint.Position;
                var snapshot3 = position2.Snapshot;
                position2 = activePoint.Position;
                var snapshot4 = position2.Snapshot;
                if (snapshot3 == snapshot4)
                {
                    InsertionPoint = insertionPoint;
                    AnchorPoint = anchorPoint;
                    ActivePoint = activePoint;
                    InsertionPointAffinity = insertionPointAffinity;
                    return;
                }
            }
            throw new ArgumentException("All points must be on the same snapshot.");
        }

        public Selection(SnapshotPoint insertionPoint, SnapshotPoint anchorPoint, SnapshotPoint activePoint, PositionAffinity insertionPointAffinity = PositionAffinity.Successor)
        {
            this = new Selection(new VirtualSnapshotPoint(insertionPoint), new VirtualSnapshotPoint(anchorPoint), new VirtualSnapshotPoint(activePoint), insertionPointAffinity);
        }

        public bool IsValid
        {
            get
            {
                if (this != Invalid)
                    return InsertionPoint.Position.Snapshot != null;
                return false;
            }
        }

        public VirtualSnapshotPoint InsertionPoint { get; }

        public VirtualSnapshotPoint AnchorPoint { get; }

        public VirtualSnapshotPoint ActivePoint { get; }

        public PositionAffinity InsertionPointAffinity { get; }

        public bool IsReversed => ActivePoint < AnchorPoint;

        public bool IsEmpty => ActivePoint == AnchorPoint;

        public VirtualSnapshotPoint Start => !IsReversed ? AnchorPoint : ActivePoint;

        public VirtualSnapshotPoint End => !IsReversed ? ActivePoint : AnchorPoint;

        public VirtualSnapshotSpan Extent => new VirtualSnapshotSpan(Start, End);

        public override int GetHashCode()
        {
            var hashCode1 = (uint)InsertionPoint.GetHashCode();
            var num1 = (uint)((ushort.MaxValue & (int)hashCode1) << 16) | (4294901760U & hashCode1) >> 16;
            var virtualSnapshotPoint = AnchorPoint;
            var hashCode2 = virtualSnapshotPoint.GetHashCode();
            virtualSnapshotPoint = ActivePoint;
            var hashCode3 = virtualSnapshotPoint.GetHashCode();
            var num2 = hashCode2 ^ hashCode3 ^ (int)num1;
            var num4 = InsertionPointAffinity != PositionAffinity.Predecessor ? 10172014 : 4122013;
            return num2 ^ num4;
        }

        public override bool Equals(object obj)
        {
            if (obj is Selection selection)
                return Equals(selection);
            return false;
        }

        public bool Equals(Selection other)
        {
            if (ActivePoint == other.ActivePoint && AnchorPoint == other.AnchorPoint && InsertionPoint == other.InsertionPoint)
                return InsertionPointAffinity == other.InsertionPointAffinity;
            return false;
        }

        public static bool operator ==(Selection left, Selection right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Selection left, Selection right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"Ins:{InsertionPoint} Anc:{AnchorPoint} Act:{ActivePoint} Aff:{InsertionPointAffinity}";
        }
    }
}