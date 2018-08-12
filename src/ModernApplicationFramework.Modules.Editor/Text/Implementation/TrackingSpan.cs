using System;
using System.Globalization;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.Text.Implementation
{
    internal abstract class TrackingSpan : ITrackingSpan
    {
        protected TrackingSpan(ITextVersion version, Span span, SpanTrackingMode trackingMode)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));
            if (span.End > version.Length)
                throw new ArgumentOutOfRangeException(nameof(span));
            switch (trackingMode)
            {
                case SpanTrackingMode.EdgeExclusive:
                case SpanTrackingMode.EdgeInclusive:
                case SpanTrackingMode.EdgePositive:
                case SpanTrackingMode.EdgeNegative:
                case SpanTrackingMode.Custom:
                    TrackingMode = trackingMode;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        public abstract ITextBuffer TextBuffer { get; }

        public SpanTrackingMode TrackingMode { get; }

        public abstract TrackingFidelityMode TrackingFidelity { get; }

        public Span GetSpan(ITextVersion version)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));
            if (version.TextBuffer != TextBuffer)
                throw new ArgumentException();
            return TrackSpan(version);
        }

        public SnapshotSpan GetSpan(ITextSnapshot snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));
            if (snapshot.TextBuffer != TextBuffer)
                throw new ArgumentException();
            return new SnapshotSpan(snapshot, TrackSpan(snapshot.Version));
        }

        public SnapshotPoint GetStartPoint(ITextSnapshot snapshot)
        {
            var span = GetSpan(snapshot);
            return new SnapshotPoint(snapshot, span.Start);
        }

        public SnapshotPoint GetEndPoint(ITextSnapshot snapshot)
        {
            var span = GetSpan(snapshot);
            return new SnapshotPoint(snapshot, span.End);
        }

        public string GetText(ITextSnapshot snapshot)
        {
            return GetSpan(snapshot).GetText();
        }

        protected abstract Span TrackSpan(ITextVersion targetVersion);

        protected static string SpanTrackingModeToString(SpanTrackingMode trackingMode)
        {
            switch (trackingMode)
            {
                case SpanTrackingMode.EdgeExclusive:
                    return "→←";
                case SpanTrackingMode.EdgeInclusive:
                    return "←→";
                case SpanTrackingMode.EdgePositive:
                    return "→→";
                case SpanTrackingMode.EdgeNegative:
                    return "←←";
                case SpanTrackingMode.Custom:
                    return "custom";
                default:
                    return "??";
            }
        }

        protected static string ToString(ITextVersion version, Span span, SpanTrackingMode trackingMode)
        {
            return string.Format(CultureInfo.CurrentCulture, "V{0} {2}@{1}", version.VersionNumber, span.ToString(), SpanTrackingModeToString(trackingMode));
        }
    }
}