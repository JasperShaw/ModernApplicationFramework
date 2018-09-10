using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Text.Ui.Text;

namespace ModernApplicationFramework.Modules.Editor.MultiSelection
{
    internal class MultiSelectionMouseHandler : MouseProcessorBase
    {
        private readonly MultiSelectionMouseHandlerProvider _provider;
        private readonly ITextView _wpfTextView;
        private readonly IMultiSelectionBroker _multiSelectionBroker;
        private MultiSelectionMouseState _mouseState;

        public MultiSelectionMouseHandler(MultiSelectionMouseHandlerProvider provider, ITextView wpfTextView)
        {
            _provider = provider;
            _wpfTextView = wpfTextView;
            _multiSelectionBroker = _wpfTextView.GetMultiSelectionBroker();
        }

        private MultiSelectionMouseState MouseState => _mouseState ?? (_mouseState = MultiSelectionMouseState.GetStateForView(_wpfTextView));

        public override void PreprocessMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            e.Handled = PreprocessMouseLeftButtonDown(Keyboard.Modifiers.HasFlag(ModifierKeys.Control), Keyboard.Modifiers.HasFlag(ModifierKeys.Alt), Keyboard.Modifiers.HasFlag(ModifierKeys.Shift), e.GetPosition(_wpfTextView.VisualElement), e.ClickCount);
        }

        public void CommitProvisionalSelection()
        {
            MouseState.UserIsDraggingSelection = false;
            if (!(MouseState.ProvisionalSelection != Selection.Invalid))
                return;
            var provisionalSelection = MouseState.ProvisionalSelection;
            MouseState.ProvisionalSelection = Selection.Invalid;
            if (!(provisionalSelection != _multiSelectionBroker.BoxSelection) || !(provisionalSelection != _multiSelectionBroker.PrimarySelection))
                return;
            var selectionSet = new HashSet<Selection>();
            foreach (var selection in _multiSelectionBroker.AllSelections)
            {
                if (selection.Extent.IntersectsWith(provisionalSelection.Extent))
                    selectionSet.Add(selection);
            }
            using (_multiSelectionBroker.BeginBatchOperation())
            {
                _multiSelectionBroker.AddSelection(provisionalSelection);
                foreach (var selection in selectionSet)
                    _multiSelectionBroker.TryRemoveSelection(selection);
            }
        }

        public bool PreprocessMouseLeftButtonDown(bool ctrl, bool alt, bool shift, Point triggerPoint, int clickCount = 1)
        {
            triggerPoint.X += _wpfTextView.ViewportLeft;
            triggerPoint.Y += _wpfTextView.ViewportTop;
            var nullable = TranslateMousePosition(triggerPoint, out var containingLine);
            if (!nullable.HasValue)
                return false;
            var virtualSnapshotPoint = nullable.Value;
            CommitProvisionalSelection();
            if (ctrl)
            {
                if (true & alt && !shift)
                {
                    if (_multiSelectionBroker.IsBoxSelection)
                        _multiSelectionBroker.BreakBoxSelection();
                    return AddOrRemoveSelections(clickCount, virtualSnapshotPoint, containingLine, triggerPoint, shift);
                }
            }
            else if (alt)
            {
                if (shift)
                    AddOrUpdateBoxSelection(_multiSelectionBroker.IsBoxSelection ? _multiSelectionBroker.BoxSelection.AnchorPoint : _multiSelectionBroker.PrimarySelection.AnchorPoint, virtualSnapshotPoint);
                else
                    AddOrUpdateBoxSelection(virtualSnapshotPoint, virtualSnapshotPoint);
                MouseState.UserIsDraggingSelection = true;
            }
            else
            {
                var isBoxSelection = _multiSelectionBroker.IsBoxSelection;
                var anchorSelection = isBoxSelection ? _multiSelectionBroker.BoxSelection : _multiSelectionBroker.PrimarySelection;
                using (_multiSelectionBroker.BeginBatchOperation())
                {
                    if (!shift & isBoxSelection)
                        _multiSelectionBroker.ClearSecondarySelections();
                    MouseState.UserIsDraggingSelection = true;
                    var selectionForTriggerPoint = CreateSelectionForTriggerPoint(triggerPoint, containingLine, anchorSelection, shift);
                    if (isBoxSelection & shift)
                        _multiSelectionBroker.SetBoxSelection(selectionForTriggerPoint);
                    else
                        _multiSelectionBroker.SetSelection(selectionForTriggerPoint);
                }
            }
            return false;
        }

