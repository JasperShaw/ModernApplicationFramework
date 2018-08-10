namespace ModernApplicationFramework.Text.Data
{
    public interface ITextImageVersion
    {
        INormalizedTextChangeCollection Changes { get; }

        object Identifier { get; }

        int Length { get; }
        ITextImageVersion Next { get; }

        int ReiteratedVersionNumber { get; }

        int VersionNumber { get; }

        int TrackTo(VersionedPosition other, PointTrackingMode mode);

        Span TrackTo(VersionedSpan span, SpanTrackingMode mode);
    }
}