using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.IntraTextAdornmentSupport
{
    internal class ElisionViewModel : ITextViewModel
    {
        internal readonly IElisionBuffer visualBuffer;
        internal const string IntraTextAdornmentBufferKey = "IntraTextAdornmentBuffer";

        internal ElisionViewModel(ITextDataModel dataModel, IProjectionBufferFactoryService factory)
        {
            DataModel = dataModel;
            var currentSnapshot = dataModel.DataBuffer.CurrentSnapshot;
            visualBuffer = factory.CreateElisionBuffer(null, new NormalizedSnapshotSpanCollection(new SnapshotSpan(currentSnapshot, 0, currentSnapshot.Length)), ElisionBufferOptions.None);
            Properties =
                new PropertyCollection {["IntraTextAdornmentBuffer"] = VisualBuffer};
        }

        public ITextDataModel DataModel { get; }

        public ITextBuffer DataBuffer => DataModel.DataBuffer;

        public ITextBuffer EditBuffer => DataModel.DataBuffer;

        public ITextBuffer VisualBuffer => visualBuffer;

        public PropertyCollection Properties { get; }

        public void Dispose()
        {
        }

        public SnapshotPoint GetNearestPointInVisualBuffer(SnapshotPoint editBufferPoint)
        {
            if (editBufferPoint.Snapshot.TextBuffer != EditBuffer)
                throw new ArgumentException("The given point must be on the edit buffer.", nameof(editBufferPoint));
            editBufferPoint = editBufferPoint.TranslateTo(editBufferPoint.Snapshot.TextBuffer.CurrentSnapshot, PointTrackingMode.Positive);
            return visualBuffer.CurrentSnapshot.MapFromSourceSnapshotToNearest(editBufferPoint);
        }

        public SnapshotPoint GetNearestPointInVisualSnapshot(SnapshotPoint editBufferPoint, ITextSnapshot targetVisualSnapshot, PointTrackingMode trackingMode)
        {
            if (targetVisualSnapshot == null)
                throw new ArgumentNullException(nameof(targetVisualSnapshot));
            if (targetVisualSnapshot.TextBuffer != VisualBuffer)
                throw new ArgumentException("The given snapshot must be on the visual buffer.", nameof(targetVisualSnapshot));
            var elisionSnapshot = (IElisionSnapshot)targetVisualSnapshot;
            if (editBufferPoint.Snapshot != elisionSnapshot.SourceSnapshot)
                editBufferPoint = editBufferPoint.TranslateTo(elisionSnapshot.SourceSnapshot, trackingMode);
            return elisionSnapshot.MapFromSourceSnapshotToNearest(editBufferPoint);
        }

        public bool IsPointInVisualBuffer(SnapshotPoint editBufferPoint, PositionAffinity affinity)
        {
            if (editBufferPoint.Snapshot.TextBuffer != EditBuffer)
                throw new ArgumentException("The given point must be on the edit buffer.", nameof(editBufferPoint));
            editBufferPoint = editBufferPoint.TranslateTo(editBufferPoint.Snapshot.TextBuffer.CurrentSnapshot, PointTrackingMode.Positive);
            return visualBuffer.CurrentSnapshot.MapFromSourceSnapshot(editBufferPoint, affinity).HasValue;
        }
    }
}