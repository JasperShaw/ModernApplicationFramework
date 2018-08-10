using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface IVerticalScrollBar
    {
        event EventHandler TrackSpanChanged;
        IScrollMap Map { get; }

        double ThumbHeight { get; }

        double TrackSpanBottom { get; }

        double TrackSpanHeight { get; }

        double TrackSpanTop { get; }

        SnapshotPoint GetBufferPositionOfYCoordinate(double y);

        double GetYCoordinateOfBufferPosition(SnapshotPoint bufferPosition);

        double GetYCoordinateOfScrollMapPosition(double scrollMapPosition);
    }
}