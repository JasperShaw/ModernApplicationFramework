using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    internal class DefaultBoxTextDropHandler : DefaultTextDropHandler
    {
        private VirtualSnapshotPoint _boxStart;
        private VirtualSnapshotPoint _boxEnd;

        public DefaultBoxTextDropHandler(ITextView wpfTextView, /*ITextUndoHistory undoHistory,*/ IEditorOperations editorOperations)
            : base(wpfTextView, /* undoHistory,*/ editorOperations)
        {
        }

        protected override string ExtractText(DragDropInfo dragDropInfo)
        {
            return DataObjectManager.ExtractText(dragDropInfo.Data);
        }

        protected override void SelectText(SnapshotPoint insertionPoint, int dataLength, DragDropInfo dragDropInfo, bool reverse)
        {
            if (reverse)
                EditorOperations.SelectAndMoveCaret(_boxEnd, _boxStart, TextSelectionMode.Box);
            else
                EditorOperations.SelectAndMoveCaret(_boxStart, _boxEnd, TextSelectionMode.Box);
        }

        protected override bool MoveText(VirtualSnapshotPoint position, IList<ITrackingSpan> selectionSpans, string data)
        {
            ITrackingPoint trackingPoint = TextView.TextSnapshot.CreateTrackingPoint((int)position.Position, PointTrackingMode.Negative);
            if (!DeleteSpans(selectionSpans))
                return false;
            TextView.Caret.MoveTo(new VirtualSnapshotPoint(trackingPoint.GetPoint(TextView.TextSnapshot), position.VirtualSpaces));
            return EditorOperations.InsertTextAsBox(data, out _boxStart, out _boxEnd);
        }

        protected override bool InsertText(VirtualSnapshotPoint position, string data)
        {
            TextView.Caret.MoveTo(position.TranslateTo(TextView.TextSnapshot));
            return EditorOperations.InsertTextAsBox(data, out _boxStart, out _boxEnd);
        }
    }
}