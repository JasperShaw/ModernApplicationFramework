using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Ui.Commanding
{
    public interface ICommandHandler
    {
    }

    public interface ICommandHandler<in T> : ICommandHandler, INamed where T : CommandArgs
    {
        bool ExecuteCommand(T args, CommandExecutionContext executionContext);
        CommandState GetCommandState(T args);
    }
}