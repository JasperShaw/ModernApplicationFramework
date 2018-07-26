using System;
using System.Globalization;

namespace ModernApplicationFramework.TextEditor
{
    internal abstract class TrackingPoint : ITrackingPoint
    {
        protected TrackingPoint(ITextVersion version, int position, PointTrackingMode trackingMode)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));
            if (position < 0 | position > version.Length)
                throw new ArgumentOutOfRangeException(nameof(position));
            switch (trackingMode)
            {
                case PointTrackingMode.Positive:
                case PointTrackingMode.Negative:
                    TrackingMode = trackingMode;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        public abstract ITextBuffer TextBuffer { get; }

        public PointTrackingMode TrackingMode { get; }

        public abstract TrackingFidelityMode TrackingFidelity { get; }

        public int GetPosition(ITextVersion version)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));
            if (version.TextBuffer != TextBuffer)
                throw new ArgumentException();
            return TrackPosition(version);
        }

        public int GetPosition(ITextSnapshot snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));
            if (snapshot.TextBuffer != TextBuffer)
                throw new ArgumentException();
            return TrackPosition(snapshot.Version);
        }

        public SnapshotPoint GetPoint(ITextSnapshot snapshot)
        {
            return new SnapshotPoint(snapshot, GetPosition(snapshot));
        }

        public char GetCharacter(ITextSnapshot snapshot)
        {
            return GetPoint(snapshot).GetChar();
        }

        protected abstract int TrackPosition(ITextVersion targetVersion);

        protected static string PointTrackingModeToString(PointTrackingMode trackingMode)
        {
            return trackingMode != PointTrackingMode.Positive ? "←" : "→";
        }

        protected static string ToString(ITextVersion version, int position, PointTrackingMode trackingMode)
        {
            return string.Format(CultureInfo.CurrentCulture, "V{0} {2}@{1}", version.VersionNumber, position, PointTrackingModeToString(trackingMode));
        }
    }
}