        private Selection CreateSelectionForTriggerPoint(Point triggerPoint, ITextViewLine containingLine, Selection anchorSelection, bool shift)
        {
            var xCoordinate = containingLine.MapXCoordinate(_wpfTextView, triggerPoint.X, _provider.SmartIndentationService, true);
            var positionFromXcoordinate = containingLine.GetInsertionBufferPositionFromXCoordinate(xCoordinate);
            if (shift)
                return new Selection(anchorSelection.AnchorPoint, positionFromXcoordinate);
            var insertionPointAffinity = containingLine.IsLastTextViewLineForSnapshotLine || !(positionFromXcoordinate.Position == containingLine.End) ? PositionAffinity.Successor : PositionAffinity.Predecessor;
            return new Selection(positionFromXcoordinate, insertionPointAffinity);
        }

        private void MoveProvisionalSelection(VirtualSnapshotPoint point, bool select)
        {
            var provisionalSelection = MouseState.ProvisionalSelection;
            if (!(provisionalSelection != Selection.Invalid))
                return;
            MouseState.ProvisionalSelection = new Selection(select ? provisionalSelection.AnchorPoint : point, point);
        }

        private void AddOrUpdateBoxSelection(VirtualSnapshotPoint anchor, VirtualSnapshotPoint active)
        {
            if (_multiSelectionBroker.IsBoxSelection)
            {
                _multiSelectionBroker.TryPerformActionOnSelection(_multiSelectionBroker.BoxSelection, transformer =>
                {
                    transformer.MoveTo(anchor, active, active, PositionAffinity.Successor);
                    transformer.CapturePreferredReferencePoint();
                }, out _);
            }
            else
            {
                var selection1 = new Selection(active, anchor, active);
                using (_multiSelectionBroker.BeginBatchOperation())
                {
                    _multiSelectionBroker.SetBoxSelection(selection1);
                    var multiSelectionBroker = _multiSelectionBroker;
                    var before = selection1;
                    multiSelectionBroker.TryPerformActionOnSelection(before, transformer => transformer.CapturePreferredReferencePoint(), out _);
                }
            }
        }

        private bool AddOrRemoveSelections(int clickCount, VirtualSnapshotPoint point, ITextViewLine containingLine, Point triggerPoint, bool shift)
        {
            var num = 0;
            var intersectingSpan = _multiSelectionBroker.GetSelectionsIntersectingSpan(new SnapshotSpan(point.Position, point.Position));
            var selection = intersectingSpan.Count > 0 ? intersectingSpan[0] : Selection.Invalid;
            if (selection == Selection.Invalid)
            {
                MouseState.ProvisionalSelection = CreateSelectionForTriggerPoint(triggerPoint, containingLine, _multiSelectionBroker.PrimarySelection, shift);
                MouseState.UserIsDraggingSelection = true;
                return num != 0;
            }
            if (clickCount == 2)
            {
                _multiSelectionBroker.AddSelection(_multiSelectionBroker.TransformSelection(new Selection(point, PositionAffinity.Successor), PredefinedSelectionTransformations.SelectCurrentWord));
                return num != 0;
            }
            if (!_multiSelectionBroker.HasMultipleSelections)
                return num != 0;
            _multiSelectionBroker.TryRemoveSelection(selection);
            return num != 0;
        }

