using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface IVerticalScrollBar
    {
        IScrollMap Map { get; }

        double GetYCoordinateOfBufferPosition(SnapshotPoint bufferPosition);

        double GetYCoordinateOfScrollMapPosition(double scrollMapPosition);

        SnapshotPoint GetBufferPositionOfYCoordinate(double y);

        double ThumbHeight { get; }

        double TrackSpanTop { get; }

        double TrackSpanBottom { get; }

        double TrackSpanHeight { get; }

        event EventHandler TrackSpanChanged;
    }
}