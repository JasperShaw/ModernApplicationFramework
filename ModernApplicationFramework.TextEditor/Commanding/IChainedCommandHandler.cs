using System;
using ModernApplicationFramework.TextEditor.Implementation;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor.Commanding
{
    public interface IChainedCommandHandler<in T> : ICommandHandler, INamed where T : CommandArgs
    {
        CommandState GetCommandState(T args, Func<CommandState> nextCommandHandler);

        void ExecuteCommand(T args, Action nextCommandHandler, CommandExecutionContext executionContext);
    }
}