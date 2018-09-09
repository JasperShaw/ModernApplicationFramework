using System.Linq;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Text.Ui.Differencing;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.Differencing
{
    internal class DifferenceMapEditToData : IMapEditToData
    {
        private readonly IDifferenceViewer _viewer;

        public DifferenceMapEditToData(IDifferenceViewer viewer)
        {
            _viewer = viewer;
        }

        public int MapEditToData(int editPoint)
        {
            IProjectionSnapshot textSnapshot = (IProjectionSnapshot)_viewer.InlineView.TextSnapshot;
            SnapshotPoint snapshotPoint = new SnapshotPoint(textSnapshot, editPoint);
            SnapshotPoint sourceSnapshot = textSnapshot.MapToSourceSnapshot(editPoint, PositionAffinity.Successor);
            ITextBuffer dataBuffer = _viewer.InlineView.TextViewModel.DataBuffer;
            if (sourceSnapshot.Snapshot.TextBuffer == dataBuffer)
                return sourceSnapshot;
            int num = 0;
            foreach (SnapshotSpan sourceSpan in textSnapshot.GetSourceSpans())
            {
                if (sourceSpan.Snapshot.TextBuffer == dataBuffer && num >= snapshotPoint)
                    return sourceSpan.Start;
                num += sourceSpan.Length;
            }
            ITextSnapshot snapshot = textSnapshot.SourceSnapshots.First(s => s.TextBuffer == dataBuffer);
            return new SnapshotPoint(snapshot, snapshot.Length);
        }

        public int MapDataToEdit(int dataPoint)
        {
            IProjectionSnapshot textSnapshot = (IProjectionSnapshot)_viewer.InlineView.TextSnapshot;
            ITextBuffer dataBuffer = _viewer.InlineView.TextViewModel.DataBuffer;
            return textSnapshot.MapFromSourceSnapshot(new SnapshotPoint(textSnapshot.SourceSnapshots.First(s => s.TextBuffer == dataBuffer), dataPoint), PositionAffinity.Successor).Value;
        }
    }
}