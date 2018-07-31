using System;

namespace ModernApplicationFramework.TextEditor
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
