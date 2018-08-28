using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Interfaces.Services
{
    public interface ICommandRouter
    {
        CommandHandlerWrapper GetCommandHandler(CommandBarItemDefinition commandDefinition);
    }

    public interface ICommandRerouter
    {
        object GetHandler(CommandBarItemDefinition commandDefinition);
    }
}
