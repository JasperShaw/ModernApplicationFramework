using System;

namespace ModernApplicationFramework.TextEditor.Commanding
{
    public interface IEditorCommandHandlerService
    {
        CommandState GetCommandState<T>(Func<ITextView, ITextBuffer, T> argsFactory,
            Func<CommandState> nextCommandHandler) where T : EditorCommandArgs;

        void Execute<T>(Func<ITextView, ITextBuffer, T> argsFactory, Action nextCommandHandler)
            where T : EditorCommandArgs;
    }
}