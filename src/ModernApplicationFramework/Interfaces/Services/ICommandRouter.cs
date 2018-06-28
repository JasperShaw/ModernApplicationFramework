using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Interfaces.Services
{
    public interface ICommandRouter
    {
        CommandHandlerWrapper GetCommandHandler(CommandDefinitionBase commandDefinition);
    }

    public interface ICommandRerouter
    {
        object GetHandler(CommandDefinitionBase commandDefinition);
    }
}
