using ModernApplicationFramework.TextEditor.Implementation;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor.Commanding
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