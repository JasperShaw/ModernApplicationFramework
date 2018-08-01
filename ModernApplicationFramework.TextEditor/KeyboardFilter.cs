using System.Windows.Input;
using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class KeyboardFilter : KeyProcessor
    {
        private readonly ITextView _textView;
        private readonly IEditorOperations _editorOperations;

        private ICommandTarget _commandTarget;

        private ICommandTarget CommandTarget
        {
            get
            {
                if (_commandTarget == null)
                    _textView?.Properties.TryGetProperty(typeof(ICommandTarget), out _commandTarget);
                return _commandTarget;
            }
        }

        private bool IsValid => CommandTarget != null;

        public KeyboardFilter(ITextView textView, IEditorOperations editorOperations)
        {
            _textView = textView;
            _editorOperations = editorOperations;
        }

        public override void KeyDown(KeyEventArgs args)
        {
            if (!IsValid || args.Key != Key.System || (ModifierKeys.Shift != Keyboard.Modifiers || args.SystemKey != Key.F10))
                return;
            args.Handled = true;
        }
    }
}