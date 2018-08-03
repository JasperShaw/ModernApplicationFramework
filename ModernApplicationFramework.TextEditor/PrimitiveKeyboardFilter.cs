﻿using System;
using System.Runtime.InteropServices;
using System.Windows.Input;
using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class PrimitiveKeyboardFilter : KeyProcessor
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

        public PrimitiveKeyboardFilter(ITextView textView, IEditorOperations editorOperations)
        {
            _textView = textView;
            _editorOperations = editorOperations;
        }

        public override void KeyDown(KeyEventArgs args)
        {
            if (!IsValid)
                return;
            switch (args.Key)
            {
                case Key.Left:
                    SendCommand(MafConstants.EditorCommands.Left);
                    break;
            }
            base.KeyDown(args);
        }

        private void SendCommand(MafConstants.EditorCommands commandId)
        {
            SendCommand(_editorCommandGroup, (uint) commandId);
        }

        private void SendCommand(Guid editorCommandGroup, uint commandId)
        {
            var num = IntPtr.Zero;
            var zero = IntPtr.Zero;
            CommandTarget.Exec(ref editorCommandGroup, commandId, 0, num, zero);
            if (zero != IntPtr.Zero)
                Marshal.GetObjectForNativeVariant(zero);
        }
    }
}