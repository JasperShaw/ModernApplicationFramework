using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.Text
{
    internal class HighFidelityTrackingSpan : TrackingSpan
    {
        private VersionSpanHistory _cachedSpan;
        private readonly TrackingFidelityMode _fidelity;

        internal HighFidelityTrackingSpan(ITextVersion version, Span span, SpanTrackingMode spanTrackingMode, TrackingFidelityMode fidelity)
            : base(version, span, spanTrackingMode)
        {
            if (fidelity != TrackingFidelityMode.UndoRedo && fidelity != TrackingFidelityMode.Backward)
                throw new ArgumentOutOfRangeException(nameof(fidelity));
            List<VersionNumberPosition> noninvertibleStartHistory = null;
            List<VersionNumberPosition> noninvertibleEndHistory = null;
            if (fidelity == TrackingFidelityMode.UndoRedo && version.VersionNumber > 0)
            {
                noninvertibleStartHistory = new List<VersionNumberPosition>();
                noninvertibleEndHistory = new List<VersionNumberPosition>();
                if (version.VersionNumber != version.ReiteratedVersionNumber)
                {
                    noninvertibleStartHistory.Add(new VersionNumberPosition(version.ReiteratedVersionNumber, span.Start));
                    noninvertibleEndHistory.Add(new VersionNumberPosition(version.ReiteratedVersionNumber, span.End));
                }
                noninvertibleStartHistory.Add(new VersionNumberPosition(version.VersionNumber, span.Start));
                noninvertibleEndHistory.Add(new VersionNumberPosition(version.VersionNumber, span.End));
            }
            _cachedSpan = new VersionSpanHistory(version, span, noninvertibleStartHistory, noninvertibleEndHistory);
            _fidelity = fidelity;
        }

        public override ITextBuffer TextBuffer => _cachedSpan.Version.TextBuffer;

        public override TrackingFidelityMode TrackingFidelity => _fidelity;

        protected override Span TrackSpan(ITextVersion targetVersion)
        {
            VersionSpanHistory cachedSpan = _cachedSpan;
            if (targetVersion == cachedSpan.Version)
                return cachedSpan.Span;
            PointTrackingMode trackingMode1 = TrackingMode == SpanTrackingMode.EdgeExclusive || TrackingMode == SpanTrackingMode.EdgePositive ? PointTrackingMode.Positive : PointTrackingMode.Negative;
            PointTrackingMode trackingMode2 = TrackingMode == SpanTrackingMode.EdgeExclusive || TrackingMode == SpanTrackingMode.EdgeNegative ? PointTrackingMode.Negative : PointTrackingMode.Positive;
            List<VersionNumberPosition> noninvertibleStartHistory = cachedSpan.NoninvertibleStartHistory;
            List<VersionNumberPosition> noninvertibleEndHistory = cachedSpan.NoninvertibleEndHistory;
            Span span;
            if (targetVersion.VersionNumber > cachedSpan.Version.VersionNumber)
            {
                int num = HighFidelityTrackingPoint.TrackPositionForwardInTime(trackingMode1, _fidelity, ref noninvertibleStartHistory, cachedSpan.Span.Start, cachedSpan.Version, targetVersion);
                span = Span.FromBounds(num, Math.Max(num, HighFidelityTrackingPoint.TrackPositionForwardInTime(trackingMode2, _fidelity, ref noninvertibleEndHistory, cachedSpan.Span.End, cachedSpan.Version, targetVersion)));
                _cachedSpan = new VersionSpanHistory(targetVersion, span, noninvertibleStartHistory, noninvertibleEndHistory);
            }
            else
            {
                int num = HighFidelityTrackingPoint.TrackPositionBackwardInTime(trackingMode1, _fidelity == TrackingFidelityMode.Backward ? noninvertibleStartHistory : null, cachedSpan.Span.Start, cachedSpan.Version, targetVersion);
                span = Span.FromBounds(num, Math.Max(num, HighFidelityTrackingPoint.TrackPositionBackwardInTime(trackingMode2, _fidelity == TrackingFidelityMode.Backward ? noninvertibleEndHistory : null, cachedSpan.Span.End, cachedSpan.Version, targetVersion)));
            }
            return span;
        }

        public override string ToString()
        {
            VersionSpanHistory cachedSpan = _cachedSpan;
            StringBuilder stringBuilder = new StringBuilder("*");
            stringBuilder.Append(ToString(cachedSpan.Version, cachedSpan.Span, TrackingMode));
            if (cachedSpan.NoninvertibleStartHistory != null)
            {
                stringBuilder.Append("[Start");
                foreach (VersionNumberPosition versionNumberPosition in cachedSpan.NoninvertibleStartHistory)
                    stringBuilder.Append(string.Format(CultureInfo.CurrentCulture, "V{0}@{1}", versionNumberPosition.VersionNumber, versionNumberPosition.Position));
                stringBuilder.Append("]");
            }
            if (cachedSpan.NoninvertibleEndHistory != null)
            {
                stringBuilder.Append("[End");
                foreach (VersionNumberPosition versionNumberPosition in cachedSpan.NoninvertibleEndHistory)
                    stringBuilder.Append(string.Format(CultureInfo.CurrentCulture, "V{0}@{1}", versionNumberPosition.VersionNumber, versionNumberPosition.Position));
                stringBuilder.Append("]");
            }
            return stringBuilder.ToString();
        }

        private class VersionSpanHistory
        {
            public VersionSpanHistory(ITextVersion version, Span span, List<VersionNumberPosition> noninvertibleStartHistory, List<VersionNumberPosition> noninvertibleEndHistory)
            {
                Version = version;
                Span = span;
                NoninvertibleStartHistory = noninvertibleStartHistory;
                NoninvertibleEndHistory = noninvertibleEndHistory;
            }

            public ITextVersion Version { get; }

            public Span Span { get; }

            public List<VersionNumberPosition> NoninvertibleStartHistory { get; }

            public List<VersionNumberPosition> NoninvertibleEndHistory { get; }
        }
    }
}