namespace ModernApplicationFramework.Text.Data
{
    public interface ITrackingPoint
    {
        ITextBuffer TextBuffer { get; }

        TrackingFidelityMode TrackingFidelity { get; }

        PointTrackingMode TrackingMode { get; }

        char GetCharacter(ITextSnapshot snapshot);

        SnapshotPoint GetPoint(ITextSnapshot snapshot);

        int GetPosition(ITextSnapshot snapshot);

        int GetPosition(ITextVersion version);
    }
}