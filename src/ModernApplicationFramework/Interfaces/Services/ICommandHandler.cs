using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Interfaces.Services
{
    public interface ICommandHandler
    {
    }

    public interface ICommandHandler<TCommandDefinition> : ICommandHandler
        where TCommandDefinition : CommandBarItemDefinition
    {
        void Update(Input.Command.Command command);
    }
}
