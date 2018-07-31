﻿using System;
using System.Globalization;

namespace ModernApplicationFramework.TextEditor
{
    public struct VirtualSnapshotSpan
    {
        public VirtualSnapshotSpan(SnapshotSpan snapshotSpan)
        {
            Start = new VirtualSnapshotPoint(snapshotSpan.Start);
            End = new VirtualSnapshotPoint(snapshotSpan.End);
        }

        public VirtualSnapshotSpan(VirtualSnapshotPoint start, VirtualSnapshotPoint end)
        {
            if (start.Position.Snapshot == null || end.Position.Snapshot == null)
                throw new ArgumentException("The VirtualSnapshotPoint is not initialized.");
            if (start.Position.Snapshot != end.Position.Snapshot)
                throw new ArgumentException("The specified VirtualSnapshotPoints belong to different ITextSnapshots.");
            if (end < start)
                throw new ArgumentOutOfRangeException(nameof(end));
            Start = start;
            End = end;
        }

        public VirtualSnapshotPoint Start { get; }

        public VirtualSnapshotPoint End { get; }

        public ITextSnapshot Snapshot => Start.Position.Snapshot;

        public int Length
        {
            get
            {
                VirtualSnapshotPoint virtualSnapshotPoint1 = Start;
                SnapshotPoint position1 = virtualSnapshotPoint1.Position;
                virtualSnapshotPoint1 = End;
                SnapshotPoint position2 = virtualSnapshotPoint1.Position;
                if (!(position1 == position2))
                    return SnapshotSpan.Length + End.VirtualSpaces;
                VirtualSnapshotPoint virtualSnapshotPoint2 = End;
                int virtualSpaces1 = virtualSnapshotPoint2.VirtualSpaces;
                virtualSnapshotPoint2 = Start;
                int virtualSpaces2 = virtualSnapshotPoint2.VirtualSpaces;
                return virtualSpaces1 - virtualSpaces2;
            }
        }

        public string GetText()
        {
            return SnapshotSpan.GetText();
        }

        public SnapshotSpan SnapshotSpan
        {
            get
            {
                VirtualSnapshotPoint virtualSnapshotPoint = Start;
                SnapshotPoint position1 = virtualSnapshotPoint.Position;
                virtualSnapshotPoint = End;
                SnapshotPoint position2 = virtualSnapshotPoint.Position;
                return new SnapshotSpan(position1, position2);
            }
        }

        public bool IsInVirtualSpace
        {
            get
            {
                if (!Start.IsInVirtualSpace)
                    return End.IsInVirtualSpace;
                return true;
            }
        }

        public bool IsEmpty => Start == End;

        public bool Contains(VirtualSnapshotPoint virtualPoint)
        {
            if (virtualPoint >= Start)
                return virtualPoint < End;
            return false;
        }

        public bool Contains(VirtualSnapshotSpan virtualSpan)
        {
            if (virtualSpan.Start >= Start)
                return virtualSpan.End <= End;
            return false;
        }

        public bool OverlapsWith(VirtualSnapshotSpan virtualSpan)
        {
            return (Start > virtualSpan.Start ? Start : virtualSpan.Start) < (End < virtualSpan.End ? End : virtualSpan.End);
        }

        public VirtualSnapshotSpan? Overlap(VirtualSnapshotSpan virtualSpan)
        {
            VirtualSnapshotPoint start = Start > virtualSpan.Start ? Start : virtualSpan.Start;
            VirtualSnapshotPoint end = End < virtualSpan.End ? End : virtualSpan.End;
            if (start < end)
                return new VirtualSnapshotSpan(start, end);
            return new VirtualSnapshotSpan?();
        }

        public bool IntersectsWith(VirtualSnapshotSpan virtualSpan)
        {
            if (virtualSpan.Start <= End)
                return virtualSpan.End >= Start;
            return false;
        }

        public VirtualSnapshotSpan? Intersection(VirtualSnapshotSpan virtualSpan)
        {
            VirtualSnapshotPoint start = Start > virtualSpan.Start ? Start : virtualSpan.Start;
            VirtualSnapshotPoint end = End < virtualSpan.End ? End : virtualSpan.End;
            if (start <= end)
                return new VirtualSnapshotSpan(start, end);
            return new VirtualSnapshotSpan?();
        }

        public override int GetHashCode()
        {
            VirtualSnapshotPoint virtualSnapshotPoint = Start;
            int hashCode1 = virtualSnapshotPoint.GetHashCode();
            virtualSnapshotPoint = End;
            int hashCode2 = virtualSnapshotPoint.GetHashCode();
            return hashCode1 ^ hashCode2;
        }

        public VirtualSnapshotSpan TranslateTo(ITextSnapshot snapshot)
        {
            return TranslateTo(snapshot, SpanTrackingMode.EdgePositive);
        }

        public VirtualSnapshotSpan TranslateTo(ITextSnapshot snapshot, SpanTrackingMode trackingMode)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));
            int versionNumber1 = snapshot.Version.VersionNumber;
            VirtualSnapshotPoint virtualSnapshotPoint1 = Start;
            int versionNumber2 = virtualSnapshotPoint1.Position.Snapshot.Version.VersionNumber;
            if (versionNumber1 < versionNumber2)
                throw new ArgumentException("VirtualSnapshotSpans can only be translated to later snapshots", nameof(snapshot));
            ITextSnapshot textSnapshot = snapshot;
            virtualSnapshotPoint1 = Start;
            ITextSnapshot snapshot1 = virtualSnapshotPoint1.Position.Snapshot;
            if (textSnapshot == snapshot1)
                return this;
            virtualSnapshotPoint1 = Start;
            VirtualSnapshotPoint virtualSnapshotPoint2 = virtualSnapshotPoint1.TranslateTo(snapshot, GetStartPointMode(trackingMode));
            virtualSnapshotPoint1 = End;
            VirtualSnapshotPoint end = virtualSnapshotPoint1.TranslateTo(snapshot, GetEndPointMode(trackingMode));
            if (virtualSnapshotPoint2 <= end)
                return new VirtualSnapshotSpan(virtualSnapshotPoint2, end);
            return new VirtualSnapshotSpan(virtualSnapshotPoint2, virtualSnapshotPoint2);
        }

        private static PointTrackingMode GetStartPointMode(SpanTrackingMode trackingMode)
        {
            return trackingMode == SpanTrackingMode.EdgeInclusive || trackingMode == SpanTrackingMode.EdgeNegative ? PointTrackingMode.Negative : PointTrackingMode.Positive;
        }

        private static PointTrackingMode GetEndPointMode(SpanTrackingMode trackingMode)
        {
            return trackingMode == SpanTrackingMode.EdgeInclusive || trackingMode == SpanTrackingMode.EdgePositive ? PointTrackingMode.Positive : PointTrackingMode.Negative;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "({0},{1})", Start, End);
        }

        public override bool Equals(object obj)
        {
            if (obj is VirtualSnapshotSpan span)
                return span == this;
            return false;
        }

        public static bool operator ==(VirtualSnapshotSpan left, VirtualSnapshotSpan right)
        {
            if (left.Start == right.Start)
                return left.End == right.End;
            return false;
        }

        public static bool operator !=(VirtualSnapshotSpan left, VirtualSnapshotSpan right)
        {
            return !(left == right);
        }
    }
}