using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface IScrollMap : IVerticalFractionMap
    {
        bool AreElisionsExpanded { get; }

        double End { get; }

        double Start { get; }

        double ThumbSize { get; }

        SnapshotPoint GetBufferPositionAtCoordinate(double coordinate);
        double GetCoordinateAtBufferPosition(SnapshotPoint bufferPosition);
    }
}