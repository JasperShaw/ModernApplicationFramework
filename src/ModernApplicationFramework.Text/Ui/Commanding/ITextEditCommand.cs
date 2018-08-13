namespace ModernApplicationFramework.Text.Ui.Commanding
{
    public interface ITextEditCommand
    {
    }

    public interface ITextEditCommand<in T> : ITextEditCommand where T : CommandArgs
    {
        CommandState GetCommandState(T args);

        bool ExecuteCommand(T args, CommandExecutionContext executionContext);
    }
}