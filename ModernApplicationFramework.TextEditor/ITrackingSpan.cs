namespace ModernApplicationFramework.TextEditor
{
    public interface ITrackingSpan
    {
        ITextBuffer TextBuffer { get; }

        SpanTrackingMode TrackingMode { get; }

        TrackingFidelityMode TrackingFidelity { get; }

        SnapshotSpan GetSpan(ITextSnapshot snapshot);

        Span GetSpan(ITextVersion version);

        string GetText(ITextSnapshot snapshot);

        SnapshotPoint GetStartPoint(ITextSnapshot snapshot);

        SnapshotPoint GetEndPoint(ITextSnapshot snapshot);
    }
}