        public override void PreprocessMouseMove(MouseEventArgs e)
        {
            e.Handled = PreprocessMouseMove(Keyboard.Modifiers.HasFlag(ModifierKeys.Control), Keyboard.Modifiers.HasFlag(ModifierKeys.Alt), Keyboard.Modifiers.HasFlag(ModifierKeys.Shift), e.GetPosition(_wpfTextView.VisualElement), e.LeftButton == MouseButtonState.Pressed);
        }

        public bool PreprocessMouseMove(bool ctrl, bool alt, bool shift, Point triggerPoint, bool leftButtonDown = false)
        {
            triggerPoint.X += _wpfTextView.ViewportLeft;
            triggerPoint.Y += _wpfTextView.ViewportTop;
            if (!leftButtonDown && MouseState.UserIsDraggingSelection)
            {
                CommitProvisionalSelection();
                return false;
            }
            if (!MouseState.UserIsDraggingSelection)
                return false;
            var nullable = TranslateMousePosition(triggerPoint, out var containingLine);
            if (!nullable.HasValue)
                return false;
            var point = nullable.Value;
            var provisionalSelection = MouseState.ProvisionalSelection;
            if ((ctrl & alt || !alt) && provisionalSelection != Selection.Invalid)
            {
                if (!_wpfTextView.Options.IsVirtualSpaceEnabled())
                {
                    var xCoordinate = containingLine.MapXCoordinate(_wpfTextView, triggerPoint.X, _provider.SmartIndentationService, true);
                    point = containingLine.GetInsertionBufferPositionFromXCoordinate(xCoordinate);
                }
                MoveProvisionalSelection(point, true);
            }
            else if (!ctrl & alt)
            {
                _multiSelectionBroker.SetBoxSelection(new Selection((!(provisionalSelection != Selection.Invalid) ? (!_multiSelectionBroker.IsBoxSelection ? _multiSelectionBroker.PrimarySelection : _multiSelectionBroker.BoxSelection) : provisionalSelection).AnchorPoint, point));
                MouseState.ProvisionalSelection = Selection.Invalid;
            }
            else
            {
                Selection before;
                if (_multiSelectionBroker.IsBoxSelection)
                {
                    before = _multiSelectionBroker.BoxSelection;
                }
                else
                {
                    before = _multiSelectionBroker.PrimarySelection;
                    if (!_wpfTextView.Options.IsVirtualSpaceEnabled())
                    {
                        var xCoordinate = containingLine.MapXCoordinate(_wpfTextView, triggerPoint.X, _provider.SmartIndentationService, true);
                        point = containingLine.GetInsertionBufferPositionFromXCoordinate(xCoordinate);
                    }
                }

                _multiSelectionBroker.TryPerformActionOnSelection(before, transformer => transformer.MoveTo(point, true, PositionAffinity.Successor), out _);
            }
            return false;
        }

        public override void PreprocessMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            e.Handled = PreprocessMouseLeftButtonUp(Keyboard.Modifiers.HasFlag(ModifierKeys.Control), Keyboard.Modifiers.HasFlag(ModifierKeys.Alt), Keyboard.Modifiers.HasFlag(ModifierKeys.Shift), e.GetPosition(_wpfTextView.VisualElement));
        }

        public bool PreprocessMouseLeftButtonUp(bool ctrl, bool alt, bool shift, Point triggerPoint)
        {
            return PreprocessMouseMove(ctrl, alt, shift, triggerPoint);
        }

        private VirtualSnapshotPoint? TranslateMousePosition(Point point, out ITextViewLine containingLine)
        {
            containingLine = null;
            if (_wpfTextView.IsClosed || _wpfTextView.InLayout)
                return new VirtualSnapshotPoint?();
            if (!_wpfTextView.TryGetClosestTextViewLine(point.Y, out containingLine) && _wpfTextView.TextViewLines != null)
                containingLine = point.Y > 0.0 ? _wpfTextView.TextViewLines.LastVisibleLine : _wpfTextView.TextViewLines.FirstVisibleLine;
            return containingLine?.GetInsertionBufferPositionFromXCoordinate(point.X);
        }
    }
}
