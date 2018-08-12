using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.Text
{
    internal class ForwardFidelityTrackingPoint : TrackingPoint
    {
        private VersionPosition _cachedPosition;

        public override ITextBuffer TextBuffer => _cachedPosition.Version.TextBuffer;

        public override TrackingFidelityMode TrackingFidelity => TrackingFidelityMode.Forward;

        public ForwardFidelityTrackingPoint(ITextVersion version, int position, PointTrackingMode trackingMode)
            : base(version, position, trackingMode)
        {
            _cachedPosition = new VersionPosition(version, position);
        }

        public override string ToString()
        {
            var cachedPosition = _cachedPosition;
            return ToString(cachedPosition.Version, cachedPosition.Position, TrackingMode);
        }

        protected override int TrackPosition(ITextVersion targetVersion)
        {
            var cachedPosition = _cachedPosition;
            int position;
            if (targetVersion == cachedPosition.Version)
            {
                position = cachedPosition.Position;
            }
            else if (targetVersion.VersionNumber > cachedPosition.Version.VersionNumber)
            {
                position = Tracking.TrackPositionForwardInTime(TrackingMode, cachedPosition.Position,
                    cachedPosition.Version, targetVersion);
                _cachedPosition = new VersionPosition(targetVersion, position);
            }
            else
            {
                position = Tracking.TrackPositionBackwardInTime(TrackingMode, cachedPosition.Position,
                    cachedPosition.Version, targetVersion);
            }

            return position;
        }

        private class VersionPosition
        {
            public int Position { get; }

            public ITextVersion Version { get; }

            public VersionPosition(ITextVersion version, int position)
            {
                Version = version;
                Position = position;
            }
        }
    }
}