using System;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Ui.Commanding
{
    public interface IChainedCommandHandler<in T> : ICommandHandler, INamed where T : CommandArgs
    {
        CommandState GetCommandState(T args, Func<CommandState> nextCommandHandler);

        void ExecuteCommand(T args, Action nextCommandHandler, CommandExecutionContext executionContext);
    }
}