using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Interfaces.Command
{
    public interface ICommandHandler
    {
    }

    public interface ICommandHandler<TCommandDefinition> : ICommandHandler
        where TCommandDefinition : CommandDefinition
    {
        void Update(CommandBase.Command command);
    }
}
