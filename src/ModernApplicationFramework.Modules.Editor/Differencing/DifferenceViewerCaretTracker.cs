using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Logic.Differencing;
using ModernApplicationFramework.Text.Ui.Differencing;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Operations;

namespace ModernApplicationFramework.Modules.Editor.Differencing
{
    internal class DifferenceViewerCaretTracker
    {
        private IDifferenceBuffer _differenceBuffer;
        private readonly ITextView _textView;
        private readonly IEditorOperations _operations;
        private VirtualSnapshotPoint? _savedSelectionAnchor;
        private VirtualSnapshotPoint? _savedSelectionActive;
        private TextSelectionMode _savedSelectionMode;
        private SnapshotPoint? _savedFirstVisiblePoint;
        private double _savedOffset;
        private bool _savedCheckToShiftView;

        public DifferenceViewerCaretTracker(ITextView textView, IDifferenceBuffer differenceBuffer, IEditorOperations operations)
        {
            _textView = textView;
            _differenceBuffer = differenceBuffer;
            _operations = operations;
        }

        public void SaveCaretAndSelection(ISnapshotDifference current)
        {
            if (current == null)
                return;
            _savedSelectionAnchor = MapToSource(current, _textView.Selection.AnchorPoint);
            _savedSelectionActive = MapToSource(current, _textView.Selection.ActivePoint);
            _savedSelectionMode = _textView.Selection.Mode;
            if (_textView.TextViewLines == null)
                return;
            var firstVisibleLine = _textView.TextViewLines.FirstVisibleLine;
            var sourceSnapshot = current.MapToSourceSnapshot(firstVisibleLine.Start);
            _savedFirstVisiblePoint = sourceSnapshot;
            _savedOffset = firstVisibleLine.Top - _textView.ViewportTop;
            _savedCheckToShiftView = false;
            if (sourceSnapshot.Snapshot != current.RightBufferSnapshot)
                return;
            current.FindMatchOrDifference(sourceSnapshot, out _, out var difference);
            _savedCheckToShiftView = difference == null || sourceSnapshot.GetContainingLine().LineNumber != difference.Right.Start;
        }

        private static VirtualSnapshotPoint MapToSource(ISnapshotDifference difference, VirtualSnapshotPoint point)
        {
            var snapshot = (IProjectionSnapshot)point.Position.Snapshot;
            var position = snapshot.MapToSourceSnapshot(point.Position, PositionAffinity.Successor);
            if (position.Snapshot.TextBuffer != difference.LeftBufferSnapshot.TextBuffer && position.Snapshot.TextBuffer != difference.RightBufferSnapshot.TextBuffer)
            {
                var sourceSpans = snapshot.GetSourceSpans();
                position = sourceSpans[sourceSpans.Count - 3].End;
            }
            return new VirtualSnapshotPoint(position, point.VirtualSpaces);
        }

        private static VirtualSnapshotPoint MapToInline(ISnapshotDifference difference, VirtualSnapshotPoint point)
        {
            return new VirtualSnapshotPoint(difference.MapToInlineSnapshot(point.Position), point.VirtualSpaces);
        }

        public void RestoreCaretAndSelection(ISnapshotDifference after)
        {
            if (_savedSelectionActive.HasValue && _savedSelectionAnchor.HasValue)
            {
                _operations.SelectAndMoveCaret(MapToInline(after, _savedSelectionAnchor.Value), MapToInline(after, _savedSelectionActive.Value), _savedSelectionMode, EnsureSpanVisibleOptions.None);
                if (_savedFirstVisiblePoint.HasValue)
                {
                    var snapshot = after.TranslateToSnapshot(_savedFirstVisiblePoint.Value);
                    var snapshotPoint = after.MapToInlineSnapshot(snapshot);
                    if (_savedCheckToShiftView)
                    {
                        after.FindMatchOrDifference(snapshot, out _, out var difference);
                        if (difference != null && snapshot.GetContainingLine().LineNumber == difference.Right.Start)
                        {
                            var lineNumber = snapshotPoint.GetContainingLine().LineNumber;
                            if (lineNumber > 0)
                            {
                                snapshotPoint = snapshotPoint.Snapshot.GetLineFromLineNumber(lineNumber - 1).End;
                                _savedOffset = 0.0;
                            }
                        }
                    }
                    _textView.DisplayTextLineContainingBufferPosition(snapshotPoint.TranslateTo(_textView.TextSnapshot, PointTrackingMode.Positive), _savedOffset, ViewRelativePosition.Top);
                }
            }
            _savedSelectionActive = _savedSelectionAnchor = new VirtualSnapshotPoint?();
            _savedFirstVisiblePoint = new SnapshotPoint?();
            _savedCheckToShiftView = false;
        }
    }
}