using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal class LeftSelectionMargin : ContainerMargin
    {
        private readonly IEditorOperations _editorOperations;
        private static Cursor _rightArrowCursor;
        private bool _isCapturingMouse;

        private LeftSelectionMargin(ITextViewHost textViewHost, IEditorOperations editorOperations, GuardedOperations guardedOperations, TextViewMarginState marginState)
            : base("LeftSelection", Orientation.Vertical, textViewHost, guardedOperations, marginState)
        {
            _editorOperations = editorOperations;
            _isCapturingMouse = false;
            Background = Brushes.Transparent;
            Cursor = RightArrowCursor;
        }

        public static ITextViewMargin Create(ITextViewHost textViewHost, IEditorOperations editorOperations, GuardedOperations guardedOperations, TextViewMarginState marginState)
        {
            var leftSelectionMargin = new LeftSelectionMargin(textViewHost, editorOperations, guardedOperations, marginState);
            leftSelectionMargin.Initialize();
            return leftSelectionMargin;
        }

        protected override void RegisterEvents()
        {
            base.RegisterEvents();
            MouseLeftButtonDown += OnMouseLeftButtonDown;
            MouseMove += OnMouseMove;
            MouseLeftButtonUp += OnMouseLeftButtonUp;
            TextViewHost.TextView.ZoomLevelChanged += OnZoomLevelChanged;
            var textView = TextViewHost.TextView;
            LayoutTransform = new ScaleTransform(textView.ZoomLevel / 100.0, textView.ZoomLevel / 100.0);
            LayoutTransform.Freeze();
        }

        protected override void UnregisterEvents()
        {
            base.UnregisterEvents();
            MouseLeftButtonDown -= OnMouseLeftButtonDown;
            MouseMove -= OnMouseMove;
            MouseLeftButtonUp -= OnMouseLeftButtonUp;
            TextViewHost.TextView.ZoomLevelChanged -= OnZoomLevelChanged;
        }

        private void OnZoomLevelChanged(object sender, ZoomLevelChangedEventArgs e)
        {
            LayoutTransform = e.ZoomTransform;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                _isCapturingMouse = true;
                if (!CaptureMouse())
                    return;
                var extendSelection = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
                SelectLine(e.GetPosition(this), extendSelection);
                e.Handled = true;
            }
            finally
            {
                _isCapturingMouse = false;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isCapturingMouse || e.LeftButton != MouseButtonState.Pressed || !IsMouseCaptured)
                return;
            var position = e.GetPosition(this);
            if (!SelectLine(position, true))
            {
                if (position.Y <= 0.0)
                    _editorOperations.MoveLineUp(true);
                else if (position.Y > TextViewHost.TextView.ViewportHeight)
                    _editorOperations.MoveLineDown(true);
            }
            e.Handled = true;
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsMouseCaptured)
                return;
            ReleaseMouseCapture();
            e.Handled = true;
        }

        private static Cursor RightArrowCursor
        {
            get
            {
                if (_rightArrowCursor == null)
                {
                    using (var manifestResourceStream = typeof(LeftSelectionMargin).Module.Assembly.GetManifestResourceStream("Microsoft.VisualStudio.UI.Text.Wpf.View.Implementation.Resources.RightArrow.cur"))
                    {
                        if (manifestResourceStream != null)
                            _rightArrowCursor = WpfHelper.LoadCursorDpiAware(manifestResourceStream);
                        if (_rightArrowCursor == null)
                            _rightArrowCursor = Cursors.Arrow;
                    }
                }
                return _rightArrowCursor;
            }
        }

        internal bool SelectLine(Point mouseLocation, bool extendSelection)
        {
            Keyboard.Focus(TextViewHost.TextView.VisualElement);
            var containingYcoordinate = TextViewHost.TextView.TextViewLines.GetTextViewLineContainingYCoordinate(mouseLocation.Y + TextViewHost.TextView.ViewportTop);
            if (containingYcoordinate == null)
                return false;
            _editorOperations.SelectLine(containingYcoordinate, extendSelection);
            return true;
        }
    }
}