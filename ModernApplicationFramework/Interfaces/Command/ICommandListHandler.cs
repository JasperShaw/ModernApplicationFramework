using System.Collections.Generic;
using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Interfaces.Command
{
    public interface ICommandListHandler<TCommandDefinition> : ICommandHandler
        where TCommandDefinition : CommandListDefinition
    {
        void Populate(CommandBase.Command command, List<CommandDefinition> commands);
    }
}
