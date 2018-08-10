using System;
using System.Collections.Generic;
using System.Windows;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Operations;

namespace ModernApplicationFramework.Text.Ui.Editor.DragDrop
{
    public abstract class DropHandlerBase : IDropHandler
    {
        protected DropHandlerBase(ITextView wpfTextView, IEditorOperations editorOperations)
        {
            TextView = wpfTextView ?? throw new ArgumentNullException(nameof(wpfTextView));
            EditorOperations = editorOperations ?? throw new ArgumentNullException(nameof(editorOperations));
        }

        public virtual DragDropPointerEffects HandleDragStarted(DragDropInfo dragDropInfo)
        {
            return GetDragDropEffect(dragDropInfo);
        }

        public virtual DragDropPointerEffects HandleDraggingOver(DragDropInfo dragDropInfo)
        {
            return GetDragDropEffect(dragDropInfo);
        }

        public virtual DragDropPointerEffects HandleDataDropped(DragDropInfo dragDropInfo)
        {
            if (dragDropInfo == null)
                throw new ArgumentNullException(nameof(dragDropInfo));
            var selection = TextView.Selection;
            var dropPointerEffects = DragDropPointerEffects.None;
            var virtualBufferPosition = dragDropInfo.VirtualBufferPosition;
            var text = ExtractText(dragDropInfo);
            var isReversed = selection.IsReversed;
            var flag1 = (dragDropInfo.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey;
            var flag2 = (dragDropInfo.AllowedEffects & DragDropEffects.Copy) == DragDropEffects.Copy;
            var textSnapshot = TextView.TextSnapshot;
            var trackingPoint = textSnapshot.CreateTrackingPoint(virtualBufferPosition.Position, PointTrackingMode.Negative);
            var trackingSpanList = new List<ITrackingSpan>();
            foreach (var selectedSpan in selection.SelectedSpans)
                trackingSpanList.Add(textSnapshot.CreateTrackingSpan(selectedSpan, SpanTrackingMode.EdgeExclusive));
            PerformPreEditActions(dragDropInfo);
            if (!selection.IsEmpty)
                selection.Clear();
            var offset = 0;
            if (dragDropInfo.VirtualBufferPosition.IsInVirtualSpace)
                offset = EditorOperations.GetWhitespaceForVirtualSpace(dragDropInfo.VirtualBufferPosition).Length;
            bool successfulEdit;
            if (flag1 & flag2)
            {
                successfulEdit = InsertText(virtualBufferPosition, text);
                if (successfulEdit)
                    dropPointerEffects = DragDropPointerEffects.Copy;
            }
            else
            {
                successfulEdit = !dragDropInfo.IsInternal ? InsertText(virtualBufferPosition, text) : MoveText(virtualBufferPosition, trackingSpanList, text);
                if (successfulEdit)
                    dropPointerEffects = DragDropPointerEffects.Move;
            }
            if (dropPointerEffects != DragDropPointerEffects.None)
            {
                var insertionPoint = trackingPoint.GetPoint(TextView.TextSnapshot);
                if (offset != 0)
                    insertionPoint = insertionPoint.Add(offset);
                SelectText(insertionPoint, text.Length, dragDropInfo, isReversed);
            }
            PerformPostEditActions(dragDropInfo, successfulEdit);
            return dropPointerEffects;
        }

        public virtual void HandleDragCanceled()
        {
        }

        public virtual bool IsDropEnabled(DragDropInfo dragDropInfo)
        {
            if (dragDropInfo == null)
                throw new ArgumentNullException(nameof(dragDropInfo));
            if ((dragDropInfo.AllowedEffects & DragDropEffects.Copy) != DragDropEffects.Copy && (dragDropInfo.AllowedEffects & DragDropEffects.Move) != DragDropEffects.Move)
                return false;
            return !TextView.Options.GetOptionValue(DefaultTextViewOptions.ViewProhibitUserInputId);
        }

        protected ITextView TextView { get; }

        protected IEditorOperations EditorOperations { get; }

        protected abstract string ExtractText(DragDropInfo dragDropInfo);

        protected abstract void PerformPreEditActions(DragDropInfo dragDropInfo);

        protected abstract void PerformPostEditActions(DragDropInfo dragDropInfo, bool successfulEdit);

        protected virtual void SelectText(SnapshotPoint insertionPoint, int dataLength, DragDropInfo dragDropInfo, bool reverse)
        {
            if (dragDropInfo == null)
                throw new ArgumentNullException(nameof(dragDropInfo));
            var virtualSnapshotPoint1 = new VirtualSnapshotPoint(insertionPoint);
            var virtualSnapshotPoint2 = new VirtualSnapshotPoint(insertionPoint.Add(dataLength));
            if (dragDropInfo.IsInternal & reverse)
                EditorOperations.SelectAndMoveCaret(virtualSnapshotPoint2, virtualSnapshotPoint1, TextSelectionMode.Stream);
            else
                EditorOperations.SelectAndMoveCaret(virtualSnapshotPoint1, virtualSnapshotPoint2, TextSelectionMode.Stream);
        }

        protected virtual DragDropPointerEffects GetDragDropEffect(DragDropInfo dragDropInfo)
        {
            if (dragDropInfo == null)
                throw new ArgumentNullException(nameof(dragDropInfo));
            if (TextView.TextBuffer.IsReadOnly(dragDropInfo.VirtualBufferPosition.TranslateTo(TextView.TextSnapshot).Position))
                return DragDropPointerEffects.None;
            if ((dragDropInfo.AllowedEffects & DragDropEffects.Copy) == DragDropEffects.Copy && (dragDropInfo.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey)
                return DragDropPointerEffects.Copy | DragDropPointerEffects.Track;
            if ((dragDropInfo.AllowedEffects & DragDropEffects.Move) == DragDropEffects.Move && (dragDropInfo.KeyStates & DragDropKeyStates.ShiftKey) == DragDropKeyStates.ShiftKey || (dragDropInfo.AllowedEffects & DragDropEffects.Move) == DragDropEffects.Move)
                return DragDropPointerEffects.Move | DragDropPointerEffects.Track;
            return (dragDropInfo.AllowedEffects & DragDropEffects.Copy) == DragDropEffects.Copy ? DragDropPointerEffects.Copy | DragDropPointerEffects.Track : DragDropPointerEffects.None;
        }

        protected virtual bool InsertText(VirtualSnapshotPoint position, string data)
        {
            TextView.Caret.MoveTo(position.TranslateTo(TextView.TextSnapshot));
            return EditorOperations.InsertText(data);
        }

        protected virtual bool MoveText(VirtualSnapshotPoint position, IList<ITrackingSpan> selectionSpans, string data)
        {
            var textSnapshot = TextView.TextSnapshot;
            position = position.TranslateTo(textSnapshot);
            var trackingPoint = textSnapshot.CreateTrackingPoint(position.Position, PointTrackingMode.Negative);
            if (!DeleteSpans(selectionSpans))
                return false;
            TextView.Caret.MoveTo(new VirtualSnapshotPoint(trackingPoint.GetPoint(TextView.TextSnapshot), position.VirtualSpaces));
            return EditorOperations.InsertText(data);
        }

        protected bool DeleteSpans(IList<ITrackingSpan> spans)
        {
            if (spans == null)
                throw new ArgumentNullException(nameof(spans));
            var textSnapshot = TextView.TextSnapshot;
            using (var edit = TextView.TextBuffer.CreateEdit())
            {
                foreach (var span in spans)
                {
                    if (!edit.Delete(span.GetSpan(textSnapshot)))
                        return false;
                }
                edit.Apply();
                if (edit.Canceled)
                    return false;
            }
            return true;
        }
    }
}