using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Ui.Commanding
{
    public interface ICommandHandler
    {
    }

    public interface ICommandHandler<in T> : ICommandHandler, INamed where T : CommandArgs
    {
        CommandState GetCommandState(T args);

        bool ExecuteCommand(T args, CommandExecutionContext executionContext);
    }
}