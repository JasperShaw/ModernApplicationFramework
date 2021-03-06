﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Editor.DragDrop;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Text.Ui.Operations;
using ModernApplicationFramework.Text.Ui.Text;

namespace ModernApplicationFramework.Modules.Editor.DragDrop
{
    //TODO: Add undo stuff

    internal class DragDropMouseProcessor : MouseProcessorBase, IDragDropMouseProcessor
    {
        internal bool IsDragDropEnabled;
        internal DragDropStateManager StateManager;
        private readonly GuardedOperations _guardedOperations;
        private readonly DragDropVisualManager _visualManager;
        private readonly ITextView _textView;
        private Point? _mouseDownAnchorPoint;

        public DragDropMouseProcessor(ITextView wpfTextView,
            List<Lazy<IDropHandlerProvider, IDropHandlerMetadata>> dropHandlers,
            IEditorOperations editorOperations, IRtfBuilderService2 rtfBuilderService,
            IClassificationFormatMapService classificationFormatMapService, /*ITextUndoHistory undoHistory,*/
            GuardedOperations guardedOperations)
        {
            if (editorOperations == null)
                throw new ArgumentNullException(nameof(editorOperations));
            if (classificationFormatMapService == null)
                throw new ArgumentNullException(nameof(classificationFormatMapService));
            //if (undoHistory == null)
            //    throw new ArgumentNullException(nameof(undoHistory));
            if (guardedOperations == null)
                throw new ArgumentNullException(nameof(guardedOperations));
            _textView = wpfTextView ?? throw new ArgumentNullException(nameof(wpfTextView));
            _visualManager = new DragDropVisualManager(wpfTextView, classificationFormatMapService);
            StateManager = new DragDropStateManager(wpfTextView, rtfBuilderService,
                new DropHandlerManager(dropHandlers, wpfTextView, guardedOperations), _visualManager, /*undoHistory,*/
                guardedOperations);
            IsDragDropEnabled = _textView.Options.IsDragDropEditingEnabled();
            _mouseDownAnchorPoint = new Point?();
            _guardedOperations = guardedOperations;
            _textView.Options.OptionChanged += (sender, eventArgs) =>
            {
                if (eventArgs.OptionId != DefaultTextViewOptions.DragDropEditingId.Name)
                    return;
                IsDragDropEnabled = _textView.Options.IsDragDropEditingEnabled();
            };
        }

        public void DoPostprocessMouseLeave(MouseEventArgs e)
        {
            if (StateManager.State != DragDropState.MouseDown)
                return;
            _mouseDownAnchorPoint = new Point?();
            StateManager.SetToStart();
        }

        public void DoPostprocessMouseLeftButtonUp(MouseButtonEventArgs e, Point position)
        {
            if (!IsDragDropEnabled || StateManager.State != DragDropState.MouseDown)
                return;
            _mouseDownAnchorPoint = new Point?();
            StateManager.SetToStart();
        }

        public void DoPreprocessDragEnter(DragEventArgs e, Point position)
        {
            var effects = e.Effects;
            if (!HandleDragEnter(CreateDragDropInfo(e, position), ref effects))
                return;
            e.Effects = effects;
            e.Handled = true;
        }

        public void DoPreprocessDragLeave(DragEventArgs e)
        {
            e.Handled = HandleDragLeave();
        }

        public void DoPreprocessDragOver(DragEventArgs e, Point position)
        {
            var effects = e.Effects;
            if (!HandleDragOver(CreateDragDropInfo(e, position), e.Effects, ref effects))
                return;
            e.Effects = effects;
            e.Handled = true;
        }

        public void DoPreprocessDrop(DragEventArgs e, Point position)
        {
            var effects = e.Effects;
            if (!HandleDrop(CreateDragDropInfo(e, position), ref effects))
                return;
            e.Effects = effects;
            e.Handled = true;
        }

        public void DoPreprocessMouseLeftButtonDown(MouseButtonEventArgs e, Point position)
        {
            if (e.ClickCount != 1 || !HandleMouseLeftButtonDown(e.ButtonState == MouseButtonState.Pressed, position, Keyboard.Modifiers))
                return;
            e.Handled = true;
        }

