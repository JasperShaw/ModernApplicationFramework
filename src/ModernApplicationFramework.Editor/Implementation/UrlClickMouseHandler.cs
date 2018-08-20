using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Editor.Interop;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal class UrlClickMouseHandler : MouseProcessorBase
    {
        private readonly ITextView _view;
        private readonly IEditorAdaptersFactoryService _adaptersFactory;
        private ITagAggregator<IUrlTag> _tagAggregator;
        private readonly CtrlKeyStateTracker _state;
        private bool _enabled;
        private ITagSpan<IUrlTag> _currentUrlSpan;
        private bool _changedCursorState;
        private Cursor _lastViewCursor;
        private ICommandTarget _commandTarget;

        private ITagSpan<IUrlTag> CurrentUrlSpan
        {
            get
            {
                if (_currentUrlSpan == null || _currentUrlSpan.Span.Snapshot != _view.TextSnapshot)
                    _currentUrlSpan = TryGetURLSpanUnderMouse();
                else
                {
                    if (!TryGetBufferPositionOfMouse(out var point) || !_currentUrlSpan.Span.Contains(point))
                        _currentUrlSpan = null;
                }

                return _currentUrlSpan;
            }
        }

        private ICommandTarget CommandTarget => _commandTarget ?? (_commandTarget = _adaptersFactory.GetViewAdapter(_view) as ICommandTarget);

        public UrlClickMouseHandler(ITextView view, IEditorAdaptersFactoryService adaptersFactory, 
            ITagAggregator<IUrlTag> tagAggregator, CtrlKeyStateTracker state)
        {
            _view = view;
            _adaptersFactory = adaptersFactory;
            _tagAggregator = tagAggregator;
            _state = state;
            _state.CtrlKeyStateChanged += (sender, args) => UpdateMouseCursor();
            view.Options.OptionChanged += ViewOptionsChanged;
            _tagAggregator.BatchedTagsChanged += BatchedTagsChanged;
            view.Closed += ViewClosed;
            _currentUrlSpan = null;
            _enabled = _view.Options.GetOptionValue(DefaultTextViewOptions.DisplayUrlsAsHyperlinksId);
        }

        public override void PreprocessMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!_enabled || !_state.IsCtrlKeyDown)
                return;
            var currentUrlSpan = CurrentUrlSpan;
            if (currentUrlSpan == null || !TryGetBufferPositionOfMouse(out var point))
                return;
            ClearCursor();
            var commandTarget = CommandTarget;
            if (commandTarget == null)
                e.Handled = Common.OpenUrl(currentUrlSpan.Tag.Url.AbsoluteUri);
            else
            {
                var num1 = IntPtr.Zero;
                try
                {
                    var containingLine = point.GetContainingLine();
                    var lineNumber = containingLine.LineNumber;
                    var num2 = point - containingLine.Start;
                    num1 = Marshal.AllocCoTaskMem(32);
                    Marshal.GetNativeVariantForObject(lineNumber, num1);
                    var pDstNativeVariant = new IntPtr(num1.ToInt32() + 16);
                    Marshal.GetNativeVariantForObject(num2, pDstNativeVariant); 
                    var guid = MafConstants.EditorCommandGroup;
                    e.Handled = commandTarget.Exec(guid, (uint) MafConstants.EditorCommands.OpenUrl, 0, num1, IntPtr.Zero) != 0;
                }
                finally
                {
                    Marshal.FreeCoTaskMem(num1);
                }
            }
        }

        public override void PreprocessMouseMove(MouseEventArgs e)
        {
            UpdateMouseCursor();
        }

        private void ViewClosed(object sender, EventArgs args)
        {
            if (_tagAggregator == null)
                return;
            _tagAggregator.Dispose();
            _tagAggregator = null;
        }

        private void BatchedTagsChanged(object sender, BatchedTagsChangedEventArgs e)
        {
            UpdateMouseCursor();
        }

        private void ViewOptionsChanged(object sender, EditorOptionChangedEventArgs e)
        {
            if (e.OptionId != DefaultTextViewOptions.DisplayUrlsAsHyperlinksId.Name)
                return;
            _enabled = _view.Options.GetOptionValue(DefaultTextViewOptions.DisplayUrlsAsHyperlinksId);
            UpdateMouseCursor();
        }

        private void UpdateMouseCursor()
        {
            if (_enabled && _state.IsCtrlKeyDown && CurrentUrlSpan != null)
                SetCursor();
            else
            {
                _currentUrlSpan = null;
                ClearCursor();
            }
        }

        private void SetCursor()
        {
            if (_changedCursorState || _view.VisualElement.Cursor == Cursors.Hand)
                return;
            _lastViewCursor = _view.VisualElement.Cursor;
            _view.VisualElement.Cursor = Cursors.Hand;
            _changedCursorState = true;
        }

        private void ClearCursor()
        {
            if (!_changedCursorState || _view.VisualElement.Cursor != Cursors.Hand)
                return;
            _view.VisualElement.Cursor = _lastViewCursor;
            _lastViewCursor = null;
            _changedCursorState = false;
        }

        private bool TryGetBufferPositionOfMouse(out SnapshotPoint point)
        {
            point = new SnapshotPoint();
            if (_view.IsClosed || _view.TextViewLines == null || !_view.VisualElement.IsVisible)
                return false;
            var view = RelativeToView(Mouse.GetPosition(_view.VisualElement));
            var containingYcoordinate = _view.TextViewLines.GetTextViewLineContainingYCoordinate(view.Y);
            var positionFromXcoordinate = containingYcoordinate?.GetBufferPositionFromXCoordinate(view.X);
            if (!positionFromXcoordinate.HasValue)
                return false;
            point = positionFromXcoordinate.Value;
            return true;
        }

        private Point RelativeToView(Point position)
        {
            return new Point(position.X + _view.ViewportLeft, position.Y + _view.ViewportTop);
        }

        private ITagSpan<IUrlTag> TryGetURLSpanUnderMouse()
        {
            if (_tagAggregator == null)
                return null;
            if (!TryGetBufferPositionOfMouse(out var point))
                return null;
            foreach (var tag in _tagAggregator.GetTags(new SnapshotSpan(point, point)))
            {
                var spans = tag.Span.GetSpans(_view.TextSnapshot);
                if (spans.Count == 1)
                {
                    var span = spans[0];
                    var length1 = span.Length;
                    span = tag.Span.GetSpans(tag.Span.AnchorBuffer)[0];
                    var lenght2 = span.Length;
                    if (length1 == lenght2)
                    {
                        span = spans[0];
                        if (span.Contains(point))
                            return new TagSpan<IUrlTag>(spans[0], tag.Tag);
                    }
                }
            }

            return null;
        }
    }
}