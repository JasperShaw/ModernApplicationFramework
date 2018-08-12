using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    internal sealed class PreviewTextViewModel : ITextViewModel
    {
        public readonly ITextView SourceView;

        public ITextBuffer DataBuffer { get; }

        public ITextDataModel DataModel { get; }

        public ITextBuffer EditBuffer => SourceView.TextBuffer;

        public PropertyCollection Properties { get; internal set; }

        public ITextBuffer VisualBuffer => EditBuffer;

        public PreviewTextViewModel(ITextView sourceView)
        {
            SourceView = sourceView;
            DataModel = SourceView.TextViewModel.DataModel;
            DataBuffer = SourceView.TextViewModel.DataBuffer;
        }

        public void Dispose()
        {
        }

        public SnapshotPoint GetNearestPointInVisualBuffer(SnapshotPoint editBufferPoint)
        {
            return editBufferPoint;
        }

        public SnapshotPoint GetNearestPointInVisualSnapshot(SnapshotPoint editBufferPoint,
            ITextSnapshot targetVisualSnapshot, PointTrackingMode trackingMode)
        {
            return editBufferPoint.TranslateTo(targetVisualSnapshot, trackingMode);
        }

        public bool IsPointInVisualBuffer(SnapshotPoint editBufferPoint, PositionAffinity affinity)
        {
            return true;
        }
    }
}