namespace ModernApplicationFramework.Text.Data
{
    public interface ITextImageVersion
    {
        ITextImageVersion Next { get; }

        int Length { get; }

        INormalizedTextChangeCollection Changes { get; }

        int VersionNumber { get; }

        int ReiteratedVersionNumber { get; }

        object Identifier { get; }

        int TrackTo(VersionedPosition other, PointTrackingMode mode);

        Span TrackTo(VersionedSpan span, SpanTrackingMode mode);
    }
}