namespace ModernApplicationFramework.Text.Data
{
    public interface ITrackingPoint
    {
        ITextBuffer TextBuffer { get; }

        PointTrackingMode TrackingMode { get; }

        TrackingFidelityMode TrackingFidelity { get; }

        SnapshotPoint GetPoint(ITextSnapshot snapshot);

        int GetPosition(ITextSnapshot snapshot);

        int GetPosition(ITextVersion version);

        char GetCharacter(ITextSnapshot snapshot);
    }
}