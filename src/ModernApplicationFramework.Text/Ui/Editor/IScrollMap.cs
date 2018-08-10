using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface IScrollMap : IVerticalFractionMap
    {
        double GetCoordinateAtBufferPosition(SnapshotPoint bufferPosition);

        bool AreElisionsExpanded { get; }

        SnapshotPoint GetBufferPositionAtCoordinate(double coordinate);

        double Start { get; }

        double End { get; }

        double ThumbSize { get; }
    }
}