namespace ModernApplicationFramework.Text.Data
{
    public interface ITrackingSpan
    {
        ITextBuffer TextBuffer { get; }

        TrackingFidelityMode TrackingFidelity { get; }

        SpanTrackingMode TrackingMode { get; }

        SnapshotPoint GetEndPoint(ITextSnapshot snapshot);

        SnapshotSpan GetSpan(ITextSnapshot snapshot);

        Span GetSpan(ITextVersion version);

        SnapshotPoint GetStartPoint(ITextSnapshot snapshot);

        string GetText(ITextSnapshot snapshot);
    }
}