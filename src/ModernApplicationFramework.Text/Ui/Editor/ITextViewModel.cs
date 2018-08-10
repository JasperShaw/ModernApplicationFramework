using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface ITextViewModel : IPropertyOwner, IDisposable
    {
        ITextBuffer DataBuffer { get; }
        ITextDataModel DataModel { get; }

        ITextBuffer EditBuffer { get; }

        ITextBuffer VisualBuffer { get; }

        SnapshotPoint GetNearestPointInVisualBuffer(SnapshotPoint editBufferPoint);

        SnapshotPoint GetNearestPointInVisualSnapshot(SnapshotPoint editBufferPoint, ITextSnapshot targetVisualSnapshot,
            PointTrackingMode trackingMode);

        bool IsPointInVisualBuffer(SnapshotPoint editBufferPoint, PositionAffinity affinity);
    }
}