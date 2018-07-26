using System;

namespace ModernApplicationFramework.TextEditor
{
    internal class TextVersion : ITextVersion
    {
        private readonly TextImageVersion _textImageVersion;

        public TextVersion(ITextBuffer textBuffer, TextImageVersion imageVersion)
        {
            TextBuffer = textBuffer ?? throw new ArgumentNullException(nameof(textBuffer));
            _textImageVersion = imageVersion ?? throw new ArgumentNullException(nameof(imageVersion));
        }

        internal TextVersion CreateNext(INormalizedTextChangeCollection changes, int newLength = -1, int reiteratedVersionNumber = -1)
        {
            if (Next != null)
                throw new InvalidOperationException("Not allowed to CreateNext twice");
            TextVersion textVersion = new TextVersion(TextBuffer, _textImageVersion.CreateNext(reiteratedVersionNumber, newLength, changes));
            Next = textVersion;
            return textVersion;
        }

        internal void SetLength(int length)
        {
            _textImageVersion.SetLength(length);
        }

        internal void SetChanges(INormalizedTextChangeCollection changes)
        {
            _textImageVersion.SetChanges(changes);
        }

        public ITextBuffer TextBuffer { get; }

        public int VersionNumber => ImageVersion.VersionNumber;

        public int ReiteratedVersionNumber => ImageVersion.ReiteratedVersionNumber;

        public ITextVersion Next { get; private set; }

        public INormalizedTextChangeCollection Changes => ImageVersion.Changes;

        public int Length => ImageVersion.Length;

        public ITextImageVersion ImageVersion => _textImageVersion;

        public ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode)
        {
            return new ForwardFidelityTrackingPoint(this, position, trackingMode);
        }

        public ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode, TrackingFidelityMode trackingFidelity)
        {
            if (trackingFidelity == TrackingFidelityMode.Forward)
                return new ForwardFidelityTrackingPoint(this, position, trackingMode);
            return new HighFidelityTrackingPoint(this, position, trackingMode, trackingFidelity);
        }

        public ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode)
        {
            if (trackingMode == SpanTrackingMode.Custom)
                throw new ArgumentOutOfRangeException(nameof(trackingMode));
            return new ForwardFidelityTrackingSpan(this, new Span(start, length), trackingMode);
        }

        public ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode, TrackingFidelityMode trackingFidelity)
        {
            return CreateTrackingSpan(new Span(start, length), trackingMode, trackingFidelity);
        }

        public ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode)
        {
            if (trackingMode == SpanTrackingMode.Custom)
                throw new ArgumentOutOfRangeException(nameof(trackingMode));
            return new ForwardFidelityTrackingSpan(this, span, trackingMode);
        }

        public ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode, TrackingFidelityMode trackingFidelity)
        {
            if (trackingMode == SpanTrackingMode.Custom)
                throw new ArgumentOutOfRangeException(nameof(trackingMode));
            if (trackingFidelity == TrackingFidelityMode.Forward)
                return new ForwardFidelityTrackingSpan(this, span, trackingMode);
            return new HighFidelityTrackingSpan(this, span, trackingMode, trackingFidelity);
        }

        public ITrackingSpan CreateCustomTrackingSpan(Span span, TrackingFidelityMode trackingFidelity, object customState, CustomTrackToVersion behavior)
        {
            if (behavior == null)
                throw new ArgumentNullException(nameof(behavior));
            if (trackingFidelity != TrackingFidelityMode.Forward)
                throw new NotImplementedException();
            return new ForwardFidelityCustomTrackingSpan(this, span, customState, behavior);
        }

        public override string ToString()
        {
            return $"V{VersionNumber} (r{ReiteratedVersionNumber})";
        }
    }
}