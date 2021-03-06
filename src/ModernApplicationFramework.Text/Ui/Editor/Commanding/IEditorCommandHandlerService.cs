﻿using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Commanding;

namespace ModernApplicationFramework.Text.Ui.Editor.Commanding
{
    public interface IEditorCommandHandlerService
    {
        void Execute<T>(Func<ITextView, ITextBuffer, T> argsFactory, Action nextCommandHandler)
            where T : EditorCommandArgs;

        CommandState GetCommandState<T>(Func<ITextView, ITextBuffer, T> argsFactory,
            Func<CommandState> nextCommandHandler) where T : EditorCommandArgs;
    }
}