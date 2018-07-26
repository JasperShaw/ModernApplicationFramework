namespace ModernApplicationFramework.TextEditor
{
    public interface ITextVersion
    {
        ITextVersion Next { get; }

        INormalizedTextChangeCollection Changes { get; }

        ITextImageVersion ImageVersion { get; }

        int Length { get; }

        ITextBuffer TextBuffer { get; }

        int VersionNumber { get; }

        int ReiteratedVersionNumber { get; }

        ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode);

        ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode, TrackingFidelityMode trackingFidelity);

        ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode);

        ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode, TrackingFidelityMode trackingFidelity);

        ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode);

        ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode, TrackingFidelityMode trackingFidelity);

        ITrackingSpan CreateCustomTrackingSpan(Span span, TrackingFidelityMode trackingFidelity, object customState, CustomTrackToVersion behavior);
    }
}