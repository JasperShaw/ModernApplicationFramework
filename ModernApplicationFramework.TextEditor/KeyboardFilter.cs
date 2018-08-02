using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class KeyboardFilter : KeyProcessor
    {
        private readonly ITextView _textView;
        private readonly IEditorOperations _editorOperations;
        private Guid _editorCommandGroup = MafConstants.EditorCommandGroup;

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
            ShowContextMenu();
            args.Handled = true;
        }

        private void ShowContextMenu()
        {
            var screen = _textView.VisualElement.PointToScreen(SimpleTextViewWindow.CalculateContextMenuPosition(_textView));
            var num = Marshal.AllocCoTaskMem(32);
            Marshal.GetNativeVariantForObject((object)(short)screen.X, num);
            Marshal.GetNativeVariantForObject((object)(short)screen.Y, new IntPtr(num.ToInt32() + 16));
            CommandTarget.Exec(ref _editorCommandGroup, (uint) MafConstants.EditorCommands.ShowContextMenu, 0U, num, IntPtr.Zero);
            Marshal.FreeCoTaskMem(num);
        }
    }
}