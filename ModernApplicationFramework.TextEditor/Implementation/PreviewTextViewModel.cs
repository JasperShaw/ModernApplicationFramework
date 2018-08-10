using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal sealed class PreviewTextViewModel : ITextViewModel
    {
        public readonly ITextView SourceView;

        public PreviewTextViewModel(ITextView sourceView)
        {
            SourceView = sourceView;
            DataModel = SourceView.TextViewModel.DataModel;
            DataBuffer = SourceView.TextViewModel.DataBuffer;
        }

        public ITextDataModel DataModel { get; }

        public ITextBuffer DataBuffer { get; }

        public ITextBuffer EditBuffer => SourceView.TextBuffer;

        public ITextBuffer VisualBuffer => EditBuffer;

        public void Dispose()
        {
        }

        public PropertyCollection Properties { get; internal set; }

        public SnapshotPoint GetNearestPointInVisualBuffer(SnapshotPoint editBufferPoint)
        {
            return editBufferPoint;
        }

        public SnapshotPoint GetNearestPointInVisualSnapshot(SnapshotPoint editBufferPoint, ITextSnapshot targetVisualSnapshot, PointTrackingMode trackingMode)
        {
            return editBufferPoint.TranslateTo(targetVisualSnapshot, trackingMode);
        }

        public bool IsPointInVisualBuffer(SnapshotPoint editBufferPoint, PositionAffinity affinity)
        {
            return true;
        }
    }
}