        public void DoPreprocessMouseLeftButtonUp(MouseButtonEventArgs e, Point position)
        {
            if (e.ClickCount != 1 || !HandleMouseLeftButtonUp(position))
                return;
            e.Handled = true;
        }

        public void DoPreprocessMouseMove(MouseEventArgs e, Point position)
        {
            if (FromTouch(e) || !HandleMouseMove(e.LeftButton == MouseButtonState.Pressed, position))
                return;
            e.Handled = true;
        }

        public void DoPreprocessQueryContinueDrag(QueryContinueDragEventArgs e)
        {
            var action = e.Action;
            if (!HandleQueryContinueDrag(e.EscapePressed, e.Action == DragAction.Cancel, ref action))
                return;
            e.Action = action;
            e.Handled = true;
        }

        public override void PostprocessMouseLeave(MouseEventArgs e)
        {
            DoPostprocessMouseLeave(e);
        }

        public override void PostprocessMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            DoPostprocessMouseLeftButtonUp(e, GetClickedPoint(e));
        }

        public override void PreprocessDragEnter(DragEventArgs e)
        {
            DoPreprocessDragEnter(e, GetClickedPoint(e));
        }

        public override void PreprocessDragLeave(DragEventArgs e)
        {
            DoPreprocessDragLeave(e);
        }

        public override void PreprocessDragOver(DragEventArgs e)
        {
            DoPreprocessDragOver(e, GetClickedPoint(e));
        }

        public override void PreprocessDrop(DragEventArgs e)
        {
            DoPreprocessDrop(e, GetClickedPoint(e));
        }

