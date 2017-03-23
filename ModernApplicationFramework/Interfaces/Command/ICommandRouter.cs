using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Interfaces.Command
{
    public interface ICommandRouter
    {
        CommandHandlerWrapper GetCommandHandler(CommandDefinition commandDefinition);
    }

    public interface ICommandRerouter
    {
        object GetHandler(CommandDefinition commandDefinition);
    }
}
