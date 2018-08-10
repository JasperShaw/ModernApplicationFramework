using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface ITextViewModel : IPropertyOwner, IDisposable
    {
        ITextDataModel DataModel { get; }

        ITextBuffer DataBuffer { get; }

        ITextBuffer EditBuffer { get; }

        ITextBuffer VisualBuffer { get; }

        bool IsPointInVisualBuffer(SnapshotPoint editBufferPoint, PositionAffinity affinity);

        SnapshotPoint GetNearestPointInVisualBuffer(SnapshotPoint editBufferPoint);

        SnapshotPoint GetNearestPointInVisualSnapshot(SnapshotPoint editBufferPoint, ITextSnapshot targetVisualSnapshot, PointTrackingMode trackingMode);
    }
}
