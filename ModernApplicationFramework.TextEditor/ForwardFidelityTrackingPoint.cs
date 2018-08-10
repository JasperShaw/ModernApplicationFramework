using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.TextEditor
{
    internal class ForwardFidelityTrackingPoint : TrackingPoint
    {
        private VersionPosition _cachedPosition;

        public ForwardFidelityTrackingPoint(ITextVersion version, int position, PointTrackingMode trackingMode)
            : base(version, position, trackingMode)
        {
            _cachedPosition = new VersionPosition(version, position);
        }

        public override ITextBuffer TextBuffer => _cachedPosition.Version.TextBuffer;

        public override TrackingFidelityMode TrackingFidelity => TrackingFidelityMode.Forward;

        protected override int TrackPosition(ITextVersion targetVersion)
        {
            VersionPosition cachedPosition = _cachedPosition;
            int position;
            if (targetVersion == cachedPosition.Version)
                position = cachedPosition.Position;
            else if (targetVersion.VersionNumber > cachedPosition.Version.VersionNumber)
            {
                position = Tracking.TrackPositionForwardInTime(TrackingMode, cachedPosition.Position, cachedPosition.Version, targetVersion);
                _cachedPosition = new VersionPosition(targetVersion, position);
            }
            else
                position = Tracking.TrackPositionBackwardInTime(TrackingMode, cachedPosition.Position, cachedPosition.Version, targetVersion);
            return position;
        }

        public override string ToString()
        {
            var cachedPosition = _cachedPosition;
            return ToString(cachedPosition.Version, cachedPosition.Position, TrackingMode);
        }

        private class VersionPosition
        {
            public VersionPosition(ITextVersion version, int position)
            {
                this.Version = version;
                this.Position = position;
            }

            public ITextVersion Version { get; }

            public int Position { get; }
        }
    }
}