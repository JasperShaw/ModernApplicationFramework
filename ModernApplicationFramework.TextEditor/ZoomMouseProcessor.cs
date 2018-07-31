using System;
using System.Windows.Input;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class ZoomMouseProcessor : MouseProcessorBase
    {
        private readonly ITextView _textView;
        private readonly ZoomMouseProcessorProvider _factory;

        private ITextView _targetTextView;
        private IEditorOperations _targetEditorOperations;

        private ITextView TargetTextView
        {
            get
            {
                if (_targetTextView == null)
                {
                    _targetTextView = _textView;
                    if (_textView.Roles.Contains("EMBEDDED_PEEK_TEXT_VIEW") && _textView.Properties.TryGetProperty<ITextView>("PeekContainingTextView", out var property) && property != null)
                        _targetTextView = property;
                }
                return _targetTextView;
            }
        }

        private IEditorOperations TargetEditorOperations => _targetEditorOperations ?? (_targetEditorOperations =
                                                                _factory.EditorOperationsFactoryService.GetEditorOperations(TargetTextView));


        public ZoomMouseProcessor(ITextView textView, ZoomMouseProcessorProvider factory)
        {
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public override void PreprocessMouseWheel(MouseWheelEventArgs e)
        {
            if (!ZoomHelper(e.Delta, (Keyboard.Modifiers & ModifierKeys.Control) > 0))
                return;
            e.Handled = true;
        }

        internal bool ZoomHelper(double delta, bool isTriggered)
        {
            if (!isTriggered || !TargetTextView.Options.GetOptionValue(DefaultViewOptions.EnableMouseWheelZoomId))
                return false;
            if (delta > 0.0)
                TargetEditorOperations.ZoomIn();
            else
                TargetEditorOperations.ZoomOut();
            return true;
        }
    }
}