namespace ModernApplicationFramework.TextEditor
{
    internal class ForwardFidelityTrackingSpan : TrackingSpan
    {
        private VersionSpan _cachedSpan;

        public ForwardFidelityTrackingSpan(ITextVersion version, Span span, SpanTrackingMode trackingMode)
            : base(version, span, trackingMode)
        {
            _cachedSpan = new VersionSpan(version, span);
        }

        public override ITextBuffer TextBuffer => _cachedSpan.Version.TextBuffer;

        public override TrackingFidelityMode TrackingFidelity => TrackingFidelityMode.Forward;

        protected override Span TrackSpan(ITextVersion targetVersion)
        {
            var cachedSpan = _cachedSpan;
            Span span;
            if (targetVersion == cachedSpan.Version)
                span = cachedSpan.Span;
            else if (targetVersion.VersionNumber > cachedSpan.Version.VersionNumber)
            {
                span = TrackSpanForwardInTime(cachedSpan.Span, cachedSpan.Version, targetVersion);
                _cachedSpan = new VersionSpan(targetVersion, span);
            }
            else
                span = TrackSpanBackwardInTime(cachedSpan.Span, cachedSpan.Version, targetVersion);
            return span;
        }

        protected virtual Span TrackSpanForwardInTime(Span span, ITextVersion currentVersion, ITextVersion targetVersion)
        {
            return Tracking.TrackSpanForwardInTime(TrackingMode, span, currentVersion, targetVersion);
        }

        protected virtual Span TrackSpanBackwardInTime(Span span, ITextVersion currentVersion, ITextVersion targetVersion)
        {
            return Tracking.TrackSpanBackwardInTime(TrackingMode, span, currentVersion, targetVersion);
        }

        public override string ToString()
        {
            var cachedSpan = _cachedSpan;
            return ToString(cachedSpan.Version, cachedSpan.Span, TrackingMode);
        }

        private class VersionSpan
        {
            public VersionSpan(ITextVersion version, Span span)
            {
                Version = version;
                Span = span;
            }

            public ITextVersion Version { get; }

            public Span Span { get; }
        }
    }
}