using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class MouseProcessor : MouseProcessorBase, IMouseProcessor2
    {
        private readonly ITextView _textView;
        private readonly IViewPrimitives _viewPrimitives;
 
        private SimpleTextViewWindow _simpleTextViewWindow;

        private SimpleTextViewWindow SimpleTextViewWindow
        {
            get
            {
                if (_simpleTextViewWindow == null && _textView != null)
                    _simpleTextViewWindow = EditorAdaptersFactoryService.GetSimpleTextViewWindowFromTextView(_textView);
                return _simpleTextViewWindow;
            }
        }

        public MouseProcessor(ITextView textView, IEditorPrimitivesFactoryService editorPrimitivesFactoryService)
        {
            if (editorPrimitivesFactoryService == null)
                throw new ArgumentNullException(nameof(editorPrimitivesFactoryService));
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _viewPrimitives = editorPrimitivesFactoryService.GetViewPrimitives(textView);
        }

        public void PostprocessStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
        }

        public void PreprocessManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e)
        {
        }

        public void PostprocessManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e)
        {
        }

        public void PreprocessManipulationStarting(ManipulationStartingEventArgs e)
        {
        }

        public void PostprocessManipulationStarting(ManipulationStartingEventArgs e)
        {
        }

        public void PreprocessManipulationDelta(ManipulationDeltaEventArgs e)
        {
        }

        public void PostprocessManipulationDelta(ManipulationDeltaEventArgs e)
        {
        }

        public void PreprocessManipulationCompleted(ManipulationCompletedEventArgs e)
        {
        }

        public void PostprocessManipulationCompleted(ManipulationCompletedEventArgs e)
        {
        }

        public void PreprocessTouchUp(TouchEventArgs e)
        {
        }

        public void PostprocessTouchUp(TouchEventArgs e)
        {
        }

        public void PreprocessTouchDown(TouchEventArgs e)
        {
        }

        public override void PostprocessMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            HandleClick(e);
            SimpleTextViewWindow?.ClearCommandContext();
        }

        public override void PreprocessMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (SimpleTextViewWindow == null)
                return;
            if (e.ClickCount == 1 && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                SimpleTextViewWindow.ClearCommandContext();
                SimpleTextViewWindow.SetCommandContext(MafConstants.EditorCommands.SelectCurrentWord);
            }
            else if (e.ClickCount == 2)
            {
                SimpleTextViewWindow.ClearCommandContext();
                SimpleTextViewWindow.SetCommandContext(MafConstants.EditorCommands.SelectCurrentWord);
            }
            if (e.ClickCount != 2 || HandleClick(e) != 0)
                return;
            e.Handled = true;
        }

        public override void PreprocessMouseRightButtonUp(MouseButtonEventArgs e)
        {
            HandlePreprocessRightButtonUp(e);
        }

        public override void PreprocessMouseRightButtonDown(MouseButtonEventArgs e)
        {
            HandlePreprocessRightButtonDown(e);
        }

        public void PreprocessStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            if (_textView == null || _textView.IsClosed || e.SystemGesture != SystemGesture.HoldEnter)
                return;
            Keyboard.Focus(_textView.VisualElement);
            HandlePreprocessRightButtonDown(e);
            HandlePreprocessRightButtonUp(e);
        }

        public void PostprocessTouchDown(TouchEventArgs e)
        {
            if (_textView == null || _textView.IsClosed)
                return;
            Keyboard.Focus(_textView.VisualElement);
        }

        internal void MoveCaretToPosition(Point mousePosition)
        {
            mousePosition.X += _textView.ViewportLeft;
            mousePosition.Y += _textView.ViewportTop;
            var containingYcoordinate = _textView.TextViewLines.GetTextViewLineContainingYCoordinate(mousePosition.Y);
            if (containingYcoordinate != null)
            {
                var clickedBufferPosition = containingYcoordinate.GetInsertionBufferPositionFromXCoordinate(mousePosition.X);
                bool flag;
                if (!_textView.Options.GetOptionValue(DefaultTextViewOptions.UseVirtualSpaceId) && _textView.Selection.Mode != TextSelectionMode.Box)
                {
                    var nonVirtualPoint = clickedBufferPosition.Position;
                    flag = _textView.Selection.SelectedSpans.Any(selectionSpan => selectionSpan.Contains(nonVirtualPoint));
                }
                else
                    flag = _textView.Selection.VirtualSelectedSpans.Any(selectionSpan => selectionSpan.Contains(clickedBufferPosition));
                if (flag)
                    return;
                _textView.Caret.MoveTo(containingYcoordinate, mousePosition.X);
                _textView.Selection.Clear();
                _textView.Caret.EnsureVisible();
            }
            else
                _viewPrimitives.Caret.MoveTo(_textView.TextSnapshot.Length);
        }

        internal void DisplayContextMenu(Point mousePosition)
        {
            if (SimpleTextViewWindow == null)
                return;
            Point screen = _textView.VisualElement.PointToScreen(mousePosition);
            var num = Marshal.AllocCoTaskMem(32);
            Marshal.GetNativeVariantForObject((short)screen.X, num);
            Marshal.GetNativeVariantForObject((short)screen.Y, new IntPtr(num.ToInt32() + 16));

            var guid = MafConstants.EditorCommandGroup;

            if (SimpleTextViewWindow.Exec(ref guid, (uint) MafConstants.EditorCommands.ShowContextMenu, 0, num, IntPtr.Zero) != 0)
            {
                //TODO
            }
            Marshal.FreeCoTaskMem(num);
        }

        private void HandlePreprocessRightButtonDown(InputEventArgs e)
        {
            MoveCaretToPosition(GetPosition(e, _textView.VisualElement));
            e.Handled = true;
        }

        private Point GetPosition(InputEventArgs e, IInputElement element)
        {
            var point = new Point(0.0, 0.0);
            if (e is MouseButtonEventArgs args)
                point = args.GetPosition(element);
            else if (e is TouchEventArgs eventArgs)
                point = eventArgs.GetTouchPoint(element).Position;
            else if (e is StylusSystemGestureEventArgs gestureEventArgs)
                point = gestureEventArgs.GetPosition(element);
            return point;
        }

        private void HandlePreprocessRightButtonUp(InputEventArgs e)
        {
            if (PresentationSource.FromDependencyObject(_textView.VisualElement) == null || IsComingFromFindAdornment(e, _textView))
                return;
            DisplayContextMenu(GetPosition(e, _textView.VisualElement));
            e.Handled = true;
        }

        private int HandleClick(MouseButtonEventArgs e)
        {
            var num = int.MinValue;
            if (SimpleTextViewWindow == null ||
                PresentationSource.FromDependencyObject(_textView.VisualElement) == null)
                return num;
            var position = GetPosition(e, _textView.VisualElement);

            position.X += _textView.ViewportLeft;
            position.Y += _textView.ViewportTop;
            var textViewLine = _textView.TextViewLines.GetTextViewLineContainingYCoordinate(position.Y) ??
                               (position.Y < _textView.TextViewLines[0].Top
                                   ? _textView.TextViewLines[0]
                                   : _textView.TextViewLines[_textView.TextViewLines.Count - 1]);
            var nullable1 = textViewLine.GetBufferPositionFromXCoordinate(position.X) ?? (position.X < textViewLine.TextLeft
                                ? textViewLine.Start
                                : textViewLine.End);
            var nullable2 = SimpleTextViewWindow.DataPointFromViewPoint(nullable1);
            if (!nullable2.HasValue)
                return num;
            var containingLine = nullable2.Value.GetContainingLine();
            var lineNumber = containingLine.LineNumber;
            var num2 = nullable2.Value - containingLine.Start;
            var guid = MafConstants.EditorCommandGroup;
            var id = e.ClickCount == 1 ? (uint) MafConstants.EditorCommands.LeftClick : (uint) MafConstants.EditorCommands.DoubleClick;

            var prgCmds = new[]
            {
                new Olecmd{cmdID = id}
            };

            if (SimpleTextViewWindow.QueryStatus(ref guid, 1, prgCmds, IntPtr.Zero) >= 0 && ((int) prgCmds[0].cmdf & 2) != 0)
            {
                var num3 = Marshal.AllocCoTaskMem(32);
                Marshal.GetNativeVariantForObject(lineNumber, num3);
                Marshal.GetNativeVariantForObject(num2, new IntPtr(num3.ToInt32() + 16));
                num = SimpleTextViewWindow.Exec(ref guid, id, 0, num3, IntPtr.Zero);
                Marshal.FreeCoTaskMem(num3);
            }
            return num;
        }

        private static bool IsComingFromFindAdornment(RoutedEventArgs e, ITextView textView)
        {
            for (DependencyObject reference = (e.Source as Visual); reference != null; reference = VisualTreeHelper.GetParent(reference))
            {
                //TODO: Add FindUI 
                //if (reference is FindUI)
                //    return true;
                if (reference == textView.VisualElement)
                    return false;
            }
            return false;
        }
    }
}