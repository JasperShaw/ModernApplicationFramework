namespace ModernApplicationFramework.Text.Data
{
    public interface ITextVersion
    {
        INormalizedTextChangeCollection Changes { get; }

        ITextImageVersion ImageVersion { get; }

        int Length { get; }
        ITextVersion Next { get; }

        int ReiteratedVersionNumber { get; }

        ITextBuffer TextBuffer { get; }

        int VersionNumber { get; }

        ITrackingSpan CreateCustomTrackingSpan(Span span, TrackingFidelityMode trackingFidelity, object customState,
            CustomTrackToVersion behavior);

        ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode);

        ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode,
            TrackingFidelityMode trackingFidelity);

        ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode);

        ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode,
            TrackingFidelityMode trackingFidelity);

        ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode);

        ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode,
            TrackingFidelityMode trackingFidelity);
    }
}