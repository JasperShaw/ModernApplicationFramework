using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Editor.Interop;
using ModernApplicationFramework.Editor.TextManager;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Operations;

namespace ModernApplicationFramework.Editor.Implementation
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
            if (!IsValid || args.Key != Key.System || ModifierKeys.Shift != Keyboard.Modifiers || args.SystemKey != Key.F10)
                return;
            ShowContextMenu();
            args.Handled = true;
        }

        public override void KeyUp(KeyEventArgs args)
        {
            if (!IsValid || args.Key != Key.Apps)
                return;
            ShowContextMenu();
            args.Handled = true;
        }

        public override void TextInput(TextCompositionEventArgs args)
        {
            if (!IsValid)
                return;
            if (args.Text.Length > 0)
            {
                foreach (var num in args.Text)
                {
                    if (num > 31)
                        SendCommand(MafConstants.EditorCommands.TypeChar, num);
                }
            }
            else
            {
                if (_editorOperations.ProvisionalCompositionSpan == null)
                    return;
                SendCommand(MafConstants.EditorCommands.Backspace, null);
                args.Handled = true;
            }
        }

        public override void TextInputStart(TextCompositionEventArgs args)
        {
            if (!(args.TextComposition is ImeTextComposition))
                return;
            HandleProvisionalImeInput(args);
        }

        public override void TextInputUpdate(TextCompositionEventArgs args)
        {
            if (args.TextComposition is ImeTextComposition)
                HandleProvisionalImeInput(args);
            else
                args.Handled = false;
        }

        private void HandleProvisionalImeInput(TextCompositionEventArgs args)
        {
            if (!IsValid || args.Text.Length <= 0)
                return;
            var commandTarget = CommandTarget as ITypedTextTarget;
            try
            {
                if (commandTarget != null)
                    commandTarget.InProvisionalInput = true;
                foreach (var c in args.Text)
                    SendCommand(MafConstants.EditorCommands.TypeChar, c);
            }
            finally
            {
                if (commandTarget != null)
                    commandTarget.InProvisionalInput = false;
            }
            args.Handled = true;
        }

        private void SendCommand(MafConstants.EditorCommands commandId, object inParam)
        {
            SendCommand(_editorCommandGroup, (uint) commandId, inParam);
        }

        private object SendCommand(Guid cmdGroup, uint cmdID, object inParam)
        {
            object obj = null;
            var num = IntPtr.Zero;
            if (inParam != null)
            {
                num = Marshal.AllocHGlobal(256);
                Marshal.GetNativeVariantForObject(inParam, num);
            }

            var zero = IntPtr.Zero;
            CommandTarget.Exec(cmdGroup, cmdID, 0, num, zero);
            if (zero != IntPtr.Zero)
                obj = Marshal.GetObjectForNativeVariant(zero);
            if (inParam != null)
                Marshal.FreeHGlobal(num);
            return obj;
        }

        private void ShowContextMenu()
        {
            var screen = _textView.VisualElement.PointToScreen(SimpleTextViewWindow.CalculateContextMenuPosition(_textView));
            var num = Marshal.AllocCoTaskMem(32);
            Marshal.GetNativeVariantForObject((object)(short)screen.X, num);
            Marshal.GetNativeVariantForObject((object)(short)screen.Y, new IntPtr(num.ToInt32() + 16));
            CommandTarget.Exec(_editorCommandGroup, (uint) MafConstants.EditorCommands.ShowContextMenu, 0U, num, IntPtr.Zero);
            Marshal.FreeCoTaskMem(num);
        }
    }
}