using System.Collections.Generic;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Editor.DragDrop;
using ModernApplicationFramework.Text.Ui.Operations;

namespace ModernApplicationFramework.Modules.Editor.DragDrop
{
    internal class DefaultBoxTextDropHandler : DefaultTextDropHandler
    {
        private VirtualSnapshotPoint _boxEnd;
        private VirtualSnapshotPoint _boxStart;

        public DefaultBoxTextDropHandler(ITextView wpfTextView, /*ITextUndoHistory undoHistory,*/
            IEditorOperations editorOperations)
            : base(wpfTextView, /* undoHistory,*/ editorOperations)
        {
        }

        protected override string ExtractText(DragDropInfo dragDropInfo)
        {
            return DataObjectManager.ExtractText(dragDropInfo.Data);
        }

        protected override bool InsertText(VirtualSnapshotPoint position, string data)
        {
            TextView.Caret.MoveTo(position.TranslateTo(TextView.TextSnapshot));
            return EditorOperations.InsertTextAsBox(data, out _boxStart, out _boxEnd);
        }

        protected override bool MoveText(VirtualSnapshotPoint position, IList<ITrackingSpan> selectionSpans,
            string data)
        {
            var trackingPoint =
                TextView.TextSnapshot.CreateTrackingPoint(position.Position, PointTrackingMode.Negative);
            if (!DeleteSpans(selectionSpans))
                return false;
            TextView.Caret.MoveTo(new VirtualSnapshotPoint(trackingPoint.GetPoint(TextView.TextSnapshot),
                position.VirtualSpaces));
            return EditorOperations.InsertTextAsBox(data, out _boxStart, out _boxEnd);
        }

        protected override void SelectText(SnapshotPoint insertionPoint, int dataLength, DragDropInfo dragDropInfo,
            bool reverse)
        {
            if (reverse)
                EditorOperations.SelectAndMoveCaret(_boxEnd, _boxStart, TextSelectionMode.Box);
            else
                EditorOperations.SelectAndMoveCaret(_boxStart, _boxEnd, TextSelectionMode.Box);
        }
    }
}