        public override void PreprocessMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            DoPreprocessMouseLeftButtonDown(e, GetClickedPoint(e));
        }

        public override void PreprocessMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            DoPreprocessMouseLeftButtonUp(e, GetClickedPoint(e));
        }

        public override void PreprocessMouseMove(MouseEventArgs e)
        {
            DoPreprocessMouseMove(e, GetClickedPoint(e));
        }

        public override void PreprocessQueryContinueDrag(QueryContinueDragEventArgs e)
        {
            DoPreprocessQueryContinueDrag(e);
        }

        internal bool HandleDragEnter(DragDropInfo dragDropInfo, ref DragDropEffects resultingDragEffects)
        {
            if (!IsDragDropEnabled)
                return false;
            DragDropPointerEffects dropPointerEffects;
            if (StateManager.State == DragDropState.Dragging && StateManager.DropHandler != null)
            {
                dropPointerEffects = _guardedOperations.CallExtensionPoint(
                    () => StateManager.DropHandler.HandleDraggingOver(dragDropInfo), DragDropPointerEffects.None);
                DrawTrackerAndScroll(dragDropInfo, dropPointerEffects);
            }
            else
            {
                dropPointerEffects = StateManager.SetToDragging(dragDropInfo);
                if (StateManager.DropHandler == null)
                    return false;
            }

            resultingDragEffects = ConvertToDragDropEffect(dropPointerEffects);
            return true;
        }

        internal bool HandleDragLeave()
        {
            if (StateManager.State == DragDropState.Canceled)
                return true;
            if (StateManager.State != DragDropState.Dragging)
                return false;
            StateManager.SetToStart();
            return true;
        }

        internal bool HandleDragOver(DragDropInfo dragDropInfo, DragDropEffects effects,
            ref DragDropEffects resultingDragEffects)
        {
            var currentDropHandler = StateManager.DropHandler;
            if (!IsDragDropEnabled || StateManager.State != DragDropState.Dragging || currentDropHandler == null)
                return false;
            var dropPointerEffects =
                _guardedOperations.CallExtensionPoint(() => currentDropHandler.HandleDraggingOver(dragDropInfo),
                    DragDropPointerEffects.None);
            DrawTrackerAndScroll(dragDropInfo, dropPointerEffects);
            resultingDragEffects = ConvertToDragDropEffect(dropPointerEffects);
            return true;
        }

        internal bool HandleDrop(DragDropInfo dragDropInfo, ref DragDropEffects resultingDragEffects)
        {
            if (!IsDragDropEnabled || StateManager.State != DragDropState.Dragging)
                return false;
            var dropped = StateManager.SetToDropped(dragDropInfo);
            if (!dragDropInfo.IsInternal)
                StateManager.SetToStart();
            resultingDragEffects = ConvertToDragDropEffect(dropped);
            if (!_textView.VisualElement.IsKeyboardFocused)
                Keyboard.Focus(_textView.VisualElement);
            return true;
        }

        internal bool HandleMouseLeftButtonDown(bool buttonPressed, Point viewRelativeMousePosition, ModifierKeys modifierKeys = ModifierKeys.None)
        {
            if ((modifierKeys & (ModifierKeys.Alt | ModifierKeys.Shift)) != ModifierKeys.None)
                return false;
            IMultiSelectionBroker multiSelectionBroker = _textView.GetMultiSelectionBroker();
            bool flag = false;
            for (int index = 0; !flag && index < multiSelectionBroker.AllSelections.Count; ++index)
                flag = !multiSelectionBroker.AllSelections[index].IsEmpty;
            if (!flag || !IsDragDropEnabled || !buttonPressed)
                return false;
            ITextViewLine containingYcoordinate = _textView.TextViewLines.GetTextViewLineContainingYCoordinate(viewRelativeMousePosition.Y);
            if (containingYcoordinate == null)
                return false;
            VirtualSnapshotSpan? selectionOnTextViewLine = _textView.Selection.GetSelectionOnTextViewLine(containingYcoordinate);
            if (!selectionOnTextViewLine.HasValue)
                return false;
            VirtualSnapshotPoint positionFromXcoordinate = containingYcoordinate.GetVirtualBufferPositionFromXCoordinate(viewRelativeMousePosition.X);
            VirtualSnapshotPoint virtualSnapshotPoint1 = positionFromXcoordinate;
            VirtualSnapshotSpan virtualSnapshotSpan = selectionOnTextViewLine.Value;
            VirtualSnapshotPoint start = virtualSnapshotSpan.Start;
            if (!(virtualSnapshotPoint1 < start))
            {
                VirtualSnapshotPoint virtualSnapshotPoint2 = positionFromXcoordinate;
                virtualSnapshotSpan = selectionOnTextViewLine.Value;
                VirtualSnapshotPoint end = virtualSnapshotSpan.End;
                if (!(virtualSnapshotPoint2 >= end))
                {
                    _mouseDownAnchorPoint = viewRelativeMousePosition;
                    StateManager.SetToMouseDown();
                    return true;
                }
            }
            return false;
        }

        internal bool HandleMouseLeftButtonUp(Point clickedPoint)
        {
            if (!IsDragDropEnabled || StateManager.State != DragDropState.MouseDown)
                return false;
            var textViewLines = _textView.TextViewLines;
            var y = clickedPoint.Y;
            var textLine = y > textViewLines.FirstVisibleLine.Top
                ? (y < textViewLines.LastVisibleLine.Bottom
                    ? textViewLines.GetTextViewLineContainingYCoordinate(y)
                    : textViewLines.LastVisibleLine)
                : textViewLines.FirstVisibleLine;
            if (textLine != null)
            {
                _textView.Selection.Clear();
                _textView.Caret.MoveTo(textLine, clickedPoint.X);
            }

            _mouseDownAnchorPoint = new Point?();
            StateManager.SetToStart();
            return true;
        }

        internal bool HandleMouseMove(bool leftButtonPressed, Point viewRelativeMousePosition)
        {
            if (!IsDragDropEnabled)
                return false;
            if (StateManager.State == DragDropState.Dragging)
                return true;
            if (!((StateManager.State == DragDropState.MouseDown) & leftButtonPressed) ||
                _textView.Selection.IsEmpty)
                return false;
            if (_mouseDownAnchorPoint.HasValue)
            {
                var point = _mouseDownAnchorPoint.Value;
                if (Math.Abs(point.X - viewRelativeMousePosition.X) < SystemParameters.MinimumHorizontalDragDistance)
                {
                    point = _mouseDownAnchorPoint.Value;
                    if (Math.Abs(point.Y - viewRelativeMousePosition.Y) < SystemParameters.MinimumVerticalDragDistance)
                        return true;
                    ;
                }

                StateManager.StartAndFinishDragDrop();
            }
            else
            {
                StateManager.StartAndFinishDragDrop();
            }

            return true;
        }

        internal bool HandleQueryContinueDrag(bool escapePressed, bool dragCanceled, ref DragAction resultingDragAction)
        {
            if (!(escapePressed | dragCanceled))
                return false;
            StateManager.SetToCanceled();
            resultingDragAction = DragAction.Cancel;
            return true;
        }

        private static DragDropEffects ConvertToDragDropEffect(DragDropPointerEffects original)
        {
            if (original == DragDropPointerEffects.All)
                return DragDropEffects.All;
            if (original == DragDropPointerEffects.None)
                return DragDropEffects.None;
            var dragDropEffects = DragDropEffects.None;
            if ((original & DragDropPointerEffects.Copy) == DragDropPointerEffects.Copy)
                dragDropEffects |= DragDropEffects.Copy;
            if ((original & DragDropPointerEffects.Move) == DragDropPointerEffects.Move)
                dragDropEffects |= DragDropEffects.Move;
            if ((original & DragDropPointerEffects.Link) == DragDropPointerEffects.Link)
                dragDropEffects |= DragDropEffects.Link;
            if ((original & DragDropPointerEffects.Scroll) == DragDropPointerEffects.Scroll)
                dragDropEffects |= DragDropEffects.Scroll;
            return dragDropEffects;
        }

        private VirtualSnapshotPoint CalculateInsertionPoint(Point location)
        {
            var textViewLines = _textView.TextViewLines;
            var virtualSnapshotPoint =
                (textViewLines.GetTextViewLineContainingYCoordinate(location.Y) ??
                 (location.Y < textViewLines.FirstVisibleLine.Top
                     ? textViewLines.FirstVisibleLine
                     : textViewLines.LastVisibleLine)).GetInsertionBufferPositionFromXCoordinate(location.X);
            if (!_textView.Options.IsVirtualSpaceEnabled())
                virtualSnapshotPoint = new VirtualSnapshotPoint(virtualSnapshotPoint.Position);
            return virtualSnapshotPoint;
        }

        private DragDropInfo CreateDragDropInfo(DragEventArgs dragEventArgs, Point position)
        {
            return new DragDropInfo(position, dragEventArgs.KeyStates, dragEventArgs.Data,
                StateManager.IsInternalDragDrop, dragEventArgs.Source, dragEventArgs.AllowedEffects,
                CalculateInsertionPoint(position));
        }

        private void DrawTrackerAndScroll(DragDropInfo dragDropInfo, DragDropPointerEffects dropHandlerFeedback)
        {
            var virtualBufferPosition = dragDropInfo.VirtualBufferPosition;
            var containingBufferPosition =
                _textView.GetTextViewLineContainingBufferPosition(virtualBufferPosition.Position);
            var extendedCharacterBounds = containingBufferPosition.GetExtendedCharacterBounds(virtualBufferPosition);
            if ((dropHandlerFeedback & DragDropPointerEffects.Track) == DragDropPointerEffects.Track)
                _visualManager.DrawTracker(extendedCharacterBounds);
            else
                _visualManager.ClearTracker();
            _visualManager.ScrollView(containingBufferPosition, extendedCharacterBounds);
        }

        private bool FromTouch(MouseEventArgs e)
        {
            return e.StylusDevice != null;
        }

        private Point GetClickedPoint(MouseEventArgs e)
        {
            var position = e.GetPosition(_textView.VisualElement);
            position.X += _textView.ViewportLeft;
            position.Y += _textView.ViewportTop;
            return position;
        }

        private Point GetClickedPoint(DragEventArgs e)
        {
            var position = e.GetPosition(_textView.VisualElement);
            position.X += _textView.ViewportLeft;
            position.Y += _textView.ViewportTop;
            return position;
        }
    }
}