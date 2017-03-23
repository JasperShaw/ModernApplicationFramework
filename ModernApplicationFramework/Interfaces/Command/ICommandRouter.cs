using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Interfaces.Command
{
    public interface ICommandRouter
    {
        CommandHandlerWrapper GetCommandHandler(DefinitionBase commandDefinition);
    }

    public interface ICommandRerouter
    {
        object GetHandler(DefinitionBase commandDefinition);
    }
}
